using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Next_Puyo : MonoBehaviour
{
    [SerializeField]
    private GameObject[] PUYO = new GameObject[2];
    [SerializeField]
    private Sprite[] Puyo_Sprite = new Sprite[5];//RGBYP
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        //êFê›íË
        for (int i = 0; i < 2; i++)
        {
            PUYO[i].GetComponent<SpriteRenderer>().sprite = Puyo_Sprite[P_GameController.GC.N_Color[i]];
        }
    }
}
