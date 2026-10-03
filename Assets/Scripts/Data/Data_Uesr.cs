using UnityEngine;

namespace Jing.Data
{
    public class Data_Uesr
    {
        public Data_UserInformation Info;
        public Data_UserResume Resume;

    }

    public class Data_UserResume
    {
        //public string Supebase_URL;     //Resume storage path in Supabase
        public string OpenAI_FileId;    //OpenAI File ID of the uploaded resume
        public string OpenAI_FileName;

        public string Resume_Name;

    }


    public class Data_UserInformation
    {
        public string UserName;
        public string Password;

    }
}
