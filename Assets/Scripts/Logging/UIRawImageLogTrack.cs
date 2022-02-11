﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIRawImageLogTrack : LogTrack
{

	RawImage myImage;
	Texture currentTexture;
	Color currentColor = Color.black;

	bool firstLog = false; //should make an initial log

	// Use this for initialization
	void Awake()
	{
		myImage = GetComponent<RawImage>();
		currentTexture = myImage.texture;
		currentColor = myImage.color;
	}

	//log on late update so that everything for that frame gets set first
	void LateUpdate()
	{
		if (myImage == null)
		{
			Debug.Log("UI RawImage not found!");
		}
		if (!firstLog)
		{
			firstLog = true;
			LogImage();
			LogColor();
		}
		if (ExperimentSettings_CoinTask.isLogging && (currentTexture != myImage.texture))
		{ //if the text has changed, log it!
			LogImage();
		}
		if (ExperimentSettings_CoinTask.isLogging && (currentColor != myImage.color))
		{ //if the text has changed, log it!
			LogColor();
		}
	}

	void LogImage()
	{
		if (myImage.texture != null)
		{
			currentTexture = myImage.texture;
			subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "IMAGE" + separator + currentTexture.name.ToString());
		}
		else
		{
			subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "IMAGE" + separator + "None");
		}
	}



	void LogColor()
	{

		currentColor = myImage.color;
		subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "IMAGE_COLOR" + separator + currentColor.r + separator + currentColor.g + separator + currentColor.b + separator + currentColor.a);
	}
}