using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEnemyController : BasicEnemyController
{
    [SerializeField]
    GameObject shockwave_attack;
    [SerializeField]
    private float shockwave_cooldown;

    private float shockwave_countdown;
    private float firepause;

    void Start()
    {
        base.Init();
        shockwave_countdown = shockwave_cooldown;
        firepause = 0.0f;
    }

    void Update()
    {
        base.MoveEnemy();

        if(shockwave_countdown > 0)
            shockwave_countdown -= Time.deltaTime;

        if (firepause > 0)
            firepause -= Time.deltaTime;
    }

    public override void Enemy_React(Rigidbody2D enemy_rigidbody, Vector2 player_position, Vector2 last_seen_player_location)
    {
        //In-range of player
        Vector2 distance_to_player = (player_position - enemy_rigidbody.position);
        if (distance_to_player.sqrMagnitude < 2.0)
        {
            fire_shockwave(enemy_rigidbody);
            firepause = 0.5f;
        }

        if (firepause <= 0)
        {
            base.Chase_Player();
        }
    }

    private void fire_shockwave(Rigidbody2D enemy_rigidbody)
    {
        if(shockwave_countdown <= 0)
        {
            Instantiate(shockwave_attack, enemy_rigidbody.position, Quaternion.AngleAxis(enemy_rigidbody.rotation, new Vector3(0.0f, 0.0f, 1.0f)));
            shockwave_countdown = shockwave_cooldown;
        }
        
    }
}
