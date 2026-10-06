using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Jing.Data;
using Cysharp.Threading.Tasks;
using static ManagerAI;
using static ManagerAI.OPEN_AI;


public class AIBase
{
    private readonly string common_ai_model = "gpt-4o-mini";
    protected OpenAISchemaGenerator openAISchemaGenerator = new OpenAISchemaGenerator();

    protected Request CreateRequest()
    {
        var request_ai = new Request()
        {
            model = common_ai_model,
            max_completion_tokens = 4000,
        };
        return request_ai;
    }

    protected void SetPrompt(Request request_ai, string prompt)
    {
        if (!string.IsNullOrEmpty(prompt))
        {
            var message = new TextMessage();
            {
                message.role = "developer";

                message.content = prompt;
            }
            request_ai.messages.Add(message);
        }
    }

    protected void SendToOpenAI(Request request_ai, Action<Result> on_success, Action on_error)
    {
        var json = JsonConvert.SerializeObject(request_ai);
        UnityEngine.Debug.Log($"[OpenAI Request]:\n{json}");
        ManagerAI.OPEN_AI.SEND_MESSAGE(
            json,
            (result) =>
            {
                on_success?.Invoke(result);
            },
            () => on_error?.Invoke()
        );
    }
}
