using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class DragAndDrop : MonoBehaviour,  IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Canvas canvas;
    private RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    private RectTransform initialTransformRect;
    private Vector3 initialPosition;
    private Vector2 initialLocalPosition;
    public bool isDrop = false;
    private float horizontalLayoutSpacing = 10f;
    public bool isClicked = false;
    public bool isWrong = false;
    public Material wrongWordColorMaterial;
    public Material rightWordColorMaterial;
    public AudioSource m_feedBackSFX;
    public AudioClip m_correctSFX;
    public AudioClip m_incorrectSFX;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        initialTransformRect = this.GetComponent<RectTransform>();
        initialLocalPosition = transform.localPosition;
        initialPosition = transform.parent.position;

    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        isClicked = true;
        if (!isDrop)
        {
            canvasGroup.alpha = .7f;
        }
        canvasGroup.blocksRaycasts = false;
     
    }

    public void OnDrag(PointerEventData eventData)
    {
        //DRAGGING THE TEXT BOX 
        if(!isDrop)
        {
            transform.position = eventData.position;
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        if(!isDrop)
        {
            InitPosition();
        }
        canvasGroup.blocksRaycasts = true;
        if (isWrong && isClicked)
        {
            //this.GetComponent<Image>().material = wrongWordColorMaterial;
            this.GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.BAD_ANWSER].outlineColor; 
            this.transform.GetChild(0).GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.BAD_ANWSER].primaryColor; 
            this.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.BAD_ANWSER].textColor; 
            m_feedBackSFX.clip = m_incorrectSFX;    //assign the incorrect SFX clip
            m_feedBackSFX.Play();   //play the current assign clip
        }
        if(isDrop)
        {
            //this.GetComponent<Image>().material = rightWordColorMaterial;
            this.GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.GOOD_ANWSER].outlineColor;
            this.transform.GetChild(0).GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.GOOD_ANWSER].primaryColor;
            this.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.GOOD_ANWSER].textColor;
            m_feedBackSFX.clip = m_correctSFX;    //assign the correct SFX clip
            m_feedBackSFX.Play();   //play the current assign clip
        }
    }

   


    public void InitPosition()
    {
        GameObject missingWordsContainer = GameObject.Find("Missing_words_container");
        float addedSpacing = Random.Range(0.01f, 0.011f);
        missingWordsContainer.GetComponent<HorizontalLayoutGroup>().spacing = horizontalLayoutSpacing + addedSpacing;
    }

}
