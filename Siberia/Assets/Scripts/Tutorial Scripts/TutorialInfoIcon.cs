using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Attach to rollover icons to display information dialogs that provide helpful (?) tips
 */
public class TutorialInfoIcon : MonoBehaviour
{
    [SerializeField]
    private GameObject tutorialBox1;
    [SerializeField]
    private GameObject tutorialBox2;
    [SerializeField]
    private GameObject tutorialBox3;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (tutorialBox1 != null)
                tutorialBox1.SetActive(true);

            if (tutorialBox2 != null)
                tutorialBox2.SetActive(true);

            if (tutorialBox3 != null)
                tutorialBox3.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (tutorialBox1 != null)
                tutorialBox1.SetActive(false);

            if (tutorialBox2 != null)
                tutorialBox2.SetActive(false);

            if (tutorialBox3 != null)
                tutorialBox3.SetActive(false);
        }
    }
}
