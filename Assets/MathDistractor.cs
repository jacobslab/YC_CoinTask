using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MathDistractor : MonoBehaviour
{
    private CanvasGroup distractorGroup;
    private int firstRandInt = 0;
    private int secondRandInt = 0;
    private int correctAnswer = 0;
    private List<int> numbers;
    public Text distractorText;
    public Text answerText;
    private int answerInt = 0;
    private bool mathAnswered = false;

    public Color correctFeedback;
    public Color incorrectFeedback;

    void Awake()
    {
        distractorGroup = gameObject.GetComponent<CanvasGroup>();
        GenerateNumbersList();
    }

    private void Start()
    {

        distractorGroup.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            GenerateNewMathProblem();
        }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                CreateAnswer(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CreateAnswer(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CreateAnswer(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                CreateAnswer(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                CreateAnswer(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                CreateAnswer(5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                CreateAnswer(6);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                CreateAnswer(7);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                CreateAnswer(8);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                CreateAnswer(9);
            }
  
    }

    private void GenerateNumbersList()
    {
        numbers = new List<int>();
        for (int i = 1; i < 10; i++)
        {
            numbers.Add(i);
        }
    }

    void CreateAnswer(int pressedInt)
    {
        answerInt = pressedInt;
        answerText.text = answerInt.ToString();
        mathAnswered = true;
    }

    //generating a new math problem
    void GenerateNewMathProblem()
    {
        //reset the answer text and answered bool
        answerText.text = "";
        answerInt = 0;
        mathAnswered = false;

        int firstRandIndex = Random.Range(0, numbers.Count - 1);
        firstRandInt = numbers[firstRandIndex];
        //Debug.Log ("removing " + firstRandInt + " at index " + firstRandIndex);
        int upperLimit = 9 - firstRandInt;
        //Debug.Log ("upper limit: " + upperLimit);
        int secondRandIndex = Random.Range(0, upperLimit); //we need to make sure that the second number is at max 7

        if (firstRandIndex == secondRandIndex)
        {
            if (firstRandIndex == 0)
            {
                secondRandIndex = 1;
            }
            else if (firstRandIndex == numbers.Count - 1)
            {
                secondRandIndex = numbers.Count - 2;
            }
            else
                secondRandIndex = firstRandIndex + 1;
        }
        secondRandInt = numbers[secondRandIndex];
        Debug.Log("first index: " + firstRandIndex + " and second: " + secondRandIndex);
        correctAnswer = firstRandInt + secondRandInt;

        distractorText.text = firstRandInt.ToString() + " + " + secondRandInt.ToString() + " = ";
        UnityEngine.Debug.Log("correct answer is: " + correctAnswer.ToString());

    }

    IEnumerator GiveMathFeedback()
    {
        if(answerInt == correctAnswer)
        {
            Debug.Log("correct");
            SetDistractorTextColor(correctFeedback);
        }
        else
        {
            Debug.Log("incorrect");
            SetDistractorTextColor(incorrectFeedback);

        }
        //show feedback for a second

        Debug.Log("waiting for a second");
        yield return new WaitForSeconds(1f);


        //reset to normal text color
        SetDistractorTextColor(Color.white);

        yield return null;
    }

    void SetDistractorTextColor(Color feedbackColor)
    {
        answerText.color = feedbackColor;
        distractorText.color = feedbackColor;
    }
    public IEnumerator RunMathDistractor()
    {
        distractorGroup.alpha = 1f;
        //GenerateNewMathProblem();
        float timer = 0f;
        while(timer < 20f)
        {

            GenerateNewMathProblem();
            while(!mathAnswered && timer < 20f)
            {
                timer += Time.deltaTime;
                yield return 0;
            }
            if (mathAnswered)
            {
                yield return StartCoroutine(GiveMathFeedback());
                timer += 1f; //add a second lost during the feedback coroutine
            }
            yield return 0;
        }
        Debug.Log("turning off distractor group");
        distractorGroup.alpha = 0f;

        yield return null;
    }
}
