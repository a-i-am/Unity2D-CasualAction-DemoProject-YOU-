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
    // Inventory
    Inventory inven;

    [Header("HP")]
    // HP
    [SerializeField] private Image playerHP;
    
    private float playerHPFillAmount;

    [SerializeField] private float lerpSpeed;
    [SerializeField] private Color fullColor;
    [SerializeField] private Color lowColor;
    [SerializeField] private bool lerpColors;


    // Player HP Value
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
    // Charge Casting Spell Gauage
    //[SerializeField] TextMeshProUGUI pickupMobCountText;
    [SerializeField] private Image circularSpellGauge;
    [SerializeField] float currentChargeValue = 0;
    [SerializeField] double canChargeMaxValue = 1.0; // 최대 게이지 값
    [SerializeField] float gaugeChargeSpeed = 25;

    // 단계별 스킬 프리팹들
    private GameObject skillPrefab;
    [SerializeField] private GameObject skillPrefabLevel1;
    [SerializeField] private GameObject skillPrefabLevel2;
    [SerializeField] private GameObject skillPrefabLevel3;
    [SerializeField] private GameObject skillPrefabLevel4;


    // Projectile : 발사체
    private GameObject originalProjectilePrefab; // 기본공격용 projectilePrefab
    private GameObject currentProjectilePrefab; // 현재 사용 중인 projectilePrefab

    // AOE : Area of Effect // 범위 공격
    private GameObject currentAOEPrefab; // 현재 사용 중인 AOEPrefab

    public float skillDuration = 15f; // 스킬 지속 시간

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
        // 초기화
        originalProjectilePrefab = player.projectilePrefab; // 원래 projectilePrefab 초기화
    }

    void Update()
    {
        HandlePlayerHpBar();
        ChargeSpellGauge();

        // 스킬 사용 체크
        if (Input.GetKeyUp(KeyCode.X))
        {
            CastSpell();
        }
    }

    //void FixedUpdate()
    //{
    //    //pickupMobCountText.text = string.Format("{0}", inven.pickupMobCount);
    //}

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
            // 1단계 스킬
            skillPrefab = skillPrefabLevel1;
        }
        else if (circularSpellGauge.fillAmount >= 0.5f && circularSpellGauge.fillAmount < 0.75f)
        {
            // 2단계 스킬
            skillPrefab = skillPrefabLevel2;
        }
        else if (circularSpellGauge.fillAmount >= 0.75f && circularSpellGauge.fillAmount < 1.0f)
        {
            // 3단계 스킬
            skillPrefab = skillPrefabLevel3;
        }
        else if (circularSpellGauge.fillAmount >= 0.1f)
        {
            // 4단계 스킬 (궁극기)
            skillPrefab = skillPrefabLevel4;
        }

        if(skillPrefab != null)
        {
            // 스킬 발동 후 게이지 초기화
            currentChargeValue = 0;
            circularSpellGauge.fillAmount = 0;

            //if (skillPrefab.GetComponent<Projectile>() != null)
            //{
                UseSkill(skillPrefab);
            //}
        }

    }
    private void UseSkill(GameObject newSkillPrefab)
    {
        // 스킬 1,2는 발사체 스킬
        // 현재 projectilePrefab을 원래의 것으로 저장
        if(newSkillPrefab == skillPrefabLevel1 || newSkillPrefab == skillPrefabLevel2)
        {
            currentProjectilePrefab = newSkillPrefab;
            player.projectilePrefab = currentProjectilePrefab;
            // projectilePrefab을 동적으로 등록된 프리팹으로 변경
            // 코루틴을 호출하여 일정 시간 후에 원래의 projectilePrefab으로 되돌리기
            StartCoroutine(ResetProjectile(skillDuration));
        }
        // 스킬 3, 4
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

        // 원래 projectilePrefab으로 되돌리기
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
