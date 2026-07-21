using System.Collections.Generic;
using UnityEngine;

public class FieldItemSpawner
{
    public static void Spawn(GameObject prefab, Vector3[] positions, List<Item.ItemData> items)
    {
        if (prefab == null || positions == null || items == null || items.Count == 0) return;

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject go = Object.Instantiate(prefab, positions[i], Quaternion.identity);
            FieldItems fieldItem = go.GetComponent<FieldItems>();
            if (fieldItem != null) fieldItem.SetItem(items[Random.Range(0, items.Count)]);
        }
    }
}
