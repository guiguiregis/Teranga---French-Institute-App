using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using TMPro;
public class TeamScreenCtrlr : MonoBehaviour
{

    public Image m_player_avatar;
    public TextMeshProUGUI m_player_name;
    public TextMeshProUGUI m_company_name;
    public TextMeshProUGUI m_header_company_name;
    public TextMeshProUGUI m_player_level_index;
    public Image PlayerProgression_stats_image;
    public TextMeshProUGUI m_team_members_counter;
    public List<Sprite> m_player_avatars;

    public GameObject m_partners_container;
    public GameObject m_partner_prefab;
    public List<TextMeshProUGUI> m_trophees_counters;
    public List<TextMeshProUGUI> m_medals_counters;
    public GameObject m_options_menu_panel;
    public Button m_option_btn;
    public GameObject m_partnerBoxPrefab;
    public GameObject m_progressManager;
    public GameObject m_partnerGridLayout;
    public SpriteAtlas m_partners;
    public TextMeshProUGUI m_player_turnover;

    // Start is called before the first frame update
    void Start()
    {
        m_player_level_index.text = "Niveau " + ProgressManager.Instance.GetPlayerLevel().ToString();//show current player level index
        m_progressManager.GetComponent<ProgressManager>().LoadPartners(ProgressManager.Instance.m_partner_struct_path);    //load partner datas
        m_progressManager.GetComponent<ProgressManager>().LoadJobTitles(ProgressManager.Instance.m_jobs_struct_path);    ////load job titles datas
        LoadHeaderData();
        LoadPlayer();
        LoadPlayerProgressionStats();

        //LoadPartners();

        LoadTropheesCount();
        LoadMedalsCount();
        PopulatePartnerGrid();
    }


    public void LoadHeaderData()
    {

        Progress_struct progress_data = ProgressManager.Instance.GetProgressStruct();

        string company_name = StageManager.instance.GetString("company_name");
        
        int partners_count = progress_data.partners.Count;
        // Set team partners size
        int real_partners_count = partners_count; //
        if(real_partners_count < 0) real_partners_count = 0;
        string plural = real_partners_count > 1 ? "s" : "";
        m_team_members_counter.text = real_partners_count + " Employé" + plural;
        m_player_turnover.text = ProgressManager.Instance.GetTurnoverValue() + " F";//should show the turnover 
    }
  

    public void LoadPlayer()
    {
        int player_index = ProgressManager.Instance.m_persistentCharacterIndex.Get();
        string player_name = ProgressManager.Instance.m_persistentCharacterName.Get();
        string player_company_name = ProgressManager.Instance.m_persistentCompanyName.Get();

        m_player_avatar.sprite = m_player_avatars[player_index];
        m_player_name.text = player_name;
        m_company_name.text = player_company_name;
        m_header_company_name.text = player_company_name;
    
    }


    public void LoadPlayerProgressionStats()
    {
        float PlayerProgression_stats = Mathf.Round(ProgressManager.Instance.GetPlayerGlobalProgress());


        float max = PlayerProgression_stats / 100;
        
        if (PlayerProgression_stats == 0)
        {
            PlayerProgression_stats_image.fillAmount = 0f;
        }
        else
        {

            PlayerProgression_stats_image.fillAmount = max;
        }
    }

 


    public void LoadPartners()
    {
        foreach (Transform item in m_partners_container.transform)
        {
            Destroy(item.gameObject);
        }

        Progress_struct progress_data = ProgressManager.Instance.GetProgressStruct();
        int partners_count = progress_data.partners.Count;
        // Debug.Log(partners_count);
        for(int i = 0; i < partners_count; i++)
        {
            
            Progress_partner_struct partner = progress_data.partners[i];
            // Debug.Log(partner.name);
            int partner_scrob_index = partner.index;

            string name = partner.name;
            string title = partner.status;

            // Debug.Log("partner : " + partner_scrob_index);
 
            m_partner_prefab.transform.GetComponent<PartnerCtrl>().UpdatePrefab(partner_scrob_index, true, name, title);
            Instantiate(m_partner_prefab, Vector3.zero, Quaternion.identity, m_partners_container.transform );

        }

        for(int i = partners_count; i < (m_partner_prefab.transform.GetComponent<PartnerCtrl>().partners.Count); i++)
        {
             
            int partner_scrob_index = i;

            //Debug.Log("partner : " + partner_scrob_index);
 
            m_partner_prefab.transform.GetComponent<PartnerCtrl>().UpdatePrefab(partner_scrob_index, false);
            Instantiate(m_partner_prefab, Vector3.zero, Quaternion.identity, m_partners_container.transform );

        }
    }



    public void LoadTropheesCount()
    {

       Module_trophees trophees = ProgressManager.Instance.GetGlobalTrophees();

       // Setting trophees count
       m_trophees_counters[0].text = trophees.bronze.ToString() ;
       m_trophees_counters[1].text = trophees.silver.ToString() ;
       m_trophees_counters[2].text = trophees.gold.ToString() ;

    }

    public void LoadMedalsCount()
    {

       Module_medals medals = ProgressManager.Instance.GetGlobalMedals();

       // Setting medals count
       m_medals_counters[0].text = medals.bronze.ToString() ;
       m_medals_counters[1].text = medals.silver.ToString() ;
       m_medals_counters[2].text = medals.gold.ToString() ;

    }
    

    public void ToggleOptionMenu(bool status)
    {
        m_options_menu_panel.SetActive(status);
        m_option_btn.gameObject.SetActive(!status);
    }


    public void GotoMainMenu()
    {
        SceneManager.LoadScene("Menu_screen", LoadSceneMode.Single);
    }

    public void PopulatePartnerGrid()
    {

        Progress_struct this_progress_data = ProgressManager.Instance.GetProgressStruct();   //get the progress partners so that we can see which partners the player has already unlocked
        bool hasFilledPartnerDetails = false;
        foreach (Progress_partner_struct pa in this_progress_data.partners)
        {
            hasFilledPartnerDetails = false;
            foreach (Transform child in m_partnerGridLayout.transform) //should check if the grid content has available partner box
            {
                if(child.GetComponent<PartnerBox>() != null && !hasFilledPartnerDetails)    //check if it is a partner box
                {
                    if(child.GetComponent<PartnerBox>().m_partner_profil.sprite == null)    //if the partner image is not set then the partner box is free
                    {
                        child.GetComponent<PartnerBox>().m_partner_profil.sprite = m_partners.GetSprite(GetPartnerProfilImageByName(pa.name));
                        child.GetComponent<PartnerBox>().m_partner_profil.color = Color.white;
                        child.GetComponent<PartnerBox>().m_partner_name.text = pa.name;
                        child.GetComponent<PartnerBox>().m_partner_job.text = pa.status;
                        hasFilledPartnerDetails = true; // the partner box has been filled
                    }
                }
            }

            if(!hasFilledPartnerDetails)    //if we haven't filled the partner details then we should instantiate the partner box prefab
            {
                GameObject partnerBox = Instantiate(m_partnerBoxPrefab, m_partnerGridLayout.transform);
                partnerBox.GetComponent<PartnerBox>().m_partner_name.text = pa.name;
                partnerBox.GetComponent<PartnerBox>().m_partner_job.text = pa.status;
                partnerBox.GetComponent<PartnerBox>().m_partner_profil.sprite = m_partners.GetSprite(GetPartnerProfilImageByName(pa.name));
            }
        }
    }


    public string GetPartnerProfilImageByName(string name)
    {
        return m_progressManager.GetComponent<ProgressManager>().m_partners.partners.Where(pa => pa.partner_name.Equals(name)).FirstOrDefault().image_name;
    }
}
