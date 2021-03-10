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
    bool hasJumped;
    bool canPressSpace = true;
    public float jumpSpeed = 5;
    public float gravity = 9.81F;
    [SerializeField]Vector3 velocity;
    public float fallMultiplier = 2;
    public float lowJumpMultiplier = 2.5f;

    [Header("Barging")]
    public bool canBarge;
    bool isBarging;
    public float bargeTime;
    public float bargeSpeed;
    float bargeDelay;
    public GameObject trailEffect;
    public float kbStrength;
    Vector3 kbDir;

    [Header("Throwing")]
    public bool holding;
    public bool holdingBig;
    bool stopped;
    private GameObject throwObject;
    private Rigidbody throwRb;
    public Transform throwPos;
    public Transform bigThrowPos;
    public float minThrowForce;
    public float maxThrowForce;
    float throwForce;
    public float chargeRate;
    EnemyController enemy;

    [Header("GroundPound")]
    public bool groundPounding;
    public float gpDelay;
    public float gpForce;
    float waitTime;
    public GameObject gpSphere;

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
        if (Input.GetKeyUp(KeyCode.Space) && !hasJumped)
        {
            canPressSpace = true;
        }

        if (cc.isGrounded)
        {
            velocity.y = 0f;

            if (Input.GetKey(KeyCode.Space) && canPressSpace)
            {
                velocity.y = jumpSpeed;
                hasJumped = true;
            }

            if (hasJumped)
            {
                canPressSpace = false;
                hasJumped = false;
            }

            if (waitTime <= 0)
            {
                waitTime = 0;
                stopped = false;
                groundPounding = false;
            }
            else
            {
                stopped = true;
                groundPounding = true;
            }
        }

        if (velocity.y < 0)
        {
            velocity.y += gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            velocity.y += gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (!cc.isGrounded && Input.GetKeyDown(KeyCode.Mouse1))
        {
            waitTime = 0.5f;
            groundPounding = true;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            cc.Move(moveDir.normalized * speed * Time.deltaTime);
        }        

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);

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

        trailEffect.SetActive(false);
        speed = moveSpeed;
        kbStrength = 0.1f;
        canBarge = true;
        isBarging = false;


        if (stopped)
        {
            cc.enabled = false;

            if (holding)
            {
                sweat.Play();
            }
        }
        else
        {
            cc.enabled = true;
            sweat.Stop();
        }

        if (groundPounding)
        {
            StartCoroutine(GroundPound());

            gpSphere.SetActive(true);
        }
        else gpSphere.SetActive(false);

        if(cc.isGrounded && groundPounding)
        {
            waitTime -= Time.deltaTime;
        }

        /*if (Input.GetKey(KeyCode.Mouse1))
        {
            speed = chargeSpeed;
            charging = true;
        }
        else
        {
            speed = moveSpeed;
            charging = false;
        }

        if (holdingBig == true)
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
            isBarging = true;
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
        enemy = other.GetComponent<EnemyController>();

        if (!holding && canBarge)
        {
            if (other.CompareTag("Throwable"))
            {
                throwObject = other.gameObject;
                throwRb = other.GetComponent<Rigidbody>();

                Pickup();
            }

            if (enemy != null)
            {
                throwObject = enemy.gameObject;
                throwRb = enemy.rb;

                if (other.CompareTag("Enemy"))
                {
                    if (enemy.canGrab == true)
                    {
                        {
                            Pickup();
                        }
                    }
                }
                else if(other.CompareTag("BigEnemy"))
                {
                    if (enemy.canGrab == true)
                    {
                        BigPickup();
                    }
                }
            }
        }

        if (!holding && !canBarge)
        {

            if (other.CompareTag("BigEnemy"))
            {
                if (enemy.isProne == false)
                {
                    enemy.KnockBack();
                }
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

        if (enemy != null)
        {
            enemy.isHeld = true;
        }
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
        if (enemy !=null)
        {
            enemy.isHeld = false;

            throwRb.AddForce(transform.forward * throwForce, ForceMode.Impulse);

            DropObject();

            speed = moveSpeed;
        }
        else
        {
            throwRb.AddForce(transform.forward * throwForce, ForceMode.Impulse);

            DropObject();

            speed = moveSpeed;
        }
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
        sweat.Play();
    }

    IEnumerator GroundPound()
    {
        stopped = true;

        yield return new WaitForSeconds(gpDelay);
        velocity.y = gpForce;

        stopped = false;
    }
}
