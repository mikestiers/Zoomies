using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : Singleton<HUDManager>
{
    public Text speedText;
    public Text gearText;
    public GameObject speedometerNeedle;

    public void UpdateSpeedometer(int gear, float speed)
    {
        gearText.text = gear.ToString();
        speedText.text = ((int) speed).ToString();
        float z = Mathf.Clamp(90 - speed, 0, 180);
        speedometerNeedle.transform.eulerAngles = new Vector3(0, 0, z);
    }
}
