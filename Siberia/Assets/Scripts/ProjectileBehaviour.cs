using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    private float time_alive = 0f;
	public float duration = 3f, speed = 2f;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
		time_alive += Time.deltaTime;
		if(time_alive > duration){
			Destroy(gameObject);
		}
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
            collision.gameObject.GetComponent<BasicEnemyController>().take_damage(10);
        Destroy(gameObject);
    }
}
