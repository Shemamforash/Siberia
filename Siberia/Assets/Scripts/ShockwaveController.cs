using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveController : MonoBehaviour {

    [SerializeField]
    private float initialSpeed;

    private float speed;

    private void Start()
    {
        speed = initialSpeed;
    }

    private void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
        speed = Mathf.Max(speed - Time.deltaTime * 1.0f * initialSpeed, initialSpeed * 0.01f);
    }

	public void destroySelf()
    {
        Destroy(gameObject);
    }
}
