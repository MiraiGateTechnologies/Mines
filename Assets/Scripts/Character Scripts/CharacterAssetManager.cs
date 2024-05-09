using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;

[Serializable]
public class CharacterAssetManager : MonoBehaviour
{
    public CharacterNames characterNames;
    public CharactersDescriptions charactersDescriptions;
    public List<AssetObject> assetObjects;
    void Start()
    {
        LoadCharacterNames();
        LoadCharacterDescriptions();
    }
    void LoadCharacterNames()
    {
        string json = Resources.Load<TextAsset>("names").text;
        characterNames = JsonUtility.FromJson<CharacterNames>(json);
    }
    void LoadCharacterDescriptions()
    {
        string json = Resources.Load<TextAsset>("descriptions").text;
        charactersDescriptions = JsonUtility.FromJson<CharactersDescriptions>(json);
    }


}


[Serializable]
public class AssetObject
{
    public int assetID;
    public List<AssetContainer> assetlist;
}

[Serializable]
public class AssetContainer
{
    public int styleID;
    public Sprite assetItem;
}

[Serializable]
public class CharacterNames
{
    [SerializeField]
    public List<string> names;
}

[Serializable]
public class CharactersDescriptions
{
    [SerializeField]
    public List<string> descriptions;
}
