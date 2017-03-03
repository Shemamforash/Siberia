using UnityEngine;
using System.Collections;

/*
 * Door for the tutorial that opens a dialog box when the player walks into it
 */
public class DialogDoorController : MonoBehaviour
{
    [SerializeField]
    private GameObject skipTutorialDialog;

    public void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            skipTutorialDialog.SetActive(true);
        }
    }

    public void OpenDoor()
    {
        Destroy(gameObject);
    }
}
