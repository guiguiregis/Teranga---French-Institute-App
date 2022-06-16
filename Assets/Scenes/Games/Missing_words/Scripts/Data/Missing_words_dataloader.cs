using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class Missing_words_dataloader : MonoBehaviour
{
    public SentencesDataStruct loadedData;
    public MissingWordsGameDataStruct missingWordData = new MissingWordsGameDataStruct();
    //VARIABLE IN ORDER TO GENERATE ONE RANDOM SENTENCE
    public GameDataSentence generatedSentence = new GameDataSentence(); 
    string gameDataPath = "Games\\Missing_words\\";
    string gameLevelPath = "\\Missing_words_data";
    string defaultGameDataPath = "Games\\Missing_words\\Leadership\\Level_2\\Missing_words_data";
    //WORD BOX PREFAB SO THAT WE CAN INSTANTIATE THE PREFAB
    public GameObject wordBoxPrefab;
    //WORD SLOT SO THAT WE CAN INSTANTIATE THE MISSING SLOT PREFAB
    public GameObject WordSlot;
    //WORD PREFAB SO THAT WE CAN HAVE THE SAME STYLE FOR ALL WORDS IN THE SENTENCE
    public GameObject wordPrefab;
    //THE THE SENTENCE LAYOUT SO THAT WE CAN PUT ALL THE WORDS IN THE SENTENCE IN IT 
    public GameObject sentenceContainer;
    //THE MISSING WORD LAYOUT SO THAT WE CAN PUT ALL THE MISSING WORDS IN IT
    public GameObject missingWordsContainer;
    //LIST OF GENERATED WORD INDEXES
    public List<int> generatedIndex =  new List<int>();
    //CURRENT SENTENCE INDEX TO KNOW WHICH SENTENCE IS GENERATED
    public int currentSentenceIndex = 0;
    //ROW SENTENCE PREFAB
    public GameObject rowSentencePrefab;
    //GAME DIFFICULTY
    int gameLevelDifficulty = 0;
    //VARIABLE TO GET THE CORRESPONDING MISSING WORDS TO GENERATE, IT ALLOWS TO GENERATE A RANDOM POSITION WETHER IT IS THE FIRST 
    //OR THE LAST WORD IN THE SENTENCE
    public List<int> missingWordsPos = new List<int>();

    //NUMBER OF WORDS PER SENTENCE 
   static int numberOfWordsPerSentence = 7;

   float m_wordFontSize = 0;
   Vector3 m_wordFontScale;
    public static int m_lastSentenceIndex = 0;

    //VLayout configurations
    private float m_verticalLayoutSpacingForLongSentences = -55f;
    private float m_fontSizeForLongSentences = 32f;
    private float m_verticalLayoutSpacingForShortSentences = -60f;
    private Vector3 m_childScaleForLongSentences = new Vector3(0.8f, 0.3f, 0.5f);
    private float m_sentenceContainerYPosForLongSent = 250f;
    private float m_sentenceContainerYPosForShortSent = 330.0089f;
    private float m_sentenceContainerWidth = 600f;
    private float m_missingWordSlotWidth = 100f;

    private void Awake()
    {
        string path = "";
        if (StageManager.instance != null)
        {
            gameLevelDifficulty = StageManager.instance.GetInt("current_level_index"); // TODO: WRONG DIFFICULTY !!
            int index = gameLevelDifficulty + 1;
            path = gameDataPath + StageManager.instance.GetString("current_module_name") + "\\Level_" + index +  gameLevelPath;
        }
        LoadMissingWordsData(path);
        GenerateGameplayElementsOnScene();
 
    }

    public void LoadMissingWordsData(string path)
    {
        //string missingWordpath = gameDataPath + Path.DirectorySeparatorChar + path;
        TextAsset json = Resources.Load(path) as TextAsset;
        if (json != null)
        {
            Debug.Log(path);
            loadedData = JsonUtility.FromJson<SentencesDataStruct>(json.text);
        }
        else
        {
            //IF THE GIVEN PATH IS NOT FOUND THEN WE SHOULD USE THE DEFAULT PATH
            Debug.Log("loaded default path");
            json = Resources.Load(defaultGameDataPath) as TextAsset;
            loadedData = JsonUtility.FromJson<SentencesDataStruct>(json.text);
        }
    }

    /// <summary>
    /// Generate the differents elements on the scene
    /// </summary>
    public void GenerateGameplayElementsOnScene()
    {
        sentenceContainer.transform.localScale = new Vector3(1f, 1f, 1f);
        DestroyContainerElement();
        InitializeMissingWordsGameData();
        GenrateRandomSentence();
        GetPositionOfMissingWordsToGenerate();
        InitialiseSentenceInLayout();
        InitialiseMissingWordsOnLayout();
        Missing_words_manager.hasIncreasedScore = false;
    }

    public bool CheckIfMissingWordSlotIsGenerated(string missingWord)
    {
        bool result = false;
        foreach (Transform child in sentenceContainer.transform)
        {
            foreach(Transform c in child.transform)
            {
                if (c.GetComponent<WordSlot>() != null)
                {
                    if (c.GetComponent<WordSlot>().keyword == missingWord)
                    {
                        result = true;
                    }
                }
            }
        }
        return result;
    }
    public void DestroyContainerElement()
    {
        foreach (Transform child in sentenceContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in missingWordsContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void InitializeMissingWordsGameData()
    {
        //missingWordData.sentencesData
        foreach(Sentence s in loadedData.sentences)
        {
            //if(s.gameDifficulty.ToLower().Equals(StageManager.instance.GetGameCurrentDifficulty()))
            //{
                GameDataSentence g = new GameDataSentence();
                //GETTING EACH WORDS IN THE SENTENCE IN A STRING ARRAY
                g.sentenceWords = s.sentence.Split(' ');
                //GETTING EACH MISSING WORDS INSIDE THE LIST OF MISSING WORDS
                g.words = new List<string>();
                for (int i = 0; i < s.words.Length; i++)
                {
                    g.words.Add(s.words[i]);
                }
                //g.words = Shuffle<string>(g.words);
                g.authorName = s.author;
                g.difficulty = s.difficulty;
                //ADDING THE GAME DATA TO THE LIST OF GAME DATA
                missingWordData.sentencesData.Add(g);
            //}
        }
            
    }
    /// <summary>
    /// In order to shuffle the list of words once it has been added
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_list"></param>
    /// <returns></returns>
    public static List<T> Shuffle<T>(List<T> _list)
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

    public void GenrateRandomSentence()
    {
        //TO TEST, WE WILL JUST USE THE FIRST SENTENCE 
        currentSentenceIndex = GenerateSentenceIndex();
        generatedSentence = missingWordData.sentencesData[currentSentenceIndex];
      
    }

    public int  GenerateSentenceIndex()
    {
        int genIndex;
        //Changed:
        genIndex = Random.Range(0, missingWordData.sentencesData.Count);
        //genIndex = 10;
        while(CheckIfTheIndexIsAlreadyGenerated(genIndex))
        {
            genIndex = Random.Range(0, missingWordData.sentencesData.Count);
        }
        generatedIndex.Add(genIndex);
        return genIndex;


    }

    public bool CheckIfTheIndexIsAlreadyGenerated(int currentGeneratedIndex)
    {
        bool isExist = false;

        foreach (int i in generatedIndex)
        {
            if (i == currentGeneratedIndex)
            {
                isExist = true;
            }
        }
        return isExist;
    }

    /// <summary>
    /// INITIALIS
    /// </summary>
    public void InitialiseMissingWordsOnLayout()
    {
        bool shouldSkipMissingWord = true;
        int numbOfTrap = 0;
        bool shouldSkip = false;
        GameObject canvas = GameObject.Find("Screen_Canvas");
        float x = 0;
        float y = 0;
        float z = 0;
        generatedSentence.words = Shuffle<string>(generatedSentence.words);
        Debug.Log(generatedSentence.words.Count);
        foreach (string w in generatedSentence.words)
        {
            if (IsTrapWord(w))
            {
                if (numbOfTrap >= generatedSentence.difficulty[gameLevelDifficulty].trap)
                {
                    shouldSkip = true;
                }
                //WE SHOULD INCREASE THE NUMB OF TRAPS
                numbOfTrap++;
            }
            else
            {
                //IF IT IS NOT A TRAP WORD THEN WE SHOULD CHECK IF IT IS A MISSING WORD WHICH HIS SLOT IS GENERATED 
                //OTHERWISE WE SHOULD NOT GENERATE IT 
                if (CheckIfMissingWordSlotIsGenerated(w))
                {
                    shouldSkipMissingWord = false;
                }
            }
            if((!(shouldSkip) && (IsTrapWord(w))) || (!IsTrapWord(w) && !shouldSkipMissingWord))
            {
                GameObject newMissingWord = Instantiate(wordBoxPrefab, new Vector3(x, y, z), Quaternion.identity);
                newMissingWord.transform.localPosition = Vector2.zero;
                newMissingWord.transform.parent = missingWordsContainer.transform;
                newMissingWord.transform.localScale = wordBoxPrefab.transform.localScale;
                newMissingWord.GetComponent<DragAndDrop>().canvas = canvas.GetComponent<Canvas>();
                newMissingWord.GetComponentInChildren<TextMeshProUGUI>().text = w;
            }
            shouldSkipMissingWord = true;
        }
    }

 

    /// <summary>
    /// CHECK IF A WORD IN THE SENTENCE IS A MISSING WORD OR NOT IN ORDER TO PLACE A SLOT BOX 
    /// </summary>
    /// <param name="word">word to be checked</param>
    /// <returns></returns>
    public bool IsMissingWord(string word)
    {
        bool result = false;
        //int word_index = generatedSentence.sentenceWords.ToList().FindIndex(w => w == word);
        //foreach(string s in generatedSentence.sentenceWords)
        //{
        //    Debug.Log(s);
        //}

        //if (word_index != -1)
        //    result = true;

        if (IsLettersConsecutiveInArray(word, generatedSentence.ReturnWholeSentence()))
        {
            result = true;
        }

        return result;
    }


    public bool IsLettersConsecutiveInArray(string word, string sentence)
    {
        //bool isConsecutive = false;
        word = word.Replace(" ", "");
        int numConsecutive = 0;
        int j = 0; //save the true j value
        //string reformulatedSentence = "";
        //foreach(string s in sentence)
        //{
        //    reformulatedSentence += s;
        //}
        //Debug.Log(word);
        for (int i = 0; i < sentence.Length; i++)
        {
            //Debug.Log(word[j] + "  =  " + sentence[i]);
            if (word[j].ToString().ToLower().Equals(sentence[i].ToString().ToLower()))    //verify if it is the same character
            {
                numConsecutive++;               //increment the number of common character
                j++;                            //increment the index of the word counter
            }
            else
            {
                numConsecutive = 0;
                j = 0;
            }
            //Debug.Log("consecutive " + numConsecutive);
            if (numConsecutive == word.Length)  //if the the letters are consecutive and the the length of the word is equal to the number of consecutive letters than we found the word
            {
                        return true;
            }
        }



        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="word">used to seach position of the word in the sentence</param>
    /// <param name="words">List of missing words</param>
    /// <param name="wholeSentence">the whole sentence</param>
    /// <returns></returns>
    public string IsLettersConsecutiveInSentence(string word, List<string> words, string wholeSentence)
    {
        //bool isConsecutive = false;
        int numConsecutive = 0;
        int j = wholeSentence.IndexOf(word);    //we get the index of the word in the sentence so that we can search our missing word
        //var charsToRemove = new string[] {",", ".", ";", "'" };
        //foreach (var c in charsToRemove)
        //{
        //    word = word.Replace(c, string.Empty);
        //}
        if (word.Length > 0)
        {

            foreach (string s in words)  // foreach a word in CheckIfTheIndexIsAlreadyGenerated sentence we must check if it is the searched word 
            {
                //Debug.Log("compared words " + s );
                string curWord = s.Replace(" ", "");
                j = wholeSentence.IndexOf(word);
                numConsecutive = 0;
                for (int i = 0; i < curWord.Length; i++)
                {
                    if (wholeSentence[j].Equals(curWord[i]))//verify if it is the same character
                    {
                        numConsecutive++;   //if it is the same character, we increment the number of common characters
                        j = j + 1;          //we increment also the index of the word so that we can vverify if the next index is common
                    }
                    else
                    {
                        numConsecutive = 0; //if it is not the same character, we reset the counter
                        j = wholeSentence.IndexOf(word);    //and we reset the counter for the word
                    }

                    if (numConsecutive == curWord.Length)  //if the the letters are consecutive and the the length of the word is equal to the number of consecutive letters than we found the word
                    {
                        m_lastSentenceIndex = j;
                        return s;
                    }
                }
            }
        }


        return "";
    }

    public bool IsTrapWord(string word)
    {
        bool result = false;
        //int word_index = generatedSentence.sentenceWords.ToList().FindIndex(w => w == word);
     
        // if(word_index == -1)
        //    result = true;


        if (!IsLettersConsecutiveInArray(word, generatedSentence.ReturnWholeSentence()))
        {
            result = true;
        }

        return result;
    }

    /// <summary>
    /// Returns the number of missing words in a sentence
    /// </summary>
    /// <returns></returns>
    public int GetNumberOfMissingWords()
    {
        int counter = 0;
        
        foreach (string w in generatedSentence.sentenceWords)
        {
                if(!IsLettersConsecutiveInSentence(w, generatedSentence.words, generatedSentence.ReturnWholeSentence()).Equals(""))
                {
                    counter++;
                }
        }
        return counter;
    }

    /// <summary>
    /// GET THE POSITION OF THE MISSING WORDS TO GENERATE
    /// </summary>
    public void  GetPositionOfMissingWordsToGenerate()
    {
        int pos;
        for(int j = 0; j < generatedSentence.difficulty[gameLevelDifficulty].missing_words; j++)
        {
            pos = Random.Range(0, GetNumberOfMissingWords());
            if (missingWordsPos != null)
            {
                foreach (int i in missingWordsPos)
                {
                    if (pos == i)
                    {
                        pos = Random.Range(0, GetNumberOfMissingWords());
                    }
                }
            }
            missingWordsPos.Add(pos);
        }
    }


    // Get among missing words , N random existing missing words in order to randomize slots  generation
    public List<string> GetRandomSlotWords(List<string> words , int missingWordsLength)
    {
        // Shuffle order
        words = Shuffle(words);
        List<string> missing_words = new List<string>();
        foreach (string w in words)
        {
            if (IsMissingWord(w) && (missing_words.Count < missingWordsLength))
             {
                missing_words.Add(w);
             }

        }
        return missing_words; 
    }


    // Get among missing words , N random trap words
    public List<string> GetRandomTrapWords(List<string> words , int trapWordsLength)
    {
        // Shuffle order
        words = Shuffle(words);
        List<string> trap_words = new List<string>();
        foreach (string w in words)
        {

            if (IsTrapWord(w) && (trap_words.Count < trapWordsLength))
             {
                trap_words.Add(w);
             }

        }
        return trap_words; 
    }

    public int GetWordAtCurrentIndex(int _lastIndex, string[] words)
    {
        int charCounter = 0;
        int wordIndex = 0;
        foreach(string w in words)
        {
            for(int i = 0; i < w.Length; i++)
            {
                charCounter += 1;
                if(charCounter == _lastIndex)
                {
                    return wordIndex;
                }
            }
            wordIndex++;
        }
        return wordIndex;
    }

    /// <summary>
    /// INITIALIZE EVERY WORD IN THE SENTENCE LAYOUT
    ///char </summary>
    public void InitialiseSentenceInLayout()
    {
        //WE COUNT THE NUMBER OF WORDS AND IF WE REACH 8 THEN WE SHOULD CREATE A NEW ROW AND ATTACH
        //THE FOLLOWING WORDS TO IT
        int currentRowIndex = 0;
        List<GameObject> rowList = new List<GameObject>();
        rowList.Add(Instantiate(rowSentencePrefab, new Vector3(0, 0, 0), Quaternion.identity));
        rowList[currentRowIndex].transform.SetParent(sentenceContainer.transform, false);
        int wordCounter = 0;
        List<string> generated_slots = new List<string>();
        int totalAvailableWords = generatedSentence.difficulty[gameLevelDifficulty].missing_words;
        int difficultyTrapWords = generatedSentence.difficulty[gameLevelDifficulty].trap;

        int difficultyMissingWords = totalAvailableWords - difficultyTrapWords;

        List<string> slotWords = generatedSentence.words.ToList();
        List<string> trapWords = GetRandomTrapWords(generatedSentence.words.ToList(), difficultyTrapWords);
        List<string> sentenceWords = generatedSentence.sentenceWords.ToList();
        float width = 0f;

        for (int i = 0; i < sentenceWords.Count; i++)
        {
            string w = sentenceWords[i];
            //if(generatedSentence.ReturnWholeSentence().IndexOf(w) < m_lastSentenceIndex)
            //{
            //    i = GetWordAtCurrentIndex(m_lastSentenceIndex, sentenceWords.ToArray());
            //    w = sentenceWords[i];
            //}
            wordCounter++;

            string foundSlotWord = IsLettersConsecutiveInSentence(w, slotWords, generatedSentence.ReturnWholeSentence());
            string foundTrapWord = IsLettersConsecutiveInSentence(w, trapWords, generatedSentence.ReturnWholeSentence());
            
            if (!foundSlotWord.Equals("") || !foundTrapWord.Equals(""))
            {
                // if not yet generated : in order to avoid duplicated missing words
                if(!generated_slots.Contains(foundSlotWord) && !foundSlotWord.Equals(""))
                {
                    generated_slots.Add(foundSlotWord);
                    width += GenertateWordSlotBoxInGrouplayout(foundSlotWord, rowList[currentRowIndex]);
                }
                else
                {
                    width += GenerateWordInGroupLayout(foundSlotWord, rowList[currentRowIndex]);
                }

            }
            else
            {
                width += GenerateWordInGroupLayout(w, rowList[currentRowIndex]); 
            }
            
            if(i == sentenceWords.Count - 1)
            {
                Vector3 locScale = sentenceContainer.transform.localScale;
                float baseFactor = 0.8f;
                sentenceContainer.transform.localScale = new Vector3(locScale.x * baseFactor, locScale.y * baseFactor, locScale.z * baseFactor);
            }


            if (/*wordCounter == numberOfWordsPerSentence*/ width >= m_sentenceContainerWidth && i != (sentenceWords.Count - 2))    //verify if we are not at the last word before going to another row
            {
                wordCounter = 0;
                width = 0f;
                rowList.Add(Instantiate(rowSentencePrefab, new Vector3(0, 0, 0), Quaternion.identity));
                currentRowIndex++;
                rowList[currentRowIndex].transform.SetParent(sentenceContainer.transform, false);
            }


            if (foundSlotWord != "")
            {
                int currIndex = GetIteratorPosition(foundSlotWord, sentenceWords, generatedSentence.ReturnWholeSentence());
                if(currIndex != -1)
                {
                    i = currIndex;
                }
            }
            //if(rowList[currentRowIndex].GetComponent<TextMeshProUGUI>().sizeDelta)

        }
        //ADD THE AUTHOR NAME TO A NEW LINE
        rowList.Add(Instantiate(rowSentencePrefab, new Vector3(0, 0, 0), Quaternion.identity));
        currentRowIndex++;
        rowList[currentRowIndex].transform.SetParent(sentenceContainer.transform, false);

        //if (rowList.Count > 4)  //verify if the number of rows is greater than 4
        //{
        //    // rowList = ResizeListContainer(rowList);
        //}
        //else
        //{
        //    // sentenceContainer.GetComponent<VerticalLayoutGroup>().spacing = m_verticalLayoutSpacingForShortSentences;
        //    // sentenceContainer.transform.position = new Vector3(sentenceContainer.transform.position.x, m_sentenceContainerYPosForShortSent, sentenceContainer.transform.position.z);
        //}
        sentenceContainer = ReverseChildren(sentenceContainer);
        // GenerateWordInGroupLayout(generatedSentence.authorName, rowList[currentRowIndex]);  
        SetupSlotsFontSize();

    }

    public int GetIteratorPosition(string _word, List<string> _words,  string _sentence)
    {
        int i = 0;
        int j = 0;
        int index = -1;
        int numConsecutive = 0;
        //foreach(string s in _listWords)
        //{
        //    if(_word.Contains(s))
        //    {
        //        index = i;
        //    }
        //    i++;
        //}
        string curWord = _word.Replace(" ", "");

        foreach (string w in _words)  // foreach a word in CheckIfTheIndexIsAlreadyGenerated sentence we must check if it is the searched word 
        {
            for(int k = 0; k < w.Length; k++)
            {
                if (w[k].ToString().ToLower().Equals(curWord[j].ToString().ToLower()))//verify if it is the same character
                {
                    numConsecutive++;   //if it is the same character, we increment the number of common characters
                    j = j + 1;          //we increment also the index of the word so that we can vverify if the next index is common
                }
                else
                {
                    numConsecutive = 0;
                    j = 0;
                }


                if (numConsecutive == curWord.Length)  //if the the letters are consecutive and the the length of the word is equal to the number of consecutive letters than we found the word
                {
                    index = i;
                    return index;
                }
            }
            i++;
        }
        return index;
    }
    public List<GameObject> ResizeListContainer(List<GameObject> _list)
    {
            // sentenceContainer.GetComponent<VerticalLayoutGroup>().spacing = m_verticalLayoutSpacingForLongSentences;
            // sentenceContainer.transform.position = new Vector3(sentenceContainer.transform.position.x, m_sentenceContainerYPosForLongSent, sentenceContainer.transform.position.z);
            foreach (GameObject obj in _list)
            {
                foreach (Transform child in obj.transform)
                {
                    if (child.GetComponent<TextMeshProUGUI>() != null)
                    {
                        child.GetComponent<TextMeshProUGUI>().fontSizeMin = m_fontSizeForLongSentences;
                        child.GetComponent<TextMeshProUGUI>().fontSizeMax = m_fontSizeForLongSentences;
                    }
                    else
                    {
                        child.transform.localScale = m_childScaleForLongSentences;
                    }
                }
            }
        return _list;
    }

    private GameObject ReverseChildren(GameObject obj)
    {
        for (var i = 1; i < obj.transform.childCount; i++)
        {
            obj.transform.GetChild(0).SetSiblingIndex(obj.transform.childCount - i);
        }
        return obj;
    }

    public float GenertateWordSlotBoxInGrouplayout(string wordText, GameObject sentence)
    {
        float x = wordBoxPrefab.transform.position.x;
        float y = wordBoxPrefab.transform.position.y;
        float z = wordBoxPrefab.transform.position.z;
        GameObject wordSlot = Instantiate(WordSlot, new Vector3(x, y, z), Quaternion.identity);
        wordSlot.transform.SetParent(sentence.transform, false);
        //wordSlot.transform.localScale = new Vector3(wordSlot.transform.localScale.x, WordSlot.transform.localScale.y, WordSlot.transform.localScale.z);
        //ASSIGNING THE WORD SLOT KEY TO THE KEY IN THE SCRIPT OF THE WORD SLOT
        wordSlot.GetComponent<WordSlot>().keyword = wordText;
        return m_missingWordSlotWidth;
    }



    public float GenerateWordInGroupLayout(string wordText, GameObject sentence)
    {
        float x = wordPrefab.transform.position.x;
        float y = wordPrefab.transform.position.y;
        float z = wordPrefab.transform.position.z;
        GameObject word = Instantiate(wordPrefab, new Vector3(x, y, z), Quaternion.identity);
        word.transform.parent = sentence.transform;
        word.transform.localScale = wordPrefab.transform.localScale;
        word.GetComponent<TextMeshProUGUI>().text = wordText;

        m_wordFontSize = word.GetComponent<TextMeshProUGUI>().fontSize;
        m_wordFontScale = word.transform.localScale;
        return word.GetComponent<TextMeshProUGUI>().preferredWidth;
    }

    void SetupSlotsFontSize()
    {
        foreach (Transform item in missingWordsContainer.transform)
        {
            item.GetComponentInChildren<TextMeshProUGUI>().fontSizeMax = m_wordFontSize;
            // item.GetChild(0).localScale = m_wordFontScale;
        }
    }
}
