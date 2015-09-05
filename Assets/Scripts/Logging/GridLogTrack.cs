using UnityEngine;
using System.Collections;

public class GridLogTrack : LogTrack {

	public enum LoggedTileType{
		currentSelectedTile,
		chosenTestTile,
		correctTestTile
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void LogGridTile(Tile gridTile, LoggedTileType tileType){
		if (ExperimentSettings_CoinTask.isLogging) {
			string tileTypeString = "TILE_SELECTED";
			if(tileType == LoggedTileType.chosenTestTile){
				tileTypeString = "TEST_TILE_CHOSEN";
			}
			else if(tileType == LoggedTileType.correctTestTile){
				tileTypeString = "CORRECT_TEST_TILE";
			}

			//log tile type, tile indices, grid tile position
			string tileIndicesLogString = tileTypeString + separator + "ROW" + separator + gridTile.GridIndices.x + separator + "COLUMN" + separator + gridTile.GridIndices.y;
			string tilePositionLogString = tileTypeString + separator + "POSITION" + separator + gridTile.transform.position.x + separator + gridTile.transform.position.y + separator + gridTile.transform.position.z;
			//log the indices
			subjectLog.Log( exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), tileIndicesLogString);
			//log the position
			subjectLog.Log( exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), tilePositionLogString);
		}
	}
}
