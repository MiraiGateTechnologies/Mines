using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoBetManager : MonoBehaviour
{
    public static bool stopRequested;
    int numberOfRounds;
    bool infiniteRounds = false;
    float winnings;
    public bool stopAtAnyWin = false;
    public bool autoBetStarted = false;
    public bool roundWinStatus = true;
    List<GameObject> selectedGridElements=new List<GameObject>();
    List<int> IndexOfSelectedElements=new List<int>();
    public float winAmount;
    public bool _newSession;
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
        while (!stopRequested)
        {
            GameManager.Instance.minesManager.InstantiateWithoutDelay();//Instantiate elements

            yield return new WaitForSeconds(0.4f); // Adjust time as needed
            MinesManager.Instance.ShowAllItems();//Reveal all the objects

            AddElementsSelectedToList();

            yield return new WaitForSeconds(1f); // Adjust time as needed

            autoPlayStart();// reveal all the objects and calculate total winnings

            yield return new WaitForSeconds(2f); // Adjust time as needed
            GameManager.Instance.minesManager.DestroyAllTheObjects();
            selectedGridElements.Clear();
            GameManager.InstantiatedGridObjects.Clear();
            
        }
        if(stopRequested)
        {
            MinesManager.Instance.InstantiateWithoutDelay();
        }
    }

    private IEnumerator FixedAutoBetRoutine(int numberOfRounds)
    {
        for (int i = 0; i < numberOfRounds && !stopRequested; i++)
        {
            GameManager.Instance.minesManager.InstantiateWithoutDelay();//Instantiate elements

            yield return new WaitForSeconds(0.4f); // Adjust time as needed
            MinesManager.Instance.ShowAllItems();//Reveal all the objects

            AddElementsSelectedToList();

            yield return new WaitForSeconds(1f); // Adjust time as needed

            autoPlayStart();// reveal all the objects and calculate total winnings


            yield return new WaitForSeconds(2f); // Adjust time as needed
            GameManager.Instance.minesManager.DestroyAllTheObjects();
            selectedGridElements.Clear();
            GameManager.InstantiatedGridObjects.Clear();
        }
    }

    public void StopAutoBet()
    {
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
        IndexOfSelectedElements.Add(index);
        gridGameObject.GetComponent<Button>().interactable = false;
        UIManager.Instance.currentMultiplierIndex++;
        UIManager.Instance.CheckAndAdjustMultiplierPanels();
        UIManager.Instance.HighlightMultiplierPanel(UIManager.Instance.currentMultiplierIndex);
        GameManager.Instance.diamondsOpened++;

        float baseMultiplier = BettingManager.Instance.minesMultipliers[MinesManager.Instance.totalMines];
        float incrementedMultiplier = baseMultiplier + (GameManager.Instance.diamondsOpened) * MinesManager.multiplierIncrement;
        Debug.Log("Current Multiplier = " + incrementedMultiplier);
        winnings=incrementedMultiplier;

    //    selectedGridElements.Add(gridGameObject);
    }

    public void AddElementsSelectedToList()
    {
        for(int i=0;i<GameManager.Instance.minesManager.allGridItems.Count;i++)
        {
            if(IndexOfSelectedElements.Contains(i))
            {
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