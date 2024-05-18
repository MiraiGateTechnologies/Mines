using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinPanelFadeManager : MonoBehaviour
{
    public CanvasGroup winPanelCanvasGroup; // Reference to the CanvasGroup
    public TMP_Text WinAmountTxt;
    public Button checkHistoryButton; // Reference to the "check history" button
    public float fadeDuration = 1.0f;
    public float stayDuration = 1.0f;

    private void Start()
    {
        winPanelCanvasGroup.alpha = 0; // Make the panel transparent
       // checkHistoryButton.interactable = false; // Initially, the button is not interactable
    }

    public void ShowWinPanel()
    {
        StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        yield return FadeTo(1.0f); // Fade in
        yield return new WaitForSeconds(stayDuration);
        yield return FadeTo(0.0f); // Fade out
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float startTime = Time.time;
        float startAlpha = winPanelCanvasGroup.alpha;

        while (Time.time - startTime < fadeDuration)
        {
            float elapsed = (Time.time - startTime) / fadeDuration;
            winPanelCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed);
            yield return null;
        }

        winPanelCanvasGroup.alpha = targetAlpha;

        // Update the interactability of the "check history" button
       // checkHistoryButton.interactable = (targetAlpha == 1.0f);
       // BettingManager.Instance.ResetTotalWinnings();
    }

    public void UpdateWinAmountText(string amount)
    {
        WinAmountTxt.text = amount;
    }
}
