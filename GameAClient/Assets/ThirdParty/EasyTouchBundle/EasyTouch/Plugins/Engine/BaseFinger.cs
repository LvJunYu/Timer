/***********************************************
				EasyTouch V
	Copyright © 2014-2015 The Hedgehog Team
    http://www.thehedgehogteam.com/Forum/
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace HedgehogTeam.EasyTouch{
public class BaseFinger{

	public int fingerIndex;	
	public int touchCount;
	public Vector2 startPosition;
	public Vector2 position;
	public Vector2 deltaPosition;	
	public float actionTime;
	public float deltaTime;		
	
	public Camera pickedCamera;
	public GameObject pickedObject;
	public bool isGuiCamera;
		
	public bool isOverGui;
	public GameObject pickedUIElement;


	public float altitudeAngle;
	public float azimuthAngle;
	public float maximumPossiblePressure;
	public float pressure;

	public float radius;
	public float radiusVariance;
	public TouchType touchType;

	

	public Gesture GetGesture(){

		Gesture gesture = new Gesture();
		gesture.fingerIndex = fingerIndex;
		gesture.touchCount = touchCount;
		gesture.startPosition = startPosition;
		gesture.position = position;
		gesture.deltaPosition = deltaPosition;
		gesture.actionTime = actionTime;
		gesture.deltaTime = deltaTime;
		gesture.isOverGui = isOverGui;

		gesture.pickedCamera = pickedCamera;
		gesture.pickedObject = pickedObject;
		gesture.isGuiCamera = isGuiCamera;

		gesture.pickedUIElement = pickedUIElement;

		gesture.altitudeAngle = altitudeAngle;
		gesture.azimuthAngle = azimuthAngle;
		gesture.maximumPossiblePressure = maximumPossiblePressure;
		gesture.pressure = pressure;
		gesture.radius = radius;
		gesture.radiusVariance = radiusVariance;
		gesture.touchType = touchType;

		return gesture;
	}

	public override string ToString()
	{
		return string.Format(
			"FingerIndex: {0}, TouchCount: {1}, StartPosition: {2}, Position: {3}, DeltaPosition: {4}, " +
			"ActionTime: {5}, DeltaTime: {6}, PickedCamera: {7}, PickedObject: {8}, IsGuiCamera: {9}, " +
			"IsOverGui: {10}, PickedUIElement: {11}, AltitudeAngle: {12}, AzimuthAngle: {13}, " +
			"MaximumPossiblePressure: {14}, Pressure: {15}, Radius: {16}, RadiusVariance: {17}, TouchType: {18}",
			fingerIndex, touchCount, startPosition, position, deltaPosition, actionTime, deltaTime, pickedCamera, pickedObject,
			isGuiCamera, isOverGui, pickedUIElement, altitudeAngle, azimuthAngle, maximumPossiblePressure, pressure, radius,
			radiusVariance, touchType);
	}
}
}