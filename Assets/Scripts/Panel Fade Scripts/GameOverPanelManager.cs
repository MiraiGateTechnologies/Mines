using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanelManager : MonoBehaviour
{
   /* public Image fadePanel;
    public TextMeshProUGUI 
*//*     float fadeDuration = 1.0f;
    public float stayDuration = 2.0f;*//*

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////            Initializes the panel to be transparent at the start            ////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private void Start()
    {
        fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 0);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////        Triggers the fading in and then fading out of the game over panel         /////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void ShowGameOver()
    {
        StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        // Fade in
        float startTime = Time.time;
        while (Time.time - startTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0, 1, (Time.time - startTime) / fadeDuration);
            fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, alpha);
            yield return null;
        }

        // Stay for the specified duration
        yield return new WaitForSeconds(stayDuration);

        // Fade out
        startTime = Time.time;
        while (Time.time - startTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, (Time.time - startTime) / fadeDuration);
            fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, alpha);
            yield return null;
        }
        fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 0f);
    }*/
}