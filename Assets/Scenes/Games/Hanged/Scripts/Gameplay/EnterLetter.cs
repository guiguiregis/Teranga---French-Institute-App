using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class EnterLetter : MonoBehaviour
{
    public AudioSource m_feedbackSFX;
    public AudioClip m_correctSFX;
    public AudioClip m_incorrectSFX;
  
    public void EnterLetterToInput(string letter)
    {
        bool hasCorrectLetter = false;

        if (!GenerateRandomWord.Instance.m_resetButton.gameObject.activeSelf && !GenerateRandomWord.Instance.m_nextButton.gameObject.activeSelf)  //check if the reset or the next buttton is activated before making changes
        {
            if (GameObject.Find(letter[0].ToString()).GetComponent<Image>().color != Color.red)    //check if the button is marked red
            {
                for (int i = 0; i < GenerateRandomWord.Instance.m_generatedWord.Length; i++)
                {
                    if (letter.Where(s => s.Equals(GenerateRandomWord.Instance.m_generatedWord[i])).FirstOrDefault() != '\0')
                    {
                        hasCorrectLetter = true;
                        Transform letterField = GenerateRandomWord.Instance.m_lettersContainer.transform.GetChild(i);
                        TextMeshProUGUI letterFieldText = letterField.GetComponentInChildren<TextMeshProUGUI>();
                        if (letterFieldText.text == "")
                        {
                            GameObject.Find(letter[0].ToString()).GetComponent<Image>().color = Color.green;
                            letterFieldText.text = letter[0].ToString();
                            GenerateRandomWord.Instance.CheckIfThePlayerHasWonTheGame();
                        }
                        else
                        {
                            // Debug.Log("--0");
                            GameObject.Find(letter[0].ToString()).GetComponent<Image>().color = Color.green;

                        }
                        m_feedbackSFX.clip = m_correctSFX;  //assign the correct SFX to the audio source clip
                        m_feedbackSFX.Play();   //play the correct SFX
                    }
                    
                }

                if (!hasCorrectLetter)
                {
                    GenerateRandomWord.Instance.score -= GenerateRandomWord.Instance.decreaseRate;
                    if (GenerateRandomWord.Instance.score < 0)
                    {
                        GenerateRandomWord.Instance.score = 0;
                    }
                    GameObject.Find(letter[0].ToString()).GetComponent<Image>().color = Color.red;
                    GenerateRandomWord.Instance.IncreaseCurrentSpriteIndex();
                    GenerateRandomWord.Instance.CheckIfPlayeHasLost();

                    m_feedbackSFX.clip = m_incorrectSFX;  //assign the correct SFX to the audio source clip
                    m_feedbackSFX.Play();   //play the correct SFX
                }
            }
        }
    }
}
