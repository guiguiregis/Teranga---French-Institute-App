using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CarouselCtrl : MonoBehaviour
{
    public TextMeshProUGUI m_carouselText;
    public Button m_playButton;
    private void Start()
    {
        ShowRandomLevelOnCarousel();
    }


    public void ShowRandomLevelOnCarousel()
    {
        Progress_struct my_Progress = new Progress_struct();
        my_Progress = ProgressManager.Instance.GetProgressStruct();
        List<Progress_module_struct> randModule = new List<Progress_module_struct>();
        int randInd = 0;
        int level_ind = 0;
        if (!IsModulesCompleted(my_Progress.modules))    //if all the modules aren't completed
        {
            foreach (Progress_module_struct m in my_Progress.modules)
            {
                if (ProgressManager.Instance.GetModuleGlobalProgress(m.name) < 100)
                {
                    randModule.Add(m);
                }
            }
            Progress_level_struct chosenLevel = new Progress_level_struct();
            if (randModule.Count > 0)
            {
                randInd = Random.Range(0, randModule.Count);
                int c = 0;
                foreach (Progress_level_struct l in randModule[randInd].levels)
                {
                    //if(StageManager.instance.m_notAvailableLevels.Where(s => s.ToLower().Equals(StageManager.instance.GetNoAvailableLevelKey(randModule[randInd].name, c).ToLower())).FirstOrDefault() != null)//we should make sure that the level index exist 
                    {
                        if ((SumFloats(l.progress) / StageManager.instance.GetNumberOfAvailableLevels(randModule[randInd].name, false)) != 100 && l.unlocked && c < StageManager.instance.GetNumberOfAvailableLevels(randModule[randInd].name, false)) //if the level is unlocked and not completed than we should play that level
                        {
                            chosenLevel = l;
                            level_ind = c;
                        }
                        c++;
                    }
                }
            }
            else
            {
                //The player hasn't started a module than we choose the first module and the first level
                randModule.Add(my_Progress.modules[0]);
                chosenLevel = my_Progress.modules[0].levels[0];
                level_ind = 0;
            }
            int module_global_progress = (int)(ProgressManager.Instance.GetModuleGlobalProgress(randModule[randInd].name)); //get the module progress
            if(module_global_progress > 0)  //verified if the module progress is greater than 0
            {
                m_carouselText.text = "Continue ton parcours sur le module " + randModule[randInd].name.ToLower();//show the carousel message
            }
            else
            {
                m_carouselText.text = "Commences ton parcours sur le module " + randModule[randInd].name.ToLower();//show the carousel message
            }

            m_playButton.onClick.AddListener(() => { StartModuleLevel(randModule[randInd].name, level_ind);  });
        }
        else
        {
            //if all the modules are completed we should display a text
        }
        StageManager.instance.SetString("current_module_name", randModule[randInd].name);
    }


    public bool IsModulesCompleted(List<Progress_module_struct> _modules)
    {
        //GetModuleGlobalProgress
        float global_progress = 0;
        foreach(Progress_module_struct m in _modules)
        {
            global_progress += ProgressManager.Instance.GetModuleGlobalProgress(m.name);
        }
        if(global_progress/_modules.Count == 100)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public float SumFloats(List<float> _floats)
    {
        float sum = 0;
        foreach(float f in _floats)
        {
            sum += f;
        }
        return sum;
    }

    public void StartModuleLevel(string _module_name, int _level_index)
    {
        // Display StoryTelling and Start Intro
        StoryTellerCtrlr.Instance.m_storyIntroduced = false;
        StoryTellerCtrlr.Instance.InitializeStorytelling();
        StoryTellerCtrlr.Instance.LaunchIntro(_module_name,_level_index);
        StageManager.instance.m_currentMiniGameIndex.Set(0);    //reset the mini-game index
        StageManager.instance.LoadModule(_module_name, _level_index);
    }

}
