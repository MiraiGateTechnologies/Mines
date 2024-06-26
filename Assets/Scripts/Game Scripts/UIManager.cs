using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region References 
    public static UIManager Instance { get; private set; }

    [Header("1. Scripts References")]
    public BettingManager bettingManager;
    private SwipeManager swipeManager;
    public BalanceFadeManager balanceFadeManager;
    public WinPanelFadeManager winPanelFadeManager;
    public CharacterAssetManager characterAssetManager;
    public CharacterGenerator characterGenerator;

    [Header("2. Buttons")]
    public Button likeButton;
    public Button dislikeButton;

    public Button startButton;
    public Button cancelButton;
    public Button cashOutButton;

    public Button increaseCatfishCountButton;
    public Button decreaseCatfishCountButton;

    public Button increseBetAmntButton;
    public Button decreaseBetAmntButton;
    public Button minbetAmntButton;
    public Button maxbetAmntButton;

    public Button setCatfishTo3Button;
    public Button setCatfishTo5Button;
    public Button setCatfishTo10Button;
    public Button setCatfishTo20Button;

    public Button aboutButton;
    public Button about_crossButton;

    public GameObject likeButtonGameObj;
    public GameObject dislikeButtonGameObj;

    public GameObject SoundOffBtnGameObj;
    public GameObject SoundOnBtnGameObj;

    [Header("3. Texts")]
    public TextMeshProUGUI maxCatfishCountText;
    public TextMeshProUGUI swipeCountText;
    public TextMeshProUGUI riskPercentageText;
    public TextMeshProUGUI winPercentText;
    public TextMeshProUGUI lastMultiplierText;

    [Header("4. Audio References")]
    public AudioSource startButtonSound;
    public AudioSource genericButtonSound;
    public AudioSource diamondOpenSound;
    public AudioSource minesBlastSound;
    public AudioSource cashOutSound;


    [Header("5. Canvas Groups")]
    public CanvasGroup winPanelCanvasGroup;
    public CanvasGroup historyPanelCanvasGroup;

    [Header("6. Sprites")]
    public Sprite currentMultiplierSprite;
    public Sprite previousMultiplierSprite;
    public Sprite defaultMultiplierSprite;
    public Sprite popUpLikeButtonSprite;
    public Sprite defaultLikeButtonSprite;
    public Sprite popUpDislikeButtonSprite;
    public Sprite defaultDislikeButtonSprite;

    [Header("7. Multiplier func References")]
    public GameObject multiplierValuePanelPrefab;
    public Transform multiplierPanelParent;
    public int multiplierValuePanelNumber = 5;

    [Header("8. History func References")]
    //New ones
    public GameObject historyContainerPrefab;
    public Transform historyContainerParent;

    //unused
    public GameObject historyPanelPrefab;
    public Transform contentParentofHistory;

    [Header("9. Panels")]
    public GameObject aboutPanel;
    public GameObject historyPanel;
    public GameObject settingsPanel;

    [Header("10. Game Over")]
    public Image gameOverPanel;
    public TextMeshProUGUI gameOverText;

    [Header("10. Other References")]
    public int currentMultiplierIndex = 0;
    private List<GameObject> instantiatedPanels = new List<GameObject>();

    [Header("11. Win Panel and Text")]
    public Image winPanelImage;
    public TextMeshProUGUI winPanelText;

    [Header("12. Auto Bet buttons and Panels")]
    public Button autoBetButton;
    public TMP_InputField numberOfRounds;
    public Button whenWinningReset;
    public Button whenWinningIncrease;
    public Button whenLosingReset;
    public Button whenLosingIncrease;
    public TMP_InputField whenWinningIncreaseBy;
    public TMP_InputField whenLosingIncreaseBy;
    public Button stopAtAnyWin;
    public Sprite stopAtWinFalse;
    public Sprite stopAtWinTrue;
    public Button startAutoPlay;
    public Button stopAutoPlay;
    public Sprite AutoManualEnabledSprite;
    public Sprite AutoManualDisabledSprite;
    public Button manualButton;

    string currentMultiplierNumber;
    #endregion References 

    #region Awake and Start
    ///////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////      Awake & start     ////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////
    void Awake()
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
    void Start()
    {
        SpawnMultiplierValuePanels(multiplierValuePanelNumber);
        UpdateMaxMinesCountDisplay(); // Initial display update
        InitializeButtons();
    }
    #endregion Awake and Start

    #region Initialize On Click Buttons Listeners
    ///////////////////////////////////////////////////////////////////////////////////////
    ///////////////////      Initialize On Click Buttons Listeners     ////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////
    void InitializeButtons()
    {
        autoBetButton.onClick.AddListener(() =>
        {
            AutoBetButtonPressed();
        });
        startAutoPlay.onClick.AddListener(() =>
        {
            StartAutoPlayButtonPressed();
        });
        cashOutButton.onClick.AddListener(() => OnCashOutButtonPressed());
        setCatfishTo3Button.onClick.AddListener(() => SetMinesCount(7));
        setCatfishTo5Button.onClick.AddListener(() => SetMinesCount(10));
        setCatfishTo10Button.onClick.AddListener(() => SetMinesCount(15));
        setCatfishTo20Button.onClick.AddListener(() => SetMinesCount(20));
    }
    #endregion Initialize On Click Buttons Listeners

    #region Auto Bet Functionality

    public void EnableStartAutoPlayButton()
    {
        startAutoPlay.interactable = true;
    }
    void AutoBetButtonPressed()
    {
        HighlightMultiplierPanel(UIManager.Instance.currentMultiplierIndex);
        autoBetButton.image.sprite=AutoManualEnabledSprite;
        manualButton.image.sprite=AutoManualDisabledSprite;
        GameManager.Instance.autoBet = true;

        startButton.gameObject.SetActive(false);
        startAutoPlay.gameObject.SetActive(true);
        startAutoPlay.interactable = false;
        MinesManager.Instance.InstantiateGridObjects();

        DisableMinesCountButtons();
        DisableBetAmntButtons();
        DisableCatfishDirectSetButtons();

        UpdateRiskPercentageDisplay();


        bettingManager.CalculateNextMultipliers();
        UpdateMultiplierPanels();

    }

    void StartAutoPlayButtonPressed()
    {
        GameManager.Instance.autoBetManager.StartAutoBet();
    }

    #endregion Auto Bet Functionality

    #region Risk Percentage Display Functions
    ///////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////      Risk Percentage Display     ////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////
    public void UpdateRiskPercentageDisplay()
    {
        if (riskPercentageText != null)
        {
            int totalProfiles = 25;
            float riskPercentage = ((float)CatfishManager.Instance.maxCatfishCount / totalProfiles) * 100;
            riskPercentageText.text = $"{riskPercentage.ToString("0.00")}%";
        }
    }
    public void ResetRiskPercentageDisplay()  // Reset the risk percentage display
    {
        if (riskPercentageText != null)
        {
            riskPercentageText.text = "0%";
        }
    }
    #endregion Risk Percentage Display Functions

    #region Swipe Count Display Functions
    ///////////////////////////////////////////////////////////////////////////////////////
    //////////////////////      Swipe Count Display Functions     /////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////
    public void UpdateSwipeCountDisplay(int currentSwipe, int totalSwipes)
    {
        if (swipeCountText != null)
        {
            swipeCountText.text = $"{currentSwipe}/{totalSwipes}";
        }
    }
    public void ResetSwipeCountDisplay()
    {
        if (swipeCountText != null)
        {
            swipeCountText.text = $"0/25";
        }

    }
    #endregion Swipe Count Display Functions

    #region Catfish Count Functions
    ///////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////      Catfish Count Funct     /////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////
    public void IncreaseMaxMinesCount()
    {
        if (GameManager.Instance.totalMinesCount < 24)
        {
            GameManager.Instance.totalMinesCount++;
            MinesManager.Instance.updateTotalMinesCount(GameManager.Instance.totalMinesCount);

            bettingManager.CalculateNextMultipliers();
            UpdateMultiplierPanels();
            UpdateMaxMinesCountDisplay();
        }
    }
    public void DecreaseMaxMinesCount()
    {
        if (GameManager.Instance.totalMinesCount > 5)
        {
            GameManager.Instance.totalMinesCount--;
            MinesManager.Instance.updateTotalMinesCount(GameManager.Instance.totalMinesCount);

            bettingManager.CalculateNextMultipliers();
            UpdateMultiplierPanels();
            UpdateMaxMinesCountDisplay();
        }
    }
    void SetMinesCount(int count)
    {
        MinesManager.Instance.totalMines = count;
        GameManager.Instance.totalMinesCount= count;
        UpdateMaxMinesCountDisplay();

        bettingManager.CalculateNextMultipliers();
        UpdateMultiplierPanels();
    }
    void UpdateMaxMinesCountDisplay()
    {
        maxCatfishCountText.text = $"{GameManager.Instance.totalMinesCount}";

        increaseCatfishCountButton.interactable = GameManager.Instance.totalMinesCount < 24;
        decreaseCatfishCountButton.interactable = GameManager.Instance.totalMinesCount > 5;
    }
    public void DisableMinesCountButtons()
    {
        increaseCatfishCountButton.interactable = false;
        decreaseCatfishCountButton.interactable = false;
    }
    public void EnableMinesCountButtons()
    {
        increaseCatfishCountButton.interactable = true;
        decreaseCatfishCountButton.interactable = true;
    }
    void DisableCatfishDirectSetButtons()
    {
        setCatfishTo3Button.interactable = false;
        setCatfishTo5Button.interactable = false;
        setCatfishTo10Button.interactable = false;
        setCatfishTo20Button.interactable = false;
    }
    public void EnableCatfishDirectSetButtons()
    {
        setCatfishTo3Button.interactable = true;
        setCatfishTo5Button.interactable = true;
        setCatfishTo10Button.interactable = true;
        setCatfishTo20Button.interactable = true;
    }
    #endregion Catfish Count Functions

    #region Bet Amount Buttons
    ///////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////      Bet Amnt Btns     ////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////
    public void DisableBetAmntButtons()
    {
        increseBetAmntButton.interactable = false;
        decreaseBetAmntButton.interactable = false;
        minbetAmntButton.interactable = false;
        maxbetAmntButton.interactable = false;
    }
    public void EnableBetAmntButtons()
    {
        increseBetAmntButton.interactable = true;
        decreaseBetAmntButton.interactable = true;
        minbetAmntButton.interactable = true;
        maxbetAmntButton.interactable = true;
    }
    public void addBalanceAmount()
    {
        bettingManager.balanceAmount += 100;
        bettingManager.UpdateBalanceText();
    }
    #endregion Bet Amount Buttons

    #region Like & Dislike Button Functionalities
    ///////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////      Like & Disklike Btns      ////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////  Like & Disklike interactivity  ///////////////////////////
    public void DisableLikeDislikeButtons()
    {
        likeButton.interactable = false;
        dislikeButton.interactable = false;
    }
    public void EnableLikeDislikeButtons()
    {
        likeButton.interactable = true;
        dislikeButton.interactable = true;
    }
    /////////////////////////  Like & Disklike Btns swipe func //////////////////////////////

    public void SetSwipeManager(SwipeManager manager)
    {
        swipeManager = manager;
    }
    public void UpdateSwipeManagerReference()
    {
        var swipeManagers = FindObjectsOfType<SwipeManager>();
        if (swipeManagers.Length > 1) // Ensure not to select the one being destroyed
        {
            foreach (var manager in swipeManagers)
            {
                if (manager != swipeManager) // Assuming swipeManager is the current reference
                {
                    SetSwipeManager(manager);
                    break;
                }
            }
        }
        else
        {
            // Handle the case where no other SwipeManager is available
            // This could involve disabling the like/dislike buttons or other UI elements
            DisableLikeDislikeButtons();
        }
    }
    public void OnLikeButtonPressed()
    {
        if (swipeManager != null)
        {
            swipeManager.SwipeRight();
        }
        else
        {
            Debug.LogError("SwipeManager reference is null.");
        }
    }
    public void OnDislikeButtonPressed()
    {
        swipeManager?.SwipeLeft();
    }
    public void ChangeLikeButtonSpriteAndSize(Sprite newSprite, Vector2 newSize)
    {
        likeButton.image.sprite = newSprite; // Change sprite
        likeButton.GetComponent<RectTransform>().sizeDelta = newSize; // Change size
    }
    public void ChangeDislikeButtonSpriteAndSize(Sprite newSprite, Vector2 newSize)
    {
        dislikeButton.image.sprite = newSprite; // Change sprite
        dislikeButton.GetComponent<RectTransform>().sizeDelta = newSize; // Change size
    }
    public void DisableDislikeButton()
    {
        dislikeButton.interactable = false;
    }




    #endregion Like & Dislike Button Functionalities

    #region Start - Cancel - Cashout Btn Functionalities
    ///////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////      Start - Cancel - Cashout Btns      //////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////

    ///////////////////  Start - Cancel - Cashout Btns interactivity  /////////////////////
    public void ShowStartButton(bool show)
    {
        startButton.gameObject.SetActive(show);
    }
    public void ShowCancelButton(bool show)
    {
        cancelButton.gameObject.SetActive(show);
    }
    public void ShowCashOutButton(bool show)
    {
        cashOutButton.gameObject.SetActive(show);
    }

    public void ShowAutoBetButton(bool show)
    {
        autoBetButton.interactable = show;
    }
    ///////////////////  Start - Cancel - Cashout Btns on-click methods  ////////////////////
    public void OnStartButtonPressed() //To be called when Start button is clicked
    {
        // Play the button click sound
        if (startButtonSound != null)
        {
            startButtonSound.Play();
        }

        if (bettingManager.balanceAmount < bettingManager.minBetAmount)
        {

            //Show insufficient balance panel
            if (balanceFadeManager != null)
            {
                balanceFadeManager.ShowInsufficientBalancePanel();
            }
            else
            {
                Debug.LogError("BalanceFadeManager not found in the scene.");
            }
            return;
        }
        GameManager.Instance.minesManager.InstantiateGridObjects();
        bettingManager.StartGame();

        bettingManager.betAmountInput.readOnly = true;
        ShowAutoBetButton(false);
        HighlightMultiplierPanel(UIManager.Instance.currentMultiplierIndex);
        ShowStartButton(false);
        ShowCancelButton(true);


        DisableMinesCountButtons();
        DisableBetAmntButtons();
        DisableCatfishDirectSetButtons();

        UpdateRiskPercentageDisplay();


        bettingManager.CalculateNextMultipliers();
        UpdateMultiplierPanels();

    }
    public void OnCancelButtonPressed() //To be called when Cancel button is clicked
    {
        bettingManager.betAmountInput.readOnly = false;
        ShowAutoBetButton(true);
        ShowCancelButton(false);
        ShowCashOutButton(false);
        ShowStartButton(true);
        EnableMinesCountButtons();
        EnableBetAmntButtons();
        EnableCatfishDirectSetButtons();

        GameManager.Instance.minesManager.DestroyAllTheObjects();
        ResetRiskPercentageDisplay();
        ResetSwipeCountDisplay();

        bettingManager.ResetMultipliers(); // Reset the multipliers
        ResetMultiplierPanelsToDefault(); // Reset UI panels to default
        UpdateMultiplierPanels(); // Update the UI to reflect the reset state

    }
    public void OnAutoWin()
    {
        cashOutSound.Play();
        winPercentText.text = "+" + CalculateWinPercent(GameManager.Instance.autoBetManager.winAmount, bettingManager.betAmount);
        lastMultiplierText.text = currentMultiplierNumber;
        bettingManager.CashOutWinnings();
        UpdateAndShowWinPanel();

        bettingManager.ResetMultipliers(); // Reset the multipliers
        ResetMultiplierPanelsToDefault(); // Reset UI panels to default
        UpdateMultiplierPanels(); // Update the UI to reflect the reset
    }
    public void OnCashOutButtonPressed() //To be called when Cashout button is clicked
    {
        // Play the button click sound
        /*        if (startButtonSound != null)
                {
                    startButtonSound.Play();
                }*/
        MinesManager.Instance.DisableAllObjects();
        cashOutSound.Play();
        winPercentText.text= "+"+CalculateWinPercent(bettingManager.totalWinnings, bettingManager.betAmount) +"%";
        lastMultiplierText.text = currentMultiplierNumber;
        //MinesManager.Instance.ResetTotalMinesCount();
        GameManager.Instance.ResetMinesTracker();
        SwipesTracker.Instance.ResetGameStartedFlag();
        MinesManager.Instance.DestroyAllTheObjects();
        bettingManager.betAmountInput.readOnly = false;
        bettingManager.CashOutWinnings();

        ShowCashOutButton(false);
        ShowStartButton(true);
        ShowCancelButton(false);

        UpdateAndShowWinPanel();

        //Buttons
        EnableMinesCountButtons();
        EnableBetAmntButtons();
        EnableCatfishDirectSetButtons();

        ResetRiskPercentageDisplay();
        ResetSwipeCountDisplay();

        bettingManager.ResetMultipliers(); // Reset the multipliers
        ResetMultiplierPanelsToDefault(); // Reset UI panels to default
        UpdateMultiplierPanels(); // Update the UI to reflect the reset
        GameManager.Instance.gameStarted = false;

    }


    string CalculateWinPercent(float totalWinnings,float betAmount)
    {
        float winPercent = ((totalWinnings- betAmount) /betAmount) * 100f;
        string formattedWinPercentage = winPercent.ToString("0");
        return formattedWinPercentage;
    }
    #endregion Start - Cancel - Cashout Btn Functionalities

    #region Multiplier Panel Functionalities
    ///////////////////////////////////////////////////////////////////////////////////////
    /////////////////////      Multiplier Panel Functionalities      //////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////
    void SpawnMultiplierValuePanels(int count)
    {
        instantiatedPanels.Clear();
        for (int i = 0; i < count; i++)
        {
            GameObject panel = Instantiate(multiplierValuePanelPrefab, multiplierPanelParent);
            instantiatedPanels.Add(panel);
        }
    }
    public void UpdateMultiplierPanels()
    {
        for (int i = 0; i < instantiatedPanels.Count; i++)
        {
            TextMeshProUGUI textComponent = instantiatedPanels[i].GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = $"x{bettingManager.nextMultipliers[i]:F2}";
            }
        }
    }
    private void RecycleAndAddMultiplierPanels()
    {
        // Delete the first two panels
        for (int i = 0; i < 2; i++)
        {
            Destroy(instantiatedPanels[0].gameObject); // Always remove the first element
            instantiatedPanels.RemoveAt(0);
        }

        currentMultiplierIndex -= 2;
        currentMultiplierIndex = Mathf.Max(0, currentMultiplierIndex);
        Debug.Log($"Adjusted currentMultiplierIndex: {currentMultiplierIndex}");


        // Add two new panels at the end
        int startIndexForNewMultipliers = instantiatedPanels.Count + GameManager.Instance.diamondsOpened - 4; // Adjust based on current multiplier index
        Debug.Log("start Index=" + startIndexForNewMultipliers);
        for (int i = 0; i < 2; i++)
        {
            GameObject newPanel = Instantiate(multiplierValuePanelPrefab, multiplierPanelParent);
            TextMeshProUGUI textComponent = newPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null && startIndexForNewMultipliers + i < bettingManager.nextMultipliers.Count)
            {
                // Update the text with the next multiplier
                textComponent.text = $"x{bettingManager.nextMultipliers[startIndexForNewMultipliers + i]:F2}";
            }
            instantiatedPanels.Add(newPanel);
        }

    }
    public void CheckAndAdjustMultiplierPanels()//called in swipe tracker class whenever we right swipe
    {
        Debug.Log($"[CheckAndAdjustMultiplierPanels] Current Multiplier Index: {currentMultiplierIndex}, Panels Count: {instantiatedPanels.Count}");

        // Assuming currentMultiplierIndex is properly updated somewhere in your logic
        // to reflect the current multiplier panel in focus
        if (currentMultiplierIndex >= instantiatedPanels.Count)
        {
            Debug.Log("[CheckAndAdjustMultiplierPanels] Adjusting Multiplier Panels due to reaching the threshold.");
            RecycleAndAddMultiplierPanels();
        }
        else
        {
            Debug.Log("[CheckAndAdjustMultiplierPanels] No adjustment needed at this time.");
        }
    }
    public void HighlightMultiplierPanel(int currentMultiplierIndex)
    {
        Color defaultTextColor = new Color32(0xE6, 0x49, 0x7E, 0xFF); // #E6497E
        Color currentTextColor = Color.white; // #FFFFFF

        // Iterate through all instantiated panels
        for (int i = 0; i < instantiatedPanels.Count; i++)
        {
            Image panelImage = instantiatedPanels[i].GetComponent<Image>();
            TextMeshProUGUI textComponent = instantiatedPanels[i].GetComponentInChildren<TextMeshProUGUI>();

            // Update the sprite based on the panel's index relative to the current multiplier
            if (i < currentMultiplierIndex)
            {
                panelImage.sprite = previousMultiplierSprite;
                //textComponent.color = defaultTextColor; // Set text color for previous multipliers
            }
            else if (i == currentMultiplierIndex)
            {
                panelImage.sprite = currentMultiplierSprite;
                currentMultiplierNumber = instantiatedPanels[i].GetComponentInChildren<TextMeshProUGUI>().text;
                //textComponent.color = currentTextColor; // Set text color for the current multiplier
            }
            else
            {
                panelImage.sprite = defaultMultiplierSprite;
                //textComponent.color = defaultTextColor; // Set text color for future multipliers
            }
        }
    }
    public void ResetMultiplierPanelsToDefault()
    {
        currentMultiplierIndex = 0;

        // Set default text color (assuming you want to revert to this color)
        Color defaultTextColor = new Color32(0xE6, 0x49, 0x7E, 0xFF); // Example color

        // Iterate through all instantiated panels
        foreach (var panel in instantiatedPanels)
        {
            // Set the panel's sprite to the default sprite
            var panelImage = panel.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.sprite = defaultMultiplierSprite;
            }

            // Set the text color of the panel to the default text color
            var textComponent = panel.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                //textComponent.color = defaultTextColor;
            }
        }
    }
    #endregion Multiplier Panel Functionalities

    #region History Panel Functionalities
    ///////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////      History panel func      //////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////

    public void ShowHistoryPanel()
    {
      //  historyPanel.gameObject.SetActive(true);
      //  SpawnHistoryContainers();
    }
    public void HideHistoryPanel()
    {
        historyPanel.gameObject.SetActive(false);
        ClearHistoryContainers();
        SwipesTracker.Instance.ClearSwipeDecisions();
    }
    //--------------------------------------------------------------------------------------
    private void SpawnHistoryContainers()
    {
        // Clear out any existing history containers first
        foreach (Transform child in historyContainerParent)
        {
            Destroy(child.gameObject);
        }

        // Get the list of session images from CharacterGenerator and reverse it
        List<Sprite> images = characterGenerator.GetSessionImages();
        images.Reverse(); // Reverse the order of images

        // Ensure there are 25 images
        if (images.Count != 25)
        {
            Debug.LogError("The number of session images does not match the expected count (25).");
            return;
        }

        // Get catfish decisions
        List<string> catfishDecisions = CatfishManager.Instance.GetProfileDecisions();
        if (catfishDecisions.Count != 25)
        {
            Debug.LogError("The number of catfish decisions does not match the expected count (25).");
            return;
        }

        // Instantiate and set images and catfish decisions for history containers in reversed order
        for (int i = 0; i < 25; i++)
        {
            GameObject container = Instantiate(historyContainerPrefab, historyContainerParent);
            HistoryContainerPanel panel = container.GetComponent<HistoryContainerPanel>();
            if (panel != null)
            {
                panel.womenImageHistory.sprite = images[i];
                panel.catfishDecisionTxt.text = catfishDecisions[i];
                panel.countTxt.text = (i + 1).ToString();// Set the count text for each panel

                bool isCatfish = catfishDecisions[i] == "Catfish profile"; // Adjust this condition based on your actual decision texts
                panel.SetCatfishDecisionPanel(isCatfish);
                panel.SetCountPanel(isCatfish);
                panel.SetTextColors(isCatfish);

                // check if there's a swipe decision for this profile
                bool? rightSwipe = i < SwipesTracker.Instance.swipeDecisions.Count ? (SwipesTracker.Instance.swipeDecisions[i] == "Right Swiped") : (bool?)null;
                panel.SetSwipeDecisionImage(rightSwipe);

                // Update the highlight panel with swipe and catfish status
                bool hasBeenSwiped = rightSwipe.HasValue;
                panel.UpdateHighlightPanel(hasBeenSwiped, isCatfish);
                panel.UpdateSwipeDecisionPanelVisibility(hasBeenSwiped);
            }
        }
    }
    private void ClearHistoryContainers()
    {
        // Loop through all children of the historyContainerParent and destroy them
        foreach (Transform child in historyContainerParent)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion History Panel Functionalities

    #region Other Dependent Methods
    ///////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////// Other Dependent Methods ////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////
    public void UpdateAndShowWinPanel()
    {
        if (winPanelFadeManager != null)
        {
            float winAmountValue = bettingManager.GetTotalWinnings();
            string winAmount = winAmountValue.ToString("0.00");

            PanelFadeOutManager panelFadeOutManager = FindObjectOfType<PanelFadeOutManager>();
            panelFadeOutManager.FadeInOut(winPanelImage, winPanelText);
            winPanelFadeManager.UpdateWinAmountText(winAmount);
            winPanelFadeManager.ShowWinPanel();
           // bettingManager.ResetTotalWinnings();
        }
        else
        {
            Debug.LogError("WinPanelFadeManager is not assigned.");
        }
    }
    /*private void HandleAboutButtonClicked()
    {
        // Toggle the active state of the About Panel
        aboutPanel.SetActive(!aboutPanel.activeInHierarchy);
    }
    private void HandleDisableAboutPanel()
    {
        // Disable the About Panel when the background is clicked
        aboutPanel.SetActive(false);
    }*/
    //--------------------------------------------------------------------------------------

    public void OnSettingsBtnClick()
    {
        settingsPanel.SetActive(true);
    }
    public void OnBackToGameBtnClick()
    {
        settingsPanel.SetActive(false);
    }
    public void OnAboutBtnClick()
    {
        aboutPanel.SetActive(true);
    }
    public void OnExitAboutPanel()
    {
        aboutPanel.SetActive(false);
    }
    public void OnSoundOffBtnClick()
    {
        SoundOffBtnGameObj.SetActive(false);
        SoundOnBtnGameObj.SetActive(true);
    }
    public void OnSoundOnBtnClick()
    {
        SoundOffBtnGameObj.SetActive(true);
        SoundOnBtnGameObj.SetActive(false);
    }


    #endregion Other Dependent Methods
}