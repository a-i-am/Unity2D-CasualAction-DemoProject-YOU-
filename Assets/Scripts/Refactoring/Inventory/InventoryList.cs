using System;
using System.Collections.Generic;

public class InventoryList<T>
{
    public List<T> List { get; } = new List<T>();
    public int Acquired { get; private set; }

    private int _slotCount;
    public int SlotCount
    {
        get => _slotCount;
        set
        {
            _slotCount = value;
            OnSlotCountChange?.Invoke(_slotCount);
        }
    }

    public event Action OnChange;
    public event Action<int> OnSlotCountChange;

    public bool Add(T item)
    {
        if (List.Count >= _slotCount) return false;
        List.Add(item);
        Acquired++;
        OnChange?.Invoke();
        return true;
    }

    public void Remove(T item)
    {
        if (List.Remove(item))
        {
            Acquired--;
            OnChange?.Invoke();
        }
    }

    public void RemoveAt(int index)
    {
        if (index >= 0 && index < List.Count)
        {
            List.RemoveAt(index);
            Acquired--;
            OnChange?.Invoke();
        }
    }

    public void ForceUpdate()
    {
        OnChange?.Invoke();
    }
}
