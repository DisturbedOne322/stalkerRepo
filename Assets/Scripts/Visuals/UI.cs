using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//FIX PLAYER HP UI

public class UI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI magazineBulletCountText;

    [SerializeField]
    private TextMeshProUGUI weaponJammedNotification;

    [SerializeField]
    private Slider staminaSlider;

    [SerializeField]
    private Slider healthBarSlider;

    [SerializeField]
    private Slider headlightCapacitySlider;

    [SerializeField]
    private PlayerMovement player;

    [SerializeField]
    private FocusedHeadlight focusedHeadlight;

    private float notificationEnabledTimer;
    private float notificationEnabledTimerTotal = 2f;

    //magazine UI
    [SerializeField]
    private Image[] bulletImages;

    //health UI
    [SerializeField]
    private Sprite[] sanitySprites;
    [SerializeField] 
    private Image sanityImage;

    [SerializeField]
    private TextMeshProUGUI healthText;


    //UI Materials
    #region StaminaBar
    [SerializeField]
    private Material staminaBarMaterial;
    private const float staminaBarMaterialDepleteSpeed = 1f;
    private const float staminaBarMaterialRegenSpeed = -3f;
    #endregion
    #region HealthBar
    [SerializeField]
    private Material healthBarMaterial;
    private const string HEALTH_BAR_OUTLINE_THICKNESS = "_OutlineThickness";
    private float healthBarRestoreTime = 0;
    private const float damageHealthBarOutlineThickness = 7;
    private const float defaultHealthBarOutlineThickness = 0;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Shoot.OnSuccessfulShoot += Shoot_OnSuccessfulShoot;
        Shoot.OnWeaponJammed += Shoot_OnWeaponJammed;
        Shoot.OnSuccessfulReload += Shoot_OnSuccessfulReload;
        player.OnHealthChanged += Player_OnHealthChanged;
        player.OnStaminaStateChange += Player_OnStaminaStateChange;
    }

    private void Player_OnStaminaStateChange(PlayerMovement.StaminaState obj)
    {
        float speed = -1;
        switch (obj)
        {
            case PlayerMovement.StaminaState.Regen:
                speed = staminaBarMaterialRegenSpeed;
                break;
            case PlayerMovement.StaminaState.Deplete:
                speed = staminaBarMaterialDepleteSpeed;
                break;

        }
        staminaBarMaterial.SetFloat("_ScrollingSpeed", speed);
    }

    private void Player_OnHealthChanged(GameManager.PlayerHealthStatus hpStatus)
    {
        healthBarMaterial.SetFloat(HEALTH_BAR_OUTLINE_THICKNESS, damageHealthBarOutlineThickness);
        healthBarRestoreTime = 0f;
        switch (hpStatus)
        {
            case GameManager.PlayerHealthStatus.HighHP:
                sanityImage.sprite = sanitySprites[0];
                break;
            case GameManager.PlayerHealthStatus.MidHP:
                sanityImage.sprite = sanitySprites[1];
                break;
            case GameManager.PlayerHealthStatus.LowHP:
                sanityImage.sprite = sanitySprites[2];
                break;
        }

        healthText.text = (player.HealthPoints * 1.0f / player.MaxHealthPoint * 100) + "%";
    }

    private void Shoot_OnSuccessfulReload(int magSize)
    {
        magazineBulletCountText.text = magSize + "/" + magSize;
        for(int i = 0;i < bulletImages.Length; i++)
        {
            bulletImages[i].enabled = true;
        }
    }

    private void Shoot_OnWeaponJammed()
    {
        weaponJammedNotification.enabled = true;
        notificationEnabledTimer = notificationEnabledTimerTotal;
    }

    private void Shoot_OnSuccessfulShoot(int bullets, int magSize)
    {
        magazineBulletCountText.text = bullets + "/" + magSize;
        bulletImages[magSize - bullets - 1].enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        healthBarRestoreTime += Time.deltaTime;
        healthBarMaterial.SetFloat(HEALTH_BAR_OUTLINE_THICKNESS, 
            Mathf.Lerp(healthBarMaterial.GetFloat(HEALTH_BAR_OUTLINE_THICKNESS), 
            defaultHealthBarOutlineThickness, healthBarRestoreTime ));

        staminaSlider.value = player.Stamina;

        headlightCapacitySlider.value = focusedHeadlight.CurrentFocusedLightCapacity;

        notificationEnabledTimer -= Time.deltaTime;
        if(notificationEnabledTimer < 0 )
        {
            weaponJammedNotification.enabled = false;
        }
    }
}
