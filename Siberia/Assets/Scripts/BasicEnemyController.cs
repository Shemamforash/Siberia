using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour {

    [SerializeField]
    private float move_speed = 1;
    [SerializeField]
    private int enemy_HP = 5;
    [SerializeField]
    public GameObject player_object;

    private Transform enemy_transform;
    private Transform player_transform;

    // Use this for initialization
    void Start () {
        enemy_transform = gameObject.transform;
        player_transform = player_object.transform;
    }
	
	// Update is called once per frame
	void Update (){
        Vector3 dirToPlayer = player_transform.position - enemy_transform.position;
        dirToPlayer.Normalize();

        enemy_transform.position += dirToPlayer * move_speed * Time.deltaTime;
        Face_direction(dirToPlayer);
	}

    private void Face_direction(Vector3 direction)
    {
        Vector3 reference_dir = new Vector3(0, 1, 0);
        float dot_product = Vector3.Dot(reference_dir, direction);
        float theta = Mathf.Acos(dot_product) * Mathf.Rad2Deg;
        if (direction.x < 0)
        {
            theta = 360 - theta;
        }
        Vector3 vect_rotation = new Vector3(0, 0, 360 - theta);
        Quaternion new_rotation = Quaternion.Euler(vect_rotation);
        transform.rotation = new_rotation;
    }

    public void take_damage(int dmg)
    {
        enemy_HP -= dmg;
        if(enemy_HP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
