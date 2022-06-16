using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlatCharacterItemCtrlr : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler,IPointerExitHandler
{

    bool m_isSelected = false;

    Transform m_character;

    // Start is called before the first frame update
    void Start()
    {
        m_character = transform.GetChild(0);
    }

    public void OnPointerClick(PointerEventData ped)
    {
        if(!m_isSelected)
            FindObjectOfType<FlatCharacterSetupCtrlr>().SelectCharacter(transform.GetSiblingIndex());
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        if(!m_isSelected)
            m_character.GetComponent<Animation>().Play("character_item_hovered");
    }
    
    public void OnPointerExit(PointerEventData ped)
    {
        if(!m_isSelected)
            m_character.GetComponent<Animation>().Play("character_item_default");
        
    }
    
    public void Toggle(bool toggle)
    {
        m_isSelected = toggle;

        if(m_isSelected)
        {
            m_character.GetComponent<Animation>().Play("character_item_selected");
        }
        else
        {
            m_character.GetComponent<Animation>().Play("character_item_default");
        }

    }
}
