using UnityEngine;
using System.Collections;
using System;

public class SniperEnemyController : BasicEnemyController
{
    [SerializeField]
    private GameObject sniper_shot;
    [SerializeField]
    private float shot_range = 15f;
    [SerializeField]
    private float charge_time = 1.0f;
    [SerializeField]
    private float warning_time = 0.1f;
    [SerializeField]
    private float cooldown_time = 1.0f;
    [SerializeField]
    private LayerMask sniper_mask = -1;

    private LineRenderer laser_sight;

    private float charge_countdown;
    private float cooldown_countdown;
    private float warning_countdown;
    private int state; //0: aiming, 1: locking, 2: warning, 3: cooldown

    private Vector2 fire_direction;

    void Start()
    {
        base.Init();

        laser_sight = this.gameObject.transform.GetChild(0).GetComponent<LineRenderer>();
        laser_sight.widthMultiplier = 0.05f;

        charge_countdown = 0;
        cooldown_countdown = 0;
        warning_countdown = 0;
        state = 0;
    }

    void Update()
    {
        base.MoveEnemy();
    }

    public override void Enemy_React(Rigidbody2D enemy_rigidbody, Vector2 player_position, Vector2 last_seen_player_location)
    {
        Vector2 dir_to_player = player_position - enemy_rigidbody.position;

        if (state == 0 || state == 1 || state == 2)
        {
            laser_sight.enabled = true;
            fire_direction = dir_to_player.normalized; 
            
            //Aim in the player's direction
            RaycastHit2D laser_hit = Physics2D.Raycast(enemy_rigidbody.position, dir_to_player, shot_range, sniper_mask);
            if (laser_hit.collider.tag == "Player")
            {
                state = 1;
                //Purple laser indicates lock-on
                laser_sight.startColor = Color.magenta;
                laser_sight.endColor = Color.magenta;

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
                        Instantiate(sniper_shot, enemy_rigidbody.position, Quaternion.LookRotation(new Vector3(0.0f, 0.0f, 1.0f), fire_direction));
                        warning_countdown = 0;
                        charge_countdown = 0;
                        laser_sight.enabled = false;
                    }
                }
            }
            else
            {
                state = 0;
                //White laser to indicate no lock
                laser_sight.startColor = Color.white;
                laser_sight.endColor = Color.white;

                charge_countdown = 0;
            }

            //Update the laser sight position
            Vector3[] laser_sight_points = { enemy_rigidbody.position, laser_hit.point };
            laser_sight.SetPositions(laser_sight_points);
        }
        else if(state == 3)
        {
            //Recharge
            cooldown_countdown += Time.deltaTime;

            if(cooldown_countdown > cooldown_time)
            {
                state = 0;
                cooldown_countdown = 0;
            }
        }

        //base.Chase_Player();
    }

    private void fire_sniper_bullet(Rigidbody2D enemy_rigidbody)
    {
        
        
    }
}
