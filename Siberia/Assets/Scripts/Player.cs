﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Veles and Perun

public class Player : MonoBehaviour
{
    public float move_speed = 1;
    public float x_max_bound = 1, x_min_bound = -1, y_max_bound = 1, y_min_bound = -1;

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
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
		mousePosition.Normalize();
        float dot_product = Vector3.Dot(reference_dir, mousePosition);
        float theta = Mathf.Acos(dot_product);
        Vector3 vect_rotation = new Vector3(0, 0, theta * Mathf.Rad2Deg);
        Quaternion new_rotation = Quaternion.Euler(vect_rotation);
        transform.rotation = new_rotation;
    }

    private void TakeInput()
    {
        Vector3 movement_difference = new Vector3();
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
        transform.position += movement_difference;
    }

    // Update is called once per frame
    void Update()
    {
		TakeInput();
		ClampToBounds();
        PointToMouse();
    }
}
