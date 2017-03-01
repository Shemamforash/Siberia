using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehaviour : MonoBehaviour
{
    public GameObject enemy_prefab, sapper_prefab, tank_prefab, sniper_prefab;
    private int total_enemies = 100;
    private int enemies_allowed_on_screen = 10;

    private List<GameObject> enemies_on_screen = new List<GameObject>();
    private float interval = 1f, time_since_last;

    public void Unregister(GameObject enemy)
    {
        enemies_on_screen.Remove(enemy);
    }

    // Update is called once per frame
    void Update()
    {
        time_since_last += Time.deltaTime;
        if (time_since_last >= interval)
        {
            time_since_last = 0f;
            if (enemies_on_screen.Count < enemies_allowed_on_screen)
            {
                GameObject new_enemy = null;
                bool enemy_spawned = false;
                int enemy_ID = Random.Range(0, 10);
                GameObject prefab = null;

                while (enemy_spawned == false)
                {
                    enemy_ID += 1;
                    if (enemy_ID > 10)
                    {
                        enemy_ID = 0;
                    }

                    int size = 0;
                    if (enemy_ID <= 1)
                    {
                        prefab = tank_prefab;
                        size = (int)GameController.GetGameData()["tank_size"];
                    }
                    else if (enemy_ID <= 2)
                    {
                        prefab = sniper_prefab;
                        size = (int)GameController.GetGameData()["sniper_size"];
                    }
                    else if (enemy_ID <= 5)
                    {
                        prefab = sapper_prefab;
                        size = (int)GameController.GetGameData()["sapper_size"];
                    }
                    else
                    {
                        prefab = enemy_prefab;
                        size = (int)GameController.GetGameData()["grunt_size"];

                    }
                    if (total_enemies - size >= 0)
                    {
                        Vector3 random_pos = (Vector2)transform.position + Random.insideUnitCircle;
                        new_enemy = GameObject.Instantiate(prefab, random_pos, transform.rotation);
                        new_enemy.GetComponent<BasicEnemyController>().SetSpawner(gameObject);
                        GameController.RegisterEnemy(new_enemy);
                        total_enemies -= size;
                        enemy_spawned = true;
                    }
                }
                enemies_on_screen.Add(new_enemy);
                if (total_enemies == 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
