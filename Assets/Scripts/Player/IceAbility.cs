﻿using UnityEngine;
using System.Collections;

public class IceAbility : AbilityBase {

	const int SPEED = 20;
	const int MAX_LIFE = 90;

	int delayTimer = 60;
	int delayTimerInit = 60;
	int cooldownResolution = 60;		// assumping 60fps here (could use Time.fixedTime)

	void FixedUpdate(){
		++delayTimer;
	}

	void OnGUI() {
		if(delayTimer < delayTimerInit){
			EZGUI.init();
			
			EZOpt e = new EZOpt(Color.red, new Color(0.1f, 0.1f, 0.1f));
			e.dropShadowX = e.dropShadowY = 1;
			e.font = GLOBAL.spookyMagic;
			
			EZGUI.placeTxt("cooldown " + (delayTimerInit/cooldownResolution - (delayTimer / cooldownResolution)) + "s", 25, EZGUI.FULLW - 170, 130, e);
			EZGUI.drawBox(EZGUI.FULLW - 225, 130, 200 * ((float)(delayTimerInit - delayTimer) / (float)delayTimerInit), 20, new Color(1f, 0f, 0f, 1f));
			//(float x, float y, float w, float h, Color c){
			//
		}
	}

	public override void fire(){
		GameAudio.playFreezeSpell();

		print(delayTimer);
		if(delayTimer < delayTimerInit) {
			GameAudio.playMagicFail();
			return;
		}
		
		delayTimer = 0;

		Transform w = GLOBAL.myWizard.transform;

		GameObject ice = PhotonNetwork.Instantiate(
			"IcePrefab",
			w.position + w.forward.normalized * 2,// + new Vector3(0, 0, 0.3f),
			gameObject.transform.rotation,
			0
		) as GameObject;

		ice.GetComponent<ProjectileBase>().wizard = GLOBAL.myWizard;
		ice.rigidbody.velocity = GLOBAL.MainCamera.transform.forward * SPEED;
		ice.transform.parent = w.parent;
	}

	public override string getAbilityName() {
		return "Ice Blast";
	}

	public override string getAbilityDescription() {
		return "Burrrrr...";
	}
}
