using System.Collections;
using System.Collections.Generic;
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
    List<GameObject> selectedGridElements=new List<GameObject>();
    List<int> IndexOfSelectedElements=new List<int>();
    public float winAmount;
    public bool _newSession;
    public int currentMultiplierInAuto = 0;
    public bool p_StopAtWinClicked
    {
        get
        {
            return stopAtAnyWin;
        }
        set
        {
            if(value==true)
            {
                stopAtAnyWin = value;
                UIManager.Instance.StopAtAnyWinEnable();
            }
            else
            {
                stopAtAnyWin=value;
                UIManager.Instance.StopAtAnyWinDisable();
            }
        }
    }
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

    private IEnumerator IndefiniteAutoBetRoutine()
    {
        if (BettingManager.Instance.balanceAmount <= 0 )
        {
            MinesManager.Instance.InstantiateWithoutDelay();
            ShowInsufficiecntBalanceMessage();
            StopAutoBet();
        }


        while (!stopRequested && BettingManager.Instance.betAmount <= BettingManager.Instance.balanceAmount)
        {
            BettingManager.Instance.balanceAmount -= BettingManager.Instance.betAmount;


            BettingManager.Instance.UpdateBalanceText();
            GameManager.Instance.minesManager.InstantiateWithoutDelay();//Instantiate elements
            MinesManager.Instance.SetInteractableOffGridItems();

            yield return new WaitForSeconds(0.4f); // Adjust time as needed
            MinesManager.Instance.ShowAllItems();//Reveal all the objects

            AddElementsSelectedToList();

            yield return new WaitForSeconds(1f); // Adjust time as needed

            autoPlayStart();// reveal all the objects and calculate total winnings

            yield return new WaitForSeconds(1f); // Adjust time as needed
            GameManager.Instance.minesManager.DestroyAllTheObjects();
            selectedGridElements.Clear();
            GameManager.InstantiatedGridObjects.Clear();

        }
        if (stopRequested)
        {
            MinesManager.Instance.InstantiateWithoutDelay();
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
                GameManager.Instance.minesManager.InstantiateWithoutDelay();//Instantiate elements
                MinesManager.Instance.SetInteractableOffGridItems();
                yield return new WaitForSeconds(0.4f); // Adjust time as needed

                AddElementsSelectedToList();
                MinesManager.Instance.ShowAllItems();//Reveal all the objects

                yield return new WaitForSeconds(1f); // Adjust time as needed

                autoPlayStart();// calculate total winnings


                yield return new WaitForSeconds(1f); // Adjust time as needed
                GameManager.Instance.minesManager.DestroyAllTheObjects();
                selectedGridElements.Clear();
                GameManager.InstantiatedGridObjects.Clear();
            }
            if(BettingManager.Instance.betAmount > BettingManager.Instance.balanceAmount)
            {
                ShowInsufficiecntBalanceMessage();
            }
            StopAutoBet();
            MinesManager.Instance.InstantiateWithoutDelay();
        }
      
    }

    private void ShowInsufficiecntBalanceMessage()
    {
        var balanceFadeManager = FindObjectOfType<BalanceFadeManager>();
        balanceFadeManager.ShowInsufficientBalancePanel();
    }

    public void StopAutoBet()
    {
        BettingManager.Instance.betAmountInput.interactable = true;
        p_NewAutoBetSession = false;
        IndexOfSelectedElements.Clear();
        UIManager.Instance.AutoBetUiInteractableSet(true);
        UIManager.Instance.StopAutoPlayButtonSet(false);
        UIManager.Instance.StartAutoPlaySet(true);
        UIManager.Instance.startAutoPlay.interactable = false;
        UIManager.Instance.manualButton.interactable = true;

        GameManager.Instance.ResetMinesTracker();
        // minesManager.ResetTotalMinesCount();
        BettingManager.Instance.ResetTotalWinnings();
        BettingManager.Instance.UpdateBalanceText();

        BettingManager.Instance.ResetMultipliers();
        UIManager.Instance.ResetMultiplierPanelsToDefault();
        UIManager.Instance.UpdateMultiplierPanels();
        UIManager.Instance.EnableUIWhenGameEnded.Invoke();


    }

    public void AddBetElements(GameObject gridGameObject, int index)
    {
        if (GameManager.Instance.diamondsOpened < (25 - MinesManager.Instance.totalMines))
        {
            gridGameObject.GetComponent<GridItem>().selectedForAuto = true;
            gridGameObject.GetComponent<GridItem>().autoImage.gameObject.SetActive(true);
            IndexOfSelectedElements.Add(index);
            //gridGameObject.GetComponent<Button>().interactable = false;
            GameManager.Instance.diamondsOpened++;
            UIManager.Instance.currentMultiplierIndex++;
           // UIManager.Instance.CheckAndAdjustMultiplierPanels();

            UIManager.Instance.CheckAndAdjustMultiplierPanelInAuto();
            UIManager.Instance.HighlightMultiplierPanel(UIManager.Instance.currentMultiplierIndex);


            float baseMultiplier = BettingManager.Instance.minesMultipliers[MinesManager.Instance.totalMines];
            float incrementedMultiplier = baseMultiplier + (GameManager.Instance.diamondsOpened-1) * MinesManager.multiplierIncrement;
            winnings = incrementedMultiplier;
            Debug.Log("Current Multiplier = " + incrementedMultiplier);

        }

    //    selectedGridElements.Add(gridGameObject);
    }
    public void RemoveBetELements(GameObject gridGameObject, int index) 
    {
        if (GameManager.Instance.diamondsOpened < (25 - MinesManager.Instance.totalMines))
        {
            gridGameObject.GetComponent<GridItem>().autoImage.gameObject.SetActive(false);
            gridGameObject.GetComponent<GridItem>().selectedForAuto = false;
            IndexOfSelectedElements.Remove(index);
            UIManager.Instance.currentMultiplierIndex--;
            GameManager.Instance.diamondsOpened--;
            UIManager.Instance.CheckAndAdjustMultiplierPanelInAuto();
            UIManager.Instance.HighlightMultiplierPanel(UIManager.Instance.currentMultiplierIndex);
            if(GameManager.Instance.diamondsOpened<1)
            {
                UIManager.Instance.startAutoPlay.interactable = false;
                GameManager.Instance.gameStarted = false;
            }

            float baseMultiplier = BettingManager.Instance.minesMultipliers[MinesManager.Instance.totalMines];
            Debug.Log("<color:Green>Base Multiplier = </color>" + baseMultiplier);
            float decrementMultiplier = baseMultiplier-(GameManager.Instance.diamondsOpened-1) * MinesManager.multiplierIncrement ;
            Debug.Log("<color:REd>decrement Multiplier  = </color>" + decrementMultiplier);
            winnings = decrementMultiplier;

        }
    }

    public void AddElementsSelectedToList()
    {
        for(int i=0;i<GameManager.Instance.minesManager.allGridItems.Count;i++)
        {
            if(IndexOfSelectedElements.Contains(i))
            {
                if (GameManager.Instance.minesManager.allGridItems[i].isMine)
                {
                    UIManager.Instance.minesBlastSound.Play();
                }
                GameManager.Instance.minesManager.allGridItems[i].ChangeSpriteToHighlighted();
                selectedGridElements.Add(GameManager.Instance.minesManager.allGridItems[i].gameObject);
            }
        }
    }

    public void autoPlayStart()
    {
        bool status= CalculateWinAmount();
        if(status==true)
        {
            UIManager.Instance.OnAutoWin();
            if(p_StopAtWinClicked==true)
            {
                StopAutoBet();
            }
            if(UIManager.Instance.IncreaseWhenWinningEnabled==true)
            {
                var TempBetAmount = BettingManager.Instance.betAmount;
                TempBetAmount += (BettingManager.Instance.betAmount * UIManager.Instance.IncreaseWhenWinningValue) / 100;
                if(TempBetAmount < BettingManager.Instance.balanceAmount)
                {
                    BettingManager.Instance.betAmount= TempBetAmount;
                    BettingManager.Instance.UpdateBetAmountIfIncrease(TempBetAmount);
                }
            }
        }
        else
        {
          //  UIManager.Instance.minesBlastSound.Play();
         //   BettingManager.Instance.balanceAmount -= BettingManager.Instance.betAmount;
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
        }
    }

    public void HideAutoSprite()
    {
        foreach (GameObject items in selectedGridElements)
        {
            items.GetComponent<GridItem>().autoImage.gameObject.SetActive(false);
        }
    }
    bool CalculateWinAmount()
    {
        winAmount = BettingManager.Instance.betAmount * winnings;
        foreach(GameObject items in selectedGridElements)
        {
            if (items.GetComponent<GridItem>().isMine)
            {
                return false;
            }               
        }
        return true;
    }


}