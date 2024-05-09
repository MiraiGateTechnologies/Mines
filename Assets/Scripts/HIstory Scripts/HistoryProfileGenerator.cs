using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryProfileGenerator : MonoBehaviour
{
    public GameObject historyProfilePrefab;
    private Queue<GameObject> historyProfiles = new Queue<GameObject>();
    private Transform historyPanelTransform;
    public CharacterAssetManager assetManager;
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////              Initializes the history panel transform on Awake              ////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        historyPanelTransform = transform;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //// Add a new profile to the history panel, maintaining a limited number of displayed profiles ////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void AddProfileToHistory(int nameIndex, int descriptionIndex)
    {
        GameObject newHistoryProfile = Instantiate(historyProfilePrefab, historyPanelTransform);
        HistoryHolder historyHolder = newHistoryProfile.GetComponent<HistoryHolder>();

        if (historyHolder != null)
        {
            historyHolder.nameText.text = assetManager.characterNames.names[nameIndex];
            historyHolder.descriptionText.text = assetManager.charactersDescriptions.descriptions[descriptionIndex];
        }

        historyProfiles.Enqueue(newHistoryProfile);

        if (historyProfiles.Count > 6)
        {
            GameObject oldProfile = historyProfiles.Dequeue();
            Destroy(oldProfile);
        }
    }
}
