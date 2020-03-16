using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UserInterface : MonoBehaviour
{
    public static bool filling;
    public GameObject fillingAnim;

    public GameObject menuPanel;

    private void Update()
    {
        fillingAnim.SetActive(filling);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Exit(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        menuPanel.SetActive(!menuPanel.activeSelf);
    }
}