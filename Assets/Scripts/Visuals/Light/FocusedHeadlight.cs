using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FocusedHeadlight : MonoBehaviour
{
    private Light2D light2D;
    [SerializeField]
    private Light2D focusedLight2D;
    private float defaultLightIntensity;
    private readonly float focusedLightIntensity = 6f;
    private readonly float brokenLightIntensity = 0.3f;

    public static event Action OnGhostFound;
    public static event Action<TentacleStateManager> OnTentacleFound;

    public static event Action OnExecutionerFound;

    private float focusSpeed = 1;
    private float lightSmDampVelocity;

    private bool focusedLightEnabled = false;

    private readonly float focusedLightCapacity = 1f;
    private float currentFocusedLightCapacity;
    public float CurrentFocusedLightCapacity
    {
        get { return currentFocusedLightCapacity; }
    }
    private readonly float focusedLightSpendRate = 0.15f;//0.2f;
    private readonly float focusedLightRestoreRate = 0.075f;


    private readonly float brokenHeadlightProbability = 0.03f; 

    private RaycastHit2D hit;
    private readonly float boxLength = 10f;
    private readonly float boxHeight = 1.5f;

    [SerializeField]
    private LayerMask ghostLayerMask;

    [SerializeField]
    private LayerMask tentacleLayerMask;

    [SerializeField]
    private LayerMask executionerLayerMask;



    private Vector2[] focusedHeadlightBoxPoints;

    //increase headlightBox with time
    private float headlightBoxSizeMultiplier = 0.4f;
    private float growSpeed = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        light2D = GetComponent<Light2D>();
        defaultLightIntensity = light2D.intensity;
        InputManager.Instance.OnHeadlightAction += Instance_OnHeadlightAction;
        InvokeRepeating("TryToBrakeHeadlight", 0, 0.5f);
        currentFocusedLightCapacity = focusedLightCapacity;
        focusedHeadlightBoxPoints = new Vector2[3];
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnHeadlightAction -= Instance_OnHeadlightAction;
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
            hit = Physics2D.BoxCast(transform.position, new Vector2(boxLength, boxHeight), transform.parent.parent.rotation.eulerAngles.z, Vector2.right, 0, tentacleLayerMask);
            if (hit)
            {
                OnTentacleFound?.Invoke(hit.collider.gameObject.GetComponent<TentacleStateManager>());
            }
            hit = Physics2D.BoxCast(transform.position, new Vector2(boxLength, boxHeight), transform.parent.parent.rotation.eulerAngles.z, Vector2.right, 0, executionerLayerMask);
            if (hit)
            {
                OnExecutionerFound?.Invoke();
            }
            BoxCastDrawer.Draw(hit, transform.position, new Vector2(boxLength, boxHeight), transform.parent.parent.rotation.eulerAngles.z, Vector2.right);
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

    public Vector2[] GetFocusedHeadlightBoxPoints()
    {
        return focusedHeadlightBoxPoints;
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
            focusedLight2D.intensity = Mathf.SmoothDamp(focusedLight2D.intensity, focusedLightIntensity, ref lightSmDampVelocity, focusSpeed);
            currentFocusedLightCapacity -= focusedLightSpendRate * Time.deltaTime;

            headlightBoxSizeMultiplier = Mathf.Clamp01(headlightBoxSizeMultiplier + Time.deltaTime * growSpeed);

            focusedHeadlightBoxPoints[0] = BoxCastDrawer.GetTopLeftOfBox(transform.position, new Vector2(boxLength, boxHeight) * headlightBoxSizeMultiplier, transform.parent.parent.rotation.eulerAngles.z);
            focusedHeadlightBoxPoints[1] = BoxCastDrawer.GetTopRightOfBox(transform.position, new Vector2(boxLength, boxHeight) * headlightBoxSizeMultiplier, transform.parent.parent.rotation.eulerAngles.z);
            focusedHeadlightBoxPoints[2] = BoxCastDrawer.GetBottomRightOfBox(transform.position, new Vector2(boxLength, boxHeight) * headlightBoxSizeMultiplier, transform.parent.parent.rotation.eulerAngles.z);
        }
        else
        {
            focusedLight2D.intensity = Mathf.SmoothDamp(focusedLight2D.intensity, defaultLightIntensity, ref lightSmDampVelocity, focusSpeed);
            currentFocusedLightCapacity += focusedLightRestoreRate * Time.deltaTime;
            headlightBoxSizeMultiplier = 0.4f;
            focusedHeadlightBoxPoints[0] = Vector2.zero;
            focusedHeadlightBoxPoints[1] = Vector2.zero;
            focusedHeadlightBoxPoints[2] = Vector2.zero;
        }

        if (currentFocusedLightCapacity < 0)
        {
            focusedLightEnabled = false;
            SoundManager.Instance.PlayFocusedLightSound(focusedLightEnabled);
        }

        currentFocusedLightCapacity = Mathf.Clamp(currentFocusedLightCapacity, 0, focusedLightCapacity);
    }
}
