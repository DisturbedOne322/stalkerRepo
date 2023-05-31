using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMovement : MonoBehaviour
{
    [SerializeField]
    Transform pointA;
    [SerializeField]
    Transform pointB;
    [SerializeField]
    Transform pointC;
    [SerializeField]
    Transform pointD;
    private float interpolateAmount = 0f;

    private float speed = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        interpolateAmount += Time.deltaTime * speed;

        if(interpolateAmount > 1)
        {
            interpolateAmount = 0;

            pointD = pointA;
            pointA = pointC;
            pointC = pointD;
        }

        transform.position = QuadraticLerp(pointA.position, pointB.position, pointC.position, interpolateAmount);
    }

    private Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(ab, bc, interpolateAmount);
    }
}
