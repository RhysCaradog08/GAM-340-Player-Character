using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Animator anim;

    public Rigidbody rb;

    public Vector3 kbDir;
    public float kbStrength;

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
        Debug.Log("KB Strength: " + kbStrength);

        if(!isProne)
        {
            canGrab = false;
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

        kbStrength = 1;
    }

    void GrabDelay()
    {
        new WaitForSeconds(0.5f);
        canGrab = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            kbDir = other.transform.position + transform.position;

            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            if (player.holding == false && player.canBarge == false)
            {
                kbStrength = 50;

                KnockBack();
            }
            else if (player.holding == false && player.canBarge == true)
            {
                kbStrength = 1;

                if(!isProne)
                {
                    KnockBack();
                }
            }

            /*if(player.holding == false && player.canBarge ==false)
            {
                kbStrength = 10;

                KnockBack();
            }*/
        }
    }

    public void KnockBack()
    {
        rb.AddForce(kbDir.normalized * kbStrength, ForceMode.Impulse);
        //kbDir.y = 0;

        isProne = true;
    }
}
