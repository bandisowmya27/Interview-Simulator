using System;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public static class OpenFile
{
    private static int limit_size = 3;

#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")]
    private static extern void TriggerPdfPicker(string objectName, string methodName);

    private class WebGLPickerBridge : MonoBehaviour
    {
        public TaskCompletionSource<(string file_name, byte[] data, string error)> Tcs;

        public void OnPdfSelected(string base64Result)
        {
            if (string.IsNullOrEmpty(base64Result))
            {
                Tcs?.TrySetResult((null, null, "Operation canceled: No file was selected."));
                Destroy(gameObject);
                return;
            }
            
            if (base64Result == "ERROR_SIZE")
            {
                Tcs?.TrySetResult((null, null, $"The file size exceeds the {limit_size}MB limit!"));
                Destroy(gameObject);
                return;
            }

            try
            {
                if (base64Result.StartsWith("data:"))
                {
                    int commaIndex = base64Result.IndexOf(',');
                    if (commaIndex >= 0)
                    {
                        base64Result = base64Result.Substring(commaIndex + 1);
                    }
                }

                base64Result = base64Result.Trim().Replace("\r", "").Replace("\n", "");
                byte[] bytes = Convert.FromBase64String(base64Result);
                string name = Rename();

                Tcs?.TrySetResult((name, bytes, string.Empty));
            }
            catch (Exception e)
            {
                Tcs?.TrySetResult((null, null, $"Resume upload failed: {e.Message}"));
            }
            finally
            {
                Destroy(gameObject);
            }
        }
    }
#endif

    public static async UniTask<(string file_name, byte[] data, string error)> Open()
    {
#if UNITY_EDITOR
        string path = EditorUtility.OpenFilePanel($"Please select a PDF file (Max {limit_size}MB).", "", "pdf");

        if (string.IsNullOrEmpty(path))
        {
            return (null, null, "Operation canceled: No file was selected.");
        }

        try
        {
            byte[] bytes = File.ReadAllBytes(path);

            long maxSizeBytes = limit_size * 1024 * 1024;
            if (bytes.Length > maxSizeBytes)
            {
                return (null, null, $"The file size exceeds the {limit_size}MB limit!");
            }

            string name = Rename();
            return (name, bytes, string.Empty);
        }
        catch (Exception e)
        {
            return (null, null, $"Resume upload failed: {e.Message}");
        }

#elif UNITY_WEBGL
    
        var tcs = new TaskCompletionSource<(string file_name, byte[] data, string error)>();

        GameObject bridgeObject = new GameObject("[WebGLFilePickerBridge]");
        UnityEngine.Object.DontDestroyOnLoad(bridgeObject);
        var bridge = bridgeObject.AddComponent<WebGLPickerBridge>();
        bridge.Tcs = tcs;

        TriggerPdfPicker(bridgeObject.name, "OnPdfSelected");

        return await tcs.Task;

#else
        await Task.Yield();
        return (null, null, "Platform not supported.");
#endif
    }

    private static string Rename()
    {
        string times = DateTime.UtcNow.ToString("yyyyMMdd_HHmm");
        return $"pdf/{ManagerPlayFab.PlayFab_UserName}_{times}.pdf";
    }
}