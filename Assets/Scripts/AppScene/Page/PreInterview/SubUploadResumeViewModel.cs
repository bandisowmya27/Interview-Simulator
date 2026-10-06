using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Jing.Data;
using Jing.Game.PlayFab;
using Jing.Game.Services;
using UnityEditor.Rendering;
using UnityEngine;
using VContainer;
using static ManagerAI.OPEN_AI;

namespace Jing.Game.PreInterview
{
    public class SubUploadResumeViewModel
    {

        #region ::: Action :::
        public event Action<string> Action_OpenPopup;
        public event Action<string> Action_UpdateFileName;
        #endregion

        #region ;;; Property :::
        public string FileId { get; private set; }
        public string FileName { get; private set; }

        #endregion
        #region ::: Inject :::
        private IUserDataService userDataService;
        private IAIResume aiResume;
        private IPlayFabService playFabService;


        [Inject]
        public void Construct(IUserDataService userDataService, IAIResume aiResume, IPlayFabService playFabService)
        {
            this.userDataService = userDataService;
            this.aiResume = aiResume;
            this.playFabService = playFabService;

        }

        #endregion

        #region ::: Public Methods :::
        public void InitGet()
        {
            FileId = userDataService.UserData?.ResumeUpload?.OpenAI_FileId ?? string.Empty;
            FileName = userDataService.UserData?.ResumeUpload?.OpenAI_FileName ?? string.Empty;

        }
        public async UniTask UploadResume()
        {
            (byte[] bytes, string error) = await OpenFile.Open();
            if (!ConfirmOpenFile(error)) { return; }

            string times = DateTime.UtcNow.ToString("yyyyMMdd_HHmm");
            string user_name = userDataService?.UserData?.UserInfo?.UserName ?? "Guest";
            string file_name = $"{user_name}_{times}";

            Request_File result = await aiResume.Send_PDF_File(bytes, file_name);
            if (!ConfirmUploadToOpenAI(result)) { return; }
            Action_UpdateFileName?.Invoke(FileName);
        }

        public async UniTask AnalyResume()
        {
            Data_ResumeInfomation data = await aiResume.RequestOpenAIResumeAnalysis();
            userDataService.UpdateResumeContentInfo(data);
            await playFabService.SaveResumeInfo(data);
        }

        #endregion

        #region ::: Private Methods :::
        #endregion
        #region ::: Private Methods - Open File :::
        bool ConfirmOpenFile(string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                if (error != "Operation canceled: No file was selected.")
                {
                    Action_OpenPopup?.Invoke(error);
                }
                return false;
            }
            return true;
        }
        #endregion
        #region ::: Private Methods - Upload To OpenAI :::
        private bool ConfirmUploadToOpenAI(Request_File result)
        {
            if (result is null || string.IsNullOrEmpty(result.id))
            {
                Action_OpenPopup?.Invoke("Error uploading to OpenAI!");
                return false;
            }
            FileId = result.id;
            FileName = result.filename;
            Data_UploadResume info = new Data_UploadResume()
            {
                OpenAI_FileId = FileId,
                OpenAI_FileName = FileName
            };
            userDataService.UpdateResumeUploadInfo(info);
            playFabService.SaveUploadResume(info);

            return true;
        }

        #endregion

    }
}

