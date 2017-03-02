using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDoorController : MonoBehaviour
{

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
            Destroy(gameObject);
        }
    }

}
