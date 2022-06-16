using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class BDControlsActivator : MonoBehaviour,  IPointerClickHandler
{
    
    public GameObject controls;

    bool is_active = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    public void OnPointerClick(PointerEventData eventData)
     {

        is_active = !is_active;
        controls.SetActive(is_active);

        if(is_active)
          StartCoroutine(I_HideControls());

    }


   public void HideControls()
   {
        is_active = false;
        controls.SetActive(is_active);
   }


   IEnumerator I_HideControls()
    {
        
         yield return new WaitForSeconds(5f);
         HideControls();

    }
   

}
