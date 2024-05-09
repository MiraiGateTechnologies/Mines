using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
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
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////   Creates a new character card and initializes it with generated character data    ////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    [ContextMenu("generateNewCharacter")]    
    public void GenerateNewCards(int numberOfCards)
    {
        characterGenerator.usedImageIds.Clear();
        for (int i = 0; i < numberOfCards; i++)
        {
            GameObject newCharacterBody = Instantiate(characterBodyPrefab, characterBG.transform);
            CharacterBodyHolder characterBodyHolder = newCharacterBody.GetComponent<CharacterBodyHolder>();

            characterGenerator.SetPlaceHolders(characterBodyHolder.WomenImage.gameObject);

            /*characterGenerator.GenerateNewCharacter();*/
            characterGenerator.GenerateNewCharacter(characterBodyHolder);

            // Assuming each newCharacterBody GameObject has a SwipeManager component
            SwipeManager swipeManager = newCharacterBody.GetComponent<SwipeManager>();
            if (swipeManager != null)
            {
                // Update the UIManager with the new SwipeManager reference
                UIManager.Instance.SetSwipeManager(swipeManager);
            }
            else
            {
                Debug.LogError("SwipeManager component not found on the newly instantiated character.");
            }
        }
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

        characterGenerator.history.Clear();
       // minesManager.ShowAllItems();

        UIManager.Instance.ShowStartButton(true);
        UIManager.Instance.ShowCashOutButton(false);

        UIManager.Instance.EnableMinesCountButtons();
        UIManager.Instance.EnableBetAmntButtons();

        UIManager.Instance.ResetRiskPercentageDisplay();
        UIManager.Instance.EnableCatfishDirectSetButtons();
        UIManager.Instance.ResetSwipeCountDisplay();

        CatfishManager.Instance.ResetProfileIndex();
        ResetMinesTracker();
       // minesManager.ResetTotalMinesCount();
        BettingManager.Instance.ResetTotalWinnings();
        BettingManager.Instance.UpdateBalanceText();


     /*   PanelFadeOutManager panelFadeOutManager = FindObjectOfType<PanelFadeOutManager>();
        panelFadeOutManager.FadeInOut(UIManager.Instance.gameOverPanel, UIManager.Instance.gameOverText);*/
        // Destroy all the mines object
        //Reset Multipliers
        BettingManager.Instance.ResetMultipliers();
        UIManager.Instance.ResetMultiplierPanelsToDefault();
        UIManager.Instance.UpdateMultiplierPanels();
    }
    public void ResetMinesTracker()
    {
        diamondsOpened = 0;
    }
}