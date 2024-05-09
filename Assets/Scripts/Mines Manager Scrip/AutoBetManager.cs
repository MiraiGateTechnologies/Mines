using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoBetManager : MonoBehaviour
{
    public static bool stopRequested;
    int numberOfRounds;
    bool infiniteRounds = false;
    float winnings;

    public bool autoBetStarted = false;
    public bool roundWinStatus = true;
    List<GameObject> selectedGridElements=new List<GameObject>();
    List<int> IndexOfSelectedElements=new List<int>();
    public float winAmount;
    public void StartAutoBet()
    {
        roundWinStatus = true;
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
            autoPlayStart();// reveal all the objects and calculate total winnings

            yield return new WaitForSeconds(1f); // Adjust time as needed

            if(roundWinStatus)
            {
                BettingManager.Instance.totalWinnings = winAmount;
                UIManager.Instance.OnAutoWin();//Display win amount and win panel
            }

            yield return new WaitForSeconds(1f); // Adjust time as needed
            MinesManager.Instance.DestroyAllTheObjects();
            IndexOfSelectedElements.Clear();
            selectedGridElements.Clear();
            GameManager.InstantiatedGridObjects.Clear();
        }
    }

    private IEnumerator FixedAutoBetRoutine(int numberOfRounds)
    {
        for (int i = 0; i < numberOfRounds && !stopRequested; i++)
        {
            yield return new WaitForSeconds(1f); // Adjust time as needed
        }
    }

    public void StopAutoBet()
    {
        stopRequested = true;
    }

    public void AddBetElements(GameObject gridGameObject, int index)
    {
        IndexOfSelectedElements.Add(index);
        UIManager.Instance.currentMultiplierIndex++;
        UIManager.Instance.CheckAndAdjustMultiplierPanels();
        UIManager.Instance.HighlightMultiplierPanel(UIManager.Instance.currentMultiplierIndex);
        GameManager.Instance.diamondsOpened++;

        float baseMultiplier = BettingManager.Instance.minesMultipliers[MinesManager.Instance.totalMines];
        float incrementedMultiplier = baseMultiplier + (GameManager.Instance.diamondsOpened) * MinesManager.multiplierIncrement;

    //    winnings = BettingManager.Instance.betAmount * incrementedMultiplier;

        selectedGridElements.Add(gridGameObject);
    }

    public void autoPlayStart()
    {
        MinesManager.Instance.ShowAllItems();
        HideAutoSprite();
        bool status= CalculateWinAmount();
        if(status)
        {
            UIManager.Instance.OnAutoWin();
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