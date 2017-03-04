using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LOS;

//Veles and Perun

public class Player : MonoBehaviour
{
    private static float player_health, meter_loss_amount, base_move_speed, base_armour, base_accuracy, base_range, dark_fire_rate, light_fire_rate, base_light_damage, base_dark_damage;

    public static void SetPlayerVals(float player_health, float meter_loss_amount, float base_move_speed, float base_armour, float base_accuracy, float base_range, float dark_fire_rate, float light_fire_rate, float base_light_damage, float base_dark_damage)
    {
        Player.player_health = player_health;
        Player.meter_loss_amount = meter_loss_amount;
        Player.base_move_speed = base_move_speed;
        Player.base_armour = base_armour;
        Player.base_accuracy = base_accuracy;
        Player.base_range = base_range;
        Player.dark_fire_rate = dark_fire_rate;
        Player.light_fire_rate = light_fire_rate;
        Player.base_light_damage = base_light_damage;
        Player.base_dark_damage = base_dark_damage;
    }

    //States
    public enum states { dark, light, none };
    private states current_state = states.light;

    //Exposed Variables
    public GameObject torch_object, permanent_torch_object, projectile_prefab;
    public GameObject health_slider, health_color;
    public LayerMask mask;
    private Rigidbody2D my_rigidBody;
    private SpriteRenderer sprite_renderer;

    //Colors!
    private Color lightColour = new Color(0.9f, 0.9f, 0.9f);
    private Color darkColour = new Color(0.1f, 0.1f, 0.1f);
    private float damage_countdown;

    //Sounds
    [SerializeField]
    private AudioClip pickup_sfx;
    [SerializeField]
    private AudioClip dark_sfx_1;
    [SerializeField]
    private AudioClip dark_sfx_2;
    [SerializeField]
    private AudioClip dark_sfx_3;
    private AudioClip[] dark_sfx;
    [SerializeField]
    private AudioClip light_sfx;
    private AudioSource audio_source;

    //Game logic
    private bool fired_projectile_light = false, fired_projectile_dark = false;
    private float current_health, damage, accuracy, range, move_speed, armour;
    private float time_since_last_fire_dark = 0f, time_since_last_fire_light = 0f;

    private ParticleSystem blast_wave_particles, residual_particles;

    void Start()
    {
        my_rigidBody = gameObject.GetComponent<Rigidbody2D>();
        torch_object = transform.Find("Torch").gameObject;
        permanent_torch_object = transform.Find("Permanent Torch").gameObject;
        sprite_renderer = GetComponent<SpriteRenderer>();
        residual_particles = gameObject.GetComponent<ParticleSystem>();
        blast_wave_particles = permanent_torch_object.GetComponent<ParticleSystem>();

        audio_source = gameObject.GetComponent<AudioSource>();
        dark_sfx = new AudioClip[3] { dark_sfx_1, dark_sfx_2, dark_sfx_3 };

        damage_countdown = 0.0f;

        current_health = Player.player_health;
        ToggleState();
    }

    private void TakeMouse()
    {
        if (current_state == states.dark)
        {
            if (Input.GetMouseButton(0) && !fired_projectile_dark)
            {
                current_health -= 0.2f;

                float z_value = transform.rotation.eulerAngles.z;
                z_value += Random.Range(-accuracy, accuracy);

                Quaternion projectile_rotation = Quaternion.Euler(0, 0, z_value);
                GameObject new_projectile = GameObject.Instantiate(projectile_prefab, transform.position, projectile_rotation);
                new_projectile.GetComponent<ProjectileBehaviour>().SetDamage((int)damage);

                //Play random shot sound
                audio_source.PlayOneShot(dark_sfx[Random.Range(0, 3)], 0.5f);

                fired_projectile_dark = true;
            }
        }
        else if (current_state == states.light)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!fired_projectile_light)
                {
                    current_blast_radius = 0;
                    blast_timer = 0;
                    current_health -= 10f;
                    max_blast_radius = range;
                    blast_epicentre = transform.position;
                    var blast_module = blast_wave_particles.main;
                    blast_module.startSpeed = range;
                    var residual_module = blast_wave_particles.main;
                    residual_module.startSpeed = range;

                    audio_source.PlayOneShot(light_sfx, 0.5f);

                    fired_projectile_light = true;
                    emitting_blast = true;

                    enemies_hit_this_blast = new List<GameObject>();
                    blast_wave_particles.Play();
                    residual_particles.Play();
                }
            }
        }

    }

    private float current_blast_radius = 0, max_blast_radius = 0f, blast_timer, blast_max_time = 2f;
    private List<GameObject> enemies_hit_this_blast = new List<GameObject>();
    private bool emitting_blast = false;
    private Vector3 blast_epicentre;

    public void UpdateLightPushBack()
    {
        if (emitting_blast)
        {
            blast_timer += Time.deltaTime;
            if (blast_timer > blast_max_time)
            {
                emitting_blast = false;
            }
            current_blast_radius += max_blast_radius * Time.deltaTime;

            Collider2D[] objects_in_range = Physics2D.OverlapCircleAll(blast_epicentre, current_blast_radius + 0.7f);
            foreach (Collider2D collider in objects_in_range)
            {
                GameObject collider_object = collider.gameObject;
                if (collider_object.tag == "Enemy")
                {
                    if(!enemies_hit_this_blast.Contains(collider_object)){
                        collider_object.GetComponent<BasicEnemyController>().take_damage((int)Player.base_light_damage, Player.states.light);
                        enemies_hit_this_blast.Add(collider_object);
                    }
                    Vector3 dir_to_enemy = collider_object.transform.position - blast_epicentre;
                    dir_to_enemy.Normalize();
                    dir_to_enemy *= Time.deltaTime * max_blast_radius;
                    collider_object.transform.Translate(dir_to_enemy, Space.World);
                }
            }
        }
    }

    public Player.states GetState()
    {
        return current_state;
    }

    private void UpdateWeaponCooldowns()
    {
        if (fired_projectile_dark)
        {
            time_since_last_fire_dark += Time.deltaTime;

            if (time_since_last_fire_dark >= Player.dark_fire_rate)
            {
                time_since_last_fire_dark = 0;
                fired_projectile_dark = false;
            }

        }
        if (fired_projectile_light)
        {
            //Debug.Log(time_since_last_fire_dark + " " + Player.light_fire_rate);
            time_since_last_fire_light += Time.deltaTime;
            if (time_since_last_fire_light >= Player.light_fire_rate)
            {
                time_since_last_fire_light = 0;
                fired_projectile_light = false;
            }
        }
    }

    private void PointToMouse()
    {
        Vector3 reference_dir = new Vector3(0, 1, 0);
        Vector3 mouse_screen = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouse_position = mouse_screen;
        mouse_position = mouse_position - transform.position;
        mouse_position.z = 0;
        float dot_product = Vector3.Dot(reference_dir, mouse_position);
        float theta = Mathf.Acos(dot_product / mouse_position.magnitude) * Mathf.Rad2Deg;
        if (mouse_screen.x < transform.position.x)
        {
            theta = 360 - theta;
        }
        Vector3 vect_rotation = new Vector3(0, 0, 360 - theta);
        Quaternion new_rotation = Quaternion.Euler(vect_rotation);
        transform.rotation = new_rotation;
        torch_object.transform.rotation = Quaternion.Euler(Vector3.up);
        permanent_torch_object.transform.rotation = Quaternion.Euler(Vector3.up);
    }

    private void TakeInput()
    {
        Vector2 movement_difference = new Vector2();
        if (Input.GetKey("w"))
        {
            movement_difference.y += move_speed;
        }
        else if (Input.GetKey("s"))
        {
            movement_difference.y -= move_speed;
        }
        if (Input.GetKey("a"))
        {
            movement_difference.x -= move_speed;
        }
        else if (Input.GetKey("d"))
        {
            movement_difference.x += move_speed;
        }
        my_rigidBody.MovePosition(my_rigidBody.position + movement_difference * Time.fixedDeltaTime);
        if (Input.GetKeyUp("space") || Input.GetMouseButtonUp(1))
        {
            ToggleState();
        }
    }

    private void DevInput()
    {
        if (Input.GetKey("e"))
        {
            current_health = 0;
        }
    }

    private void ToggleState()
    {
        if (current_state == states.dark)
        {
            current_state = states.light;
            health_color.GetComponent<Image>().color = lightColour;
            torch_object.GetComponent<LOSRadialLight>().color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            current_state = states.dark;
            health_color.GetComponent<Image>().color = darkColour;
            torch_object.GetComponent<LOSRadialLight>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DevInput();
        TakeInput();
        PointToMouse();
        TakeMouse();
        UpdateMeters();
        UpdateWeaponCooldowns();
        UpdateColour();
        UpdateLightPushBack();
    }

    private void UpdateColour()
    {
        if (damage_countdown > 0.0f)
        {
            damage_countdown -= Time.deltaTime;
        }

        if (current_state == states.dark)
        {
            sprite_renderer.color = darkColour + new Color(damage_countdown, 0.0f, 0.0f);
        }
        else
        {
            sprite_renderer.color = lightColour - new Color(0.0f, damage_countdown, damage_countdown, 0.0f);
        }
    }

    private void UpdateMeters()
    {
        if (current_health <= 0)
        {
            Camera.main.GetComponent<MenuNavigator>().LoadGameOver();
        }
        else if (current_health > Player.player_health)
        {
            current_health = Player.player_health;
        }

        /*
* One health bar 0-100 hp
* Light form modifier = 1 / 100 * hp
* Dark form modifier = 1 - light form modifier
* Player attributes: Damage, accuracy/range, Armour, Speed
* Dark form (closer to high health) -> lower damage, higher accuracy, higher armour, lower speed
* Light form (closer to high health) -> higher damage, lower range, lower armour, higher speed
*/

        //Min move speed is 50% base_speed
        float light_mod = current_health / 100f;
        float dark_mod = 1 - light_mod;

        float half_speed = Player.base_move_speed * 0.5f;

        if (current_state == states.dark)
        {
            float quarter_damage = 0.25f * Player.base_dark_damage;
            damage = quarter_damage + (3 * quarter_damage * dark_mod);
            accuracy = Player.base_accuracy * dark_mod;
            move_speed = half_speed + (dark_mod * half_speed);
            armour = 1 + Player.base_armour * light_mod;
        }
        else
        {
            float quarter_damage = 0.25f * Player.base_light_damage;
            damage = quarter_damage + (3 * quarter_damage * light_mod);
            range = Player.base_range * dark_mod + 2;
            move_speed = half_speed + (light_mod * half_speed);
            armour = 1 + Player.base_armour * dark_mod;
        }

        health_slider.GetComponent<Slider>().value = current_health;
    }

    public void ReceivePickup(int value, states type)
    {
        audio_source.PlayOneShot(pickup_sfx, 1.0f);

        if (type == current_state)
        {
            current_health += value;
        }
        else
        {
            current_health += (int)(0.5f * value);
        }
    }

    private float wave_particle_damage_timer = 1, wave_particle_damage_cooldown = 1;

    void OnParticleCollision(GameObject other)
    {
        if (wave_particle_damage_timer >= wave_particle_damage_cooldown)
        {
            wave_particle_damage_timer = 0;
            TakeDamage(10);
        }
        wave_particle_damage_timer += Time.deltaTime;
    }

    public void TakeDamage(float amount)
    {
        current_health -= (int)(amount / armour);
        damage_countdown = 1.0f;
        if (current_health < 0)
        {
            current_health = 0;
        }
    }
}
