// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using static ManagerAI.OPEN_AI;
// using Newtonsoft.Json;
//
//
// ///  Set use_local_system_prompt = true to test prompts locally no GitHub push 
// /// Edit local_system_prompt string to experiment with different AI behaviors
// /// Set use_local_system_prompt = false to use the GitHub ai_role_system.txt file
// ///  Adjust max_history_messages to control conversation memory
// /// Toggle conversation_history_enabled to test with/without conversation memory
// public class ManagerAIChat
// {
//     public static bool page_context_enabled = true;
//     public static bool use_local_system_prompt = true; // Toggle true = use local, false = use GitHub
//     public static bool conversation_history_enabled = true; // Add conversation history
//     public static int max_history_messages = 6; // Limit history to last 6 messages 3 exchanges
//     public static readonly string common_ai_model = "gpt-5-mini";
//     
//     public static bool use_few_shot_examples = true; // Toggle few-shot prompting on/off
//     public static List<Class_Setting.BackgroundData> quiz_background_data = new List<Class_Setting.BackgroundData>();
//     public static List<Message> conversation_history_messages = new();
//     
//     public static void AddMessageHistory(string role, string message)
//     {
//         Message msg = new TextMessage()
//         {
//             role = role,
//             content = message
//         };
//         conversation_history_messages.Add(msg);
//     }
//     // FEW-SHOT EXAMPLES - Example conversations to guide AI behavior
//     // Keep examples SHORT, CONSISTENT, and aligned with system prompt rules
//     // Include "Quiz Context (Private Reference):" format to match real messages
//     public static List<Message> fewShotExamples = new()
//     {
//         // Example 1: Student asks for direct answer → Guide without giving it
//         new TextMessage
//         {
//             role = "user",
//             content =
//                 "What is the answer to question 2?\n\nQuiz Context (Private Reference):\nQuestion 2: What does print() do in Python?"
//         },
//         new TextMessage
//         {
//             role = "assistant",
//             content =
//                 "I can't give the answer directly. What do you think print() does? Have you tried using it in code before?"
//         },
//     
//         // Example 2: Gibberish with sheet context → Ask for clarification, DO NOT assume they chose an option
//         new TextMessage
//         {
//             role = "user",
//             content =
//                 "asdfgh\n\nQuiz Context (Private Reference):\nQuestion 1: Which loop is best when you know the number of iterations?\nA) while\nB) for\nC) do-while"
//         },
//         new TextMessage
//         {
//             role = "assistant",
//             content =
//                 "I didn't understand your message. What would you like help with? Do you have a question about the quiz?"
//         },
//     
//         // Example 3: Random letters → Ask for clarification, ignore sheet context
//         new TextMessage { role = "user", content = "aaabb\n\nQuiz Context (Private Reference):\nQuestion 3: What is a variable?" },
//         new TextMessage
//         {
//             role = "assistant", content = "I'm not sure what you're asking. Could you rephrase your question?"
//         },
//     
//         // Example 4: Off-topic question → Redirect politely
//         new TextMessage
//         {
//             role = "user",
//             content = "What's the weather today?\n\nQuiz Context (Private Reference):\nQuestion 1: Define a function in Python."
//         },
//         new TextMessage
//         {
//             role = "assistant",
//             content = "I'm here to help with your programming quiz. Is there a concept you'd like me to explain?"
//         },
//     
//         // Example 5: Student makes a guess → Encourage verification
//         new TextMessage
//         {
//             role = "user",
//             content =
//                 "I think the answer is B\n\nQuiz Context (Private Reference):\nQuestion 2: Which keyword defines a function?\nA) var\nB) def\nC) func"
//         },
//         new TextMessage
//         {
//             role = "assistant",
//             content = "What makes you think B is correct? Can you explain what 'def' does in Python?"
//         }
//     };
//
//     //  LOCAL SYSTEM PROMPT 
//     public static string local_system_prompt =
//         "You are an educational AI assistant for programming students taking quizzes.\n\n" +
//         "STRICT RULES:\n" +
//         "1. The quiz content may be provided as text, images, or both. Carefully review all available information before responding.\n" +
//         "2. ONLY answer questions related to programming concepts, the quiz, or sheet context\n" +
//         "3. For ANY off-topic question (weather, movies, music, sports, personal questions, etc.), respond: " +
//         "\"I'm here to help with your programming quiz. Is there a concept you'd like me to explain?\"\n" +
//         "4. NEVER give direct answers to quiz questions - guide students to think through it\n" +
//         "5. If the user's message is gibberish or unclear:\n" +
//         "   - DO NOT assume they selected an answer option\n" +
//         "   - DO NOT say 'you chose option X'\n" +
//         "   - Simply ask: \"I didn't understand your message. What would you like help with?\"\n\n" +
//         "6. If quiz content in text or images is unclear or unreadable, ask for clarification instead of guessing.\n\n" +
//
//         "Your role:\n" +
//         "- Help students understand concepts without giving direct answers.\n" +
//         "- Use the sheet context only to support explanations and guidance.\nNever reveal, quote, or reproduce the sheet context.\n" +
//         "- Ask guiding questions to encourage critical thinking.\n" +
//         "- Keep responses concise (under 100 words).\n\n" +
//         "Treat both \"Quiz Context (Private Reference)\" and \"Quiz Background (Developer-Provided Private Reference)\" as private reference material." +
//         "Never reveal, quote, or reproduce either source.";
//
//     public static void Clear_All()
//     {
//         conversation_history_messages.Clear();
//     }
//
//     public static void GET_SUGGESTIONS(
//         Action<Result> on_success,
//         Action on_error)
//     {
//         var message_user_input =
//             "Based on this sheet context, generate a single prompt the user could send in a chat for learn.";
//         List<string> message_lines = new()
//         {
//             message_user_input
//         };
//         // page context
//         {
//             Page page = null;
//             if (page == null)
//                 page = UIScene.Instance.App.Quizzes.Quiz.page_current;
//             if (page == null)
//                 if (UIScene.Instance.App.Quizzes.Menu.ActiveSelf)
//                     page = GameData.Class_Current.Quizzes_Menu;
//             if (page != null)
//             {
//                 message_lines.Add("\nQuiz Context (Private Reference):");
//                 var validElements = (page.elements ?? new List<Element>())
//                     .Where(e =>
//                         e.type == Element.TYPE.GENERAL_TEXT ||
//                         e.type == Element.TYPE.CHALLENGE_MULTIPLE_CHOICE ||
//                         e.type == Element.TYPE.SYSTEM_QUIZ_RESULT ||
//                         e.type == Element.TYPE.SYSTEM_QUIZ_ANSWERS)
//                     .ToList();
//                 for (int i = 0; i < validElements.Count; i++)
//                 {
//                     var element_base = validElements[i];
//                     message_lines.Add($"{element_base.To_Text()}");
//                 }
//             }
//         }
//         var message_user = string.Join("\n", message_lines.Where(s => !string.IsNullOrWhiteSpace(s)));
//         var request_ai = new Request()
//         {
//             //model = "gpt-4-turbo",
//             //model = "gpt-4o-mini-2024-07-18",
//             //model = "gpt-4o-2024-08-06",
//             model = "gpt-5-mini",
//             //model= "gpt-4o-2024-11-20",
//
//             max_completion_tokens = 4000,
//             // temperature removed - gpt-5-mini only supports default 1.0
//             // temperature = 0.7f, //  Added for consistency
//             n = 1
//         };
//         // system
//         //its not useful 
//         {
//             var message_system =
//                 "You are a knowledgeable and patient teacher that generates direct suggestions" +
//                 " to query about the sheet context" +
//                 "Do not include explanations, introductions, or framing language.";
//             var message = new TextMessage();
//             {
//                 message.role = "developer";
//                 message.content = message_system;
//             }
//             request_ai.messages.Add(message);
//         }
//         // user
//         {
//             var message = new TextMessage();
//             {
//                 message.role = "user";
//                 message.content = message_user;
//             }
//             request_ai.messages.Add(message);
//         }
//         var json = JsonUtility.ToJson(request_ai);
//         ManagerAI.OPEN_AI.SEND_MESSAGE(
//             json,
//             (result) => { on_success?.Invoke(result); },
//             () => on_error?.Invoke()
//         );
//     }
//
//     //CHALLENGE_INPUT_NORMAL Use only (first get ai feedback
//     public static void GET_FEEDBACK_INPUT(
//         Action<Result> on_success,
//         Action on_error,
//         bool? verifiedCorrectness = null
//     )
//     {
//         var request_ai = CreateRequest();
//         {   //SystemPrompt
//             var message_system =
//                 "You are a supportive teacher who evaluates student answers based on your knowledge of programming concepts.\n\n" +
//                 "Instructions:\n" +
//                 "1. Carefully review all available question content from text, images, or both.\n" +
//                 "2. Apply your programming knowledge to determine whether the student's answer is correct.\n" +
//                 "3. Focus on conceptual understanding rather than exact phrasing.\n" +
//                 "4. If the answer is correct, acknowledge it is correct\n" +
//                 "5. If the answer is incorrect,just acknowledge it is incorrect\n" +
//                 "6. Keep responses concise and encouraging (under 80 words)\n" +
//                 (verifiedCorrectness.HasValue
//                     ? $"7. The application has already verified that the student's answer is {(verifiedCorrectness.Value ? "CORRECT" : "INCORRECT")}. Do not contradict this evaluation.\n"
//                     : string.Empty) +
//                 "Example responses:\n" +
//                 "- 'That's correct! Well done understanding that concept.'\n" +
//                 "- 'Not quite. What's your thought on this?'\n" +
//                 "- 'That's incorrect. What is your confusion so I can help you?'\n" +
//                 "- 'That's right! Your explanation shows good understanding.'" +
//                 "The question may be provided as:\n" +
//                 "- Text\n" +
//                 "- An image\n" +
//                 "- Both text and image";
//             
//
//             var message = new TextMessage();
//             {
//                 message.role = "developer";
//                 message.content = message_system;
//             }
//             request_ai.messages.Add(message);
//             {
//                 SaveByMessageType text_content = GetPageTextContent();
//                 if (text_content != null)
//                 {
//                     request_ai.messages.Add(CreateMessageFromMessageList("user", text_content));
//                 }
//             }
//             {
//                 SaveByMessageType image_content = GetPageImageContent();
//                 if (image_content != null)
//                 {
//                     request_ai.messages.Add(CreateMessageFromMessageList("user", image_content));
//                 }
//             }
//         }
//         SendToOpenAI(request_ai, on_success, on_error);
//     }
//     public static void SEND_MESSAGE(string message_user_input, Data_SaveUploadFile user_upload_files, Action<Result> on_success, Action on_error)
//     {
//         var request_ai = CreateRequest();
//
//         SetSystemPrompt(request_ai);
//         SetExamples(request_ai);
//         SetConversationHistory(request_ai);
//
//         SetBackgroundInfo(request_ai);
//         {
//             //page : type
//             SaveByMessageType text_content = GetPageTextContent();
//             // page : type = image(image_url)
//             SaveByMessageType image_content = GetPageImageContent();
//             // student mssage
//             SaveByMessageType message_content = new SaveByMessageType();
//             message_content.SaveText("Student Message (Answer this):");
//             message_content.SaveText(message_user_input);
//             
//             
//             List<MessageTypePart> new_one = new List<MessageTypePart>();
//             new_one.AddRange(text_content.GetDataList());
//             new_one.AddRange(image_content.GetDataList());
//             new_one.AddRange(message_content.GetDataList());
//             
//             ListMessage msg =new ListMessage()
//             {
//                 role = "user",
//                 content = new_one
//             };
//             AddMessageHistory("user", message_user_input);
//             request_ai.messages.Add(msg); 
//         }
//         SendToOpenAI(request_ai, on_success, on_error);
//     }
//     
//     public static void SEND_PDF_FILE(byte[] pdfBytes, string fileName, Action<Request_File> on_success, Action on_error)
//     {
//         
//         UPLOAD_PDF( pdfBytes, fileName,
//             (result) =>
//             {
//                 //Add to conversation history
//                 SaveByMessageType type = new SaveByMessageType();
//                 type.SaveFile(result.id);
//                 ListMessage msg = new ListMessage();
//                 msg.role = "user";
//                 msg.content = type.GetDataList();
//                 conversation_history_messages.Add(msg);
//                 
//                 on_success?.Invoke(result);
//             },
//             () => on_error?.Invoke()
//         );
//     }
//     #region ::: Manager Private Methods :::
//     private static SaveByMessageType GetPageTextContent()
//     {
//         Page page = GetPage();
//         if (page == null)
//         {
//             return new SaveByMessageType();
//         }
//         else
//         {
//             var files = new SaveByMessageType();
//             files.SaveText("Quiz Context (Private Reference):");
//             var validElements = (page.elements ?? new List<Element>())
//                 .Where(e =>
//                     e.type == Element.TYPE.GENERAL_TEXT ||
//                     e.type == Element.TYPE.CHALLENGE_MULTIPLE_CHOICE ||
//                     e.type == Element.TYPE.CHALLENGE_INPUT_NORMAL ||
//                     e.type == Element.TYPE.SYSTEM_QUIZ_RESULT ||
//                     e.type == Element.TYPE.SYSTEM_QUIZ_ANSWERS)
//                 .ToList();
//             if (validElements.Count == 0)
//             {
//                 return new SaveByMessageType();
//             }
//             for (int i = 0; i < validElements.Count; i++)
//             {
//                 var element_base = validElements[i];
//                 files.SaveText(element_base.To_Text());
//             }
//             return files;
//         }
//     }
//     private static SaveByMessageType GetPageImageContent()
//     {
//         Page page = GetPage();
//         if (page == null)
//         {
//             return new SaveByMessageType();
//         }
//         else
//         {
//             var files = new SaveByMessageType();
//             files.SaveText("Quiz Context - Image (Private Reference)");
//             var validElements = (page.elements ?? new List<Element>())
//                 .Where(e =>
//                     e.type == Element.TYPE.GENERAL_IMAGE)
//                 .ToList();
//             if (validElements.Count == 0)
//             {
//                 return new SaveByMessageType();
//             }
//             for (int i = 0; i < validElements.Count; i++)
//             {
//                 var element_base = validElements[i];
//                 files.SaveText(element_base.To_Text());
//             }
//             return files;
//         }
//     }
//     
//     private static Page GetPage()
//     {
//         Page page = null;
//         if (page == null)
//         {
//             page = UIScene.Instance.App.Quizzes.Quiz.page_current;
//         }
//
//         if (page == null && UIScene.Instance.App.Quizzes.Menu.ActiveSelf)
//         {
//             page = GameData.Class_Current.Quizzes_Menu;
//         }
//
//         return page;
//     }
//     private static Request CreateRequest()
//     {
//         // var request_ai = new Request()
//         // {
//         //     //model = "gpt-4-turbo",
//         //     //model = "gpt-4o-mini-2024-07-18",
//         //     //model = "gpt-4o-2024-08-06",
//         //     model = "gpt-5-mini",
//         //     //model= "gpt-4o-2024-11-20",
//         //
//         //     max_completion_tokens = 4000,
//         //     //temperature = 0.7f
//         //     reasoning_effort = "low", // "low", "medium", "high" - use "low" to minimize reasoning
//         // };
//         var request_ai = new Request()
//         {
//             model = "gpt-5-mini",
//             max_completion_tokens = 4000,
//             reasoning_effort = "low", // "low", "medium", "high" - use "low" to minimize reasoning
//         };
//         return request_ai;
//     }
//     
//     private static void SetSystemPrompt(Request request_ai)
//     {
//         string message_system = string.Empty;
//         // Use local prompt if enabled, otherwise use GitHub prompt
//         if (use_local_system_prompt)
//         {
//             message_system = local_system_prompt;
//         }
//         else
//         {
//             message_system = PlayerPrefs.GetString("chat_role_system");
//         }
//
//         if (!string.IsNullOrEmpty(message_system))
//         {
//             var message = new TextMessage();
//             {
//                 message.role = "developer";  //ChatGPT told me that the latest official documentation recommends using developer.
//                                              
//                 message.content = message_system;
//             }
//             request_ai.messages.Add(message);
//         }
//     }
//     private static void SetExamples(Request request_ai)
//     {
//         //few-shot examples (after system, before conversation history)
//         if (use_few_shot_examples && fewShotExamples.Count > 0)
//         {
//             foreach (var example in fewShotExamples)
//             {
//                 request_ai.messages.Add(example);
//             }
//         }
//     }
//     private static void SetConversationHistory(Request request_ai)
//     {
//         // conversation history last N messages for context
//         if (conversation_history_messages != null && conversation_history_messages.Count > 0)
//         {
//             var recent_messages = conversation_history_messages.TakeLast(max_history_messages).ToList();
//             foreach (var msg in recent_messages)
//             {
//                 request_ai.messages.Add(msg);
//             }
//         }
//     }
//     private static void SetBackgroundInfo(Request request_ai)
//     {
//         
//         List<Class_Setting.BackgroundData> data = GameData.Class_Current.ClassSetting.Quiz_Background_Info;
//         if (data.Count == 0)
//         {
//             return;
//         }
//
//         var files = new SaveByMessageType();
//         files.SaveText("Quiz Context (Private Reference):");
//         foreach (var i in data)
//         {
//             files.SaveFile(i.OpenAI_FildId);
//         }
//
//         List<MessageTypePart> new_one = new List<MessageTypePart>();
//         new_one.AddRange(files.GetDataList());
//         ListMessage msg =new ListMessage()
//         {
//             role = "user",
//             content = new_one
//         };
//         request_ai.messages.Add(msg);
//     }
//
//     private static Message CreateMessageFromMessageList (string role, SaveByMessageType content)
//     {
//         var textData = new ListMessage();
//         textData.role = role;
//         textData.content = content.GetDataList();
//         return textData;
//     }
//     
//     private static void SendToOpenAI(Request request_ai, Action<Result> on_success, Action on_error)
//     {
//         var json = JsonConvert.SerializeObject(request_ai);
//         ManagerAI.OPEN_AI.SEND_MESSAGE(
//             json,
//             (result) =>
//             {
//                 AddMessageHistory("assistant", result.choices[0].message.content);
//                 on_success?.Invoke(result);
//             },
//             () => on_error?.Invoke()
//         );
//     }
//     #endregion
// }
