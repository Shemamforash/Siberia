using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyController : BasicEnemyController
{
    public override void Enemy_React(Rigidbody2D enemy_rigidbody, Vector2 last_seen_player_location)
    {
        //Move directly towards last known location of player
        Vector2 dir_to_target = last_seen_player_location - enemy_rigidbody.position;
        if (dir_to_target.magnitude > 0.1f)
        {
            dir_to_target.Normalize();

            Vector2 dir_to_move = dir_to_target * move_speed;

            //Use raycasts to repel from walls
            float raycastRange = 1.0f;
            Debug.DrawRay(enemy_rigidbody.position, Quaternion.AngleAxis(45, new Vector3(0.0f, 0.0f, 1.0f)) * dir_to_target, Color.red);
            Debug.DrawRay(enemy_rigidbody.position, Quaternion.AngleAxis(-45, new Vector3(0.0f, 0.0f, 1.0f)) * dir_to_target, Color.blue);
            RaycastHit2D wallAvoidCastLeft = Physics2D.Raycast(enemy_rigidbody.position, Quaternion.AngleAxis(45, new Vector3(0.0f, 0.0f, 1.0f)) * dir_to_target, raycastRange, environment_layer_mask);
            RaycastHit2D wallAvoidCastRight = Physics2D.Raycast(enemy_rigidbody.position, Quaternion.AngleAxis(-45, new Vector3(0.0f, 0.0f, 1.0f)) * dir_to_target, raycastRange, environment_layer_mask);
            if (wallAvoidCastLeft.collider != null)
            {
                float x_distance = wallAvoidCastLeft.point.x - enemy_rigidbody.position.x;
                float y_distance = wallAvoidCastLeft.point.y - enemy_rigidbody.position.y;
                float sq_distance = x_distance * x_distance + y_distance * y_distance;
                float repelForce = raycastRange * raycastRange - sq_distance;

                Vector3 avoid_strength = Quaternion.AngleAxis(-90, new Vector3(0.0f, 0.0f, 1.0f)) * dir_to_target * wall_avoidance_strength;
                dir_to_move += new Vector2(avoid_strength.x, avoid_strength.y);
            }
            if (wallAvoidCastRight.collider != null)
            {
                float x_distance = wallAvoidCastRight.point.x - enemy_rigidbody.position.x;
                float y_distance = wallAvoidCastRight.point.y - enemy_rigidbody.position.y;
                float sq_distance = x_distance * x_distance + y_distance * y_distance;
                float repelForce = raycastRange * raycastRange - sq_distance;

                Vector3 avoid_strength = Quaternion.AngleAxis(90, new Vector3(0.0f, 0.0f, 1.0f)) * dir_to_target * wall_avoidance_strength;
                dir_to_move += new Vector2(avoid_strength.x, avoid_strength.y);
            }


            enemy_rigidbody.MovePosition(enemy_rigidbody.position + dir_to_move * Time.deltaTime);
            Face_direction(dir_to_target);
        }
        else
        {
            //Reached last known location of player. Need to re-establish eye contact.
            seen_player = false;
            //Reset waypoint in case visual contact not re-established
            //The enemy will then start wandering from this new point
            waypoint = enemy_rigidbody.position;
        }
    }
}
