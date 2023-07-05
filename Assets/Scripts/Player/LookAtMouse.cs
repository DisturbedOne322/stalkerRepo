using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    private Quaternion originalRotation;
    private Quaternion lastRotation;
    private Vector3 lastMousePosition;

    private readonly float topLookAtAngleBound = 45;
    private readonly float bottomLookAtAngleBound = -30f;

    private readonly float returnToOriginalRotationSpeed = 2f;

    [SerializeField]
    private bool focusFeature;
    private bool focused;

    private PlayerMovement player;
    private bool isAlive = true;

    public bool Focused
    {
        get { return focused; }
        private set { focused = value; }       
    }

    private void Awake()
    {
        originalRotation = transform.rotation;
    }

    private void Player_OnPlayerDied()
    {
        isAlive = false;
    }

    private void Start()
    {
        if (focusFeature)
        {
            InputManager.Instance.OnFocusActionStarted += InputManager_OnFocusAction;
            InputManager.Instance.OnFocusActionEnded += InputManager_OnFocusActionEnded;
        }
        player = GameManager.Instance.GetPlayerReference();
        player.OnPlayerDied += Player_OnPlayerDied;
    }

    private void InputManager_OnFocusActionEnded()
    {
        focused = false;
    }

    private void InputManager_OnFocusAction()
    {
        focused = true;
    }

    private void LateUpdate()
    {
        if (!isAlive)
            return;
        if(Idle.IsIdle) 
        {
            ReturnToOriginalPosition();
        }
        else
        {
            LookAt();
        }
    }

    private void LookAt()
    {
        //convert mouse position into world coordinates
        Vector2 direction = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);

        if(Input.mousePosition != lastMousePosition)
        {
            Idle.ReportAction();
            lastRotation = transform.localRotation;
        }

        if(transform.position.x > direction.x)
        {
            direction = -direction;
        }

        if(focusFeature && focused) 
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if(!focusFeature)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        Vector3 eulerDir = transform.localEulerAngles;
        eulerDir.z = Mathf.Clamp(eulerDir.z - (eulerDir.z > 180 ? 360 : 0), bottomLookAtAngleBound, topLookAtAngleBound);
        transform.localEulerAngles = eulerDir;

        lastMousePosition = Input.mousePosition;
    }

    private void ReturnToOriginalPosition()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, returnToOriginalRotationSpeed * Time.deltaTime);
    }
}
