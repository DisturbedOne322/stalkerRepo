using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPlayer : MonoBehaviour
{
    private PlayerMovement player;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private ApproachPlayer approachPlayer;

    private bool canShoot = true;
    private float shootCD = 2;
    private float shootDistance = 20f;

    private RaycastHit2D hit;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        approachPlayer.OnPlayerInRange += ApproachPlayer_OnPlayerInRange;
    }

    private void ApproachPlayer_OnPlayerInRange(bool inRange)
    {
        if (!canShoot)
            return;

        if(inRange)
            TryShootPlayer();
    }

    private void TryShootPlayer()
    {
        if (player == null)
            return;

        hit = Physics2D.Raycast(transform.position, transform.right, shootDistance, layerMask);
        if(hit)
        {
            player.GetDamaged(1);
            canShoot = false;
            StartCoroutine(ShootCooldown(shootCD));
        }
    }

    private IEnumerator ShootCooldown(float cd)
    {
        yield return new WaitForSeconds(cd);
        canShoot = true;
    }
}
