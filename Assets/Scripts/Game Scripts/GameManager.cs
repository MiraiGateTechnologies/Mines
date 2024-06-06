using System;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public GameObject characterBodyPrefab;
    public GameObject characterBG;
    public CharacterGenerator characterGenerator;
    private static GameManager _instance;
    public MinesManager minesManager;
    public AutoBetManager autoBetManager;
    public int diamondsOpened = 0;
    public static List<GameObject> InstantiatedGridObjects=new List<GameObject>();
    public bool autoBet = false;
    public bool gameStarted=false;
    public int totalMinesCount=5;
    public bool gameOver;
    public bool _gameUIDisableEnable;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////         Singleton Pattern          ////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("GameManager");
                    _instance = singletonObject.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        HandleGameLoss();
        minesManager.ShowAllItems();
    }

    public void HandleGameLoss()
    {

        gameStarted = false;

        UIManager.Instance.autoBetButton.interactable = true;
        UIManager.Instance.ShowStartButton(true);
        UIManager.Instance.ShowCancelButton(false);
        UIManager.Instance.ShowCashOutButton(false);

        UIManager.Instance.EnableMinesCountButtons();
        UIManager.Instance.EnableBetAmntButtons();

        UIManager.Instance.EnableMinesDirectSetButtons();

        ResetMinesTracker();

        BettingManager.Instance.ResetTotalWinnings();
        BettingManager.Instance.UpdateBalanceText();

        BettingManager.Instance.ResetMultipliers();
        UIManager.Instance.ResetMultiplierPanelsToDefault();
        UIManager.Instance.UpdateMultiplierPanels();
    }
    public void ResetMinesTracker()
    {
        diamondsOpened = 0;
    }
}