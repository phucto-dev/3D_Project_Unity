using UnityEngine;

public class DropInfo : MonoBehaviour
{
    public ItemInstance ItemData { get; private set; }

    public void Initialize(ItemInstance itemInstance)
    {
        ItemData = itemInstance;
    }
}
