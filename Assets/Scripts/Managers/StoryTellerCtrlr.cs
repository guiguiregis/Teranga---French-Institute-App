using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using Kayfo;
using System.Linq;
using UnityEngine.U2D;
public class StoryTellerCtrlr : Singleton<StoryTellerCtrlr>
{


    public Canvas m_canvas;
    public Image m_backgroundImage;
    public Image m_avatar;
    public GameObject m_dialogCloud;
    public TextMeshProUGUI m_dialogText;
    public GameObject m_skipBtn;
    // Avatars
    public List<MediaBloc> m_characters;
    // Backgrounds
    public List<MediaBloc> m_backgrounds;



    const string STORYTELLING_KEY = "storytelling";
    private string m_module_name = "communication";
    private int m_level_index = 0;
    private string m_storytelling_struct_path = "Game_presets\\Storytelling";
    private TextAsset m_json;

    Storytelling_struct m_storytelling_struct;

    [HideInInspector]
    public bool m_storyIntroduced = false;
    [HideInInspector]
    public bool m_storyOutroduced = false;
    [HideInInspector]
    public float INTRO_DURATION = 2.0f;

    Coroutine m_storytellingCoroutine = null;
    public Mentors_struct m_mentors;
    public string m_mentors_struct_path = "Mentors\\Mentors";
    public Mentor m_curr_mentor;
    public SpriteAtlas m_mentors_sprites;
    // Start is called before the first frame update    
    void Awake()
    {
 
    }

    void Init()
    {
        GetStorytellingStruct();
    }
    public void LoadStruct()
    {

        string dataPath = m_storytelling_struct_path + Path.DirectorySeparatorChar + "Storytelling_" + m_module_name;
        m_curr_mentor = m_mentors.mentors.Where(m => m.module_name.ToLower().Equals(m_module_name.ToLower())).FirstOrDefault(); //get the curr mentor based on the module
        m_json = Resources.Load(dataPath) as TextAsset;
    }

    public void LoadMentors(string path)
    {
        TextAsset json = Resources.Load(path) as TextAsset;
        if (json != null)
        {
            m_mentors.mentors = new List<Mentor>();  //we should init the list of partner inside the struct
            m_mentors = JsonUtility.FromJson<Mentors_struct>(json.text);  //we should load the partners data using the given path
        }
        else
        {
            //IF THE GIVEN PATH IS NOT FOUND THEN WE SHOULD USE THE DEFAULT PATH
            Debug.LogWarning("Mentor struct path not found");
        }
    }

    public void SaveStorytelling()
    {
        string prevJson = "";

        if (PlayerPrefs.HasKey(STORYTELLING_KEY))
            prevJson = PlayerPrefs.GetString(STORYTELLING_KEY);

        string currentJson = m_json.ToString();

        //if ((!PlayerPrefs.HasKey(STORYTELLING_KEY)) || (prevJson != "" && (prevJson != currentJson )) )
        {
            //SAVING THE Storytelling STRUCT INTO PLAYER PREF
            StageManager.instance.SetString(STORYTELLING_KEY, m_json.ToString());
        }
    }

    public void InitializeStorytelling()
    {
        m_module_name = StageManager.instance.GetString("current_module_name");
        LoadMentors(m_mentors_struct_path);
        LoadStruct();
        SaveStorytelling();
    }

    public Storytelling_struct GetStorytellingStruct()
    {
        string myJson = StageManager.instance.GetString(STORYTELLING_KEY);
        Storytelling_struct my_story = new Storytelling_struct();
        my_story = JsonUtility.FromJson<Storytelling_struct>(myJson);

        m_storytelling_struct = my_story;
        return my_story;
    }

    Sprite GetMedia(List<MediaBloc> medias, string name)
    {
        return medias.Find(m => m.name == name).image;
    }

    public Sprite GetMentorProtrait(string _image_name)
    {
        return m_mentors_sprites.GetSprite(_image_name) != null ? m_mentors_sprites.GetSprite(_image_name): GetMedia(m_characters,"male"); 
    }

    void GetPresets(string introOrOutro)
    {
            string intro;
            string character_name;
            string background_name;

            if(introOrOutro == "intro")
            {
                intro = m_storytelling_struct.levels[m_level_index].intro.text;
                character_name = m_curr_mentor.image_name;
                background_name = m_storytelling_struct.levels[m_level_index].intro.background;
            }
            else
            {
                intro = m_storytelling_struct.levels[m_level_index].outro.text;
                character_name = m_curr_mentor.image_name;
                background_name = m_storytelling_struct.levels[m_level_index].outro.background;
            }

            if(character_name == "player")
            {
                // Current character avatar is set in ProgressManager 
                int caharacter_index = ProgressManager.Instance.m_persistentCharacterIndex.Get();

                character_name = "male";
                if(caharacter_index == 0) character_name = "female";

            }

            if(background_name == ""  || background_name == null)
            {
                background_name = "dakar_city";
            }

            Sprite character = GetMentorProtrait(character_name);
            Sprite background = GetMedia(m_backgrounds, background_name);

            m_avatar.sprite = character;
            m_backgroundImage.sprite = background;
            m_dialogText.text = intro;

            
    }
    public void StartIntro()
    {
        Debug.Log(m_storytelling_struct.levels);
        // If didn't yet define an intro for this level
        if(m_level_index >= m_storytelling_struct.levels.Count)
        {
            string intro = "Lorem ipsum dolore sit amet";
            Sprite character = GetMedia(m_characters, "male");
            Sprite background = GetMedia(m_backgrounds, "dakar_city");

            m_avatar.sprite = character;
            m_backgroundImage.sprite = background;
            m_dialogText.text = intro;

        }
        else
        {
            GetPresets("intro");  
        }
     
        // Wait some secs and hide intro
        m_storytellingCoroutine = StartCoroutine(IToggleStorytelling(false));
    }
    
    public void StartOutro()
    {
        if(m_level_index >= m_storytelling_struct.levels.Count)
        {
            string outro = "Adiscipae ellemis verna aego persit";
            Sprite character = GetMedia(m_characters, "male");
            Sprite background = GetMedia(m_backgrounds, "dakar_city");

            m_avatar.sprite = character;
            m_backgroundImage.sprite = background;
            m_dialogText.text = outro;

        }
        else
        {  
            GetPresets("outro");
        }
        // Wait some secs and hide outro
        m_storytellingCoroutine = StartCoroutine(IToggleStorytelling(false));
    }

    public void ToggleStorytelling(bool status)
    {
        if(status && !m_storyIntroduced)
        {
            Init();
        }

        m_storytellingCoroutine = StartCoroutine(IToggleStorytelling(status));
    }

    IEnumerator IToggleStorytelling(bool status)
    {

        //StageManager.GAME_DIFFICULTY difficulty = StageManager.instance.GetLevelDifficulty(StageManager.instance.GetString("current_module_name"), StageManager.instance.GetInt("current_level_index"));
        //if (!difficulty.Equals(StageManager.GAME_DIFFICULTY.EASY))
        //{
        //    Debug.Log("Not Easy");
        //    SkipStorytelling();
        //}
        //else
        //{

            // Debug.Log("Easy");

            //WE SHOULD CHECK IF THE PLAYER HAS AT LEAST ONE BRONZE MEDAL BEFORE UNLOCKING THE LEVEL OTHERWISE WE JUST GIVE HIM THE BRONZE MEDAL
            float delay = 0f;
            if(status == false) 
            {
                delay = INTRO_DURATION;
            }

            yield return new WaitForSeconds(delay);
            

            //if(FindObjectOfType<TimerScript>() != null)
            //    FindObjectOfType<TimerScript>().ToggleTimer(true);

            m_canvas.gameObject.SetActive(true);

        //yield return new WaitForSeconds(INTRO_DURATION);
        //}


    }


    public void SkipStorytelling()
    {
        m_canvas.gameObject.SetActive(false);
        m_storyIntroduced = true;
        if(m_storytellingCoroutine != null)
            StopCoroutine(m_storytellingCoroutine);
        
        TimerScript.Instance.Init();

    }
    public void LaunchIntro(string module_name, int level_index = 0)
    {

        string game_difficulty = StageManager.instance.GetGameDifficulty(module_name, level_index, 0);
        m_skipBtn.transform.GetComponentInChildren<TextMeshProUGUI>().text = "JOUER";

        m_level_index = level_index;
        if(game_difficulty.ToLower().Equals(StageManager.GAME_DIFFICULTY.EASY.ToString().ToLower()))    //verifiy if the difficulty is easy
        {
            //Display StoryTelling and Start Intro
            StoryTellerCtrlr.Instance.ToggleStorytelling(true);
            StoryTellerCtrlr.Instance.StartIntro();
        }
    }
    public void LaunchOutro(string module_name, int level_index = 0, float global_score = 0f)
    {
        string game_difficulty = StageManager.instance.GetGameDifficulty(module_name, level_index, 0);
        m_skipBtn.transform.GetComponentInChildren<TextMeshProUGUI>().text = "SUIVANT";

        m_level_index = level_index;
        if(global_score >= (float)ProgressManager.Medals.GOLD && game_difficulty.ToLower().Equals(StageManager.GAME_DIFFICULTY.HARD.ToString().ToLower()))    //verifiy if the difficulty is easy
        {
            //Display StoryTelling and Start Outro
            StoryTellerCtrlr.Instance.ToggleStorytelling(true);
            StoryTellerCtrlr.Instance.StartOutro();
        }
    }






}

[System.Serializable]
public class Storytelling_preset_struct
{
    public string character;
    public string text;
    public string background;
}

[System.Serializable]
public class Storytelling_level_struct
{
    public int level;
    public Storytelling_preset_struct intro;
    public Storytelling_preset_struct outro;
}

[System.Serializable]
public class Storytelling_struct
{
    public List<Storytelling_level_struct> levels;
}

[System.Serializable]
public class MediaBloc
{
    public string name;
    public Sprite image;
}