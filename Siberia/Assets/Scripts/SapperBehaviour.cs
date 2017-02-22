using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SapperBehaviour : BasicEnemyController
{
    public int damage_radius = 5, damage = 10;
    public LayerMask mask;

    void Start()
    {
        base.Init();
        move_speed = 4;
        enemy_HP = 1;
        detection_radius = 8;
        wander_radius = 10;
    }

    void Update()
    {
        base.MoveEnemy();
    }

    public override void Enemy_React(Rigidbody2D enemy_rigidbody, Vector2 player_position, Vector2 last_seen_player_location)
    {

    }

    private void Detonate(GameObject other)
    {
        if (other.tag == "Player" || other.tag == "Bullet")
        {
            Collider2D[] colliders_in_range = Physics2D.OverlapCircleAll(transform.position, damage_radius, mask);
            foreach (Collider2D g in colliders_in_range)
            {
                float distance = Vector2.Distance(transform.position, g.transform.position);
                float damage_modifier = 10 - (damage / damage_radius * distance);
                if (g.gameObject.tag == "Enemy")
                {
                    g.GetComponent<BasicEnemyController>().take_damage((int)damage_modifier, Player.states.none);
                }
                else
                {
                    // TODO player take damage
                }
            }
            take_damage(100, Player.states.none);
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
