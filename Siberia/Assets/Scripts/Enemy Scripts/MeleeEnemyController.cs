using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyController : BasicEnemyController
{

    void Start(){
        base.Init();
        SetGruntVals();
    }

    private void SetGruntVals(){
        Dictionary<string, float> game_data = GameController.GetGameData();
        this.move_speed = game_data["grunt_move_speed"];
        this.health = game_data["grunt_hp"];
        this.detection_radius = game_data["grunt_detection_rad"];
        this.chase_radius_multiplier = game_data["grunt_chase_radius_multiplier"];
        this.wander_radius = game_data["grunt_wander_radius"];
        this.damage = game_data["grunt_damage"];
        this.fire_rate = game_data["grunt_fire_rate"];
        this.powerup_value = game_data["grunt_powerup_value"];
        this.wall_avoidance_strength = game_data["grunt_wall_avoidance"];
    }

    public override void Enemy_React(Rigidbody2D enemy_rigidbody, Vector2 player_position, Vector2 last_seen_player_location)
    {
        base.Chase_Player();
    }
}
