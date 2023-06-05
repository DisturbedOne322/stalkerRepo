using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private Transform shootPoint;

    private RaycastHit2D hit;

    [SerializeField]
    private LayerMask groundLayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,0,10) * Time.deltaTime);
        hit = Physics2D.Raycast(transform.position, transform.right, 10, groundLayer);
        Debug.Log(hit.point);
        
        if (hit)
        {
            lineRenderer.SetPosition(1, hit.point);
        }
    }

    private void Enable()
    {
        lineRenderer.enabled = true;
    }

    private void Disable()
    {
        lineRenderer.enabled = false;
    }
}
