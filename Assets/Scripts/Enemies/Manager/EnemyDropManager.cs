using System.Collections.Generic;
using UnityEngine;

public class EnemyDropManager : MonoBehaviour
{
    [SerializeField] private LootItemSO _lootItemSO;

    private void Awake()
    {

    }
    public void ExecuteDrop()
    {
        if (_lootItemSO == null) return;
        List<ItemInstance> items = _lootItemSO.GetRandomDrops();

        foreach (ItemInstance item in items)
        {
            if (item.ItemDefinition == null) continue;
            if (item.DropPrefab == null)
            {
                Debug.LogWarning("EnemyDropManager: DropPrefab == null");
                continue;
            }
            GameObject orbDrop = Instantiate(item.DropPrefab, this.transform.position, Quaternion.identity);
            if (orbDrop != null)
            {
                DropInfo dropInfo = orbDrop.GetComponent<DropInfo>();
                if (dropInfo == null) continue;
                dropInfo.Initialize(item);
            }
        }
    }
}
