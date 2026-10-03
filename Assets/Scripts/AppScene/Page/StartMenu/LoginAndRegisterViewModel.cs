using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Jing.Data;
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
        private Data_Uesr data_uesr;

        [Inject]
        public virtual void Construct(Data_Uesr data_uesr)
        {
            this.data_uesr = data_uesr;
        }
        #endregion

        #region ::: Public Methods :::
        public async UniTask<bool> Register(string account, string password)
        {
            var tcs = new UniTaskCompletionSource<bool>();
            ManagerPlayFab.Try_SignUp(account, account, password, onSuccess, onFailed);
            return await tcs.Task;

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

        public async UniTask<bool> Login(string account, string password)
        {
            var tcs = new UniTaskCompletionSource<bool>();
            ManagerPlayFab.Try_LogIn(account, password, onSuccess, onFailed);
            return await tcs.Task;

            void onSuccess(LoginResult result)
            {
                Debug.Log("Login onSuccess");
                AfterLogin_InitData(account, password);
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
        private void AfterLogin_InitData(string account, string password)
        {
            data_uesr.Info = new Data_UserInformation()
            {
                UserName = account,
                Password = password
            };
            data_uesr.Resume = new Data_UserResume()
            {
                OpenAI_FileId = PlayerPrefs.GetString("OpenAI_FileId"),
                OpenAI_FileName = PlayerPrefs.GetString("OpenAI_FileName")
            };

        }
        #endregion

    }
}
