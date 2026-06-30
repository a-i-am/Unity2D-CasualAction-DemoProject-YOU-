using Assets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UI : MonoBehaviour
{
    private Player player;

    Inventory inven;

    [Header("HP")]

    [SerializeField] private Image playerHP;

    private float playerHPFillAmount;

    [SerializeField] private float lerpSpeed;
    [SerializeField] private Color fullColor;
    [SerializeField] private Color lowColor;
    [SerializeField] private bool lerpColors;



    public float PlayerMaxValue { get; set; }
    public float PlayerValue
    {
        get { return playerHPFillAmount; }
        set
        {
            if (PlayerMaxValue <= 0)
            {
                Debug.LogWarning("[HP] PlayerMaxValue is 0 or less. Aborting fillAmount calculation.");
                return;
            }
            playerHPFillAmount = Map(value, 0, PlayerMaxValue, 0, 1);
        }
    }




    [Header("Charge Casting Spell Gauage")]


    [SerializeField] private Image circularSpellGauge;
    [SerializeField] float currentChargeValue = 0;
    [SerializeField] double canChargeMaxValue = 1.0;
    [SerializeField] float gaugeChargeSpeed = 25;


    private GameObject skillPrefab;
    [SerializeField] private GameObject skillPrefabLevel1;
    [SerializeField] private GameObject skillPrefabLevel2;
    [SerializeField] private GameObject skillPrefabLevel3;
    [SerializeField] private GameObject skillPrefabLevel4;



    private GameObject originalProjectilePrefab;
    private GameObject currentProjectilePrefab;


    private GameObject currentAOEPrefab;

    public float skillDuration = 15f;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void Start()
    {
        inven = Inventory.Instance;
        playerHP.fillAmount = playerHPFillAmount;

        if (lerpColors)
        {
            playerHP.color = fullColor;
        }

        originalProjectilePrefab = player.projectilePrefab;
    }

    void Update()
    {
        HandlePlayerHpBar();
        ChargeSpellGauge();


        if (Input.GetKeyUp(KeyCode.X))
        {
            CastSpell();
        }
    }






    void HandlePlayerHpBar()
    {
        if (playerHPFillAmount != playerHP.fillAmount)
        {
            playerHP.fillAmount = Mathf.Lerp(playerHP.fillAmount, playerHPFillAmount, Time.deltaTime * lerpSpeed);
        }

        if (lerpColors)
        {
            playerHP.color = Color.Lerp(lowColor, fullColor, playerHPFillAmount);
        }
    }

    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }

    private void ChargeSpellGauge()
    {
        if (Input.GetKey(KeyCode.X) && circularSpellGauge.fillAmount < canChargeMaxValue)
        {
            currentChargeValue += gaugeChargeSpeed * Time.deltaTime;
        }
        else if(currentChargeValue > 0)
        {
            currentChargeValue -= gaugeChargeSpeed * Time.deltaTime;
        }

        if(circularSpellGauge.fillAmount >= 0)
            circularSpellGauge.fillAmount = currentChargeValue / 100;
    }

    private void CastSpell()
    {
        skillPrefab = null;

        if(circularSpellGauge.fillAmount >= 0.25f && circularSpellGauge.fillAmount < 0.5f)
        {

            skillPrefab = skillPrefabLevel1;
        }
        else if (circularSpellGauge.fillAmount >= 0.5f && circularSpellGauge.fillAmount < 0.75f)
        {

            skillPrefab = skillPrefabLevel2;
        }
        else if (circularSpellGauge.fillAmount >= 0.75f && circularSpellGauge.fillAmount < 1.0f)
        {

            skillPrefab = skillPrefabLevel3;
        }
        else if (circularSpellGauge.fillAmount >= 0.1f)
        {

            skillPrefab = skillPrefabLevel4;
        }

        if(skillPrefab != null)
        {

            currentChargeValue = 0;
            circularSpellGauge.fillAmount = 0;



                UseSkill(skillPrefab);

        }

    }
    private void UseSkill(GameObject newSkillPrefab)
    {


        if(newSkillPrefab == skillPrefabLevel1 || newSkillPrefab == skillPrefabLevel2)
        {
            currentProjectilePrefab = newSkillPrefab;
            player.projectilePrefab = currentProjectilePrefab;


            StartCoroutine(ResetProjectile(skillDuration));
        }

        else
        {
            currentAOEPrefab = newSkillPrefab;
            player.playerAOEPrefab = currentAOEPrefab;
            StartCoroutine(ResetAOE(skillDuration));
        }
    }

    private IEnumerator ResetProjectile(float delay)
    {
        yield return new WaitForSeconds(delay);


        player.projectilePrefab = originalProjectilePrefab;
    }

    private IEnumerator ResetAOE(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.playerAOEPrefab = null;
        player.isUseAOE = false;
        skillPrefab = null;
    }
}
