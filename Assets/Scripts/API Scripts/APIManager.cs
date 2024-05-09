using System.Collections;
using UnityEngine;

public class APIManager : MonoBehaviour
{
    public static APIManager Instance { get; private set; }

    private AuthControlApiManager authControlApiManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize all API managers here
            authControlApiManager = new AuthControlApiManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SendAuthControlRequest()
    {
        authControlApiManager.SendRequest(this, OnSuccess, OnFailure);
    }

    private void OnSuccess(string response)
    {
        Debug.Log($"Success: {response}");
        // Handle success response
    }

    private void OnFailure(string error)
    {
        Debug.LogError($"Error: {error}");
        // Handle failure response
    }
    public void onAPITestBtnClick()
    {
        StartCoroutine(APIauthCheck());
    }
    IEnumerator APIauthCheck()
    {
        // The URL for your login API
        string url = "https://13.235.114.49:3000/api/v1/auth/login";

        // Prepare the form data with username and password
        WWWForm form = new WWWForm();
        form.AddField("username", "your_username");
        form.AddField("password", "your_password");
        yield return new WaitForSeconds(0.1f);

        // Start the coroutine to call the PostFormRequest function
        StartCoroutine(HelperUtil.PostFormRequest(url, form,
            OnSuccess =>
            {
                Debug.Log("Success");
            },
            OnFailure=>
            {
                Debug.Log("Error");
            }
            ));
    }
}
