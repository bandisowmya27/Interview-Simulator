using System;
using Jing.Data;


namespace Jing.Game.Services
{
    public interface IUserDataService
    {
        event Action Action_DataChanged;
        Data_Uesr UserData { get; }

        void UpdateUserInfo(Data_UserInformation newInfo);
        void UpdateResumeContentInfo(Data_ResumeInfomation newInfo);
        void UpdateResumeUploadInfo(Data_UploadResume newInfo);
    }
}