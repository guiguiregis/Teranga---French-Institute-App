using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kayfo;
public class ProfileCtrlr : MonoBehaviour
{
    public List <Sprite> m_avatarsList;
    public Image m_avatar;

    public Image m_progressFill;
    public PersistentFloat m_last_player_level_progression = new PersistentFloat("PROFILE_PLAYER_DOTNUT_PROGRESSION", 0f);
    // Start is called before the first frame update
    void Start()
    {
        int player_index = ProgressManager.Instance.m_persistentCharacterIndex.Get();
        
        float PlayerProgression_stats = ProgressManager.Instance.GetPlayerGlobalProgress();

        float max = PlayerProgression_stats/100;

        m_avatar.sprite = m_avatarsList[player_index];

        if (PlayerProgression_stats == 0)
        {
            // PlayerProgression_stats_text.gameObject.SetActive(true);
            m_progressFill.fillAmount = 0f;
        }
        if (max != m_last_player_level_progression.Get())
        {
            StartCoroutine(TweenPlayerProgressionStats(max));
            m_last_player_level_progression.Set(max);   //set the last player progression
        }
        else
        {
            m_progressFill.fillAmount = max;
        }
    }



    IEnumerator TweenPlayerProgressionStats(float max)
    {


        float progress = 0f;
        float progress_fill = 0f;
        float duration = 3;
        float step = max / duration;

        while (progress < max)
        {
            progress += (step * Time.deltaTime);
            progress_fill += (step * Time.deltaTime);
            m_progressFill.fillAmount = progress_fill;

            if (progress_fill >= 1)
            {
                // PlayerProgression_stats_text.gameObject.SetActive(true);
                //StopCoroutine(TweenPlayerProgressionStats(0));
                m_progressFill.fillAmount = 0;
                progress_fill = 0;
            }

            yield return 0;
        }

    }

}
