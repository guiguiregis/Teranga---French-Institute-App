using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using Kayfo;

public class MenuScreenCtrl : MonoBehaviour
{

    
    public enum Medals
    {
        BRONZE = 50,
        SILVER = 75,
        GOLD = 100
    }

    public GameObject modules_container;
    public GameObject module_btn_prefab;
    public GameObject profiles_container;
    public GameObject team_members_counter;
    public TextMeshProUGUI m_player_turnover;
    public TextMeshProUGUI company_placeholder;
    public Image m_player_avatar;
    public TextMeshProUGUI m_player_name;
    public TextMeshProUGUI m_company_name;
    public TextMeshProUGUI m_header_company_name;
    public TextMeshProUGUI m_player_level_index;
    public GameObject profile;
    public Image PlayerProgression_stats_image;
    public PersistentFloat m_last_player_level_progression = new PersistentFloat("PLAYER_DOTNUT_PROGRESSION", 0f);
    public GameObject PlayerProgression_stats_text;

    //Male and Female avatar
    public List<Sprite> player_avatars;
    public List<Sprite> modules_thumbnails;

    //Module btns backgrounds : GOLD SILVER BRONZE
    public List<Sprite> modules_backgrounds;
    // Trophees count
    public List<Text> trophees_counters;

    public TextMeshProUGUI m_ongoingModuleText;

    public GameObject m_options_menu_panel;
    public Button m_option_btn;

    private Progress_struct progress_data;

    string m_ongoing_module_name = "Communication";

    public GameObject m_confirmation_box;

    public GameObject m_SFXBtn;
    public GameObject m_musicBtn;
    // Start is called before the first frame update
    void Start()
    {

        // Get progress data at any time we load this screen
        progress_data = ProgressManager.Instance.GetProgressStruct();
        m_player_level_index.text = "Niveau " + ProgressManager.Instance.GetPlayerLevel().ToString();
        LoadHeaderData();

        // Loading the menu
        LoadMenu();

        // Loadplayer data
        LoadPlayer();

        // Ongoing panel
        LoadOngoingModule();

        // Loading PlayerProgression stats
        LoadPlayerProgressionStats();

    }
   
    public void LoadHeaderData()
    {

        Progress_struct progress_data = ProgressManager.Instance.GetProgressStruct();

        string company_name = StageManager.instance.GetString("company_name");
        if(company_name == null)
        {
            company_name = "Kayfo";
        }
        // Company Name
        // company_placeholder.text = company_name;


        int partners_count = progress_data.partners.Count;
        // Set team partners size
        int real_partners_count = partners_count; //
        if(real_partners_count < 0) real_partners_count = 0;
        string plural = real_partners_count > 1 ? "s" : "";
        team_members_counter.transform.GetComponent<TextMeshProUGUI>().text = real_partners_count + " Employé" + plural;
        m_player_turnover.text = ProgressManager.Instance.GetTurnoverValue() + " F";//should show the turnover 
    }

    public void LoadMenu()
    {

        foreach (Transform item in modules_container.transform)
        {
            Destroy(item.gameObject);
        }

        for(int i = 0; i < progress_data.modules.Count; i++)
        {  
            string module_name = progress_data.modules[i].name;
            Sprite module_thumb = modules_thumbnails[i];
            module_btn_prefab.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = StageManager.instance.m_modulesDisplayName.Where(m => m.fileName.Equals(module_name)).FirstOrDefault().dislpayName;    //get the display name based of the name which is written inside the JSON file

            GameObject module_btn_prefab_instance = Instantiate(module_btn_prefab, Vector3.zero, Quaternion.identity, modules_container.transform );

            if(module_name == "En développement")
            {
                module_btn_prefab_instance.transform.Find("Button").GetComponent<Button>().interactable = false;
                module_btn_prefab_instance.transform.Find("Thumbnail").GetComponent<Image>().color = Color.gray;
                module_btn_prefab_instance.SetActive(false);
            }
            else
            {
                module_btn_prefab_instance.transform.Find("Thumbnail").GetComponent<Image>().sprite = modules_thumbnails[i];

                module_btn_prefab_instance.transform.Find("Button").GetComponent<Button>().interactable = true;
                 // Getting module global progress
                int module_global_progress = (int)(ProgressManager.Instance.GetModuleGlobalProgress(module_name));

                // Setting stars ui progress

                // Setting medal ui progress
                if(module_global_progress >= (int)Medals.BRONZE && module_global_progress < (int)Medals.SILVER)
                {
                    module_btn_prefab_instance.transform.GetComponent<ModuleBtnCtrl>().SetBtnUI(0 , module_global_progress, new Color(0.92f, 0.73f, 0.01f));
                }
                else if(module_global_progress >= (int)Medals.SILVER && module_global_progress < (int)Medals.GOLD)
                {
                    module_btn_prefab_instance.transform.GetComponent<ModuleBtnCtrl>().SetBtnUI(1 ,module_global_progress, new Color(0.92f, 0.73f, 0.01f));
                }
                else if (module_global_progress >= (int)Medals.GOLD)
                {
                    module_btn_prefab_instance.transform.GetComponent<ModuleBtnCtrl>().SetBtnUI(2 ,module_global_progress, new Color(0.92f, 0.73f, 0.01f));
                }
                else //Default
                {
                    module_btn_prefab_instance.transform.GetComponent<ModuleBtnCtrl>().SetBtnUI(-1 ,module_global_progress, new Color(0.85f, 0.85f, 0.85f));
                }

            }

           
        }
    }

    


  

    public void LoadPlayer()
    {
        int player_index = ProgressManager.Instance.m_persistentCharacterIndex.Get();
        string player_name = ProgressManager.Instance.m_persistentCharacterName.Get();
        string player_company_name = ProgressManager.Instance.m_persistentCompanyName.Get();

        m_player_avatar.sprite = player_avatars[player_index];
        m_player_name.text = player_name;
        m_company_name.text = player_company_name;
        m_header_company_name.text = player_company_name;
    
    }

  
    public void LoadPlayerProgressionStats()
    {
        float PlayerProgression_stats = ProgressManager.Instance.GetPlayerGlobalProgress();

        // PlayerProgression_stats_text.transform.GetComponent<TextMeshProUGUI>().text = PlayerProgression_stats + "%";

        float max = PlayerProgression_stats/100;
        
        if (PlayerProgression_stats == 0)
        {
                // PlayerProgression_stats_text.gameObject.SetActive(true);
                PlayerProgression_stats_image.fillAmount = 0f;
        }
        if(max != m_last_player_level_progression.Get())
        {
            StartCoroutine(TweenPlayerProgressionStats(max));
            m_last_player_level_progression.Set(max);   //set the last player progression
        }
        else
        {
            PlayerProgression_stats_image.fillAmount = max;
        }

    }

    public void LoadOngoingModule()
    {
        m_ongoing_module_name = ProgressManager.Instance.GetOngoingModule();

        //m_ongoingModuleText.text = "Continue ton parcours sur le module " + m_ongoing_module_name;
    }

  IEnumerator TweenPlayerProgressionStats(float max)
  {
       
      
       float progress = 0f;
       float progress_fill = 0f;
       float duration = 3;
       float step = max / duration;

        while (progress < max )
       {
            progress += (step * Time.deltaTime);
            progress_fill += (step * Time.deltaTime);
            PlayerProgression_stats_image.fillAmount = progress_fill;
            
            if (progress_fill >= 1 )
            {
                // PlayerProgression_stats_text.gameObject.SetActive(true);
                //StopCoroutine(TweenPlayerProgressionStats(0));
                PlayerProgression_stats_image.fillAmount = 0;
                progress_fill = 0;
            }

            yield return 0;
       }

  }

public void ToggleOptionMenu(bool status)
{
    m_options_menu_panel.SetActive(status);
    m_option_btn.gameObject.SetActive(!status);

    if(m_options_menu_panel.activeInHierarchy)
        {
            if (AudioManager.Instance.m_musicActivated.Get())
            {
                m_musicBtn.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                m_musicBtn.transform.GetChild(1).gameObject.SetActive(true);
            }


            if (AudioManager.Instance.m_SFXActivated.Get())
            {
                m_SFXBtn.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                m_SFXBtn.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
}

public void ContinueModule()
{

    string module_name = m_ongoing_module_name;
    if(module_name != "En développement")
    {
        StageManager.instance.SetString("current_module_name", module_name);
        StageManager.instance.LoadModuleScreen();
    }

    StoryTellerCtrlr.Instance.InitializeStorytelling();

}

public void GotoTeamScreen()
{
      SceneManager.LoadScene("Team_screen");
}

public void DeleteTrophiesCelebrationKey()
{
        string key_prefix = "TROPHY_CELEBRATION";
        
        foreach(Game_module_struct m in StageManager.instance.game_structure.modules)
        {
            PlayerPrefs.DeleteKey(key_prefix+m.name+ ProgressManager.Trophees.BRONZE.ToString());
            PlayerPrefs.DeleteKey(key_prefix+m.name+ ProgressManager.Trophees.SILVER.ToString());
            PlayerPrefs.DeleteKey(key_prefix+m.name+ ProgressManager.Trophees.GOLD.ToString());
        }
    }

  public void Reboot()
  {
      StageManager.instance.DeleteLevelDifficultyKeys();
      PlayerPrefs.DeleteKey("progress");
      PlayerPrefs.DeleteKey("game_structure");
      DeleteTrophiesCelebrationKey();
      ProgressManager.Instance.ResetCharacter();
      ProgressManager.Instance.DeleteEmployeeKey(progress_data);
      PlayerPrefs.DeleteKey(ProgressManager.Instance.player_level);
      SceneManager.LoadScene("Boot_screen");
  }

    public void OpenConfirmationBox()
    {
        m_confirmation_box.SetActive(true);
        m_confirmation_box.GetComponent<Animator>().SetTrigger("Trigger_confirm_box");
    }


    public void CloseConfirmationBox()
    {
        m_confirmation_box.SetActive(false);

    }

}
