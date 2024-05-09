using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine.TextCore.Text;

public class CharacterGenerator : MonoBehaviour
{
    public CharacterAssetManager assetManager;

    private GameObject WomenImagePlaceHolder;

    private TextMeshProUGUI nameText;
    private TextMeshProUGUI descriptionText;

    //To store the list of women images - to show in hisr
    public List<Sprite> sessionImages = new List<Sprite>();

    public void ClearSessionImages()
    {
        sessionImages.Clear();
    }

    public List<Sprite> GetSessionImages()
    {
        return sessionImages;
    }



    public List<int> currentObject;
    public List<HistoryObject> history;
    public HashSet<int> usedImageIds = new HashSet<int>();

    public enum ASSET_TYPE
    {
        WOMENIMAGE,
    }

    #region Placeholders
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////   Sets the placeholders and text components for the character's assets    //////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void SetPlaceHolders(GameObject _WomenImagePlaceHolder
        
        /*TextMeshProUGUI _nameText,
        TextMeshProUGUI _descriptionText*/)
    {
        WomenImagePlaceHolder = _WomenImagePlaceHolder;
        /*nameText = _nameText;
        descriptionText = _descriptionText;*/
    }
    private void Start()
    {
        currentObject = new List<int>();
        history = new List<HistoryObject>();
    }
    #endregion Placeholders

    #region Generate Character
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////   Generates a new character with random attributes and updates the UI accordingly  /////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void GenerateNewCharacter(CharacterBodyHolder characterBodyHolder)
    {
        currentObject.Clear();

        GetRandomCharectorGenrator(WomenImagePlaceHolder, ASSET_TYPE.WOMENIMAGE);

        // Assign a random name
        if (assetManager.characterNames.names.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, assetManager.characterNames.names.Count);
            string selectedName = assetManager.characterNames.names[randomIndex];
            characterBodyHolder.nameText.text = selectedName;
        }
        else
        {
            Debug.LogError("No names loaded.");
        }


        if (assetManager.charactersDescriptions.descriptions.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, assetManager.charactersDescriptions.descriptions.Count);
            string selectedDescription = assetManager.charactersDescriptions.descriptions[randomIndex];
            characterBodyHolder.descriptionText.text = selectedDescription;
        }
        else
        {
            Debug.LogError("No descriptions loaded.");
        }



        /*List<int> historyObjectCopy = new List<int>(currentObject);
        int randomNameIndex;
        string selectedName = GetRandomStringFromList(assetManager.characterNames.names, out randomNameIndex);
        int randomDescriptionIndex;
        string selectedDescription = GetRandomStringFromList(assetManager.charactersDescriptions.descriptions, out randomDescriptionIndex);
        nameText.text = selectedName;
        descriptionText.text = selectedDescription;
        history.Add(new HistoryObject(randomNameIndex, randomDescriptionIndex, historyObjectCopy));*/
    }
    #endregion Generate Character

    #region GetRandomStringFromList
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////  Selects a random string from a list and returns the selected string along with its index /////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    string GetRandomStringFromList(List<string> list, out int randomIndex)
    {
        if (list == null || list.Count == 0)
        {
            randomIndex = -1;
            return null;
        }
        int seed = DateTime.Now.Millisecond;
        UnityEngine.Random.InitState(seed);
        randomIndex = UnityEngine.Random.Range(0, list.Count);
        string selectedName = list[randomIndex];
        return selectedName;
    }
    #endregion GetRandomStringFromList

    #region GetRandomCharectorGenrator
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////   Generates a random character asset for a given placeholder and asset type   ////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void GetRandomCharectorGenrator(GameObject placeholder, ASSET_TYPE _TYPE)
    {
        

        var availableAssets = assetManager.assetObjects.Find(q => q.assetID == (int)_TYPE)?.assetlist
            .Where(a => !usedImageIds.Contains(a.styleID))
            .ToList();

        if (availableAssets == null || availableAssets.Count == 0)
        {
            Debug.LogError("No available assets to choose from. All images have been used.");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, availableAssets.Count);
        var selectedAsset = availableAssets[randomIndex];

        sessionImages.Add(selectedAsset.assetItem);
        // Add the selected asset's ID to the usedImageIds to prevent it from being selected again
        usedImageIds.Add(selectedAsset.styleID);

        placeholder.GetComponent<Image>().sprite = selectedAsset.assetItem;

        
    }

    /*{

        currentObject.Add((int)assetManager.assetObjects.Find(q => q.assetID == (int)_TYPE)?.assetlist.OrderBy(q => Guid.NewGuid()).First().styleID);
        placeholder.GetComponent<Image>().sprite = assetManager.assetObjects[(int)_TYPE].assetlist[currentObject[(int)_TYPE]].assetItem;
    }*/

    public string GetCurrentCharacterName()
    {
        return nameText.text;
    }
    public string GetCurrentCharacterDescription()
    {
        return descriptionText.text;
    }

    #endregion GetRandomCharectorGenrator
}

#region HistoryObject Class
[Serializable]
public class HistoryObject
{
    public int nameIndex;
    public int descriptionIndex;
    public List<int> historyObjectHolder;
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //Constructs a HistoryObject with specified name and description indices and a list of character object IDs
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public HistoryObject(int nameIndex, int descriptionIndex, List<int> Obj)
    {
        this.nameIndex = nameIndex;
        this.descriptionIndex = descriptionIndex;
        historyObjectHolder = Obj;
    }
}
#endregion HistoryObject Class
