using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerangaDatasParser
{
    public enum GAME_TYPE
    {
        QUIZ = 0,
        ONE_PIC_FOUR_WORDS = 1,
        WORD_CLOUDS = 2,
        PENDU = 3,
        MISSING_WORDS = 4
    }


    public enum DIFFICULTY
    {
        EASY = 0,
        MEDIUM = 1,
        HARD = 2
    }

    class Program
    {
        public static List<string> m_difficulties = new List<string>() { "Facile", "Moyen", "Difficile" };

        public static List<string> m_moduleName = new List<string>()
        {
                "Communication",
                "Financement",
                "Leadership",
                "Tech au Sénégal",
                "Structuration",
                "LEAN CANVAS"
        };

        public static List<string> m_gameType = new List<string>()
        {
                "Quizz",
                "1 image 4 mots",
                "Nuage de mots",
                "Pendu",
                "Phrase à trous"
        };


        public static List<string> m_gamePath = new List<string>()
        {
                "..\\..\\Assets\\Resources\\Games\\Quiz\\",
                "..\\..\\Assets\\Resources\\Games\\One_pic_four_words\\",
                "..\\..\\Assets\\Resources\\Games\\Word_clouds\\",
                "..\\..\\Assets\\Resources\\Games\\Hanged",
                "..\\..\\Assets\\Resources\\Games\\Missing_words"
        };

        public static string GetDataDifficulty(string _difficulty)
        {
            if (_difficulty.ToLower().Equals(m_difficulties[(int)DIFFICULTY.EASY].ToLower()))
            {
                return DIFFICULTY.EASY.ToString();
            }
            else if (_difficulty.ToLower().Equals(m_difficulties[(int)DIFFICULTY.MEDIUM].ToLower()))
            {
                return DIFFICULTY.MEDIUM.ToString();
            }
            else
            {
                return DIFFICULTY.HARD.ToString();
            }
        }

        static void Main(string[] args)
        {
            string path = "";
            Console.WriteLine("Saisir le nom du fichier avec l'extenstion");
            path = Console.ReadLine();
            List<ModuleDatas> datas = ParseCSV(path);
            GenerateErrorLogFile(datas);
            Console.WriteLine("Tap enter to save JSON files");
            datas = FilterContent(datas);
            //SAVE WORLD CLOUDS MINI GAME DATAS
            CreateWordCloudsJSONFile(datas);
            //SAVED QUIZ GAME DATAS
            CreateQuizJSONFile(datas);
            //SAVED ONE PIC FOUR WORDS DATAS
            CreateOnePicFourWordsJSONFile(datas);
            //SAVED HANGED GAME DATAS
            CreateHangedJSONFile(datas);
            //SAVED MISSING WORDS DATA
            CreateMissingWordsJSONFile(datas);
            //SAVED STORYTELLING DATA
            CreateStoryTellingJSONFile(datas);
            Console.ReadLine();
        }


        public static List<ModuleDatas> FilterContent(List<ModuleDatas> datas)
        {
            int maxLetterForHangedMiniGame = 15;
            int maxAnwsersForQuiz = 4;
            List<ModuleDatas> uncoruptedDatas = new List<ModuleDatas>();
            bool dataIsCorupted = false;
            foreach (ModuleDatas data in datas)
            {
                dataIsCorupted = false;
                if (data.gameType != m_gameType[(int)GAME_TYPE.PENDU] && data.goodAnwsers.Split(',').Length > data.anwsers.Count)//if the number of good anwsers is greater than the numbers of anwser 
                {
                    dataIsCorupted = true;
                }

                if (data.goodAnwsers.Count() == 0)
                {
                    dataIsCorupted = true;
                }

                if (data.gameType == m_gameType[(int)GAME_TYPE.PENDU] && data.question.Length > maxLetterForHangedMiniGame)
                {
                    dataIsCorupted = true;
                }

                if (data.gameType != m_gameType[(int)GAME_TYPE.PENDU] && data.anwsers.Count == 0)    //if the mini-game is different from hanged mini-game and the length is equal to zero 
                {
                    dataIsCorupted = true;
                }


                if (data.gameType == m_gameType[(int)GAME_TYPE.MISSING_WORDS] && !IsAnwserInsideQuestion(data.question, data.anwsers))
                {
                    dataIsCorupted = true;
                }
                
                if (data.entryMessage.Equals("") || data.exitMessage.Equals("") || data.level == 0 || data.moduleName.Equals("") || data.level == 0)
                {
                    dataIsCorupted = true;
                }

                if (data.gameType == m_gameType[(int)GAME_TYPE.QUIZ] && data.anwsers.Count > maxAnwsersForQuiz)  //filter the quiz games which contains more than 4 anwsers
                {
                    List<string> _anwsers = new List<string>();
                    string ind = data.goodAnwsers.Split(',').ToList()[0];
                    _anwsers.Add(data.anwsers[int.Parse(ind) - 1]); //at first we add the correct anwser
                    int counter = 0;
                    foreach (string s in data.anwsers)
                    {
                        if (!_anwsers.Contains(s) && counter < 3)   //we add the other anwsers that we didn't add
                        {                                          // < 3 because we already add the good anwser
                            _anwsers.Add(s);
                            counter++;
                        }
                    }
                    data.anwsers = _anwsers;
                    data.goodAnwsers = "1";
                }

                if (!dataIsCorupted)
                {
                    uncoruptedDatas.Add(data);
                }
            }
            return uncoruptedDatas;
        }


        public static void GenerateErrorLogFile(List<ModuleDatas> datas)
        {
            int maxLetterForHangedMiniGame = 15;
            int maxAnwsersForQuiz = 4;
            int numberOfLevels = 6;
            List<int> existingLevels = new List<int>();
            List<Error> errors = new List<Error>();
            int lines = 4;
            foreach (ModuleDatas data in datas)
            {
                if (data.gameType != m_gameType[(int)GAME_TYPE.PENDU] && data.goodAnwsers.Split(',').Length > data.anwsers.Count)//if the number of good anwsers is greater than the numbers of anwser 
                {
                    Error error = new Error();
                    error.module = data.moduleName;
                    error.level = data.level.ToString();
                    error.miniGame = data.gameType;
                    error.line = lines.ToString();
                    error.description = "Le nombre de réponses est infèrieur au nombre de bonnes réponses";
                    errors.Add(error);
                }

                if (data.goodAnwsers.Count() == 0)
                {
                    Error error = new Error();
                    error.module = data.moduleName;
                    error.level = data.level.ToString();
                    error.miniGame = data.gameType;
                    error.line = lines.ToString();
                    error.description = "La bonne réponse à la question n'est pas indiquée";
                    errors.Add(error);
                }

                if (data.gameType == m_gameType[(int)GAME_TYPE.PENDU] && data.question.Length > maxLetterForHangedMiniGame)
                {
                    Error error = new Error();
                    error.module = data.moduleName;
                    error.level = data.level.ToString();
                    error.miniGame = data.gameType;
                    error.line = lines.ToString();
                    error.description = "Le nombre de lettres doit être inférieure ou égale à 15";
                    errors.Add(error);
                }


                if (data.gameType != m_gameType[(int)GAME_TYPE.PENDU] && data.anwsers.Count == 0)    //if the mini-game is different from hanged mini-game and the length is equal to zero 
                {
                    Error error = new Error();
                    error.module = data.moduleName;
                    error.level = data.level.ToString();
                    error.miniGame = data.gameType;
                    error.line = lines.ToString();
                    error.description = "Il n'y a pas de reponses pour cette question.";
                    errors.Add(error);
                }


                if (data.gameType == m_gameType[(int)GAME_TYPE.MISSING_WORDS] && !IsAnwserInsideQuestion(data.question, data.anwsers))    //if the mini-game is different from hanged mini-game and the length is equal to zero 
                {
                    Error error = new Error();
                    error.module = data.moduleName;
                    error.level = data.level.ToString();
                    error.miniGame = data.gameType;
                    error.line = lines.ToString();
                    error.description = "La réponse doit figurer dans la phrase.";
                    errors.Add(error);
                }

                if (data.moduleName.Equals(""))
                {
                    Error error = new Error();
                    error.module = data.moduleName;
                    error.level = data.level.ToString();
                    error.miniGame = data.gameType;
                    error.line = lines.ToString();
                    error.description = "Le nom du module n'est pas renseigné.";
                    errors.Add(error);
                }

                if (data.level == 0)
                {
                    Error error = new Error();
                    error.module = data.moduleName;
                    error.level = data.level.ToString();
                    error.miniGame = data.gameType;
                    error.line = lines.ToString();
                    error.description = "Le niveau de cette question n'est pas renseigné.";
                    errors.Add(error);
                }

                if (data.entryMessage.Equals(""))
                {
                    Error error = new Error();
                    error.module = data.moduleName;
                    error.level = data.level.ToString();
                    error.miniGame = data.gameType;
                    error.line = lines.ToString();
                    error.description = "Le message d'entrée n'est pas renseigné.";
                    errors.Add(error);
                }

                if (data.exitMessage.Equals(""))
                {
                    Error error = new Error();
                    error.module = data.moduleName;
                    error.level = data.level.ToString();
                    error.miniGame = data.gameType;
                    error.line = lines.ToString();
                    error.description = "Le message de sortie n'est pas renseigné.";
                    errors.Add(error);
                }


                if (data.gameType == m_gameType[(int)GAME_TYPE.QUIZ] && data.anwsers.Count > maxAnwsersForQuiz)  ///
                {
                    Error error = new Error();
                    error.module = data.moduleName;
                    error.level = data.level.ToString();
                    error.miniGame = data.gameType;
                    error.line = lines.ToString();
                    error.description = "Le nombre de réponses doit être égale à 4.";
                    errors.Add(error);
                }

                lines++;
                existingLevels.Add(data.level);
            }
            for (int i = 1; i <= numberOfLevels; i++)
            {
                if (!existingLevels.Contains(i))
                {
                    Error error = new Error();
                    error.module = datas[0].moduleName;
                    error.level = i.ToString();
                    error.description = "Le level " + i + " n'existe pas pour ce module.";
                    errors.Add(error);
                }
            }
            WriteError(errors);
        }



        public static bool IsAnwserInsideQuestion(string question, List<string> _anwsers)
        {
            bool anwserIsFound = false;
            foreach (string a in _anwsers)
            {
                if (question.Contains(a) || question.Contains(a.ToLower()) || question.Contains(UppercaseFirst(a)))
                {
                    anwserIsFound = true;
                }
            }
            return anwserIsFound;
        }

        public static void WriteError(List<Error> errors)
        {
            string path = "";
            if(errors.Count > 0)
            {
                path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory)) + @"\Errors\" + errors[0].module + "_ERRORS_" + DateTime.Now.ToString(@"yyyy-MM-dd") + ".txt";// + DateTime.Now.ToString("h:mm:ss tt") + ".txt";
            }
            else
            {
                path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory)) + @"\Errors\_ERRORS_" + DateTime.Now.ToString(@"yyyy-MM-dd") + ".txt";// + DateTime.Now.ToString("h:mm:ss tt") + ".txt";
            }
            //Console.WriteLine(path);
            var sb = new System.Text.StringBuilder();
            foreach (Error e in errors)
            {
                string errorMessage = "";
                try
                {
                    errorMessage = "---------------------------------------------------------------------------------------";
                    sb.AppendLine(errorMessage);
                    errorMessage = "HEURE: " + DateTime.Now + "\n";
                    sb.AppendLine(errorMessage);
                    errorMessage = "MODULE: " + e.module;
                    sb.AppendLine(errorMessage);
                    errorMessage = "NIVEAU: " + e.level;
                    sb.AppendLine(errorMessage);
                    errorMessage = "MINI-JEU: " + e.miniGame ;
                    sb.AppendLine(errorMessage);
                    errorMessage = "LIGNE: " + e.line;
                    sb.AppendLine(errorMessage);
                    errorMessage = "DESCRIPTION: " + e.description + "\n";
                    sb.AppendLine(errorMessage);
                    errorMessage = "---------------------------------------------------------------------------------------";
                    sb.AppendLine(errorMessage);

                }catch(Exception ex)
                {
                    Console.WriteLine(ex.Data);
                }
            }


            using (var tw = new StreamWriter(path, true, Encoding.UTF8))
            {
                if (!File.Exists(path))
                {
                    File.Create(path);
                    System.Threading.Thread.Sleep(5000);
                }
                tw.WriteLine(sb.ToString());
                Console.WriteLine("Saved " + path);
                tw.Close();
            }
        }

        private static List<ModuleDatas> ParseCSV(string path)
        {
            return File.ReadAllLines(path)
                .Skip(3)
                .Where(row => row.Length > 0)
                .Select(ModuleDatas.ParseRow).ToList();
        }

        /// <summary>
        /// CREATE QUIZ JSON DATA FILE
        /// </summary>
        /// <param name="datas"></param>
        public static void CreateQuizJSONFile(List<ModuleDatas> datas)
        {
            string json = "";
            string path = "";
            string moduleName = "";
            string dir = "";
            QuizDataStruct quizzes = new QuizDataStruct();
            quizzes.quiz_sentence = new Quiz[GetNumberOfRowByGame(GAME_TYPE.QUIZ, datas)];
            int index = 1;
            int i = 0;
            List<string> correctAnwsersIndexes = new List<string>();

            List<ModuleDatas> quizData = datas.Where(d => d.gameType == m_gameType[(int)GAME_TYPE.QUIZ]).ToList();
            //we should order the data by level so that we generate each level to the correct folder
            quizData = quizData.OrderBy(o => o.level).ToList();
            //we use last level to check if we have changed  level 
            int lastLevel = 1;
            quizzes.quiz_sentence = new Quiz[GetNumberOfRowsByLevel(quizData[0].level, quizData)];
            foreach (ModuleDatas data in quizData)
            {
                Quiz quiz = new Quiz();
                index = 1;
                correctAnwsersIndexes = data.goodAnwsers.Split(',').ToList();
                //before we start processing the data, we should check if the last level is 
                //equal to the current level otherwise we generate the json file and we start 
                //processing the new level datas
                if (data.level != lastLevel)
                {

                    //Create JSON file 
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(quizzes);
                    moduleName = m_moduleName.Where(m => m.ToLower().Equals(data.moduleName.ToLower())).FirstOrDefault();
                    //IF THE DATA LEVEL IS DIFFERENT FROM THE LAST LEVEL THEN WE SHOULD SAVE THE LAST LEVEL 
                    path = m_gamePath[(int)GAME_TYPE.QUIZ] + moduleName + "\\" + "Level_" + (data.level - 1) + "\\Quiz.json";

                    dir = m_gamePath[(int)GAME_TYPE.QUIZ] + moduleName + "\\" + "Level_" + (data.level - 1);
                    VeriFyDirectory(dir);
                    //we should create the JSON  file
                    SaveJSONFile(path, json);
                    quizzes.quiz_sentence = new Quiz[GetNumberOfRowsByLevel(data.level, quizData)];
                    i = 0;
                }

                quiz.anwser = new Anwser[data.anwsers.Where(s => !s.Equals("")).ToList().Count];

                foreach (string a in data.anwsers)
                {
                    if (!a.Equals(""))
                    {
                        Anwser anwser = new Anwser();
                        anwser.text_content = a;
                        //check if the anwser is amongst the correct one
                        if (correctAnwsersIndexes.Where(s => s.ToLower().Equals(index.ToString().ToLower())).FirstOrDefault() != null)
                        {
                            anwser.is_correct = true;
                        }
                        else
                        {
                            anwser.is_correct = false;
                        }
                        quiz.sentence = data.question;
                        quiz.gameDifficulty = GetDataDifficulty(data.difficulty);
                        quiz.anwser[index - 1] = anwser;
                        index++;
                    }
                }

                quizzes.quiz_sentence[i] = quiz;
                i++;
                //assign the last level index 
                lastLevel = data.level;
                //assign the module so that ^we get the value of the last module name
                moduleName = m_moduleName.Where(m => m.ToLower().Equals(data.moduleName.ToLower())).FirstOrDefault();
            }


            //WE SHOULD SAVE THE LAST PROCESSED DATA 

            //Create JSON file 
            json = Newtonsoft.Json.JsonConvert.SerializeObject(quizzes);
            //IF THE DATA LEVEL IS DIFFERENT FROM THE LAST LEVEL THEN WE SHOULD SAVE THE LAST LEVEL 
            path = m_gamePath[(int)GAME_TYPE.QUIZ] + moduleName + "\\" + "Level_" + lastLevel + "\\Quiz.json";
            dir = m_gamePath[(int)GAME_TYPE.QUIZ] + moduleName + "\\" + "Level_" + lastLevel;
            VeriFyDirectory(dir);
            //we should create the JSON  file
            SaveJSONFile(path, json);

        }


        public static void CreateStoryTellingJSONFile(List<ModuleDatas> datas)
        {

            string json = "";
            string path = "";
            string moduleName = "";
            Storytelling_struct storyTelling = new Storytelling_struct();
            storyTelling.levels = new List<Storytelling_level_struct>();

            foreach (ModuleDatas data in datas)
            {
                Storytelling_level_struct level = new Storytelling_level_struct();
                level.level = data.level;
                level.intro =
                level.intro =
                //SET INTRO INFORMATIONS
                level.intro = new Storytelling_preset_struct();
                level.intro.text = data.entryMessage;
                level.intro.background = "";
                level.intro.character = "player";

                //SET OUTRO INFORMATIONS
                level.outro = new Storytelling_preset_struct();
                level.outro.text = data.exitMessage;
                level.outro.background = "";
                level.outro.character = "player";

                if (storyTelling.levels.Where(l => l.level == level.level).FirstOrDefault() == null) //veriffy if the level hasn't been added before
                {
                    storyTelling.levels.Add(level);
                }

            }

            //Create JSON file 
            json = Newtonsoft.Json.JsonConvert.SerializeObject(storyTelling);
            moduleName = m_moduleName.Where(m => m.ToLower().Equals(datas[0].moduleName.ToLower())).FirstOrDefault();
            //IF THE DATA LEVEL IS DIFFERENT FROM THE LAST LEVEL THEN WE SHOULD SAVE THE LAST LEVEL 
            path = "../../Assets/Resources/Game_presets/Storytelling/Storytelling_" + UppercaseFirst(moduleName) + ".json";

            string dir = "../../Assets/Resources/Game_presets/Storytelling/Storytelling_" + UppercaseFirst(moduleName);
            VeriFyDirectory(dir);
            //we should create the JSON  file
            SaveJSONFile(path, json);
        }

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            s = s.ToLower();
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }


        public static void CreateWordCloudsJSONFile(List<ModuleDatas> datas)
        {
            string json = "";
            string path = "";
            string moduleName = "";
            string dir = "";
            Word_clouds_quiz wordCloudsDatas = new Word_clouds_quiz();
            int index = 1;
            int i = 0;
            List<ModuleDatas> wordCloudData = datas.Where(d => d.gameType == m_gameType[(int)GAME_TYPE.WORD_CLOUDS]).ToList();
            //we should order the data by level so that we generate each level to the correct folder
            wordCloudData = wordCloudData.OrderBy(o => o.level).ToList();
            //we use last level to check if we have changed  level 
            int lastLevel = 1;
            wordCloudsDatas.quizes = new Word_clouds_quiz_item[GetNumberOfRowsByLevel(wordCloudData[0].level, wordCloudData)];
            foreach (ModuleDatas data in wordCloudData)
            {
                //before we start processing the data, we should check if the last level is 
                //equal to the current level otherwise we generate the json file and we start 
                //processing the new level datas
                if (data.level != lastLevel)
                {

                    //Create JSON file 
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(wordCloudsDatas);
                    moduleName = m_moduleName.Where(m => m.ToLower().Equals(data.moduleName.ToLower())).FirstOrDefault();
                    //IF THE DATA LEVEL IS DIFFERENT FROM THE LAST LEVEL THEN WE SHOULD SAVE THE LAST LEVEL 
                    path = m_gamePath[(int)GAME_TYPE.WORD_CLOUDS] + moduleName + "\\" + "Level_" + (data.level - 1) + "\\Word_clouds_data.json";

                    dir = m_gamePath[(int)GAME_TYPE.WORD_CLOUDS] + moduleName + "\\" + "Level_" + (data.level - 1);
                    VeriFyDirectory(dir);
                    //we should create the JSON  file
                    SaveJSONFile(path, json);
                    wordCloudsDatas.quizes = new Word_clouds_quiz_item[GetNumberOfRowsByLevel(data.level, wordCloudData)];
                    i = 0;
                }
                index = 1;
                int relatedIndex = 0;
                Word_clouds_quiz_item quiz = new Word_clouds_quiz_item();
                quiz.words = new string[data.anwsers.Count];
                if (data.gameType.ToLower().Equals(m_gameType[(int)GAME_TYPE.WORD_CLOUDS].ToLower()))
                {
                    quiz.key_word = data.question;
                    quiz.gameDifficulty = GetDataDifficulty(data.difficulty);
                    quiz.words.ToList();
                    quiz.related = new string[data.goodAnwsers.Split(',').Length];
                    while ((relatedIndex) != data.goodAnwsers.Split(',').Length)
                    {
                        if (int.TryParse(data.goodAnwsers.Split(',')[relatedIndex], out int n))
                        {
                            if (index == int.Parse(data.goodAnwsers.Split(',')[relatedIndex]))
                            {
                                quiz.related[relatedIndex] = data.anwsers[index - 1];
                                relatedIndex++;
                            }
                        }
                        index++;
                    }
                    quiz.words = data.anwsers.ToArray();
                    Console.WriteLine(data.anwsers.Count);
                }
                wordCloudsDatas.quizes[i] = quiz;
                i++;
                //assign the last level index 
                lastLevel = data.level;
                //assign the module so that ^we get the value of the last module name
                moduleName = m_moduleName.Where(m => m.ToLower().Equals(data.moduleName.ToLower())).FirstOrDefault();
            }
            //WE SHOULD SAVE THE LAST PROCESSED DATA 

            //Create JSON file 
            json = Newtonsoft.Json.JsonConvert.SerializeObject(wordCloudsDatas);
            //IF THE DATA LEVEL IS DIFFERENT FROM THE LAST LEVEL THEN WE SHOULD SAVE THE LAST LEVEL 
            path = m_gamePath[(int)GAME_TYPE.WORD_CLOUDS] + moduleName + "\\" + "Level_" + lastLevel + "\\Word_clouds_data.json";

            dir = m_gamePath[(int)GAME_TYPE.WORD_CLOUDS] + moduleName + "\\" + "Level_" + lastLevel;
            VeriFyDirectory(dir);
            //we should create the JSON  file
            SaveJSONFile(path, json);
        }

        public static void CreateOnePicFourWordsJSONFile(List<ModuleDatas> datas)
        {
            string json = "";
            string path = "";
            string moduleName = "";
            OnePicFourWordsLevel onePicFourWordsLevelDatas = new OnePicFourWordsLevel();
            int index = 1;
            int i = 0;
            List<string> correctAnwsersIndexes = new List<string>();



            List<ModuleDatas> OnePicFourWordsDatas = datas.Where(d => d.gameType == m_gameType[(int)GAME_TYPE.ONE_PIC_FOUR_WORDS]).ToList();
            //we should order the data by level so that we generate each level to the correct folder
            OnePicFourWordsDatas = OnePicFourWordsDatas.OrderBy(o => o.level).ToList();
            //we use last level to check if we have changed  level 
            int lastLevel = 1;
            if (OnePicFourWordsDatas.Count > 0)
            {
                onePicFourWordsLevelDatas.level_1 = new OnePicFourWordsData[GetNumberOfRowsByLevel(OnePicFourWordsDatas[0].level, OnePicFourWordsDatas)];
                foreach (ModuleDatas data in OnePicFourWordsDatas)
                {

                    //before we start processing the data, we should check if the last level is 
                    //equal to the current level otherwise we generate the json file and we start 
                    //processing the new level datas
                    if (data.level != lastLevel)
                    {

                        //Create JSON file 
                        json = Newtonsoft.Json.JsonConvert.SerializeObject(onePicFourWordsLevelDatas);
                        moduleName = m_moduleName.Where(m => m.ToLower().Equals(data.moduleName.ToLower())).FirstOrDefault();
                        //IF THE DATA LEVEL IS DIFFERENT FROM THE LAST LEVEL THEN WE SHOULD SAVE THE LAST LEVEL 
                        path = m_gamePath[(int)GAME_TYPE.ONE_PIC_FOUR_WORDS] + moduleName + "\\" + "Level_" + (data.level - 1) + "\\OnePicFourWordsDatas.json";

                        string dir = m_gamePath[(int)GAME_TYPE.ONE_PIC_FOUR_WORDS] + moduleName + "\\" + "Level_" + (data.level - 1);
                        VeriFyDirectory(dir);
                        //we should create the JSON  file
                        SaveJSONFile(path, json);
                        onePicFourWordsLevelDatas.level_1 = new OnePicFourWordsData[GetNumberOfRowsByLevel(data.level, OnePicFourWordsDatas)];
                        i = 0;
                    }
                    OnePicFourWordsData onePicFourWordsData = new OnePicFourWordsData();
                    onePicFourWordsData.value = new keyWordValue();
                    index = 1;
                    correctAnwsersIndexes = data.goodAnwsers.Split(',').ToList();
                    onePicFourWordsData.value.words = new string[data.anwsers.Where(s => !s.Equals("")).ToList().Count];
                    onePicFourWordsData.value.gameDifficulty = GetDataDifficulty(data.difficulty);
                    // get the full stop index 
                    int fullStopIndex = data.question.IndexOf('.');
                    moduleName = m_moduleName.Where(m => m.ToLower().Equals(data.moduleName.ToLower())).FirstOrDefault();
                    string imagePath = "Games\\One_pic_four_words\\Images\\" + data.question.Substring(0, fullStopIndex);
                    onePicFourWordsData.value.imageUrl = imagePath;
                    foreach (string a in data.anwsers)
                    {
                        if (a != "")
                        {
                            //check if the anwser is amongst the correct one
                            if (correctAnwsersIndexes.Where(s => s.ToLower().Equals(index.ToString().ToLower())).FirstOrDefault() != null)
                            {
                                onePicFourWordsData.value.keyword = a;
                                onePicFourWordsData.key = a;
                            }

                            onePicFourWordsData.value.words[index - 1] = a;
                            index++;
                        }
                    }

                    onePicFourWordsLevelDatas.level_1[i] = onePicFourWordsData;

                    i++;
                    //assign the last level index 
                    lastLevel = data.level;
                    //assign the module so that ^we get the value of the last module name
                    moduleName = m_moduleName.Where(m => m.ToLower().Equals(data.moduleName.ToLower())).FirstOrDefault();

                }

            }

        }


        public static void CreateHangedJSONFile(List<ModuleDatas> datas)
        {
            string json = "";
            string path = "";
            string moduleName = "";
            Hanged_struct hangedLevelDatas = new Hanged_struct();
            List<string> correctAnwsersIndexes = new List<string>();
            List<ModuleDatas> hangedDatas = datas.Where(d => d.gameType == m_gameType[(int)GAME_TYPE.PENDU]).ToList();
            //we should order the data by level so that we generate each level to the correct folder
            hangedLevelDatas.m_difficulties = new List<Hanged_difficulty>(3);
            for (int i = 0; i < 3; i++)
            {
                hangedLevelDatas.m_difficulties.Add(new Hanged_difficulty());
            }
            if (hangedDatas.Count > 0) //before processing the data, we must check if the list isn't empty
            {
                hangedDatas = hangedDatas.OrderBy(o => o.level).ToList();
                int ind = 0;
                foreach (string s in m_difficulties)
                {
                    List<ModuleDatas> difficultyDatas = new List<ModuleDatas>();
                    difficultyDatas = hangedDatas.Where(h => h.difficulty.ToLower().Equals(s.ToLower())).ToList();

                    //we use last level to check if we have changed  level 
                    hangedLevelDatas.m_difficulties[ind].difficulty = s;
                    hangedLevelDatas.m_difficulties[ind].levelOne = InitHangedLevelValues(1, difficultyDatas);
                    hangedLevelDatas.m_difficulties[ind].levelTwo = InitHangedLevelValues(2, difficultyDatas);
                    hangedLevelDatas.m_difficulties[ind].levelThree = InitHangedLevelValues(3, difficultyDatas);
                    hangedLevelDatas.m_difficulties[ind].levelFour = InitHangedLevelValues(4, difficultyDatas);
                    hangedLevelDatas.m_difficulties[ind].levelFive = InitHangedLevelValues(5, difficultyDatas);
                    hangedLevelDatas.m_difficulties[ind].levelSix = InitHangedLevelValues(6, difficultyDatas);
                    ind++;
                }
                //Create JSON file 
                json = Newtonsoft.Json.JsonConvert.SerializeObject(hangedLevelDatas);
                moduleName = m_moduleName.Where(m => m.ToLower().Equals(hangedDatas[0].moduleName.ToLower())).FirstOrDefault();
                //IF THE DATA LEVEL IS DIFFERENT FROM THE LAST LEVEL THEN WE SHOULD SAVE THE LAST LEVEL 
                path = m_gamePath[(int)GAME_TYPE.PENDU] + "\\" + moduleName + "\\" + "Levels" + "\\HangedLevelsData.json";

                string dir = m_gamePath[(int)GAME_TYPE.PENDU] + "\\" + moduleName + "\\" + "Levels";
                VeriFyDirectory(dir);
                //we should create the JSON  file
                SaveJSONFile(path, json);
            }
        }


        //EXPORT MISSING WORD DATAS
        public static void CreateMissingWordsJSONFile(List<ModuleDatas> datas)
        {

            string json = "";
            string path = "";
            string moduleName = "";
            string dir = "";
            SentencesDataStruct sentencesDatas = new SentencesDataStruct();
            List<ModuleDatas> sentencedData = datas.Where(d => d.gameType == m_gameType[(int)GAME_TYPE.MISSING_WORDS]).ToList();
            //we should order the data by level so that we generate each level to the correct folder
            sentencedData = sentencedData.OrderBy(o => o.level).ToList();
            sentencesDatas.sentences = new Sentence[sentencedData.Where(s => s.level == 1).ToList().Count];
            int ind = 0;
            int index = 0;
            int lastLevel = 0;
            int level = 1;
            if (sentencedData.Count > 0) //we must check if the list contains at least one value before processing the datas
            {
                foreach (ModuleDatas data in sentencedData)
                {
                    Sentence sentence = new Sentence();
                    index = 0;
                    sentence.sentence = data.question.Split('—')[0];
                    sentence.author = "—" + data.question.Split('—').Skip(1).FirstOrDefault();
                    sentence.gameDifficulty = GetDataDifficulty(data.difficulty);
                    Difficulty lvlDifficulty = new Difficulty();
                    sentence.words = new string[data.anwsers.Where(s => !s.Equals("")).ToList().Count()];
                    foreach (string s in data.anwsers.Where(s => !s.Equals("")).ToList())
                    {
                        sentence.words[index] = s;
                        index++;
                    }

                    sentence.difficulty = new List<Difficulty>();
                    for (int i = 0; i < 5; i++)
                    {
                        Difficulty difficulty = new Difficulty();
                        difficulty.missing_words = data.goodAnwsers.Split(',').Count();
                        difficulty.trap = data.anwsers.Where(s => !s.Equals("")).ToList().Count() - data.goodAnwsers.Split(',').Count();
                        sentence.difficulty.Add(difficulty);
                    }

                    moduleName = data.moduleName;
                    //Create JSON file
                    if (level != data.level)
                    {
                        SentencesDataStruct sentencesLevelDatas = new SentencesDataStruct();
                        sentencesLevelDatas.sentences = new Sentence[sentencedData.Where(s => s.level == level).ToList().Count];
                        sentencesLevelDatas.sentences = sentencesDatas.sentences;
                        json = Newtonsoft.Json.JsonConvert.SerializeObject(sentencesLevelDatas);
                        if(sentencesDatas.sentences.Count() > 0)
                        {
                            //IF THE DATA LEVEL IS DIFFERENT FROM THE LAST LEVEL THEN WE SHOULD SAVE THE LAST LEVEL 
                            path = m_gamePath[(int)GAME_TYPE.MISSING_WORDS] + "\\" + moduleName + "\\" + "Level_" + (data.level) + "\\Missing_words_data.json";

                            dir = m_gamePath[(int)GAME_TYPE.MISSING_WORDS] + "\\" + moduleName + "\\" + "Level_" + (data.level);
                            VeriFyDirectory(dir);
                            //we should create the JSON  file
                            SaveJSONFile(path, json, true);
                        }
                        sentencesDatas.sentences = new Sentence[sentencedData.Where(s => s.level == data.level).ToList().Count];
                        ind = 0;
                    }
                    if(sentencesDatas.sentences.Count() > 0)
                    {
                        sentencesDatas.sentences[ind] = sentence;
                    }
                    lastLevel = data.level;
                    level = data.level;
                    ind++;
                }
                //WE SHOULD SAVE THE LAST PROCESSED DATA 

                //Create JSON file 
                json = Newtonsoft.Json.JsonConvert.SerializeObject(sentencesDatas);
                moduleName = m_moduleName.Where(m => m.ToLower().Equals(sentencedData[0].moduleName.ToLower())).FirstOrDefault();
                //IF THE DATA LEVEL IS DIFFERENT FROM THE LAST LEVEL THEN WE SHOULD SAVE THE LAST LEVEL 
                path = m_gamePath[(int)GAME_TYPE.MISSING_WORDS] + "\\" + moduleName + "\\" + "Level_" + lastLevel + "\\Missing_words_data.json";

                dir = m_gamePath[(int)GAME_TYPE.MISSING_WORDS] + "\\" + moduleName + "\\" + "Level_" + lastLevel;
                VeriFyDirectory(dir);
                //we should create the JSON  file
                SaveJSONFile(path, json);

            }
        }

        public static List<String> InitHangedLevelValues(int levelIndex, List<ModuleDatas> hangedDatas)
        {
            List<string> level = new List<string>();
            foreach (ModuleDatas data in hangedDatas)
            {

                //before we start processing the data, we should check if the last level is 
                //equal to the current level otherwise we generate the json file and we start 
                //processing the new level datas



                if (data.level == levelIndex)
                {
                    level.Add(data.question);
                }


            }
            return level;
        }



        public static int GetNumberOfRowByGame(GAME_TYPE type, List<ModuleDatas> datas)
        {
            int counter = 0;
            foreach (ModuleDatas data in datas)
            {
                if (data.gameType.ToLower().Equals(m_gameType[(int)type].ToLower()))
                {
                    counter++;
                }
            }
            return counter;
        }


        public static int GetNumberOfRowsByLevel(int level, List<ModuleDatas> datas)
        {

            int counter = 0;
            foreach (ModuleDatas data in datas)
            {
                if (data.level == level)
                {
                    counter++;
                }
            }
            return counter;
        }


        /// <summary>
        /// SAVE THE JSON FILE BY USING THE JSON STRING AND THE PATH
        /// </summary>
        /// <param name="path"></param>
        /// <param name="JSONString"></param>
        public static void SaveJSONFile(string path, string json, bool _append = false)
        {
            //FileStream fcreate = File.Open(path, FileMode.Create);

            using (var tw = new StreamWriter(path, _append, Encoding.UTF8))
            {
                if (!File.Exists(path))
                {
                    File.Create(path);
                    System.Threading.Thread.Sleep(5000);
                }
                tw.WriteLine(json);
                Console.WriteLine("Saved " + path);
                tw.Close();
            }
        }

        public static void VeriFyDirectory(string dir)
        {

            bool dirExists = System.IO.Directory.Exists(dir);  //verify if the path exist
            if (!dirExists)
            {
                //File.SetAttributes(dir, FileAttributes.Normal);
                System.IO.Directory.CreateDirectory(dir);
                System.Threading.Thread.Sleep(5000);
            }
        }
    }

    public class Error
    {
        public string module;
        public string miniGame;
        public string level;
        public string line;
        public string description;
    }

    public class ModuleDatas
    {
        public string moduleName { get; set; }
        public int level { get; set; }
        public string levelName { get; set; }
        public string entryMessage { get; set; }
        public string exitMessage { get; set; }
        public string question { get; set; }
        public string difficulty { get; set; }
        public string gameType { get; set; }
        public string goodAnwsers { get; set; }
        public string checkAnwsers { get; set; }
        public List<string> anwsers { get; set; }

        internal static ModuleDatas ParseRow(string row)
        {
            var columns = row.Split(';');
            List<string> responses = new List<string>();
            string col = "";
            for (int i = 10; i <= 17; i++)
            {
                col = columns[i];
                if (!col.Equals("")) //if the word cloud column is not empty then we could add it to the response
                {
                    responses.Add(col);
                }
            }

            return new ModuleDatas()
            {
                moduleName = columns[0],
                level = int.Parse(columns[1]),
                levelName = columns[2],
                entryMessage = columns[3],
                exitMessage = columns[4],
                question = columns[5],
                difficulty = columns[6],
                gameType = columns[7],
                goodAnwsers = columns[8],
                checkAnwsers = columns[9],
                anwsers = responses
            };
        }
    }
}
