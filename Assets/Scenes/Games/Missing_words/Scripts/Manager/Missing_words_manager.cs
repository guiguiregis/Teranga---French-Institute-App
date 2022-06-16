using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missing_words_manager : MonoBehaviour
{

    public static GameObject missingWordsContainer;
    //NUMBER OF TURN SO THAT AT EACH END OF GAME WE SHOULD GENERATE A NEW WORD
    public static int numberOfTurn = 1;
    //COUNTER TO KNOW HOW MANY TURNS HAVE BEEN PLAYED
    public static int numberOfPlayedTurn;
    //THE SCORE REPRESENTS THE SCORE EARNED EACH TURN
    public static int score;
    //THE TOTAL SCORE PREPRESENTS THE SUM OF ALL THE SCORES EARNED IN A MINIGAME
    public static int totalScore = 0;
    //WE DECREASE THE FOLLOWING BY THE FOLLOWING VALUE
    public static int decreaseRate = 25;

    //STATIC BOOL TO CHECK IF THE SCORE HAS ALREADY BEEN INCREASED
    public static bool hasIncreasedScore = false;
    

    private void Start()
    {
        totalScore = 0;
        score = 100;
        //INITIALIZE THE NUMBER OF PLAYED TURN TO 0
        numberOfPlayedTurn = 1;
        if(StageManager.instance != null)
        {
            numberOfTurn = StageManager.instance.GetInt("current_game_turns");
        }
        missingWordsContainer = GameObject.Find("Missing_words_container");

    }

    public static void DecreaseTheMiniGameScore()
    {
        score = score - decreaseRate;   //If the player has failed one turn we should decrease 
        if (score < 0)
        {
            score = 0;
        }
    }

    /// <summary>
    /// CHECK IF THE PLAYER HAS COMPLETED THE SENTENCE
    /// </summary>
    public static bool CheckIfThePlayerHasCompletedTheSentence()
    {
        int counter = 0;
        bool isCompleted = false;
        DragAndDrop[] wordBoxesScript = missingWordsContainer.GetComponentsInChildren<DragAndDrop>();
        foreach(DragAndDrop g in wordBoxesScript)
        {
            if(g.isDrop)
            {
                counter++;
            }
        }
        WordSlot[] slots = FindObjectsOfType<WordSlot>(); 
        if(slots.Length == counter)
        {
            isCompleted = true;
        }
        else
        {
            isCompleted = false;
        }
        return isCompleted;
    }

    /// <summary>
    /// WE GENERATE A NEXT SENTENCE IF THE NUMBER OF TURN IS NOT REACHED
    /// </summary>
    public static IEnumerator GenerateNextSentence()
    {
        float delay = 0.3f;
        yield return new WaitForSeconds(delay);
        if(!hasIncreasedScore)
        {
            totalScore += score;
            score = 100;
            if (numberOfPlayedTurn < numberOfTurn)
            {
                numberOfPlayedTurn++;
                //WE SHOULD GENERATE THE NEXT SENTENCE

                FindObjectOfType<Missing_words_dataloader>().GenerateGameplayElementsOnScene();
            }
            else
            {
                GoToNextGame();
            }
        }
    }

    public static  void GoToNextGame()
    {

        string current_module_name = PlayerPrefs.GetString("current_module_name");
        int current_level_index = PlayerPrefs.GetInt("current_level_index");
        string current_game_name = PlayerPrefs.GetString("current_game_name");

        int current_game_score = totalScore / numberOfTurn;

        if(StageManager.instance != null)
        {
            StageManager.instance.LoadNextGame(current_module_name, current_level_index, current_game_name, current_game_score);

        }
    }
}
