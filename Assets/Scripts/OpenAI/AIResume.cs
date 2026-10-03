using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ManagerAI.OPEN_AI;
using Newtonsoft.Json;
using Jing.Data;
using Cysharp.Threading.Tasks;

public interface IAIResume
{
    UniTask<Request_File> Send_PDF_File(byte[] pdfBytes, string fileName);

}

public class AIResume : IAIResume
{
    public readonly string common_ai_model = "gpt-5-mini";

    public async UniTask<Request_File> Send_PDF_File(byte[] pdfBytes, string fileName)
    {
        var tcs = new UniTaskCompletionSource<Request_File>();
        await UPLOAD_PDF(pdfBytes, fileName, onSuccess, onFailed);
        return await tcs.Task;

        void onSuccess(Request_File result)
        {
            tcs.TrySetResult(result);
        }
        void onFailed()
        {
            tcs.TrySetResult(null);
        }
    }

    #region ::: AI Setting :::
    private void SetResumePrompt(string resumeText)
    {
        Data_UserResume defaultData = new Data_UserResume();

        string jsonSchemaTemplate = JsonConvert.SerializeObject(defaultData, Formatting.Indented);

        string prompt = $"請分析以下履歷，並嚴格按照以下 JSON 結構回傳，不要包含任何額外文字：\n{jsonSchemaTemplate}\n\n履歷內容：\n{resumeText}";

        Debug.Log(prompt);
    }
    #endregion
    #region ::: Manager Private Methods :::
    private Request CreateRequest()
    {
        var request_ai = new Request()
        {
            model = common_ai_model,
            max_completion_tokens = 4000,
            reasoning_effort = "low", // "low", "medium", "high" - use "low" to minimize reasoning
        };
        return request_ai;
    }


    private void SetResumePrompt(Request request_ai)
    {
        string message_system = string.Empty;


        if (!string.IsNullOrEmpty(message_system))
        {
            var message = new TextMessage();
            {
                message.role = "developer";

                message.content = message_system;
            }
            request_ai.messages.Add(message);
        }
    }

    private void SendToOpenAI(Request request_ai, Action<Result> on_success, Action on_error)
    {
        var json = JsonConvert.SerializeObject(request_ai);
        ManagerAI.OPEN_AI.SEND_MESSAGE(
            json,
            (result) =>
            {
                on_success?.Invoke(result);
            },
            () => on_error?.Invoke()
        );
    }
    #endregion
}
