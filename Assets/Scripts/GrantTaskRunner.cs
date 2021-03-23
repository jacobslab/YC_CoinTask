using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrantTaskRunner : MonoBehaviour
{

    Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }
    public Transform startA;
    public Transform retrievalA;
    public Transform transitionDoorA;
    public Transform tunnelDoorA;
    public Transform tunnelDoorB;
    public Transform transitionDoorB;
    public Transform startB;
    public Transform retrievalB;
    
    //object spawn locations
    public Transform spawnA_1;
    public Transform spawnA_2;
    public Transform spawnA_3;
    public Transform spawnA_4;
    public Transform spawnB_1;
    public Transform spawnB_2;
    public Transform spawnB_3;
    public Transform spawnB_4;

    //fixed objects to be spawned
    public GameObject applePrefab;
    public GameObject crownPrefab;
    public GameObject barrelPrefab;
    public GameObject refrigeratorPrefab;

    public GameObject lampPrefab;
    public GameObject basketballPrefab;
    public GameObject fanPrefab;
    public GameObject vasePrefab;

    public GameObject chestPrefab;

    public GameObject fenceToDoorA;
    public GameObject fenceToDoorB;

    //coin shower
    public GameObject coinShowerPrefab;


    public GameObject doorA;
    public GameObject doorB;

    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("BeginTask");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator BeginTask()
    {
        exp.player.controls.ShouldLockControls = true;
        UnityEngine.Debug.Log("smooth moving to start A");
        yield return StartCoroutine(player.controls.SmoothMoveTo(null, startA.position, startA.rotation, true));
        //exp.uiController.goText.Play();
       
        exp.player.controls.ShouldLockControls = false;
        UnityEngine.Debug.Log("unlocked controls");

        GameObject objA = null;
        GameObject objB = null;

           GameObject specialObject_A = Instantiate(applePrefab, spawnA_1.position, Quaternion.identity) as GameObject;
        //GameObject specialObject_A = Instantiate(basketballPrefab, spawnA_1.position, Quaternion.identity) as GameObject;

        specialObject_A.transform.localScale = new Vector3(30f, 30f, 30f);

        //   specialObject_A.transform.localScale = new Vector3(30f, 30f, 30f);

        GameObject specialObject_B = Instantiate(crownPrefab, spawnA_2.position, Quaternion.identity) as GameObject;

        specialObject_B.transform.localScale = new Vector3(5f, 5f, 5f);
        exp.uiController.recencyCanvasGroup.alpha = 1f;
        while (!Input.GetKeyDown(KeyCode.X))
        {
                yield return 0;
        }
        exp.uiController.recencyCanvasGroup.alpha = 0f;

       // GameObject coinShower = Instantiate(coinShowerPrefab, player.transform.position + (player.transform.forward * 5f), Quaternion.identity) as GameObject;
        exp.uiController.feedbackRecencyPanel.alpha = 1f;
        yield return new WaitForSeconds(1f);
        exp.uiController.feedbackRecencyPanel.alpha = 0f;
       // Destroy(coinShower);
        Destroy(specialObject_A);
        Destroy(specialObject_B);


        UnityEngine.Debug.Log("smooth moving to retrieval position");
        yield return StartCoroutine(player.controls.SmoothMoveTo(null, transitionDoorA.position, transitionDoorA.rotation, true));


        ///TODO///
        //make sure they're oriented towards the tunnel
        //remove the two fences blocking the door
        fenceToDoorA.SetActive(false);
        exp.uiController.ShowDoorInstruction(true);
        //wait for X to open the door
        while (!Input.GetKeyDown(KeyCode.X))
        {
            yield return 0;
        }
        doorA.GetComponent<Animation>().clip = doorA.GetComponent<Animation>().GetClip("Opening");
        doorA.GetComponent<Animation>().Play();
        exp.uiController.ShowDoorInstruction(false);
        exp.uiController.travellingBetween.alpha = 1f;
        yield return new WaitForSeconds(0.5f);
        //open the door via animation and move them towards the next door
        yield return StartCoroutine(player.controls.SmoothMoveTo(null, tunnelDoorB.position, tunnelDoorB.rotation, true));
        // stop there and wait for them to open door
        exp.uiController.travellingBetween.alpha = 0f;
        exp.uiController.ShowDoorInstruction(true);
        //wait for X to open the door
        while (!Input.GetKeyDown(KeyCode.X))
        {
            yield return 0;
        }
        doorB.GetComponent<Animation>().clip = doorB.GetComponent<Animation>().GetClip("Opening");
        doorB.GetComponent<Animation>().Play();
        exp.uiController.ShowDoorInstruction(false);
        yield return new WaitForSeconds(0.5f);


        yield return StartCoroutine(player.controls.SmoothMoveTo(null, startB.position, startB.rotation, true));
        exp.player.controls.ShouldLockControls = false;
        //reset everything back to normal
        fenceToDoorA.SetActive(true);

        doorA.GetComponent<Animation>().clip = doorA.GetComponent<Animation>().GetClip("Closing");
        doorA.GetComponent<Animation>().Play();

        doorB.GetComponent<Animation>().clip = doorB.GetComponent<Animation>().GetClip("Closing");
        doorB.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.5f);

        //repeat same trial structure but with Env B transforms

        specialObject_A = Instantiate(lampPrefab, spawnB_1.position, Quaternion.identity) as GameObject;

        specialObject_A.transform.localScale = new Vector3(30f, 30f, 30f);
        specialObject_B = Instantiate(basketballPrefab, spawnB_2.position, Quaternion.identity) as GameObject;
        specialObject_B.transform.localScale = new Vector3(5f, 5f, 5f);
        exp.uiController.recencyCanvasGroup.alpha = 1f;
        while(!Input.GetKeyDown(KeyCode.X))
        {
            yield return 0;
        }
        exp.uiController.recencyCanvasGroup.alpha = 0f;
       // coinShower = Instantiate(coinShowerPrefab, player.transform.position + (player.transform.forward * 5f), Quaternion.identity) as GameObject;
        exp.uiController.feedbackRecencyPanel.alpha = 1f;
        yield return new WaitForSeconds(1f);
        exp.uiController.feedbackRecencyPanel.alpha = 0f;
      //  Destroy(coinShower);
        Destroy(specialObject_A);
        Destroy(specialObject_B);

        //MOVING from B to A
        yield return StartCoroutine(player.controls.SmoothMoveTo(null,transitionDoorB.position, transitionDoorB.rotation, true));


        ///TODO///
        //make sure they're oriented towards the tunnel
        //remove the two fences blocking the door
        fenceToDoorB.SetActive(false);
        exp.uiController.ShowDoorInstruction(true);
        //wait for X to open the door
        while (!Input.GetKeyDown(KeyCode.X))
        {
            yield return 0;
        }
        doorB.GetComponent<Animation>().clip = doorB.GetComponent<Animation>().GetClip("Opening");
        doorB.GetComponent<Animation>().Play();
        exp.uiController.ShowDoorInstruction(false);

        exp.uiController.travellingBetween.alpha = 1f;
        yield return new WaitForSeconds(0.5f);
        //open the door via animation and move them towards the next door
        yield return StartCoroutine(player.controls.SmoothMoveTo(null, tunnelDoorA.position, tunnelDoorA.rotation, true));
        // stop there and wait for them to open door

        exp.uiController.travellingBetween.alpha = 0f;
        exp.uiController.ShowDoorInstruction(true);
        //wait for X to open the door
        while (!Input.GetKeyDown(KeyCode.X))
        {
            yield return 0;
        }
        doorA.GetComponent<Animation>().clip = doorA.GetComponent<Animation>().GetClip("Opening");
        doorA.GetComponent<Animation>().Play();
        exp.uiController.ShowDoorInstruction(false);
        yield return new WaitForSeconds(0.5f);
        //repeat same trial structure but with Env B transforms


        yield return StartCoroutine(player.controls.SmoothMoveTo(null, startA.position, startA.rotation, true));
        exp.player.controls.ShouldLockControls = false;



        fenceToDoorB.SetActive(true);

        doorB.GetComponent<Animation>().clip = doorB.GetComponent<Animation>().GetClip("Closing");
        doorB.GetComponent<Animation>().Play();

        doorA.GetComponent<Animation>().clip = doorA.GetComponent<Animation>().GetClip("Closing");
        doorA.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.5f);
        //SECOND trial in Env A

        exp.player.controls.ShouldLockControls = true;
        UnityEngine.Debug.Log("smooth moving to start A");
        yield return StartCoroutine(player.controls.SmoothMoveTo(null, startA.position, startA.rotation, true));
        //exp.uiController.goText.Play();

        exp.player.controls.ShouldLockControls = false;
        UnityEngine.Debug.Log("unlocked controls");

        
        specialObject_A = Instantiate(applePrefab, spawnA_4.position, Quaternion.identity) as GameObject;

        specialObject_A.transform.localScale = new Vector3(30f, 30f, 30f);


        specialObject_B = Instantiate(basketballPrefab, spawnA_3.position, Quaternion.identity) as GameObject;

        specialObject_B.transform.localScale = new Vector3(5f, 5f, 5f);
        exp.uiController.recencyCanvasGroup.alpha = 1f;
        while (!Input.GetKeyDown(KeyCode.X))
        {
            yield return 0;
        }
        exp.uiController.recencyCanvasGroup.alpha = 0f;
     //   coinShower = Instantiate(coinShowerPrefab, player.transform.position + (player.transform.forward * 5f), Quaternion.identity) as GameObject;
        exp.uiController.feedbackRecencyPanel.alpha = 1f;
        yield return new WaitForSeconds(1f);
        exp.uiController.feedbackRecencyPanel.alpha = 0f;
       // Destroy(coinShower);

        Destroy(specialObject_A);
        Destroy(specialObject_B);
        
        yield return StartCoroutine(player.controls.SmoothMoveTo(null, transitionDoorA.position, transitionDoorA.rotation, true));


        ///TODO///
        //make sure they're oriented towards the tunnel
        //remove the two fences blocking the door
        fenceToDoorA.SetActive(false);
        exp.uiController.ShowDoorInstruction(true);
        //wait for X to open the door
        while (!Input.GetKeyDown(KeyCode.X))
        {
            yield return 0;
        }
        doorA.GetComponent<Animation>().clip = doorA.GetComponent<Animation>().GetClip("Opening");
        doorA.GetComponent<Animation>().Play();
        exp.uiController.ShowDoorInstruction(false);

        exp.uiController.travellingBetween.alpha = 1f;
        yield return new WaitForSeconds(0.5f);
        //open the door via animation and move them towards the next door
        yield return StartCoroutine(player.controls.SmoothMoveTo(null, tunnelDoorB.position, tunnelDoorB.rotation, true));
        // stop there and wait for them to open door

        exp.uiController.travellingBetween.alpha = 0f;

        exp.uiController.ShowDoorInstruction(true);


        //wait for X to open the door
        while (!Input.GetKeyDown(KeyCode.X))
        {
            yield return 0;
        }
        doorB.GetComponent<Animation>().clip = doorB.GetComponent<Animation>().GetClip("Opening");
        doorB.GetComponent<Animation>().Play();
        exp.uiController.ShowDoorInstruction(false);
        yield return new WaitForSeconds(0.5f);


        //reset everything back to normal
        fenceToDoorA.SetActive(true);

        doorA.GetComponent<Animation>().clip = doorA.GetComponent<Animation>().GetClip("Closed");
        doorA.GetComponent<Animation>().Play();

        doorA.GetComponent<Animation>().clip = doorA.GetComponent<Animation>().GetClip("Closed");
        doorA.GetComponent<Animation>().Play();

        //SECOND trial in Env B
        exp.player.controls.ShouldLockControls = true;
        yield return StartCoroutine(player.controls.SmoothMoveTo(null, startB.position, startB.rotation, true));
        exp.player.controls.ShouldLockControls = false;
        specialObject_A = Instantiate(basketballPrefab, spawnB_3.position, Quaternion.identity) as GameObject;

        specialObject_A.transform.localScale = new Vector3(5f, 5f, 5f);


        specialObject_B = Instantiate(vasePrefab, spawnB_4.position, Quaternion.identity) as GameObject;
        specialObject_B.transform.localScale = new Vector3(1f, 1f, 1f);
        exp.uiController.recencyCanvasGroup.alpha = 1f;
        while (!Input.GetKeyDown(KeyCode.X))
        {
            yield return 0;
        }
        Destroy(specialObject_A);
        Destroy(specialObject_B);
        exp.uiController.recencyCanvasGroup.alpha = 0f;



        yield return null;
    }

}
