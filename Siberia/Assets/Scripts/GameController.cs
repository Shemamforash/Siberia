using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public Transform player_transform;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 new_pos = player_transform.position;
		new_pos.z = -10;
		transform.position = new_pos;
	}
}
