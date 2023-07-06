using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraShootShake : MonoBehaviour
{
    private CinemachineVirtualCamera vCam;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    [SerializeField]
    private FollowMouseInput mousPos;

    private float shakeTimer;
    private readonly float shakeTimerTotal = 0.25f;

    private readonly float shootAmp = 2.3f;
    private readonly float shootFreq = 1.4f;

    private readonly float flameballHitGroundAmp = 5f;
    private readonly float flameballHitGroundFreq = 5f;

    private float originalShakeAmp;
    private float originalShakeFreq;

    private CinemachineComposer composer;
    private CinemachineTransposer transposer;

    private readonly float leftBodyOffset = -3;
    private readonly float rightBodyOffset = 3;

    private readonly float aimCameraSpeed = 2f;

    [SerializeField]
    private bool AimCamera;


    #region QTE
    private readonly float qteShakeFreq = 5f;
    #endregion

    #region player damaged
    private readonly float playerDamagedShakeAmp = 2.5f;
    private readonly float playerDamagedShakeFreq = 10f;
    private PlayerMovement player;
    #endregion

    /*
     * MAKE DIFFERENTY TIMERS
     */

    private void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        transposer =  vCam.GetCinemachineComponent<CinemachineTransposer>();
        composer = vCam.GetCinemachineComponent<CinemachineComposer>();
        originalShakeAmp = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain;
        originalShakeFreq = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain;
        cinemachineBasicMultiChannelPerlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        player.OnHealthChanged += Player_OnHealthChanged;
        player.OnPlayerTeleported += Player_OnPlayerTeleported;
        Shoot.OnSuccessfulShoot += Instance_OnShootAction;
        QTE.instance.OnQTEStart += QTE_OnQTEStart;
        QTE.instance.OnQTEEnd += QTE_OnQTEEnd;
        FlameballFallingState.OnFlameballHitGround += FlameballFallingState_OnFlameballHitGround;
    }

    private void OnDestroy()
    {
        player.OnHealthChanged -= Player_OnHealthChanged;
        player.OnPlayerTeleported -= Player_OnPlayerTeleported;
        Shoot.OnSuccessfulShoot -= Instance_OnShootAction;
        QTE.instance.OnQTEStart -= QTE_OnQTEStart;
        QTE.instance.OnQTEEnd -= QTE_OnQTEEnd;
        FlameballFallingState.OnFlameballHitGround -= FlameballFallingState_OnFlameballHitGround;
    }

    private void Player_OnPlayerTeleported()
    {
        vCam.LookAt = null;
        vCam.Follow = null;
    }


    private void FlameballFallingState_OnFlameballHitGround()
    {
        ShakeCamera(flameballHitGroundAmp, flameballHitGroundFreq, shakeTimerTotal);
    }

    private void Player_OnHealthChanged(GameManager.PlayerHealthStatus obj)
    {
        ShakeCamera(playerDamagedShakeAmp, playerDamagedShakeFreq, shakeTimerTotal);
    }

    private void QTE_OnQTEStart()
    {
        ShakeCamera(originalShakeAmp, qteShakeFreq, 12);
    }



    private void QTE_OnQTEEnd(IQTECaller caller)
    {
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = originalShakeAmp;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = originalShakeFreq;
        shakeTimer = 0;
    }


    private void Instance_OnShootAction(int a,int b)
    {
        ShakeCamera(shootAmp, shootFreq, shakeTimerTotal);
    }

    private void ShakeCamera(float intensity, float freq, float time)
    {
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = freq;
        shakeTimer = time;
    }

    private void Update()
    {
        if(shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(cinemachineBasicMultiChannelPerlin.m_AmplitudeGain, originalShakeAmp, 1 - (shakeTimer/shakeTimerTotal));
            cinemachineBasicMultiChannelPerlin.m_FrequencyGain = Mathf.Lerp(cinemachineBasicMultiChannelPerlin.m_FrequencyGain, originalShakeFreq, 1 - (shakeTimer/shakeTimerTotal));
        }

        if(AimCamera)
        {
            float offsetLerp = Mathf.Lerp(transposer.m_FollowOffset.x, mousPos.MousePosition == FollowMouseInput.Position.LeftOfPlayer ? leftBodyOffset : rightBodyOffset, aimCameraSpeed * Time.deltaTime);
            transposer.m_FollowOffset.x = offsetLerp;
            composer.m_TrackedObjectOffset.x = offsetLerp;
        }
    }
}
