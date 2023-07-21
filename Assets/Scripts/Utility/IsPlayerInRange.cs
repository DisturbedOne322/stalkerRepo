using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPlayerInRange : MonoBehaviour
{
    private PlayerMovement player;

    [SerializeField]
    private float desiredDistance;

    public event Action<bool> OnPlayerInRange;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        StartCoroutine(EvaluatePlayerInRange());
    }

    private WaitForSeconds delay = new WaitForSeconds(0.01f);

    private IEnumerator EvaluatePlayerInRange()
    {
        while(true)
        {
            OnPlayerInRange?.Invoke(Vector2.Distance(transform.position, player.transform.position) < desiredDistance);
            yield return delay;
        }
    }
}
