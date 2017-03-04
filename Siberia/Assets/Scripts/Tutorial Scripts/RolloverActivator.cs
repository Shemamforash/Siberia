using UnityEngine;
using System.Collections;

/*
 * Generic script for objects that activate other objects when the player passes over them
 */
public class RolloverActivator : MonoBehaviour
{
    //Object to activate
    [SerializeField]
    private GameObject obj;
    //Only want to activate it once
    bool activated = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        //Make sure that it is the player that has passed over
        if(!activated && other != null && other.gameObject.tag == "Player")
        {
            obj.SetActive(true);
            activated = true;
        }
    }
}
