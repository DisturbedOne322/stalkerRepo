using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flameball : MonoBehaviour
{
    private Transform spawnPos;
    private float speed = 1;
    private Animator animator;

    private const string PUDDLE_ANIM = "PuddleAnim";


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector2(0, -speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //deal dmg to player
            Destroy(this.gameObject);
        }
        //hit the ground
        else
        {
            //turn into puddle
            animator.SetBool(PUDDLE_ANIM, true);
        }
    }
}
