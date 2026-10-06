using System.Collections.Generic;
using UnityEngine;

namespace Jing.Data
{
    public class Data_Uesr
    {
        public Data_UserInformation UserInfo;
        public Data_UploadResume ResumeUpload;
        public Data_ResumeInfomation ResumeInfo;

    }

    [System.Serializable]
    public class Data_UserInformation
    {
        public string UserName;
        public string Password;

    }

    [System.Serializable]
    public class Data_UploadResume
    {
        //public string Supebase_URL;     //Resume storage path in Supabase
        public string OpenAI_FileId;    //OpenAI File ID of the uploaded resume
        public string OpenAI_FileName;

    }


    [System.Serializable]
    public struct Data_ResumeInfomation
    {
        public string UserName;
        public string HighestEducation;
        public string YearsOfExperience;
        public List<string> Skills;
        public List<string> ProjectHighlights;

    }


}
