using UnityEngine;
using System.Collections;

public class SniperProjectileBehaviour : MonoBehaviour
{
    [SerializeField]
    private float sniper_bullet_duration = 3.0f;
    [SerializeField]
    private float sniper_bullet_speed = 30f;

    private float lifetime;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * Time.deltaTime * sniper_bullet_speed;

        lifetime += Time.deltaTime;
        if (lifetime > sniper_bullet_duration)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Hit the player bastard!");
        }
        Destroy(gameObject);
    }
}
