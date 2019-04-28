﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup_Spawner : MonoBehaviour
{

    public List<GameObject> PowerupPrefabs;
    public List<int> listIndex;
    private Hashtable NumberSpawnable;


 // Start is called before the first frame update
void Start()
    {
        NumberSpawnable.Add(0, 1);//Bullet Time
        NumberSpawnable.Add(1, 1);//Cooldown delay
        NumberSpawnable.Add(2, 99);//Damage
        NumberSpawnable.Add(3, 1);//Explosion
        NumberSpawnable.Add(4, 3);//FireRate
        NumberSpawnable.Add(5, 1);//HealthRegen
        NumberSpawnable.Add(6, 3);//HeatCost
        NumberSpawnable.Add(7, 1);//Knockback
        NumberSpawnable.Add(8, 3);//Mystery Box
        NumberSpawnable.Add(9, 99);//Speed
        NumberSpawnable.Add(10, 99);//Spread
        NumberSpawnable.Add(11, 4);//HealthMax
        NumberSpawnable.Add(12, 2);//Accuracy
        NumberSpawnable.Add(13, 2);//Volume
    }



    public void SpawnPowerUp(GameObject objectIn)
    {
                int powerup = (int) Mathf.Floor(Random.Range(0, listIndex.Count));
                int powerupIn = listIndex[powerup];
         
                             
                    Instantiate(PowerupPrefabs[powerupIn], objectIn.transform);
                    NumberSpawnable[powerupIn] = (int) NumberSpawnable[powerupIn] - 1;
                    
        if((int)NumberSpawnable[powerupIn] == 0)
        {
            listIndex.Remove(powerupIn);
        }

    }

        
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
