using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kayfo;
public class AudioManager : Singleton<AudioManager>
{

    public AudioClip m_theme_song_for_menu_screen;
    public AudioClip m_theme_song_for_mini_game_screen;
    public List<string> m_menu_screen_names;
    private string m_current_scene_name;
    public PersistentBool m_SFXActivated = new PersistentBool("SFX", true);
    public PersistentBool m_musicActivated = new PersistentBool("MUSIC", true);

    private void Awake()
    {
        if(FindObjectsOfType<AudioManager>().Count() == 1)
        {
            DontDestroyOnLoad(this.transform.gameObject);
        }
        m_current_scene_name = SceneManager.GetActiveScene().name;

        if(!m_musicActivated.Get())//if the music is not activated then we should stop the current playing clip
        {
            this.GetComponent<AudioSource>().Stop();
        }

    }


    private void Update()
    {
        if(m_musicActivated.Get())
        {
            if (!m_current_scene_name.Equals(SceneManager.GetActiveScene().name))
            {
                m_current_scene_name = SceneManager.GetActiveScene().name;  //assign the current scene name
                if (m_menu_screen_names.Where(s => s.Equals(m_current_scene_name)).FirstOrDefault() == null)//check if the current scene is a menu screen or not
                {
                    if (this.GetComponent<AudioSource>().clip != m_theme_song_for_mini_game_screen)
                    {
                        this.GetComponent<AudioSource>().clip = m_theme_song_for_mini_game_screen;
                        this.GetComponent<AudioSource>().Play();
                    }
                }
                else
                {
                    if (this.GetComponent<AudioSource>().clip != m_theme_song_for_menu_screen)
                    {
                        this.GetComponent<AudioSource>().clip = m_theme_song_for_menu_screen;
                        this.GetComponent<AudioSource>().Play();
                    }
                }
            }
        }
    }
}
