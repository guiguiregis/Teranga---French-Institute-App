using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class Quiz_dataloader : MonoBehaviour
{
    //THE GAME DATA PATH IN THE RESOURCE FOLDER
    public string gameDataPath = "Games\\Quiz\\";
    //THE GAME JSON FILE IN THE RESOURCE FOLDER
    string gameLevelPath = "\\Quiz";
    //VARIABLE TO DEFINE TH E QUIZ DATA STRUCT
    public QuizDataStruct loadedData;

    //GAME LEVEL DIFFICULTY
    public int gameLevel = 0;

    //GAMEOBJECT TO CONTAIN THE GENERATED SENTENCE
    public GameObject sentenceContainer;

    //ROW SENTENCE PREFAB
    public GameObject sentence;

    //DEFAULT GAME DATA PATH
    private string defaultGameDataPath = "Games\\Quiz\\Communication\\Level_3\\Quiz";

    //GAMEOBJECT TO CONTAIN THE ANWSERS LAYOUT
    public GameObject anwsersContainer;

    //GAMEOBJECT PREFAB
    public GameObject anwserButton;



    private void Awake()
    {
        string path = "";
        //IF THE STAGE MANAGER IS EQUAL TO NULL THEN WE USE THE DEFAULT DATA PATH IN ORDER TO LOAD LEVEL DATA
        if(StageManager.instance != null)
        {
            
            gameLevel = StageManager.instance.GetCurrentLevelIndex() + 1;
            string levelIndex = "\\Level_" + gameLevel;
            path = gameDataPath + StageManager.instance.GetString("current_module_name") + levelIndex + gameLevelPath;
        }
        else
        {
            path = defaultGameDataPath;
        }

        LoadQuizData(path);
        InitializeQuizGameData();
        Quiz_manager.Instance.GenerateSentenceIndex();
        InitializeSentenceLayout();
        InitializeAnwserLayout();
    }


    /// <summary>
    /// LOAD THE QUIZ DATA BASED ON THE GIVEN PATH
    /// </summary>
    /// <param name="path"></param>
    public void LoadQuizData(string path)
    {
        //string missingWordpath = gameDataPath + Path.DirectorySeparatorChar + path;
        TextAsset json = Resources.Load(path) as TextAsset;
        if (json != null)
        {
            loadedData = JsonUtility.FromJson<QuizDataStruct>(json.text);
        }
        else
        {
            Debug.Log("CANNOT FIND THE FILE " + path);
        }
    }

    //INITIALIZE QUIZ GAME DATA 
    public void InitializeQuizGameData()
    {
        //if(loadedData.quiz_sentence.Where(q => q.gameDifficulty.ToLower().Equals(StageManager.instance.GetGameCurrentDifficulty().ToLower())).FirstOrDefault() != null)
        //{
        //    foreach (Quiz q in loadedData.quiz_sentence)
        //    {
        //        if (q.gameDifficulty.ToLower().Equals(StageManager.instance.GetGameCurrentDifficulty().ToLower()))
        //        {
        //            Quiz_manager.Instance.quizzes.Add(q);
        //        }
        //    }
        //}
        //else
        //{
        //    Debug.LogWarning("The The level of difficulty " + StageManager.instance.GetGameCurrentDifficulty() + " does not exist in the content");
            foreach (Quiz q in loadedData.quiz_sentence)
            {
                    Quiz_manager.Instance.quizzes.Add(q);
            }
        //}
    }

 

    public void InitializeSentenceLayout()
    {

        //WE COUNT THE NUMBER OF WORDS AND IF WE REACH 8 THEN WE SHOULD CREATE A NEW ROW AND ATTACH
        //THE FOLLOWING WORDS TO IT
        sentence.GetComponent<TextMeshProUGUI>().text = Quiz_manager.Instance.quizzes[Quiz_manager.Instance.currentSentenceIndex].sentence;
    }


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
    public void InitializeAnwserLayout()
    {
        
        //BEFORE CREATING NEW BUTTONS, WE SHOULD DESTROY THE LAST GENERATED BUTTONS
        foreach (Transform child in anwsersContainer.transform)
        {
            //Destroy(child.gameObject);
            child.gameObject.SetActive(false);
        }
        List<Anwser> answers =  Quiz_manager.Instance.quizzes[Quiz_manager.Instance.currentSentenceIndex].anwser.ToList();
        answers = Shuffle(answers);
        int iterator = 0;
        foreach (Anwser a in answers)
        {
            
            Transform btnAnwser = anwsersContainer.transform.GetChild(iterator);
            btnAnwser.gameObject.SetActive(true);
            btnAnwser.GetComponentInChildren<TextMeshProUGUI>().text = a.text_content;
            btnAnwser.GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.DEFAULT].outlineColor;
            btnAnwser.transform.GetChild(0).GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.DEFAULT].primaryColor;
            btnAnwser.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.DEFAULT].textColor;
            btnAnwser.GetComponent<Anwser_button>().isCorrect = a.is_correct;
            //btnAnwser.GetComponent<Button>().onClick.AddListener(() => { GetButton(a.is_correct); });

            iterator++;

        }
    }
 
}
