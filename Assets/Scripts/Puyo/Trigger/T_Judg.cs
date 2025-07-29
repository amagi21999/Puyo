using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_Judg : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Move_PuyoController.MPC.T_True();
    }
}
