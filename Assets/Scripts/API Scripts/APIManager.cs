using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public static APIManager Instance { get; private set; }
    [SerializeField] UIManager uiManager;
    private string Accesstoken = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJyb290IiwiaWF0IjoxNzE0OTgwNDQ5LCJleHAiOjE3MTc1NzI0NDl9.lxmcpNdKML3vXeEvOg-gVj3nxpDCkgsEqflwe4ITl4g";
    public List<minesDetail> minesDetails = new List<minesDetail>();
    public bool betStatus = true;
    public float requestTimeout = 10f; // Timeout in seconds
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);

        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
       // betInsert(20, 3);
    }

    void Update()
    {


    }


    #region _BetInsert_Api

    public void betInsert(float amount, int mines)
    {
        StartCoroutine(_BetInsert(amount, mines));
        Debug.Log(amount + " " + mines);
    }

    public IEnumerator _BetInsert(float amount, int mines)
    {
        JSONObject jsonForm = new JSONObject();
        jsonForm.Add("amount", amount);
        jsonForm.Add("mines", mines);

        //UnityWebRequest www = UnityWebRequest.PostWwwForm(LinkReferences.BetInsert_link + LinkReferences.BetInsert_link, jsonForm.ToString());
        UnityWebRequest www = UnityWebRequest.PostWwwForm("https://miraigames.org/api/v1/mine/insertBet", jsonForm.ToString());
        www.method = UnityWebRequest.kHttpVerbPOST;
       // www.timeout = (int)requestTimeout; // Set the timeout

        UploadHandler up = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonForm.ToString()));
        up.contentType = "application/json";
        www.SetRequestHeader("Authorization", "Bearer " + Accesstoken);
        www.uploadHandler = up;
        yield return www.SendWebRequest();
        if (www.error != null)
        {
            if (www.error == "Request timeout")
            {
                Debug.LogError("Your API BETInsert request is timeout");
            }
            else
            {
                Debug.Log("Error: " + www.error);
            }
        }
        else
        {
            if (www.responseCode == 200)
            {
                JSONObject jo = JSONObject.Parse(www.downloadHandler.text) as JSONObject;
                if (jo["responseStatus"]["status"] == false)
                {

                    Debug.Log("Bet is not placed......");
                }
                else
                {
                    JSONArray ja = jo["mines"] as JSONArray;
                    Debug.Log(www.downloadHandler.text);
                    minesDetails.Clear();
                    for (int i = 0; i < ja.Count; i++)
                    {
                        minesDetail item = new minesDetail(
                            ja[i]
                            );
                        minesDetails.Add(item);
                        betStatus = true;


                    }
                   
                }
                uiManager.OnStartButtonPressed();
                PlayerPrefs.SetString("id", jo["id"]);
                Debug.Log(PlayerPrefs.GetString("id"));
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }



    #endregion

    #region OpenMines_API
    // This swipe card api hit on every swiping 
    public IEnumerator OpenMines(string minesId, string Id)
    {
        JSONObject jsonForm = new JSONObject();
        jsonForm.Add("mineId", minesId);
        jsonForm.Add("id", Id);

        UnityWebRequest www = UnityWebRequest.PostWwwForm(LinkReferences.BASE_API + LinkReferences.openMine_link, jsonForm.ToString());
        UploadHandler up = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonForm.ToString()));
        www.method = UnityWebRequest.kHttpVerbPOST;
        up.contentType = "application/json";
        www.SetRequestHeader("Authorization", "Bearer " + Accesstoken);
        www.uploadHandler = up;
        yield return www.SendWebRequest();

        if (www.error != null)
        {
            Debug.LogError(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {

                JSONObject js = JSONObject.Parse(www.downloadHandler.text) as JSONObject;
                Debug.Log(www.downloadHandler.text);
                

            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    #endregion

    #region CollectMine_API

    // This swipe card api hit on every swiping 
    public IEnumerator CollectMine(string Id)
    {
        JSONObject jsonForm = new JSONObject();

        UnityWebRequest www = UnityWebRequest.PostWwwForm(LinkReferences.BASE_API + LinkReferences.collectMine_link + "/" + Id, jsonForm.ToString());
        UploadHandler up = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonForm.ToString()));
        www.method = UnityWebRequest.kHttpVerbPOST;
        up.contentType = "application/json";
        www.SetRequestHeader("Authorization", "Bearer " + Accesstoken);
        www.uploadHandler = up;
        yield return www.SendWebRequest();

        if (www.error != null)
        {
            Debug.LogError(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {

                JSONObject js = JSONObject.Parse(www.downloadHandler.text) as JSONObject;
                Debug.Log(www.downloadHandler.text);

            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    #endregion
}
[System.Serializable] // added just to show in inspector.
public class minesDetail
{
    public string minesId;

    public minesDetail(string minesId)
    {

        this.minesId = minesId;

    }
}