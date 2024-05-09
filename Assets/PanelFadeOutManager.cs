using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelFadeOutManager : MonoBehaviour
{
    public float fadeDuration = 1.0f;
    public float stayDuration = 2.0f;
    public void FadeInOut(Image images, TextMeshProUGUI texts)
    {
        StartCoroutine(IEnableDisable(images,texts));
    }

    private IEnumerator IEnableDisable(Image images, TextMeshProUGUI texts)
    {

        images.gameObject.SetActive(true);

        yield return new WaitForSeconds(stayDuration);

        images.gameObject.SetActive(false);
    }
    private IEnumerator IFadeInAndOut(Image images, TextMeshProUGUI texts)
    {
        // Fade in
        images.gameObject.SetActive(true);
        float startTime = Time.time;
        while (Time.time - startTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0, 1, (Time.time - startTime) / fadeDuration);

                    images.color = new Color(images.color.r, images.color.g, images.color.b, alpha);
                    texts.color = new Color(texts.color.r, texts.color.g, texts.color.b, alpha);

        }
    

         yield return null;
        

        // Stay for the specified duration
        yield return new WaitForSeconds(stayDuration);

        // Fade out
        startTime = Time.time;
        while (Time.time - startTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, (Time.time - startTime) / fadeDuration);

            images.color = new Color(images.color.r, images.color.g, images.color.b, alpha);
            texts.color = new Color(texts.color.r, texts.color.g, texts.color.b, alpha);

            yield return null;
        }


        images.color = new Color(images.color.r, images.color.g, images.color.b, 0f);
        texts.color = new Color(texts.color.r, texts.color.g, texts.color.b, 0f);
        images.gameObject.SetActive(false);
    }
}
