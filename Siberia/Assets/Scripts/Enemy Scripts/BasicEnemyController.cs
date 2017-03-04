using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LOS;

public abstract class BasicEnemyController : MonoBehaviour
{
    [SerializeField]
    protected LayerMask environment_layer_mask = -1;

    [SerializeField]
    public GameObject player_object;
    public GameObject light_pickup_prefab, dark_pickup_prefab;

    private Transform player_transform;
    private Rigidbody2D enemy_rigidbody;

    public GameObject damage_text;
    private GameObject canvas_object;

    private float active_detection_radius;
    private Vector2 last_seen_player_location;
    protected bool seen_player;

    protected Vector2 waypoint;
    private float wander_counter;

    protected GameObject spawner;
    [SerializeField]
    protected GameObject door;

    protected float move_speed, health, detection_radius, chase_radius_multiplier, wander_radius, damage, fire_rate, powerup_value, wall_avoidance_strength, size;

    private LOSRadialLight permanent_torch_light, other_torch_light;

    public void SetSpawner(GameObject spawner)
    {
        this.spawner = spawner;
    }

    // Use this for initialization
    void Start()
    {
        Init();
    }

    public int GetSize()
    {
        return (int)size;
    }

    protected void Init()
    {
        canvas_object = GameObject.Find("Canvas");

        player_object = GameObject.Find("Player");
        player_transform = player_object.transform;
        enemy_rigidbody = gameObject.GetComponent<Rigidbody2D>();

        active_detection_radius = detection_radius;
        seen_player = false;

        waypoint = enemy_rigidbody.position;
        wander_counter = Random.Range(0.0f, 3.0f);

        permanent_torch_light = player_object.transform.Find("Permanent Torch").GetComponent<LOSRadialLight>();
        other_torch_light = player_object.transform.Find("Torch").GetComponent<LOSRadialLight>();

        //Tutorial: Add one enemy to door counter
        if (door != null)
        {
            door.GetComponent<TutorialDoorController>().addEnemy();
        }
    }

    void Update()
    {
        UpdateColor();
        MoveEnemy();
    }


    protected void UpdateColor()
    {
        float distance_to_player = Vector3.Distance(player_transform.position, transform.position);
        float main_torch_range = permanent_torch_light.radius - 3;
        float other_torch_range = other_torch_light.radius - 3;
        //main torch up to 0.6 alpha
        //other_torch up to 0.4 alpha
        float main_torch_light_level = 0, other_torch_light_level = 0;
        if (distance_to_player < main_torch_range)
        {
            main_torch_light_level = (1 - (1 / main_torch_range * distance_to_player)) * 0.6f;
        }
        if (distance_to_player < other_torch_range)
        {
            other_torch_light_level = (1 - (1 / other_torch_range * distance_to_player)) * 0.4f;
        }
        float total_illumination = main_torch_light_level + other_torch_light_level;
        Color new_color = new Color(total_illumination, total_illumination, total_illumination);
        transform.GetComponent<SpriteRenderer>().color = new_color;
    }

    // Update is called once per frame
    protected void MoveEnemy()
    {
        Vector2 distance_to_player = new Vector2(player_transform.position.x, player_transform.position.y) - enemy_rigidbody.position;

        //Is the player within sight range of the enemy?
        if (distance_to_player.magnitude < active_detection_radius)
        {
            seen_player = true;
            Vector2 dir_to_player = distance_to_player.normalized;

            //Can the enemy see the player, or has it seen it recently?
            RaycastHit2D raycast_hit = Physics2D.Raycast(enemy_rigidbody.position, dir_to_player, distance_to_player.magnitude, environment_layer_mask);
            if (raycast_hit.collider != null)
            {
                if (seen_player)
                {
                    Enemy_React(enemy_rigidbody, new Vector2(player_transform.position.x, player_transform.position.y), last_seen_player_location);
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

                Enemy_React(enemy_rigidbody, last_seen_player_location, last_seen_player_location);
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

        if (wander_counter <= 0)
        //Find new waypoint
        {
            float wander_distance = Random.Range(0.0f, wander_radius);
            Vector2 wander_direction = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            wander_direction.Normalize();

            //See if the desired walk collides with a wall
            RaycastHit2D raycast_hit = Physics2D.Raycast(enemy_rigidbody.position, wander_direction, wander_distance, environment_layer_mask);
            if (raycast_hit.collider != null)
            {
                //Debug.Log("Can't move there");
            }
            else
            {
                waypoint = enemy_rigidbody.position + wander_direction * wander_distance;
            }

            wander_counter = Random.Range(5.0f, 10.0f);
        }
        else
        //Move towards current waypoint
        {
            Vector2 dir_to_waypoint = waypoint - enemy_rigidbody.position;

            //If the distance to the waypoint is small, stop moving
            //This prevents the enemy from spinning on the target
            if (dir_to_waypoint.magnitude > 0.1f)
            {
                dir_to_waypoint.Normalize();
                enemy_rigidbody.MovePosition(enemy_rigidbody.position + dir_to_waypoint * move_speed * Time.deltaTime);
                Face_direction(dir_to_waypoint);
            }
        }
    }

    public abstract void Enemy_React(Rigidbody2D enemy_rigidbody, Vector2 player_position, Vector2 last_seen_player_location);

    /*
     * Move to the last known position of the player, avoiding walls
     */
    public void Chase_Player()
    {
        //Move directly towards last known location of player
        Vector2 dir_to_target = last_seen_player_location - enemy_rigidbody.position;
        if (dir_to_target.magnitude > 0.1f)
        {
            dir_to_target.Normalize();

            Vector2 dir_to_move = dir_to_target * move_speed;

            //Use raycasts to repel from walls
            float raycastRange = 1.0f;
            RaycastHit2D wallAvoidCastLeft = Physics2D.Raycast(enemy_rigidbody.position, Quaternion.AngleAxis(45, new Vector3(0.0f, 0.0f, 1.0f)) * dir_to_target, raycastRange, environment_layer_mask);
            RaycastHit2D wallAvoidCastRight = Physics2D.Raycast(enemy_rigidbody.position, Quaternion.AngleAxis(-45, new Vector3(0.0f, 0.0f, 1.0f)) * dir_to_target, raycastRange, environment_layer_mask);
            if (wallAvoidCastLeft.collider != null)
            {
                float x_distance = wallAvoidCastLeft.point.x - enemy_rigidbody.position.x;
                float y_distance = wallAvoidCastLeft.point.y - enemy_rigidbody.position.y;
                float sq_distance = x_distance * x_distance + y_distance * y_distance;
                float repelForce = raycastRange * raycastRange - sq_distance;

                Vector3 avoid_strength = Quaternion.AngleAxis(-90, new Vector3(0.0f, 0.0f, 1.0f)) * dir_to_target * repelForce * wall_avoidance_strength;
                dir_to_move += new Vector2(avoid_strength.x, avoid_strength.y);
            }
            if (wallAvoidCastRight.collider != null)
            {
                float x_distance = wallAvoidCastRight.point.x - enemy_rigidbody.position.x;
                float y_distance = wallAvoidCastRight.point.y - enemy_rigidbody.position.y;
                float sq_distance = x_distance * x_distance + y_distance * y_distance;
                float repelForce = raycastRange * raycastRange - sq_distance;

                Vector3 avoid_strength = Quaternion.AngleAxis(90, new Vector3(0.0f, 0.0f, 1.0f)) * dir_to_target * repelForce * wall_avoidance_strength;
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

    public void Face_direction(Vector3 direction)
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
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Terrain")
        {
            //Stop wandering until a new direction is picked
            waypoint = enemy_rigidbody.position;
        }
    }

    public void take_damage(int dmg, Player.states type)
    {
        health -= dmg;
        Vector2 enemy_screen_location = Camera.main.WorldToScreenPoint(transform.position);
        GameObject new_damage = GameObject.Instantiate(damage_text, transform.position, Quaternion.Euler(Vector3.up));
        new_damage.transform.SetParent(Camera.main.transform);
        new_damage.GetComponent<DamageTextBehaviour>().SetDamage(dmg);
        if (type == Player.states.dark)
        {
            transform.Translate(Vector2.down * 0.2f);
        }
        if (health <= 0)
        {
            GameObject new_pickup = null;
            if (powerup_value != 0)
            {
                if (type == Player.states.light)
                {
                    new_pickup = GameObject.Instantiate(dark_pickup_prefab, transform.position, transform.rotation);
                    type = Player.states.dark;
                }
                else
                {
                    new_pickup = GameObject.Instantiate(light_pickup_prefab, transform.position, transform.rotation);
                    type = Player.states.light;
                }
            }
            if (new_pickup != null)
            {
                new_pickup.GetComponent<Spinny>().SetPickupValue((int)powerup_value, type);
            }
            GameController.UnregisterEnemy(gameObject);
            if (spawner != null)
            {
                spawner.GetComponent<SpawnerBehaviour>().Unregister(gameObject);
            }

            //Tutorial: Count down the number of enemies to kill before door opens
            if (door != null)
            {
                door.GetComponent<TutorialDoorController>().removeEnemy();
            }
            Destroy(gameObject);

        }
    }
}
