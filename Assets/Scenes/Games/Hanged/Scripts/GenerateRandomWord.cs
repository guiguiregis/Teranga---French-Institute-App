using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kayfo;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GenerateRandomWord : Singleton<GenerateRandomWord>
{
    public List<GameObject> inputTextField = new List<GameObject>();
    public List<Sprite> m_hangManSprites = new List<Sprite>();
    public GameObject m_lettersContainer ;
    public GameObject m_resetButton;
    public GameObject m_nextButton;
    public string m_generatedWord;
    private int m_lastGeneratedIndex;
    public int currentHangmanSpriteIndex;
    public List<Image> m_hangmanImage;
    private int lastHangManSpriteIndex = 6;
    public List<string> m_words = new List<string>();
    public List<WordsList> m_listOfWords = new List<WordsList>();
    public Sprite m_UIMaskSprite;
    //THE SCORE REPRESENTS THE SCORE EARNED EACH TURN
    public int score;
    //THE TOTAL SCORE PREPRESENTS THE SUM OF ALL THE SCORES EARNED IN A MINIGAME
    public int totalScore = 0;
    //KEEPS TRACK OF THE CURRENT TURN INDEX
    public int currentTurn = 1;
    //WE DECREASE THE FOLLOWING BY THE FOLLOWING VALUE
    public int decreaseRate = 25;
    //VARIABLE IN ORDER TO GET THE NUMBER OF TURNS
    public int numbTurn;
    //DEFAULT NUMBER OF TURN 
    public int defaultNumberOfTurn = 3;

    enum HANGMAN_IMAGES
    {
        HANGMAN_0 = 0,
        HANGMAN_1,
        HANGMAN_2,
        HANGMAN_3,
        HANGMAN_4,
        HANGMAN_5,
        HANGMAN_6
    }


    private void Start()
    {
        if(StageManager.instance != null)
        {
            numbTurn = StageManager.instance.GetInt("current_game_turns");
        }
        else
        {
            numbTurn = defaultNumberOfTurn;
        }
        score = 100;
        currentHangmanSpriteIndex = 0;
        m_lastGeneratedIndex = 99;
        m_generatedWord = ReturnGeneratedWord();
    }


    private void Update()
    {
        //m_hangmanImage.sprite = m_hangManSprites[currentHangmanSpriteIndex];            //change hangman image based on the image
        AddCorrespondingHangmanImages(currentHangmanSpriteIndex);           //adding images sprites
        /*if (currentHangmanSpriteIndex == lastHangManSpriteIndex)
        {
            m_resetButton.gameObject.SetActive(true);
        }
        else
        {
            m_resetButton.gameObject.SetActive(false);
        }*/
    }

    public void CheckIfPlayeHasLost()
    {
        if (currentHangmanSpriteIndex == lastHangManSpriteIndex)
        {
            //NextTurn();
            StartCoroutine(DelayBeforeNextTurn());
        }
    }


    public void AddCorrespondingHangmanImages(int currentHangmanSprite)
    {
        switch(currentHangmanSprite)
        {
            case (int)HANGMAN_IMAGES.HANGMAN_0:
                m_hangmanImage[currentHangmanSprite].sprite = m_hangManSprites[currentHangmanSprite];
                break;
            case (int)HANGMAN_IMAGES.HANGMAN_1:
                m_hangmanImage[currentHangmanSprite].sprite = m_hangManSprites[currentHangmanSprite];
                break;
            case (int)HANGMAN_IMAGES.HANGMAN_2:
                m_hangmanImage[currentHangmanSprite].sprite = m_hangManSprites[currentHangmanSprite];
                break;
            case (int)HANGMAN_IMAGES.HANGMAN_3:
                m_hangmanImage[currentHangmanSprite].sprite = m_hangManSprites[currentHangmanSprite];
                break;
            case (int)HANGMAN_IMAGES.HANGMAN_4:
                m_hangmanImage[currentHangmanSprite].sprite = m_hangManSprites[currentHangmanSprite];
                break;
            case (int)HANGMAN_IMAGES.HANGMAN_5:
                m_hangmanImage[currentHangmanSprite].sprite = m_hangManSprites[currentHangmanSprite];
                break;
            case (int)HANGMAN_IMAGES.HANGMAN_6:
                m_hangmanImage[currentHangmanSprite].sprite = m_hangManSprites[currentHangmanSprite];
                break;
        }
    }

    public string ReturnGeneratedWord()
    {
        int index = Random.Range(0, m_words.Count);
        currentHangmanSpriteIndex = 0;
        while (index == m_lastGeneratedIndex)
        {
            index = Random.Range(0, m_words.Count);
        }
        m_lastGeneratedIndex = index;
        ActivateInputField(m_words[index].ToUpper());
        return m_words[index].ToUpper();
    }

    /// <summary>
    /// Activate the input field based on the generated word
    /// </summary>
    /// <param name="generatedWord"> string to know the number of generated input field</param>
    public void ActivateInputField(string generatedWord)
    {
        
        int lastCharPos = generatedWord.Length + 1;                 //initialisze the last char pos to a int different 
        int numberOfLetterToShow = generatedWord.Length / 2;
        int hiddenLetters = generatedWord.Length - numberOfLetterToShow;
        decreaseRate = score / hiddenLetters;
        //Debug.Log(decreaseRate);
         
        ResetInputField();
        ResetKeyboardColor();
        for (int i = 0 ; i < numberOfLetterToShow ; i++)
        {
            int charPos = Random.Range(0, generatedWord.Length);
            while(charPos == lastCharPos)
            {
                charPos = Random.Range(0, generatedWord.Length);
            }
            Transform letterField = m_lettersContainer.transform.GetChild(charPos);
            TextMeshProUGUI letterFieldText = letterField.GetComponentInChildren<TextMeshProUGUI>();
            letterFieldText.text = generatedWord[charPos].ToString();
            lastCharPos = charPos;
        }
      
        for (int i = 0; i < generatedWord.Length; i++)
        {
            // dash and space case
            Transform letterField = m_lettersContainer.transform.GetChild(i);
            TextMeshProUGUI letterFieldText = letterField.GetComponentInChildren<TextMeshProUGUI>();
            char c = generatedWord[i];
            if (c == '-' )
            {
              letterFieldText.text = generatedWord[i].ToString();
            }
            if (c == ' ' )
            {
              letterFieldText.text = generatedWord[i].ToString();

              Color transparent = Color.white;
              transparent.a = 0f;
              letterField.GetChild(0).GetComponentInChildren<Image>().color = transparent;
              letterField.GetChild(1).GetComponentInChildren<Image>().color = transparent;
            }
            // Activate textfields
            letterField.gameObject.SetActive(true);
          
           
            

        }

      
    }

    public void ResetInputText()
    {
        for (int i = 0; i < m_lettersContainer.transform.childCount; i++)
        {
            Transform letterField = m_lettersContainer.transform.GetChild(i);
            TextMeshProUGUI letterFieldText = letterField.GetComponentInChildren<TextMeshProUGUI>();
            letterFieldText.text = "";
        }
    }

    public void ResetKeyboardColor()
    {
        for (char c = 'A'; c <= 'Z'; c++)
        {
            GameObject.Find(c.ToString()).GetComponent<Image>().color = Color.white;
        }
    }

    public void ResetHangmanImage()
    {
        for(int i = 1; i < m_hangmanImage.Count; i++)
        {
                m_hangmanImage[i].sprite = m_UIMaskSprite;

        }
    }

    public void ResetGame()
    {
        currentHangmanSpriteIndex = 0;
        ResetHangmanImage();
        ResetInputText();
        ActivateInputField(m_generatedWord);
    }

    public void ResetInputField()
    {
        for(int i = 0; i< m_lettersContainer.transform.childCount; i++)
        {
            Transform letterField = m_lettersContainer.transform.GetChild(i);
            letterField.gameObject.SetActive(false);
        }
    }

    public void CheckIfThePlayerHasWonTheGame()
    {
        int counter = 0;
        for(int i = 0; i < m_lettersContainer.transform.childCount; i++)
        {
            Transform letterField = m_lettersContainer.transform.GetChild(i);
            TextMeshProUGUI letterFieldText = letterField.GetComponentInChildren<TextMeshProUGUI>();
            if(letterFieldText.text != "")
            {
                counter++;
            }
        }
        
        if(counter == m_generatedWord.Length)
        {
            //NextTurn();
            StartCoroutine(DelayBeforeNextTurn());
        }
     
    }

    public void IncreaseCurrentSpriteIndex()
    {
        if(currentHangmanSpriteIndex < m_hangManSprites.Count)
        {
            currentHangmanSpriteIndex++;
        }
    }


    public IEnumerator DelayBeforeNextTurn()
    {
        float delay = 1f;
        yield return new WaitForSeconds(delay);
        NextTurn();
    }
    public void NextTurn()
    {
        totalScore += score;

        if (currentTurn < numbTurn)
        {
            score = 100;
            ResetInputText();
            m_generatedWord = ReturnGeneratedWord();
            currentTurn++;
        }
        else
        {
            GoToNextLevel();
        }
    }

    //@ Set Levels transition
    public void GoToNextLevel()
    {

        string current_module_name = PlayerPrefs.GetString("current_module_name");
        int current_level_index = PlayerPrefs.GetInt("current_level_index");
        string current_game_name = PlayerPrefs.GetString("current_game_name");
        int current_game_score = totalScore / numbTurn;

        //CHECKING IF THE STAGE MANAGER IS DIFFERENT FROM NULL BEDFORE LOADING NEXT SCENE
        if(StageManager.instance != null)
        {
            StageManager.instance.LoadNextGame(current_module_name, current_level_index, current_game_name, current_game_score);
        }

    }


}


[System.Serializable]
public class WordsList
{
    public int difficulty;
    public List<string> words = new List<string>();

}
