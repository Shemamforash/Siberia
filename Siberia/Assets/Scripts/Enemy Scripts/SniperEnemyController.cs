﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SniperEnemyController : BasicEnemyController
{
    [SerializeField]
    private GameObject sniper_shot;
    [SerializeField]
    private float shot_range;
    [SerializeField]
    private float charge_time; //How long the player has to be in view before firing
    [SerializeField]
    private float warning_time; //How long the laser sight goes red for before firing
    [SerializeField]
    private float cooldown_time; //How long after firing can the enemy start aiming again
    [SerializeField]
    private float hunt_time; //How long will the enemy track the player behind walls before getting bored
    [SerializeField]
    private LayerMask sniper_mask = -1;
    [SerializeField]
    private float min_preferred_range = 5.0f;
    private float max_preferred_range;

    private LineRenderer laser_sight;

    private float charge_countdown;
    private float cooldown_countdown;
    private float warning_countdown;
    private float hunt_countdown;
    private int state; //0: aiming, 1: locking, 2: warning, 3: cooldown

    //Audio
    [SerializeField]
    private AudioClip sniper_sfx;
    private AudioSource audio_source;

    private Vector2 fire_direction;

    void Start()
    {
        base.Init();
        SetSniperVals();
        laser_sight = this.gameObject.transform.GetChild(0).GetComponent<LineRenderer>();
        laser_sight.widthMultiplier = 0.05f;

        charge_countdown = 0;
        cooldown_countdown = 0;
        warning_countdown = 0;
        hunt_countdown = 0;
        state = 0;
    }

    private void SetSniperVals()
    {
        Dictionary<string, float> game_data = GameController.GetGameData();
        this.move_speed = game_data["sniper_move_speed"];
        this.health = game_data["sniper_hp"];
        this.detection_radius = game_data["sniper_detection_rad"];
        this.chase_radius_multiplier = game_data["sniper_chase_radius_multiplier"];
        this.wander_radius = game_data["sniper_wander_radius"];
        this.damage = game_data["sniper_damage"];
        this.charge_time = game_data["sniper_charge_time"];
        this.warning_time = game_data["sniper_warning_time"];
        this.cooldown_time = game_data["sniper_cooldown_time"];
        this.powerup_value = game_data["sniper_powerup_value"];
        this.hunt_time = game_data["sniper_hunt_time"];
        this.min_preferred_range = game_data["sniper_min_range"];
        this.max_preferred_range = game_data["sniper_max_range"];
        this.wall_avoidance_strength = game_data["sniper_wall_avoidance"];
        this.shot_range = game_data["sniper_range"];
        this.size = game_data["sniper_size"];

        audio_source = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        base.MoveEnemy();
        UpdateColor();
    }

    public override void Enemy_React(Rigidbody2D enemy_rigidbody, Vector2 player_position, Vector2 last_seen_player_location)
    {
        Vector2 dir_to_player = player_position - enemy_rigidbody.position;

        RaycastHit2D laser_hit;

        if (state == 0 || state == 1 || state == 2)
        {
            laser_sight.enabled = true;
            fire_direction = dir_to_player.normalized;

            //Aim in the player's direction
            laser_hit = Physics2D.Raycast(enemy_rigidbody.position, dir_to_player, shot_range, sniper_mask);
            if (laser_hit.collider != null && laser_hit.collider.tag == "Player")
            {
                state = 1;
                //Purple laser indicates lock-on
                float alpha = 1 / charge_time * charge_countdown;
                if(alpha > 1){
                    alpha = 1;
                }
                laser_sight.startColor = new Color(0.7f, 0.7f, 0.7f, alpha);
                laser_sight.endColor = new Color(1f, 1f, 1f, alpha);

                charge_countdown += Time.deltaTime;

                if (charge_countdown >= charge_time)
                {
                    state = 2;

                    //RED LASER - WARNING (Ready to fire)
                    laser_sight.startColor = Color.red;
                    laser_sight.endColor = Color.red;

                    warning_countdown += Time.deltaTime;

                    if (warning_countdown > warning_time)
                    {
                        state = 3;
                        //Play firing noise
                        audio_source.PlayOneShot(sniper_sfx, 0.5f);
                        GameObject projectile = Instantiate(sniper_shot, enemy_rigidbody.position, Quaternion.LookRotation(new Vector3(0.0f, 0.0f, 1.0f), fire_direction));
                        projectile.GetComponent<SniperProjectileBehaviour>().SetDamage(damage);
                        warning_countdown = 0;
                        charge_countdown = 0;
                        laser_sight.enabled = false;
                    }
                }

                hunt_countdown = 0;
            }
            else
            {
                state = 0;
                //White laser to indicate no lock
                laser_sight.startColor = Color.white;
                laser_sight.endColor = Color.white;

                charge_countdown = 0;
                hunt_countdown += Time.deltaTime;

                if (hunt_countdown > hunt_time)
                {
                    laser_sight.enabled = false;
                    //Player hasn't been visible for a while.
                    seen_player = false;
                    //Reset waypoint in case visual contact not re-established
                    //The enemy will then start wandering from this new point
                    waypoint = enemy_rigidbody.position;
                }
            }

            //Update the laser sight position
            if(laser_hit.collider != null)
            {
                Vector3[] laser_sight_points = { enemy_rigidbody.position, laser_hit.point };
                laser_sight.SetPositions(laser_sight_points);
            }
            else
            {
                laser_sight.enabled = false;
            }
           
        }
        else if (state == 3)
        {
            //Recharge
            cooldown_countdown += Time.deltaTime;

            if (cooldown_countdown > cooldown_time)
            {
                state = 0;
                cooldown_countdown = 0;
            }
        }

        Face_direction(dir_to_player.normalized);

        track_player(enemy_rigidbody, player_position);
    }

    /*
     * Try to stay in the preferred range from the player
     */
    private void track_player(Rigidbody2D enemy_rigidbody, Vector2 player_position)
    {
        Vector2 dir_awayfrom_player = enemy_rigidbody.position - player_position;
        //Compare square edistances to avoid costly sqrt calls
        if (dir_awayfrom_player.sqrMagnitude < min_preferred_range * min_preferred_range)
        {
            change_distance(enemy_rigidbody, dir_awayfrom_player);
        }
        else if(dir_awayfrom_player.sqrMagnitude > max_preferred_range * max_preferred_range)
        {
            change_distance(enemy_rigidbody, new Vector2(-dir_awayfrom_player.x, -dir_awayfrom_player.y));
        }
    }

    /*
     * Used to apprach and flee from player, avoiding walls
     */
    private void change_distance(Rigidbody2D enemy_rigidbody, Vector2 direction_to_move)
    {
        direction_to_move.Normalize();

        Vector2 dir_to_move = direction_to_move * move_speed;

        //Use raycasts to repel from walls
        float raycastRange = 1.0f;
        RaycastHit2D wallAvoidCastLeft = Physics2D.Raycast(enemy_rigidbody.position, Quaternion.AngleAxis(45, new Vector3(0.0f, 0.0f, 1.0f)) * direction_to_move, raycastRange, environment_layer_mask);
        RaycastHit2D wallAvoidCastRight = Physics2D.Raycast(enemy_rigidbody.position, Quaternion.AngleAxis(-45, new Vector3(0.0f, 0.0f, 1.0f)) * direction_to_move, raycastRange, environment_layer_mask);
        if (wallAvoidCastLeft.collider != null)
        {
            float x_distance = wallAvoidCastLeft.point.x - enemy_rigidbody.position.x;
            float y_distance = wallAvoidCastLeft.point.y - enemy_rigidbody.position.y;
            float sq_distance = x_distance * x_distance + y_distance * y_distance;
            float repelForce = raycastRange * raycastRange - sq_distance;

            Vector3 avoid_strength = Quaternion.AngleAxis(-90, new Vector3(0.0f, 0.0f, 1.0f)) * direction_to_move * repelForce * wall_avoidance_strength;
            dir_to_move += new Vector2(avoid_strength.x, avoid_strength.y);
        }
        if (wallAvoidCastRight.collider != null)
        {
            float x_distance = wallAvoidCastRight.point.x - enemy_rigidbody.position.x;
            float y_distance = wallAvoidCastRight.point.y - enemy_rigidbody.position.y;
            float sq_distance = x_distance * x_distance + y_distance * y_distance;
            float repelForce = raycastRange * raycastRange - sq_distance;

            Vector3 avoid_strength = Quaternion.AngleAxis(90, new Vector3(0.0f, 0.0f, 1.0f)) * direction_to_move * repelForce * wall_avoidance_strength;
            dir_to_move += new Vector2(avoid_strength.x, avoid_strength.y);
        }

        enemy_rigidbody.MovePosition(enemy_rigidbody.position + dir_to_move * Time.deltaTime);
    }
}
