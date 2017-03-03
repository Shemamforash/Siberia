using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInfoIcon : MonoBehaviour
{
    [SerializeField]
    private GameObject tutorialBox1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        tutorialBox1.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        tutorialBox1.SetActive(false);
    }
}
