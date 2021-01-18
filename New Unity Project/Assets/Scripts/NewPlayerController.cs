using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{
    CharacterController cc;
    Transform cam;

    [Header("Movement")]
    bool charging;
    float speed;
    public float moveSpeed = 10f;
    public float chargeSpeed = 30f;
    public float holdSpeed = 3f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Vector3 targetDirection;

    [Header("Jumping")]
    public float jumpSpeed = 5;
    public float gravity = 9.81F;
    private Vector3 moveDirection = Vector3.zero;
    public float airSpeed = 5f;
    Vector3 groundNormal;
    public LayerMask mask;

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

    


    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;

        speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, mask);

        groundNormal = hit.normal;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (cc.isGrounded)
        {
            moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

            if (moveDirection.magnitude >= 0.1f)
            {
                RotatePlayer();
                /*float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                targetDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                cc.Move(targetDirection.normalized * speed * Time.deltaTime);*/
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {             
                    moveDirection.y = jumpSpeed;
            }
        }
        moveDirection.y -= gravity * Time.deltaTime;
        cc.Move(moveDirection * Time.deltaTime);
        //cc.Move(Vector3.ProjectOnPlane(cam.rotation * moveDirection, groundNormal) * speed * Time.deltaTime);

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

        /////DEBUGGING/////
        Debug.Log(moveDirection);
        Debug.Log("Charging" + Input.GetKey(KeyCode.Mouse1));
        Debug.Log("Holding Big: " + holdingBig);
        Debug.Log("Speed: " + speed);
    }

    void RotatePlayer()
    {
        float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        targetDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        cc.Move(targetDirection.normalized * speed * Time.deltaTime);
    }

    IEnumerator Barge()
    {
        barging = true;

        float startTime = Time.time;

        while (Time.time < startTime + bargeTime)
        {
            cc.Move(targetDirection * bargeSpeed * Time.deltaTime);

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

            if(other.CompareTag("BigEnemy"))
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
