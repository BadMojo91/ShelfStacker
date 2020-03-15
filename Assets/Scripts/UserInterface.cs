using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public static bool filling;
    public GameObject fillingAnim;

    private void Update()
    {
        if (filling)
            fillingAnim.SetActive(true);
        else
            fillingAnim.SetActive(false);
    }
}