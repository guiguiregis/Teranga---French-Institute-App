using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class PuceCtrl : MonoBehaviour,  IPointerClickHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    public void OnPointerClick(PointerEventData eventData) {

        
       int index = transform.GetSiblingIndex();

       FindObjectOfType<BDReaderEngine>().GoToBoardIndex(index);


    }
}
