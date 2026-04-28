using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    private Light flashlight; // 引用手电筒的 Light 组件

    void Start()
    {
        // 自动获取当前物体上的 Light 组件
        flashlight = GetComponent<Light>();

        // 默认让手电筒处于开启状态，如果你想默认关闭，可以改为 false
        // flashlight.enabled = true; 
    }

    void Update()
    {
        // 检查玩家是否按下了 F 键
        if (Input.GetKeyDown(KeyCode.F))
        {
            // 切换状态：如果开着就关掉，如果关着就开启
            flashlight.enabled = !flashlight.enabled;

            // 可选：在这里添加开关灯的声音效果
            // AudioSource.PlayClipAtPoint(switchSound, transform.position);
        }
    }
}