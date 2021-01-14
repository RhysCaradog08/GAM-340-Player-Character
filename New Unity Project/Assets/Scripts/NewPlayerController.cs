using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{
    public CharacterController cc;
    Transform cam;

    [Header("Movement")]
    float speed;
    public float movespeed = 10f;
    public float chargeSpeed = 30f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Vector3 moveDir;

    [Header("Jumping")]
    public float jumpSpeed = 5;
    public float gravity = 9.81F;
    private Vector3 moveDirection = Vector3.zero;

    [Header("Barging")]
    public float bargeTime;
    public float bargeSpeed;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;

        speed = movespeed;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            cc.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (cc.isGrounded)
            {
                moveDirection.y = jumpSpeed;
            }
        }
        moveDirection.y -= gravity * Time.deltaTime;
        cc.Move(moveDirection * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(Barge());
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            speed = chargeSpeed;
        }
        else speed = movespeed;
    }

    IEnumerator Barge()
    {
        float startTime = Time.time;

        while (Time.time < startTime + bargeTime)
        {
            cc.Move(moveDir * bargeSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
