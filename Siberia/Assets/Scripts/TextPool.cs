using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPool : MonoBehaviour
{
    private List<GameObject> text_pool = new List<GameObject>();
    private List<GameObject> active_text = new List<GameObject>();
    public GameObject text_prefab;
    private int pool_size = 5;
    private GameObject canvas_object;


    // Use this for initialization
    void Start()
    {
        canvas_object = GameObject.Find("Canvas");
        for (int i = 0; i < pool_size; ++i)
        {
            GameObject new_text = GameObject.Instantiate(text_prefab, transform.position, transform.rotation);
            // new_text.SetActive(false);
            text_pool.Add(new_text);
            // new_text.GetComponent<DamageTextBehaviour>().SetParentPool(this);
            new_text.transform.SetParent(canvas_object.transform);
        }
    }

    public void SpurtText(int damage_amount)
    {
        if (text_pool.Count > 1)
        {
            GameObject pulled_object = text_pool[0];
            text_pool.RemoveAt(0);
            active_text.Add(pulled_object);
            // pulled_object.GetComponent<DamageTextBehaviour>().Setup(damage_amount);
            // pulled_object.SetActive(true);
            pulled_object.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        }
    }

    public void Deactivate(GameObject text)
    {
        active_text.Remove(text);
        text_pool.Add(text);
        // text.SetActive(false);
    }
}
