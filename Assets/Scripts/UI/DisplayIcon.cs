using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayIcon : MonoBehaviour
{
    [SerializeField]
    private Sprite icon;

    private Image iconRenderer;

    private IsPlayerInRange playerInRange;

    private float maxAlpha = 1;
    private float minAlpha = 0;

    float smDampVelocity;
    private float smDampSpeed = 0.05f;

    private void Start()
    {
        iconRenderer = GetComponent<Image>();
        iconRenderer.sprite = icon;
        playerInRange = GetComponentInParent<IsPlayerInRange>();
        playerInRange.OnPlayerInRange += PlayerInRange_OnPlayerInRange;
    }

    private void PlayerInRange_OnPlayerInRange(bool inRange)
    {
        float targetAlpha = inRange ? maxAlpha : minAlpha;

        Color tempColor = iconRenderer.color;
        tempColor.a = Mathf.SmoothDamp(tempColor.a, targetAlpha, ref smDampVelocity, smDampSpeed);

        iconRenderer.color = tempColor;
    }
}
