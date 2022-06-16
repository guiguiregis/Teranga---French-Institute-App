using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class WordSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string keyword;
    public Material defaultMaterial;
    public Material onHoverMaterial;
    public void OnDrop(PointerEventData eventData)
    {
        //IF THE OBJECT THAT IS BEING DRAG IS DIFFERENT FROM NULL THEN WE SET HIS POSITION TO THE SLOT POSITION
        if(eventData.pointerDrag != null)
        {
            //IF THE DRAG MISSING WORD TEXT IS EQUALS TO THE KEYWORD OF THE SLOT THEN WE CAN PLACE THE WORD TO THE SLOT 
            //OTHER WISE WE JUST RETURN THE MISSING WORD TO HIS PLACE IN THE LAYOUT
            if(eventData.pointerDrag.GetComponentInChildren<TextMeshProUGUI>().text.Equals(keyword) && !(
                eventData.pointerDrag.GetComponent<LayoutElement>().ignoreLayout))
            {
                eventData.pointerDrag.GetComponent<DragAndDrop>().isDrop = true;
                eventData.pointerDrag.GetComponent<LayoutElement>().ignoreLayout = true;
                eventData.pointerDrag.GetComponent<Transform>().position = GetComponent<Transform>().position;
                //IF THE SENTENCE IS COMPLETED WE MUST CHECK IF THE 
                //PLAYER HAS REACHED THE NUMBER OF TURN IF NOT THEN WE SHOULD GENERATE THE NEXT SENTENCE
                if(Missing_words_manager.CheckIfThePlayerHasCompletedTheSentence())
                {
                    StartCoroutine(Missing_words_manager.GenerateNextSentence());
                }
                
            }
            else
            {
                //IF THE PLAYER HAS MADE A WRONG CHOICE, WE SHOULD DECREASE HIS SCORE
                Missing_words_manager.DecreaseTheMiniGameScore();
                eventData.pointerDrag.GetComponent<DragAndDrop>().isWrong = true;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //WHEN THE USER HOVERS ON THE SLOT, WE CHANGE THE SLOT COLOR IN ORDER TO SHOW TO THE USER THAT HE CAN DROP A 
        //A WORD IN THAT POSITION
        this.GetComponent<Image>().material = onHoverMaterial;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //IF THE USER EXITS THE SLOT, WE RESET THE MATERIAL TO HIS DEFAULT VALUE
        this.GetComponent<Image>().material = defaultMaterial;
    }
}
