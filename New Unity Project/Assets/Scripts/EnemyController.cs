using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Animator anim;

    public Rigidbody rb;

    public float resetTimer;

    public bool isProne;
    public bool canGrab;
    public bool isHeld;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

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

            if (!isHeld)
            {
                resetTimer -= 1 * Time.deltaTime;
            }
            else if (isHeld)
            {
                rb.isKinematic = false;
            }
            else rb.isKinematic = true;
        }

        rb.isKinematic = false;

        if (resetTimer <=0)
        {
            isProne = false;
        }
    }

    void GrabDelay()
    {
        new WaitForSeconds(0.5f);
        canGrab = true;
    }
}
