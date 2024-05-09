using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class HistoryContainerPanel : MonoBehaviour
{
    public Image womenImageHistory;

    public GameObject SwipeDecisionPanel;
    public void UpdateSwipeDecisionPanelVisibility(bool hasBeenSwiped)
    {
        if (SwipeDecisionPanel != null)
        {
            // Only show the NumberPanel if the profile has been swiped
            SwipeDecisionPanel.SetActive(hasBeenSwiped);
        }
    }

    // Font color
    public Color textColor_RealProfile;
    public Color textColor_FakeProfile;

    //Texts
    public TextMeshProUGUI countTxt;
    public TextMeshProUGUI catfishDecisionTxt;
    public void SetTextColors(bool isCatfish)
    {
        SetCountTextColor(isCatfish);
        SetDecisionTextColor(isCatfish);
    }

    private void SetCountTextColor(bool isCatfish)
    {
        countTxt.color = isCatfish ? textColor_FakeProfile : textColor_RealProfile;
    }

    private void SetDecisionTextColor(bool isCatfish)
    {
        catfishDecisionTxt.color = isCatfish ? textColor_FakeProfile : textColor_RealProfile;
    }

    //Catfish Decision panel
    public Sprite pinkCatfishDecPanel;
    public Sprite greenCatfishDecPanel;
    public GameObject placeholderCatfishDecPanel;

    public void SetCatfishDecisionPanel(bool isCatfish)
    {
        Sprite decisionSprite = isCatfish ? pinkCatfishDecPanel : greenCatfishDecPanel;
        placeholderCatfishDecPanel.GetComponent<Image>().sprite = decisionSprite;
    }

    //Count panel
    public Sprite pinkCountPanel;
    public Sprite greenCountPanel;
    public GameObject placeholderCountPanel;

    public void SetCountPanel(bool isCatfish)
    {
        Sprite countSprite = isCatfish ? pinkCountPanel : greenCountPanel;
        placeholderCountPanel.GetComponent<Image>().sprite = countSprite;
    }

    //Swipe decision panel
    public Sprite crossImage_LeftSwipe;
    public Sprite heartImage_RightSwipe;
    public Sprite defaultSwipeDecisionImg;
    public GameObject placeholder_SwipeDecision;
    public void SetSwipeDecisionImage(bool? rightSwipe)
    {
        if (placeholder_SwipeDecision != null)
        {
            // Use the default image if rightSwipe is null (no swipe decision made)
            Sprite decisionSprite = defaultSwipeDecisionImg;
            if (rightSwipe.HasValue)
            {
                decisionSprite = rightSwipe.Value ? heartImage_RightSwipe : crossImage_LeftSwipe;
            }

            placeholder_SwipeDecision.GetComponent<Image>().sprite = decisionSprite;
        }
    }

    //Final result Panel
    public Image finalResultPanel_Fake;
    public Image finalResultPanel_Real;

    //Hightlight panel - this panel will only appear for the profiles that have not been swiped yet. 

    public Sprite pinkHighlightPanel;
    public Sprite greenHighlightPanel;
    public GameObject placeholderHightlightPanel;

    public void UpdateHighlightPanel(bool hasBeenSwiped, bool isCatfish)
    {
        if (placeholderHightlightPanel != null)
        {
            // Set the highlight panel active state based on whether the profile has been swiped
            placeholderHightlightPanel.SetActive(!hasBeenSwiped);

            // Set the correct sprite for the highlight panel based on whether the profile is a catfish
            Image highlightImage = placeholderHightlightPanel.GetComponent<Image>();
            if (highlightImage != null)
            {
                highlightImage.sprite = isCatfish ? pinkHighlightPanel : greenHighlightPanel;
            }
        }
    }


}
