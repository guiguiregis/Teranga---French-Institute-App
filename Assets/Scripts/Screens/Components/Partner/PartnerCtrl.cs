using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartnerCtrl : MonoBehaviour
{

    private Progress_struct progress_data;

    public static int m_partner_index = 0;
    public TextMeshProUGUI m_name;
    public TextMeshProUGUI m_status;
    public Image m_avatar;
    public List<PartnerSCROB> partners;

    public GameObject m_locker;
    // Start is called before the first frame update
    void Start()
    {
   
    }

    public void UpdatePrefab(int partner_index, bool unlocked = true, string _name = null, string _title = null )
    {
            progress_data = ProgressManager.Instance.GetProgressStruct();

            string name;
            string status;
            name = partners[partner_index].name;
            status = partners[partner_index].status;
            if(_name != null) name = _name;
            if(_title != null) status = _title;
            // Player name and status are saved in progress data : So here we dont use default ScriptableObj values
           
            m_name.text = name;
            m_status.text = status;
            m_avatar.sprite = partners[partner_index].avatar; 

            m_locker.SetActive(false);

            if(!unlocked)
            {
                m_locker.SetActive(true);
            }

    }

}
