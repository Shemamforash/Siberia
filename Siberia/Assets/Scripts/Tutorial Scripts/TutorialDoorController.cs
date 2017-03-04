using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDoorController : MonoBehaviour
{
    [SerializeField]
    private GameObject blackout_obj;

    private int enemies_left = 0;

    public void addEnemy()
    {
        enemies_left++;
    }

    public void removeEnemy()
    {
        enemies_left--;
        if(enemies_left <= 0)
        {
            //Blackout overlay isn't strictly needed, so check that it has been assigned first
            if(blackout_obj != null)
                blackout_obj.GetComponent<Fadeout>().StartCountdown();
            //Open door
            Destroy(gameObject);
        }
    }

}
