using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using UnityEditor;
using Kayfo;

public enum MINI_GAMES_INDEX
{
    HANGED = 0,
    MISSING_WORDS = 1,
    ONE_PIC_FOUR_WORDS = 2,
    QUIZ = 3,
    WORD_CLOUDS = 4
}
public class StageManager : MonoBehaviour
{
    public float time = 0.1f;
    public float timer;
    public static StageManager instance;
    public Game_struct game_structure;
    public Game_struct m_generated_game_structure;
    public string game_struct_path = "Game_presets";
    public List<BtnColor> m_btnColors;
    private List<string> m_miniGamesNames = new List<string>() { "Hanged", "Missing_words", "One_pic_four_words", "Quiz", "Word_clouds" };
    private List<string> m_miniGamesFilenames = new List<string>() { "HangedLevelsData", "Missing_words_data", "OnePicFourWordsDatas", "Quiz", "Word_clouds_data" };
    public Module_display_name[] m_modulesDisplayName =  //List which contains the module display names
    {new Module_display_name(_fileName : "Communication", _displayName : "Communication" ),
     new Module_display_name(_fileName : "Leadership", _displayName : "Leadership" ),
     new Module_display_name(_fileName : "Financement", _displayName : "Financement" ),
     new Module_display_name(_fileName : "Tech au Sénégal", _displayName : "Tech au Sénégal" ),
     new Module_display_name(_fileName : "Structuration", _displayName : "Structuration" ),
     new Module_display_name(_fileName : "LEAN CANVAS", _displayName : "Lean Canvas" )};
    public List<Difficulty_timer> m_difficulties_timer;
    private bool m_use_game_structure_algorithm = true;
    public List<string> m_notAvailableLevels = new List<string>();
    public PersistentInt m_currentMiniGameIndex =  new PersistentInt("currentMiniGameIndex", 0);

    //ENUM FOR LEVEL DIFFICULTY
    public enum GAME_DIFFICULTY
    {
        EASY = 0,
        MEDIUM = 1,
        HARD = 2
    }

    public enum BTN_COLOR
    {
        DEFAULT = 0,
        GOOD_ANWSER = 1,
        BAD_ANWSER = 2
    }
    //LIST OF STRING FOR GAME DIFFICULTY
    public List<Level_Difficulty> m_difficulties;


     void Awake() {
        // timer = time * 60f;
        if (instance != null)
         {
             Destroy(gameObject);
         }
         else
         {
             instance = this;
             DontDestroyOnLoad(gameObject);
         }
        UpdateModuleInformations();
        GenerateGameStructure();
        LoadStruct();
        //INIT LEVEL DIFFICULTY PLAYERPREFS
        InitLevelDifficulty();
    }

    public void LoadStruct()    
    {
        string modulePath = game_struct_path + Path.DirectorySeparatorChar + "Game_struct";
        if(!m_use_game_structure_algorithm) //if we are not using the game struct algorithm
        {
            TextAsset json = Resources.Load(modulePath) as TextAsset;

            if (json != null)
            {
                game_structure = JsonUtility.FromJson<Game_struct>(json.text);

            }
            else
            {
                //Debug.LogError("Cannot find file: " + modulePath);
            }
        }
        else
        {
            game_structure =  ProgressManager.Instance.GetGameStruct();
        }

    }

   public void GenerateGameStructure()
    {
        if(!PlayerPrefs.HasKey("game_structure"))
        {
            m_generated_game_structure = new Game_struct();
            //should load progress struct

            string modulePath = "Game_presets" + Path.DirectorySeparatorChar + "Progress_struct";
            ProgressManager.Instance.m_json = Resources.Load(modulePath) as TextAsset;
            Progress_struct progress_data = JsonUtility.FromJson<Progress_struct>(ProgressManager.Instance.m_json.ToString());
            m_generated_game_structure.modules = new Game_module_struct[progress_data.modules.Count];
            int numberOfMiniGames = 3;
            int numberOfLevels = 6;
            int selectedMiniGames = 0;
            int moduleIndex = 0;
            TextAsset json = new TextAsset();
            List<string> selectedMiniGamesNames = new List<string>();
            foreach (Progress_module_struct pm in progress_data.modules)
            {
                bool hangedLevelAvailable = false;
                int miniGamesIndex = 0; //iterate the list of mini-games
                Game_module_struct m = new Game_module_struct();    //set module values
                m.name = pm.name;
                m.levels = new Game_level_struct[GetNumberOfAvailableLevels(m.name, false)];
                //Debug.Log("Module " + m.name + " nbr " + GetNumberOfAvailableLevels(m.name));
                for (int i = 0; i < GetNumberOfAvailableLevels(m.name); i++)
                {
                    selectedMiniGames = 0;  //init the number of selected mini games
                    miniGamesIndex = 0; //init the mini game index
                    selectedMiniGamesNames.Clear(); //clear the added mini games name
                    Game_level_struct l = new Game_level_struct();  //set level values
                    l.duration = 90;
                    int level_index = i + 1;
                    //Debug.Log("Module " + m.name + " Level " + level_index + " " + GetNumberOfAvailableMiniGames(m.name, level_index));
                    if (GetNumberOfAvailableMiniGames(m.name, level_index, false) > 0)  //the number of available mini-games should be greater than 0
                    {
                        l.games = new Game_game_struct[GetNumberOfAvailableMiniGames(m.name, level_index)];
                        string path = "Games\\" + m_miniGamesNames[(int)MINI_GAMES_INDEX.QUIZ] + "\\" + m.name + "\\Level_" + level_index + "\\" + m_miniGamesFilenames[GetFileNameIndexBasedOnMiniGameName(m_miniGamesNames[miniGamesIndex])];
                        json = Resources.Load(path) as TextAsset;
                        if (json != null && selectedMiniGames < numberOfMiniGames) //we should verify if the quiz mini game exist and if it does then we should include it in the list of min-games to be played
                        {
                            Game_game_struct g = new Game_game_struct();    //set mini-games values
                            g.name = m_miniGamesNames[(int)MINI_GAMES_INDEX.QUIZ];
                            selectedMiniGamesNames.Add(g.name);
                            g.difficulty = 2;
                            g.turns = 2;
                            l.games[selectedMiniGames] = g;
                            selectedMiniGames++;

                        }

                        //we should now verify if the other mini-games directory exist
                        m_miniGamesNames = Shuffle(m_miniGamesNames);
                        while (selectedMiniGames < numberOfMiniGames && miniGamesIndex < m_miniGamesNames.Count)
                        {
                            level_index = i + 1;
                            if (m_miniGamesNames[miniGamesIndex].Equals("Hanged"))    //for the hanged mini game the path is different from the others
                            {
                                path = "Games\\" + m_miniGamesNames[miniGamesIndex] + "\\" + m.name + "\\Levels\\" + m_miniGamesFilenames[GetFileNameIndexBasedOnMiniGameName(m_miniGamesNames[miniGamesIndex])];
                                hangedLevelAvailable =  IsLevelAvailableInHanged(path, level_index);
                            }
                            else
                            {
                                path = "Games\\" + m_miniGamesNames[miniGamesIndex] + "\\" + m.name + "\\Level_" + level_index + "\\" + m_miniGamesFilenames[GetFileNameIndexBasedOnMiniGameName(m_miniGamesNames[miniGamesIndex])]; ;
                            }
                            json = Resources.Load(path) as TextAsset;

                            //Debug.Log(miniGamesIndex);
                            if (selectedMiniGamesNames.Where(s => s.ToLower().Equals(m_miniGamesNames[miniGamesIndex].ToLower())).FirstOrDefault() == null && json != null && selectedMiniGames < 3)   //if the mini-games hasn't been chosen and the path exist then we can choose the mini-games
                            {
                                Game_game_struct g = new Game_game_struct();    //set mini-games values
                                if(m_miniGamesNames[miniGamesIndex].Equals("Hanged") && hangedLevelAvailable)   //if it is the hanged mini-game, we should make sure that the level exist in the JSON file
                                {
                                    g.name = m_miniGamesNames[miniGamesIndex];
                                    selectedMiniGamesNames.Add(g.name);
                                    g.difficulty = 2;
                                    g.turns = 1;
                                    l.games[selectedMiniGames] = g;
                                    selectedMiniGames++;
                                }
                                if(!m_miniGamesNames[miniGamesIndex].Equals("Hanged"))
                                {
                                    g.name = m_miniGamesNames[miniGamesIndex];
                                    selectedMiniGamesNames.Add(g.name);
                                    g.difficulty = 2;
                                    g.turns = 2;
                                    l.games[selectedMiniGames] = g;
                                    selectedMiniGames++;
                                }
                            }
                            miniGamesIndex++;
                        }
                        int lastMiniGame = l.games.Count() - 1;

                        if (l.games[lastMiniGame] == null)  //if there are only two mini-games available then we should re-add the first one
                        {
                            l.games[lastMiniGame] = l.games[0];
                        }
                        if(l.games[lastMiniGame - 1] == null)  //if the second mini-game is equal to null
                        {
                            l.games[lastMiniGame - 1] = l.games[0];
                        }
                        m.levels[i] = l;

                    }
                }
                m_generated_game_structure.modules[moduleIndex] = m;
                moduleIndex++;
            }
            ProgressManager.Instance.SaveGameStructure(m_generated_game_structure);
            string jsonD = JsonUtility.ToJson(m_generated_game_structure);
        }
    }

    public void UpdateModuleInformations()  //update the module's informations
    {
        if(ProgressManager.Instance.GetProgressStruct() != null)
        {
            m_generated_game_structure = new Game_struct();
            //Loaded JSON file datas
            string modulePath = "Game_presets" + Path.DirectorySeparatorChar + "Progress_struct";
            TextAsset _json = Resources.Load(modulePath) as TextAsset;
            Progress_struct loaded_data = JsonUtility.FromJson<Progress_struct>(_json.ToString());
            //Loaded playerprefs datas
            Progress_struct progress_data = ProgressManager.Instance.GetProgressStruct();
            if(IsProgressStructUpdated(progress_data, loaded_data)) //verified if there is a difference between the old progress struct and the new progress struct
            {
                //compared the two datas
                UpdateLevelDifficulty(progress_data, loaded_data);//update the levels difficulty
                for (int i = 0; i < loaded_data.modules.Count; i++)
                {
                    if (!loaded_data.modules[i].name.Equals(progress_data.modules[i].name)) //Compared the name of the module
                    {
                        progress_data.modules[i].name = loaded_data.modules[i].name;
                    }
                }
                ProgressManager.Instance.SaveGlobalProgress(progress_data);
            }
        }
    }

    public bool IsProgressStructUpdated(Progress_struct _old_progress_struct, Progress_struct _new_progress_struct)
    {
        for (int i = 0; i < _new_progress_struct.modules.Count; i++)
        {
            if (!_new_progress_struct.modules[i].name.Equals(_old_progress_struct.modules[i].name)) //Compared the name of the module
            {
                return true;
            }
        }
        return false;
    }

    public int GetFileNameIndexBasedOnMiniGameName(string _miniGameName)
    {
        if(MINI_GAMES_INDEX.HANGED.ToString().ToLower().Equals(_miniGameName.ToLower()))
        {
            return (int)MINI_GAMES_INDEX.HANGED;
        }
        else if (MINI_GAMES_INDEX.MISSING_WORDS.ToString().ToLower().Equals(_miniGameName.ToLower()))
        {
            return (int)MINI_GAMES_INDEX.MISSING_WORDS;
        }
        else if (MINI_GAMES_INDEX.WORD_CLOUDS.ToString().ToLower().Equals(_miniGameName.ToLower()))
        {
            return (int)MINI_GAMES_INDEX.WORD_CLOUDS;
        }
        else if (MINI_GAMES_INDEX.QUIZ.ToString().ToLower().Equals(_miniGameName.ToLower()))
        {
            return (int)MINI_GAMES_INDEX.QUIZ;
        }
        else if (MINI_GAMES_INDEX.ONE_PIC_FOUR_WORDS.ToString().ToLower().Equals(_miniGameName.ToLower()))
        {
            return (int)MINI_GAMES_INDEX.ONE_PIC_FOUR_WORDS;
        }
        return 0;

    }

    public int GetNumberOfAvailableMiniGames(string _moduleName, int _levelIndex, bool _displayWarnigMsg = false)
    {
        int numberOfminiGames = 0;
        string path = "";
        TextAsset json = new TextAsset();
        for (int i = 0; i < m_miniGamesNames.Count; i++)
        {
            if (m_miniGamesNames[i].Equals("Hanged"))    //for the hanged mini game the path is different from the others
            {
                path = "Games\\" + m_miniGamesNames[i] + "\\" + _moduleName + "\\Levels\\" + m_miniGamesFilenames[GetFileNameIndexBasedOnMiniGameName(m_miniGamesNames[i])];
            }
            else
            {
                path = "Games\\" + m_miniGamesNames[i] + "\\" + _moduleName + "\\Level_" + _levelIndex + "\\" + m_miniGamesFilenames[GetFileNameIndexBasedOnMiniGameName(m_miniGamesNames[i])]; ;
            }
            json = Resources.Load(path) as TextAsset;

            if (json != null)
            {
                numberOfminiGames++;
            }
        }

        if(numberOfminiGames >= 1)
        {
            return 3;
        }
        else
        {
            if(_displayWarnigMsg)
            {
                Debug.LogWarning("The " + _moduleName + " module level " + _levelIndex + " has only " + numberOfminiGames + " mini game(s)");
            }
            return numberOfminiGames;
        }
    }

    public int GetNumberOfAvailableLevels(string _moduleName, bool _displayWarningMsg = false)
    {
        m_notAvailableLevels.Clear();
        int numberOfLevels = 0;
        string path = "";
        TextAsset json = new TextAsset();

        bool hasFoundLevel = false;
        for (int j = 1; j <= 6; j++)
            {
                hasFoundLevel = false;
                for (int i = 0; i < m_miniGamesNames.Count; i++)
                {
                    if(m_miniGamesNames[i].Equals("Hanged"))
                    {
                        path = "Games\\" + m_miniGamesNames[i] + "\\" + _moduleName + "\\Levels\\" + m_miniGamesFilenames[GetFileNameIndexBasedOnMiniGameName(m_miniGamesNames[i])];
                        if(IsLevelAvailableInHanged(path, j) && !hasFoundLevel)
                        {
                            numberOfLevels++;
                            hasFoundLevel = true;
                        }
                        else
                        {

                            if (i == m_miniGamesNames.Count - 1 && !hasFoundLevel)
                            {
                                m_notAvailableLevels.Add(GetNoAvailableLevelKey(_moduleName, j));
                            }
                        }
                    }
                    else
                    {
                        path = "Games\\" + m_miniGamesNames[i] + "\\" + _moduleName + "\\Level_" + j + "\\" + m_miniGamesFilenames[GetFileNameIndexBasedOnMiniGameName(m_miniGamesNames[i])];
                        json = Resources.Load(path) as TextAsset;
                        if (json != null && !hasFoundLevel)
                        {
                            numberOfLevels++;
                            hasFoundLevel = true;
                            break;
                        }
                        else
                        {
                            if(i == m_miniGamesNames.Count - 1 && !hasFoundLevel)
                            {
                                m_notAvailableLevels.Add(GetNoAvailableLevelKey(_moduleName, j));
                            }
                        }
                    }
                }
            }


        if(numberOfLevels >= 6)
        {
            return 6;
        }
        else
        {
            if(_displayWarningMsg)
            {
                Debug.LogWarning("The " + _moduleName + " module has only " + numberOfLevels + " level(s)");
            }
            return numberOfLevels;
        }
    }

    public bool IsLevelAvailableInHanged(string path, int levelIndex)
    {
        TextAsset json = new TextAsset();
        json = Resources.Load(path) as TextAsset;
        if(json != null)    //we should make sure that the JSON file exist before treating the datas
        {
            Hanged_struct m_hangedLevelsData;
            string myLoadedGameLevel = json.text;
            m_hangedLevelsData = JsonUtility.FromJson<Hanged_struct>(myLoadedGameLevel);
            Hanged_difficulty hangedData;
            hangedData = GetHangedDifficulty(levelIndex, m_hangedLevelsData.m_difficulties);
            return IsHangedDataNotEmpty(levelIndex, hangedData);
        }
        else
        {
            return false;
        }
    }

    public Hanged_difficulty GetHangedDifficulty(int index, List<Hanged_difficulty> _difficulties)
    {

        switch (index)
        {
            case 1:
                return _difficulties.Where(d => d.levelOne != null && d.levelOne.Count > 0).FirstOrDefault();
                break;
            case 2:
                return _difficulties.Where(d => d.levelTwo != null && d.levelTwo.Count > 0).FirstOrDefault();
                break;
            case 3:
                return _difficulties.Where(d => d.levelThree != null && d.levelThree.Count > 0).FirstOrDefault();
                break;
            case 4:
                return _difficulties.Where(d => d.levelFour != null && d.levelFour.Count > 0).FirstOrDefault();
                break;
            case 5:
                return _difficulties.Where(d => d.levelFive != null && d.levelFive.Count > 0).FirstOrDefault();
                break;
            case 6:
                return _difficulties.Where(d => d.levelSix != null && d.levelSix.Count > 0).FirstOrDefault();
                break;
            default:
                return null;
                break;
        }
    }


    public bool IsHangedDataNotEmpty(int index, Hanged_difficulty hangedData)
    {
        if(hangedData != null)
        {
            switch (index)
            {
                case 1:
                    return hangedData.levelOne.Count > 0 ? true : false;
                    break;
                case 2:
                    return hangedData.levelTwo.Count > 0 ? true : false;
                    break;
                case 3:
                    return hangedData.levelThree.Count > 0 ? true : false;
                    break;
                case 4:
                    return hangedData.levelFour.Count > 0 ? true : false;
                    break;
                case 5:
                    return hangedData.levelFive.Count > 0 ? true : false;
                    break;
                case 6:
                    return hangedData.levelSix.Count > 0 ? true : false;
                    break;
                default:
                    return false;
                    break;
            }
        }
        else
        {
            return false;
        }
    }
    public string GetNoAvailableLevelKey(string _moduleName, int _levelIndex)
    {
        return _moduleName + "\\Level_" + _levelIndex;
    }

    public void LoadModule(string module_name, int level_index, int game_index = 0)
    {
        //IF WE JUST STARTED THE GAME THEN WE SHOULD RESET THE SCORE TO 0
        if (game_index == 0)
        {
            InitModuleLevelMinigameScore(module_name, level_index);
        }

       

        string game_name = GetGameName(module_name, level_index, game_index);
        
        string game_difficulty = GetGameDifficulty(module_name, level_index, game_index);
        
        int game_turns = GetGameTurns(module_name, level_index, game_index);
        
        string game_data = GetGameData(module_name, level_index, game_index);

        //@Managing duration initialization
        int duration = GetTimerByDifficulty(game_difficulty); //get duration by difficulty

        if (game_index == 0) //@First game of the level
        {
            time = duration;
            timer = time;

        }
        SaveCurrentModule(module_name, level_index, game_name, game_difficulty, game_turns, game_data);

        GoToGame();
    }

    public int GetTimerByDifficulty(string _game_difficulty)
    {
       return  StageManager.instance.m_difficulties_timer.Where(d => d.difficulty.ToLower().Equals(_game_difficulty.ToLower())).FirstOrDefault().timer;
    }

    public void LoadModuleScreen()
    {
        if(StoryTellerCtrlr.Instance != null)
            StoryTellerCtrlr.Instance.m_storyIntroduced = false;

        SceneManager.LoadScene("Module_screen");
    }

    public void ReloadModule(string module_name, int level_index)
    {
        //@Now call LoadModule
        LoadModule(module_name , level_index);

    }

    public int GetModuleIndexByName(string module_name)
    {
        int module_index = game_structure.modules.ToList().FindIndex(a => a.name == module_name );
        return module_index;
    }

    //Takes current game name and return next game index (or -1 if not existing)
    public int GetNextGameIndex(string module_name, int level_index, string game_name)
    {


        ////Index key in  json file starts from 1
        int next_game_index;
        int current_game_index = -1;
        int module_index = GetModuleIndexByName(module_name);
        //int ind = 0;
        //foreach(Game_game_struct g in game_structure.modules[module_index].levels[level_index].games)
        //{
        //    if(g.name == game_name)
        //    {
        //        current_game_index = ind;
        //    }
        //    ind++;
        //}
        ////int current_game_index = game_structure.modules[module_index]
        ////    .levels[level_index].games.ToList().FindIndex(g => g.name == game_name);


        //next_game_index = current_game_index + 1;
        //// @IF OVERFLOW
        //if(next_game_index >= game_structure.modules[module_index]
        //    .levels[level_index].games.Length )
        //{
        //    next_game_index = -1;
        //}

        m_currentMiniGameIndex.Set(m_currentMiniGameIndex.Get() + 1);


        if (m_currentMiniGameIndex.Get() >= game_structure.modules[module_index]
            .levels[level_index].games.Length)
        {
            m_currentMiniGameIndex.Set(-1);
        }
        return m_currentMiniGameIndex.Get();
    }

    

    public void GoToGame()
    {
        SceneManager.LoadScene("Splash_screen");
    }

    public void SaveCurrentModule(string module_name, int level_index, string game_name,  string game_difficulty , int game_turns , string game_data)
    {
        SetString("current_module_name", module_name);
        SetInt("current_level_index", level_index);
        SetString("current_game_name", game_name);
        SetString("current_game_difficulty", game_difficulty);
        SetInt("current_game_turns", game_turns);
        SetString("current_game_data", game_data);
     
    }

    public string GetGameData(string module_name, int level_index, int game_index)
    {
        // Index key in json file starts from 1
        int module_index = GetModuleIndexByName(module_name);

        if (game_structure.modules[module_index].levels[level_index]
                .games[game_index].data != null )
        {
            return game_structure.modules[module_index].levels[level_index].games[game_index].data;
        }

        return "";
    }

    public string GetGameName(string module_name, int level_index,int game_index)
    {
        
        //Index key in  json file starts from 1
        int module_index = GetModuleIndexByName(module_name);

        return game_structure.modules[module_index].levels[level_index]
                .games[game_index].name;

    }


    /// <summary>
    /// GET THE CURRENT GAME DIFFICULTY 
    /// </summary>
    /// <param name="module_name"></param>
    /// <param name="level_index"></param>
    /// <param name="game_index"></param>
    /// <returns></returns>
    public string GetGameDifficulty(string module_name, int level_index,int game_index)
    {
        int last_index = 0;
        GAME_DIFFICULTY current_difficulty = GetLevelDifficulty(module_name, level_index);
        if (ProgressManager.Instance.GetLevelNbrOfBronzeMedal(module_name, level_index + 1) == 0 && current_difficulty.ToString().ToLower().Equals(GAME_DIFFICULTY.EASY.ToString().ToLower()))
        {
            current_difficulty = GAME_DIFFICULTY.EASY;
        }
        else if (ProgressManager.Instance.GetLevelNbrOfBronzeMedal(module_name, level_index + 1) > 0 && current_difficulty.ToString().ToLower().Equals(GAME_DIFFICULTY.EASY.ToString().ToLower()))
        {
            current_difficulty = GAME_DIFFICULTY.MEDIUM;
        }
        else if (ProgressManager.Instance.GetLevelNbrOfSilverMedal(module_name, level_index + 1) == 0 && current_difficulty.ToString().ToLower().Equals(GAME_DIFFICULTY.MEDIUM.ToString().ToLower()))
        {
            current_difficulty = GAME_DIFFICULTY.MEDIUM;
        }
        else
        {
            current_difficulty = GAME_DIFFICULTY.HARD;
        }
        return current_difficulty.ToString();   //returned the current game difficulty
    }

    public string GetGameCurrentDifficulty()
    {
        return instance.GetString("current_game_difficulty");
    }

    public int GetCurrentLevelIndex()
    {
        return instance.GetInt("current_level_index");

    }

    public int GetGameTurns(string module_name, int level_index,int game_index)
    {
        
        //Index key in  json file starts from 1
        int module_index = GetModuleIndexByName(module_name);

        return game_structure.modules[module_index].levels[level_index]
                .games[game_index].turns;

    }

    public void InitModuleLevelMinigameScore(string module_name, int level_index)
    {
        //Debug.Log(level_index);
        int level_ind = 0;
        foreach(Game_module_struct m in game_structure.modules)
        {
            //Debug.Log(m.name);
            //Debug.Log(module_name);

            if (m.name.Equals(module_name))
            {
                foreach(Game_level_struct l in m.levels)
                {
                    if (level_ind == level_index)
                    {
                        foreach(Game_game_struct g in l.games)
                        {
                            SetGameScore(m.name, level_ind, g.name, 0);
                        }
                    }
                    level_ind++;
                }
            }
        }
    }

    // Tells if the given level index is the last one for the current module
    public bool IsLastLevel(int level_index)
     {
       string current_module_name = GetString("current_module_name");
       int module_index = GetModuleIndexByName(current_module_name);
       int module_levels = game_structure.modules[module_index].levels.Count();
       return level_index == (module_levels) ? true : false;
       
     }
   
    public int GetLevelDuration(string module_name, int level_index)
    {
        
        int module_index = GetModuleIndexByName(module_name);

        return game_structure.modules[module_index].levels[level_index].duration;

    }



    public string GetSpecificGameDataPath(Game_level_struct level, string game_name)
    {
        return "";
    }

    //@This is called by a game when it finished its stage in order to set its score
    public void SetGameScore(string module_name , int level_index, string game_name ,float score)
    {

        //@PlayerPrefs
        SetFloat(module_name+"_Level_"+level_index+"_"+game_name+"_score" , score);
 

    }

    //@get the game score of one minigame
    public float GetGameScore(string module_name, int level_index, string game_name)
    {
        //@playerPrefs
       return  GetFloat(module_name + "_Level_" + level_index + "_" + game_name + "_score");
    }

    public float GetGlobalScore(string module_name, int level_index)
    {
        int module_index = GetModuleIndexByName(module_name);
        float global_score = 0f;
        foreach(Game_game_struct g in  game_structure.modules[module_index].levels[level_index].games)
        {
            global_score += GetGameScore(module_name, level_index, g.name);
        }
        global_score = global_score / game_structure.modules[module_index].levels[level_index].games.Count();
        return global_score;
    }

    //@Saves data (replacing content with loaded game_structure) into JSON file before leaving the app 
    public void SaveChanges()
    {


    }

    public void LoadStageScreen(int screen_index)
    {
        if(TimerScript.Instance != null)
            TimerScript.Instance.m_timerRunning = false;
            
        SetInt("stage_screen_index", screen_index) ;


        string module_name = StageManager.instance.GetString("current_module_name");
        int level_index = StageManager.instance.GetInt("current_level_index");
        float global_score = Mathf.Round(StageManager.instance.GetGlobalScore(module_name, level_index));

        if (StoryTellerCtrlr.Instance != null && global_score  >= (int)ProgressManager.Medals.BRONZE) //we should check if the player has at least earned 75%
        {
            if(screen_index == 1) // Not Time out
            {
                StoryTellerCtrlr.Instance.LaunchOutro(module_name, instance.GetInt("current_level_index"), global_score);
            }
        }

        // SceneManager.LoadScene("Stage_screen");
        SceneManager.LoadScene("Endgame_screen");

    }

 

    public void LoadNextGame(string current_module_name, int current_level_index, string current_game_name, float current_game_score)
    {
        SetGameScore(current_module_name, current_level_index, current_game_name, current_game_score);


        int module_index = GetModuleIndexByName(current_module_name);
 

        m_currentMiniGameIndex.Set(GetNextGameIndex(current_module_name,current_level_index, current_game_name));
        
        // //Debug.Log(next_game_index);
        
      
        if (m_currentMiniGameIndex.Get() != -1)
        {

          //@We call LoadModule and let it do its work :)
           LoadModule(current_module_name , current_level_index, m_currentMiniGameIndex.Get());
            
        }
        else //@No next game : This level has been "finished" : All games were played
        {
            /*@Get total level games scores to know whether we unlock the next level 
            in the current module or not : If player got enough points to pass to next level*/
            
            LoadStageScreen(1);

            bool can_unlock_next_level = false;

            if(can_unlock_next_level)
            {

            }
            else
            {


            }

            //@ Code goes here

        }
        
    }

    //INIT LEVEL DIFFICULTY
    public void InitLevelDifficulty()
    {
            int level_index = 0;
            string player_pref_key = "";
            player_pref_key = game_structure.modules[0].name.ToUpper() + "_LEVEL_1"  + "_DIFFICULTY";
            if(!PlayerPrefs.HasKey(player_pref_key))
            {
                foreach (Game_module_struct m in game_structure.modules)
                {
                        level_index = 1;
                        //INIT ALL MODULE'S LEVEL'S DIFFICULTY TO EASY
                        foreach (Game_level_struct l in m.levels)
                        {
                                player_pref_key = m.name.ToUpper() + "_LEVEL_" + level_index.ToString() + "_DIFFICULTY";
                                SetString(player_pref_key, "EASY");
                                level_index += 1;
                        }
                }
            }
    }

    public void UpdateLevelDifficulty(Progress_struct _old_progress_struct, Progress_struct _new_progress_struct)
    {
        int level_index = 1;
        string _old_player_pref_key = "";
        string _new_player_pref_key = "";

        for (int i = 0; i < _old_progress_struct.modules.Count; i++)
        {
            level_index = 1;
            //INIT ALL MODULE'S LEVEL'S DIFFICULTY TO EASY
            for (int j = 0; j < _old_progress_struct.modules[i].levels.Count; j++)
            {
                _old_player_pref_key = _old_progress_struct.modules[i].name.ToUpper() + "_LEVEL_" + level_index.ToString() + "_DIFFICULTY";
                _new_player_pref_key = _new_progress_struct.modules[i].name.ToUpper() + "_LEVEL_" + level_index.ToString() + "_DIFFICULTY";
                SetString(_new_player_pref_key, GetString(_old_player_pref_key));
                PlayerPrefs.DeleteKey(_old_player_pref_key);
                level_index += 1;
            }
        }

    }

    //
    public void DeleteLevelDifficultyKeys()
    {

        int level_index = 0;
        string player_pref_key = "";

        foreach (Game_module_struct m in game_structure.modules)
        {


            level_index = 1;
            //INIT ALL MODULE'S LEVEL'S DIFFICULTY TO EASY
            foreach (Game_level_struct l in m.levels)
            {
                player_pref_key = m.name.ToUpper() + "_LEVEL_" + level_index.ToString() + "_DIFFICULTY";
                SetString(player_pref_key, m_difficulties[(int)GAME_DIFFICULTY.EASY].difficulty);
                level_index += 1;

            }
        }
    }

    /// <summary>
    /// GET THE LEVEL DIFFICULTY BY USING THE MODULE NAME AND THE LEVEL INDEX
    /// </summary>
    /// <param name="m"></param>
    /// <param name="level_index"></param>
    /// <returns></returns>
    public GAME_DIFFICULTY GetLevelDifficulty(string module_name, int level_index)
    {
        level_index += 1;
        string player_pref_key = module_name.ToUpper() + "_LEVEL_" + level_index.ToString() + "_DIFFICULTY";
        string difficulty = m_difficulties.Where(d => d.difficulty.ToLower().Equals(GetString(player_pref_key).ToLower())).FirstOrDefault().difficulty;

        if (difficulty.ToLower().Equals(GAME_DIFFICULTY.EASY.ToString().ToLower()))
        {
                return GAME_DIFFICULTY.EASY;
        }
        else if(difficulty.ToLower().Equals(GAME_DIFFICULTY.MEDIUM.ToString().ToLower()))
        {
                return GAME_DIFFICULTY.MEDIUM;
        }
        else
        {
            return GAME_DIFFICULTY.HARD;
        }

    }

    //GET THE CURRENT LEVEL DIFFICULTY 
    public GAME_DIFFICULTY GetCurrentLevelDifficulty()
    {

        string module_name = GetString("current_module_name");
        int level_index = GetInt("current_level_index");
        level_index += 1;
        string player_pref_difficulty_key = module_name.ToUpper() + "_LEVEL_" + level_index.ToString() + "_DIFFICULTY";
        string difficulty = m_difficulties.Where(d => d.difficulty.ToLower().Equals(GetString(player_pref_difficulty_key).ToLower())).FirstOrDefault().difficulty;
        if (difficulty.ToLower().Equals(GAME_DIFFICULTY.EASY.ToString().ToLower()))
        {
            return GAME_DIFFICULTY.EASY;
        }
        else if (difficulty.ToLower().Equals(GAME_DIFFICULTY.MEDIUM.ToString().ToLower()))
        {
            return GAME_DIFFICULTY.MEDIUM;
        }
        else
        {
            return GAME_DIFFICULTY.HARD;
        }
    }

    //UNLOCK NEXT LEVEL DIFFICULTY 
    public void UnlockNextLevelDifficulty()
    {
        Debug.Log("UNLOCK NEW MEDAL");
        string module_name = GetString("current_module_name");
        int level_index = GetInt("current_level_index");
        level_index += 1;
        
        string player_pref_key = module_name.ToUpper() + "_LEVEL_" + level_index.ToString() + "_DIFFICULTY";
        string difficulty = m_difficulties.Where(d => d.difficulty.ToLower().Equals(GetString(player_pref_key).ToLower())).FirstOrDefault().difficulty;
       
        if (difficulty.ToLower().Equals(GAME_DIFFICULTY.EASY.ToString().ToLower()))
        {
            SetString(player_pref_key, m_difficulties[(int)GAME_DIFFICULTY.MEDIUM].difficulty);
        }
        else if (difficulty.ToLower().Equals(GAME_DIFFICULTY.MEDIUM.ToString().ToLower()))
        {
            SetString(player_pref_key, m_difficulties[(int)GAME_DIFFICULTY.HARD].difficulty);

        }
    }


    // ###### PLAYERPREFS SHORTCUTS ####### 

    public int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }
    public string GetString(string key)
    {
        return PlayerPrefs.GetString(key);
    }
    public float GetFloat(string key)
    {
        return PlayerPrefs.GetFloat(key);
    }
    public void SetString(string key, string value)
    {
         PlayerPrefs.SetString(key , value);
    }
    public void SetInt(string key, int value)
    {
         PlayerPrefs.SetInt(key , value);
    }

    public void SetFloat(string key, float value)
    {
         PlayerPrefs.SetFloat(key , value);
    }





    // UTILS
    public List<T> Shuffle<T>(List<T> _list)
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


  
 
}


[System.Serializable]
public class BtnColor
{
    public string label;
    public Color primaryColor;
    public Color outlineColor;
    public Color textColor;
}

[System.Serializable]
public class Difficulty_timer
{
    public string difficulty;
    public int timer;
}
[System.Serializable]
public class Level_Difficulty
{
    public string difficulty;
    public List<int> difficulty_index;
}

[System.Serializable]
public class Game_struct{

    public Game_module_struct[] modules;
}

[System.Serializable]
public class Game_game_struct{
    
    public string name;
    public int difficulty;
    public int turns;
    public string data;

}

[System.Serializable]
public class Game_level_struct{
    public int duration;
    public Game_game_struct[] games;
}

[System.Serializable]
public class Game_module_struct{
    public string name;
    public Game_level_struct[] levels;
}


[System.Serializable]
public class Module_display_name
{
    public string fileName;
    public string dislpayName;

    public Module_display_name(string _fileName, string _displayName)
    {
        this.fileName = _fileName;
        this.dislpayName = _displayName;
    }
}




