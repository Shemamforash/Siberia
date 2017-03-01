using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {
	public Transform player_transform;

    void Update()
    {
        Vector3 new_pos = player_transform.position;
        new_pos.z = -10;
        transform.position = new_pos;
    }
}
