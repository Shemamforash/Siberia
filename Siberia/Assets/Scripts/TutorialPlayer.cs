using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayer : Player
{
    public enum tutorial_states
    {
        movement,
        dark_locked,
        light_locked,
        full_control
    }
    private tutorial_states current_tutorial_state;

	// Use this for initialization
	void Start ()
    {
        init();

        //Start at half health to demonstrate effect of pickups
        current_health /= 2;

        //Start by only being able to move
        current_tutorial_state = tutorial_states.movement;
	}

    void Update()
    {
        TakeInput();
        PointToMouse();

        //Don't shoot if in movement only mode
        if (current_tutorial_state != tutorial_states.movement)
        {
            TakeMouse();
        }

        UpdateMeters();
        UpdateWeaponCooldowns();
        UpdateColour();
        UpdateLightPushBack();
        UpdateDamageBar();
        FadeOut();
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
        
        //Only allow switching in the full control state
        if (current_tutorial_state == tutorial_states.full_control && (Input.GetKeyUp("space") || Input.GetMouseButtonUp(1)))
        {
            ToggleState();
        }
    }

    public void switch_to_dark()
    {
        current_tutorial_state = tutorial_states.dark_locked;
        current_state = states.dark;
    }

    public void switch_to_light()
    {
        current_tutorial_state = tutorial_states.light_locked;
        current_state = states.light;
    }

    public void activate_full_control()
    {
        current_tutorial_state = tutorial_states.full_control;
    }
}
