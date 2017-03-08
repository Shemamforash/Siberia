using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Only to be used in the tutorial level
 * Other levels will not contain a player with a tutorial script, so you'll get an error
 */
public class RolloverController : MonoBehaviour
{
    public enum tutorial_states
    {
        movement,
        dark_locked,
        light_locked,
        full_control
    }
    [SerializeField]
    private tutorial_states rollover_mode;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Player")
        {
            //ONLY WORKS IN TUTORIAL
            TutorialPlayer tPlayerController = collider.gameObject.GetComponent<TutorialPlayer>();

            switch(rollover_mode)
            {
                case tutorial_states.dark_locked:
                    tPlayerController.switch_to_dark();
                    break;
                case tutorial_states.light_locked:
                    tPlayerController.switch_to_light();
                    break;
                default:
                case tutorial_states.full_control:
                    tPlayerController.activate_full_control();
                    break;
            }
        }
    }
}
