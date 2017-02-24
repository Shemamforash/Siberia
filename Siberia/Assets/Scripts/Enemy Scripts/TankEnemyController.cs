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
        SetTankVals();
        shockwave_countdown = shockwave_cooldown;
        firepause = 0.0f;
    }
    private void SetTankVals()
    {
        Dictionary<string, float> game_data = GameController.GetGameData();
        this.move_speed = game_data["tank_move_speed"];
        this.health = game_data["tank_hp"];
        this.detection_radius = game_data["tank_detection_rad"];
        this.chase_radius_multiplier = game_data["tank_chase_radius_multiplier"];
        this.wander_radius = game_data["tank_wander_radius"];
        this.damage = game_data["tank_damage"];
        this.fire_rate = game_data["tank_fire_rate"];
        this.powerup_value = game_data["tank_powerup_value"];
        this.wall_avoidance_strength = game_data["tank_wall_avoidance"];
        this.size = game_data["tank_size"];
    }

    void Update()
    {
        base.MoveEnemy();

        if (shockwave_countdown > 0)
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
        if (shockwave_countdown <= 0)
        {
            Instantiate(shockwave_attack, enemy_rigidbody.position, Quaternion.AngleAxis(enemy_rigidbody.rotation, new Vector3(0.0f, 0.0f, 1.0f)));
            shockwave_countdown = shockwave_cooldown;
        }

    }
}
