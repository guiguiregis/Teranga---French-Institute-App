using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;
public class EndScreen : MonoBehaviour
{
    public TextMeshProUGUI m_btn_replay_text;
    public TextMeshProUGUI m_module_name_text;
    public TextMeshProUGUI m_global_score_text;
    public GameObject m_notif_score_text;
    public TextMeshProUGUI m_finished_level_text;
    public Image m_finished_level_bg;
    public TextMeshProUGUI m_next_level_button_text;
    public List<Transform> m_games_progress;
    public List<Transform> m_difficulty_items;
    public GameObject m_difficulty_background;


    public GameObject m_minigame_thumbnail_prefab;
    public GameObject thumbnail_container;
    public Image m_global_score_bar;
    public Image m_earned_medal;
    public TextMeshProUGUI m_earned_medal_text;
    public List<Transform> m_medals_contaiers;
    public List<Sprite> m_medals_sprite;
    public GameObject m_next_level_button;
    public GameObject m_global_score_elements;
    public GameObject m_medals_image_elements;

    public GameObject unlocker_popup;
    public GameObject unlocked_partner_prefab;
    public Image m_slider_medal_image;
    public GameObject m_progressManager;
    public TextMeshProUGUI m_level_anouncement;
    enum Medal_index
    {
        NONE = 0,
        BRONZE = 1,
        SILVER = 2,
        GOLD   = 3
    }

    public int m_earned_medal_index = 0;


    enum Screens
    {
        GAMEOVER = 0,
        WIN = 1
    }

    Color LOW_COLOR = new Color(0.8470589f, 0.1843137f, 0.2352941f);
    Color MID_COLOR = new Color(0.9294118f, 0.7137255f, 0.2862745f);
    Color FULL_COLOR = new Color(0.1411765f, 0.7098039f, 0.3686275f);
    Dictionary<string, string> m_games_realnames = new Dictionary<string,string>{
        {"One_pic_four_words", "1 Image 4 Mots"},
        {"Quiz", "Quiz"},
        {"Hanged", "Lettres Manquantes"},
        {"Word_clouds", "Nuage de Mots"},
        {"Missing_words", "Phrase à trous"}
    };

    void Start()
    { 
        ShowScreen();
        if(StageManager.instance != null)
        {
            string module_name = StageManager.instance.GetString("current_module_name");
            int level_index = StageManager.instance.GetInt("current_level_index");
            int level_number = level_index + 1;
            m_module_name_text.text = StageManager.instance.m_modulesDisplayName.Where(m => m.fileName.Equals(module_name)).FirstOrDefault().dislpayName;  //get the module display name based of the name written inside the JSON file;
            InitializeLevelText(level_number);
            InitializeMinigameDisplay();
            // //GET THE GLOBAL SCORE
            float global_score = Mathf.Round(StageManager.instance.GetGlobalScore(module_name, level_index));
            InitializeGlobalScore(global_score);
            InitializeEarnedMedal(global_score);
            InitializeSliderMedal();
            m_progressManager.GetComponent<ProgressManager>().SaveModuleLevelProgress(module_name, level_index);

            // m_progressManager.GetComponent<ProgressManager>().SaveModuleLevelProgress(module_name, level_index);
            // we should check if the player has level up 
            //if(StageManager.instance.GetInt(m_progressManager.GetComponent<ProgressManager>().player_level) < m_progressManager.GetComponent<ProgressManager>().GetPlayerLevel())
            //{
            //    Debug.Log("should choose between two partners");
            //    m_level_anouncement.text = "Bravo ! Vous avez atteint le niveau " + ProgressManager.Instance.GetPlayerLevel().ToString();
            //    ProgressManager.Instance.UnlockPartner(ProgressManager.Instance.GetPlayerLevel());
            //}
        }
        int screen_index = PlayerPrefs.GetInt("stage_screen_index");
        if (screen_index == (int)Screens.GAMEOVER)
        {
            //IN CASE THE GAME OVER SCREEN IS ACTIVE, WE SHOULD NOT DISPLAY THE SCORE OF UNPLAYED MINIGAME
            // HideWinscreenElements();
        }


    }

    public void InitializeLevelText(int level_number)
    {

        string module_name = StageManager.instance.GetString("current_module_name");
        int level_index = StageManager.instance.GetInt("current_level_index");

        int next_level_number = level_number;
        bool is_last_level = StageManager.instance.IsLastLevel(level_number);

        //we check if we are not at the last level, the score is enough to pass to the next level or the next level is already unlocked
        if(StageManager.instance.GetGlobalScore(module_name, level_index) > (int)ProgressManager.Medals.BRONZE || ProgressManager.Instance.GetLevelProgress(module_name, level_index) > (int)ProgressManager.Medals.BRONZE)
        {
            m_finished_level_text.text = "Niveau " + level_number + " terminé";
            m_finished_level_text.color = FULL_COLOR;
            m_notif_score_text.SetActive(false);
        }
        else
        {
            m_finished_level_text.text =  "NIVEAU " + next_level_number + " - Echec";
            m_finished_level_text.color = LOW_COLOR;
            m_notif_score_text.SetActive(true);
            // m_finished_level_bg.color = LOW_COLOR;
        }

        if (!is_last_level && (StageManager.instance.GetGlobalScore(module_name, level_index) > (int)ProgressManager.Medals.BRONZE || ProgressManager.Instance.GetLevelProgress(module_name, level_index) > (int)ProgressManager.Medals.BRONZE))
        {
            next_level_number = level_number + 1;
        }
        else
        {
            m_next_level_button_text.transform.parent.gameObject.SetActive(false);
        }

        m_next_level_button_text.text = "NIVEAU " + next_level_number;


    }

    public void InitializeGlobalScore(float global_score)
    {
        m_global_score_text.text = global_score.ToString() + "%";
        Color c = GetProgressColor(global_score, true);
        m_global_score_bar.color = c;
        m_global_score_bar.fillAmount = global_score / 100;
    }

    public void InitializeEarnedMedal(float progress)
    {
        string module_name = StageManager.instance.GetString("current_module_name");
        int level_index = StageManager.instance.GetInt("current_level_index");
        StageManager.GAME_DIFFICULTY current_difficulty = StageManager.instance.GetLevelDifficulty(module_name, level_index);

        if (progress >= (int)ProgressManager.Medals.BRONZE)
        {

            
            //Checked at which difficulty the player must play with
            if (current_difficulty.ToString().ToLower().Equals(StageManager.GAME_DIFFICULTY.EASY.ToString().ToLower()))
            {
                if ((ProgressManager.Instance.GetLevelNbrOfBronzeMedal(module_name, level_index + 1) > 0))
                {
                    current_difficulty = StageManager.GAME_DIFFICULTY.MEDIUM;
                }
            }
            else if (current_difficulty.ToString().ToLower().Equals(StageManager.GAME_DIFFICULTY.MEDIUM.ToString().ToLower()))
            {
                if ((ProgressManager.Instance.GetLevelNbrOfSilverMedal(module_name, level_index + 1) > 0))
                {
                    current_difficulty = StageManager.GAME_DIFFICULTY.HARD;
                }

            }

            if (current_difficulty == StageManager.GAME_DIFFICULTY.EASY)
            {
                //PLACE BRONZE MEDAL IF THE PLAYER HAS REACHED THE REQUIRED PROGRESSION FOR THE BRONZE MEDAL
                // m_earned_medal.sprite = m_medals_sprite[(int)Medal_index.BRONZE];
                // m_earned_medal.gameObject.SetActive(true);
                // m_earned_medal_text.text = "BRONZE";
                m_earned_medal_index = 1;
                SaveDifficultyProgress(module_name, level_index, m_earned_medal_index);
            }
            else if(current_difficulty == StageManager.GAME_DIFFICULTY.MEDIUM)
            {
                //PLACE SILVER MEDAL IF THE PLAYER HAS REACHED THE REQUIRED PROGRESSION FOR THE SILVER 
                // m_earned_medal.sprite = m_medals_sprite[(int)Medal_index.SILVER];
                // m_earned_medal.gameObject.SetActive(true);
                // m_earned_medal_text.text = "ARGENT";
                m_earned_medal_index = 2;
                SaveDifficultyProgress(module_name, level_index, m_earned_medal_index);
            }
            else if(current_difficulty == StageManager.GAME_DIFFICULTY.HARD)
            {
                //PLACE GOLD MEDAL IF THE PLAYER HAS REACHED THE REQUIRED PROGRESSION FOR THE GOLD MEDAL
                // m_earned_medal.sprite = m_medals_sprite[(int)Medal_index.GOLD];
                // m_earned_medal.gameObject.SetActive(true);
                // m_earned_medal_text.text = "OR";
                m_earned_medal_index = 3;
                SaveDifficultyProgress(module_name, level_index, m_earned_medal_index);
                
            }

        }
        else if(progress < (int)ProgressManager.Medals.BRONZE)
        {
            // m_earned_medal.gameObject.SetActive(false);
            // m_earned_medal.sprite = m_medals_sprite[0];
            // m_earned_medal_text.text = "";

            
            // m_earned_medal_index = 0;
            if (current_difficulty == StageManager.GAME_DIFFICULTY.EASY)
            {
                m_earned_medal_index = 0; // No Medal won
                if(GetDifficultyProgress(module_name, level_index, 1))
                    m_earned_medal_index = 1;

            }
            else if(current_difficulty == StageManager.GAME_DIFFICULTY.MEDIUM)
            {
                m_earned_medal_index = 1;
                if(GetDifficultyProgress(module_name, level_index, 2))
                    m_earned_medal_index = 2;
            }
            else if(current_difficulty == StageManager.GAME_DIFFICULTY.HARD)
            {
                m_earned_medal_index = 2;
                if(GetDifficultyProgress(module_name, level_index, 3))
                    m_earned_medal_index = 3;
            }

        }
    }

    void SaveDifficultyProgress(string module_name, int level_index, int medal_index)
    {
        string pref_name = module_name + ":" + level_index + ":" + medal_index;
        PlayerPrefs.SetInt(pref_name, 1);
    }

    bool GetDifficultyProgress(string module_name, int level_index, int medal_index)
    {
        string pref_name = module_name + ":" + level_index + ":" + medal_index;
        bool won = false;
        int value = PlayerPrefs.GetInt(pref_name, 0);
        won = value == 1 ? true : false;

        return won;
    }

    void SetMedals(int medal)
    {
        SetUnlockedDifficulties(medal);

        int index = medal;
        for (int i = 0; i < index; i++)
        {
            m_medals_contaiers[i].GetComponentInChildren<Image>().sprite = m_medals_sprite[i+1]; // m_medals_sprite contains the default medals at index 0
        }
    }

    void InitializeSliderMedal()
    {

        string module_name = StageManager.instance.GetString("current_module_name");
        int level_index = StageManager.instance.GetInt("current_level_index");
        StageManager.GAME_DIFFICULTY current_difficulty = StageManager.instance.GetLevelDifficulty(module_name, level_index);

        //Checked at which difficulty the player must play with
        if (current_difficulty.ToString().ToLower().Equals(StageManager.GAME_DIFFICULTY.EASY.ToString().ToLower()))
        {
            if ((ProgressManager.Instance.GetLevelNbrOfBronzeMedal(module_name, level_index + 1) > 0))
            {
                current_difficulty = StageManager.GAME_DIFFICULTY.MEDIUM;
            }
        }
        else if (current_difficulty.ToString().ToLower().Equals(StageManager.GAME_DIFFICULTY.MEDIUM.ToString().ToLower()))
        {
            if ((ProgressManager.Instance.GetLevelNbrOfSilverMedal(module_name, level_index + 1) > 0))
            {
                current_difficulty = StageManager.GAME_DIFFICULTY.HARD;
            }

        }

        
        if (current_difficulty == StageManager.GAME_DIFFICULTY.EASY)
        {
            //if the difficulty is easy and the player hasn't earned the required score then he should play in easy
            if(StageManager.instance.GetGlobalScore(module_name, level_index) < (int)ProgressManager.Medals.BRONZE)
            {
                // m_btn_replay_text.text = "Rejouer en facile";
                // We Hide all other option and display only "REPLAY EASY"
                // foreach (Transform item in m_difficulty_items)
                // {
                //     item.gameObject.SetActive(false);
                // }
 
                DisplayDifficultyViewMode(0, 1); 
                SetMedals(m_earned_medal_index);
                m_difficulty_background.SetActive(false);

            }
            else
            {
                // m_btn_replay_text.text = "Rejouer en moyen";
                // We Hide all other option and display only "REPLAY EASY - Default" and "REPLAY MEDIUM - Active"
                // foreach (Transform item in m_difficulty_items)
                // {
                //     item.gameObject.SetActive(false);
                // }
 
                DisplayDifficultyViewMode(0, 0); 
                DisplayDifficultyViewMode(1, 1); 

                SetMedals(m_earned_medal_index);

            }
        }
        else if (current_difficulty == StageManager.GAME_DIFFICULTY.MEDIUM)
        {
            //assigned silver value to the slider image
            // m_slider_medal_image.sprite = m_medals_sprite[(int)Medal_index.SILVER];
            //if the difficulty is medium and the player hasn't earned the required score then he should play in medium
            if (StageManager.instance.GetGlobalScore(module_name, level_index) < (int)ProgressManager.Medals.SILVER)
            {
                // m_btn_replay_text.text = "Rejouer en moyen";
                 // We Hide all other option and display only "REPLAY EASY - Default" and "REPLAY MEDIUM - Active"
                // foreach (Transform item in m_difficulty_items)
                // {
                //     // item.gameObject.SetActive(false);
                //     item.GetComponentInChildren<Image>().color = Color.gray;
                // }
 
                DisplayDifficultyViewMode(0, 0); 
                DisplayDifficultyViewMode(1, 1); 

                SetMedals(m_earned_medal_index);
            }
            else
            {
                // m_btn_replay_text.text = "Rejouer en difficile";

                // We Hide all other option and display  "REPLAY EASY - Default",  "REPLAY MEDIUM - Default" and "REPLAY HARD - Active"
                // foreach (Transform item in m_difficulty_items)
                // {
                //     item.gameObject.SetActive(false);
                // }
 
                DisplayDifficultyViewMode(0, 0); 
                DisplayDifficultyViewMode(1, 0); 
                DisplayDifficultyViewMode(2, 1); 

                SetMedals(m_earned_medal_index);

            }

        }
        else if (current_difficulty == StageManager.GAME_DIFFICULTY.HARD)
        {
            //assigned gold value to the slider image
            // m_slider_medal_image.sprite = m_medals_sprite[(int)Medal_index.GOLD];
            // m_btn_replay_text.text = "Rejouer en difficile";

                // We Hide all other option and display  "REPLAY EASY - Default",  "REPLAY MEDIUM - Default" and "REPLAY HARD - Active"
                // foreach (Transform item in m_difficulty_items)
                // {
                //     item.gameObject.SetActive(false);
                // }
 
                DisplayDifficultyViewMode(0, 0); 
                DisplayDifficultyViewMode(1, 0); 
                DisplayDifficultyViewMode(2, 1);

                SetMedals(m_earned_medal_index);
        }
        else
        {
            //assigned bronze value to the slider image
            // m_slider_medal_image.sprite = m_medals_sprite[(int)Medal_index.BRONZE];
            // m_btn_replay_text.text = "Rejouer en facile";
            
            // We Hide all other option and display only "REPLAY EASY"
            // foreach (Transform item in m_difficulty_items)
            // {
            //     item.gameObject.SetActive(false);
            // }

            Transform current_item = m_difficulty_items[0];
            DisplayDifficultyViewMode(0, 1); 
            SetMedals(m_earned_medal_index);
            m_difficulty_background.SetActive(false);

        }
    }

    public void DisplayDifficultyViewMode(int item_index, int index) // 0 For Default , 1 For Active
    {

        int opposite_index = 1 - index; // 1 --x 0 : 0 --x 1
        m_difficulty_items[item_index].gameObject.SetActive(true);
        m_difficulty_items[item_index].GetChild(opposite_index).gameObject.SetActive(false);
        m_difficulty_items[item_index].GetChild(index).gameObject.SetActive(true);
    }

    public void SetUnlockedDifficulties(int unlocked_index)
    {
        for (int i = unlocked_index+1; i < 3; i++)
        {
            m_difficulty_items[i].GetComponentInChildren<Image>().color = Color.gray;
            m_difficulty_items[i].GetChild(0).Find("icon").gameObject.SetActive(false);
        }
    }

    public void ReturnToChooseMenu()
    {
        if(StoryTellerCtrlr.Instance != null)
            StoryTellerCtrlr.Instance.m_storyIntroduced = false;

        SceneManager.LoadScene("Module_screen", LoadSceneMode.Single);

    }

    public void GoBack()
    {
        SceneManager.LoadScene("Module_screen", LoadSceneMode.Single);
    }

    /// <summary>
    /// Loading the next level game
    /// </summary>
    public void LoadNextLevel()
    {
        string current_module_name = StageManager.instance.GetString("current_module_name");
        int nextLevelIndex = StageManager.instance.GetInt("current_level_index") + 1;
        StageManager.instance.m_currentMiniGameIndex.Set(0);    //reset the mini-game index
        // Display StoryTelling and Start Intro
        StoryTellerCtrlr.Instance.LaunchIntro(current_module_name, nextLevelIndex);

        StageManager.instance.LoadModule(current_module_name, nextLevelIndex);


    }

    public void Retry()
    {
        string module_name = PlayerPrefs.GetString("current_module_name");
        int level_index = PlayerPrefs.GetInt("current_level_index");
        StageManager.instance.m_currentMiniGameIndex.Set(0);    //reset the mini-game index
        StageManager.instance.LoadModule(module_name , level_index);

        // Display StoryTelling and Start Intro
        StoryTellerCtrlr.Instance.LaunchIntro(module_name, level_index);

    }

    public void ShowScreen()
    {

        //@Reinit Timer

        StageManager.instance.timer = StageManager.instance.time * 60f;
        // int screen_index = PlayerPrefs.GetInt("stage_screen_index");
        // transform.GetChild(screen_index).gameObject.SetActive(true);
    }

    //HIDE SCORE OF UNPLAYED MINIGAME
    public void HideUnplayedMinigameScore()
    {
        int childIndex = 1;
        string current_module_name = StageManager.instance.GetString("current_module_name");
        int current_level_index = StageManager.instance.GetInt("current_level_index");
        string current_game_name = StageManager.instance.GetString("current_game_name");
        int currentTurn = StageManager.instance.GetNextGameIndex(current_module_name, current_level_index, current_game_name);
        m_finished_level_text.text = "Temps écoulé";
        foreach (Transform child in thumbnail_container.transform)
        {
            if(childIndex > currentTurn)
            {
                child.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "-";
            }
            childIndex++;
        }

    }

    //HIDE WIN SCREEN ELEMENT
    public void HideWinscreenElements()
    {
        m_global_score_elements.gameObject.SetActive(false);
        m_global_score_bar.gameObject.SetActive(false);
        m_medals_image_elements.gameObject.SetActive(false);
        HideUnplayedMinigameScore();
        m_earned_medal.gameObject.SetActive(false);
        m_next_level_button.gameObject.SetActive(false);
    }

    public void InitializeMinigameDisplay()
    {
        string module_name = StageManager.instance.GetString("current_module_name");
        int module_index = StageManager.instance.GetModuleIndexByName(module_name);
        int level_index = StageManager.instance.GetInt("current_level_index");
        int game_list_iterator = 0;
        foreach(Game_game_struct g in StageManager.instance.game_structure.modules[module_index].levels[level_index].games)
        {
            // Define
            Transform game_progress = m_games_progress[game_list_iterator];
            Image progress_image = game_progress.Find("progress").GetComponent<Image>();
            TextMeshProUGUI game_name_text = game_progress.Find("game_name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI percentage_text = game_progress.Find("percentage").GetComponent<TextMeshProUGUI>();

            // Data 
            string game_name = g.name;
            float game_percentage = StageManager.instance.GetGameScore(module_name, level_index, g.name);
            float game_progress_value = game_percentage / 100.0f;
            Color c = GetProgressColor(game_percentage);
            
            // Display
            progress_image.color = c;
            progress_image.fillAmount = game_progress_value;
            game_name_text.text = m_games_realnames[game_name];
            percentage_text.text = game_percentage.ToString() + "%";

            game_list_iterator++;

            //Get splash screen image

            // string splash_icon_sprite_path = "Game_presets/Games_icons/" + g.name + "_icon";
            // Sprite splash_icon_sprite = Resources.Load<Sprite>(splash_icon_sprite_path);

            // GameObject minigame_thumbnail = new GameObject();
            // minigame_thumbnail = Instantiate(m_minigame_thumbnail_prefab, Vector3.zero, Quaternion.identity);
            // minigame_thumbnail.transform.GetChild(0).GetComponent<Image>().sprite = splash_icon_sprite;
            // minigame_thumbnail.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = StageManager.instance.GetGameScore(module_name, level_index, g.name).ToString() + "%";
            // minigame_thumbnail.transform.parent = thumbnail_container.transform;
        }
    }

    Color GetProgressColor(float p, bool binary = false)
    {
        Color c = FULL_COLOR;
        if(p < 50)
        {
            c = LOW_COLOR;
        }
        else if (p < 75)
        {
            if(binary)
            {
                c = LOW_COLOR;
            }
            else
            {
                c = MID_COLOR;
            }
            
        }
        return c;
    }

    public void CloseUnlockerPopup()
    {
        unlocker_popup.transform.GetComponent<Animation>().Play("unlocker_box_zoom_out");
        
    }

}
