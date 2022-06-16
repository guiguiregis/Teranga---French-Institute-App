using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreenManager : MonoBehaviour
{
   
    const float m_splashDuration = 0.5f;

    public string game_name;
    public string game_data;

    public Image splash_icon;
    
    void Awake() {



    }
    // Start is called before the first frame update
    void Start()
    {
        
        game_name = StageManager.instance.GetString("current_game_name");
        game_data = StageManager.instance.GetString("current_game_data");

        string splash_icon_sprite_path = "Game_presets/Games_icons/" + game_name + "_icon";

        Sprite splash_icon_sprite = Resources.Load<Sprite>(splash_icon_sprite_path);

        splash_icon.sprite = splash_icon_sprite;

        StartCoroutine(IEStartGame());
    }
   
    IEnumerator IEStartGame()
    {
        yield return new WaitForSeconds(m_splashDuration);
        StartGame();
    }

    public void StartGame()
    {
       
      

        // The scene name for the loading game 
        string game_scene_name = game_name ;
        // Hidden_objects specific case
        if (game_name == "Hidden_objects")
        {
            game_scene_name = "Ho_" + game_data;
        }

        // Set scene path
        string scenePath = "Scenes/Games/" + game_name + "/Scenes/" + game_scene_name;

        SceneManager.LoadScene(scenePath);

    }
   
}
