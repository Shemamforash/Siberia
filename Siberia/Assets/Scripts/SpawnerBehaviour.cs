﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehaviour : MonoBehaviour
{
    public GameObject enemy_prefab, sapper_prefab, tank_prefab, sniper_prefab;
    public int spawner_size = 100;
    public int enemies_allowed_on_screen = 10;

    private List<GameObject> enemies_on_screen = new List<GameObject>();
    public float spawn_rate = 1f;
    private float time_since_last;

    private float grunt_chance, sapper_chance, tank_chance, sniper_chance;

    public void Unregister(GameObject enemy)
    {
        enemies_on_screen.Remove(enemy);
    }

    public Vector3 spawn_chances;

    void Start(){
        tank_chance = spawn_chances.z;
        sniper_chance = tank_chance + spawn_chances.y;
        sapper_chance = sniper_chance + spawn_chances.x;
        grunt_chance = 1;
        Debug.Log(tank_chance + " " + sniper_chance + " " + sapper_chance + " " + grunt_chance);
    }

    // Update is called once per frame
    void Update()
    {
        time_since_last += Time.deltaTime;
        if (time_since_last >= spawn_rate)
        {
            time_since_last = 0f;
            if (enemies_on_screen.Count < enemies_allowed_on_screen)
            {
                GameObject new_enemy = null;
                bool enemy_spawned = false;
                float random = Random.Range(0f, 1f);
                GameObject prefab = null;

                while (enemy_spawned == false)
                {
                    int size = 0;
                    random += 0.1f;
                    if (random > 1f)
                    {
                        random = 0;
                    }
                    if (random < tank_chance)
                    {
                        prefab = tank_prefab;
                        size = (int)GameController.GetGameData()["tank_size"];
                    }
                    else if (random < sniper_chance)
                    {
                        prefab = sniper_prefab;
                        size = (int)GameController.GetGameData()["sniper_size"];
                    }
                    else if (random < sapper_chance)
                    {
                        prefab = sapper_prefab;
                        size = (int)GameController.GetGameData()["sapper_size"];
                    }
                    else
                    {
                        prefab = enemy_prefab;
                        size = (int)GameController.GetGameData()["grunt_size"];

                    }
                    if (spawner_size - size >= 0)
                    {
                        Vector3 random_pos = (Vector2)transform.position + Random.insideUnitCircle;
                        new_enemy = GameObject.Instantiate(prefab, random_pos, transform.rotation);
                        new_enemy.GetComponent<BasicEnemyController>().SetSpawner(gameObject);
                        GameController.RegisterEnemy(new_enemy);
                        spawner_size -= size;
                        enemy_spawned = true;
                    }
                }
                enemies_on_screen.Add(new_enemy);
                if (spawner_size == 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
