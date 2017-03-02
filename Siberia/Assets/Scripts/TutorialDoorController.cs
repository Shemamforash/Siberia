using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDoorController : MonoBehaviour
{

    private int enemies_left = 0;

	// Use this for initialization
	void Start ()
    {
        //enemies_left = 0;
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("Enemies on door: " + enemies_left);
	}

    public void addEnemy()
    {
        enemies_left++;
    }

    public void removeEnemy()
    {
        enemies_left--;
        if(enemies_left <= 0)
        {
            Destroy(gameObject);
        }
    }

}
