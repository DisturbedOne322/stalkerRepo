using System.Collections;
using UnityEngine;

public class DissolveOnDeath : MonoBehaviour, IDefeatable
{
    [SerializeField]
    private float dissolveAmountPerTick;
    [SerializeField]
    private float dissolveTickTime;
    [SerializeField]
    private float minDissolve = 0;
    [SerializeField]
    private float maxDissolve = 1;

    private const string DISSOLVE_AMOUNT = "_dissolveAmount";

    private Material[] dissolveMaterialArray;
    [SerializeField]
    private SpriteRenderer[] spriteRendererArray;

    private IDamagable damagable;

    private void Start()
    {
        damagable = GetComponent<IDamagable>();
        damagable.OnDeath += Damagable_OnDeath;

        dissolveMaterialArray = new Material[spriteRendererArray.Length];
        for(int i = 0; i < spriteRendererArray.Length; i++)
        {
            dissolveMaterialArray[i] = spriteRendererArray[i].material;
        }
        SetDissolveOnRenderer(maxDissolve);    
    }

    private void OnDestroy()
    {
        damagable.OnDeath -= Damagable_OnDeath;
    }

    private void Damagable_OnDeath()
    {
        Die();
    }

    public void Die()
    {
        StartCoroutine(Dissolve());
    }

    private IEnumerator Dissolve()
    {
        for (float i = 1; i > minDissolve; i -= dissolveAmountPerTick)
        {
            SetDissolveOnRenderer(i);
            yield return new WaitForSeconds(dissolveTickTime);
        }
        gameObject.SetActive(false);
    }

    private void SetDissolveOnRenderer(float amount)
    {
        for (int i = 0; i < spriteRendererArray.Length; i++)
        {
            dissolveMaterialArray[i].SetFloat(DISSOLVE_AMOUNT, amount);
        }
    }
}
