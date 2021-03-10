using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject pauseCanvas;
    public GameObject menu;
    public GameObject controls;

    public GameObject gameText;

    public bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                isPaused = false;
            }
            else if(!isPaused)
            {
                isPaused = true;
            }
        }

        if (isPaused)
        {
            Time.timeScale = Mathf.Epsilon;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            pauseCanvas.SetActive(true);
            gameText.SetActive(false);
        }
        else
        {
            Time.timeScale = 1;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;

            pauseCanvas.SetActive(false);
            gameText.SetActive(true);
        }
    }

    public void PlayGame()
    {
        isPaused = false;
    }

    public void ShowControls()
    {
        menu.SetActive(false);
        controls.SetActive(true);
    }

    public void ShowMenu()
    {
        menu.SetActive(true);
        controls.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
