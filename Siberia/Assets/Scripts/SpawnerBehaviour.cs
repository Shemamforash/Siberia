using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehaviour : MonoBehaviour
{
    public GameObject enemy_prefab, sapper_prefab, tank_prefab, sniper_prefab;
    private int total_enemies = 100;
    private int enemies_allowed_on_screen = 10;

    private List<GameObject> enemies_on_screen = new List<GameObject>();
    private float interval = 0.1f, time_since_last;

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
            while (enemies_on_screen.Count < enemies_allowed_on_screen)
            {
                GameObject new_enemy = null;
                bool enemy_spawned = false;
                int enemy_ID = Random.Range(0, 10);
                while (enemy_spawned == false)
                {
                    enemy_ID += 1;
                    if (enemy_ID > 10)
                    {
                        enemy_ID = 0;
                    }

                    int size = 0;
                    if (enemy_ID < 1)
                    {
                        new_enemy = GameObject.Instantiate(tank_prefab, transform.position, transform.rotation);
                        new_enemy.GetComponent<TankEnemyController>().SetSpawner(gameObject);
                        size = new_enemy.GetComponent<TankEnemyController>().GetSize();
                    }
                    else if (enemy_ID < 2)
                    {
                        new_enemy = GameObject.Instantiate(sniper_prefab, transform.position, transform.rotation);
                        new_enemy.GetComponent<SniperEnemyController>().SetSpawner(gameObject);
                        size = new_enemy.GetComponent<SniperEnemyController>().GetSize();
                    }
                    else if (enemy_ID < 5)
                    {
                        new_enemy = GameObject.Instantiate(sapper_prefab, transform.position, transform.rotation);
                        new_enemy.GetComponent<SapperBehaviour>().SetSpawner(gameObject);
                        size = new_enemy.GetComponent<SapperBehaviour>().GetSize();
                    }
                    else
                    {
                        new_enemy = GameObject.Instantiate(enemy_prefab, transform.position, transform.rotation);
                        new_enemy.GetComponent<MeleeEnemyController>().SetSpawner(gameObject);
                        size = new_enemy.GetComponent<MeleeEnemyController>().GetSize();

                    }
                    if (total_enemies - size >= 0)
                    {
                        total_enemies -= size;
                        enemy_spawned = true;
                    }
                }
                enemies_on_screen.Add(new_enemy);
                if (total_enemies == 0)
                {
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }
}
