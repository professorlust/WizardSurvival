﻿using UnityEngine;
using System.Collections;
using MGSpawn.Spawn;

using RAIN.Core;
using RAIN.Action;

public class MGSpawner : MonoBehaviour {
	
	public SpawnMemberType unitLevel = SpawnMemberType.Easy;
	public GameObject[] unitList = new GameObject[3];
	
	public int totalUnits = 10;
	private int numberOfUnits = 0;
	private int totalSpawnedUnits = 0;

	public int spawnID = 0;

	private bool waveSpawn = true;
	public bool spawn = true;

	private bool startedSpawn = false;

	public SpawnModes spawnType = SpawnModes.Continuous;

	//WAVE
	public float waveTimer = 30.0f;
	private float lastWaveSpawnTime = 0.0f;
	public int totalWaves = 5;
	private int numWaves = 0;

	private bool checkEnemyLevel = false;

	public float timeBetweenSpawns = 0.5f;

	//Location of Spawner
	void Start () {
		StartCoroutine("OpeningDelay");
		//StartSpawn();
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawIcon(this.transform.position, "SpawnIcon.jpeg");
	}

	//for continous waves 
	public void RemoveUnit()
	{
		numberOfUnits--;
	}

	//for continous waves (use if multiple spawners)
	public void RemoveUnit(int sID)
	{
		// if the unit's spawnID is equal to this spawnersID then remove an unit count
		if (spawnID == sID)
		{
			numberOfUnits--;
		}
	}

	private void SpawnUnit()
	{
		if (unitList[(int)unitLevel] != null && PhotonNetwork.isMasterClient)
		{
			GameObject unit = (GameObject) PhotonNetwork.Instantiate("EnemyWithAI 1", this.transform.position, Quaternion.identity,0) as GameObject;
			unit.transform.FindChild("skeletonNormal").GetComponent<MGAISuperClass>().SetOwner(this);
			//print("Spawn");
			//print(PhotonNetwork.playerList[0].ID);

			//GameObject tempPlayer = GameObject.FindGameObjectWithTag("Player");
			//set the player to follow
			//unit.transform.FindChild("AI").GetComponent<AIRig>().AI.WorkingMemory.SetItem("detectObject2", tempPlayer);

			numberOfUnits++;
			totalSpawnedUnits++;
		}
		else
		{
			Debug.LogError("Error trying to spawn unit of level " + unitLevel.ToString() + " on spawner " + spawnID + " - No unit set");
			spawn = false;
		}
	}

	private IEnumerator DoSpawn()
	{
		while (spawn)
		{
			switch (spawnType)
			{

				//Always have a certain number in the scene
			case SpawnModes.Continuous:
				if (numberOfUnits < totalUnits)
				{
					yield return new WaitForSeconds(timeBetweenSpawns);
					SpawnUnit();
				}
				break;

				//only spawn things once
			case SpawnModes.Once:
				if (totalSpawnedUnits >= totalUnits)
				{
					spawn = false;
				}
				else
				{
					yield return new WaitForSeconds(timeBetweenSpawns);
					SpawnUnit();
				}
				break;

				//Wave mode
			case SpawnModes.Wave:
				if (numWaves <= totalWaves)
				{
					if (waveSpawn)
					{
						yield return new WaitForSeconds(timeBetweenSpawns);
						SpawnUnit();
						if ((totalSpawnedUnits / (numWaves + 1)) == totalUnits)
						{
							waveSpawn = false;
							checkEnemyLevel = true;
						}
					}
					else
					{
						if (checkEnemyLevel)
						{
							if (numberOfUnits <= 0)
							{
								numWaves++;
								checkEnemyLevel = false;
								numberOfUnits = 0;
								lastWaveSpawnTime = Time.time;
								yield return new WaitForSeconds(waveTimer);
								waveSpawn = true;
							}
						}
					}
				}
				else
				{
					spawn = false;
				}
				break;
			default:
				spawn = false;
				break;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	//resets everything to beginning status
	public void Reset()
	{
		waveSpawn = false;
		checkEnemyLevel = true;
		numWaves = 0;
		totalSpawnedUnits = 0;
		lastWaveSpawnTime = Time.time;
	}

	//returns num of waves
	public int WavesLeft
	{
		get
		{
			return totalWaves - numWaves;
		}
	}

	//returns tiume till next wave
	public float TimeTillWave
	{
		get
		{
			if (numWaves >= totalWaves)
			{
				return 0;
			}
			if (spawnType == SpawnModes.Wave && waveSpawn || numberOfUnits > 0)
			{
				return 0;
			}
			
			float time = (lastWaveSpawnTime + waveTimer) - Time.time;
			if (time >= 0)
				return time;
			else
				return 0;
		}
	}

	public void StartSpawn()
	{
		//if( PhotonNetwork.isMasterClient )
		{
			spawn = true;
			StartCoroutine("DoSpawn");
		}
	}

	private IEnumerator OpeningDelay()
	{
		if( startedSpawn == true )
		{
			print ("Starting spawn");
			StartSpawn();

			//return;
		}
		else
		{
			print ("Timer started");
			startedSpawn = true;
			yield return new WaitForSeconds(5.0f);
			StartCoroutine("OpeningDelay");

		}
	}
}
