using UnityEngine;
using System.Collections;

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
