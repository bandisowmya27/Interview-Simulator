using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Jing.Data;
using Jing.Game.PlayFab;
using Jing.Game.Services;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using VContainer;

namespace Jing.LoginAndRegister
{
    public class LoginAndRegisterViewModel
    {
        #region ::: Action :::
        public event Action<string> Action_ErrorMsg;
        #endregion

        #region ::: Inject :::
        private IUserDataService userDataService;
        private IPlayFabService playFabService;
        [Inject]
        public virtual void Construct(IUserDataService userDataService, IPlayFabService playFabService)
        {
            this.userDataService = userDataService;
            this.playFabService = playFabService;
        }
        #endregion

        #region ::: Public Methods :::
        public UniTask<bool> Register(string account, string password)
        {
            var tcs = new UniTaskCompletionSource<bool>();
            ManagerPlayFab.Try_SignUp(account, account, password, onSuccess, onFailed);
            return tcs.Task;

            void onSuccess()
            {
                tcs.TrySetResult(true);
            }
            void onFailed(PlayFabError result)
            {
                Action_ErrorMsg?.Invoke($"Error: {result.ErrorMessage}");
                tcs.TrySetResult(false);
            }
        }

        public UniTask<bool> Login(string account, string password)
        {
            var tcs = new UniTaskCompletionSource<bool>();
            ManagerPlayFab.Try_LogIn(account, password, onSuccess, onFailed);
            return tcs.Task;

            async void onSuccess(LoginResult result)
            {
                await AfterLogin_InitData(account, password);
                tcs.TrySetResult(true);
            }
            void onFailed(PlayFabError result)
            {
                Debug.Log("Login onFailed");
                Action_ErrorMsg?.Invoke($"Error: {result.ErrorMessage}");
                tcs.TrySetResult(false);
            }
        }
        #endregion
        #region ::: Private Methods :::
        private async UniTask AfterLogin_InitData(string account, string password)
        {
            userDataService.UpdateUserInfo(new Data_UserInformation()
            {
                UserName = account,
                Password = password
            });

            Data_UploadResume info_upload = await playFabService.LoadUploadResume();
            userDataService.UpdateResumeUploadInfo(info_upload);

            Data_ResumeInfomation info_resume = await playFabService.LoadResumeInfo();
            userDataService.UpdateResumeContentInfo(info_resume);
        }
        #endregion

    }
}
