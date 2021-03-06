﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Controls the behaviour of energy pickups.
 * Be sure to assign a value to the pickup or else it will actually subtract energy
 */
public class Spinny : MonoBehaviour
{
    private float rotation_speed;
    private int value = -1;
    private float time_alive = 0, magnet_range = 3, force_multiplier = 10;

    private Player.states type;

    //Has the pickup entered the pickup range of the player
    private bool in_pickup_range;

    private Rigidbody2D player_body;

    //Used in tutorial
    [SerializeField]
    private GameObject door;
    private GameObject player;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        rotation_speed = Random.Range(90f, 180f);
        if (Random.Range(0f, 1f) < 0.5f)
        {
            rotation_speed *= -1;
        }

        //Tutorial: Add pickup to door's list of things to check
        if (door != null)
        {
            door.GetComponent<TutorialDoorController>().addEnemy();
            value = 5;
        }
    }
	public void SetPickupValue(int value, Player.states type){
		this.value = value;
		this.type = type;
		transform.localScale *= Mathf.Sqrt(value) * 1 / 5;
	}

    // Update is called once per frame
    void Update()
    {
        //Rotate and grow/shrink
        Vector3 new_rot = transform.rotation.eulerAngles;
        new_rot.z += rotation_speed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(new_rot);
        float new_scale = (Mathf.Sin(time_alive) / 2) + 1.5f;
        transform.localScale = new Vector3(new_scale, new_scale, 0);

        //Decay over time
        time_alive += Time.deltaTime * 2;
        //Don't destroy pickup if linked to a door; required to open it
        if (door == null && time_alive > 10){
			Destroy(gameObject);
		}

        //Move towards player if entered pickup range
        if (in_pickup_range)
        {
            // Vector3 to_player = new Vector3(player_body.position.x - transform.position.x, player_body.position.y - transform.position.y, 0.0f);
            // transform.position = transform.position + to_player * chase_speed * Time.deltaTime;

            //Try me
            float distance_to_player = Vector3.Distance(transform.position, player.transform.position);
            if (distance_to_player < magnet_range)
            {
                float force = 1 - distance_to_player / magnet_range;
                force *= force_multiplier;
                Vector3 dir = player.transform.position - transform.position;
                transform.position += dir * force * Time.deltaTime;
            }
        }


    }

    

    void OnTriggerEnter2D(Collider2D other)
    {
        if (value == -1)
        {
            Debug.Log("Looks like you forgot to initialise this pickup properly!" + gameObject);
            Destroy(gameObject);
        }

        if (other.gameObject.tag == "Pickup_range")
        {
            in_pickup_range = true;
        }
        else if (other.gameObject.name == "Player")
        {
            other.GetComponent<Player>().ReceivePickup(value, type);

            //Tutorial: Remove pickup from door's list of things to check
            if (door != null)
            {
                door.GetComponent<TutorialDoorController>().removeEnemy();
            }

            Destroy(gameObject);
        }
    }
}
