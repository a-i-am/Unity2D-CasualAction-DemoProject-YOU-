using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BossHPValue
{
    [SerializeField] private BossUI bossHP;

    [SerializeField] private float bossMaxVal;
    [SerializeField] private float bossCurrentVal;

    #region Boss HP Stat

    public float BossCurrentVal
    {
        get
        {
            return bossCurrentVal;
        }

        set
        {
            this.bossCurrentVal = Mathf.Clamp(value, 0, bossMaxVal);
            bossHP.BossValue = bossCurrentVal;
        }

    }

    public float BossMaxVal
    {
        get
        {
            return bossMaxVal;
        }
        set
        {
            this.bossMaxVal = value;
            bossHP.BossMaxHP = bossMaxVal;
        }
    }

    public void BossHPInitialize()
    {
        this.BossMaxVal = bossMaxVal;
        this.BossCurrentVal = bossCurrentVal;
    }
    #endregion
}
