using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Return_Menu : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("Start_Menu");
    }
}
