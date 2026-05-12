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
        List<ItemInfo> items = _lootItemSO.GetRandomDrops();

        foreach (ItemInfo item in items)
        {
            if (item.ItemOrbDrop == null) continue;
            GameObject orbDrop = Instantiate(item.ItemOrbDrop, this.transform.position, Quaternion.identity);
            if (orbDrop != null)
            {
                DropInfo dropInfo = orbDrop.GetComponent<DropInfo>();
                if (dropInfo == null) continue;
                dropInfo.Initialize(item);
            }
        }
    }
}
