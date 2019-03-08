using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RabbitCatcher : MonoBehaviour
{

    public GameObject rabbitObj;
    //public Text debugText;
    //public CanvasGroup distractorPanel;
    //public Text distractorText;
    //public GameObject correctParticlePrefab;
    //public GameObject wrongParticlePrefab;

    //public GameObject foundCube; //the invisible cube that will serve as a collision check for the rabbit

    public static bool rabbitLooking = false; //used to control if the device camera is looking at the rabbit or not; this is to make sure the rabbit is centrally viewed before it begins its movement

    private bool caughtRabbit = false; // flag that determines whether rabbit was caught or not

    private string catchInstructions = "Catch the rabbit by going near its location!";
    private string successInstructions = "Success! You caught the rabbit!";
    private string failureInstructions = "The rabbit escaped! Better luck next time!";
    Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

    // Use this for initialization
    void Start()
    {
        rabbitObj.SetActive(false);
        //foundCube.SetActive(false);
        Debug.Log("rabbit tag is " + rabbitObj.tag);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AutoCatchRabbit()
    {
        rabbitLooking = true;
        MarkRabbitCaught();
    }

    public void MarkRabbitCaught()
    {
        caughtRabbit = true;
    }

    public void ResetRabbitFlag()
    {
        caughtRabbit = false;
    }

    public IEnumerator Run()
    {

        //debugText.enabled = true;
        //make the rabbit visible
        rabbitObj.SetActive(true);
        //foundCube.SetActive(true);

        exp.currInstructions.headerInstructionGroup.alpha = 1f;
        exp.currInstructions.headerInstructionText.text = catchInstructions;

        Debug.Log("put rabbit on a random spawn pos");

        //find a random position that is reasonable distance away from the rabbit
        Vector3 targetPos = Vector3.zero;
        targetPos = exp.environmentController.GetRandomPositionWithinWallsXZ(Config_CoinTask.objectToWallBuffer);
        Debug.Log("finding targetpos for the rabbit");
        while (Vector3.Distance(exp.player.gameObject.transform.position, rabbitObj.transform.position) < Config_CoinTask.minRabbitSpawnDistance)
        {
            targetPos = exp.environmentController.GetRandomPositionWithinWallsXZ(Config_CoinTask.objectToWallBuffer);
            yield return 0;
        }
        //wait for the rabbit to move




        //lerp rabbit to the targetpos

        float moveTimer = 0f;
        //float maxWaitFactor = 4f;
        float smoothTime = 4f;
        float xVelocity = 0.0f;
        float zVelocity = 0.0f;

        //first make sure the rabbit is looking at the targetpos
        rabbitObj.transform.LookAt(targetPos);
        rabbitObj.transform.localEulerAngles = new Vector3(0f, rabbitObj.transform.localEulerAngles.y, 0f); //reset x and z axes angles

        Debug.Log("The target pos is " + targetPos.ToString());
        Debug.Log("waiting for rabbit to be looked at");
        yield return StartCoroutine(WaitTillRabbitLooked());

        Debug.Log("setting rabbit anim to move");
        rabbitObj.GetComponent<Animator>().SetBool("CanMove?", true);
        Debug.Log("moving the rabbit");
        while (moveTimer < smoothTime)
        {
            moveTimer += Time.deltaTime;
            //float lerpFactor = moveTimer / maxWaitFactor;
            //debugText.text = rabbitObj.transform.localPosition.ToString() + " with move timer " + moveTimer.ToString();
            float newPositionX = Mathf.SmoothDamp(rabbitObj.transform.position.x, targetPos.x, ref xVelocity, smoothTime);
            float newPositionZ = Mathf.SmoothDamp(rabbitObj.transform.position.z, targetPos.z, ref zVelocity, smoothTime);
            rabbitObj.transform.position = new Vector3(newPositionX, rabbitObj.transform.position.y, newPositionZ);

            //rabbitObj.transform.localPosition = Vector3.Lerp(rabbitObj.transform.localPosition, targetPos, lerpFactor);
            yield return 0;
        }
        rabbitObj.GetComponent<Animator>().SetBool("CanMove?", false);
        rabbitObj.transform.parent = null;

        //unlock player movement
        exp.player.controls.ShouldLockControls = false;

        //reset the rabbit caught flag
        ResetRabbitFlag();

            //then wait for the player to come closer to the rabbit
            float distance = 10f;
            float durationTimer = 0f;
        while (distance > Config_CoinTask.minRabbitCatchDistance && durationTimer < Config_CoinTask.maxRabbitCatchTime)
            {
                Debug.Log("waiting for the rabbit to be caught");
                durationTimer += Time.deltaTime;


                distance = Vector3.Distance(rabbitObj.transform.position, exp.player.controls.TiltableTransform.position);
                Debug.Log("distance is " + distance.ToString());
                yield return 0;
            }

        if (distance <= Config_CoinTask.minRabbitCatchDistance)
            {
                MarkRabbitCaught();
            }


        ////tapping on the rabbit will suffice
        //else
        //{
        //    yield return StartCoroutine(TreasureHuntController_ARKit.Instance.WaitTillObjectHit(rabbitObj, 8f));

        //}

        //set the rabbit inactive regardless of the result
        rabbitObj.SetActive(false);
        //briefly lock controls
        exp.player.controls.ShouldLockControls = true;

        //TreasureHuntController_ARKit.Instance.trialLog.LogDistractorResult(caughtRabbit);
        //show success or failure message
        if (caughtRabbit)
        {
            exp.currInstructions.headerInstructionText.text = successInstructions;
             yield return new WaitForSeconds(2.5f);
        }
        else
        {
            exp.currInstructions.headerInstructionText.text = failureInstructions;
            yield return new WaitForSeconds(2.5f);
        }

        //reset rabbit looking var
        rabbitLooking = false;
        //foundCube.SetActive(false);
        exp.currInstructions.headerInstructionText.text = "";
        exp.currInstructions.headerInstructionGroup.alpha = 0f;

        yield return null;
    }

    IEnumerator WaitTillRabbitLooked()
    {
        yield return null;
    }
}