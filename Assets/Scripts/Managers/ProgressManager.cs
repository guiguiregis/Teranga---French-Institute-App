using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Kayfo;
using System.Linq;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using System.Globalization;
using UnityEngine.U2D;
using UnityEditor;

public class ProgressManager : Singleton<ProgressManager>
{

    public enum Gender
    {
        MALE,
        FEMALE
    };


    public enum Medals
    {
        BRONZE = 75,
        SILVER = 75,
        GOLD = 75
    }

    public enum Trophees
    {
        BRONZE  = 33,
        SILVER = 66,
        GOLD = 100
    }

    private string progress_struct_path = "Game_presets";
    public TextAsset m_json;

    // Defines the base number of levels to unlock before unlocking a partner
    int partn_unlocked_levels_base = 3; 
    public GameObject partner_prefab;

    //player level text
    public TextMeshProUGUI player_level_text;
    //player pref to save the player level
    public string player_level = "PLAYER_LEVEL";

    public PersistentInt m_persistentCharacterIndex = new PersistentInt("persistentCharacterIndex", 0);
    public PersistentString m_persistentCharacterName = new PersistentString("persistentCharacterName", "");
    public PersistentString m_persistentCompanyName = new PersistentString("persistentCompanyName", "");
    private const int turnoverMultiplier = 1000000;


    public Partners_struct m_partners;//list of partners
    public string m_partner_struct_path = "Partners_struct\\Partners";  //path of partners struct
    public Job_title_struct m_job_title_struct;   // list jobs
    public string m_jobs_struct_path = "Partners_struct\\Titles";  //path of partners struct
    public GameObject m_partner_box_container;
    public SpriteAtlas m_partnerSprites;
    
    private void Start()
    {
        //LoadPartners(m_partner_struct_path);    //load partner datas
        //LoadJobTitles(m_jobs_struct_path);  //load job titles datas


        // PlayerPrefs.DeleteKey("progress");
        LoadStruct();
        SaveGlobalProgress();
        //should save the player level at the start of the game
        if(!PlayerPrefs.HasKey(player_level))
        {
            StageManager.instance.SetInt(player_level, GetPlayerLevel());
        }

        if (player_level_text != null)
        {
            player_level_text.text = "LVL " + GetPlayerLevel().ToString();
        }

    }

    public void LoadStruct()
    {
        string modulePath = progress_struct_path + Path.DirectorySeparatorChar + "Progress_struct";

        m_json = Resources.Load(modulePath) as TextAsset;
    }

    public void SaveGlobalProgress()
    {
        Progress_struct json_progress_data = new Progress_struct();
        Progress_struct progress_data = ProgressManager.Instance.GetProgressStruct();
        string myJson = m_json.ToString();
        json_progress_data = JsonUtility.FromJson<Progress_struct>(myJson); //verify if the JSON file has changed so we can add new modules
        if(progress_data != null)
        {
            foreach (Progress_module_struct m in json_progress_data.modules)
            {
                if (progress_data.modules.Where(mo => mo.name.ToLower().Equals(m.name.ToLower())).FirstOrDefault() == null)
                {
                    progress_data.modules.Add(m);   //added the no existing modules
                    SaveGlobalProgress(progress_data);  //we should save the global module progress
                }
            }
        }

        if (!PlayerPrefs.HasKey("progress"))
        {
            //SAVING THE PROGRESS STRUCT INTO PLAYER PREF
            StageManager.instance.SetString("progress", m_json.ToString());
        }
    }

    public int GetModuleIndexByName(Progress_struct progress,  string module_name)
    {
        int module_index = progress.modules.FindIndex(a => a.name == module_name);
        return module_index;
    }


    public float  GetLevelProgress(string module_name, int level_index)
    {
        Progress_struct my_Progress = new Progress_struct();
        my_Progress = GetProgressStruct();
        int module_index = GetModuleIndexByName(my_Progress, module_name);
        return my_Progress.modules[module_index].levels[level_index].GetGlobalLevelScore();
    }

    public void SaveGlobalProgress(Progress_struct my_progress)
    {

        string json = JsonUtility.ToJson(my_progress);
        //SAVING THE PROGRESS STRUCT INTO PLAYER PREF
        StageManager.instance.SetString("progress", json);
    }

    public void SaveGameStructure(Game_struct _game_structure)
    {

        string json = JsonUtility.ToJson(_game_structure);
        //SAVING THE PROGRESS STRUCT INTO PLAYER PREF
        StageManager.instance.SetString("game_structure", json);
    }

    public void SaveModuleLevelProgress(string module_name, int level_index)
     {
         Progress_struct my_progress = GetProgressStruct();
         int module_index = StageManager.instance.GetModuleIndexByName(module_name);
         float global_score = StageManager.instance.GetGlobalScore(module_name, level_index);
        //IF THE GLOBAL SCORE IS GREATER THEN THE EXISTING SCORE THAN WE SHOULD 
        //REPLACE THE CURRENT SAVED SCORE
        //switch (StageManager.instance.GetLevelDifficulty(StageManager.instance.GetString("current_module_name"), StageManager.instance.GetInt("current_level_index")))

        switch(StageManager.instance.GetCurrentLevelDifficulty())
        {
            //WE SHOULD CHECK IF THE PLAYER HAS AT LEAST ONE BRONZE MEDAL BEFORE UNLOCKING THE LEVEL OTHERWISE WE JUST GIVE HIM THE BRONZE MEDAL
            case StageManager.GAME_DIFFICULTY.EASY:
                if(GetLevelNbrOfBronzeMedal(StageManager.instance.GetString("current_module_name"), StageManager.instance.GetInt("current_level_index") + 1) >= 1 && StageManager.instance.GetCurrentLevelDifficulty() == StageManager.GAME_DIFFICULTY.EASY)
                {
                    StageManager.instance.UnlockNextLevelDifficulty();
                }
                break;
            //CHECK IF THE PLAYER HAS AT LEAST ONE SILVER MEDAL BEFORE GOING TO THE NEXT LEVEL
            case StageManager.GAME_DIFFICULTY.MEDIUM:
                if (GetLevelNbrOfSilverMedal(StageManager.instance.GetString("current_module_name"), StageManager.instance.GetInt("current_level_index") + 1) >= 1 && StageManager.instance.GetCurrentLevelDifficulty() == StageManager.GAME_DIFFICULTY.MEDIUM)
                {
                    StageManager.instance.UnlockNextLevelDifficulty();
                }
                break;
            case StageManager.GAME_DIFFICULTY.HARD:
                break;
            default:
                Debug.Log("LEVEL NOT FOUND");
                break;
        }

        if (global_score > (int)Medals.BRONZE)
        {
            //IF THE GLOBAL SCORE IS GREATER THAN THE REQUIRED SCORE FOR THE BRONZE MEDAL THAN WE SHOULD
            //UNLOCK THE NEXT LEVEL
            if (global_score >= (int)Medals.BRONZE)
            {
                int next_level_index = level_index + 1;
                if(next_level_index < my_progress.modules[module_index].levels.Count)
                {   
                    // Check if not unlocked before
                    if (my_progress.modules[module_index].levels[next_level_index].unlocked == false)
                    {
                       my_progress.modules[module_index].levels[next_level_index].unlocked = true;
                    }
                }

                // 1. Check if player can unlock a partner
                int current_level_index_in_normal_base = level_index + 1;

                // a. Check if required levels reached
                int modulo = (current_level_index_in_normal_base % partn_unlocked_levels_base);

                //if (modulo == 0)
                //{
                //    // b. Check if current level not initially played : To avoid unlocking twice a partn
                //    if (my_progress.modules[module_index].levels[level_index].progress[(int)StageManager.GAME_DIFFICULTY.EASY] == 0 )
                //    {
                //        // c. Unlock then a new partn
                //        //Debug.Log("PARTN UNLOCKED !");
                //        int screen_index = PlayerPrefs.GetInt("stage_screen_index");
                //        if(screen_index == 1) // Win screen index
                //            UnlockPartner(current_level_index_in_normal_base);
                //    }
                //}

            }

            // Now update current level_index progress : Must be placed after // 1.
            my_progress.modules[module_index].levels[level_index].progress[(int)StageManager.instance.GetCurrentLevelDifficulty()] = 100;

            // Get previously saved progress data by UnlockPartner method : In order to avoid partners field overriding
            Progress_struct progress_data_after_unlocking_partner = GetProgressStruct();
            // Re-assign partners field
            my_progress.partners = progress_data_after_unlocking_partner.partners;

            SaveGlobalProgress(my_progress);
        }
     }

    public void SaveEmployeeKey(int player_level)
    {
        int level = player_level - 1;
        string key = "EMPLOYEE_UNLOCKED_" + level;
        PlayerPrefs.SetInt(key, 1);
    }

    public void DeleteEmployeeKey(Progress_struct this_progress_data)
    {
        int numbOfModule = this_progress_data.modules.Count;
        int numbOfLevels = this_progress_data.modules[0].levels.Count;
        int player_levels = numbOfModule * numbOfLevels;

        string key = "";
        for(int i = 1; i <= player_levels; i++)
        {
            key = "";
            key = "EMPLOYEE_UNLOCKED_" + i;
            if(PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
    }

    // Saves an unlocked partner
    public void SavePartner(GameObject _progressBox)
    {
        Progress_partner_struct p = new Progress_partner_struct();
        p.name = _progressBox.GetComponent<PartnerBox>().m_partner_name.text;
        p.status = _progressBox.GetComponent<PartnerBox>().m_partner_job.text;
        Progress_struct this_progress_data = GetProgressStruct();
        this_progress_data.partners.Add(p);
        SaveGlobalProgress(this_progress_data);
        SaveEmployeeKey(StageManager.instance.GetInt(player_level));
        StageManager.instance.SetInt(player_level, GetPlayerLevel());
        _progressBox.transform.parent.transform.parent.transform.parent.GetComponent<Animation>().Play("unlocker_box_zoom_out");
    }
    
    // Unlock the next unlocked partner
    public void UnlockPartner(int level_base_index)
    {
        m_partner_box_container = GameObject.Find("Partners_container");
        Progress_struct this_progress_data = GetProgressStruct();
        int partners_count = this_progress_data.partners.Count;
        //NB : In partnerPrefab partnersScrob's list we have two item (male and female) 
        //which are supposed to be the same in term of order

        int next_unlocked_partner_scrob_index = partners_count ;// + 1; // Index starts from 0 : Male and Female Profiles

        int max_available_partners = m_partners.partners.Count;
        // Check if we haven't reached max partners to unlock
        if (next_unlocked_partner_scrob_index  < max_available_partners)
        {
            Gender neededGender = level_base_index %2 == 0 ? Gender.MALE : Gender.FEMALE; //should check if the current level is odd or even
            List<Partner> sortedPartners = GetPartnerByGender(neededGender);    //get the sorted list by gender
            List<string> jobTitles = GetJobTitleByLevel(next_unlocked_partner_scrob_index); //get a job title based on the level
            int childIndex = 0;
            DesactivatePartnerBoxes();
            foreach (string job in jobTitles)
            {
                if(childIndex < sortedPartners.Count)   //we must make sure that the index is lower then the number of partners
                {
                    m_partner_box_container.transform.GetChild(childIndex).gameObject.SetActive(true);
                    m_partner_box_container.transform.GetChild(childIndex).GetComponent<PartnerBox>().m_partner_name.text = sortedPartners[childIndex].partner_name;
                    m_partner_box_container.transform.GetChild(childIndex).GetComponent<PartnerBox>().m_partner_job.text = job;
                    m_partner_box_container.transform.GetChild(childIndex).GetComponent<PartnerBox>().m_partner_profil.sprite = m_partnerSprites.GetSprite(sortedPartners[childIndex].image_name);
                    childIndex++;
                }
            }
            // Save new partner
            //p.name = random_name;
            //p.status = random_title;
            //SavePartner(p);

            //// Display on UI
            //// next_unlocked_partner_scrob.name = random_name;
            //// next_unlocked_partner_scrob.status = random_title;
            ShowUnlockedPartner();
        }

    }

    public void DesactivatePartnerBoxes()
    {
        for(int i = 0; i < m_partner_box_container.transform.childCount; i++)
        {
            m_partner_box_container.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public List<Partner> GetPartnerByGender(Gender searchedGender)
    {
        Progress_struct this_progress_data = GetProgressStruct();   //get the progress partners so that we can see which partners the player has already unlocked
        List<Partner> sortedPartner;
        sortedPartner = m_partners.partners.Where(p => p.gender.ToLower().Equals(searchedGender.ToString().ToLower())).ToList();
        List<Partner> noRedundantList = new List<Partner>();
        for(int i = 0; i <  sortedPartner.Count; i++) //for each partner, we must check if the player hasn't already unlocked it
        {
            if(this_progress_data.partners.Where(pa => pa.name.ToLower().Equals(sortedPartner[i].partner_name.ToLower())).FirstOrDefault() == null) //if the partner already exist in the progress struct then we should remove it from the list
            {
                noRedundantList.Add(sortedPartner[i]);
            }
        }
        return noRedundantList;
    }

    public List<string> GetJobTitleByLevel(int level_index)
    {
        Progress_struct this_progress_data = GetProgressStruct();   //get the progress partners so that we can see which job title the player has already unlocked
        List<Job_title> sorted_job_title = new List<Job_title>();
        int index_counter = 0;
        foreach(Job_title jo in m_job_title_struct.titles)
        {
            index_counter++;
            if (jo.max_level >= level_index)
            {
                sorted_job_title.Add(jo);
            }
            if(index_counter == m_job_title_struct.titles.Count)
            {
                sorted_job_title = m_job_title_struct.titles;
            }

        }
        //foreach (Job_title t in sorted_job_title) //for each partner, we must check if the player hasn't already unlocked it
        //{
        //    foreach(string te in t.titles_names)
        //    {
        //        if (this_progress_data.partners.Where(pa => pa.status.Equals(te)).FirstOrDefault() != null)
        //        {
        //            t.titles_names.Remove(te);
        //        }

        //    }
        //}

        for(int i = 0; i < sorted_job_title.Count; i++)
        {
            for(int j = 0; j < sorted_job_title[i].titles_names.Count;j++)
            {

                if (this_progress_data.partners.Where(pa => pa.status.Equals(sorted_job_title[i].titles_names[j])).FirstOrDefault() != null)
                {
                    sorted_job_title[i].titles_names.Remove(sorted_job_title[i].titles_names[j]);
                }
            }
        }
        List<string> threeJobs = new List<string>(); //the list which contains the 3 random job title
        int randomJobsIndex = 0;
        int randomJobIndex = 0;
        while (threeJobs.Count < 3)
        {

            randomJobsIndex = UnityEngine.Random.Range(0, sorted_job_title.Count);
            randomJobIndex = UnityEngine.Random.Range(0, sorted_job_title[randomJobsIndex].titles_names.Count);
            threeJobs.Add(sorted_job_title[randomJobsIndex].titles_names[randomJobIndex]);
        }

        return threeJobs;
    }

    // Shows the Unlocked partner in Popup
    public void ShowUnlockedPartner()
    {
            //Debug.Log(scrob_index);
            FindObjectOfType<ModuleScreen>().ShowUnlockedPartner();
   
    }

    public float GetModuleGlobalProgress(string module_name)
    {
        Progress_struct my_progress = new Progress_struct();
        my_progress = GetProgressStruct();
        int module_index = GetModuleIndexByName(my_progress, module_name);

        float global_progress = 0f;
        foreach(Progress_level_struct l in my_progress.modules[module_index].levels)
        {
           global_progress += l.GetGlobalLevelScore();
        }
        global_progress = global_progress / my_progress.modules[module_index].levels.Count;
        return global_progress;

    }

    public float GetPlayerGlobalProgress()
    {
        float player_global_progress = 0f;
        //int active_modules_counter = 0;
        //Progress_struct my_progress = GetProgressStruct();
        //foreach (Progress_module_struct m in my_progress.modules)
        //{
        //    if (m.name != "En développement")
        //    {
        //        active_modules_counter++;
        //        float module_global_progress = GetModuleGlobalProgress(m.name);
        //        player_global_progress += module_global_progress;
        //    }

        //}
        //player_global_progress = player_global_progress / active_modules_counter;
        Module_medals medals = GetGlobalMedals();
        int total_medals = medals.bronze + medals.silver + medals.gold;
        int level = 1;
        int counter = 0;
        for(int i = 0; i < total_medals; i++)
        {
            counter++;
            if(counter == 6)
            {
                level++;
                counter = 0;
            }
        }
        //get the earned medals which lead to the next level 
        player_global_progress = (float)(counter * 100)/6;


        //return nbr of medals multiplied by 100
        return player_global_progress;
    }

    public Progress_struct GetProgressStruct()
    {
        string myJson = StageManager.instance.GetString("progress");
        Progress_struct my_progress = new Progress_struct();
        try
        {
             my_progress = JsonUtility.FromJson<Progress_struct>(myJson);
        }
        catch (Exception e)
        {
            //  if there is an exception then we should recreate the progress struct
            SceneManager.LoadScene("Profile_setup_screen");
        }
        return my_progress;
    }

    public Game_struct GetGameStruct()
    {

        string myJson = StageManager.instance.GetString("game_structure");
        Game_struct game_struct = new Game_struct();
        try
        {
            game_struct = JsonUtility.FromJson<Game_struct>(myJson);
        }
        catch (Exception e)
        {
            //  if there is an exception then we should recreate the progress struct
            SceneManager.LoadScene("Profile_setup_screen");
        }
        return game_struct;
    }

    public Module_medals GetModuleMedals(string module_name)
    {
        Progress_struct my_progress = GetProgressStruct();
        int module_index = GetModuleIndexByName(my_progress, module_name);
        Module_medals medals = new Module_medals();
        int level_index = 0;
        foreach (Progress_level_struct l in my_progress.modules[module_index].levels)
        {
            if (l.progress[(int)StageManager.GAME_DIFFICULTY.EASY] >= (int)Medals.BRONZE && StageManager.instance.GetLevelDifficulty(my_progress.modules[module_index].name, level_index) == StageManager.GAME_DIFFICULTY.EASY)
            {
                medals.bronze++;
            }
            else if (l.progress[(int)StageManager.GAME_DIFFICULTY.MEDIUM] >= (int)Medals.SILVER && StageManager.instance.GetLevelDifficulty(my_progress.modules[module_index].name, level_index) == StageManager.GAME_DIFFICULTY.MEDIUM)
            {
                //if the user has unlocked the silver medal it means that he has also unlocked the bronze one
                medals.silver++;
                medals.bronze++;
            }
            else if (l.progress[(int)StageManager.GAME_DIFFICULTY.HARD] >= (int)Medals.GOLD)
            {
                medals.gold++;
                medals.silver++;
                medals.bronze++;
            }
            level_index++;
        }
        return medals;
    }


    public Module_trophees GetModuleTrophees(string module_name)
    {
        Module_trophees trophees = new Module_trophees();
        float progress = ProgressManager.Instance.GetModuleGlobalProgress(module_name);

        if (progress >= (int)ProgressManager.Trophees.BRONZE && progress < (int)ProgressManager.Medals.SILVER)
        {
            trophees.bronze++;
        }
        else if (progress >= (int)ProgressManager.Trophees.SILVER && progress < (int)ProgressManager.Medals.GOLD)
        {
            trophees.silver++;
        }
        else if (progress >= (int)ProgressManager.Trophees.GOLD)
        {
            trophees.gold++;
        }

        return trophees;
    }

    public int GetLevelNbrOfBronzeMedal(string module_name, int level_index)
    {

        Progress_struct my_progress = GetProgressStruct();
        int module_index = GetModuleIndexByName(my_progress, module_name);
        int nbr_medals = 0;
        int index = 0;
        foreach (Progress_level_struct l in my_progress.modules[module_index].levels)
        {
            index++;
            if (my_progress.modules[module_index].name.ToLower().Equals(module_name.ToLower()) && level_index == index)
            {
                if (l.progress[(int)StageManager.GAME_DIFFICULTY.EASY] >= (int)Medals.BRONZE)
                {
                    nbr_medals++;
                }
            }
        }
        return nbr_medals;
    }

    public int GetLevelNbrOfSilverMedal(string module_name, int level_index)
    {

        Progress_struct my_progress = GetProgressStruct();
        int module_index = GetModuleIndexByName(my_progress, module_name);
        int nbr_medals = 0;
        int index = 0;
        foreach (Progress_level_struct l in my_progress.modules[module_index].levels)
        {
            index++;
            if (my_progress.modules[module_index].name.ToLower().Equals(module_name.ToLower()) && level_index == index)
            {
                if (l.progress[(int)StageManager.GAME_DIFFICULTY.MEDIUM] >= (int)Medals.SILVER)
                {
                    nbr_medals++;
                }
            }
        }
        return nbr_medals;
    }

    public Module_medals GetGlobalMedals()
    {
        Progress_struct my_progress = GetProgressStruct();
        Module_medals global_medals = new Module_medals();
        foreach(Progress_module_struct m in my_progress.modules)
        {
            Module_medals module_medals = GetModuleMedals(m.name);
            global_medals.bronze += module_medals.bronze;
            global_medals.silver += module_medals.silver;
            global_medals.gold += module_medals.gold;
        }
        return global_medals;
    }


    public Module_trophees GetGlobalTrophees()
    {
        Progress_struct my_progress = GetProgressStruct();
        Module_trophees global_trophees = new Module_trophees();
        foreach(Progress_module_struct m in my_progress.modules)
        {
            Module_trophees module_trophees = GetModuleTrophees(m.name);
            global_trophees.bronze += module_trophees.bronze;
            global_trophees.silver += module_trophees.silver;
            global_trophees.gold += module_trophees.gold;
        }
        return global_trophees;
    }

    public int GetPlayerLevel()
    {
        Module_medals global_medal = GetGlobalMedals();
        int total_of_earned_medals = global_medal.bronze + global_medal.silver + global_medal.gold;
        int player_level = (total_of_earned_medals / 6) + 1;
        return player_level;
    }

    public string GetOngoingModule()
    {
        string module_name = "Communication";
        
        Progress_struct my_progress = GetProgressStruct();
        foreach (Progress_module_struct m in my_progress.modules)
        {
            if (m.name != "En développement")
            {
                float module_global_progress = GetModuleGlobalProgress(m.name);
                if(module_global_progress < 100.0f)
                {
                    module_name = m.name;
                    break;
                }
            }

        }

        return module_name;
    }

    public bool IsLevelUnlocked(string module_name, int level_index)
    {
        Progress_struct my_Progress = new Progress_struct();
        my_Progress = GetProgressStruct();
        int module_index = GetModuleIndexByName(my_Progress, module_name);
        return my_Progress.modules[module_index].levels[level_index].unlocked;
    }

    public void ResetCharacter()
    {
        m_persistentCharacterIndex.Set(0);
        m_persistentCharacterName.Set("");
        m_persistentCompanyName.Set("");
    }

    public void PassLevelCheat()
    {
        string module_name = StageManager.instance.GetString("current_module_name");
        int lvl_index = 0;
        Progress_struct my_Progress = new Progress_struct();
        my_Progress = GetProgressStruct();
        int module_index = GetModuleIndexByName(my_Progress, module_name);
        string player_pref_key = "";

        for (int i = 0; i < 6; i++)
        {
            lvl_index = i + 1;
            player_pref_key = module_name.ToUpper() + "_LEVEL_" + lvl_index.ToString() + "_DIFFICULTY";
            if (my_Progress.modules[module_index].levels[i].unlocked && !PlayerPrefs.GetString(player_pref_key).Equals("HARD"))
            {
                for(int j = 0; j < 3; j++)
                {
                        my_Progress.modules[module_index].levels[i].progress[j] = 100;
                }

                if (i < 5)
                {
                    my_Progress.modules[module_index].levels[lvl_index].unlocked = true;
                }

                player_pref_key = module_name.ToUpper() + "_LEVEL_" + lvl_index.ToString() + "_DIFFICULTY";
                
                PlayerPrefs.SetString(player_pref_key, "HARD");
                break;
            }
        }
        SaveGlobalProgress(my_Progress);
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);    //reload the current scene

    }

    public void ResetModuleProgress()
    {

        string module_name = StageManager.instance.GetString("current_module_name");
        int lvl_index = 0;
        Progress_struct my_Progress = new Progress_struct();
        my_Progress = GetProgressStruct();
        int module_index = GetModuleIndexByName(my_Progress, module_name);
        string player_pref_key = "";

        for (int i = 0; i < 6; i++)
        {
            lvl_index = i + 1;
            player_pref_key = module_name.ToUpper() + "_LEVEL_" + lvl_index.ToString() + "_DIFFICULTY";
            if (my_Progress.modules[module_index].levels[i].unlocked && !PlayerPrefs.GetString(player_pref_key).Equals("EASY"))
            {
                for (int j = 0; j < 3; j++)
                {
                    my_Progress.modules[module_index].levels[i].progress[j] = 0;
                }

                if (i < 5)
                {
                    my_Progress.modules[module_index].levels[lvl_index].unlocked = true;
                }

                player_pref_key = module_name.ToUpper() + "_LEVEL_" + lvl_index.ToString() + "_DIFFICULTY";

                PlayerPrefs.SetString(player_pref_key, "EASY");
            }
        }
        SaveGlobalProgress(my_Progress);
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);    //reload the current scene
    }

    public string GetTurnoverValue()
    {
        string value = "";
        Progress_struct my_Progress = new Progress_struct();
        my_Progress = GetProgressStruct();
        float global_progress = 0;
        float turnoverVal = 0;
        foreach(Progress_module_struct m in my_Progress.modules)
        {
            global_progress += GetModuleGlobalProgress(m.name);
        }
        global_progress = (global_progress) / my_Progress.modules.Count;
        turnoverVal = turnoverMultiplier * Mathf.Round(global_progress);
        var format = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        format.NumberGroupSeparator = " ";
        value = turnoverVal.ToString("#,0", format);
        return value;
    }

    public void LoadPartners(string path)
    {
        TextAsset json = Resources.Load(path) as TextAsset;
        if (json != null)
        {
            m_partners.partners = new List<Partner>();  //we should init the list of partner inside the struct
            m_partners = JsonUtility.FromJson<Partners_struct>(json.text);  //we should load the partners data using the given path
        }
        else
        {
            //IF THE GIVEN PATH IS NOT FOUND THEN WE SHOULD USE THE DEFAULT PATH
            Debug.LogWarning("Partner struct path not found");
        }
    }

    public void LoadJobTitles(string path)
    {

        TextAsset json = Resources.Load(path) as TextAsset;
        if (json != null)
        {
            m_job_title_struct.titles = new List<Job_title>();  //we should init the list of job titles inside the struct
            m_job_title_struct = JsonUtility.FromJson<Job_title_struct>(json.text);  //we should load the jobs data using the given path
        }
        else
        {
            //IF THE GIVEN PATH IS NOT FOUND THEN WE SHOULD USE THE DEFAULT PATH
            Debug.LogWarning("Title struct path not found");
        }
    }
}

[System.Serializable]
public class Progress_struct
{
    public List<Progress_module_struct> modules;
    public List<Progress_partner_struct> partners;
}

[System.Serializable]
public class Progress_module_struct
{
    public string name;
    public List<Progress_level_struct> levels;
}



[System.Serializable]
public class Progress_level_struct
{
    public bool unlocked;
    public List<float> progress = new List<float>(3);
    public float GetGlobalLevelScore()
    {
        return (progress[(int)StageManager.GAME_DIFFICULTY.EASY] + progress[(int)StageManager.GAME_DIFFICULTY.MEDIUM] + progress[(int)StageManager.GAME_DIFFICULTY.HARD]) / 3;
    }
}

[System.Serializable]
public class Progress_partner_struct
{
    public int index;
    public string name = "Personne";
    public string status = "CEO";
}


[System.Serializable]
public class Module_medals
{
    public int gold = 0;
    public int silver = 0;
    public int bronze = 0;
}


[System.Serializable]
public class Module_trophees
{
    public int gold = 0;
    public int silver = 0;
    public int bronze = 0;
}

