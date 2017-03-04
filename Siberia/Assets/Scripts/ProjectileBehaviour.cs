using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    private float time_alive = 0f;
    private float duration, speed;
    private int damage;

    void Start()
    {
        Dictionary<string, float> game_data = GameController.GetGameData();
        this.duration = game_data["projectile_duration"];
        this.speed = game_data["projectile_speed"];
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
        time_alive += Time.deltaTime;
        if (time_alive > duration)
        {
            DestroyProjectile();
        }
    }

    protected void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.gameObject.name.Contains("Sapper"))
            {
                collision.gameObject.GetComponent<SapperBehaviour>().Detonate(gameObject);
            }
            else
            {
                collision.gameObject.GetComponent<BasicEnemyController>().take_damage(damage, Player.states.dark);
            }


            //Debug.Log("Hit target");
        }
        DestroyProjectile();
    }
}
