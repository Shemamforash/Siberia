using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicEnemyController : MonoBehaviour {

    [SerializeField]
    private float move_speed = 1;
    [SerializeField]
    private int enemy_HP = 100;
    [SerializeField]
    public GameObject player_object;

    private Transform enemy_transform;
    private Transform player_transform;

    public GameObject damage_text;
    private GameObject canvas_object;

    // Use this for initialization
    void Start () {
        canvas_object = GameObject.Find("Canvas");
        enemy_transform = gameObject.transform;
        player_transform = player_object.transform;
        GameController.RegisterEnemy(gameObject);
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
        Vector2 enemy_screen_location = Camera.main.WorldToScreenPoint(transform.position);
        GameObject new_damage = GameObject.Instantiate(damage_text, enemy_screen_location, Quaternion.Euler(Vector3.up));
        new_damage.transform.SetParent(canvas_object.transform);
        new_damage.GetComponent<Text>().text = dmg.ToString();
        if(enemy_HP <= 0)
        {
            GameController.UnregisterEnemey(gameObject);
            Destroy(gameObject);
        }
    }
}
