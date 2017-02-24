using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyController : BasicEnemyController
{
    private static float move_speed, health, detection_radius, chase_radius_multiplier, wander_radius, damage, fire_rate, powerup_value;

    public static void SetGruntVals(float move_speed, float health, float detection_radius, float chase_radius_multiplier, float wander_radius, float damage, float fire_rate, float powerup_value){
        MeleeEnemyController.move_speed = move_speed;
        MeleeEnemyController.health = health;
        MeleeEnemyController.detection_radius = detection_radius;
        MeleeEnemyController.chase_radius_multiplier = chase_radius_multiplier;
        MeleeEnemyController.wander_radius = wander_radius;
        MeleeEnemyController.damage = damage;
        MeleeEnemyController.fire_rate = fire_rate;
        MeleeEnemyController.powerup_value = powerup_value;
    }

    public override void Enemy_React(Rigidbody2D enemy_rigidbody, Vector2 player_position, Vector2 last_seen_player_location)
    {
        base.Chase_Player();
    }
}
