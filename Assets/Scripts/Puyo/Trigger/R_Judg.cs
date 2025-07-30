using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_Judg : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Move_PuyoController.MPC.R_True();
    }
}
