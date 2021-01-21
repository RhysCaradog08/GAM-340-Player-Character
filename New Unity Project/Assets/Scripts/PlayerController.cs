﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController cc;
    Transform cam;

    [Header("Movement")]
    bool charging;
    float speed;
    public float moveSpeed = 10f;
    public float chargeSpeed = 30f;
    public float holdSpeed = 3f;
    Vector3 direction;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Vector3 moveDir;

    [Header("Jumping")]
    public float jumpSpeed = 5;
    public float gravity = 9.81F;
    private Vector3 jumpDir = Vector3.zero;

    [Header("Barging")]
    bool barging;
    public float bargeTime;
    public float bargeSpeed;

    [Header("Throwing")]
    bool holding;
    bool holdingBig;
    private GameObject throwObject;
    private Rigidbody throwRb;
    public Transform throwPos;
    public Transform bigThrowPos;
    public float throwForce;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;

        speed = moveSpeed;
    }

    void Update()
    {
        Debug.Log("Speed: " + speed);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            cc.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (cc.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            jumpDir.y = jumpSpeed;
        }
        

        jumpDir.y -= gravity * Time.deltaTime;
        cc.Move(jumpDir * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (holding || holdingBig)
            {
                Throw();
            }
            else

            {
                StartCoroutine(Barge());
            }
        }

        barging = false;


        if (Input.GetKey(KeyCode.Mouse1))
        {
            speed = chargeSpeed;
            charging = true;
        }
        else
        {
            speed = moveSpeed;
            charging = false;
        }




        /*if (holdingBig == true)
        {
            speed = holdSpeed;
        }*/
    }
    IEnumerator Barge()
    {
        barging = true;

        float startTime = Time.time;

        while (Time.time < startTime + bargeTime)
        {
            cc.Move(moveDir * bargeSpeed * Time.deltaTime);

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();

        Vector3 hitDirection = transform.position - other.transform.position;
        hitDirection = hitDirection.normalized;

        if (!holding && !barging)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Throwable"))
            {
                throwObject = other.gameObject;
                throwRb = other.GetComponent<Rigidbody>();

                Pickup();
            }

            if (other.CompareTag("BigEnemy"))
            {
                throwObject = other.gameObject;
                throwRb = other.GetComponent<Rigidbody>();

                BigPickup();
            }
        }
    }

    void Pickup()
    {
        throwObject.transform.SetParent(throwPos);

        throwObject.transform.position = Vector3.Lerp(throwObject.transform.position, throwPos.position, Time.time);

        throwRb.constraints = RigidbodyConstraints.FreezeAll;

        holding = true;
    }

    void BigPickup()
    {
        throwObject.transform.SetParent(bigThrowPos);

        throwObject.transform.position = Vector3.Lerp(throwObject.transform.position, bigThrowPos.position, Time.time);

        throwRb.constraints = RigidbodyConstraints.FreezeAll;

        holdingBig = true;
    }

    void Throw()
    {
        throwRb.AddForce(transform.forward * throwForce, ForceMode.Impulse);

        DropObject();

        speed = moveSpeed;
    }

    void DropObject()
    {
        throwRb.constraints = RigidbodyConstraints.None;
        throwRb = null;
        throwObject.transform.parent = null;
        throwObject = null;

        holding = false;
        holdingBig = false;
    }
}
