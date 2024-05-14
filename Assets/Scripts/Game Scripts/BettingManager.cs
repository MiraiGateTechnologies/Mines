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
    public List<float> nextMultipliers = new List<float>();

    public Dictionary<int, float> minesMultipliers = new Dictionary<int, float>()
    {
        {5, 1.03f}, {6, 1.08f}, {7, 1.13f}, {8, 1.19f},
        {9, 1.25f}, {10, 1.32f}, {11, 1.40f}, {12, 1.48f},
        {13, 1.58f}, {14, 1.70f}, {15, 1.83f}, {16, 1.98f},
        {17, 2.16f}, {18, 2.38f}, {19, 2.64f}, {20, 2.97f},
        {21, 3.39f}, {22, 3.96f}, {23, 4.75f}, {24, 5.94f}
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
        betAmount += 5;
        betAmount = Mathf.Min(betAmount, balanceAmount);
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
        betAmount = balanceAmount;
        UpdateBetAmountInput();
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
    public void UpdateToBeAddedAmntText(float amount)
    {
        totalWinnings = amount; 
        Debug.Log("Total Winnings = " + totalWinnings);
        toBeAddedAmntText.text = totalWinnings.ToString("0.00");
    }
    // Adds the total winnings to the balance and updates the display
    public void CashOutWinnings()
    {
        balanceAmount += totalWinnings;
        UpdateBalanceText();
      //  ResetTotalWinnings();
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
        float baseMultiplier = minesMultipliers[currentMinesCount];
        nextMultipliers.Clear(); // Clear the list before adding new values

        for (int i = 0; i < 20; i++)
        {
            float multiplier = baseMultiplier + (i * MinesManager.Instance.multiplierIncrementValue);
            nextMultipliers.Add(multiplier);
        }

        // Debug log to verify the calculations
        Debug.Log("Next 20 Multipliers: " + string.Join(", ", nextMultipliers));
    }
    public void ResetMultipliers()
    {
        // Reset the multipliers to their initial state or any desired state
        nextMultipliers.Clear();
        // Optionally, re-calculate or re-initialize the multipliers as needed
        CalculateNextMultipliers(); // If you want to reset to the initial state of multipliers
    }

}