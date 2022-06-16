using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Kayfo;
using System.Linq;
using TMPro;

public class GenerateImage : Singleton<GenerateImage>
{

    public List<OnePicFourWordsGameLevel> m_onePicFourWordsGameLevels = new List<OnePicFourWordsGameLevel>();
    public int numbOfTurns = 2;
    public Image m_guessedImage;
    private string m_keyWord;

    public List<TextMeshProUGUI> m_words;
    int m_lastGeneratedLvelIndex;           //variable to get the last generated level index
    int m_generatedLevelIndex;
    public GameObject m_replayButton;
 
    //THE TOTAL SCORE PREPRESENTS THE SUM OF ALL THE SCORES EARNED IN A MINIGAME
    public int currentTurn = 1;
    //WE DECREASE THE FOLLOWING BY THE FOLLOWING VALUE 
    int totalScoreRef = 100;
    //THE TOTAL SCORE PREPRESENTS THE SUM OF ALL THE SCORES EARNED IN A MINIGAME
    int totalScore = 0;

    //SCORE OF THE CURRENT TURN
    int score;
    //THE MAX SCORE OF EACH TURN
    int scorePerTurnRef = 100;
    
    enum WORDS {
        WORD1 = 0,
        WORD2,
        WORD3,
        WORD4
    }

    public AudioSource m_feedbackSFX;
    public AudioClip m_correctSFX;
    public AudioClip m_incorrectSFX;
    public bool m_is_game_completed;   //check if the player has made a choice

    private void Start()
    {
        
        if(StageManager.instance != null)
        {
            numbOfTurns = StageManager.instance.GetInt("current_game_turns");
            if (numbOfTurns > m_onePicFourWordsGameLevels.Count) //verify if the number of turns is greater than the available data
            {
                numbOfTurns = m_onePicFourWordsGameLevels.Count;
            }
            scorePerTurnRef = totalScoreRef / numbOfTurns;
        }
        score = scorePerTurnRef;
        m_lastGeneratedLvelIndex = 99;
        GenerateRandomLevel();
        if(numbOfTurns > m_onePicFourWordsGameLevels.Count) //verify if the number of turns is greater than the available data
        {
            numbOfTurns = m_onePicFourWordsGameLevels.Count;
        }
    }


    public void GenerateRandomLevel()
    {

        score = scorePerTurnRef;
        ResetWordColor();
        m_generatedLevelIndex =  Random.Range(0, m_onePicFourWordsGameLevels.Count);
        //if(m_onePicFourWordsGameLevels.Where(s => s.gameDifficulty.ToLower().Equals(StageManager.instance.GetGameCurrentDifficulty())).FirstOrDefault() != null)    //if the game difficulty exist
        //{
        //    while (m_generatedLevelIndex == m_lastGeneratedLvelIndex && !m_onePicFourWordsGameLevels[m_generatedLevelIndex].gameDifficulty.ToLower().Equals(StageManager.instance.GetGameCurrentDifficulty()))
        //    {
        //        m_generatedLevelIndex = Random.Range(0, m_onePicFourWordsGameLevels.Count);
        //    }
        //}
        //else
        //{
            //Debug.LogWarning("The The level of difficulty " + StageManager.instance.GetGameCurrentDifficulty() + " does not exist in the content");
            while (m_generatedLevelIndex == m_lastGeneratedLvelIndex)
            {
                m_generatedLevelIndex = Random.Range(0, m_onePicFourWordsGameLevels.Count);
            }
        //}

        m_guessedImage.sprite = m_onePicFourWordsGameLevels[m_generatedLevelIndex].m_levelImage;
        SetKeyWord(m_onePicFourWordsGameLevels[m_generatedLevelIndex].keyword);

        m_onePicFourWordsGameLevels[m_generatedLevelIndex].m_fourWords = Shuffle<string>(m_onePicFourWordsGameLevels[m_generatedLevelIndex].m_fourWords);
         
        for (int i = 0; i < m_words.Count; i++)
        {
          m_words[i].text = m_onePicFourWordsGameLevels[m_generatedLevelIndex].m_fourWords[i];
        }
        m_lastGeneratedLvelIndex = m_generatedLevelIndex;
        m_is_game_completed = false;

    }

    public void ResetWordColor()
    {
        for (int i = 0; i < m_words.Count; i++)
        {
           //m_words[i].transform.parent.GetComponent<Image>().color = Color.white;
            m_words[i].transform.parent.GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.DEFAULT].outlineColor;
            m_words[i].transform.parent.transform.GetChild(0).GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.DEFAULT].primaryColor;
            m_words[i].GetComponent<TextMeshProUGUI>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.DEFAULT].textColor;
        }
    }

    void SetKeyWord(string key)
    {
        this.m_keyWord = key;
    }

    /// <summary>
    /// Shuffle a list of elements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_list"></param>
    /// <returns></returns>
    public static List<T> Shuffle<T>(List<T> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            T temp = _list[i];
            int randomIndex = Random.Range(i, _list.Count);
            _list[i] = _list[randomIndex];
            _list[randomIndex] = temp;
        }

        return _list;
    }

    string GetKeyWord()
    {
        return this.m_keyWord;
    }

    public void WordPressed(int word_index)
    {
        if(!m_is_game_completed)
        {
            string word = m_onePicFourWordsGameLevels[m_generatedLevelIndex].m_fourWords[word_index];
            if (word == m_keyWord)
            {
                m_words[word_index].transform.parent.GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.GOOD_ANWSER].outlineColor;
                m_words[word_index].transform.parent.transform.GetChild(0).GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.GOOD_ANWSER].primaryColor;
                m_words[word_index].GetComponent<TextMeshProUGUI>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.GOOD_ANWSER].textColor;
                m_feedbackSFX.clip = m_correctSFX;  //assign the clip value
                m_feedbackSFX.Play();  //play the current clip
            }
            else
            {
                m_words[word_index].transform.parent.GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.BAD_ANWSER].outlineColor;
                m_words[word_index].transform.parent.transform.GetChild(0).GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.BAD_ANWSER].primaryColor;
                m_words[word_index].GetComponent<TextMeshProUGUI>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.BAD_ANWSER].textColor;
                m_feedbackSFX.clip = m_incorrectSFX;  //assign the clip value
                m_feedbackSFX.Play();  //play the current clip
                DecreaseTheMiniGameScore();
            }
            StartCoroutine(NextTurnLoader());
        }
        m_is_game_completed = true;

    }

    IEnumerator NextTurnLoader()
    {
        float delay = 1f;
        yield return new WaitForSeconds(delay);
        CheckIfNumberOfTurnsHasBeenReached();

    }

    public void CheckIfNumberOfTurnsHasBeenReached()
    {

        totalScore += score;

        if (currentTurn < numbOfTurns)
        {
            currentTurn++;
            GenerateRandomLevel();
        }
        else
        {
            //StartCoroutine(NextGameLoaderDelay());
            GoToNextGame();
        }

    }

    public void DecreaseTheMiniGameScore()
    {
            score = 0;
    }

    public void GoToNextGame()
    {

        string current_module_name = PlayerPrefs.GetString("current_module_name");
        int current_level_index = PlayerPrefs.GetInt("current_level_index");
        string current_game_name = PlayerPrefs.GetString("current_game_name");
        int current_game_score = totalScore;


        //CHECKING IF THE STAGE MANAGER IS EQUAL TO NULL  BEFORE LOADING NEXT SCENE
        if(StageManager.instance != null)
        {
            StageManager.instance.LoadNextGame(current_module_name, current_level_index, current_game_name, current_game_score);  // TODO: doesn't the StageManager knows these things?
        }

    }
}


[System.Serializable]
public class OnePicFourWordsGameLevel
{
    public Sprite m_levelImage;
    public List<string> m_fourWords = new List<string>();
    public string gameDifficulty;
    public string keyword;
}
