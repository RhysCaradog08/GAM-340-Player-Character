using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{
    public CharacterController cc;
    Transform cam;

    [Header("Movement")]
    public float speed = 5f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Vector3 moveDir;

    [Header("Jumping")]
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;

    [Header("Barging")]
    public float bargeTime;
    public float bargeSpeed;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
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

        if (cc.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            moveDirection.y = jumpSpeed;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        cc.Move(moveDirection * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(Barge());
        }
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
