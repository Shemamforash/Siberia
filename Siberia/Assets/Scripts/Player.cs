using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LOS;

//Veles and Perun

public class Player : MonoBehaviour
{
    public float base_move_speed = 3f, meter_loss_amount = 16;
    private float move_speed = 3f;

    public GameObject torch_object, projectile_prefab;
    public GameObject light_slider, dark_slider;
    public Image state_UI;

    public enum states { dark, light, none };
    private states current_state;
    private bool fired_projectile_light = false, fired_projectile_dark = false, torch_on = false;

    private Color lightColour = new Color(0.9f, 0.8f, 0.1f);
    private Color darkColour = new Color(0.8f, 0.1f, 0.1f);

    private Rigidbody2D my_rigidBody;

    private float light_meter = 100, dark_meter = 100;

    private float base_damage, min_accuracy = 30f, base_range = 10f, damage, accuracy, range;

    void Start()
    {
        my_rigidBody = gameObject.GetComponent<Rigidbody2D>();
        current_state = states.dark;
        torch_object = transform.Find("Torch").gameObject;
    }

    private float time_since_last_fire_dark = 0f, time_since_last_fire_light = 0f, fire_rate_dark = 0.1f, fire_rate_light = 0.5f;
    public LayerMask mask;

    private void TakeMouse()
    {
        if (current_state == states.dark)
        {
            if (Input.GetMouseButton(0) && !fired_projectile_dark)
            {
                dark_meter -= 0.5f;
                float z_value = transform.rotation.eulerAngles.z;
                z_value += Random.Range(-accuracy, accuracy);
                Quaternion projectile_rotation = Quaternion.Euler(0, 0, z_value);
                GameObject.Instantiate(projectile_prefab, transform.position, projectile_rotation);
                fired_projectile_dark = true;
            }
            torch_object.GetComponent<LOSRadialLight>().enabled = false;
        }
        else if (current_state == states.light)
        {
            if (Input.GetMouseButton(0))
            {
                if (range != 0)
                {
                    torch_object.GetComponent<LOSRadialLight>().enabled = true;
                    torch_object.GetComponent<LOSRadialLight>().radius = range;
                }
                if (!fired_projectile_light)
                {
                    light_meter -= 0.5f;
                    for (int i = GameController.Enemies().Count - 1; i >= 0; --i)
                    {
                        GameObject enemy = GameController.Enemies()[i];
                        Vector3 dir_to_enemy = enemy.transform.position - transform.position;
                        GameObject hit_object = Physics2D.Raycast(transform.position, dir_to_enemy, 100f, mask).collider.gameObject;
                        if (hit_object.tag == "Enemy" && Vector3.Distance(transform.position, enemy.transform.position) < range)
                        {
                            enemy.GetComponent<BasicEnemyController>().take_damage(3, states.light);
                        }
                    }
                    fired_projectile_light = true;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                torch_object.GetComponent<LOSRadialLight>().enabled = false;
            }

        }
        if (fired_projectile_dark)
        {
            time_since_last_fire_dark += Time.deltaTime;

            if (time_since_last_fire_dark >= fire_rate_dark)
            {
                time_since_last_fire_dark = 0;
                fired_projectile_dark = false;
            }

        }
        else if (fired_projectile_light)
        {
            time_since_last_fire_light += Time.deltaTime;
            if (time_since_last_fire_light >= fire_rate_light)
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
        // transform.position += movement_difference * Time.deltaTime;
        if (Input.GetKeyUp("space"))
        {
            ToggleState();
        }
    }

    private void DevInput()
    {
        if (Input.GetKey("e"))
        {
            if (Input.GetKey("l"))
            {
                light_meter = 0;
            }
            else if (Input.GetKey("d"))
            {
                dark_meter = 0;
            }
        }
    }

    private void ToggleState()
    {
        if (current_state == states.dark)
        {
            current_state = states.light;
            state_UI.color = lightColour;
        }
        else
        {
            current_state = states.dark;
            state_UI.color = darkColour;
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

    }

    private void UpdateMeters()
    {
        if (current_state == states.dark)
        {
            light_meter -= meter_loss_amount * Time.deltaTime;
            if (light_meter < 0)
            {
                light_meter = 0;
                // SceneManager.LoadScene("Game Over");
            }
        }
        else
        {
            dark_meter -= meter_loss_amount * Time.deltaTime;
            if (dark_meter < 0)
            {
                dark_meter = 0;
            }
        }
        move_speed = base_move_speed * (light_meter / 100f);
        damage = base_damage - light_meter / 10f + 1;
        range = base_range * dark_meter / 100f;
        accuracy = min_accuracy * (100 - dark_meter) / 100f;

        light_slider.GetComponent<Slider>().value = light_meter;
        dark_slider.GetComponent<Slider>().value = dark_meter;
    }

    public void ReceivePickup(int value, states type)
    {
        if (type == states.dark)
        {
            dark_meter += value;
        }
        else
        {
            light_meter += value;
        }
    }
}
