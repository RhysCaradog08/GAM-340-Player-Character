using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Animator anim;

    Rigidbody rb;

    public float kbForce;

    public bool isProne;
    public bool canGrab;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        isProne = false;
        canGrab = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isProne)
        {
            anim.SetBool("Prone", false);
        }

        else if(isProne)
        {
            anim.SetBool("Prone", true);
            GrabDelay();
        }
    }

    void GrabDelay()
    {
        new WaitForSeconds(0.5f);
        canGrab = true;
    }

    public void Knockback(Vector3 direction)
    {
        Vector3 rbPos = rb.transform.position;

        rbPos = direction * kbForce;        
    }
}
