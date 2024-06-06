using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BettingManager : MonoBehaviour
{
    public static BettingManager Instance { get; private set; }
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
    public TMP_Text balanceText;
    public TMP_Text toBeAddedAmntText;
    public TMP_InputField betAmountInput;
    public float balanceAmount = 200f;
    public float betAmount = 10f;
    public float minBetAmount = 10f;
    public float totalWinnings = 0f;
    public List<double> nextMultipliers = new List<double>();
    public int mines;
    public int Diamonds;

    public Dictionary<int, float> minesMultipliers = new Dictionary<int, float>()
    {
        {2, 1.03f}, {3, 1.08f}, {4, 1.13f}, {5, 1.19f},
        {6, 1.25f}, {7, 1.32f}, {8, 1.40f}, {9, 1.48f},
        {10, 1.58f}, {11, 1.70f}, {12, 1.83f}, {13, 1.98f},
        {14, 2.16f}, {15, 2.38f}, {16, 2.64f}, {17, 2.97f},
        {18, 3.39f}, {19, 3.96f}, {20, 4.75f}, {21, 5.94f},
        {22, 7.92f}, {23,11.88f}, {24,23.75f}
    };

    public float p_BetAmount
    {
        get
        {
            return betAmount;
        }
        set
        {

        }
    }
    private void Start()
    {
        UpdateBalanceText();
        betAmountInput.onEndEdit.AddListener(delegate { ValidateBetAmountForInputField(); });
    }
    public void StartGame()
    {
        UpdateBetAmountFromInput(); 
        UpdateBalanceText(); 
        toBeAddedAmntText.text = "0.00";
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////         Btn Onclick Methods         ////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void IncreaseBet()//Call with + button
    {
        UpdateBetAmountFromInput();
        if (balanceAmount >= minBetAmount)
        {
            betAmount += 5;
            betAmount = Mathf.Min(betAmount, balanceAmount);
        }
        UpdateBetAmountInput();

        if (UIManager.Instance.genericButtonSound != null)
        {
            UIManager.Instance.genericButtonSound.Play();
        }
    }
    public void DecreaseBet()//Call with - button
    {
        if (UIManager.Instance.genericButtonSound != null)
        {
            UIManager.Instance.genericButtonSound.Play();
        } 
        UpdateBetAmountFromInput();
        betAmount -= 5;
        betAmount = Mathf.Max(betAmount, 10f);
        UpdateBetAmountInput();
    }
    public void MinButton()//Call with min button
    {
        if (UIManager.Instance.genericButtonSound != null)
        {
            UIManager.Instance.genericButtonSound.Play();
        }
        betAmount = minBetAmount;
        UpdateBetAmountInput();
    }
    public void MaxButton()//Call with max button
    {
        if (UIManager.Instance.genericButtonSound != null)
        {
            UIManager.Instance.genericButtonSound.Play();
        }
        if (balanceAmount >= minBetAmount)
        {
            betAmount = balanceAmount;
            UpdateBetAmountInput();
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////    Validates and updates bet amount based on the user's input     ///////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void UpdateBetAmountFromInput()
    {
        float inputBet;
        if (float.TryParse(betAmountInput.text, out inputBet))
        {
            betAmount = Mathf.Clamp(inputBet, 10f, balanceAmount);
        }
        else
        {
            betAmount = 10f;
        }
        UpdateBetAmountInput();
    }
    private void UpdateBetAmountInput()
    {
        betAmountInput.text = betAmount.ToString("0.00");
    }
    public void UpdateBetAmountIfIncrease(float amount)
    {
        betAmountInput.text = amount.ToString();
    }
    public void UpdateBalanceText()
    {
        balanceText.text = balanceAmount.ToString("0.00");

        // Check if balance amount is 0 and disable/enable bet amount buttons accordingly
        if (balanceAmount <= 0)
        {
            UIManager.Instance.DisableBetAmntButtons();
            //Disable typing
            betAmountInput.readOnly = true;
        }
        else
        {
            //UIManager.Instance.EnableBetAmntButtons();
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //////  Handles the update and reset of winnings. Allows cashing out and updates the display  //////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    
    ///// Updates the total winnings and the display of potential winnings
    public void UpdateToBeAddedAmntText(double amount)
    {
        totalWinnings = (float)amount;
        Debug.Log("Total Winnings = " + totalWinnings);
        toBeAddedAmntText.text = totalWinnings.ToString("0.00");
    }
    // Adds the total winnings to the balance and updates the display
    public void CashOutWinnings()
    {
        balanceAmount += totalWinnings;
        StartCoroutine(UIManager.Instance.plusAnimationPlayAndStop(totalWinnings));
        UpdateBalanceText();
    }
    // Resets the total winnings to 0 and updates the display
    public void ResetTotalWinnings()
    {
        totalWinnings = 0f;
        Debug.Log("Reset Called");
        UpdateToBeAddedAmntText(0); 
    }
    // Returns the current total winnings
    public float GetTotalWinnings()
    {
        return totalWinnings;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ///// Validates the bet amount entered in the input field to ensure it's within allowed limits /////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void ValidateBetAmountForInputField()
    {
        float inputBet;
        if (float.TryParse(betAmountInput.text, out inputBet))
        {
            inputBet = Mathf.Clamp(inputBet, 10f, balanceAmount);
        }
        else
        {
            inputBet = 10f;
        }
        betAmount = inputBet;
        UpdateBetAmountInput();
    }

    public void CalculateNextMultipliers()
    {
        int currentMinesCount = GameManager.Instance.totalMinesCount;
        Debug.Log(currentMinesCount);
        nextMultipliers.Clear(); // Clear the list before adding new values


        int multiplierCount = 25 - currentMinesCount;
        for (Diamonds = 1; Diamonds <= multiplierCount; Diamonds++)
        {

            // Calculate the multiplier
            double multiplier = CalculateMultiplier(currentMinesCount, Diamonds);
            nextMultipliers.Add(multiplier);

        }


/*        for (int i = 0; i < (25 - MinesManager.Instance.totalMines); i++)
        {
            float multiplier = baseMultiplier + (i * MinesManager.Instance.multiplierIncrementValue);
            nextMultipliers.Add(multiplier);
        }
*/
        //Disabling panels if we increase number of mines
        if(nextMultipliers.Count < 5)
        {

            UIManager.Instance.DisableInstantiatedPanels(nextMultipliers.Count);
        }
        else
        {
            UIManager.Instance.CheckAndEnableInstantiatedPanels(nextMultipliers.Count);
        }

        // Debug log to verify the calculations
        Debug.Log("Next 20 Multipliers: " + string.Join(", ", nextMultipliers));
    }
    double CalculateMultiplier(int mines, int diamonds)
    {
        double houseEdge = 0.01;
        return (1 - houseEdge) * Combination(25, diamonds) / Combination(25 - mines, diamonds);
    }

    long Combination(int n, int r)
    {
        long result = 1;
        for (int i = 0; i < r; i++)
        {
            result *= (n - i);
            result /= (i + 1);
        }
        return result;
    }
    public void ResetMultipliers()
    {
        // Reset the multipliers to their initial state or any desired state
        nextMultipliers.Clear();
        UIManager.Instance.ResetToDefaultMultipliers();
        UIManager.Instance.NumberOfInstantiatedPanels = 5;
        CalculateNextMultipliers();
    }

}