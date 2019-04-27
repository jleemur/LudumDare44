﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public bool running = false;

    public int[] ninjasWaves;
    public int[] crudeCriminalsWaves;
    public int[] oilOrksWaves;
    public int numberOfWaves;
    // Start is called before the first frame update

    public GameObject ninja;
    public GameObject crudeCriminal;
    public GameObject oilOrk;

    private int currentWaveNumber = -1;

    public int currentAliveEnemyCount = 0;

    public Transform[] spawnPoints;
    public GameObject nextDoor;
    public GameObject oldDoor;
    public GameObject roof;

    private GameObject player;


	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
        nextDoor.SetActive(false);
        nextDoor.SetActive(true);
	}
    // Update is called once per frame
    void Update()
    {
        if(running) {
            if(currentAliveEnemyCount <=0) {
                currentWaveNumber++;

                if(currentWaveNumber >= numberOfWaves) {
                    OpenNextDoor();
                    running = false;
                } else {
                    
                    Quaternion rotation = Quaternion.identity;
                    //Spawn Ninjas
                    for(int i=0; i< ninjasWaves[currentWaveNumber]; i++) {
                        GameObject spawnedNinja = Instantiate (ninja, PickSpawnPointNotOnPlayer(), rotation);
                        spawnedNinja.GetComponent<Ninja>().roomController = this;
                        currentAliveEnemyCount++;
                    }

                    //Spawn Crude Criminals
                    for(int i=0; i< crudeCriminalsWaves[currentWaveNumber]; i++) {
                        GameObject spawnedCrudeCriminal = Instantiate (crudeCriminal, PickSpawnPointNotOnPlayer(), rotation);
                        spawnedCrudeCriminal.GetComponent<CrudeCriminal>().roomController = this;
                        currentAliveEnemyCount++;
                    }

                    //Spawn Orks
                    for(int i=0; i< oilOrksWaves[currentWaveNumber]; i++) {
                        //GameObject spawnedOilOrk = Instantiate (oilOrk, PickSpawnPointNotOnPlayer(), rotation);
                        //TODO: Assign roomcontroller here
                        //currentAliveEnemyCount++;
                    }
                }
            }
        }
    }

    public Vector2 PickSpawnPointNotOnPlayer() {
		if (player == null) {
			player = GameObject.FindGameObjectWithTag("Player");
		}
		Transform closestPoint = null;
		foreach(Transform point in spawnPoints) {
			if(closestPoint == null || Vector2.Distance(point.position, player.transform.position) < Vector2.Distance(closestPoint.position, player.transform.position)) {
				closestPoint = point;
			}
		}

		Vector2 chosenPosition;
		bool spawnPointChosen = false;
		while (!spawnPointChosen) {
			chosenPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

			if(!chosenPosition.Equals(closestPoint.position)) {
				return chosenPosition;
			}
		}

		return new Vector2();
	}

    public void DecrementAliveEnemyCount() {
        currentAliveEnemyCount--;
    }

    public void OpenNextDoor() {
        nextDoor.SetActive(false);
        //Play open door noise?
    }

    public void closePreviousDoor() {
        if(!running) {
            oldDoor.SetActive(true);
            running = true;
            //Play open door noise?
        }
    }

    public void DeactivateRoom() {
        //Close door behind player and show the roof
        nextDoor.SetActive(true);
        roof.SetActive(true);
        //Play close door noise?
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" ) {
            DeactivateRoom();
        }
    }
}