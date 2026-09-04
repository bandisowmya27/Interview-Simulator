// using System;
// using System.Collections.Generic;
// using PlayFab;
// using PlayFab.ClientModels;
// using UnityEngine;
// public class ManagerPlayFab
// {
//   public static string PlayFab_UserName { get; set; }
//   public static void Try_SignUp(string display_name, string username, string password, Action on_signup_success, Action<PlayFabError> on_signup_error)
//   {
//     PlayFab_UserName = username;
//     var request = new RegisterPlayFabUserRequest
//     {
//       Username = username,
//       Password = password,
//       RequireBothUsernameAndEmail = false,
//       DisplayName = display_name,
//     };
//     PlayFabClientAPI.RegisterPlayFabUser(
//       request,
//       (register_result) =>
//       {
//         OnRegisterSuccess(register_result);
//         on_signup_success?.Invoke();
//       },
//       (error) =>
//       {
//         OnRegisterFailure(error);
//         on_signup_error?.Invoke(error);
//       });
//   }
//   public static void Try_LogIn(string username, string password, Action<LoginResult> on_login_success, Action<PlayFabError> on_login_error)
//   {
//     PlayFab_UserName = username;
//     var request = new LoginWithPlayFabRequest
//     {
//       Username = username,
//       Password = password,
//       InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
//       {
//         GetPlayerProfile = true
//       }
//     };
//
//     PlayFabClientAPI.LoginWithPlayFab(
//       request,
//       (result_login) =>
//       {
//         OnLoginSuccess(result_login);
//         Try_Update_Player_Stats();
//         on_login_success?.Invoke(result_login);
//       },
//       (error) =>
//       {
//         OnLoginFailure(error);
//         on_login_error?.Invoke(error);
//       });
//   }
//   public static void Try_Login_Anonymous(string custom_id, Action on_login_success)
//   {
//     var request = new LoginWithCustomIDRequest
//     {
//       CustomId = custom_id,
//       CreateAccount = true,
//       InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
//       {
//         GetPlayerProfile = true
//       }
//     };
//     PlayFabClientAPI.LoginWithCustomID(request, (result_login) =>
//     {
//       OnLoginSuccess(result_login);
//       Try_Update_Player_Stats();
//
//       if (string.IsNullOrEmpty(result_login.InfoResultPayload?.PlayerProfile?.DisplayName))
//         Try_Update_Display_Name(PlayerPrefs.GetString("ANONYMOUS_USER_NAME"), login_completed);
//       else
//         login_completed();
//
//       void login_completed()
//       {
//         on_login_success?.Invoke();
//       }
//     }, OnLoginFailure);
//   }
//   public static void Try_Get_Player_Data(string key, Action<string> on_success, Action on_failure)
//   {
//     var request = new GetUserDataRequest();
//     PlayFabClientAPI.GetUserData(request, result =>
//     {
//       Debug.Log("PlayFab: Get User Data");
//       if (result.Data.ContainsKey(key))
//         on_success?.Invoke(result.Data[key].Value);
//       else
//         on_failure?.Invoke();
//     }, error =>
//     {
//       Debug.LogError("Playfab: OnTryGetPlayerData error " + error.GenerateErrorReport());
//       on_failure?.Invoke();
//     });
//   }
//   public static void Try_Update_Player_Data(string key, string value, Action on_success, Action<PlayFabError> on_failure)
//   {
//     var request = new UpdateUserDataRequest
//     {
//       Data = new Dictionary<string, string> { { key, value } },
//       Permission = UserDataPermission.Public
//     };
//     PlayFabClientAPI.UpdateUserData(request, result =>
//     {
//       Debug.Log("PlayFab: Update User Data");
//       on_success?.Invoke();
//     }, error =>
//     {
//       Debug.LogError("Error al guardar datos del jugador: " + error.GenerateErrorReport());
//       on_failure?.Invoke(error);
//     });
//   }
//   public static void Try_Update_Player_Stats()
//   {
//     var stats = new List<StatisticUpdate>
//     {
//       new StatisticUpdate
//       {
//         StatisticName = "STAT_RQ",
//         Value = 1
//       },
//     };
//     var request = new UpdatePlayerStatisticsRequest
//     {
//       Statistics = stats
//     };
//     PlayFabClientAPI.UpdatePlayerStatistics(request, OnStatisticsUpdated, OnStatisticsError);
//   }
//   public static void Try_Update_Display_Name(string value, Action on_success)
//   {
//     var request = new UpdateUserTitleDisplayNameRequest
//     {
//       DisplayName = value
//     };
//     PlayFabClientAPI.UpdateUserTitleDisplayName(request,
//     (result) =>
//     {
//       OnDisplayNameUpdateSuccess(result);
//       on_success?.Invoke();
//     }, OnDisplayNameUpdateFailure);
//   }
//
//   public static void Try_Get_Title_Data(string key, Action<string> on_success, Action on_failure)
//   {
//     var request = new GetTitleDataRequest
//     {
//       Keys = new List<string> { key }
//     };
//
//     PlayFabClientAPI.GetTitleData(request, 
//       result =>
//       {
//         if (result.Data != null && result.Data.ContainsKey(key))
//         {
//           on_success?.Invoke(result.Data[key]);
//         }
//         else
//         {
//           on_failure?.Invoke();
//         }
//       }, 
//       error =>
//       {
//         on_failure?.Invoke();
//       });
//   }
//   
//   public static void Try_Get_Player_ReadOnly_Data(string key, Action<string> on_success, Action on_failure)
//   {
//     var request = new GetUserDataRequest();
//     PlayFabClientAPI.GetUserReadOnlyData(request, result =>
//     {
//       if (result.Data != null && result.Data.ContainsKey(key))
//         on_success?.Invoke(result.Data[key].Value);
//       else
//         on_failure?.Invoke();
//     }, error =>
//     {
//       on_failure?.Invoke();
//     });
//   }
//
//   private static void OnRegisterSuccess(RegisterPlayFabUserResult result)
//   {
//     Debug.Log("PlayFab: OnRegisterSuccess");
//   }
//   private static void OnRegisterFailure(PlayFabError error)
//   {
//     Debug.Log("PlayFab: OnRegisterFailure");
//     Debug.LogError(error.GenerateErrorReport());
//   }
//   private static void OnLoginSuccess(LoginResult result)
//   {
//     Debug.Log("PlayFab: OnLoginSuccess");
//     Debug.Log($"PlayFab: {result.PlayFabId}");
//     Debug.Log($"PlayFab: {result.EntityToken.Entity.Id}");
//   }
//   private static void OnLoginFailure(PlayFabError error)
//   {
//     Debug.Log("PlayFab: OnLoginFailure");
//     Debug.LogError(error.GenerateErrorReport());
//   }
//   private static void OnDisplayNameUpdateSuccess(UpdateUserTitleDisplayNameResult result)
//   {
//     Debug.Log("PlayFab: Name Updated: " + result.DisplayName);
//   }
//   private static void OnDisplayNameUpdateFailure(PlayFabError error)
//   {
//     Debug.LogError("PlayFab: Name Updated Error: " + error.GenerateErrorReport());
//   }
//   private static void OnStatisticsUpdated(UpdatePlayerStatisticsResult result)
//   {
//     // Debug.Log("Estadísticas actualizadas correctamente.");
//   }
//   private static void OnStatisticsError(PlayFabError error)
//   {
//     // Debug.LogError("Error al actualizar estadísticas: " + error.GenerateErrorReport());
//   }
// }