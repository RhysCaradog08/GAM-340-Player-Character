using System.Collections;
using System.Collections.Generic;
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

    public float moveSpeed;
    //public float bargeSpeed;
    public float chargeSpeed;
    float speed;

    public float cooldown;
    public float bargeTime;
    public float bargeSpeed;


    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    bool facingLeft;
    bool facingRight;
    bool canBarge;

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

            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            cc.Move(moveDir.normalized * speed * Time.deltaTime);

            if (targetAngle < 0)
            {
                facingLeft = true;
                facingRight = false;
            }
            else if (targetAngle > 0)
            {
                facingLeft = false;
                facingRight = true;
            }

            Debug.Log("Facing Left: " + facingLeft);
            Debug.Log("Facing Right: " + facingRight);
        }
        else
        {
            upAnim.SetBool("Moving", false);
            lowAnim.SetBool("Moving", false);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B Pressed");
            StartCoroutine(Barge());
        }

        if (canBarge == false)
        {
            if(facingLeft)
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

        if (Input.GetKeyDown(KeyCode.T))
        {
            upAnim.SetBool("HoldingLeft", false);
            upAnim.SetBool("HoldingRight", false);
        }
    }

    /*private void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Enemy"))
        {
            Debug.LogError(col.collider.name);
            upAnim.SetBool("Holding", true);
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (facingLeft)
            {
                upAnim.SetBool("HoldingLeft", true);
            }
            else if (facingRight)
            {
                upAnim.SetBool("HoldingRight", true);
            }
        }
    }

    public IEnumerator Barge()
    {
        canBarge = false;
       
        float curTimeLeft = bargeTime;

        while (curTimeLeft > 0)
        {
            moveDir = transform.forward * bargeSpeed * Time.deltaTime;

            curTimeLeft -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(cooldown);
        canBarge = true;
    }
}
