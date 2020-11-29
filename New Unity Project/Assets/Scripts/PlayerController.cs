using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController cc;

    public Animator upAnim;
    public Animator lowAnim;

    public Transform cam;

    Vector3 moveDir;
    float targetAngle;
    float angle;

    public float jumpForce;
    public float slamForce;
    private float gravity = 9.813f;
    private Vector3 jumpDir = Vector3.zero;


    public float moveSpeed;
    public float chargeSpeed;
    float speed;

    public float cooldown;
    public float bargeTime;
    public float bargeSpeed;

    private GameObject throwObject;
    public Transform throwPosL;
    public Transform throwPosR;
    private Rigidbody throwRb;
    public float throwForce;


    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    bool facingLeft;
    bool facingRight;
    bool canBarge;
    bool holdL;
    bool holdR;
    bool groundPounding;

    void Start()
    {
        cc = GetComponent<CharacterController>();

        speed = moveSpeed;
        canBarge = true;
    }

    void Update()
    {
        Debug.Log("Speed: " + speed);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            upAnim.SetBool("Moving", true);
            lowAnim.SetBool("Moving", true);

            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            cc.Move(moveDir.normalized * speed * Time.deltaTime);

            if (targetAngle < 0)
            {
                facingLeft = true;
                facingRight = false;
                Debug.Log("Facing Left: " + facingLeft);
            }
            else if (targetAngle > 0)
            {
                facingLeft = false;
                facingRight = true;
                Debug.Log("Facing Right: " + facingRight);
            }
        }
        else
        {
            upAnim.SetBool("Moving", false);
            lowAnim.SetBool("Moving", false);
        }

        if(groundPounding)
        {
            direction = Vector3.zero;
        }
        else direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (cc.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            jumpDir.y = jumpForce;
        }
        else if (!cc.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            GroundPound();
        }

        jumpDir.y -= gravity * Time.deltaTime;
        cc.Move(jumpDir * Time.deltaTime);

        if (!cc.isGrounded)
        {
            groundPounding = false;

            if (facingLeft)
            {
                upAnim.SetBool("JumpLeft", true);
                lowAnim.SetBool("JumpLeft", true);
            }
            else if (facingRight)
            {
                upAnim.SetBool("JumpRight", true);
                lowAnim.SetBool("JumpRight", true);
            }
        }
        else
        {
            upAnim.SetBool("JumpLeft", false);
            upAnim.SetBool("JumpRight", false);

            lowAnim.SetBool("JumpLeft", false);
            lowAnim.SetBool("JumpRight", false);

            upAnim.ResetTrigger("GroundPound");
            lowAnim.ResetTrigger("GroundPound");
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!holdL && !holdR)
            {
                StartCoroutine(Barge());
            }
        }

        if (canBarge == false)
        {
            if (facingLeft)
            {
                upAnim.SetBool("BargingLeft", true);
            }
            else if (facingRight)
            {
                upAnim.SetBool("BargingRight", true);
            }
            lowAnim.SetBool("Barging", true);
        }
        else if (canBarge == true)
        {
            //speed = moveSpeed;
            upAnim.SetBool("BargingRight", false);
            upAnim.SetBool("BargingLeft", false);
            lowAnim.SetBool("Barging", false);
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            speed = chargeSpeed;
            upAnim.SetBool("Charging", true);
            lowAnim.SetBool("Charging", true);
        }
        else
        {
            speed = moveSpeed;
            upAnim.SetBool("Charging", false);
            lowAnim.SetBool("Charging", false);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowObject();

            if (holdL)
            {
                upAnim.SetTrigger("ThrowLeft");
                upAnim.SetBool("HoldingLeft", false);

                holdL = false;
            }

            if (holdR)
            {
                upAnim.SetTrigger("ThrowRight");
                upAnim.SetBool("HoldingRight", false);

                holdR = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        
        if (other.CompareTag("Enemy"))
        {
            //EnemyController enemy = other.GetComponent<EnemyController>();
            
            if (enemy.isProne == false)
            {
                enemy.isProne = true;
            }

            if (enemy.isProne == true)
            {
                throwObject = other.gameObject;
                throwRb = other.GetComponent<Rigidbody>();

                //Debug.Log("ObjectRB: " + other.name);

                Pickup();
            }
        }
    }

    private void Pickup()
    {
        if (facingLeft)
        {
            upAnim.SetBool("HoldingLeft", true);

            throwObject.transform.SetParent(throwPosL);

            throwObject.transform.position = Vector3.Lerp(throwObject.transform.position, throwPosL.position, Time.time);

            holdL = true;
        }

        else if (facingRight)
        {
            upAnim.SetBool("HoldingRight", true);

            throwObject.transform.SetParent(throwPosR);

            throwObject.transform.position = Vector3.Lerp(throwObject.transform.position, throwPosR.position, Time.time);

            holdR = true;
        }

        throwRb.constraints = RigidbodyConstraints.FreezeAll;
    }

    void DropObject()
    {
        throwRb.constraints = RigidbodyConstraints.None;
        throwRb = null;
        throwObject.transform.parent = null;
        throwObject = null;
    }

    public void ThrowObject()
    {
        throwRb.AddForce(transform.forward * throwForce, ForceMode.Impulse);

        DropObject();

        upAnim.ResetTrigger("ThrowLeft");
        upAnim.ResetTrigger("ThrowRight");
    }

    public IEnumerator Barge()
    {
        canBarge = false;

        float curTimeLeft = bargeTime;

        while (curTimeLeft > 0)
        {
            speed = bargeSpeed;

            moveDir = transform.forward * speed * Time.deltaTime;

            curTimeLeft -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(cooldown);
        canBarge = true;
        speed = moveSpeed;
    }

    void GroundPound()
    {
        upAnim.SetTrigger("GroundPound");
        lowAnim.SetTrigger("GroundPound");

        upAnim.SetBool("JumpLeft", false);
        lowAnim.SetBool("JumpLeft", false);

        lowAnim.SetBool("JumpLeft", false);
        lowAnim.SetBool("JumpRight", false);

        groundPounding = true;

        jumpDir.y = -slamForce;
    }
}
