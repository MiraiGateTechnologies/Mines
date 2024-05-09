using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.TextCore.Text;
using System.Linq;
public class SwipeManager : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 initialPosition;
    private float distanceMoved;
    private bool swipeLeft;
    private Coroutine moveCardHolder;
    public Action onDestroyCard;
    private CharacterGenerator characterGenerator;

    public AudioClip swipeSound;
    private AudioSource audioSource;

    public GameObject LikeTextPopUp;
    public GameObject NopeTextPopUp;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////            Initialization           ////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Add an AudioSource component if it doesn't exist
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = swipeSound;
        UIManager.Instance.SetSwipeManager(this);

        // Find references to CharacterGenerator in the scene
        characterGenerator = GameObject.FindObjectOfType<CharacterGenerator>();
        if (characterGenerator == null)
        {
            Debug.LogError("CharacterGenerator not found in the scene!");
        }
        // Set this SwipeManager instance in the UIManager
        
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////         Handles the drag, begin drag, and end drag events          ////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void OnDrag(PointerEventData eventData) // Inherits from IDragHandler
    {
        transform.localPosition = new Vector2(transform.localPosition.x + eventData.delta.x, transform.localPosition.y);

        /////////////////////////////// Add rotation upon drag //////////////////////////////////////////
        if (transform.localPosition.x - initialPosition.x > 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(0, +20, (initialPosition.x + transform.localPosition.x) / (Screen.width / 2)));//Right of the screen
            LikeTextPopUp.gameObject.SetActive(true);
            NopeTextPopUp.gameObject.SetActive(false);

            //Change sprite of like button to PopUpLikeButtonSprite and Increase size of like button
            UIManager.Instance.ChangeLikeButtonSpriteAndSize(UIManager.Instance.popUpLikeButtonSprite, new Vector2(250, 250));
            //Change back sprite of like button to PopUpDislikeButtonSprite and Increase size of the button
            UIManager.Instance.ChangeDislikeButtonSpriteAndSize(UIManager.Instance.defaultDislikeButtonSprite, new Vector2(200, 200));
            //Disable dislike button
            UIManager.Instance.dislikeButtonGameObj.SetActive(false);

            UIManager.Instance.likeButtonGameObj.SetActive(true);
        }   
        else
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(0, -20, (initialPosition.x - transform.localPosition.x) / (Screen.width / 2)));//Left of the screen
            NopeTextPopUp.gameObject.SetActive(true);
            LikeTextPopUp.gameObject.SetActive(false);

            //Change sprite of like button to PopUpDislikeButtonSprite and Increase size of the button
            UIManager.Instance.ChangeDislikeButtonSpriteAndSize(UIManager.Instance.popUpDislikeButtonSprite, new Vector2(250, 250));
            //Change back sprite of like button to PopUpLikeButtonSprite and Increase size of like button
            UIManager.Instance.ChangeLikeButtonSpriteAndSize(UIManager.Instance.defaultLikeButtonSprite, new Vector2(200, 200));
            //Disable like button
            UIManager.Instance.likeButtonGameObj.SetActive(false);

            UIManager.Instance.dislikeButtonGameObj.SetActive(true);

        }
    }
    public void OnBeginDrag(PointerEventData eventData) // Inherits from IBeginDragHandler
    {
        //Store initial position of the drag
        initialPosition = transform.localPosition;
    }
    public void OnEndDrag(PointerEventData eventData)// Inherits from IEndDragHandler
    {
        distanceMoved = Mathf.Abs(transform.localPosition.x - initialPosition.x);
        if (distanceMoved < 0.1 * Screen.width)// If  distance moved is less than 40% of screen width
        {
            //Snap back the card
            transform.localPosition = initialPosition;
            transform.localEulerAngles = Vector3.zero;

            LikeTextPopUp.gameObject.SetActive(false);
            NopeTextPopUp.gameObject.SetActive(false);

            UIManager.Instance.ChangeDislikeButtonSpriteAndSize(UIManager.Instance.defaultDislikeButtonSprite, new Vector2(200, 200));
            UIManager.Instance.ChangeLikeButtonSpriteAndSize(UIManager.Instance.defaultLikeButtonSprite, new Vector2(200, 200));

            UIManager.Instance.likeButtonGameObj.SetActive(true);
            UIManager.Instance.dislikeButtonGameObj.SetActive(true);

        }
        else
        {
            if (transform.localPosition.x > initialPosition.x)
            {
                swipeLeft = false;
            }
            else
            {
                swipeLeft = true;
            }
            moveCardHolder = StartCoroutine(MovedCard());
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////  Manages the movement and eventual destruction of the card post-swipe  //////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private IEnumerator MovedCard()
    {

        // Notify SwipesTracker about the swipe direction
       // SwipesTracker.Instance.AddSwipe(!swipeLeft);
        /*if (swipeSound != null)
        {
            audioSource.PlayOneShot(swipeSound);
        }*/

        // Animate the card moving off-screen and fading out
        float time = 0;
        while (GetComponent<Image>().color != new Color(1, 1, 1, 0))
        {
            time += Time.deltaTime;
            if (swipeLeft)
            {
                transform.localPosition = new Vector3(Mathf.SmoothStep(transform.localPosition.x, transform.localPosition.x - Screen.width, 4 * time), transform.localPosition.y, 0);
            }
            else
            {
                transform.localPosition = new Vector3(Mathf.SmoothStep(transform.localPosition.x, transform.localPosition.x + Screen.width, 4 * time), transform.localPosition.y, 0);
            }
            GetComponent<Image>().color = new Color(1, 1, 1, Mathf.SmoothStep(1, 0, 4 * time));
            yield return null;
        }
        //CatfishManager.Instance.NextProfile();

        UIManager.Instance.ChangeDislikeButtonSpriteAndSize(UIManager.Instance.defaultDislikeButtonSprite, new Vector2(200, 200));
        UIManager.Instance.ChangeLikeButtonSpriteAndSize(UIManager.Instance.defaultLikeButtonSprite, new Vector2(200, 200));
        UIManager.Instance.likeButtonGameObj.SetActive(true);
        UIManager.Instance.dislikeButtonGameObj.SetActive(true);

        UIManager.Instance.UpdateSwipeManagerReference();

        DestroyCharacter();

        UIManager.Instance.UpdateSwipeManagerReference();

    }

    private void DestroyCharacter()
    {

        // Destroy the current card and generate a new one
        Destroy(gameObject, 0.2f);
        

        UIManager.Instance.SetSwipeManager(this);
        /*GameManager.Instance.GenerateNewCards(0);*/

        //Button Interactions
        UIManager.Instance.EnableLikeDislikeButtons();
        UIManager.Instance.ShowCancelButton(false);
        //UIManager.Instance.ShowCashOutButton(true);

    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////      Like and Dislike Button Click functionality     ////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void SwipeRight()
    {
        UIManager.Instance.DisableLikeDislikeButtons();
        StartCoroutine(SimulateSwipe(true));
    }
    public void SwipeLeft()
    { 
        UIManager.Instance.DisableLikeDislikeButtons();
        StartCoroutine(SimulateSwipe(false));
    }

    private IEnumerator SimulateSwipe(bool isRightSwipe)
    {
        // Animate the swipe action over a specified duration
        float duration = 0.3f; 
        float time = 0;
        initialPosition = transform.localPosition;  

        while (time < duration)
        {
            time += Time.deltaTime;
            float moveDistance = Mathf.Lerp(0, Screen.width, time / duration);

            // Move and rotate the card based on swipe direction
            if (isRightSwipe)
            {
                transform.localPosition = new Vector2(initialPosition.x + moveDistance, initialPosition.y);
                transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(0, -20, moveDistance / Screen.width));
            }
            else
            {
                transform.localPosition = new Vector2(initialPosition.x - moveDistance, initialPosition.y);
                transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(0, 20, moveDistance / Screen.width));
            }

            yield return null;
        }

        // End the swipe by simulating the end drag
        swipeLeft = !isRightSwipe;
        moveCardHolder = StartCoroutine(MovedCard());

    }

}