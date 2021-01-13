using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barge : MonoBehaviour
{
    NewPlayerController pc;

    public float bargeTime;
    public float bargeSpeed;

    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<NewPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(DoBarge());
        }
    }

    IEnumerator DoBarge()
    {
        float startTime = Time.time;

        while (Time.time < startTime + bargeTime)
        {
            pc.cc.Move(pc.moveDir * bargeSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
