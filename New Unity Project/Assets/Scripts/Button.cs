using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Animator anim;

    private void OnTriggerEnter(Collider other)
    {
        if(other .CompareTag("Throwable"))
        {
            Debug.Log("Button Pressed");

            anim.SetTrigger("Activated");
        }
    }
}
