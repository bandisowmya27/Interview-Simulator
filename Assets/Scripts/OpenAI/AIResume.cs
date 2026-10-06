using static ManagerAI.OPEN_AI;

using Jing.Data;
using Cysharp.Threading.Tasks;
using VContainer;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Jing.Game.Services;


public interface IAIResume
{
    UniTask<Request_File> Send_PDF_File(byte[] pdfBytes, string fileName);
    UniTask<Data_ResumeInfomation> RequestOpenAIResumeAnalysis();
}

public class AIResume : AIBase, IAIResume
{
    #region ::: Inject :::
    private IUserDataService userDataService;

    [Inject]
    public void Construct(IUserDataService userDataService)
    {
        this.userDataService = userDataService;
    }

    #endregion
    private string file_id = string.Empty;
    public async UniTask<Request_File> Send_PDF_File(byte[] pdfBytes, string fileName)
    {
        return await UPLOAD_PDF(pdfBytes, fileName);
    }

    #region ::: AI Setting :::
    public UniTask<Data_ResumeInfomation> RequestOpenAIResumeAnalysis()
    {
        var tcs = new UniTaskCompletionSource<Data_ResumeInfomation>();
        Request ai_request = CreateRequest();
        SetPrompt(ai_request,
        "You are a professional resume parsing assistant. Please extract information from the provided file and return it strictly in the specified format.");

        SaveByMessageType data = new SaveByMessageType();
        data.SaveText("Please parse the following uploaded resume file.");
        data.SaveFile(userDataService.UserData.ResumeUpload.OpenAI_FileId);

        var textData = new ListMessage();
        textData.role = "user";
        textData.content = data.GetDataList();
        ai_request.messages.Add(textData);

        ResponseFormat response_format = new ResponseFormat();
        JObject schema = openAISchemaGenerator.GenerateJsonSchema<Data_ResumeInfomation>();
        ai_request.response_format = new ResponseFormat()
        {
            type = "json_schema",
            json_schema = schema
        };

        UnityEngine.Debug.Log(ai_request);
        SendToOpenAI(ai_request, on_success, on_error);
        return tcs.Task;

        void on_success(Result result)
        {
            UnityEngine.Debug.Log(result);
            Data_ResumeInfomation data = JsonConvert.DeserializeObject<Data_ResumeInfomation>(result.choices[0].message.content);
            userDataService.UpdateResumeContentInfo(data);
            tcs.TrySetResult(data);
        }

        void on_error()
        {
            UnityEngine.Debug.Log("RequestOpenAIResumeAnalysis Error");
            tcs.TrySetResult(new Data_ResumeInfomation());
        }
    }

    #endregion

}
