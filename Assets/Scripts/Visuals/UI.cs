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
    int bulletsShot = 0;

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
    private float delayBetweenBulletShow = 0.15f;

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
        StopAllCoroutines();
        StartCoroutine(ReappearBullets(magSize));
        bulletsShot = 0;
    }

    private void Shoot_OnWeaponJammed()
    {
        weaponJammedNotification.enabled = true;
        notificationEnabledTimer = notificationEnabledTimerTotal;
    }

    private void Shoot_OnSuccessfulShoot(int bullets, int magSize)
    {
        magazineBulletCountText.text = bullets + "/" + magSize;
        StartCoroutine(HideBullet(magSize - bullets - 1));
        bulletsShot++;
    }

    IEnumerator HideBullet(int bulletImageIndex)
    {
        for(float i = 1; i > 0; i-= 0.1f) 
        {
            Color temp = bulletImages[bulletImageIndex].color;
            temp.a = i;
            bulletImages[bulletImageIndex].color = temp;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator ReappearBullets(int magSize)
    {
        //enable all the bullets that are left in the mag
        for (int i = 0; i < magSize - bulletsShot; i++)
        {
            Color temp = bulletImages[i].color;
            temp.a = 1;
            bulletImages[i].color = temp;
        }
        for (int i = magSize - bulletsShot; i < magSize; i++)
        {
            StartCoroutine(ShowBullet(i));
        }
        yield return null;
    }

    IEnumerator ShowBullet(int bulletImageIndex)
    {
        for (float i = 0; i <= 1; i += 0.1f)
        {
            Color temp = bulletImages[bulletImageIndex].color;
            temp.a = i;
            bulletImages[bulletImageIndex].color = temp;
            yield return new WaitForSeconds(0.05f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthBarRestoreTime += Time.deltaTime;
        healthBarMaterial.SetFloat(HEALTH_BAR_OUTLINE_THICKNESS,
            Mathf.Lerp(healthBarMaterial.GetFloat(HEALTH_BAR_OUTLINE_THICKNESS),
            defaultHealthBarOutlineThickness, healthBarRestoreTime));

        staminaSlider.value = player.Stamina;

        headlightCapacitySlider.value = focusedHeadlight.CurrentFocusedLightCapacity;

        notificationEnabledTimer -= Time.deltaTime;
        if (notificationEnabledTimer < 0)
        {
            weaponJammedNotification.enabled = false;
        }
    }
}
