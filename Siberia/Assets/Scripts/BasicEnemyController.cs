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

    private Transform enemy_transform;
    private Transform player_transform;
    private Rigidbody2D enemy_rigidbody;

    public GameObject damage_text;
    private GameObject canvas_object;

    private float active_detection_radius;

    private Vector2 waypoint;
    private float wander_counter;

    // Use this for initialization
    void Start () {
        canvas_object = GameObject.Find("Canvas");

        enemy_transform = gameObject.transform;
        player_transform = player_object.transform;
        enemy_rigidbody = gameObject.GetComponent<Rigidbody2D>();

        active_detection_radius = detection_radius;

        waypoint = enemy_transform.position;
        wander_counter = Random.Range(5.0f, 10.0f);

        GameController.RegisterEnemy(gameObject);
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 distance_to_player = player_transform.position - enemy_transform.position;

        if(distance_to_player.magnitude < active_detection_radius)
        {
            //Spotted player; increase detection radius
            active_detection_radius = detection_radius * chase_radius_multiplier;

            distance_to_player.Normalize();
            Enemy_Approach(new Vector2(distance_to_player.x, distance_to_player.y));
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

    private void Enemy_Approach(Vector2 dir_to_player)
    {
        enemy_rigidbody.MovePosition(enemy_rigidbody.position + dir_to_player * move_speed * Time.deltaTime);
        Face_direction(dir_to_player);
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
