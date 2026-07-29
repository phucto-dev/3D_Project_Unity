using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "GameData/DialogueSO")]
public class DialogueSO : ScriptableObject
{
    public string NPCName;
    [TextArea(3, 5)]
    public string[] Sentences;
}
