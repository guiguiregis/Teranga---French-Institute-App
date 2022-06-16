using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CheckCloud : MonoBehaviour
{


    // public void CheckIfTheClickedCloudIsCorrect(string buttonName)
    // {
    //     int currentWordIndex = GenerateClouds.Instance.selectedWordIndexes[GenerateClouds.Instance.currentWordIndex];
    //     GameObject btnGameObject;
    //     btnGameObject = GameObject.Find(buttonName);
    //     string ButtonText = btnGameObject.gameObject.GetComponentInChildren<TextMeshProUGUI>().text;
    //     for(int i = 0; i < GenerateClouds.Instance.m_wordCloudLevel[currentWordIndex].relatedWord.Count; i++)
    //     {
    //         if(ButtonText == GenerateClouds.Instance.m_wordCloudLevel[currentWordIndex].relatedWord[i].word)
    //         {
    //             if(GenerateClouds.Instance.m_wordCloudLevel[currentWordIndex].relatedWord[i].relatedToKeyWord)
    //             {
    //                 if (btnGameObject.GetComponent<Image>().color != Color.green)
    //                 {
    //                     GenerateClouds.Instance.numberOfRightChoices++;
    //                     btnGameObject.GetComponent<Image>().color = Color.green;
    //                 }

    //             }
    //             else
    //             {
    //                 if(btnGameObject.GetComponent<Image>().color != Color.red)
    //                 {
    //                     GenerateClouds.Instance.scoreRate -= 25;
    //                     if (GenerateClouds.Instance.scoreRate == 0)                                  //if the score rate is equal to 0 than we should decrease the mini game score
    //                     {
    //                         MiniGameManager.Instance.m_gameSettings[MiniGameManager.Instance.currentMiniGameIndex].DecreaseMiniGameScoring();   //If the player has failed one turn than
    //                                                                                                      //we should decrease the score
    //                     }
    //                     btnGameObject.GetComponent<Image>().color = Color.red;
    //                 }
    //             }
    //         }
    //     }
    // }
}
