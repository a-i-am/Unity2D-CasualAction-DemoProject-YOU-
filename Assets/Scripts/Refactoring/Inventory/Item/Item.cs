using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int itemID;

    public int GetItemID()
    {
        return itemID;
    }

    public void DestroyItem()
    {
        Destroy(gameObject);
    }    
}
