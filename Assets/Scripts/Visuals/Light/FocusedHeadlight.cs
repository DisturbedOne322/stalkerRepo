using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FocusedHeadlight : MonoBehaviour
{
    private Light2D light2D;
    [SerializeField]
    private Light2D focusedLight2D;
    private float defaultLightIntensity;
    private readonly float focusedLightIntensity = 2.5f;
    private readonly float brokenLightIntensity = 0.3f;

    public static event Action OnGhostFound;
    public static event Action<TentacleStateManager> OnTentacleFound;
    // private float brokenHeadlightCD = 0;
    //private float brokenHeadlightCDTotal = 5;

    private readonly float focusSpeed = 4.5f;

    private bool focusedLightEnabled = false;

    private readonly float focusedLightCapacity = 1f;
    private float currentFocusedLightCapacity;
    public float CurrentFocusedLightCapacity
    {
        get { return currentFocusedLightCapacity; }
    }
    private readonly float focusedLightSpendRate = 0.2f;
    private readonly float focusedLightRestoreRate = 0.1f;


    private readonly float brokenHeadlightProbability = 0.03f; 

    private RaycastHit2D hit;
    private readonly float boxLength = 10f;
    private readonly float boxHeight = 3f;

    [SerializeField]
    private LayerMask ghostLayerMask;

    [SerializeField]
    private LayerMask tentacleLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        light2D = GetComponent<Light2D>();
        defaultLightIntensity = light2D.intensity;
        InputManager.Instance.OnHeadlightAction += Instance_OnHeadlightAction;
        InvokeRepeating("TryToBrakeHeadlight", 0, 0.5f);
        currentFocusedLightCapacity = focusedLightCapacity;
    }

    private void FixedUpdate()
    {
        if (focusedLightEnabled)
        {
            hit = Physics2D.BoxCast(transform.position, new Vector2(boxLength, boxHeight), transform.rotation.z, Vector2.right, 0, ghostLayerMask);
            if (hit)
            {
                OnGhostFound?.Invoke();
            }
            hit = Physics2D.BoxCast(transform.position, new Vector2(boxLength, boxHeight), transform.rotation.z, Vector2.right, 0, tentacleLayerMask);
            if(hit)
            {
                OnTentacleFound?.Invoke(hit.collider.gameObject.GetComponent<TentacleStateManager>());
            }
        }
    }
    private void Instance_OnHeadlightAction()
    {
        if(!focusedLightEnabled)
        {
            if(currentFocusedLightCapacity < 0.2f)
                return;
        }
        focusedLightEnabled = !focusedLightEnabled;
        SoundManager.Instance.PlayFocusedLightSound(focusedLightEnabled);
    }

    private void TryToBrakeHeadlight()
    {
        if (!focusedLightEnabled)
        {
            return;
        }
        if (brokenHeadlightProbability > UnityEngine.Random.Range(0, 1f))
        {
            focusedLight2D.intensity = brokenLightIntensity;
            SoundManager.Instance.PlayBrokenHeadlightSound();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (focusedLightEnabled)
        {
            focusedLight2D.intensity = Mathf.Lerp(focusedLight2D.intensity, focusedLightIntensity, (light2D.intensity / focusedLightIntensity) * focusSpeed * Time.deltaTime);   
            currentFocusedLightCapacity -= focusedLightSpendRate * Time.deltaTime;
        }
        else
        {
            focusedLight2D.intensity = Mathf.Lerp(focusedLight2D.intensity, defaultLightIntensity, (defaultLightIntensity / focusedLight2D.intensity) * focusSpeed * Time.deltaTime);
            currentFocusedLightCapacity += focusedLightRestoreRate * Time.deltaTime;
        }

        if (currentFocusedLightCapacity < 0)
        {
            focusedLightEnabled = false;
            SoundManager.Instance.PlayFocusedLightSound(focusedLightEnabled);
        }

        currentFocusedLightCapacity = Mathf.Clamp(currentFocusedLightCapacity, 0, focusedLightCapacity);
    }
}
