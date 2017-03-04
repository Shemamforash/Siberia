using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedBehaviour : MonoBehaviour
{
	private Image speed_indicator;

    void Start()
    {
		speed_indicator = GetComponent<Image>();
    }

    public void UpdateSpeed(float max_speed, float current_speed)
    {
		float percent_max_speed = current_speed / max_speed;
		speed_indicator.fillAmount = percent_max_speed;
    }
}
