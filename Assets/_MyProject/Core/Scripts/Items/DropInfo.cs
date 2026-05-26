using UnityEngine;

public class DropInfo : MonoBehaviour
{
    public ItemInstance ItemData { get; private set; }

    public void Initialize(ItemInstance itemInstance)
    {
        ItemData = itemInstance;
    }

    public void SetAmount(int amount)
    {
        if (ItemData == null) return;
        ItemData.Amount = amount;
    }

    public void RemovePrefab()
    {
        gameObject.SetActive(false);
    }
}
