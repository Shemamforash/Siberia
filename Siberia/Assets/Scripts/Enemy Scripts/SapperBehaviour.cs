using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SapperBehaviour : BasicEnemyController
{
    private int damage_radius;
    public LayerMask mask;
    private bool exploding = false;
    private float exploding_time = 0f, explosion_duration = 2;

    void Start()
    {
        base.Init();
        SetSapperVals();
    }

    private void SetSapperVals()
    {
        Dictionary<string, float> game_data = GameController.GetGameData();
        this.move_speed = game_data["sapper_move_speed"];
        this.health = game_data["sapper_hp"];
        this.detection_radius = game_data["sapper_detection_rad"];
        this.chase_radius_multiplier = game_data["sapper_chase_radius_multiplier"];
        this.wander_radius = game_data["sapper_wander_radius"];
        this.damage = game_data["sapper_damage"];
        this.fire_rate = game_data["sapper_fire_rate"];
        this.powerup_value = game_data["sapper_powerup_value"];
        this.damage_radius = (int)game_data["sapper_damage_radius"];
        this.size = game_data["sapper_size"];
    }
    void Update()
    {
        base.MoveEnemy();
        if (exploding)
        {
            if (exploding_time >= explosion_duration)
            {
                GameController.UnregisterEnemy(gameObject);
                if (spawner != null)
                {
                    spawner.GetComponent<SpawnerBehaviour>().Unregister(gameObject);
                }

                //Tutorial: Count down the number of enemies to kill before door opens
                if (door != null)
                {
                    door.GetComponent<TutorialDoorController>().removeEnemy();
                }

                Destroy(gameObject);
            }
            exploding_time += Time.deltaTime;
        }
    }

    public override void Enemy_React(Rigidbody2D enemy_rigidbody, Vector2 player_position, Vector2 last_seen_player_location)
    {
        base.Chase_Player();
    }

    public void Detonate(GameObject other)
    {
        if (!exploding && (other.tag == "Player" || other.tag == "Bullet"))
        {
            exploding = true;
            Collider2D[] colliders_in_range = Physics2D.OverlapCircleAll(transform.position, damage_radius, mask);
            foreach (Collider2D g in colliders_in_range)
            {
                if (g.gameObject != this)
                {
                    float distance = Vector2.Distance(transform.position, g.transform.position);
                    if (distance <= damage_radius)
                    {
                        if (g.gameObject.tag == "Enemy")
                        {
                            if(g.gameObject.name.Contains("Sapper")){
                                g.gameObject.GetComponent<SapperBehaviour>().Detonate(other);
                            } else {
                                g.gameObject.GetComponent<BasicEnemyController>().take_damage((int)damage, player_object.GetComponent<Player>().GetState());
                            };
                        }
                        else if (g.gameObject.tag == "Player")
                        {
                            g.GetComponent<Player>().TakeDamage(damage);
                        }
                    }
                }
            }
            GetComponent<ParticleSystem>().Play();
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        Detonate(collider.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Detonate(collider.gameObject);
    }
}
