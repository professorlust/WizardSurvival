﻿using UnityEngine;
using System.Collections;

public class ItemSpawnerOnce : MonoBehaviour {
	
	public GameObject ItemPrefab;
	public int ItemQuantity;
	
	bool attemptedSpawn = false;

	//When the player connects, if the player is master spawn item.
	//ELSE, never spawn item + Destroy self.
	void Update()
	{
		if(!attemptedSpawn && GLOBAL.inRoom)
		{
			attemptedSpawn = true;
			if(PhotonNetwork.isMasterClient)
				spawn();
			else
				Destroy(gameObject);
		}
	}
	
	void spawn()
	{
		GameObject item = PhotonNetwork.InstantiateSceneObject(ItemPrefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0, null) as GameObject;
		
		item.transform.parent = transform;
		item.transform.localPosition = new Vector3(0, 0, 0);
		
		//If the spawned item is a collectable, give it a quantity
		if(item.GetComponent<CollectableBase>() != null)
			item.GetComponent<CollectableBase>().setQuantity(ItemQuantity);
	}
}
