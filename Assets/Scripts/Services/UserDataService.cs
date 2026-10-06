using System;
using System.Collections.Generic;
using Jing.Data;
using UnityEngine;
using VContainer;

namespace Jing.Game.Services
{
    public class UserDataService : IUserDataService
    {
        #region ::: Action :::
        public event Action Action_DataChanged;
        #endregion
        public Data_Uesr UserData { get; private set; }
        #region ::: Inject :::

        [Inject]
        public virtual void Construct(Data_Uesr data_uesr)
        {
            this.UserData = data_uesr;
        }
        #endregion

        public void UpdateUserInfo(Data_UserInformation newInfo)
        {
            UserData.UserInfo = newInfo;
        }
        public void UpdateResumeContentInfo(Data_ResumeInfomation newInfo)
        {

            UserData.ResumeInfo = newInfo;
            Action_DataChanged?.Invoke();
        }

        public void UpdateResumeUploadInfo(Data_UploadResume newInfo)
        {
            UserData.ResumeUpload = newInfo;
        }
    }
}