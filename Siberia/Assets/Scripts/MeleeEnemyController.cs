using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyController : BasicEnemyController
{
    public override void Enemy_React(Rigidbody2D enemy_rigidbody, Vector2 player_position, Vector2 last_seen_player_location)
    {
        base.Chase_Player();
    }
}
