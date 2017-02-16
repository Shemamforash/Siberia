using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Veles and Perun

public class Player : MonoBehaviour
{
    public float move_speed = 1f;
    public float x_max_bound = 2, x_min_bound = -2, y_max_bound = 2, y_min_bound = -2;

    public GameObject beam_prefab, projectile_prefab;
    private enum states { dark, light };
    private states current_state;
    private bool fired_projectile = false;

    private Rigidbody2D my_rigidBody;

    private void Start()
    {
        my_rigidBody = gameObject.GetComponent<Rigidbody2D>();
    }

    private void TakeMouse()
    {
        if (Input.GetMouseButton(0) && fired_projectile == false)
        {
            if (current_state == states.dark)
            {
                GameObject.Instantiate(projectile_prefab, transform.position, transform.rotation);
                fired_projectile = true;
            }
            else
            {

            }
        }
        if(Input.GetMouseButtonUp(0)){
            fired_projectile = false;
        }
    }

    private void ClampToBounds()
    {
        Vector3 new_position = transform.position;
        //Y bounds
        if (transform.position.y < y_min_bound)
        {
            new_position.y = y_min_bound;
        }
        else if (transform.position.y > y_max_bound)
        {
            new_position.y = y_max_bound;
        }

        //X Bounds
        if (transform.position.x < x_min_bound)
        {
            new_position.x = x_min_bound;
        }
        else if (transform.position.x > x_max_bound)
        {
            new_position.x = x_max_bound;
        }
        transform.position = new_position;
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
    }

    // Update is called once per frame
    void Update()
    {
        //ClampToBounds();
        PointToMouse();
        TakeMouse();
    }

    void FixedUpdate()
    {
        TakeInput();
    }
}
