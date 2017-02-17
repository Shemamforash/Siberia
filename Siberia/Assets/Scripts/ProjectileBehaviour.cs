using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float x_max_bound = 20, x_min_bound = -20, y_max_bound = 20, y_min_bound = -20;
    private float time_alive = 0f;
	private float duration = 3f, speed = 12f;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
		time_alive += Time.deltaTime;
		if(time_alive > duration){
			Destroy(gameObject);
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

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<BasicEnemyController>().take_damage(10);
            Debug.Log("Hit target");
        }
        Destroy(gameObject);
    }
}
