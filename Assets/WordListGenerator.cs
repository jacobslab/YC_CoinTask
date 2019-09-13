using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
public class WordListGenerator : MonoBehaviour
{

    public string fileName = "RAM_wordpool.txt";
    private string[] words;
    private static string[] finalList = new string[300];
    public static bool canSelectWords = false;

    List<string> tempWords = new List<string>();
    List<string> selectableWords = new List<string>();

    //public WordEncodingLogTrack wordEncodingLogTrack;
    //SINGLETON
    private static WordListGenerator _instance;

    public static WordListGenerator Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null)
        {
            Debug.Log("Instance already exists!");
            return;
        }
        _instance = this;

        QualitySettings.vSyncCount = 1;
    }

    void Start()
    {
        StartCoroutine("GenerateWordList");
    }

    //select words when queried by BillboardText.cs
    public string SelectWords()
    {
        int selectedWordIndex = Random.Range(0, selectableWords.Count);
        string selectedWord = selectableWords[selectedWordIndex];
        selectableWords.RemoveAt(selectedWordIndex);
        UnityEngine.Debug.Log("length of word pool is: " + selectableWords.Count);
        return selectedWord;
    }

    public IEnumerator GenerateWordList()
    {

        tempWords = new List<string>();
        selectableWords = new List<string>();
        yield return StartCoroutine("ReadWordpoolFile");
        yield return StartCoroutine("ShuffleWords");
        canSelectWords = true;
        // BillboardText.InitiateWord();
        //yield return StartCoroutine("CreateSelectableWordList");
        yield return null;

    }

    //read all the possible words from the wordpool
    IEnumerator ReadWordpoolFile()
    {
        words = System.IO.File.ReadAllLines(fileName);
        PrintAllWords();

        yield return null;
    }

    //add to a temporary list
    List<string> AddWordsToList(string[] wordArray)
    {
        List<string> temp = new List<string>();
        for (int i = 0; i < wordArray.Length; i++)
        {
            temp.Add(wordArray[i]);
        }
        UnityEngine.Debug.Log("The word array length is: " + wordArray.Length);
        return temp;
    }

    //shuffle the words
    IEnumerator ShuffleWords()
    {
        string allwords = "";
        tempWords = AddWordsToList(words);
        UnityEngine.Debug.Log("The temp words count is " + tempWords.Count);
        while (tempWords.Count > 0)
        {
            //   UnityEngine.Debug.Log("in here");
            // UnityEngine.Debug.Log("the finalList length is: " + finalList.Length);
            int randWord = Random.Range(0, tempWords.Count);
            selectableWords.Add(tempWords[randWord]);
            allwords += tempWords[randWord] + "\n";
            tempWords.RemoveAt(randWord);
        }
        File.WriteAllText(Experiment_CoinTask.Instance.sessionDirectory + "allwords.txt", allwords);
        yield return 0;
    }
    /*
        IEnumerator CreateSelectableWordList()
        {
            for(int i=0;i<finalList.Length;i++)
            {
                selectableWords.Add(finalList[i]);
            }

            yield return null;
        }
        */
    void PrintAllWords()
    {
        if (words != null)
        {
            for (int i = 0; i < words.Length; i++)
            {
                UnityEngine.Debug.Log(words[i]);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
