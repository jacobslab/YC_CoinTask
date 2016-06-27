using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class InstructionsController : MonoBehaviour {
	
	public float timePerInstruction;

	public bool isFinished = false;

	//TextMesh _textMesh;
	public Text text; //TODO: rename this!!!
	public Text oculusText;
	public Color textColorDefault;
	public Color textColorOverlay;
	public Image background;
	public Image oculusBackground;
	public Color backgroundColorDefault;



	//CHANGE THESE IN THE EDITOR DEPENDING ON LANGUAGE.
	//UICONTROLLER SETS THESE APPROPRIATELY IN EACH RESPECTIVE GAMEOBJECT.
	//Localization is set up this way to hopefully make it easier to add a new language. It is difficult, however, to add new text...
		//...could have done localization on each individual object, but that would make it more difficult to add new languages.

	//pirate text
	public string pirateWelcomeText = "Welcome to Treasure Hunt!";
	public string[] pirateEncouragingText = {"Ahoy, matey!", "Great work!", "So much treasure!", "Great treasure hunting matey!"};
	public string pirateEndText = "You found all of the treasure!";

	//UI
	public string pauseText = "[(B) to Pause]";
	public string gamePausedText = "✪ GAME PAUSED ✪";

	public string pointsText = "Points";

	//instructions
	public string initialInstructions1 = "Welcome to Treasure Island!" + 
		"\n\nYou are going on a treasure hunt." + 
		"\n\nUse the joystick to control your movement." + 
		"\n\nDrive into treasure chests to open them. Remember where each object is located!" +
		"\n\nPress (X) to continue.";

	public string initialInstructions2 = "When you are asked about remembering an object's location, you must answer [ YES ], [ MAYBE ], or [ NO ], and choose a location." +
		"\n\nYou will win points for correct locations and you will lose points for incorrect locations." +
		"\n\nPress (X) to continue.";

	public string initialInstructions3 = "TIPS FOR MAXIMIZING YOUR SCORE" + 
		"\n\nGet a time bonus by driving to the chests quickly." +
		"\n\nIf you are more than 75% sure, you should select [YES]." +
		"\n\nIf you are at least 50% sure, you should select [MAYBE]." +
		"\n\nOtherwise, you should select [NO]." +
		"\n\nPress (X) to begin!";

	public string yesExplanationInstruction = "[YES] win 200 points / lose 200 points";
	public string maybeExplanationInstruction = "[MAYBE] win 100 points / lose 50 points";
	public string noExplanationInstruction = "[NO] win 50 points / lose 0 points";

	//go & hurry up
	public string GoText = "GO!";
	public string hurryUpText = "hurry up!";

	//box swap
	public string boxSwapBonusTitle = "Box Swap Bonus!";
	public string boxExplanation = "when the boxes stop moving, \npress (X) to select a box";

	//recall
	public string doYouRememberText = "Do you remember where to find the...";
	public string yesAnswerText = "YES!";
	public string maybeAnswerText = "MAYBE";
	public string noAnswerText = "NO...";
	public string yesPointsText = "win a lot / lose a lot";
	public string maybePointsText = "win some / lose some";
	public string noPointsText = "win a little / lose a little";

	public string pressToSelect = "press (X) to select";
	public string selectTheLocationText = "Select the location of the ";

	public string pressToContinue = "press (X) to continue";
	public string pressToStart = "Press (X) to start!";

	//score screen
	public string trialCompleteText = "Trial Complete!";
	public string timeBonusText = "time bonus";
	public string boxSwapBonusText = "box swap bonus";
	public string trialScoreText = "Trial Score";

	//trial 1/40 completed
	public string trial = "trial";
	public string completed = "completed";

	//block screen
	public string blockCompletedText = "You completed a block!";
	//Block 1 Score:
	public string blockUpper = "Block";
	public string score = "Score";
	//for block 1/40 completed
	public string blockLower = "block";


	//finished
	public string youHaveFinishedText = "You have finished your trials! \nPress (X) to proceed.";


	List<string> _instructions;

	// Use this for initialization
	void Start () {
		TurnOffInstructions ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetInstructionsBlank(){
		SetText ("");
	}

	public void TurnOffInstructions(){
		SetInstructionsTransparentOverlay();
		SetInstructionsBlank();
	}

	void SetText(string newText){
		if(ExperimentSettings_CoinTask.isOculus){
			oculusText.text = newText;
		}
		else{
			text.text = newText;
		}
	}

	public void SetInstructionsColorful(){
		//Debug.Log("set instructions dark");
		if(ExperimentSettings_CoinTask.isOculus){
			oculusBackground.color = backgroundColorDefault;
			oculusText.color = textColorDefault;
		}
		else{
			background.color = backgroundColorDefault;
			text.color = textColorDefault;
		}
	}
	
	public void SetInstructionsTransparentOverlay(){
		//Debug.Log("set instructions transparent overlay");
		if(ExperimentSettings_CoinTask.isOculus){
			oculusBackground.color = new Color(0,0,0,0);
			oculusText.color = textColorOverlay;
		}
		else{
			background.color = new Color(0,0,0,0);
			text.color = textColorOverlay;
		}
	}

	public void DisplayText(string line){
		SetText(line);
	}

	/*public void RunInstructions(){
		SetInstructionsColorful();
		isFinished = false;
		Parse ();
		StartCoroutine (DisplayReadInText ());
	}

	string instructionsFilePath = "TextFiles/Instructions.txt";
	public void Parse(){
		_instructions = new List<string> ();

		if (Directory.Exists (instructionsFilePath)) {

			StreamReader reader = new StreamReader (instructionsFilePath);
			string line = reader.ReadLine ();

			string newInstruction = "";

			while (line != null) {
				char[] characters = line.ToCharArray ();
				if (characters.Length > 0) {
					if (characters [0] == '%') { //new instruction
						AddInstruction (newInstruction);
						newInstruction = line;
						newInstruction += '\n';
					} else {
						newInstruction += line;
						newInstruction += '\n';
					}
				} else {
					newInstruction += '\n';
				}

				line = reader.ReadLine ();
			}
			AddInstruction (newInstruction); //add the last instruction
		} 
		else {
			Debug.Log("No path for instructions file!");
		}
	}

	void AddInstruction(string newInstruction){
		if (newInstruction != "" && newInstruction != null) {
			newInstruction = newInstruction.Replace("%" , ""); //if the 'new instructions symbol' is in the line, take it out.
			_instructions.Add (newInstruction);
		}
	}

	IEnumerator DisplayReadInText(){
		for (int i = 0; i < _instructions.Count; i++) {
			SetText(_instructions[i]);
			yield return StartCoroutine(WaitForInstruction());
		}
		isFinished = true;
	}

	IEnumerator WaitForInstruction(){
		float timePassed = 0;

		bool actionButtonUp = false;

		while (timePassed < timePerInstruction) {
			//want to make sure its a new button press
			if(Input.GetAxis("Action Button") == 0.0f){
				actionButtonUp = true;
			}

			//if button pressed after action button was up -- skip the instruction
			if(Input.GetAxis("Action Button") == 1.0f && actionButtonUp){
				timePassed += timePerInstruction; // will skip instruction!
			}

			//otherwise, increment the timePassed
			timePassed += Time.deltaTime;
			yield return 0;
		}
	}*/


}
