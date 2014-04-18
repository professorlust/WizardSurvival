﻿using UnityEngine;
using System.Collections;

public class DoorRight : MonoBehaviour {
	
	private int door_LastIndex = 0;
	private int count = 0;
	
	public void playDoorAnim () {
		if (door_LastIndex == 0) {
			gameObject.animation.Play ("DoorRightOpen");
			door_LastIndex = 1;
		} else {
			gameObject.animation.Play ("DoorRightClose");
			door_LastIndex = 2;
		}
	}
	
	void Update () {
		if (door_LastIndex == 0) {
			count = PhotonNetwork.playerList.Length;
			for (int i = 0; i < PhotonNetwork.playerList.Length; ++i) {
				string p = (string) PhotonNetwork.playerList[i].customProperties["Ability"];
				if (p != null && p != "none") {
					--count;
				}
			}
			
			if (count == 0) {
				playDoorAnim ();
			}
		} else if (door_LastIndex == 1) {
			OneWayWall onewaywall = (OneWayWall) GameObject.Find("OneWayWallThrone").GetComponent(typeof(OneWayWall));
			onewaywall.closeDoor = true;

		}
		else if ( door_LastIndex == 2 )
		{
			GameObject mainCam = GameObject.FindWithTag("MainCamera") as GameObject;
			mainCam.GetComponent<GameController>().Begin();
		}
	}
}
