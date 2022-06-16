using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StageScreen : MonoBehaviour
{
    public TextMeshProUGUI m_btn_replay_text;
    public TextMeshProUGUI m_module_name_text;
    public TextMeshProUGUI m_global_score_text;
    public TextMeshProUGUI m_finished_level_text;
    public TextMeshProUGUI m_next_level_button_text;
    public GameObject m_minigame_thumbnail_prefab;
    public GameObject thumbnail_container;
    public Image m_global_score_bar;
    public Image m_earned_medal;
    public TextMeshProUGUI m_earned_medal_text;
    public List<Sprite> m_medals_sprite;
    public GameObject m_next_level_button;
    public GameObject m_global_score_elements;
    public GameObject m_medals_image_elements;

    public GameObject unlocker_popup;
    public GameObject unlocked_partner_prefab;
    public Image m_slider_medal_image;
    public GameObject m_progressManager;
    public TextMeshProUGUI m_level_anoucement;
    enum Medal_index
    {
        BRONZE = 1,
        SILVER = 2,
        GOLD   = 3
    }

    enum Screens
    {
        GAMEOVER = 0,
        WIN = 1
    }

    void Start()
    {
        ShowScreen();
        if(StageManager.instance != null)
        {
            string module_name = StageManager.instance.GetString("current_module_name");
            int level_index = StageManager.instance.GetInt("current_level_index");
            int level_number = level_index + 1;
            m_module_name_text.text = module_name;
            InitializeLevelText(level_number);
            InitializeMinigameThumbnail();
            //GET THE GLOBAL SCORE
            float global_score = Mathf.Round(StageManager.instance.GetGlobalScore(module_name, level_index));
            InitializeGlobalScore(global_score);
            InitializeEarnedMedal(global_score);
            InitializeSliderMedal();
            m_progressManager.GetComponent<ProgressManager>().SaveModuleLevelProgress(module_name, level_index);
            // we should check if the player has level up 
            //if(StageManager.instance.GetInt(m_progressManager.GetComponent<ProgressManager>().player_level) < m_progressManager.GetComponent<ProgressManager>().GetPlayerLevel())
            //{
            //    Debug.Log("should choose between two partners");
            //    m_level_anoucement.text = "Bravo ! Vous avez atteint le niveau " + ProgressManager.Instance.GetPlayerLevel().ToString();
            //    ProgressManager.Instance.UnlockPartner(ProgressManager.Instance.GetPlayerLevel());
            //}
        }
        int screen_index = PlayerPrefs.GetInt("stage_screen_index");
        if (screen_index == (int)Screens.GAMEOVER)
        {
            //IN CASE THE GAME OVER SCREEN IS ACTIVE, WE SHOULD NOT DISPLAY THE SCORE OF UNPLAYED MINIGAME
            HideWinscreenElements();
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
        }
        else
        {
            m_finished_level_text.text = "Score insuffisant, il faut atteindre 75%";
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
        m_global_score_bar.fillAmount = global_score / 100;
    }

    public void InitializeEarnedMedal(float progress)
    {
        if (progress >= (int)ProgressManager.Medals.BRONZE)
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
                //PLACE BRONZE MEDAL IF THE PLAYER HAS REACHED THE REQUIRED PROGRESSION FOR THE BRONZE MEDAL
                m_earned_medal.sprite = m_medals_sprite[(int)Medal_index.BRONZE];
                m_earned_medal.gameObject.SetActive(true);
                m_earned_medal_text.text = "BRONZE";
            }
            else if(current_difficulty == StageManager.GAME_DIFFICULTY.MEDIUM)
            {
                //PLACE SILVER MEDAL IF THE PLAYER HAS REACHED THE REQUIRED PROGRESSION FOR THE SILVER 
                m_earned_medal.sprite = m_medals_sprite[(int)Medal_index.SILVER];
                m_earned_medal.gameObject.SetActive(true);
                m_earned_medal_text.text = "ARGENT";
            }
            else if(current_difficulty == StageManager.GAME_DIFFICULTY.HARD)
            {
                //PLACE GOLD MEDAL IF THE PLAYER HAS REACHED THE REQUIRED PROGRESSION FOR THE GOLD MEDAL
                m_earned_medal.sprite = m_medals_sprite[(int)Medal_index.GOLD];
                m_earned_medal.gameObject.SetActive(true);
                m_earned_medal_text.text = "OR";
            }

        }
        else if(progress < (int)ProgressManager.Medals.BRONZE)
        {
            // m_earned_medal.gameObject.SetActive(false);
            m_earned_medal.sprite = m_medals_sprite[0];
            m_earned_medal_text.text = "";

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
            //assigned bronze value to the slider image
            m_slider_medal_image.sprite = m_medals_sprite[(int)Medal_index.BRONZE];
            //if the difficulty is easy and the player hasn't earned the required score then he should play in easy
            if(StageManager.instance.GetGlobalScore(module_name, level_index) < (int)ProgressManager.Medals.BRONZE)
            {
                m_btn_replay_text.text = "Rejouer en facile";
            }
            else
            {
                m_btn_replay_text.text = "Rejouer en moyen";
            }
        }
        else if (current_difficulty == StageManager.GAME_DIFFICULTY.MEDIUM)
        {
            //assigned silver value to the slider image
            m_slider_medal_image.sprite = m_medals_sprite[(int)Medal_index.SILVER];
            //if the difficulty is medium and the player hasn't earned the required score then he should play in medium
            if (StageManager.instance.GetGlobalScore(module_name, level_index) < (int)ProgressManager.Medals.SILVER)
            {
                m_btn_replay_text.text = "Rejouer en moyen";
            }
            else
            {
                m_btn_replay_text.text = "Rejouer en difficile";
            }

        }
        else if (current_difficulty == StageManager.GAME_DIFFICULTY.HARD)
        {
            //assigned gold value to the slider image
            m_slider_medal_image.sprite = m_medals_sprite[(int)Medal_index.GOLD];
            m_btn_replay_text.text = "Rejouer en difficile";

        }
        else
        {
            //assigned bronze value to the slider image
            m_slider_medal_image.sprite = m_medals_sprite[(int)Medal_index.BRONZE];
            m_btn_replay_text.text = "Rejouer en facile";
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
        
        // Display StoryTelling and Start Intro
        StoryTellerCtrlr.Instance.LaunchIntro(current_module_name ,nextLevelIndex);

        StageManager.instance.LoadModule(current_module_name, nextLevelIndex);


    }

    public void Retry()
    {
        string module_name = PlayerPrefs.GetString("current_module_name");
        int level_index = PlayerPrefs.GetInt("current_level_index");
        StageManager.instance.LoadModule(module_name , level_index);

        // Display StoryTelling and Start Intro
        StoryTellerCtrlr.Instance.LaunchIntro(module_name, level_index);

    }

    public void ShowScreen()
    {

        //@Reinit Timer

        StageManager.instance.timer = StageManager.instance.time * 60f;
        int screen_index = PlayerPrefs.GetInt("stage_screen_index");
        transform.GetChild(screen_index).gameObject.SetActive(true);
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

    public void InitializeMinigameThumbnail()
    {
        string module_name = StageManager.instance.GetString("current_module_name");
        int module_index = StageManager.instance.GetModuleIndexByName(module_name);
        int level_index = StageManager.instance.GetInt("current_level_index");
        foreach(Game_game_struct g in StageManager.instance.game_structure.modules[module_index].levels[level_index].games)
        {
            //Get splash screen image

            string splash_icon_sprite_path = "Game_presets/Games_icons/" + g.name + "_icon";
            Sprite splash_icon_sprite = Resources.Load<Sprite>(splash_icon_sprite_path);

            GameObject minigame_thumbnail = new GameObject();
            minigame_thumbnail = Instantiate(m_minigame_thumbnail_prefab, Vector3.zero, Quaternion.identity);
            minigame_thumbnail.transform.GetChild(0).GetComponent<Image>().sprite = splash_icon_sprite;
            minigame_thumbnail.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = StageManager.instance.GetGameScore(module_name, level_index, g.name).ToString() + "%";
            minigame_thumbnail.transform.parent = thumbnail_container.transform;
        }
    }


    public void CloseUnlockerPopup()
    {
        unlocker_popup.transform.GetComponent<Animation>().Play("unlocker_box_zoom_out");
        
    }

}
