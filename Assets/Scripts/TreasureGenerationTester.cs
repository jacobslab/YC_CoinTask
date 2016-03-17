using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class TreasureGenerationTester : MonoBehaviour {
	string treasureLogPath = "TextFiles/GeneratedTreasure.txt";

	StreamWriter streamWriter;

	int numTrials = 1000;
	int numDefObjects = 4;

	string separator = "\t";

	void Awake(){
		streamWriter = new StreamWriter(treasureLogPath, false);

		List<Vector2> objPositions;

		for(int i = 0; i < numTrials; i++){
			objPositions = Experiment_CoinTask.Instance.objectController.GenerateOrderedDefaultObjectPositions(numDefObjects, Experiment_CoinTask.Instance.player.controls.startPositionTransform1.position);

			Write (GameClock.SystemTime_Milliseconds, 0, "NUM_TRIAL" + separator + i);

			for(int j = 0; j < objPositions.Count; j++){
				Vector2 currPos = objPositions[j];
				Write(GameClock.SystemTime_Milliseconds, 0, "TREASURE_" + j + separator + "POSITION_XZ" + separator + currPos.x + separator + 0 + separator + currPos.y); //VECTOR 2
			}
		}

		streamWriter.Flush();
		streamWriter.Close();
	}

	void Write(long systemTime, long frame, string message){
		streamWriter.WriteLine( systemTime.ToString() + separator + frame + separator + message);
	}
}
