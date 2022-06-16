using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject moduleBtns;
    public Game_module_struct[] modulesData;
    public Dropdown levels_dropdown;

    int level_index = 0;


   void Start()
    {
        LoadMenu();
    }

    public void LoadMenu()
    {
        modulesData = StageManager.instance.game_structure.modules;
        for(int i = 0; i < moduleBtns.transform.childCount; i++)
        {
            Transform moduleButton = moduleBtns.transform.GetChild(i);
            moduleButton.GetComponentInChildren<Text>().text = modulesData[i].name;
        }
    }

    public void GetLevelIndex()
    {
        level_index = levels_dropdown.value;
    }

    public void LoadModule(string module_name)
    {
        StageManager.instance.LoadModule(module_name, level_index);
    }

}
