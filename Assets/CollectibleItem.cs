using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour {

    Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }


    public GameObject renderParent;

    private AudioSource collectibleCollisionSound;

    bool isExecutingPlayerCollision = false;

    void Awake()
    {
        GetComponent<VisibilityToggler>().TurnRendering(false);
        collectibleCollisionSound = gameObject.GetComponent<AudioSource>();
        InitTreasureState();
    }

    void InitTreasureState()
    {
        switch (exp.trialController.NumDefaultObjectsCollected)
        {
            case 0:
                TCPServer.Instance.SetState(TCP_Config.DefineStates.TREASURE_1, true);
                break;
            case 1:
                TCPServer.Instance.SetState(TCP_Config.DefineStates.TREASURE_2, true);
                break;
            case 2:
                TCPServer.Instance.SetState(TCP_Config.DefineStates.TREASURE_3, true);
                break;
            case 3:
                TCPServer.Instance.SetState(TCP_Config.DefineStates.TREASURE_4, true);
                break;
        }
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && (tag == "DefaultObject" || tag == "DefaultSpecialObject") && !isExecutingPlayerCollision)
        {
            if (!ExperimentSettings_CoinTask.isOculus)
            {
                GetComponent<TreasureChestLogTrack>().LogPlayerChestCollision();
                isExecutingPlayerCollision = true;
                StartCoroutine(RunCollision());
            }
          
        }
    }

    IEnumerator WaitForObjectLookAt()
    {
        yield return StartCoroutine(Experiment_CoinTask.Instance.trialController.WaitForPlayerToLookAt(gameObject));
        //open the object
        //StartCoroutine(Open(Experiment_CoinTask.Instance.player.gameObject));


        yield return StartCoroutine(RunDefaultCollision());
      
    }


    IEnumerator RunCollision()
    { 
        //yield return StartCoroutine(Experiment_CoinTask.Instance.trialController.WaitForPlayerRotationToTreasure(gameObject));
        yield return StartCoroutine(RunDefaultCollision());
    }

    IEnumerator RunDefaultCollision()
    {
        //shouldDie = true;

        //PlayJuice(false);
        Debug.Log("treasure collected");
        exp.trialController.chestCollided = true;

        yield return StartCoroutine(Experiment_CoinTask.Instance.trialController.WaitForTreasurePause(null));



        //unlock the avatar controls
        //Experiment_CoinTask.Instance.player.controls.ShouldLockControls = false;


        //Debug.Log("INCREMENT CHEST COUNT");
        //Experiment_CoinTask.Instance.trialController.IncrementNumDefaultObjectsCollected();


        //turn the gameobject inactive
        gameObject.SetActive(false);

        //we won't destroy collectible objects yet; we will only destroy them after the feedback phase
        //Destroy(gameObject); 
        yield return null;
    }

}
