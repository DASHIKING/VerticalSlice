using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    public static bool IsFlashlightOn = true; // 静态变量，任何脚本都能访问

    private Light flashlight;

    void Start()
    {
        flashlight = GetComponent<Light>();
        IsFlashlightOn = flashlight.enabled;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlight.enabled = !flashlight.enabled;
            IsFlashlightOn = flashlight.enabled; // 同步更新静态变量
        }
    }
}