using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridItem : MonoBehaviour
{
    public Button gridButton;
    public Image mineImage;
    public Image BgImage;
   // public List<Image> diamondImage;
    public Image pinkDiamond;
    public Image autoImage;
    public bool isMine;
    public bool selectedForAuto = false;
    public int randomIndex;
    public static bool buttonClicked;
    private void Start()
    {
        gridButton.onClick.AddListener(ButtonClickedOnUI);
        randomIndex = Random.Range(0, 3);
    }

    private void ButtonClickedOnUI()
    {
        if (GameManager.Instance.autoBet == false)
        {
            if (isMine)
            {
                GameManager.Instance.minesManager.HandleIfMinesDisclosed(this.gameObject, mineImage);
            }
            else
            {
                GameManager.Instance.minesManager.HandleIfDiamondDisclosed(this.gameObject, pinkDiamond);
                BgImage.sprite = UIManager.Instance.HighlightedBoxSprite;
            }
        }
        else if(selectedForAuto==true)
        {
            GameManager.Instance.autoBetManager.RemoveBetELements(this.gameObject, GameManager.InstantiatedGridObjects.IndexOf(this.gameObject));
/*            autoImage.gameObject.SetActive(false);*/

        }
        else
        {
            if(GameManager.Instance.gameStarted == false)
            {
                GameManager.Instance.gameStarted = true;
                UIManager.Instance.EnableStartAutoPlayButton();
            }
/*            autoImage.gameObject.SetActive(true);*/
            GameManager.Instance.autoBetManager.AddBetElements(this.gameObject,GameManager.InstantiatedGridObjects.IndexOf(this.gameObject));


        }
    }
    public void ChangeSpriteToHighlighted()
    {
        BgImage.sprite=UIManager.Instance.HighlightedBoxSprite;
    }
}
