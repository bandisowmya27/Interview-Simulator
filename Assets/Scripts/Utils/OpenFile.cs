// using System;
// using System.IO;
// using System.Threading.Tasks;
// using UnityEngine;
//
// #if UNITY_EDITOR
// using UnityEditor;
// #endif
//
// #if UNITY_WEBGL && !UNITY_EDITOR
// using System.Runtime.InteropServices;
// #endif
//
// public static class OpenFile
// {
//     private static int limit_size = 3;
//
// #if UNITY_WEBGL && !UNITY_EDITOR
//
//     [DllImport("__Internal")]
//     private static extern void TriggerPdfPicker(string objectName, string methodName);
//
//     private class WebGLPickerBridge : MonoBehaviour
//     {
//         public TaskCompletionSource<(string file_name, byte[] data)> Tcs;
//
//         public void OnPdfSelected(string base64Result)
//         {
//             if (string.IsNullOrEmpty(base64Result) || base64Result == "ERROR_SIZE")
//             {
//                 Tcs?.TrySetResult((null, null));
//                 Destroy(gameObject);
//                 return;
//             }
//
//             try
//             {
//                 if (base64Result.StartsWith("data:"))
//                 {
//                     int commaIndex = base64Result.IndexOf(',');
//                     if (commaIndex >= 0)
//                     {
//                         base64Result = base64Result.Substring(commaIndex + 1);
//                     }
//                 }
//
//                 base64Result = base64Result.Trim().Replace("\r", "").Replace("\n", "");
//                 byte[] bytes = Convert.FromBase64String(base64Result);
//
//                 if (bytes.Length > limit_size * 1024 * 1024)
//                 {
//                     Tcs?.TrySetResult((null, null));
//                     Destroy(gameObject);
//                     return;
//                 }
//
//                 string name = Rename();
//
//                 Tcs?.TrySetResult((name, bytes));
//             }
//             catch (Exception e)
//             {
//                 Tcs?.TrySetResult((null, null));
//             }
//             finally
//             {
//                 Destroy(gameObject);
//             }
//         }
//     }
// #endif
//     
//     public static async Task<(string file_name, byte[] data)> Open()
//     {
// #if UNITY_EDITOR
//         string path = EditorUtility.OpenFilePanel($"Please select a PDF file (Max {limit_size}MB).", "", "pdf");
//         if (string.IsNullOrEmpty(path))
//         {
//             return (null, null);
//         }
//
//         byte[] bytes = File.ReadAllBytes(path);
//         long maxSizeBytes = limit_size * 1024 * 1024;
//
//         if (bytes.Length > maxSizeBytes)
//         {
//             Debug.LogWarning($"The file size exceeds the {limit_size}MB limit!");
//             return (null, null);
//         }
//
//         string name = Rename();
//
//         return (name, bytes);
//
// #elif UNITY_WEBGL
//     
//         var tcs = new TaskCompletionSource<(string file_name, byte[] data)>();
//
//         GameObject bridgeObject = new GameObject("[WebGLFilePickerBridge]");
//         UnityEngine.Object.DontDestroyOnLoad(bridgeObject);
//         var bridge = bridgeObject.AddComponent<WebGLPickerBridge>();
//         bridge.Tcs = tcs;
//
//         TriggerPdfPicker(bridgeObject.name, "OnPdfSelected");
//
//         return await tcs.Task;
//
// #else
//         await Task.Yield();
//         return (null, null);
// #endif
//     }
//
//     private static string Rename()
//     {
//         string times = DateTime.UtcNow.ToString("yyyyMMdd_HHmm");
//         return  $"pdf/{ManagerPlayFab.PlayFab_UserName}_{times}.pdf";
//     }
// }