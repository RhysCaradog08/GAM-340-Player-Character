using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Animator anim;

    public Rigidbody rb;

    public float kbForce;
    float resetTimer;

    public bool isProne;
    public bool canGrab;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
        //rb.isKinematic = true;

        isProne = false;
        canGrab = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isProne)
        {
            resetTimer = 3;
            anim.SetBool("Prone", false);
        }

        else if(isProne)
        {
            anim.SetBool("Prone", true);
            GrabDelay();

            resetTimer -= 1 * Time.deltaTime;
        }

        if(resetTimer <=0)
        {
            isProne = false;
        }
        rb.isKinematic = false;

    }

    void GrabDelay()
    {
        new WaitForSeconds(0.5f);
        canGrab = true;
        rb.isKinematic = true;
    }
}
