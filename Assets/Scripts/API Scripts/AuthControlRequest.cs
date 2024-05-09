using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections;

[Serializable]
public class AuthControlRequest
{
    public string partnerKey;
    public string userId;
    public string userDisplayName;
    public string gameCode;
    public int balance;
}

public class AuthControlApiManager
{
    private string apiUrl = "http://13.235.114.49:3000/api/v1/auth/login";

    public IEnumerator SendRequest(MonoBehaviour monoBehaviour, Action<string> onSuccess, Action<string> onFailure)
    {
        AuthControlRequest request = new AuthControlRequest
        {
            partnerKey = "your_partner_key",
            userId = "your_user_id",
            userDisplayName = "your_user_display_name",
            gameCode = "your_game_code",
            balance = 0 // Set this to the appropriate value
        };

        string jsonData = JsonUtility.ToJson(request);
        UnityWebRequest webRequest = UnityWebRequest.Put(apiUrl, jsonData);
        webRequest.method = UnityWebRequest.kHttpVerbPOST;
        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return monoBehaviour.StartCoroutine(SendWebRequest(webRequest, onSuccess, onFailure));
    }

    private IEnumerator SendWebRequest(UnityWebRequest webRequest, Action<string> onSuccess, Action<string> onFailure)
    {
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            onFailure?.Invoke(webRequest.error);
        }
        else
        {
            onSuccess?.Invoke(webRequest.downloadHandler.text);
        }
    }
}
