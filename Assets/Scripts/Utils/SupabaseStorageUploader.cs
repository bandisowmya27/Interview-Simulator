using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class SupabaseStorageUploader
{
    private const string supabaseUrl = "https://reaymuosokhwlkwtqixx.supabase.co";  //Project URL
    private const string supabaseAnonKey = "sb_publishable_knH64-hJaq8cIAZJLI_iNw_ArKH238x"; //publishable Key
    private const string buckets_name = "User-Uploaded-Files";
    public static async Task UploadDocument(string file_name, byte[] bytes)
    {
        string url = $"{supabaseUrl}/storage/v1/object/{buckets_name}/{file_name}";

     
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            request.SetRequestHeader("apikey", supabaseAnonKey);
            request.SetRequestHeader("Authorization", $"Bearer {supabaseAnonKey}");
            request.SetRequestHeader("Content-Type", "application/pdf");
            
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield(); 
            }
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Upload To Supabase Successful：" + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"Upload To Supabase Error：{request.error}");
            }
        }
    }
}