using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalanceFadeManager : MonoBehaviour
{
    public Image insufficientBalancePanel;
    public float fadeDuration = 1.0f;
    public float stayDuration = 2.0f;
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////            Initializes the panel to be transparent at the start            ////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private void Start()
    {
        // Ensure the panel is transparent initially
        insufficientBalancePanel.color = new Color(insufficientBalancePanel.color.r, insufficientBalancePanel.color.g, insufficientBalancePanel.color.b, 0);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////   Triggers the fading in and then fading out of the insufficient balance panel   /////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void ShowInsufficientBalancePanel()
    {
        StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        // Fade in effect
        float startTime = Time.time;
        while (Time.time - startTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0, 1, (Time.time - startTime) / fadeDuration);
            insufficientBalancePanel.color = new Color(insufficientBalancePanel.color.r, insufficientBalancePanel.color.g, insufficientBalancePanel.color.b, alpha);
            yield return null;
        }

        // Stay visible for the specified duration
        yield return new WaitForSeconds(stayDuration);

        // Fade out effect
        startTime = Time.time;
        while (Time.time - startTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, (Time.time - startTime) / fadeDuration);
            insufficientBalancePanel.color = new Color(insufficientBalancePanel.color.r, insufficientBalancePanel.color.g, insufficientBalancePanel.color.b, alpha);
            yield return null;
        }
    }
}