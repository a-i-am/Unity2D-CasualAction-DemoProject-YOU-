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
    private static bool hasSavedPlayerCurrentVal;
    private static float savedPlayerCurrentVal;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSavedPlayerHP()
    {
        hasSavedPlayerCurrentVal = false;
        savedPlayerCurrentVal = 0f;
    }

    //public float MaxVal { get => maxVal; set => maxVal = value; }

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
            savedPlayerCurrentVal = this.playerCurrentVal;
            hasSavedPlayerCurrentVal = true;
            EnsurePlayerHP();
            if (playerHP != null) playerHP.PlayerValue = playerCurrentVal;
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
            EnsurePlayerHP();
            if (playerHP != null) playerHP.PlayerMaxValue = playerMaxVal;
        }
    }

    public void PlayerHPInitialize()
    {
        this.PlayerMaxVal = playerMaxVal;
        this.PlayerCurrentVal = hasSavedPlayerCurrentVal ? savedPlayerCurrentVal : playerCurrentVal;
    }

    private void EnsurePlayerHP()
    {
        if (playerHP == null) playerHP = UnityEngine.Object.FindObjectOfType<UI>(true);
    }

    #endregion


}
