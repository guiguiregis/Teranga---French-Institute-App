using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
public class BDCarouselArrow : MonoBehaviour ,  IPointerClickHandler
{
    // Start is called before the first frame update
    int dir = 1;
    void Awake()
    {
        int arrow_index = transform.GetSiblingIndex();
        if (arrow_index == 0)
        {
            dir = -1;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
 
       Debug.Log("Touched");
       FindObjectOfType<BDInteractivity>().SelectCharacter(dir);

    }
    

  
}
