using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CatfishManager : MonoBehaviour
{
    public static CatfishManager Instance { get; private set; }
    public bool[] isCatfishProfile; 
    public int CurrentProfileIndex { get; private set; } = 0;
    public int maxCatfishCount = 5;
    public List<string> GetProfileDecisions()
    {
        List<string> decisions = new List<string>();
        foreach (var isCatfish in isCatfishProfile)
        {
            decisions.Add(isCatfish ? "Catfish profile" : "Real profile");
        }
        return decisions;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////            Start And Awake Methods                     //////////////////////
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
    private void Start()
    {
        isCatfishProfile = new bool[25]; 
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////    Assigns catfish profiles randomly based on the maxCatfishCount setting    ////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void AssignCatfishProfiles()
    {
        // Reset all profiles to not be catfish
        for (int i = 0; i < isCatfishProfile.Length; i++)
        {
            isCatfishProfile[i] = false;
        }

        // Randomly assign maxCatfishCount profiles to be catfish
        int catfishCount = 0;
        while (catfishCount < maxCatfishCount)
        {
            int randomIndex = UnityEngine.Random.Range(0, isCatfishProfile.Length);
            if (!isCatfishProfile[randomIndex]) // If not already a catfish
            {
                isCatfishProfile[randomIndex] = true;
                catfishCount++;
            }
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////// Goes to the next profile, handling end of profiles by triggering cash out ///////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
/*    public void NextProfile()
    {
        CurrentProfileIndex++;
        if (CurrentProfileIndex >= isCatfishProfile.Length)
        {
            Debug.Log("Max limit reached of swiping");
           // UIManager.Instance.OnCashOutButtonPressed();

            //Reset Like/Dislike button pop-up
            UIManager.Instance.ChangeDislikeButtonSpriteAndSize(UIManager.Instance.defaultDislikeButtonSprite, new Vector2(200, 200));
            UIManager.Instance.ChangeLikeButtonSpriteAndSize(UIManager.Instance.defaultLikeButtonSprite, new Vector2(200, 200));

            UIManager.Instance.ShowHistoryPanel();
        }
    }*/
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////         Checks if the current profile is a catfish           ///////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public bool IsCurrentProfileCatfish()
    {
        if (CurrentProfileIndex < isCatfishProfile.Length)
        {
            return isCatfishProfile[CurrentProfileIndex];
        }
        return false; //Return false if the index is out of bounds as a safe default
    }
    //////////////////// Resets the profile index to 0 for a new game session ////////////////////////////
    public void ResetProfileIndex()
    {
        CurrentProfileIndex = 0;
    }
    public int GetCurrentCatfishCount()
    {
        return maxCatfishCount;
    }
}