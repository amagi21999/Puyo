using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Puyo_Controller2 : MonoBehaviour
{
    [SerializeField]
    private Sprite[] Puyo_Sprite = new Sprite[5];//RGBYP
    [SerializeField]
    private GameObject[] PUYO = new GameObject[2];

    private float Down_Speed = 0.001f;
    private float X_Move = 0.7f;

    public static Move_Puyo_Controller2 MPC2;

    //色
    public char[] Color_Data = new char[2]; //RGBYP

    public void Awake()
    {
        if (MPC2 == null)
        {
            MPC2 = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //色設定
        for (int i = 0; i < 2; i++)
        {
            PUYO[i].GetComponent<SpriteRenderer>().sprite = Puyo_Sprite[P_GameController.GC.Color[i]];
        }
        /*Vector3 pos = transform.position;
        P_GameController.GC.Move_judge(pos.x, 0);*/
    }

    // Update is called once per frame
    void Update()
    {
        //Move_Puyo();
    }
    
    public void Down(int a)
    {
        Vector3 pos = transform.position;
        Vector3 pos1 = PUYO[0].transform.localPosition;
        Vector3 pos2 = PUYO[1].transform.localPosition;
        //オブジェクト移動
        pos.y -= 0.7f;
        transform.position = pos;
        //画像移動
        pos1.y = 0f;
        switch (a)
        {
            case 0:
                pos2.y = 0.32f;
                break;
            case 1:
            case 3:
                pos2.y = 0f;
                break;
            case 2:
                pos2.y = -0.32f;
                break;
        }
        PUYO[0].transform.localPosition = pos1;
        PUYO[1].transform.localPosition = pos2;
    }

    private void Move_Puyo()
    {
        Vector3 pos = transform.position;
        Vector3 pos1 = PUYO[0].transform.localPosition;
        Vector3 pos2 = PUYO[1].transform.localPosition;
        Vector3 angle = transform.localEulerAngles;
        pos1.y -= Down_Speed;
        pos2.y -= Down_Speed;
        PUYO[0].transform.localPosition = pos1;
        PUYO[1].transform.localPosition = pos2;
        transform.localEulerAngles = angle;
        PUYO[0].transform.localEulerAngles = angle * -1;
        PUYO[1].transform.localEulerAngles = angle * -1;
    }
    public void Move_R()
    {
        Vector3 pos = transform.position;
        pos.x += X_Move;
        transform.position = pos;
    }
    public void Move_L()
    {
        Vector3 pos = transform.position;
        pos.x -= X_Move;
        transform.position = pos;
    }

    public void Spen(int a)
    {
        Vector3 pos2 = PUYO[1].transform.localPosition;
        switch (a)
        {
            case 0:
                pos2.x = 0f;
                pos2.y = 0.32f;
                break;
            case 1:
                pos2.x = -0.32f;
                pos2.y = 0f;
                break;
            case 3:
                pos2.x = 0.32f;
                pos2.y = 0f;
                break;
            case 2:
                pos2.x = 0f;
                pos2.y = -0.32f;
                break;
        }
        PUYO[1].transform.localPosition = pos2;
    }

    public void Destroy_Obj()
    {
        Destroy(gameObject);
    }
}
