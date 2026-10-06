using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Jing.Data;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;

namespace Jing.Game.PlayFab
{
    public interface IPlayFabService
    {
        UniTask SaveUploadResume(Data_UploadResume data);
        UniTask<Data_UploadResume> LoadUploadResume();

        UniTask SaveResumeInfo(Data_ResumeInfomation data);
        UniTask<Data_ResumeInfomation> LoadResumeInfo();
    }
    public class PlayFabService : IPlayFabService
    {

        public UniTask SaveUploadResume(Data_UploadResume data)
        {
            var tcs = new UniTaskCompletionSource();
            string info = JsonConvert.SerializeObject(data);
            Debug.Log(info);
            ManagerPlayFab.Try_Update_Player_Data("UploadResume", info, onSuccess, onFailed);
            return tcs.Task;
            void onSuccess()
            {
                Debug.Log("onSuccess");
                tcs.TrySetResult();
            }

            void onFailed(PlayFabError error)
            {
                Debug.Log("onFailed");
                tcs.TrySetResult();
            }
        }
        public UniTask<Data_UploadResume> LoadUploadResume()
        {
            var tcs = new UniTaskCompletionSource<Data_UploadResume>();
            ManagerPlayFab.Try_Get_Player_Data("UploadResume", onSuccess, onFailed);
            return tcs.Task;

            void onSuccess(string info)
            {
                Data_UploadResume data = JsonConvert.DeserializeObject<Data_UploadResume>(info);
                Debug.Log(info);
                tcs.TrySetResult(data);
            }

            void onFailed()
            {
                Debug.Log("onFailed");
                tcs.TrySetResult(new Data_UploadResume());
            }
        }

        public UniTask SaveResumeInfo(Data_ResumeInfomation data)
        {
            var tcs = new UniTaskCompletionSource();
            string info = JsonConvert.SerializeObject(data);
            Debug.Log(info);
            ManagerPlayFab.Try_Update_Player_Data("ResumeInfo", info, onSuccess, onFailed);
            return tcs.Task;
            void onSuccess()
            {
                Debug.Log("onSuccess");
                tcs.TrySetResult();
            }

            void onFailed(PlayFabError error)
            {
                Debug.Log("onFailed");
                tcs.TrySetResult();
            }

        }
        public UniTask<Data_ResumeInfomation> LoadResumeInfo()
        {
            var tcs = new UniTaskCompletionSource<Data_ResumeInfomation>();
            ManagerPlayFab.Try_Get_Player_Data("ResumeInfo", onSuccess, onFailed);
            return tcs.Task;

            void onSuccess(string info)
            {
                Data_ResumeInfomation data = JsonConvert.DeserializeObject<Data_ResumeInfomation>(info);
                Debug.Log(info);
                tcs.TrySetResult(data);
            }

            void onFailed()
            {
                Debug.Log("onFailed");
                tcs.TrySetResult(new Data_ResumeInfomation());
            }

        }

    }
}
