using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBehaviour : MonoBehaviour
{
    private Image shield_image;
    private float alpha_falloff = 0.8f;
    private bool faded = true;

    void Start()
    {
        shield_image = GetComponent<Image>();
    }

    public void RefreshShield()
    {
        shield_image.color = new Color(1, 1, 1, 1);
        faded = false;
    }

    void Update()
    {
        if (!faded)
        {
            float alpha = shield_image.color.a;
            alpha -= alpha_falloff * Time.deltaTime;
            if (alpha < 0)
            {
                faded = true;
                alpha = 0;
            }
			shield_image.color = new Color(1, 1, 1, alpha);
        }
    }
}
