using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicEnemyController : MonoBehaviour {

    [SerializeField]
    private float move_speed = 1;
    [SerializeField]
    private int enemy_HP = 100;

    [SerializeField]
    private float detection_radius = 4;
    [SerializeField]
    private float chase_radius_multiplier = 2;

    [SerializeField]
    private float wander_radius = 4;
    [SerializeField]
    private LayerMask environment_layer_mask = -1;

    [SerializeField]
    public GameObject player_object;

    private Transform player_transform;
    private Rigidbody2D enemy_rigidbody;

    public GameObject damage_text;
    private GameObject canvas_object;

    private float active_detection_radius;
    private Vector2 last_seen_player_location;
    private bool seen_player;

    private Vector2 waypoint;
    private float wander_counter;
    

    // Use this for initialization
    void Start () {
        canvas_object = GameObject.Find("Canvas");

        player_transform = player_object.transform;
        enemy_rigidbody = gameObject.GetComponent<Rigidbody2D>();

        active_detection_radius = detection_radius;
        seen_player = false;

        waypoint = enemy_rigidbody.position;
        wander_counter = Random.Range(5.0f, 10.0f);

        GameController.RegisterEnemy(gameObject);
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector2 distance_to_player = new Vector2(player_transform.position.x, player_transform.position.y) - enemy_rigidbody.position;

        //Is the player within sight range of the enemy?
        if(distance_to_player.magnitude < active_detection_radius)
        {
            Vector2 dir_to_player = distance_to_player.normalized;

            //Can the enemy see the player, or has it seen it recently?
            RaycastHit2D raycast_hit = Physics2D.Raycast(enemy_rigidbody.position, dir_to_player, distance_to_player.magnitude, environment_layer_mask);
            if (raycast_hit.collider != null)
            {
                if (seen_player)
                {
                    Enemy_Approach();
                }
                else
                {
                    //No sight of player; reset detection radius
                    active_detection_radius = detection_radius;

                    //Wander
                    Enemy_Wander();
                }
            }
            else
            {
                //Spotted player; increase detection radius
                seen_player = true;
                active_detection_radius = detection_radius * chase_radius_multiplier;
                last_seen_player_location = new Vector2(player_transform.position.x, player_transform.position.y);

                Enemy_Approach();
            }
        }
        else
        {
            //No sight of player; reset detection radius
            active_detection_radius = detection_radius;

            //Wander
            Enemy_Wander();
        }
	}

    private void Enemy_Wander()
    {
        wander_counter -= Time.deltaTime;

        if(wander_counter <= 0)
        //Find new waypoint
        {
            float wander_distance = Random.Range(0.0f, wander_radius);
            Vector2 wander_direction = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            wander_direction.Normalize();

            //See if the desired walk collides with a wall
            RaycastHit2D raycast_hit = Physics2D.Raycast(enemy_rigidbody.position, wander_direction, wander_distance, environment_layer_mask);
            if(raycast_hit.collider != null)
            {
                //Debug.Log("Can't move there");
            }
            else
            {
                waypoint = wander_direction * wander_distance;
            }

            wander_counter = Random.Range(5.0f, 10.0f);
        }
        else
        //Move towards current waypoint
        {
            Vector2 dir_to_waypoint = waypoint - enemy_rigidbody.position;

            //If the distance to the waypoint is small, stop moving
            //This prevents the enemy from spinning on the target
            if(dir_to_waypoint.magnitude > 0.1f)
            {
                dir_to_waypoint.Normalize();
                enemy_rigidbody.MovePosition(enemy_rigidbody.position + dir_to_waypoint * move_speed * Time.deltaTime);
                Face_direction(dir_to_waypoint);
            }
        }
    }

    private void Enemy_Approach()
    {
        //Move directly towards last known location of player
        Vector2 dir_to_target = last_seen_player_location - enemy_rigidbody.position;
        if(dir_to_target.magnitude > 0.1f)
        {
            dir_to_target.Normalize();
            enemy_rigidbody.MovePosition(enemy_rigidbody.position + dir_to_target * move_speed * Time.deltaTime);
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

    private void Face_direction(Vector3 direction)
    {
        Vector3 reference_dir = new Vector3(0, 1, 0);
        float dot_product = Vector3.Dot(reference_dir, direction);
        float theta = Mathf.Acos(dot_product) * Mathf.Rad2Deg;
        if (direction.x < 0)
        {
            theta = 360 - theta;
        }
        Vector3 vect_rotation = new Vector3(0, 0, 360 - theta);
        Quaternion new_rotation = Quaternion.Euler(vect_rotation);
        transform.rotation = new_rotation;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Terrain")
        {
            //Stop wandering until a new direction is picked
            waypoint = enemy_rigidbody.position;
        }
    }

    public void take_damage(int dmg)
    {
        enemy_HP -= dmg;
        Vector2 enemy_screen_location = Camera.main.WorldToScreenPoint(transform.position);
        GameObject new_damage = GameObject.Instantiate(damage_text, enemy_screen_location, Quaternion.Euler(Vector3.up));
        new_damage.transform.SetParent(canvas_object.transform);
        new_damage.GetComponent<Text>().text = dmg.ToString();
        if(enemy_HP <= 0)
        {
            GameController.UnregisterEnemey(gameObject);
            Destroy(gameObject);
        }
    }


}
