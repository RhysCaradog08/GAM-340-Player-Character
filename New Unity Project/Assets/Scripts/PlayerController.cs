using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController cc;
    Animator anim;

    public Transform cam;

    public float moveSpeed;
    public float bargeSpeed;
    public float chargeSpeed;
    float speed;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
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
            anim.SetBool("Moving", true);

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            cc.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        else anim.SetBool("Moving", false);

        if (Input.GetKey(KeyCode.B))
        {
            Debug.Log("B Pressed");
            speed = bargeSpeed;
            anim.SetBool("Barging", true);
        }
        else
        {
            speed = moveSpeed;
            anim.SetBool("Barging", false);
        }

        if (Input.GetKey(KeyCode.C))
        {
            speed = chargeSpeed;
        }
        else
        {
            speed = moveSpeed;
        }
    }
}
