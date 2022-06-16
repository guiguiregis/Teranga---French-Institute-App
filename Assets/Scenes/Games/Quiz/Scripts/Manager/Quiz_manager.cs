using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Kayfo;

public class Quiz_manager : Singleton<Quiz_manager>
{
    //LIST OF QUIZ 
    public List<Quiz> quizzes = new List<Quiz>();

    //LIST OF GENERATED WORD INDEXES
    public List<int> generatedIndex = new List<int>();

    //CURRENT SENTENCE INDEX TO KNOW WHICH SENTENCE IS GENERATED
    public int currentSentenceIndex;
    //NUMBER OF TURN SO THAT AT EACH END OF GAME WE SHOULD GENERATE A NEW WORD
    public static int numberOfTurn = 3;
    //COUNTER TO KNOW HOW MANY TURNS HAVE BEEN PLAYED
    int numberOfPlayedTurn;
    //THE REFERENCE FOR GLOBAL SCORE
    int totalScoreRef = 100;
    //THE TOTAL SCORE PREPRESENTS THE SUM OF ALL THE SCORES EARNED IN A MINIGAME
    int totalScore = 0;

    //SCORE OF THE CURRENT TURN
    int score;
    //THE MAX SCORE OF EACH TURN
    int scorePerTurnRef = 33;

    public bool m_is_choice_selected;   //check if the player has made a choice

    private void Start()
    {
        m_is_choice_selected = false;
       totalScore = 0;
        numberOfPlayedTurn = 1;

        //GETTING THE NUMBER OF TURN
        if(StageManager.instance != null)
        {
            numberOfTurn = StageManager.instance.GetInt("current_game_turns");
            scorePerTurnRef = totalScoreRef / numberOfTurn;
        }
        score = scorePerTurnRef;

        if (numberOfTurn > Quiz_manager.Instance.quizzes.Count)
        {
            numberOfTurn = Quiz_manager.Instance.quizzes.Count;
        }
    }

    public void DecreaseTheMiniGameScore()
    {
        score = 0;
    }

    public void GenerateSentenceIndex()
    {
        m_is_choice_selected = false;
        int genIndex;
        genIndex = Random.Range(0, Quiz_manager.Instance.quizzes.Count);
        while (CheckIfTheIndexIsAlreadyGenerated(genIndex))
        {
            genIndex = Random.Range(0, Quiz_manager.Instance.quizzes.Count);
        }

        score = scorePerTurnRef;
        currentSentenceIndex = genIndex;
        generatedIndex.Add(genIndex);

    }

    public bool CheckIfTheIndexIsAlreadyGenerated(int currentGeneratedIndex)
    {
        bool isExist = false;
        int index = generatedIndex.Where(i => i == currentGeneratedIndex).FirstOrDefault();
        foreach (int i in generatedIndex)
        {
            if (i == currentGeneratedIndex)
            {
                isExist = true;
            }
        }
        return isExist;
    }

    public void CheckIfTheNumberOfTurnsHasBeenReached()
    {

        totalScore += score;

        if (numberOfPlayedTurn < numberOfTurn)
        {
            numberOfPlayedTurn++;
            //WE SHOULD GENERATE THE NEXT TURN
           GoToNextTurn();
        }
        else
        {
           StartCoroutine(IGoToNextGame());
        }
    }

    public void GoToNextTurn()
    {
        GenerateSentenceIndex();
        FindObjectOfType<Quiz_dataloader>().InitializeSentenceLayout();
        FindObjectOfType<Quiz_dataloader>().InitializeAnwserLayout();
    }
    
    IEnumerator IGoToNextGame()
    {
        yield return new WaitForSeconds(0.3f);
        GoToNextGame();

    }
    public void GoToNextGame()
    {
        string current_module_name = PlayerPrefs.GetString("current_module_name");
        int current_level_index = PlayerPrefs.GetInt("current_level_index");
        string current_game_name = "Quiz";
        int current_game_score = totalScore;
        if (StageManager.instance != null)
        {
            StageManager.instance.LoadNextGame(current_module_name, current_level_index, current_game_name, current_game_score);
        }

    }
 


}
