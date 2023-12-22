using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

static class BackendConnection
{
    public static IEnumerator GetAvailableUnits(
        Action<List<UserUnit>> successCallback
    )
    {
        string url = "http://localhost:4000/users-characters/theo_device/get_units";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                if (webRequest.downloadHandler.text.Contains("NOT_FOUND"))
                {
                    // errorCallback?.Invoke("USER_NOT_FOUND");
                }
                else
                {
                    string jsonString = "[{\"unit_id\":\"ecde2d4f-d4c6-4fb4-9a32-e9a69b74e4e4\",\"position\":null,\"level\":1,\"selected\":true,\"character\":\"muflus\"},{\"unit_id\":\"ecde2d4f-d4c6-4fb4-9a32-e9a69b74e4e5\",\"position\":null,\"level\":8,\"selected\":false,\"character\":\"uMA\"}]";
                    // List<UserUnit> units = JsonConvert.DeserializeObject<List<UserUnit>>(
                    //     webRequest.downloadHandler.text
                    // );
                    List<UserUnit> units = JsonConvert.DeserializeObject<List<UserUnit>>(
                        jsonString
                    );
                    successCallback?.Invoke(units);
                }
                webRequest.Dispose();
            }
            else
            {
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("Something unexpected happened");
                        break;
                    case UnityWebRequest.Result.ConnectionError:
                        Debug.LogError("Connection Error");
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Data processing error.");
                        break;
                    default:
                        Debug.LogError("Unhandled error.");
                        break;
                }
            }
        }
    }
}

[Serializable]
public class UserUnit
{
    public string unit_id { get; set; }
    public int? position { get; set; }
    public int level { get; set; }
    public bool selected { get; set; }
    public string character { get; set; }
}
