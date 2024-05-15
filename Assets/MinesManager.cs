using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinesManager : MonoBehaviour
{
    private const float Seconds = 0.05f;
    public GameObject gridParent;
    public GameObject gridItemPrefab;
    public float totalCoins;
    public int totalMines=5;
    public float multiplierIncrementValue=0.08f;
    int totalObjects = 25;
    public List<GridItem> allGridItems = new List<GridItem>();
    public static MinesManager Instance;
    public static float multiplierIncrement=0.08f;

    private void Awake()
    {
        Instance = this;
    }
    public void InstantiateGridObjects()
    {
        StartCoroutine(InstantiateWithADelay());
    }

    IEnumerator InstantiateWithADelay()
    {
        for (int i = 0; i < totalObjects; i++)
        {
            GameObject newInstance = Instantiate(gridItemPrefab, gridParent.transform);
            newInstance.name = "gridItem" + i; // Optional: Name the instantiated object for easier identification
            allGridItems.Add(newInstance.GetComponent<GridItem>());
            GameManager.InstantiatedGridObjects.Add(newInstance);
            // Wait for the specified delay
            yield return new WaitForSeconds(Seconds); // Convert milliseconds to seconds
        }
        PlaceMinesRandomly();
    }

    public void InstantiateWithoutDelay()
    {
        for (int i = 0; i < totalObjects; i++)
        {
            GameObject newInstance = Instantiate(gridItemPrefab, gridParent.transform);
            newInstance.name = "gridItem" + i; // Optional: Name the instantiated object for easier identification
            allGridItems.Add(newInstance.GetComponent<GridItem>());
            GameManager.InstantiatedGridObjects.Add(newInstance);
        }
        PlaceMinesRandomly();
    }

    public void PlaceMinesRandomly()
    {
        foreach (var item in allGridItems)//Ensuring that these elements are not mine
        {
            item.isMine = false;
        }

        for (int i = 0; i < totalMines; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, allGridItems.Count);
            }
            while (allGridItems[randomIndex].isMine); // Ensure we don't pick an already picked item

            allGridItems[randomIndex].isMine = true;
        }
    }

    public void ShowAllItems()
    {
        foreach (var item in allGridItems)
        {
            if (item.isMine)
            {
                item.mineImage.gameObject.SetActive(true);
            }
            else
            {
                item.diamondImage[item.randomIndex].gameObject.SetActive(true);
            }
        }
    }
    public void DestroyAllTheObjects()
    {
        if (allGridItems.Count > 0)
        {
            foreach (var item in allGridItems)
            {
                Destroy(item.gameObject);
            }
            allGridItems.Clear();
        }
    }

    public void HandleIfMinesDisclosed(GameObject minesObject, Image mineImage)
    {
        if (!GameManager.Instance.gameStarted)
        {
            BettingManager.Instance.balanceAmount -= BettingManager.Instance.betAmount;
        }
            DisableAllObjects();
        mineImage.GetComponentInParent<Button>().interactable = false;
        mineImage.gameObject.SetActive(true);

        GameManager.Instance.gameOver = true;
        GameManager.Instance.GameOver();
        UIManager.Instance.minesBlastSound.Play();
    }

    public void HandleIfDiamondDisclosed(GameObject diamondObject, Image diamondImage)
    {
        GameManager.Instance.diamondsOpened++;
        if (!GameManager.Instance.gameStarted)
        {
            GameManager.Instance.gameStarted=true;
            UIManager.Instance.ShowCashOutButton(true);
            BettingManager.Instance.balanceAmount -= BettingManager.Instance.betAmount;
            BettingManager.Instance.UpdateBalanceText();
            /*BettingManager.Instance.ResetTotalWinnings();*/
        }
        if(GameManager.Instance.diamondsOpened==(25-totalMines))
        {
            UIManager.Instance.cashOutButton.onClick.Invoke();
        }
        diamondImage.GetComponentInParent<Button>().interactable = false;
        UIManager.Instance.diamondOpenSound.Play();
        diamondImage.gameObject.SetActive(true);


        UIManager.Instance.currentMultiplierIndex++;
        UIManager.Instance.CheckAndAdjustMultiplierPanels();
        UIManager.Instance.HighlightMultiplierPanel(UIManager.Instance.currentMultiplierIndex);

        float baseMultiplier = BettingManager.Instance.minesMultipliers[totalMines];
        float incrementedMultiplier = baseMultiplier + (GameManager.Instance.diamondsOpened) * multiplierIncrement;
        Debug.Log("Multiplier Current = " + incrementedMultiplier);
        float winnings = BettingManager.Instance.betAmount * incrementedMultiplier;
/*        BettingManager.Instance.ResetTotalWinnings();*/

        BettingManager.Instance.UpdateToBeAddedAmntText(winnings);

    }

    public void ResetTotalMinesCount()
    {
        totalMines = 5;
        GameManager.Instance.totalMinesCount = 5;
    }

    public void updateTotalMinesCount(int count)
    {
        totalMines  = count;
    }
    public void DisableAllObjects()
    {
       foreach(var items in allGridItems)
        {
            items.GetComponent<Button>().interactable = false;
        }
    }

    public void SetInteractableOffGridItems()
    {
        foreach (var items in allGridItems)
        {
            items.GetComponent<Button>().interactable = false;
        }
    }
}
