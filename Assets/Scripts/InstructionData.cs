using UnityEngine;

// 这一行让你可以像创建材质球一样创建这个数据文件
[CreateAssetMenu(fileName = "NewInstruction", menuName = "GDIM/InstructionData")]
public class InstructionData : ScriptableObject
{
    public string Title;       
    [TextArea(10, 20)]         
    public string Content;     
}