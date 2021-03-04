using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    [SerializeField] MeshRenderer startMesh;
    [SerializeField] MeshRenderer brokenMesh;

    public float health;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (health == 2)
        {
            startMesh.enabled= true;
            brokenMesh.enabled = false;
        }
        else if (health == 1)
        {
            startMesh.enabled = false;
            brokenMesh.enabled = true;
        }      
        else if (health <= 0)
        {
            Destroy(this.gameObject);
        }

       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();

            if (pc.canBarge == false)
            {
                health -= 1;
            }
        }

        if (other.CompareTag("GP Sphere"))
        {
            health -= 1;

            Debug.Log("Being Stomped");
        }
    }
}
