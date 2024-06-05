using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AutoBetManager : MonoBehaviour
{
    public static bool stopRequested;
    int numberOfRounds;
    bool infiniteRounds = false;
    public float winnings;
    public bool stopAtAnyWin = false;
    public bool autoBetStarted = false;
    public bool roundWinStatus = true;
    List<GameObject> selectedGridElements = new List<GameObject>();
    List<int> IndexOfSelectedElements = new List<int>();
    public float winAmount;
    public bool _newSession;
    public int currentMultiplierInAuto = 0;
    public BalanceFadeManager balanceFadeManager;

    #region Properties
    public bool p_StopAtWinClicked
    {
        get
        {
            return stopAtAnyWin;
        }
        set
        {
            if (value == true)
            {
                stopAtAnyWin = value;
                UIManager.Instance.StopAtAnyWinEnable();
            }
            else
            {
                stopAtAnyWin = value;
                UIManager.Instance.StopAtAnyWinDisable();
            }
        }
    }

    /// <summary>
    /// PROPERTY TO DEFINE GAMEOVER IN AUTO PLAY
    /// </summary>
    public bool p_NewAutoBetSession
    {
        get
        {
            return _newSession;
        }

        set
        {
            _newSession = value;
            GameManager.Instance.gameStarted = value;
            stopRequested = !value;
        }
    }

    #endregion Properties

    #region Start And Stop Bet

    public void StartAutoBet()
    {
        UIManager.Instance.disableUIWhenGameStarted.Invoke();
        GameManager.Instance.minesManager.DestroyAllTheObjects();



        UIManager.Instance.AutoBetUiInteractableSet(false);
        roundWinStatus = true;
        UIManager.Instance.StopAutoPlayButtonSet(true);
        UIManager.Instance.StartAutoPlaySet(false);
        // Parse settings from UI
        int.TryParse(UIManager.Instance.numberOfRounds.text, out numberOfRounds);
        if (numberOfRounds == 0)
        {
            infiniteRounds = true;
            StartCoroutine(IndefiniteAutoBetRoutine());
        }
        else
        {
            StartCoroutine(FixedAutoBetRoutine(numberOfRounds));
        }
    }
    public void StopAutoBet()
    {
       
        BettingManager.Instance.betAmountInput.interactable = true;
        p_NewAutoBetSession = false;
        StartCoroutine(ClearSelectedIndexList());
        UIManager.Instance.AutoBetUiInteractableSet(true);
        UIManager.Instance.StopAutoPlayButtonSet(false);
        UIManager.Instance.StartAutoPlaySet(true);
        UIManager.Instance.startAutoPlay.interactable = false;


        GameManager.Instance.ResetMinesTracker();
        // minesManager.ResetTotalMinesCount();
        BettingManager.Instance.ResetTotalWinnings();
        BettingManager.Instance.UpdateBalanceText();

        BettingManager.Instance.ResetMultipliers();
        UIManager.Instance.ResetMultiplierPanelsToDefault();
        UIManager.Instance.UpdateMultiplierPanels();
        UIManager.Instance.EnableUIWhenGameEnded.Invoke();

        if(UIManager.Instance.IncreaseWhenWinningEnabled==true)
        {
            UIManager.Instance.WhenWinningIncreasePressed();
        }
        else
        {
            UIManager.Instance.WhenWinningResetPressed();
        }

        if(UIManager.Instance.IncreaseWhenLosingEnabled == true)
        {
            UIManager.Instance.WhenLosingIncreasePressed();
        }
        else
        {
            UIManager.Instance.WhenLosingResetPressed();
        }
       // UIManager.Instance.ResetToDefaultMultipliers();
    }

    private IEnumerator ClearSelectedIndexList()
    {
        yield return new WaitForSeconds(1.2f);
        Debug.Log("<color=red> BET ITEMS CLEARED</color>");
        IndexOfSelectedElements.Clear();
    }

    #endregion Start and Stop Bet

    #region Bet Iterations
    private IEnumerator IndefiniteAutoBetRoutine()
    {
        if (BettingManager.Instance.balanceAmount <= 0)
        {
            MinesManager.Instance.InstantiateWithoutDelay();
            ShowInsufficiecntBalanceMessage();
           StopAutoBet();
        }


        while (!stopRequested && BettingManager.Instance.betAmount <= BettingManager.Instance.balanceAmount)
        {
            BettingManager.Instance.balanceAmount -= BettingManager.Instance.betAmount;
            UIManager.Instance.manualButton.interactable = false;

            BettingManager.Instance.UpdateBalanceText();
            GameManager.Instance.minesManager.InstantiateWithoutDelay();//Instantiate elements
            MinesManager.Instance.DisableAllObjects();

            yield return new WaitForSeconds(0.4f); // Adjust time as needed
            MinesManager.Instance.ShowAllItems();//Reveal all the objects
            
            AddElementsSelectedToList();

            yield return new WaitForSeconds(1f); // Adjust time as needed

            WinOrLoseCalculator();// reveal all the objects and calculate total winnings
            Debug.Log("<color=blue> Iteration</color>");
            yield return new WaitForSeconds(1.2f); // Adjust time as needed
            GameManager.Instance.minesManager.DestroyAllTheObjects();
            GameManager.InstantiatedGridObjects.Clear();
            selectedGridElements.Clear();

        }
        if (stopRequested)
        {

            MinesManager.Instance.InstantiateWithoutDelay();

            yield return new WaitForSeconds(1f);
            UIManager.Instance.manualButton.interactable = true;
        }

        if (BettingManager.Instance.betAmount > BettingManager.Instance.balanceAmount)
        {
            ShowInsufficiecntBalanceMessage();
            MinesManager.Instance.InstantiateWithoutDelay();
           StopAutoBet();
        }

    }

    private IEnumerator FixedAutoBetRoutine(int numberOfRounds)
    {
        if (BettingManager.Instance.balanceAmount < 0)
        {
            MinesManager.Instance.InstantiateWithoutDelay();
            ShowInsufficiecntBalanceMessage();
           StopAutoBet();
        }
        else if (BettingManager.Instance.betAmount <= BettingManager.Instance.balanceAmount)
        {

            for (int i = 0; i < numberOfRounds && !stopRequested && (BettingManager.Instance.betAmount <= BettingManager.Instance.balanceAmount); i++)

            {
                BettingManager.Instance.balanceAmount -= BettingManager.Instance.betAmount;
                UIManager.Instance.manualButton.interactable = false;
                GameManager.Instance.minesManager.InstantiateWithoutDelay();//Instantiate elements
                MinesManager.Instance.DisableAllObjects();
                yield return new WaitForSeconds(0.4f); // Adjust time as needed

                AddElementsSelectedToList();
                MinesManager.Instance.ShowAllItems();//Reveal all the objects

                yield return new WaitForSeconds(1f); // Adjust time as needed

                WinOrLoseCalculator();// calculate total winnings


                yield return new WaitForSeconds(1f); // Adjust time as needed
                GameManager.Instance.minesManager.DestroyAllTheObjects();
                selectedGridElements.Clear();
                GameManager.InstantiatedGridObjects.Clear();
            }
            if (BettingManager.Instance.betAmount > BettingManager.Instance.balanceAmount)
            {
                ShowInsufficiecntBalanceMessage();
            }

           StopAutoBet();
            MinesManager.Instance.InstantiateWithoutDelay();
            yield return new WaitForSeconds(1f);
            UIManager.Instance.manualButton.interactable = true;
        }

    }

    #endregion Bet Iterations

    #region Add or Remove bet elements
    public void AddBetElements(GameObject gridGameObject, int index)
    {
        if (GameManager.Instance.diamondsOpened < (25 - MinesManager.Instance.totalMines))
        {
            gridGameObject.GetComponent<GridItem>().selectedForAuto = true;
            gridGameObject.GetComponent<GridItem>().autoImage.gameObject.SetActive(true);
            IndexOfSelectedElements.Add(index);

            GameManager.Instance.diamondsOpened++;
            UIManager.Instance.currentMultiplierIndex++;

            UIManager.Instance.CheckAndAdjustMultiplierPanelInAuto();

            UIManager.Instance.HighlightMultiplierPanel(UIManager.Instance.currentMultiplierIndex);


            float baseMultiplier = BettingManager.Instance.minesMultipliers[MinesManager.Instance.totalMines];
            float incrementedMultiplier = baseMultiplier + (GameManager.Instance.diamondsOpened - 1) * MinesManager.multiplierIncrement;
            winnings = incrementedMultiplier;
            Debug.Log("Current Multiplier = " + incrementedMultiplier);

        }

    }
    public void RemoveBetELements(GameObject gridGameObject, int index)
    {
        //if (GameManager.Instance.diamondsOpened < (25 - MinesManager.Instance.totalMines))
        {
            gridGameObject.GetComponent<GridItem>().autoImage.gameObject.SetActive(false);
            gridGameObject.GetComponent<GridItem>().selectedForAuto = false;
            IndexOfSelectedElements.Remove(index);
            Debug.Log("<color=pink>Bet item removed  </color>");
            UIManager.Instance.currentMultiplierIndex--;
            GameManager.Instance.diamondsOpened--;
            UIManager.Instance.CheckAndAdjustMultiplierPanelInAuto();
            UIManager.Instance.HighlightMultiplierPanel(UIManager.Instance.currentMultiplierIndex);
            if (GameManager.Instance.diamondsOpened < 1)
            {
                UIManager.Instance.startAutoPlay.interactable = false;
                GameManager.Instance.gameStarted = false;
            }

            float baseMultiplier = BettingManager.Instance.minesMultipliers[MinesManager.Instance.totalMines];
            Debug.Log("<color:Green>Base Multiplier = </color>" + baseMultiplier);
            float decrementMultiplier = baseMultiplier - (GameManager.Instance.diamondsOpened - 1) * MinesManager.multiplierIncrement;
            Debug.Log("<color:REd>decrement Multiplier  = </color>" + decrementMultiplier);
            winnings = decrementMultiplier;

        }
    }

    public void AddElementsSelectedToList()
    {
        Debug.Log("<color=purple>Index of Selected Elements count = </color>" + IndexOfSelectedElements.Count);
        for (int i = 0; i < GameManager.Instance.minesManager.allGridItems.Count; i++)
        {
            if (IndexOfSelectedElements.Contains(i))
            {
                Debug.Log("<color=yellow>Index of Selected Elements = </color>" +i);
                if (GameManager.Instance.minesManager.allGridItems[i].isMine)
                {
                    UIManager.Instance.minesBlastSound.Play();
                    GameManager.Instance.minesManager.allGridItems[i].ChangeSpriteToHighlighted();
                }
                GameManager.Instance.minesManager.allGridItems[i].ChangeSpriteToHighlighted();
                selectedGridElements.Add(GameManager.Instance.minesManager.allGridItems[i].gameObject);
            }
        }
    }

    #endregion Add or remove bet elements

    #region Bet Amount and Balance Related Functions
    private void ShowInsufficiecntBalanceMessage()
    {
        balanceFadeManager.ShowInsufficientBalancePanel();
    }
    public void WinOrLoseCalculator()
    {
        bool status = CalculateWinAmount();
        if (status == true)
        {
            UIManager.Instance.OnAutoWin();
            if (p_StopAtWinClicked == true)
            {
               StopAutoBet();
            }
            if (UIManager.Instance.IncreaseWhenWinningEnabled == true)
            {
                var TempBetAmount = BettingManager.Instance.betAmount;
                TempBetAmount += (BettingManager.Instance.betAmount * UIManager.Instance.IncreaseWhenWinningValue) / 100;
                if (TempBetAmount < BettingManager.Instance.balanceAmount)
                {
                    BettingManager.Instance.betAmount = TempBetAmount;
                    BettingManager.Instance.UpdateBetAmountIfIncrease(TempBetAmount);
                }
            }
            else
            {
                UIManager.Instance.ResettingWhenIncrease();
            }
        }
        else
        {
            BettingManager.Instance.UpdateBalanceText();
            if (UIManager.Instance.IncreaseWhenLosingEnabled == true)
            {
                var TempBetAmount = BettingManager.Instance.betAmount;
                TempBetAmount += (BettingManager.Instance.betAmount * UIManager.Instance.IncreaseWhenLosingValue) / 100;
                if (TempBetAmount < BettingManager.Instance.balanceAmount)
                {
                    BettingManager.Instance.betAmount = TempBetAmount;
                    BettingManager.Instance.UpdateBetAmountIfIncrease(TempBetAmount);
                }
            }
            else
            {
                UIManager.Instance.ResettingWhenLosing();
            }
        }
    }

    bool CalculateWinAmount()
    {
        winAmount = BettingManager.Instance.betAmount * winnings;
        foreach (GameObject items in selectedGridElements)
        {
            if (items.GetComponent<GridItem>().isMine)
            {
                return false;
            }
        }
        return true;
    }

}
    #endregion Bet Amount and balance related functions