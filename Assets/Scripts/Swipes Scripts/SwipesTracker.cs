using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwipesTracker : MonoBehaviour
{
    public static SwipesTracker Instance { get; private set; }
    public BettingManager bettingManager;
    private bool gameStarted = false;
    public int TotalSwipes { get; private set; } = 0;// Total number of swipes
    public int LeftSwipes { get; private set; } = 0;// Count of left swipes
    public int RightSwipes { get; private set; } = 0;// Count of right swipes
    
    public float multiplierIncrement = 0.08f;
    public CharacterGenerator characterGenerator;

    public List<string> swipeDecisions = new List<string>();
    public void ClearSwipeDecisions()
    {
        swipeDecisions.Clear();
    }

    public List<string> catfishDecisions = new List<string>();
    public List<string> namesHistory = new List<string>();
    public List<string> descriptionHistory = new List<string>();

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////            Initialization           ////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////   Track swipe actions and updates game state accordingly   ////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void AddSwipe(bool isRightSwipe)
    {
        // Update the swipe count display each time a swipe is added
        

        if (!gameStarted)
        {
            gameStarted = true;
            bettingManager.balanceAmount -= bettingManager.betAmount;
            bettingManager.UpdateBalanceText();
     /*       bettingManager.ResetTotalWinnings();*/
        }

        TotalSwipes++;
        UIManager.Instance.UpdateSwipeCountDisplay(TotalSwipes, 25);

        

        if (CatfishManager.Instance.IsCurrentProfileCatfish())
        {
            catfishDecisions.Add("Was a Catfish");
        }
        else
        {
            catfishDecisions.Add("Was not a Catfish");
        }


        if (isRightSwipe)
        {
            RightSwipes++;
            swipeDecisions.Add("Right Swiped");

            Debug.Log($"Swipe Right: Total Swipes = {TotalSwipes}, Right Swipes = {RightSwipes}, Left Swipes = {LeftSwipes}");


            UIManager.Instance.currentMultiplierIndex++;
            UIManager.Instance.CheckAndAdjustMultiplierPanels();

            //UIManager.Instance.HighlighMultiplierPanel(RightSwipes - 1);
            UIManager.Instance.HighlightMultiplierPanel(UIManager.Instance.currentMultiplierIndex);

            // call HandleSwipeRightOnNonCatfish only if it's not a catfish profile
            if (!CatfishManager.Instance.IsCurrentProfileCatfish())
            {
                //HandleSwipeRightOnNonCatfish();
            }
          //  CheckForCatfishProfile(isRightSwipe);
        }
        else
        {
            LeftSwipes++;
            swipeDecisions.Add("Left Swiped");

            Debug.Log($"Swipe Left: Total Swipes = {TotalSwipes}, Right Swipes = {RightSwipes}, Left Swipes = {LeftSwipes}");
        }

        CatfishManager.Instance.NextProfile();

        /*string currentCharacterName = characterGenerator.GetCurrentCharacterName();
        namesHistory.Add(currentCharacterName);

        string currentCharacterDescription = characterGenerator.GetCurrentCharacterDescription();
        descriptionHistory.Add(currentCharacterDescription);*/
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////      Check if the swiped profile is a catfish and handle game loss         ////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private void CheckForCatfishProfile(bool isRightSwipe)
    {
        if (isRightSwipe && CatfishManager.Instance.IsCurrentProfileCatfish())
        {
            Debug.Log("Swiped right on a catfish profile.");
         //   HandleGameLoss();
        }
    }
    public void HandleGameLoss()
    {
        UIManager.Instance.likeButtonGameObj.SetActive(false);
        UIManager.Instance.dislikeButtonGameObj.SetActive(false);
        UIManager.Instance.ShowHistoryPanel();

        UIManager.Instance.DisableLikeDislikeButtons();

        bettingManager.betAmountInput.readOnly = false; // Re-enable typing in the bet amount input field

        bettingManager.StartGame();

        swipeDecisions.Clear();
        catfishDecisions.Clear();
        namesHistory.Clear();
        descriptionHistory.Clear();
        characterGenerator.history.Clear();

        ResetGameStartedFlag();

        UIManager.Instance.ShowStartButton(true);
        UIManager.Instance.ShowCashOutButton(false);

        UIManager.Instance.EnableMinesCountButtons();
        UIManager.Instance.EnableBetAmntButtons();

        UIManager.Instance.ResetRiskPercentageDisplay();
        UIManager.Instance.EnableCatfishDirectSetButtons();
        UIManager.Instance.ResetSwipeCountDisplay();

        CatfishManager.Instance.ResetProfileIndex();
        ResetTracker();/*
        bettingManager.ResetTotalWinnings();*/
        bettingManager.UpdateBalanceText();

        //Reset Like/Dislike button pop-up
        UIManager.Instance.ChangeDislikeButtonSpriteAndSize(UIManager.Instance.defaultDislikeButtonSprite, new Vector2(200, 200));
        UIManager.Instance.ChangeLikeButtonSpriteAndSize(UIManager.Instance.defaultLikeButtonSprite, new Vector2(200, 200));

        GameOverPanelManager gameOverPanel = FindObjectOfType<GameOverPanelManager>();
/*        if (gameOverPanel != null)
        {
            gameOverPanel.ShowGameOver();
        }

    */    // Destroy all the mines object
        //Reset Multipliers
        bettingManager.ResetMultipliers();
        UIManager.Instance.ResetMultiplierPanelsToDefault(); 
        UIManager.Instance.UpdateMultiplierPanels(); 
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////           Handle right swipes on non-catfish profiles              /////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
/*    private void HandleSwipeRightOnNonCatfish()
    {
        if (!CatfishManager.Instance.IsCurrentProfileCatfish())
        {
            // Get the current number of Catfish profiles to determine the base multiplier.
            int currentCatfishCount = CatfishManager.Instance.GetCurrentCatfishCount();
            float baseMultiplier = bettingManager.minesMultipliers[currentCatfishCount];

            // Calculate the incremented multiplier based on the number of right swipes,
            // starting with the base multiplier for the first swipe and adding 0.08 for subsequent swipes.
            // Since the base multiplier applies from the first swipe, increment starts from the second swipe.
            float incrementedMultiplier = baseMultiplier + ((RightSwipes - 1) * multiplierIncrementValue);

            // Calculate the winnings based on the current bet amount and the incremented multiplier.
            float winnings = bettingManager.betAmount * incrementedMultiplier;

            // As per your request, reset total winnings before updating to ensure it's not cumulative.
       *//*     bettingManager.ResetTotalWinnings();*//*

            // Update the winnings to reflect only the latest swipe's outcome.
            bettingManager.UpdateToBeAddedAmntText(winnings);
        }
    }*/
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////              Reset methods for a new game session              //////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void ResetTracker()
    {
        TotalSwipes = 0;
        LeftSwipes = 0;
        RightSwipes = 0;
        //swipeDecisions.Clear();
    }
    public void ResetGameStartedFlag()
    {
        gameStarted = false; 
    }
}