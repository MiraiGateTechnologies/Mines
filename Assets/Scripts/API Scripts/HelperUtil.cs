using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public static class HelperUtil 
{
    #region Serialisation Util

    /// <summary>
    /// Serialize any Object in JSON Format
    /// </summary>
    /// <param name="obj">Object to serialise</param>
    /// <returns>JSON string</returns>
    public static string SerializeToJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    /// <summary>
    /// Deserialize any Object from JSON Format
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static T DeserializeFromJson<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }

    #endregion Serialisation Util

    #region Scene Util

    /// <summary>
    /// Load any scene using this function
    /// </summary>
    /// <param name="sceneName"></param>
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// this function returns active scene name
    /// </summary>
    /// <returns></returns>
    public static string GetActiveSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    #endregion Scene Util

    #region Timer Utils

    /// <summary>
    /// Call this function to delay any action
    /// </summary>
    /// <param name="delay"> time in seconds</param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IEnumerator DelayedAction(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }

    /// <summary>
    /// Call this to repeat coroutines
    /// </summary>
    /// <param name="interval"></param>
    /// <param name="action"></param>
    /// <param name="repeatCount"></param>
    /// <returns></returns>
    public static IEnumerator RepeatCoroutine(float interval, Action action, int repeatCount = -1)
    {
        while (repeatCount != 0)
        {
            action.Invoke();
            yield return new WaitForSeconds(interval);

            if (repeatCount > 0)
                repeatCount--;
        }
    }

    /// <summary>
    /// Call this to delay any function for a particular time
    /// </summary>
    /// <param name="interval"></param>
    /// <param name="function"></param>
    /// <param name="repeatCount"></param>
    /// <returns></returns>
    public static IEnumerator RepeatFunction(float interval, Func<bool> function, int repeatCount = -1)
    {
        while (repeatCount != 0)
        {
            if (function.Invoke() == false)
                break;

            yield return new WaitForSeconds(interval);

            if (repeatCount > 0)
                repeatCount--;
        }
    }

    /// <summary>
    /// On complete of any coroutine any action will be called
    /// </summary>
    /// <param name="coroutine"></param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    public static IEnumerator CoroutineWithCallback(IEnumerator coroutine, Action onComplete)
    {
        yield return coroutine;
        onComplete?.Invoke();
    }

    #endregion Timer Utils

    #region API Util

    /// <summary>
    /// Call this Coroutine to Send request
    /// </summary>
    /// <param name="url"></param>
    /// <param name="method"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    /// <returns></returns>
    public static IEnumerator SendHTTPRequest(string url, string method, Action<string> onSuccess, Action<string> onError = null)
    {
        using (UnityWebRequest www = new UnityWebRequest(url, method))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                onSuccess.Invoke(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"Error: {www.error}");
                onError?.Invoke(www.error);
            }
        }
    }

    /// <summary>
    /// Post request from any API
    /// </summary>
    /// <param name="url"></param>
    /// <param name="form"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    /// <returns></returns>
    public static IEnumerator PostFormRequest(string url, WWWForm form, Action<string> onSuccess, Action<string> onError = null)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                onSuccess.Invoke(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"Error: {www.error}");
                onError?.Invoke(www.error);
            }
        }
    }

    /// <summary>
    /// Call this to Authorize any API url
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    public static void AddAuthorizationHeader(UnityWebRequest request, string token)
    {
        request.SetRequestHeader("Authorization", $"Bearer {token}");
    }
    #endregion API Util

    #region String Utils

    /// <summary>
    /// To Check whether a String is null or not
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(string value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Remove Whitespaces from any string
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string RemoveWhitespace(string input)
    {
        return new string(input.ToCharArray()
            .Where(c => !Char.IsWhiteSpace(c))
            .ToArray());
    }

    #endregion String Utils
}
