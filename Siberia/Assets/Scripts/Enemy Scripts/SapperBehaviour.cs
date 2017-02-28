using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SapperBehaviour : BasicEnemyController
{
    private int damage_radius;
    public LayerMask mask;

    void Start()
    {
        base.Init();
        SetSapperVals();
    }

    private void SetSapperVals(){
        Dictionary<string, float> game_data = GameController.GetGameData();
        this.move_speed = game_data["sapper_move_speed"];
        this.health = game_data["sapper_hp"];
        this.detection_radius = game_data["sapper_detection_rad"];
        this.chase_radius_multiplier = game_data["sapper_chase_radius_multiplier"];
        this.wander_radius = game_data["sapper_wander_radius"];
        this.damage = game_data["sapper_damage"];
        this.fire_rate = game_data["sapper_fire_rate"];
        this.powerup_value = game_data["sapper_powerup_value"];
        this.damage_radius = (int)game_data["sapper_damage_radius"];
        this.size = game_data["sapper_size"];
    }
    void Update()
    {
        base.MoveEnemy();
    }

    public override void Enemy_React(Rigidbody2D enemy_rigidbody, Vector2 player_position, Vector2 last_seen_player_location)
    {
        base.Chase_Player();
    }

    private void Detonate(GameObject other)
    {
        if (other.tag == "Player" || other.tag == "Bullet")
        {
            Collider2D[] colliders_in_range = Physics2D.OverlapCircleAll(transform.position, damage_radius, mask);
            foreach (Collider2D g in colliders_in_range)
            {
                float distance = Vector2.Distance(transform.position, g.transform.position);
                if(distance <= damage_radius){
                    if (g.gameObject.tag == "Enemy")
                    {
                        g.GetComponent<BasicEnemyController>().take_damage((int)damage, Player.states.none);
                    }
                    else if(g.gameObject.tag == "Player")
                    {
                        g.GetComponent<Player>().TakeDamage(damage);
                    }
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
