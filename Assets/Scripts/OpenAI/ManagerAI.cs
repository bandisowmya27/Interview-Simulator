using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

public class ManagerAI
{
    public static class OPEN_AI
    {
        public static bool is_requesting = false;
        private static event EventHandler<string> on_cancellation_token = null;

        public static void invoke_on_cancellation_token()
            => on_cancellation_token?.Invoke(null, string.Empty);

        public static void Abort_Request()
        {
            invoke_on_cancellation_token();
        }

        public static async void SEND_MESSAGE(
            string json,
            Action<Result> on_success,
            Action on_error)
        {
            var cancellation_token = new CancellationTokenSource();
            EventHandler<string> handler_cancellation_token = (sender, args) => { cancellation_token.Cancel(); };
            on_cancellation_token += handler_cancellation_token;
            is_requesting = true;

            // Start timing
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            byte[] post_data = Encoding.UTF8.GetBytes(json);
            using var request = new UnityWebRequest(
                "https://open-ai-key-gray.vercel.app/api/chat", "POST");
            request.uploadHandler = new UploadHandlerRaw(post_data);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();
            try
            {
                while (!operation.isDone)
                {
                    cancellation_token.Token.ThrowIfCancellationRequested();
                    await Task.Yield();
                }

                // Stop timing
                stopwatch.Stop();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log(
                        $"[AI Response Time] {stopwatch.ElapsedMilliseconds} ms ({stopwatch.Elapsed.TotalSeconds:F2} seconds)");
                    Debug.Log(request.downloadHandler.text);
                    invoke_on_success(JsonConvert.DeserializeObject<Result>(request.downloadHandler.text));
                }
                else
                {

                    Debug.Log($"[AI Response Time] {stopwatch.ElapsedMilliseconds} ms (FAILED)");
                    string responseBody = request.downloadHandler?.text;

                    if (string.IsNullOrEmpty(responseBody))
                    {
                        responseBody = "(伺服器沒有回傳內容)";
                    }

                    Debug.LogError(
                        $"[AI Request Failed]\n" +
                        $"HTTP Status: {request.responseCode}\n" +
                        $"Unity Error: {request.error}\n" +
                        $"Response Body:\n{responseBody}\n" +
                        $"Elapsed: {stopwatch.ElapsedMilliseconds} ms"
                    );

                    invoke_error();
                }
            }
            catch (OperationCanceledException)
            {
                stopwatch.Stop();
                Debug.Log($"[AI Response Time] {stopwatch.ElapsedMilliseconds} ms (CANCELLED)");
                request.Abort();
                invoke_error();
            }
            finally
            {
                on_cancellation_token -= handler_cancellation_token;
            }

            void invoke_on_success(Result result)
            {
                is_requesting = false;
                on_success?.Invoke(result);
            }

            void invoke_error()
            {
                is_requesting = false;
                on_error?.Invoke();
            }
        }

        public static async UniTask<Request_File> UPLOAD_PDF(byte[] pdfBytes, string fileName)
        {
            var cancellation_token = new CancellationTokenSource();
            EventHandler<string> handler_cancellation_token = (sender, args) => { cancellation_token.Cancel(); };
            on_cancellation_token += handler_cancellation_token;

            is_requesting = true;

            var formSections = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("purpose", "user_data"),
                new MultipartFormFileSection("file", pdfBytes, $"{fileName}.pdf", "application/pdf")
            };

            using var request = UnityWebRequest.Post("https://open-ai-key-gray.vercel.app/api/files", formSections);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                await request.SendWebRequest().WithCancellation(cancellation_token.Token);

                stopwatch.Stop();
                Debug.Log($"[AI Response Time] {stopwatch.ElapsedMilliseconds} ms ({stopwatch.Elapsed.TotalSeconds:F2} seconds)");
                Debug.Log(request.downloadHandler.text);

                return JsonConvert.DeserializeObject<Request_File>(request.downloadHandler.text);
            }
            catch (OperationCanceledException)
            {
                stopwatch.Stop();
                Debug.Log($"[AI Response Time] {stopwatch.ElapsedMilliseconds} ms (CANCELLED)");
                return null;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Debug.Log($"[AI Response Time] {stopwatch.ElapsedMilliseconds} ms (FAILED): {ex.Message}");
                return null;
            }
            finally
            {
                is_requesting = false;
                on_cancellation_token -= handler_cancellation_token;
            }
        }

        [Serializable]
        public class Request
        {
            public string model = string.Empty;
            public int max_completion_tokens = 0;
            public List<Message> messages = new();
            public float temperature = 1.0f;
            public int n = 1;
            public ResponseFormat response_format = null;
        }

        [Serializable]
        public class Request_File
        {
            public string @object;
            public string id;
            public string purpose;
            public string filename;
            public int bytes;
            public long created_at;
            public string status;
        }

        [Serializable]
        public class Result
        {
            public Choice[] choices;
        }

        [Serializable]
        public class Choice
        {
            public TextMessage message;
        }

        [Serializable]
        public abstract class Message
        {
            public string role;
        }

        [Serializable]
        public class ResponseFormat
        {
            public string type = "json_schema";
            public Newtonsoft.Json.Linq.JObject json_schema;
        }

        [Serializable]
        public sealed class TextMessage : Message
        {
            public string content { get; set; }
        }

        [Serializable]
        public sealed class ListMessage : Message
        {
            public List<MessageTypePart> content { get; set; }
        }

        [Serializable]
        public class SaveByMessageType
        {
            private List<MessageTypePart> total_data = new List<MessageTypePart>();

            public void SaveText(string text)
            {
                MessageTypePart_Text data = new MessageTypePart_Text();
                data.type = "text";
                data.text = text;
                total_data.Add(data);
            }

            public void SaveImage(string url)
            {
                MessageTypePart_Image.Image img = new MessageTypePart_Image.Image();
                img.url = url;

                MessageTypePart_Image data = new MessageTypePart_Image();
                data.type = "image";
                data.image_url = img;
                total_data.Add(data);
            }

            public void SaveFile(string fileId)
            {
                MessageTypePart_File.File file = new MessageTypePart_File.File();
                file.file_id = fileId;

                MessageTypePart_File data = new MessageTypePart_File();
                data.type = "file";
                data.file = file;
                total_data.Add(data);
            }

            public List<MessageTypePart> GetDataList()
            {
                return total_data;
            }
        }

        [Serializable]
        public abstract class MessageTypePart
        {
            public string type; //file,image_url,text 
        }

        [Serializable]
        public sealed class MessageTypePart_Text : MessageTypePart
        {
            public string text;
        }

        [Serializable]
        public sealed class MessageTypePart_Image : MessageTypePart
        {
            public Image image_url;

            [Serializable]
            public class Image
            {
                public string url;
            }
        }

        [Serializable]
        public sealed class MessageTypePart_File : MessageTypePart
        {
            public File file;

            public class File
            {
                public string file_id;
            }
        }
    }
}