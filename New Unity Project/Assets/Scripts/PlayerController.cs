using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController cc;
    Rigidbody rb;
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
    public Vector3 stopPos;

    [Header("Jumping")]
    public float jumpSpeed = 5;
    public float gravity = 9.81F;
    private Vector3 jumpDir = Vector3.zero;

    [Header("Barging")]
    bool canBarge;
    public float bargeTime;
    public float bargeSpeed;
    float bargeDelay;
    public GameObject trailEffect;

    [Header("Throwing")]
    bool holding;
    bool holdingBig;
    bool stopped;
    private GameObject throwObject;
    private Rigidbody throwRb;
    public Transform throwPos;
    public Transform bigThrowPos;
    public float minThrowForce;
    public float maxThrowForce;
    float throwForce;
    public float chargeRate;

    [Header("VFX")]
    public ParticleSystem sweat;
    public float particleSpeed = 1f;
    

    void Start()
    {
        cc = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;

        speed = moveSpeed;
        throwForce = minThrowForce;

        ParticleSystem.MainModule main = sweat.main;
        //main.simulationSpeed = particleSpeed;
    }

    void Update()
    {
        //Debug.Log("Speed: " + speed);
        //Debug.Log("Throw Force: " + throwForce);
        //Debug.Log("Particle Speed: " + particleSpeed);
        Debug.Log("Can Barge: " + canBarge);

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

        bargeDelay -= Time.deltaTime;


        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (holding || holdingBig)
            {
                canBarge = false;

                stopped = true;

                throwForce += chargeRate;

                if(throwForce >= maxThrowForce)
                {
                    throwForce = maxThrowForce;
                }
            }
        }        

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Throw();
            stopped = false;
            throwForce = minThrowForce;

            new WaitForSeconds(1);
            canBarge = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (canBarge && bargeDelay <= 0)
            {
                StartCoroutine(Barge());
            }
        }

        speed = moveSpeed;
        canBarge = true;
        trailEffect.SetActive(false);


        if (stopped)
        {
            cc.enabled = false;
            sweat.Play();
        }
        else
        {
            cc.enabled = true;
            sweat.Stop();
        }


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
        float startTime = Time.time;

        new WaitForSeconds(1); 

        while (Time.time < startTime + bargeTime)
        {
            trailEffect.SetActive(true);
            speed = 0;

            canBarge = false;
            cc.Move(moveDir * bargeSpeed * Time.deltaTime);
            bargeDelay = 0.5f;

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        throwObject = other.gameObject;
        throwRb = other.GetComponent<Rigidbody>();

        EnemyController enemy = other.GetComponent<EnemyController>();

        if (!canBarge)
        {
            if(other.CompareTag("Enemy"))
            {
                Destroy(other.gameObject);
            }
        }

        if (!holding && canBarge)
        {
            if (other.CompareTag("Throwable"))
            {
                Pickup();
            }

            if (other.CompareTag("Enemy"))
            {
                if (enemy.isProne == false)
                {
                    enemy.isProne = true;
                }
                else if (enemy.canGrab == true)
                {
                    Pickup();
                }
            }

            if (other.CompareTag("BigEnemy"))
            {
                BigPickup();
            }
        }
    }

    void Pickup()
    {
        throwObject.transform.SetParent(throwPos);
        throwObject.transform.position = Vector3.Lerp(throwObject.transform.position, throwPos.position, Time.time);

        throwRb.isKinematic = false;
        throwRb.constraints = RigidbodyConstraints.FreezeAll;

        holding = true;
    }

    void BigPickup()
    {
        throwObject.transform.SetParent(bigThrowPos);
        throwObject.transform.position = Vector3.Lerp(throwObject.transform.position, bigThrowPos.position, Time.time);

        throwRb.isKinematic = false;
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

    void SweatVFX()
    {
        Debug.Log("Sweating");
        sweat.Play();
    }
}
