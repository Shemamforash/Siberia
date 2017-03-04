using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Generic class for objects that shouls fade out of existance over time.
 * Essentially just a nicer looking destroy command.
 * Used in the tutorial for hiding extra rooms before unlocking them.
 */
public class Fadeout : MonoBehaviour
{
    private float fadeout_counter;
    private bool activated;

    //Control the opacity of the sprite to draw the fadeout
    private SpriteRenderer sprite;

	void Start()
    {
        //Set the countdown
        fadeout_counter = 1.0f;
        //Doesn't start counting down
        activated = false;

        sprite = GetComponent<SpriteRenderer>();
	}
	
	void Update()
    {
		if(activated)
        {
            fadeout_counter -= Time.deltaTime;

            if(fadeout_counter <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                //Reduce the alpha of the sprite
                sprite.color = new Color(0.0f, 0.0f, 0.0f, fadeout_counter);
            }
        }
	}

    public void StartCountdown()
    {
        activated = true;
    }
}
