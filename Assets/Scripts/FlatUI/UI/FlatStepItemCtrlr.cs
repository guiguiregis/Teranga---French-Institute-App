using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlatStepItemCtrlr : MonoBehaviour
{

    bool m_isActive = false;

    // Start is called before the first frame update
    void Start()
    {
    }
 
    public void Toggle(bool toggle)
    {
        m_isActive = toggle;

        if(m_isActive)
        {
            transform.GetComponent<Animation>().Play("step_item_active");
        }
        else
        {
            transform.GetComponent<Animation>().Play("step_item_inactive");
        }

    }
}
