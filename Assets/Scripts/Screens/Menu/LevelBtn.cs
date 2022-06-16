using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class LevelBtn : MonoBehaviour
{
    public Image m_medal_image;
    public TextMeshProUGUI m_level_text;
    public TextMeshProUGUI m_level_progress;
    private string module_name = "Communication";
    public int level_index;

    
    public enum Medal_sprite_index
    {
        BRONZE = 1,
        SILVER = 2,
        GOLD  = 3
    }

    //GAMEOBJECT THAT SHOULD BE ACTIVATED IF THE LEVEL IS LOCKED
    public GameObject lock_level;

    //LIST OF MEDALS SPRITES
    public List<Sprite> medals_sprites;

    private void Start()
    {
        if (StageManager.instance != null)
        {
            module_name = StageManager.instance.GetString("current_module_name");
        }
        // ShowLevelProgress();
        m_level_text.text = (level_index + 1).ToString();
        IsLevelUnlocked();
        IsLevelMedalEarned(module_name, level_index);
    }
    public void ShowLevelProgress()
    {
        float progress = ProgressManager.Instance.GetLevelProgress(module_name, level_index);
        if(StageManager.instance.GetLevelDifficulty(module_name, level_index) == StageManager.GAME_DIFFICULTY.HARD)
        {
            m_level_progress.text = Mathf.Round(ProgressManager.Instance.GetLevelProgress(module_name, level_index)).ToString() + " %";
        }
        else
        {
            m_level_progress.gameObject.SetActive(false);
        }
    }

    public void IsLevelUnlocked()
    {
        if(ProgressManager.Instance.IsLevelUnlocked(module_name, level_index))
        {
            lock_level.gameObject.SetActive(false);
            transform.Find("Button").GetComponent<Button>().interactable = true;
        }
        else
        {
            lock_level.gameObject.SetActive(true);
            transform.Find("Button").GetComponent<Button>().interactable = false;
        }
        int index = level_index + 1;
        if(StageManager.instance.m_notAvailableLevels.Where(s => s.ToLower().Equals(StageManager.instance.GetNoAvailableLevelKey(module_name, index).ToLower())).FirstOrDefault() != null)//verified if the level is available
        {
            lock_level.gameObject.SetActive(true);
            transform.Find("Button").GetComponent<Button>().interactable = false;
            Debug.LogWarning("Level " + index + " is not available");
        }
    }

    public void IsLevelMedalEarned(string module_name, int level_index)
    {
        int nbrBronzeMedals = ProgressManager.Instance.GetLevelNbrOfBronzeMedal(module_name, level_index + 1);
        int nbrSilverMedals = ProgressManager.Instance.GetLevelNbrOfSilverMedal(module_name, level_index + 1);

        //Debug.Log(nbrSilverMedals);
        if (nbrBronzeMedals > 0 || nbrSilverMedals > 0 )
        {
            
            if(StageManager.instance.GetLevelDifficulty(module_name, level_index) == StageManager.GAME_DIFFICULTY.EASY)
            {
                //PLACE BRONZE MEDAL IF THE PLAYER HAS REACHED THE REQUIRED PROGRESSION FOR THE BRONZE MEDAL
                m_medal_image.sprite = medals_sprites[(int)Medal_sprite_index.BRONZE];
            }
            else if(StageManager.instance.GetLevelDifficulty(module_name, level_index) == StageManager.GAME_DIFFICULTY.MEDIUM)
            {
                //PLACE SILVER MEDAL IF THE PLAYER HAS REACHED THE REQUIRED PROGRESSION FOR THE SILVER 
                m_medal_image.sprite = medals_sprites[(int)Medal_sprite_index.SILVER];
            }
            else
            {
                //PLACE GOLD MEDAL IF THE PLAYER HAS REACHED THE REQUIRED PROGRESSION FOR THE GOLD MEDAL
                m_medal_image.sprite = medals_sprites[(int)Medal_sprite_index.GOLD];
            }
        }
        else
        {
            m_medal_image.sprite = medals_sprites[0];
        }
    }

    public void StartModuleLevel()
    {
        string current_module_name = StageManager.instance.GetString("current_module_name");
        // Display StoryTelling and Start Intro
        StoryTellerCtrlr.Instance.LaunchIntro(module_name, level_index);
        StageManager.instance.m_currentMiniGameIndex.Set(0);    //reset the mini-game index
        StageManager.instance.LoadModule(current_module_name, level_index);
    }
}
