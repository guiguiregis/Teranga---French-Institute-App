using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using TMPro;

public class WC_LevelManager : MonoBehaviour
{
    public GameObject clouds_container;
    public GameObject m_central_cloud;
    public GameObject Hint_text;
    public List<Word_clouds_quiz_item> m_wordCloudLevel;                                           //save all the words in the current level
    public Word_clouds_quiz_item selectedCloudLevel;                                           //To load the current word cloud level and corresponding word
    // public GameObject m_scorePanel;
    // public Text m_score;

    public List<Sprite> m_slotSprites;
    public int selectedWordIndex = 0;

    //Total point of the current level : Initialized to 100
    public float score_rate;
     //Defines points of a turn
    float turn_points = 100;
    private float quiz_rate;

    //Stores final rate of each turn
    float total_quizes_rate; 
    private float malus_per_wrong_answer;
    public int numberOfRightChoices = 0; 
    public int[] selectedWordIndexes;
    public int currentWordCloudsIndex;
    int MAX_WRONG_CHOICES = 5;

    int left_wrong_choices = 5;
    int m_total_words_found = 0;
    int m_total_words_to_find = 5;
    Word_clouds_quiz level_quiz;

    //PlayerPrefs Data
    string module_name;
    int level_index;
    string game_name;
    int numb_turns;
    int turns_iterator = 0;
    //DEFAULT GAME DATA PATH
    string defaultGameDataPath = "Games\\Word_clouds\\Communication\\Level_1\\Word_clouds_data";
    //DEFAULT NUMBER OF TURNS 
    int defaultNumberOfTurns = 3;
    //GENERATED INDEXES
    List<int> m_generatedIndex = new List<int>();


    public AudioSource m_feedbackSFX;
    public AudioClip m_correctSFX;
    public AudioClip m_incorrectSFX;

    public bool m_is_game_completed;   //check if the player has made a choice
    void Awake() {
        int level_difficulty = 0;
        if (StageManager.instance != null)
        {
            module_name = StageManager.instance.GetString("current_module_name");
            level_index = StageManager.instance.GetCurrentLevelIndex() + 1;
            game_name = StageManager.instance.GetString("current_game_name");
            numb_turns = StageManager.instance.GetInt("current_game_turns");

            level_difficulty = StageManager.instance.GetInt("current_game_difficulty");
        }
        else
        {
            numb_turns = defaultNumberOfTurns;
        }

        MAX_WRONG_CHOICES = MAX_WRONG_CHOICES - (level_difficulty - 1); // level_difficulty max = 5
        left_wrong_choices = MAX_WRONG_CHOICES;

        string data_path = "";
        if(StageManager.instance != null)
        {
            data_path = "Games\\Word_clouds\\" + module_name + "\\Level_" + level_index + "\\Word_clouds_data";
        }
        else
        {
            data_path = defaultGameDataPath;
        }
        level_quiz = FindObjectOfType<WC_LevelLoader>().LoadLevel(data_path);
        
        //if(level_quiz.quizes.Where(q => q.gameDifficulty.ToLower().Equals(StageManager.instance.GetGameCurrentDifficulty().ToLower())).FirstOrDefault() != null)//we should verify if the difficulty exist
        //{
        //    m_wordCloudLevel = level_quiz.quizes.Where(q => q.gameDifficulty.ToLower().Equals(StageManager.instance.GetGameCurrentDifficulty().ToLower())).ToList();
        //}
        //else
        //{
            m_wordCloudLevel = level_quiz.quizes.ToList();
        //}

        if (numb_turns > m_wordCloudLevel.Count)
        {
            numb_turns = m_wordCloudLevel.Count;
        }
    }

    // Start is called before the first frame update

    private void Start()
    {
        currentWordCloudsIndex = 0;
        selectedCloudLevel = GetRandomQuizItem();
        malus_per_wrong_answer = turn_points / MAX_WRONG_CHOICES; 
        AddWordToCloud();
    }

   public void AddWordToCloud()
    {
        DesactivateAllClouds();
        // Get new words counter
        m_total_words_found = 0;
        m_total_words_to_find = selectedCloudLevel.related.Length;

        //Initializing turn data
        quiz_rate = turn_points;
        numberOfRightChoices = 0;
        left_wrong_choices = MAX_WRONG_CHOICES;
        //Hint_text.transform.GetComponent<Text>().text = (left_wrong_choices).ToString();
        Hint_text.transform.GetComponent<Text>().text = "0/"+m_total_words_to_find.ToString();

        //assigning key word value
        Transform cloud = m_central_cloud.transform;

        cloud.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = selectedCloudLevel.key_word;
        selectedCloudLevel.words  = Shuffle<string>(selectedCloudLevel.words);
        for (int i = 0; i < selectedCloudLevel.words.Count(); i++)
        {
            cloud = clouds_container.transform.GetChild(i).transform;
            cloud.gameObject.SetActive(true);
            cloud.GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.DEFAULT].outlineColor;
            //cloud.GetComponent<Image>().color = Color.white;
            cloud.GetChild(0).GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.DEFAULT].primaryColor;
            cloud.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = selectedCloudLevel.words[i];
            cloud.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.DEFAULT].textColor;
            cloud.GetComponent<CloudItem>().m_clicked = false; // Reset clicked
        }
        turns_iterator++;
    }

    public void DesactivateAllClouds()
    {
        int maxCloud = 8;
        for (int i = 0; i < maxCloud; i++)
        {
            Transform cloud = clouds_container.transform.GetChild(i).transform;
            cloud.gameObject.SetActive(false);
        }
    }

    public static T[] Shuffle<T>(T[] _list)
    {
        for (int i = 0; i < _list.Length; i++)
        {
            T temp = _list[i];
            int randomIndex = UnityEngine.Random.Range(i, _list.Length);
            _list[i] = _list[randomIndex];
            _list[randomIndex] = temp;
        }

        return _list;
    }



    //Load Data for the next turn 
    public void NextTurn()
   {
       //@Get another random quiz
       selectedCloudLevel = GetRandomQuizItem();
       
       //@AND add this data to cloud :::: THIS AUTOMATICALLY RESET CURRENT DATA 
       AddWordToCloud();
   }

  IEnumerator INextTurn()
    {
        yield return new  WaitForSeconds(0.5f);
        m_is_game_completed = false;
        NextTurn();
    }
   //Chec whenever we pass to the next turn or we end the current game level
   public void CheckLevel()
   {
        //Debug.Log("iterator" + turns_iterator);
        if (quiz_rate == 0 || numberOfRightChoices == selectedCloudLevel.related.Length )
        {
            //Save quiz_rate
            total_quizes_rate += quiz_rate;
            if(total_quizes_rate <= 0) total_quizes_rate = 0;

            if (turns_iterator < numb_turns)
            {
                // Instantiate next turn 
                m_is_game_completed = true;
                StartCoroutine(INextTurn());
            }
            else
            {
                // Goto next game : Request StageManager
                // Get final score_rate
                m_is_game_completed = true;
                StartCoroutine(GoToNextGame());
            }
        }
   }

    public void CheckWord(int cloud_index , string cloud_word)
    {
        if(!m_is_game_completed)
        {

            Transform btnGameObject;
            btnGameObject = clouds_container.transform.GetChild(cloud_index);

            if (selectedCloudLevel.related.ToList().FindIndex(r => r == cloud_word) != -1)
            {
                if (btnGameObject.GetComponent<Image>().color != Color.green)
                {
                    numberOfRightChoices++;
                    m_total_words_found = m_total_words_found + 1;
                    Hint_text.transform.GetComponent<Text>().text = m_total_words_found + "/" + m_total_words_to_find;
                    btnGameObject.GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.GOOD_ANWSER].outlineColor;
                    btnGameObject.transform.GetChild(0).GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.GOOD_ANWSER].primaryColor;
                    btnGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.GOOD_ANWSER].textColor;
                    m_feedbackSFX.clip = m_correctSFX;  //assign the clip value
                    m_feedbackSFX.Play();  //play the current clip
                                           // btnGameObject.GetComponent<Image>().color = Color.green;

                }
            }
            else
            {
                if (btnGameObject.GetComponent<Image>().color != Color.red)
                {
                    quiz_rate -= malus_per_wrong_answer;

                    btnGameObject.GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.BAD_ANWSER].outlineColor;
                    btnGameObject.transform.GetChild(0).GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.BAD_ANWSER].primaryColor;
                    btnGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.BAD_ANWSER].textColor;

                    m_feedbackSFX.clip = m_incorrectSFX;  //assign the clip value
                    m_feedbackSFX.Play();  //play the current clip
                                           // btnGameObject.GetComponent<Image>().color = Color.red;
                                           //left_wrong_choices = left_wrong_choices - 1;
                                           //Hint_text.transform.GetComponent<Text>().text = (left_wrong_choices).ToString();

                }

            }
            CheckLevel();
        }
    }


     //@Gets a random quiz to display
     public Word_clouds_quiz_item GetRandomQuizItem()
     {
     
         int max_index = numb_turns;
         int random_index = UnityEngine.Random.Range(0 , (max_index)); //+1

        while (isGenerated(random_index))
        {
            random_index = UnityEngine.Random.Range(0, (max_index)); //+1
        }
        //}

        Word_clouds_quiz_item item = m_wordCloudLevel[random_index];
        //@Add the generated index to the list
        m_generatedIndex.Add(random_index);
        return item;
     }

//@Verify if the index is already generated
public bool isGenerated(int index)
{
        foreach(int i in m_generatedIndex)
        {
            if(i == index)
            {
                return true;
            }
        }
        return false;
}




 
//@ Set Levels transition
public IEnumerator GoToNextGame()
{
        float delay = 1f;
        yield return new WaitForSeconds(delay);
        if(StageManager.instance != null)
        {
            string current_module_name = StageManager.instance.GetString("current_module_name");
            int current_level_index = StageManager.instance.GetInt("current_level_index");
            string current_game_name = StageManager.instance.GetString("current_game_name");
            float current_game_score = total_quizes_rate / numb_turns;
            current_game_score = Mathf.Round(current_game_score);
            StageManager.instance.LoadNextGame(current_module_name, current_level_index, current_game_name, current_game_score);
        }
}
}



[System.Serializable]
public class Word_clouds_quiz
{
    public Word_clouds_quiz_item[] quizes;
}

[System.Serializable]
public class Word_clouds_quiz_item
{
    public string key_word;
    public string gameDifficulty;
    public string[] words;
    public string[] related;
}
 
