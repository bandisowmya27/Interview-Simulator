using System.Runtime.InteropServices;
using UnityEngine;
using System;

public class WebMicInput : MonoBehaviour
{
    
    [DllImport("__Internal")]
    private static extern void StartSpeech();
    
    public Action<string> Mic_Result;
    /// <summary>
    /// Button Click
    /// </summary>
    public void OnStartVoiceInput()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        StartSpeech();
#endif
    }

    /// <summary>
    /// Speech Successful (Web send Msg to this)
    /// </summary>
    public void OnSpeechResult(string recognizedText)
    {
        Mic_Result?.Invoke(recognizedText);
    }
    
    /// <summary>
    /// Speech Error (Web send Msg to this)
    /// </summary>
    public void OnSpeechError(string errorMessage)
    {
        Debug.LogError("error" + errorMessage);
    }
    
}