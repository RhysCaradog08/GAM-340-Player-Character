using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController cc;
    public Animator upAnim;
    public Animator lowAnim;

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
            upAnim.SetBool("Moving", true);
            lowAnim.SetBool("Moving", true);

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            cc.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        else
        {
            upAnim.SetBool("Moving", false);
            lowAnim.SetBool("Moving", false);
        }

        if (Input.GetKey(KeyCode.B))
        {
            Debug.Log("B Pressed");
            speed = bargeSpeed;
            upAnim.SetBool("Barging", true);
        }
        else
        {
            speed = moveSpeed;
            upAnim.SetBool("Barging", false);
        }

        if (Input.GetKey(KeyCode.C))
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
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Enemy"))
        {
            upAnim.SetBool("Holding", true);
        }
    }
}
