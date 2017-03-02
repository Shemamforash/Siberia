using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyController : BasicEnemyController
{
    private float cooldown;

    void Start()
    {
        base.Init();
        SetTutGruntVals();
    }

    private void SetTutGruntVals()
    {
        Dictionary<string, float> game_data = GameController.GetGameData();
        this.move_speed = game_data["grunt_move_speed"];
        this.health = game_data["grunt_hp"];
        this.detection_radius = game_data["tutorial_grunt_detection_rad"];
        this.chase_radius_multiplier = game_data["grunt_chase_radius_multiplier"];
        this.wander_radius = game_data["tutorial_grunt_wander_radius"];
        this.damage = game_data["grunt_damage"];
        this.fire_rate = game_data["grunt_fire_rate"];
        this.powerup_value = game_data["grunt_powerup_value"];
        this.wall_avoidance_strength = game_data["grunt_wall_avoidance"];
        this.size = game_data["grunt_size"];
        cooldown = fire_rate;
    }

    public override void Enemy_React(Rigidbody2D enemy_rigidbody, Vector2 player_position, Vector2 last_seen_player_location)
    {
        base.Chase_Player();
        if (Vector2.Distance(player_object.transform.position, gameObject.transform.position) < 0.5f)
        {
            if (cooldown >= fire_rate)
            {
                player_object.GetComponent<Player>().TakeDamage(this.damage);
                cooldown = 0;
            }
        }
        cooldown += Time.deltaTime;
    }
}
