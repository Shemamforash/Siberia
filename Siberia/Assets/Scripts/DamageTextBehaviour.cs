using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextBehaviour : MonoBehaviour
{

    public float duration = 2, time_alive = 0;

    private float v_x, v_y, g = -9.8f, anim_speed = 10;

    void Start()
    {
		g *= anim_speed;
        v_x = Random.Range(-6, 6f) * anim_speed;
        v_y = Random.Range(-10, 10f) * anim_speed;
    }
    // Update is called once per frame
    void Update()
    {
        time_alive += Time.deltaTime;
        if (time_alive > duration)
        {
            Destroy(gameObject);
        }
        else
        {
            float alpha = (1f / duration * time_alive);
			alpha *= alpha;
			alpha = 1 - alpha;
            Color current_col = GetComponent<Text>().color;
            current_col.a = alpha;
            GetComponent<Text>().color = current_col;

            //s = ut + 1/2at^2
            float t_squared = Time.deltaTime * Time.deltaTime;
            float a_t_squared = 0.5f * t_squared * g;
            float y_dist = v_y * Time.deltaTime + a_t_squared;
            v_y = v_y + g * Time.deltaTime;
            float x_dist = Time.deltaTime * v_x;
            Vector3 pos_diff = new Vector3(x_dist, y_dist, 0);
            transform.position += pos_diff;
            // transform.position += new Vector3(0, 10 * Time.deltaTime, 0);
        }
    }
}
