using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    Animator buttonAnim;

    public Animator targetAnim;

    public bool bargeButton;
    public bool throwButton;
    public bool groundButton;

    private void Start()
    {
        buttonAnim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (throwButton)
        {
            if (other.CompareTag("Throwable"))
            {
                buttonAnim.SetTrigger("Pressed");

                targetAnim.SetTrigger("Activated");
            }
        }

        if (bargeButton)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController player = other.gameObject.GetComponent<PlayerController>();

                if (player.canBarge == false)
                {
                    buttonAnim.SetTrigger("Pressed");

                    targetAnim.SetTrigger("Activated");
                }
            }
        }

        if (groundButton)
        {
            Debug.Log("In Trigger");

            if (other.CompareTag("Player"))
            {
                PlayerController player = other.gameObject.GetComponent<PlayerController>();

                if (player.groundPounding)
                {
                    buttonAnim.SetTrigger("Pressed");

                    targetAnim.SetTrigger("Activated");
                }
            }
        }
    }
}
