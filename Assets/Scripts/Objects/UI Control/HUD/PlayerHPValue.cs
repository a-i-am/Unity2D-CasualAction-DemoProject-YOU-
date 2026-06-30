using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerHPValue
{
    [SerializeField] private UI playerHP;

    [SerializeField] private float playerMaxVal;
    [SerializeField] private float playerCurrentVal;



    #region Player HP Stat

    public float PlayerCurrentVal
    {
        get
        {
            return playerCurrentVal;
        }

        set
        {
            this.playerCurrentVal = Mathf.Clamp(value, 0, playerMaxVal);
            playerHP.PlayerValue = playerCurrentVal;
        }

    }

    public float PlayerMaxVal
    {
        get
        {
            return playerMaxVal;
        }
        set
        {
            this.playerMaxVal = value;
            playerHP.PlayerMaxValue = playerMaxVal;
        }
    }

    public void PlayerHPInitialize()
    {
        this.PlayerMaxVal = playerMaxVal;
        this.PlayerCurrentVal = playerCurrentVal;
    }

    #endregion


}
