using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region References 
    public static UIManager Instance { get; private set; }

    [Header("1. Scripts References")]
    public BettingManager bettingManager;
    public BalanceFadeManager balanceFadeManager;
    public WinPanelFadeManager winPanelFadeManager;
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
    private int instantiatedPanelsEnabledCount = 5;

    [Header("11. Win Panel and Text")]
    public Image winPanelImage;
    public TextMeshProUGUI winPanelText;
    public GameObject plusAmountAnimation;

    [Header("12. Auto Bet buttons and Panels")]
    public Button autoBetButton;
    public TMP_InputField numberOfRounds;
    public Button whenWinningReset;
    public Button whenWinningIncrease;
    public Button whenLosingReset;
    public Button whenLosingIncrease;
    public TMP_InputField whenWinningIncreaseBy;
    public float betAmountBeforeWhenWinningIncrease;
    public float betAmountBeforeWhenLosingIncrease;
    public TMP_InputField whenLosingIncreaseBy;
    public Button stopAtAnyWin;
    public Sprite stopAtWinFalse;
    public Sprite stopAtWinTrue;
    public Button startAutoPlay;
    public Button stopAutoPlay;
    public Sprite AutoManualEnabledSprite;
    public Sprite AutoManualDisabledSprite;
    public Button manualButton;
    public Sprite resetOffSprite;
    public Sprite resetOnSprite;
    public Sprite IncreaseOffSprite;
    public Sprite IncreaseOnSprite;
    public Sprite HighlightedBoxSprite;
    public Image numberOfRoundsInfinite;

    public UnityAction disableUIWhenGameStarted;
    public UnityAction EnableUIWhenGameEnded;

    public bool IncreaseWhenWinningEnabled = false;
    public bool IncreaseWhenLosingEnabled = false;

    public float IncreaseWhenWinningValue;
    public float IncreaseWhenLosingValue;

    string currentMultiplierNumber;
    private float tolerance= 0.3f;

    public int NumberOfInstantiatedPanels = 5;
    #endregion References 
      

    #region Properties


    #endregion Properties

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
        AutoBetUiInteractableSet(false);


        disableUIWhenGameStarted += DisableBetAmntButtons;
        disableUIWhenGameStarted += DisableCatfishDirectSetButtons;
        disableUIWhenGameStarted += DisableMinesCountButtons;

        EnableUIWhenGameEnded += EnableMinesCountButtons;
        EnableUIWhenGameEnded += EnableBetAmntButtons;
        EnableUIWhenGameEnded += EnableMinesDirectSetButtons;

        manualButton.interactable = false;
        numberOfRounds.onSubmit.AddListener((value) =>
        {
            if (int.Parse(value) == 0||value==null)
            {
                numberOfRounds.text = "";
                numberOfRoundsInfinite.enabled = true;
            }
            else
            {
                numberOfRoundsInfinite.enabled = false;
            }
        });
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

        stopAutoPlay.onClick.AddListener(() =>
        {
            StopAutoPlayPressed();
        });
        manualButton.onClick.AddListener(() =>
        {
            ManualButtonPressed();
        });
        whenWinningIncrease.onClick.AddListener(() =>
        {
            WhenWinningIncreasePressed();
        });
        whenWinningReset.onClick.AddListener(() =>
        {
            WhenWinningResetPressed();
        });
        whenLosingIncrease.onClick.AddListener(() =>
        {
            WhenLosingIncreasePressed();
        });
        whenLosingReset.onClick.AddListener(() =>
        {
            WhenLosingResetPressed();
        });
        whenWinningIncreaseBy.onValueChanged.AddListener(value =>
        {
            float.TryParse(value, out IncreaseWhenWinningValue);
        });
        whenLosingIncreaseBy.onValueChanged.AddListener(value =>
        {
            float.TryParse(value, out IncreaseWhenLosingValue); 
        });
        stopAtAnyWin.onClick.AddListener(() =>
        {
            if(GameManager.Instance.autoBetManager.p_StopAtWinClicked==false)
            GameManager.Instance.autoBetManager.p_StopAtWinClicked = true;
            else if (GameManager.Instance.autoBetManager.p_StopAtWinClicked == true)
                GameManager.Instance.autoBetManager.p_StopAtWinClicked = false;
        });
        cashOutButton.onClick.AddListener(() => OnCashOutButtonPressed());
        setCatfishTo3Button.onClick.AddListener(() => SetMinesCount(3));
        setCatfishTo5Button.onClick.AddListener(() => SetMinesCount(5));
        setCatfishTo10Button.onClick.AddListener(() => SetMinesCount(10));
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
        MinesManager.Instance.DestroyAllTheObjects();
        AutoBetUiInteractableSet(true);
        WhenWinningIncreasePressed();
        WhenLosingIncreasePressed();
        HighlightMultiplierPanel(currentMultiplierIndex);
        autoBetButton.image.sprite = AutoManualEnabledSprite;
        manualButton.image.sprite = AutoManualDisabledSprite;
        GameManager.Instance.autoBet = true;

        autoBetButton.image.sprite = AutoManualEnabledSprite;
        manualButton.image.sprite = AutoManualDisabledSprite;
        manualButton.interactable = true;
        autoBetButton.interactable = false;


        startButton.gameObject.SetActive(false);
        startAutoPlay.gameObject.SetActive(true);
        startAutoPlay.interactable = false;
        MinesManager.Instance.InstantiateGridObjects();

        /*        DisableMinesCountButtons();
                DisableBetAmntButtons();
                DisableCatfishDirectSetButtons();*/

        UpdateRiskPercentageDisplay();


        bettingManager.CalculateNextMultipliers();
        UpdateMultiplierPanels();

    }

    void StartAutoPlayButtonPressed()
    {
        if (bettingManager.balanceAmount < bettingManager.minBetAmount)
        {

            //Show insufficient balance panel
            if (balanceFadeManager != null)
            {
                balanceFadeManager.ShowInsufficientBalancePanel();
                MinesManager.Instance.InstantiateWithoutDelay();
               GameManager.Instance.autoBetManager.StopAutoBet();
            }
            else
            {
                Debug.LogError("BalanceFadeManager not found in the scene.");
            }
            return;
        }
        GameManager.Instance.autoBetManager.p_NewAutoBetSession = true;
        GameManager.Instance.autoBetManager.StartAutoBet();
        SettingBetAmountInIncrrease();
        bettingManager.betAmountInput.interactable = false;
    }

    public void StopAtAnyWinEnable()
    {
        stopAtAnyWin.image.sprite = stopAtWinTrue;
    }

    public void StopAtAnyWinDisable()
    {
        stopAtAnyWin.image.sprite = stopAtWinFalse;
    }

    public void AutoBetUiInteractableSet(bool enable)
    {
        stopAtAnyWin.interactable = enable;
        whenLosingIncrease.interactable = enable;
        whenWinningIncrease.interactable = enable;
        numberOfRounds.interactable = enable;
        whenWinningReset.interactable = enable;
        whenLosingReset.interactable = enable;
    }
    public void StopAutoPlayButtonSet(bool enable)
    {
        stopAutoPlay.gameObject.SetActive(enable);
    }

    public void StartAutoPlaySet(bool enable)
    {
        startAutoPlay.gameObject.SetActive(enable);
    }

    public void StopAutoPlayPressed()
    {
       GameManager.Instance.autoBetManager.StopAutoBet();
    }

    public void ManualButtonPressed()
    {
        BettingManager.Instance.ResetTotalWinnings();
        BettingManager.Instance.ResetMultipliers();
        UIManager.Instance.ResetMultiplierPanelsToDefault();
        GameManager.Instance.gameStarted = false;
        manualButton.interactable = false;
        AutoBetUiInteractableSet(false);
        manualButton.image.sprite = AutoManualEnabledSprite;
        autoBetButton.image.sprite = AutoManualDisabledSprite;
        autoBetButton.interactable = true;
        StartAutoPlaySet(false);
        MinesManager.Instance.DestroyAllTheObjects();
        GameManager.Instance.autoBet = false;
        ShowStartButton(true);

        MinesManager.Instance.InstantiateGridObjects();
        MinesManager.Instance.DisableAllObjects();
        //ResetToDefaultMultipliers();
    }

    public void WhenWinningIncreasePressed()
    {
       
        whenWinningIncreaseBy.interactable = true;
        whenWinningIncrease.image.sprite = IncreaseOnSprite;
        whenWinningReset.image.sprite = resetOffSprite;
        IncreaseWhenWinningEnabled = true;
        whenWinningIncreaseBy.text = "0";
        IncreaseWhenWinningValue = 0;
        whenWinningReset.interactable = true;
        whenWinningIncrease.interactable = false;
    }
    public void WhenWinningResetPressed()
    {
/*        bettingManager.betAmount = betAmountBeforeWhenWinningIncrease;
        BettingManager.Instance.UpdateBetAmountIfIncrease(betAmountBeforeWhenWinningIncrease);*/
        whenWinningIncreaseBy.interactable = false;
        whenWinningIncrease.image.sprite = IncreaseOffSprite;
        whenWinningReset.image.sprite = resetOnSprite;
        IncreaseWhenWinningEnabled = false;
        whenWinningReset.interactable = false;
        whenWinningIncrease.interactable = true;
    }
    public void SettingBetAmountInIncrrease()
    {
        betAmountBeforeWhenWinningIncrease = bettingManager.betAmount;
        betAmountBeforeWhenLosingIncrease = bettingManager.betAmount;
    }

    public void ResettingWhenIncrease()
    {
        bettingManager.betAmount = betAmountBeforeWhenWinningIncrease;
        BettingManager.Instance.UpdateBetAmountIfIncrease(betAmountBeforeWhenWinningIncrease);
    }
    
    public void ResettingWhenLosing()
    {
        bettingManager.betAmount = betAmountBeforeWhenLosingIncrease;
        BettingManager.Instance.UpdateBetAmountIfIncrease(betAmountBeforeWhenLosingIncrease);
    }

    public void WhenLosingIncreasePressed()
    {
       
        whenLosingIncreaseBy.interactable = true;
        whenLosingIncrease.image.sprite = IncreaseOnSprite;
        whenLosingReset.image.sprite = resetOffSprite;
        IncreaseWhenLosingEnabled = true;
        whenLosingIncreaseBy.text = "0";
        IncreaseWhenLosingValue = 0;
        whenLosingReset.interactable = true;
        whenLosingIncrease.interactable = false;
    }


    public void WhenLosingResetPressed()
    {
/*        bettingManager.betAmount = betAmountBeforeWhenLosingIncrease;
        BettingManager.Instance.UpdateBetAmountIfIncrease(betAmountBeforeWhenLosingIncrease);*/
        whenLosingIncreaseBy.interactable = false;
        whenLosingIncrease.image.sprite = IncreaseOffSprite;
        whenLosingReset.image.sprite = resetOnSprite;
        IncreaseWhenLosingEnabled = false;
        whenLosingReset.interactable = false;
        whenLosingIncrease.interactable = true;
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
            float riskPercentage = ((float)MinesManager.Instance.totalMines / totalProfiles) * 100;
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


    #region Catfish Count Functions
    ///////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////      Mines Count Funct     /////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////
    
    public bool CheckIfTheMinesCountExceedsTheSelectedItems()
    {
        if (25 - GameManager.Instance.diamondsOpened <= GameManager.Instance.totalMinesCount && GameManager.Instance.autoBet == true)
        {
            return true;
        }
        return false;
    }
    public void IncreaseMaxMinesCount()
    {        
        if(CheckIfTheMinesCountExceedsTheSelectedItems()==true)
        {
            return;
        }
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
        if (GameManager.Instance.totalMinesCount > 2)
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
        if(GameManager.Instance.autoBet == true && count>25-GameManager.Instance.diamondsOpened)
        {
            count = 25 - GameManager.Instance.diamondsOpened;
        }
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
        decreaseCatfishCountButton.interactable = GameManager.Instance.totalMinesCount > 2;
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
    public void EnableMinesDirectSetButtons()
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

    public void checkStartFunction()
    {
        APIManager.Instance.betInsert(BettingManager.Instance.betAmount, MinesManager.Instance.totalMines);

    }
    public void OnStartButtonPressed() //To be called when Start button is clicked
    {
        MinesManager.Instance.DestroyAllTheObjects();
        
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
        GameManager.Instance.minesManager.DestroyAllTheObjects();
        bettingManager.betAmountInput.readOnly = false;
        ShowAutoBetButton(true);
        ShowCancelButton(false);
        ShowCashOutButton(false);
        ShowStartButton(true);
        EnableMinesCountButtons();
        EnableBetAmntButtons();
        EnableMinesDirectSetButtons();

        ResetRiskPercentageDisplay();

        bettingManager.ResetMultipliers(); // Reset the multipliers
        ResetMultiplierPanelsToDefault(); // Reset UI panels to default
        UpdateMultiplierPanels(); // Update the UI to reflect the reset state
        MinesManager.Instance.InstantiateGridObjects();
        MinesManager.Instance.DisableAllObjects();
    }
    public void OnAutoWin()
    {
        cashOutSound.Play();
        bettingManager.UpdateToBeAddedAmntText(GameManager.Instance.autoBetManager.winAmount);
        winPercentText.text = "+" + CalculateWinPercent(GameManager.Instance.autoBetManager.winAmount, bettingManager.betAmount)+"%";
        lastMultiplierText.text = GameManager.Instance.autoBetManager.winnings.ToString();
        bettingManager.CashOutWinnings();
        UpdateAndShowWinPanel();
/*
        bettingManager.ResetMultipliers(); // Reset the multipliers
        ResetMultiplierPanelsToDefault(); // Reset UI panels to default
        UpdateMultiplierPanels(); // Update the UI to reflect the reset*/
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
        lastMultiplierText.text ="x" +MinesManager.Instance.winningsInManual.ToString();

        GameManager.Instance.ResetMinesTracker();
        GameManager.Instance.gameStarted = false;
        MinesManager.Instance.ShowAllItems();
        bettingManager.betAmountInput.readOnly = false;
        bettingManager.CashOutWinnings();
        autoBetButton.interactable = true;
        ShowCashOutButton(false);
        ShowStartButton(true);
        ShowCancelButton(false);

        UpdateAndShowWinPanel();

        //Buttons
        EnableMinesCountButtons();
        EnableBetAmntButtons();
        EnableMinesDirectSetButtons();

        ResetRiskPercentageDisplay();

        bettingManager.ResetMultipliers(); // Reset the multipliers
        ResetMultiplierPanelsToDefault(); // Reset UI panels to default
        UpdateMultiplierPanels(); // Update the UI to reflect the reset
        GameManager.Instance.gameStarted = false;

    }

    public IEnumerator plusAnimationPlayAndStop(float amount)
    {
        plusAmountAnimation.GetComponent<TextMeshProUGUI>().text="+"+amount.ToString();
        plusAmountAnimation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        plusAmountAnimation.SetActive(false);
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
    public void DisableInstantiatedPanels(int count)
    {
        if (instantiatedPanelsEnabledCount > count)
        {
            instantiatedPanelsEnabledCount = count;
            for (int i = instantiatedPanels.Count - 1; i >= count; i--)
            {
                instantiatedPanels[i].GetComponent<Image>().enabled = false;
                instantiatedPanels[i].GetComponentInChildren<TextMeshProUGUI>().enabled = false;

            }

        }
        else
        {
            for(int i = 0;i<count; i++)
            {
                instantiatedPanels[i].GetComponent<Image>().enabled = true;
                instantiatedPanels[i].GetComponentInChildren<TextMeshProUGUI>().enabled = true;
            }
            instantiatedPanelsEnabledCount = count;
        
        }
    }
    public void CheckAndEnableInstantiatedPanels(int count)
    {
        for (int i = count - 1; i < 5; i++)
        {
            instantiatedPanels[i].GetComponent<Image>().enabled = true;
            instantiatedPanels[i].GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        }
        instantiatedPanelsEnabledCount = 5;
    }
    public void UpdateMultiplierPanels()
    {
        for (int i = 0; i < instantiatedPanelsEnabledCount; i++)
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

        Debug.Log("START INDEX FOR NEW MULTIPLIERS = " + startIndexForNewMultipliers);
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
            NumberOfInstantiatedPanels++;
            if (NumberOfInstantiatedPanels > bettingManager.nextMultipliers.Count)
            {
                textComponent.enabled = false;
                newPanel.GetComponent<Image>().enabled = false;
            }
        }

    }

    public void ResetToDefaultMultipliers()
    {
        for(int i = 0;i<instantiatedPanelsEnabledCount;i++)
        {
            var panel = instantiatedPanels[i];
            panel.GetComponent<Image>().enabled = true;
            panel.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        }
/*        foreach(var panel in instantiatedPanels)
        {
            panel.GetComponent<Image>().enabled = true;
            panel.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        }*/
    }

    public void CheckAndAdjustMultiplierPanelInAuto()
    {
        if (currentMultiplierIndex >= instantiatedPanels.Count)
        {
            Debug.Log("[CheckAndAdjustMultiplierPanels] Adjusting Multiplier Panels due to reaching the threshold.");
            RecycleAndAddMultiplierPanels();
        }
        else if (currentMultiplierIndex < 0)
        {
            RecycleAutoBackwards();
        }
    }
    private void RecycleAutoBackwards()
    {
        #region checking if it is the first element

        int startIndexTemp = CheckAndReturnStartIndex();
        if (startIndexTemp < 0)
        {
            HighlightMultiplierPanel(currentMultiplierIndex);
            return;
        }

        #endregion checking if it is the first element


        int count = instantiatedPanels.Count;
        /*currentMultiplierIndex += 2;*/
        for (int i = count - 1; i >= count - 2; i--)
        {
            NumberOfInstantiatedPanels--;
            Destroy(instantiatedPanels[i].gameObject);
            instantiatedPanels.RemoveAt(i);
        }

        int startindex = CheckAndReturnStartIndex();


        for (int i = 0; i < 2; i++)
        {
            GameObject newPanel = Instantiate(multiplierValuePanelPrefab, multiplierPanelParent);
            TextMeshProUGUI textComponent = newPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null && startindex + i < bettingManager.nextMultipliers.Count)
            {
                // Update the text with the next multiplier
                textComponent.text = $"x{bettingManager.nextMultipliers[startindex+i]:F2}";
                Debug.Log("StartIndex = " + startindex);
            }
            // Set the sibling index to 0 to add the new panel at the beginning
            newPanel.transform.SetSiblingIndex(i);
            instantiatedPanels.Insert(i, newPanel);
            // instantiatedPanels.Insert(0, newPanel); // Update the list to reflect the new order

        }
        currentMultiplierIndex += 2;
        HighlightMultiplierPanel(currentMultiplierIndex);

    }
    private int CheckAndReturnStartIndex()
    {
        string startString = instantiatedPanels[0].GetComponentInChildren<TextMeshProUGUI>().text;
        Debug.Log("<color=red>Start Index = </color>" + startString);
        startString = startString.Substring(1);
        double startValue;
        double.TryParse(startString, out startValue);
        int temp = bettingManager.nextMultipliers.IndexOf(startValue);
        if (temp == -1)
        {
            for (int i = 0; i < bettingManager.nextMultipliers.Count; i++)
            {
                if (Mathf.Abs((float)(bettingManager.nextMultipliers[i] - startValue)) < tolerance)
                {
                    var val = Mathf.Abs((float)(bettingManager.nextMultipliers[i] - startValue));
                    temp = i;
                    break;
                }
            }
        }
        int startindex = temp - 2;
        return startindex;
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
       // Color defaultTextColor = new Color32(0xE6, 0x49, 0x7E, 0xFF); // #E6497E
       //olor currentTextColor = Color.white; // #FFFFFF

        // Iterate through all instantiated panels
        for (int i = 0; i < instantiatedPanels.Count; i++)
        {
            Image panelImage = instantiatedPanels[i].GetComponent<Image>();
            TextMeshProUGUI textComponent = instantiatedPanels[i].GetComponentInChildren<TextMeshProUGUI>();

            // Update the sprite based on the panel's index relative to the current multiplier
            if (i < currentMultiplierIndex)
            {
                panelImage.sprite = previousMultiplierSprite;
            }
            else if (i == currentMultiplierIndex)
            {
                panelImage.sprite = currentMultiplierSprite;
                currentMultiplierNumber = instantiatedPanels[i].GetComponentInChildren<TextMeshProUGUI>().text;
            }
            else
            {
                panelImage.sprite = defaultMultiplierSprite;
            }
        }
    }
    public void ResetMultiplierPanelsToDefault()
    {
        currentMultiplierIndex = -1;

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