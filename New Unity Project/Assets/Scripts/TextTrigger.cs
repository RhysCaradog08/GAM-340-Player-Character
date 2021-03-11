using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTrigger : MonoBehaviour
{
    public GameObject gameText;

    public Text text;

    [TextArea]
    public string instructions;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameText.SetActive(true);

            text.text = instructions;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        gameText.SetActive(false);
    }
}
