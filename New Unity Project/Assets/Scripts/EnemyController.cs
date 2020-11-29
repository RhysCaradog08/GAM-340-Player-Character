using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Animator anim;

    public bool isProne;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        isProne = false;
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
        }
    }
}
