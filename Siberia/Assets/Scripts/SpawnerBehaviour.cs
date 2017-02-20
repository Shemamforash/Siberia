﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehaviour : MonoBehaviour {
	public GameObject enemy_prefab;
	private int total_enemies = 100;
	private int enemies_allowed_on_screen = 10;

	private List<GameObject> enemies_on_screen = new List<GameObject>();
	private float interval = 0.1f, time_since_last;

	public void Unregister(GameObject enemy){
		enemies_on_screen.Remove(enemy);
	}
	
	// Update is called once per frame
	void Update () {
		time_since_last += Time.deltaTime;
		if(time_since_last >= interval){
			while(enemies_on_screen.Count < enemies_allowed_on_screen){
				GameObject new_enemy = GameObject.Instantiate(enemy_prefab, transform.position, transform.rotation);
				new_enemy.GetComponent<BasicEnemyController>().SetSpawner(gameObject);
				enemies_on_screen.Add(new_enemy);
				--total_enemies;
				if(total_enemies == 0){
					Destroy(gameObject);
					break;
				}
			}
		}	
	}
}
