using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class ModuleScreen : MonoBehaviour
{
    private string m_module_name =  "Communication";
    public TextMeshProUGUI m_module_name_text;

    public GameObject m_trophies_container;
    public Image m_module_progress_bar;

    //LIST OF TROPHEE 
    public List<Sprite> trophees_sprite;
    public List<Sprite> trophees_shadow_sprite;
    public List<Module_thumbs_bloc> m_modules_thumbs;
    public Transform m_levels_container;
    //TROPHEE SPRITE IMAGE
    public TextMeshProUGUI m_progress_text;
    public TextMeshProUGUI m_trophy_progression_text;

    public GameObject m_options_menu_panel;
    public Button m_option_btn;

    // Unlocked Trophy section
    public GameObject m_unlockTrophyPanel;
    public Image m_unlockTrophyImage;
    public TextMeshProUGUI m_unlockTrophyLabel;
    public GameObject unlocker_popup;
    //public TextMeshProUGUI m_unlockTrophyBtnText;


    //TROPHEE SPRITES INDEXES 
    public enum Trophee_index
    {
        BRONZE = 0,
        SILVER = 1,
        GOLD = 2
    }

    private string m_trophies_prefix_key = "TROPHY_CELEBRATION";

    public GameObject m_cheatsContainer;    //contains all the cheat buttons
    public GameObject m_progressManager;
    public TextMeshProUGUI m_level_anoucement;
    private string m_employee_unlocked_key = "EMPLOYEE_UNLOCKED_";
    // Start is called before the first frame update
    void Awake()
    {
        m_module_name_text.text = "Communication";
        if (StageManager.instance != null)
        {
            m_module_name =  StageManager.instance.GetString("current_module_name");
            m_module_name_text.text = StageManager.instance.m_modulesDisplayName.Where(m => m.fileName.Equals(StageManager.instance.GetString("current_module_name"))).FirstOrDefault().dislpayName;  //get the module display name based of the name written inside the JSON file
        }
        StageManager.instance.GetNumberOfAvailableLevels(m_module_name, false); //verified the number of available levels
        float progress = ProgressManager.Instance.GetModuleGlobalProgress(m_module_name);
        m_module_progress_bar.fillAmount = progress / 100;
        m_progress_text.text = (int)progress + "%";

        DeactivateAllTrophies();
        CheckUnlockedTrophies(progress);
        ToggleCheatContainer(); //verify if we are in editor mode in order to activate cheat
        SetThumbnails();
        // we should check if the player has level up 
        if (StageManager.instance.GetInt(m_progressManager.GetComponent<ProgressManager>().player_level) < m_progressManager.GetComponent<ProgressManager>().GetPlayerLevel() && !IsEmployeeKeyExist(m_employee_unlocked_key, StageManager.instance.GetInt(m_progressManager.GetComponent<ProgressManager>().player_level)))
        {
            Debug.Log("should choose between two partners");

            m_progressManager.GetComponent<ProgressManager>().m_partners = new Partners_struct(); //init partners struct
            m_progressManager.GetComponent<ProgressManager>().m_job_title_struct = new Job_title_struct();    //init job title struct
            m_progressManager.GetComponent<ProgressManager>().LoadPartners(ProgressManager.Instance.m_partner_struct_path);    //load partner datas
            m_progressManager.GetComponent<ProgressManager>().LoadJobTitles(ProgressManager.Instance.m_jobs_struct_path);    ////load job titles datas
            m_level_anoucement.text = "Bravo ! Vous avez atteint le niveau " + ProgressManager.Instance.GetPlayerLevel().ToString();
            m_progressManager.GetComponent<ProgressManager>().UnlockPartner(ProgressManager.Instance.GetPlayerLevel());
        }
    }


    private void Start()
    {
        float PlayerProgression_stats = ProgressManager.Instance.GetModuleGlobalProgress(m_module_name);

        float max = PlayerProgression_stats / 100;
        if (PlayerProgression_stats == 0)
        {
            // PlayerProgression_stats_text.gameObject.SetActive(true);
            m_module_progress_bar.fillAmount = 0f;
        }
        if (max != GetModuleLastProgress(m_module_name))
        {
            StartCoroutine(TweenPlayerProgressionStats(max));
            SetModuleLastProgress(m_module_name, max);
        }
        else
        {
            m_module_progress_bar.fillAmount = max;
        }
    }

    public void SetModuleLastProgress(string _module_name, float _max)
    {
        string key = _module_name + "_MODULE_DOTNUT_PROGRESSION";
        StageManager.instance.SetFloat(key, _max);
    }

    public float GetModuleLastProgress(string _module_name)
    {
        string key = _module_name + "_MODULE_DOTNUT_PROGRESSION";
        return StageManager.instance.GetFloat(key);
    }

    public bool IsEmployeeKeyExist(string _employee_key, int p_player_level)
    {
        string key = _employee_key + p_player_level;
        return PlayerPrefs.HasKey(key);
    }

   

    public void SetThumbnails()
    {
        Module_thumbs_bloc bloc = m_modules_thumbs.Find((e) => e.name == m_module_name);
        for (int i = 0; i < m_levels_container.childCount; i++)
        {
            Transform lvl = m_levels_container.GetChild(i);
            lvl.Find("thumbnail").GetComponent<Image>().sprite = bloc.thumbs[i];
        }
    }
    public void CheckUnlockedTrophies(float global_score)
    {
        const string BASE_UNLOCK_LABEL_TEXT = "Bravo. Vous avez obtenu la coupe";
        const string BASE_UNLOCK_BTN_TEXT = "Decrocher la coupe";
        if (global_score >= (int)ProgressManager.Trophees.BRONZE && global_score < (int)ProgressManager.Trophees.SILVER)
        {
            //PLACE BRONZE MEDAL IF THE PLAYER HAS REACHED THE REQUIRED PROGRESSION FOR THE BRONZE MEDAL
            UpdateTrophy(0, true);
            m_trophy_progression_text.text = "Prochain objectif : obtenir la coupe d'Argent";

            if (!IsKeyExist(GetTrophyKey(m_module_name, ProgressManager.Trophees.BRONZE.ToString())))    //if the key doesn't exist then we should play the trophy panel animation
            {
                m_unlockTrophyPanel.SetActive(true);
                m_unlockTrophyLabel.text = BASE_UNLOCK_LABEL_TEXT + " de Bronze";
                //m_unlockTrophyBtnText.text = BASE_UNLOCK_BTN_TEXT + " d'Argent'";
                m_unlockTrophyImage.sprite = trophees_sprite[0];
                // m_unlockTrophyPanel.transform.GetChild(0).gameObject.SetActive(true);
                // m_unlockTrophyPanel.GetComponent<Animator>().SetTrigger("Unlock_trophy_trigger");
                PlayerPrefs.SetInt(GetTrophyKey(m_module_name, ProgressManager.Trophees.BRONZE.ToString()), 1); //save the playerpref after playing the trophy panel animation
            }
        }
        else if (global_score >= (int)ProgressManager.Trophees.SILVER && global_score < 100)
        {
            //PLACE SILVER MEDAL IF THE PLAYER HAS REACHED THE REQUIRED PROGRESSION FOR THE SILVER 
            UpdateTrophy(0, true);
            UpdateTrophy(1, true);
            m_trophy_progression_text.text = "Prochain objectif : obtenir la coupe en Or";
            if (!IsKeyExist(GetTrophyKey(m_module_name, ProgressManager.Trophees.SILVER.ToString())))    //if the key doesn't exist then we should play the trophy panel animation
            {
                m_unlockTrophyPanel.SetActive(true);
                m_unlockTrophyLabel.text = BASE_UNLOCK_LABEL_TEXT + " d' Argent";
                //m_unlockTrophyBtnText.text = BASE_UNLOCK_BTN_TEXT + " en Or";
                m_unlockTrophyImage.sprite = trophees_sprite[1];
                // m_unlockTrophyPanel.transform.GetChild(0).gameObject.SetActive(true);
                // m_unlockTrophyPanel.GetComponent<Animator>().SetTrigger("Unlock_trophy_trigger");
                PlayerPrefs.SetInt(GetTrophyKey(m_module_name, ProgressManager.Trophees.SILVER.ToString()), 1); //save the playerpref after playing the trophy panel animation
            }

        }
        else if (global_score >= (int)ProgressManager.Trophees.GOLD)
        {
            //PLACE GOLD MEDAL IF THE PLAYER HAS REACHED THE REQUIRED PROGRESSION FOR THE GOLD MEDAL
            UpdateTrophy(0, true);
            UpdateTrophy(1, true);
            UpdateTrophy(2, true);
            m_trophy_progression_text.text = "Bravo ! Vous avez obtenu tous les trophées";

            if (!IsKeyExist(GetTrophyKey(m_module_name, ProgressManager.Trophees.GOLD.ToString())))    //if the key doesn't exist then we should play the trophy panel animation
            {
                m_unlockTrophyPanel.SetActive(true);
                m_unlockTrophyLabel.text = BASE_UNLOCK_LABEL_TEXT + " en Or";
                m_unlockTrophyImage.sprite = trophees_sprite[2];
                //m_unlockTrophyBtnText.transform.parent.gameObject.SetActive(false);
                // m_unlockTrophyPanel.transform.GetChild(0).gameObject.SetActive(true);
                // m_unlockTrophyPanel.GetComponent<Animator>().SetTrigger("Unlock_trophy_trigger");
                PlayerPrefs.SetInt(GetTrophyKey(m_module_name, ProgressManager.Trophees.GOLD.ToString()), 1); //save the playerpref after playing the trophy panel animation
            }
        }
        else
        {
            m_trophy_progression_text.text = "Prochain objectif : obtenir la coupe de Bronze";
            if(global_score == 0)
            {
                // m_trophy_progression_text.text = "Aucun trophee";
            }
        }
         
    }

    public void ToggleCheatContainer()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        m_cheatsContainer.gameObject.SetActive(true);
#else
            m_cheatsContainer.gameObject.SetActive(false);
#endif
        //if (Application.isEditor)
        //{
        //    m_cheatsContainer.gameObject.SetActive(true);
        //}
        //else
        //{
        //    m_cheatsContainer.gameObject.SetActive(false);
        //}
    }

    public void CloseUnlockedTrophyPanel()
    {
        m_unlockTrophyPanel.SetActive(false);
    }

    string GetTrophyKey(string _module_name, string _trophy_type)
    {
        return m_trophies_prefix_key + _module_name + _trophy_type;
    }

    bool IsKeyExist(string _key)
    {
        if(PlayerPrefs.HasKey(_key))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void UpdateTrophy(int index, bool activate)
    {
        Color c = new Color(1f, 1f,1f, 0.60f);
        m_trophies_container.transform.GetChild(index).GetComponent<Image>().sprite = trophees_shadow_sprite[index];
        if (activate)
        {
            c = new Color(1f, 1f,1f, 1f);
            m_trophies_container.transform.GetChild(index).GetComponent<Image>().sprite = trophees_sprite[index];
        }

        m_trophies_container.transform.GetChild(index).GetComponent<Image>().color = c;
    }

    void DeactivateAllTrophies()
    {
        for (int i = 0; i < 3; i++)
        {
            UpdateTrophy(i, false);
        }
    }

    public void ToggleOptionMenu(bool status)
    {
        m_options_menu_panel.SetActive(status);
        m_option_btn.gameObject.SetActive(!status);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu_screen", LoadSceneMode.Single);
    }

    // Shows the unlocked partner
    public void ShowUnlockedPartner()
    {
        // Debug.Log("Show : " + partner_scrob_index);
        //unlocked_partner_prefab.transform.GetComponent<PartnerCtrl>().UpdatePrefab(partner_scrob_index, true ,name, title);
        unlocker_popup.transform.GetComponent<Animation>().Play("unlocker_box_slide_in");
    }

    IEnumerator TweenPlayerProgressionStats(float max)
    {


        float progress = 0f;
        float progress_fill = 0f;
        float duration = 3;
        float step = max / duration;

        while (progress < max)
        {
            progress += (step * Time.deltaTime);
            progress_fill += (step * Time.deltaTime);
            m_module_progress_bar.fillAmount = progress_fill;

            if (progress_fill >= 1)
            {
                // PlayerProgression_stats_text.gameObject.SetActive(true);
                //StopCoroutine(TweenPlayerProgressionStats(0));
                m_module_progress_bar.fillAmount = 0;
                progress_fill = 0;
            }

            yield return 0;
        }

        if(max == 1f)   //if the progress bar reaches 100 then we should blocked it at 100
        {
            m_module_progress_bar.fillAmount = 1f;
        }

    }

}

[System.Serializable]
public class Module_thumbs_bloc
{
    public string name;
    public List<Sprite> thumbs;
}
