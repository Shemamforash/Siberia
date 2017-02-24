using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinny : MonoBehaviour {
	private float rotation_speed;
	private int value = -1;
	private float time_alive = 0;

	private Player.states type;

	// Use this for initialization
	void Start () {
		rotation_speed = Random.Range(90f, 180f);
		if(Random.Range(0f, 1f) < 0.5f){
			rotation_speed *= -1;
		}
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 new_rot = transform.rotation.eulerAngles;
		new_rot.z += rotation_speed * Time.deltaTime;
		transform.rotation = Quaternion.Euler(new_rot);
		time_alive += Time.deltaTime * 2;
		float new_scale = (Mathf.Sin(time_alive) / 2) + 1.5f;
		transform.localScale = new Vector3(new_scale, new_scale, 0);
	}

	public void SetPickupValue(int value, Player.states type){
		this.value = value;
		this.type = type;
		transform.localScale *= Mathf.Sqrt(value) * 1 / 5;
	}

	void OnTriggerEnter2D(Collider2D other){
		if(value == -1){
			Debug.Log("Looks like you forgot to initialise this pickup properly!" + gameObject);
			Destroy(gameObject);
		}
		if(other.gameObject.name == "Player"){
			other.GetComponent<Player>().ReceivePickup(value, type);
			Destroy(gameObject);
		}
	}
}
