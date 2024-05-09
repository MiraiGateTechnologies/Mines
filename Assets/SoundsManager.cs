using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundsManager : MonoBehaviour
{
    public Sprite soundOffIcon;
    public Sprite soundOnIcon;

    public Image soundImage;
    bool isSoundOn = true;
    public List<AudioSource> allSounds;
    public void OnSoundButtonClicked()
    {
        if (isSoundOn)
        {
            soundImage.sprite = soundOffIcon;
        }
        else
        {
            soundImage.sprite = soundOnIcon;
        }
        foreach (var sounds in allSounds)
        {
            sounds.mute = isSoundOn;           
        }
        isSoundOn=!isSoundOn;
    }
}
