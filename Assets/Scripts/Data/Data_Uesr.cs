using UnityEngine;

namespace Jing.Data
{
    public class Data_Uesr
    {
        public Data_UserResume Resume;

    }

    public class Data_UserResume
    {
        public string Resume_Name;
        public string Supebase_URL;     //Resume storage path in Supabase
        public string OpenAI_FileId;    //OpenAI File ID of the uploaded resume

    }
}
