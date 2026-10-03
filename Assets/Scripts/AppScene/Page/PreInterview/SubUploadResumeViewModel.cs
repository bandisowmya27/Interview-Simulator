using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Jing.Data;
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
        private Data_Uesr data_uesr;
        private IAIResume aiResume;


        [Inject]
        public void Construct(Data_Uesr data_uesr, IAIResume aiResume)
        {
            this.data_uesr = data_uesr;
            this.aiResume = aiResume;
        }

        #endregion

        #region ::: Public Methods :::
        public void InitGet()
        {
            FileId = PlayerPrefs.GetString("OpenAI_FileId");
            FileName = PlayerPrefs.GetString("OpenAI_FileName");

        }
        public async UniTask UploadResume()
        {
            (byte[] bytes, string error) = await OpenFile.Open();
            if (!ConfirmOpenFile(error)) { return; }

            string times = DateTime.UtcNow.ToString("yyyyMMdd_HHmm");
            string user_name = data_uesr?.Info?.UserName ?? "Guest";
            string file_name = $"{user_name}_{times}";

            Request_File result = await aiResume.Send_PDF_File(bytes, file_name);
            if (!ConfirmUploadToOpenAI(result)) { return; }
            string file_id = result.id;
            data_uesr.Resume.OpenAI_FileId = file_id;
            //test use

            FileId = PlayerPrefs.GetString("OpenAI_FileId");
            FileName = PlayerPrefs.GetString("OpenAI_FileName");

            Action_UpdateFileName?.Invoke(FileName);
            AnalyResume();
        }

        public async UniTask RemoveResume()
        {

        }
        #endregion

        #region ::: Private Methods :::
        private void AnalyResume()
        {

        }
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
            return true;
        }

        #endregion

    }
}

