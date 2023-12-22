using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

static class BackendConnection
{
    public static IEnumerator GetAvailableUnits(
        Action<List<UnitDTO>> successCallback
    )
    {
        string url = "http://localhost:4000/users-characters/faker_device/get_units";
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
                    List<UnitDTO> units = JsonConvert.DeserializeObject<List<UnitDTO>>(
                        webRequest.downloadHandler.text
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

    public static IEnumerator SelectUnit(
        Unit unit,
        int slot
    )
    {
        string url = $"http://localhost:4000/users-characters/faker_device/select_unit/{unit.unit_id}";
        string parametersJson = "{\"slot\":\"" + slot + "\"}";
        byte[] byteArray = Encoding.UTF8.GetBytes(parametersJson);
        using (UnityWebRequest webRequest = UnityWebRequest.Put(url, byteArray))
        {
            // webRequest.certificateHandler = new AcceptAllCertificates();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (webRequest.downloadHandler.text.Contains("INEXISTENT_USER"))
                    {
                        // errorCallback?.Invoke(webRequest.downloadHandler.text);
                    }
                    else
                    {
                        // UserCharacterResponse response =
                        //     JsonUtility.FromJson<UserCharacterResponse>(
                        //         webRequest.downloadHandler.text
                        //     );
                        // successCallback?.Invoke(response);
                        Debug.Log(webRequest.downloadHandler.text);
                    }
                    break;
                default:
                    // errorCallback?.Invoke(webRequest.downloadHandler.error);
                    Debug.LogError(webRequest.downloadHandler.error);
                    break;
            }
        }

    }
}

[Serializable]
public class UnitDTO
{
    public string id { get; set; }
    public int? slot { get; set; }
    public int level { get; set; }
    public bool selected { get; set; }
    public string character { get; set; }
}

public class Unit
{
    public string unit_id { get; set; }
    public int level { get; set; }
    public Character character { get; set; }
    public int? slot { get; set; }
    public bool selected { get; set; }
}
