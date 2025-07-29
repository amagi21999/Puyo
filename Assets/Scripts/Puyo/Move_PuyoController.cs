using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_PuyoController : MonoBehaviour
{
    [SerializeField]
    private Material[] Puyo_Color = new Material[5];//RGBYP
    [SerializeField]
    private Sprite[] Puyo_Sprite = new Sprite[5];//RGBYP
    [SerializeField]
    private GameObject[] PUYO=new GameObject[2];

    private float Down_Speed = 0.001f;
    private int Quick_Down = 10;
    private float X_Move = 0.7f;
    private bool L_Trigger = false;
    private bool R_Trigger = false;
    private bool T_Trigger = false;
    private bool B_Trigger = false;

    public static Move_PuyoController MPC;

    //色
    public char[] Color_Data = new char[2]; //RGBYP

    public void Awake()
    {
        if(MPC == null)
        {
            MPC = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //色設定
        for(int i = 0; i < 2; i++)
        {
            PUYO[i].GetComponent<SpriteRenderer>().sprite = Puyo_Sprite[P_GameController.GC.Color[i]];
        }
        Vector3 pos = transform.position;
        P_GameController.GC.Move_judge(pos.x,0);
    }

    // Update is called once per frame
    void Update()
    {
        Move_Puyo();
    }
    private void Move_Puyo()
    {
        Vector3 pos = transform.position;
        Vector3 angle = transform.localEulerAngles;
        if (Input.GetKeyDown(KeyCode.DownArrow)) Down_Speed = Down_Speed * Quick_Down;  //落下加速
        if (Input.GetKeyUp(KeyCode.DownArrow) && Down_Speed > 0.005f) Down_Speed = Down_Speed / Quick_Down;    //落下加速解除
        //左右移動
        if (Input.GetKeyDown(KeyCode.RightArrow) && (((int)angle.z == 0 && R_Trigger == false) ||
                                                    ((int)angle.z == 270 && T_Trigger == false) ||
                                                    (((int)angle.z == 180 || (int)angle.z == -180) && L_Trigger == false) ||
                                                    ((int)angle.z == 90 && B_Trigger == false)))
        {
            pos.x += X_Move;     //右移動
            T_Trigger = false;
            B_Trigger = false;
            L_Trigger = false;
            R_Trigger = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && ((((int)angle.z == 0 || (int)angle.z == 360) && L_Trigger == false) ||
                                                   ((int)angle.z == 270 && B_Trigger == false) ||
                                                   (((int)angle.z == 180 || (int)angle.z == -180) && R_Trigger == false) ||
                                                   ((int)angle.z == 90 && T_Trigger == false)))
        {
            pos.x -= X_Move;     //左移動
            T_Trigger = false;
            B_Trigger = false;
            L_Trigger = false;
            R_Trigger = false;
        }

        //左右回転
        if (Input.GetKeyDown(KeyCode.X))
        {
            angle.z -= 90f;            //右回転
            if (R_Trigger == true && ((int)angle.z == -90 || (int)angle.z == -89 || (int)angle.z == 90))
            {
                angle.z -= 90f;
                T_Trigger = false;
                B_Trigger = false;
                L_Trigger = false;
                R_Trigger = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            angle.z += 90f;            //左回転
            if (L_Trigger == true && ((int)angle.z == 270 || (int)angle.z == 90))
            {
                angle.z += 90f;
                L_Trigger = false;
            }
        }
        //クイックドロップ
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            P_GameController.GC.Set_PUYO_Data();
            Destroy(gameObject);
        }
        //落下位置予測
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Z))
        {
            P_GameController.GC.Move_judge(pos.x, angle.z);
        }
        //落下処理
        pos.y -= Down_Speed;
        transform.position = pos;
        transform.localEulerAngles = angle;
        PUYO[0].transform.localEulerAngles = angle * -1;
        PUYO[1].transform.localEulerAngles = angle * -1;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        P_GameController.GC.Set_PUYO_Data();
        Destroy(gameObject);
    }
    public void L_True()
    {
        L_Trigger = true;
    }
    public void R_True()
    {
        R_Trigger = true;
    }
    public void T_True()
    {
        T_Trigger = true;
    }
    public void B_True()
    {
        B_Trigger = true;
    }
}
