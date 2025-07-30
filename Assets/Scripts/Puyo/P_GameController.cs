using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_GameController : MonoBehaviour
{
    //置き位置予想用
    [SerializeField]
    private GameObject[] Pre_Puyo = new GameObject[84];
    [SerializeField]
    private Material[] Puyo_Color = new Material[5];//RGBYP
    //置きぷよ
    [SerializeField]
    private GameObject[] Set_Puyo = new GameObject[84];
    //落下ぷよ
    [SerializeField]
    GameObject Move_Puyo;

    
    //ぷよ画像
    [SerializeField]
    private Sprite[] Puyo = new Sprite[5];
    [SerializeField]
    private Sprite[] W_Puyo = new Sprite[5];
    [SerializeField]
    private Sprite[] S_Puyo = new Sprite[5];
    [SerializeField]
    private Sprite[] WS_Puyo = new Sprite[5];
    [SerializeField]
    private Sprite[] A_Puyo = new Sprite[5];
    [SerializeField]
    private Sprite[] WA_Puyo = new Sprite[5];
    [SerializeField]
    private Sprite[] AS_Puyo = new Sprite[5];
    [SerializeField]
    private Sprite[] WAS_Puyo = new Sprite[5];
    [SerializeField]
    private Sprite[] D_Puyo = new Sprite[5];
    [SerializeField]
    private Sprite[] WD_Puyo = new Sprite[5];
    [SerializeField]
    private Sprite[] SD_Puyo = new Sprite[5];
    [SerializeField]
    private Sprite[] WSD_Puyo = new Sprite[5];
    [SerializeField]
    private Sprite[] AD_Puyo = new Sprite[5];
    [SerializeField]
    private Sprite[] WAD_Puyo = new Sprite[5];
    [SerializeField]
    private Sprite[] ASD_Puyo = new Sprite[5];
    [SerializeField]
    private Sprite[] WASD_Puyo = new Sprite[5];
    [SerializeField]
    private Sprite[] Delete_Puyo = new Sprite[5];

    //盤面データ
    public char[,] Puyo_Data = new char[6,14];//RGBYP N
    //生成時の色
    public int[] Color = new int[2];
    //次生成予定の色
    public int[] N_Color = new int[2];
    //消去判定
    private bool[,] Puyo_Delete = new bool[6,13];
    //探索判定
    private bool[,] Search = new bool[6, 13];
    //探索個数
    private int Puyo_Count = 0;
    //消去処理に入るか判定用(Puyo_Count<4)
    private bool delete = false;
    //消去時間
    private float Delete_Time = 0.1f;

    //共有用
    public static P_GameController GC;

    //SE
    [SerializeField]
    private AudioClip[] SE = new AudioClip[5];
    [SerializeField]
    private AudioSource audioSource;

    //Text
    [SerializeField]
    private Text Conbo_Text;
    [SerializeField]
    private Text Score_Text;
    [SerializeField]
    private GameObject Conbo_Obj;


    private int Score = 0;
    //連鎖数
    private int Conbo = 0;
    //連鎖確認用
    private bool Conbo_Check = false;
    //同時消去色確認用
    private int Delete_Color = 0;
    private bool[] Delete_Color_Check = new bool[5];
   
    private bool Game_Over_Flag = false;

    [SerializeField]
    private GameObject Fade;
    [SerializeField]
    private Text Fade_Text;
    [SerializeField]
    private GameObject[] Game_Over_Obj = new GameObject[3];
    [SerializeField]
    private Text Game_Over_Text;

    private float Move_time;
    private float Move_Temp = 2;
    private int[,] Move_Puyo_Pos = new int[2,2];//Obj,xy
    private int Move_Puyo_Angle;//反時計回り0,1,2,3
    private char[] Move_Puyo_Color = new char[2];
    private bool Move = false;
    private bool Esc = false;

    public void Awake()
    {
        if (GC == null)
        {
            GC = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        for (int i=0; i < 14; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                Puyo_Data[j, i] = 'N';
            }
        }
        N_Color[0] = Random.Range(0, 5);
        N_Color[1] = Random.Range(0, 5);
        Rand_Color();
        StartCoroutine(Game_Start(1f));
    }

    // Update is called once per frame
    void Update()
    {
        if (Esc == false) PUYO_Move();
        Game_Stop();
    }

    private IEnumerator Game_Start(float time)
    {
        audioSource.PlayOneShot(SE[2]);
        Fade_Text.text = "用意・・・";
        yield return new WaitForSeconds(time);
        audioSource.PlayOneShot(SE[3]);
        Fade_Text.text = "スタート!!";
        yield return new WaitForSeconds(time);
        Fade.SetActive(false);
        StartCoroutine(Summon_PUYO(0.5f));
    }

    private void Game_End()
    {
        audioSource.PlayOneShot(SE[4]);
        Fade_Text.text = "終了!!";
        Game_Over_Text.text = $"スコア：{Score}";
        Fade.SetActive(true);
        Game_Over_Obj[0].SetActive(true);
        Game_Over_Obj[1].SetActive(true);
        Game_Over_Obj[2].SetActive(true);
    }

    private void Game_Stop()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Esc");
            if (Esc == false)
            {
                Time.timeScale = 0;
                Fade_Text.text = "一時停止";
                Game_Over_Text.text = $"スコア：{Score}";
                Fade.SetActive(true);
                Game_Over_Obj[0].SetActive(true);
                Game_Over_Obj[1].SetActive(true);
                Game_Over_Obj[2].SetActive(true);
                Esc = true;
            }
            else
            {
                Time.timeScale = 1;
                Fade.SetActive(false);
                Game_Over_Obj[0].SetActive(false);
                Game_Over_Obj[1].SetActive(false);
                Game_Over_Obj[2].SetActive(false);
                Esc = false;
            }
        }
    }

    //召喚前の色生成
    private void Rand_Color()
    {
        for (int i = 0; i < 2; i++)
        {
            Color[i] = N_Color[i];
            N_Color[i] = Random.Range(0, 5);
        }
    }

    //ぷよ召喚
    /*public IEnumerator Summon_PUYO(float time)
    {
        Rand_Color();
        Conbo = 0;
        Delete_Color = 0;
        Conbo_Obj.SetActive(false);
        for (int i = 0; i < 5; i++)
        {
            Delete_Color_Check[i] = false;
        }
        float x = 0.35f;
        float y = 5f;
        float z = 0;
        if (Puyo_Data[3, 11] != 'N')
        {
            for (int i = 0; i < 6; i++)
            {
                if (Puyo_Data[i, 11] == 'N')
                {
                    x = -1.75f + (0.7f * i);
                    break;
                }
            }
        }
        yield return new WaitForSeconds(time);
        Instantiate(Move_Puyo, new Vector3(x, y, z), Quaternion.identity);
    }*/
    //ぷよ召喚 Ver2
    public IEnumerator Summon_PUYO(float time)
    {
        Rand_Color();
        Conbo = 0;
        Delete_Color = 0;
        Conbo_Obj.SetActive(false);
        for (int i = 0; i < 5; i++)
        {
            Delete_Color_Check[i] = false;
        }
        float x = -0.35f;
        float y = 4f;
        float z = 0;
        int dx = 2;
        int dy = 11;
        yield return new WaitForSeconds(time);
        for (int i = 0; i < 2; i++)
        {
            char cc='R';
            switch (Color[i])
            {
                case 0:
                    cc = 'R';
                    break;
                case 1:
                    cc = 'G';
                    break;
                case 2:
                    cc = 'B';
                    break;
                case 3:
                    cc = 'Y';
                    break;
                case 4:
                    cc = 'P';
                    break;
            }
            Move_Puyo_Color[i] = cc;
            Move_Puyo_Pos[i, 0] = dx;
            Move_Puyo_Pos[i, 1] = dy + i;
            Move_Puyo_Angle = 0;
        }
        Move_time = 0;
        Instantiate(Move_Puyo, new Vector3(x, y, z), Quaternion.identity);
        Move = true;
    }

    //置き位置予測
    public void Move_judge(int x, int angle)
    {
        bool end = false;
        for (int i = 0; i < 84; i++)
        {
            Pre_Puyo[i].SetActive(false);
        }
        int y = 0;
        switch ((int)angle)
        {
            case 0:
                while (end != true)
                {
                    if (Puyo_Data[x, y] == 'N')
                    {
                        Pre_Puyo[x + (y * 6)].SetActive(true);
                        Pre_Puyo[x + (y * 6)].GetComponent<Renderer>().material = Puyo_Color[Color[0]];
                        Pre_Puyo[x + ((y + 1) * 6)].SetActive(true);
                        Pre_Puyo[x + ((y + 1) * 6)].GetComponent<Renderer>().material = Puyo_Color[Color[1]];
                        end = true;
                    }
                    else
                    {
                        y++;
                    }
                }
                break;
            case 1:
                while (end != true)
                {
                    if (Puyo_Data[x, y] == 'N')
                    {
                        Pre_Puyo[x + (y * 6)].SetActive(true);
                        Pre_Puyo[x + (y * 6)].GetComponent<Renderer>().material = Puyo_Color[Color[0]];
                        end = true;
                        y = 0;
                    }
                    else
                    {
                        y++;
                    }
                }
                end = false;
                while (end != true)
                {
                    if (Puyo_Data[x - 1, y] == 'N')
                    {
                        Pre_Puyo[x + (y * 6) - 1].SetActive(true);
                        Pre_Puyo[x + (y * 6) - 1].GetComponent<Renderer>().material = Puyo_Color[Color[1]];
                        end = true;
                    }
                    else
                    {
                        y++;
                    }
                }
                break;
            case 2:
                while (end != true)
                {
                    if (Puyo_Data[x, y] == 'N')
                    {
                        Pre_Puyo[x + (y * 6)].SetActive(true);
                        Pre_Puyo[x + (y * 6)].GetComponent<Renderer>().material = Puyo_Color[Color[1]];
                        Pre_Puyo[x + ((y + 1) * 6)].SetActive(true);
                        Pre_Puyo[x + ((y + 1) * 6)].GetComponent<Renderer>().material = Puyo_Color[Color[0]];
                        end = true;
                    }
                    else
                    {
                        y++;
                    }
                }
                break;
            case 3:
                while (end != true)
                {
                    if (Puyo_Data[x, y] == 'N')
                    {
                        Pre_Puyo[x + (y * 6)].SetActive(true);
                        Pre_Puyo[x + (y * 6)].GetComponent<Renderer>().material = Puyo_Color[Color[0]];
                        end = true;
                        y = 0;
                    }
                    else
                    {
                        y++;
                    }
                }
                end = false;
                while (end != true)
                {
                    if (Puyo_Data[x + 1, y] == 'N')
                    {
                        Pre_Puyo[x + (y * 6) + 1].SetActive(true);
                        Pre_Puyo[x + (y * 6) + 1].GetComponent<Renderer>().material = Puyo_Color[Color[1]];
                        end = true;
                    }
                    else
                    {
                        y++;
                    }
                }
                break;
        }
    }
    #region ぷよ動作
    private void PUYO_Move()
    {
        if (Move == true)
        {
            Move_judge(Move_Puyo_Pos[0, 0], Move_Puyo_Angle);
            Move_time += Time.deltaTime;
            //一定時間ごとにPuyo_Dataを下げる
            if (Move_time > Move_Temp)
            {
                //Moveデータの下にデータがないなら落下
                if (Move_Puyo_Pos[0, 1] != 0 && Move_Puyo_Pos[1, 1] != 0 && Puyo_Data[Move_Puyo_Pos[0, 0], Move_Puyo_Pos[0, 1] - 1] == 'N' && Puyo_Data[Move_Puyo_Pos[1, 0], Move_Puyo_Pos[1, 1] - 1] == 'N')
                {
                    Move_Puyo_Controller2.MPC2.Down(Move_Puyo_Angle);
                    Move_Puyo_Pos[0, 1]--;
                    Move_Puyo_Pos[1, 1]--;
                    Move_time = 0;
                }
                else
                {
                    Move = false;
                    Move_Puyo_Controller2.MPC2.Destroy_Obj();
                    Set_PUYO_Data();
                }
            }
            //左右
            /*キー入力
             壁際確認
            隣接マス確認*/
            if (Input.GetKeyDown(KeyCode.LeftArrow) &&
                (Move_Puyo_Pos[0, 0] != 0 && Move_Puyo_Pos[1, 0] != 0) &&
                (Puyo_Data[Move_Puyo_Pos[0, 0] - 1, Move_Puyo_Pos[0, 1]] == 'N' && Puyo_Data[Move_Puyo_Pos[1, 0] - 1, Move_Puyo_Pos[1, 1]] == 'N'))
            {
                Move_Puyo_Pos[0, 0]--;
                Move_Puyo_Pos[1, 0]--;
                Move_Puyo_Controller2.MPC2.Move_L();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) &&
                (Move_Puyo_Pos[0, 0] != 5 && Move_Puyo_Pos[1, 0] != 5) &&
                (Puyo_Data[Move_Puyo_Pos[0, 0] + 1, Move_Puyo_Pos[0, 1]] == 'N' && Puyo_Data[Move_Puyo_Pos[1, 0] + 1, Move_Puyo_Pos[1, 1]] == 'N'))
            {
                Move_Puyo_Pos[0, 0]++;
                Move_Puyo_Pos[1, 0]++;
                Move_Puyo_Controller2.MPC2.Move_R();
            }

            //回転
            if (Input.GetKeyDown(KeyCode.Z))
            {
                bool Spin = false;
                while (Spin == false)
                {
                    switch (Move_Puyo_Angle)
                    {
                        case 0:
                            Move_Puyo_Pos[1, 0]--;
                            Move_Puyo_Pos[1, 1]--;
                            Move_Puyo_Angle++;
                            if (Move_Puyo_Pos[0, 0] != 0 && Puyo_Data[Move_Puyo_Pos[0, 0] - 1, Move_Puyo_Pos[0, 1]] == 'N')
                            {
                                Spin = true;
                            }
                            else
                            {
                                continue;
                            }
                            break;
                        case 1:
                            Move_Puyo_Pos[1, 0]++;
                            Move_Puyo_Pos[1, 1]--;
                            Move_Puyo_Angle++;
                            if (Move_Puyo_Pos[0, 1] != 0 && Puyo_Data[Move_Puyo_Pos[0, 0], Move_Puyo_Pos[0, 1] - 1] == 'N')
                            {
                                Spin = true;
                            }
                            else
                            {
                                continue;
                            }
                            break;
                        case 2:
                            Move_Puyo_Pos[1, 0]++;
                            Move_Puyo_Pos[1, 1]++;
                            Move_Puyo_Angle++;
                            if (Move_Puyo_Pos[0, 0] != 5 && Puyo_Data[Move_Puyo_Pos[0, 0] + 1, Move_Puyo_Pos[0, 1]] == 'N')
                            {
                                Spin = true;
                            }
                            else
                            {
                                continue;
                            }
                            break;
                        case 3:
                            Move_Puyo_Pos[1, 0]--;
                            Move_Puyo_Pos[1, 1]++;
                            Move_Puyo_Angle = 0;
                            Spin = true;
                            break;
                    }
                }
                Move_Puyo_Controller2.MPC2.Spen(Move_Puyo_Angle);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                bool Spin = false;
                while (Spin == false)
                {
                    switch (Move_Puyo_Angle)
                    {
                        case 0:
                            Move_Puyo_Pos[1, 0]++;
                            Move_Puyo_Pos[1, 1]--;
                            Move_Puyo_Angle = 3;
                            if (Move_Puyo_Pos[0, 0] != 5 && Puyo_Data[Move_Puyo_Pos[0, 0] + 1, Move_Puyo_Pos[0, 1]] == 'N')
                            {
                                Spin = true;
                            }
                            else
                            {
                                continue;
                            }
                            break;
                        case 3:
                            Move_Puyo_Pos[1, 0]--;
                            Move_Puyo_Pos[1, 1]--;
                            Move_Puyo_Angle--;
                            if (Move_Puyo_Pos[0, 1] != 0 && Puyo_Data[Move_Puyo_Pos[0, 0], Move_Puyo_Pos[0, 1] - 1] == 'N')
                            {
                                Spin = true;
                            }
                            else
                            {
                                continue;
                            }
                            break;
                        case 2:
                            Move_Puyo_Pos[1, 0]--;
                            Move_Puyo_Pos[1, 1]++;
                            Move_Puyo_Angle--;
                            if (Move_Puyo_Pos[0, 0] != 0 && Puyo_Data[Move_Puyo_Pos[0, 0] - 1, Move_Puyo_Pos[0, 1]] == 'N')
                            {
                                Spin = true;
                            }
                            else
                            {
                                continue;
                            }
                            break;
                        case 1:
                            Move_Puyo_Pos[1, 0]++;
                            Move_Puyo_Pos[1, 1]++;
                            Move_Puyo_Angle--;
                            Spin = true;
                            break;
                    }
                }
                Move_Puyo_Controller2.MPC2.Spen(Move_Puyo_Angle);
            }

            //降下加速
            if (Input.GetKey(KeyCode.DownArrow))
            {
                Move_Temp = 0.5f;
            }
            else Move_Temp = 2;

            //クイックドロップ
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Move = false;
                Move_Puyo_Controller2.MPC2.Destroy_Obj();
                Set_PUYO_Data();
            }
        }
    }
    #endregion
    #region ぷよ配置
    //データ配置
    public void Set_PUYO_Data()
    {
        for (int i = 0; i < 78; i++)
        {
            if (Pre_Puyo[i].activeSelf == true)
            {
                switch (Pre_Puyo[i].GetComponent<Renderer>().material.name)
                {
                    case "R (Instance)":
                        Puyo_Data[i % 6, i / 6] = 'R';
                        break;
                    case "G (Instance)":
                        Puyo_Data[i % 6, i / 6] = 'G';
                        break;
                    case "B (Instance)":
                        Puyo_Data[i % 6, i / 6] = 'B';
                        break;
                    case "Y (Instance)":
                        Puyo_Data[i % 6, i / 6] = 'Y';
                        break;
                    case "P (Instance)":
                        Puyo_Data[i % 6, i / 6] = 'P';
                        break;
                }
            }
        }
        for (int i = 0; i < 6; i++)
        {
            if (Puyo_Data[i, 12] != 'N')
            {
                Game_Over_Flag = true;
            }
        }
        if (Game_Over_Flag == false)
        {
            Set_PUYO_Obj();
        }
        else
        {
            //Game_End();
            Set_PUYO_Obj();
        }
    }
    //Obj配置
    public void Set_PUYO_Obj()
    {
        audioSource.PlayOneShot(SE[0]);
        for (int i = 0; i < 78; i++)
        {
            if (Puyo_Data[i % 6, i / 6] != 'N')
            {
                int WASD = 0;
                int Set_Color = 0;
                Set_Puyo[i].SetActive(true);
                //上下左右に同じ色がないか確認
                if (i / 6 != 12 && Puyo_Data[i % 6, i / 6] == Puyo_Data[i % 6, i / 6 + 1]) WASD += 1;
                if (i % 6 != 0 && Puyo_Data[i % 6, i / 6] == Puyo_Data[i % 6 - 1, i / 6]) WASD += 2;
                if (i / 6 != 0 && Puyo_Data[i % 6, i / 6] == Puyo_Data[i % 6, i / 6 - 1]) WASD += 4;
                if (i % 6 != 5 && Puyo_Data[i % 6, i / 6] == Puyo_Data[i % 6 + 1, i / 6]) WASD += 8;
                //色の確認
                switch (Puyo_Data[i % 6, i / 6])
                {
                    case 'R':
                        Set_Color = 0;
                        break;
                    case 'G':
                        Set_Color = 1;
                        break;
                    case 'B':
                        Set_Color = 2;
                        break;
                    case 'Y':
                        Set_Color = 3;
                        break;
                    case 'P':
                        Set_Color = 4;
                        break;
                }
                //画像貼り付け
                switch (WASD)
                {
                    case 0:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = Puyo[Set_Color];
                        break;
                    case 1:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = W_Puyo[Set_Color];
                        break;
                    case 2:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = A_Puyo[Set_Color];
                        break;
                    case 3:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = WA_Puyo[Set_Color];
                        break;
                    case 4:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = S_Puyo[Set_Color];
                        break;
                    case 5:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = WS_Puyo[Set_Color];
                        break;
                    case 6:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = AS_Puyo[Set_Color];
                        break;
                    case 7:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = WAS_Puyo[Set_Color];
                        break;
                    case 8:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = D_Puyo[Set_Color];
                        break;
                    case 9:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = WD_Puyo[Set_Color];
                        break;
                    case 10:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = AD_Puyo[Set_Color];
                        break;
                    case 11:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = WAD_Puyo[Set_Color];
                        break;
                    case 12:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = SD_Puyo[Set_Color];
                        break;
                    case 13:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = WSD_Puyo[Set_Color];
                        break;
                    case 14:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = ASD_Puyo[Set_Color];
                        break;
                    case 15:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = WASD_Puyo[Set_Color];
                        break;
                }
            }
            else
            {
                Set_Puyo[i].SetActive(false);
            }
        }
        for (int i = 0; i < 78; i++)
        {
            Pre_Puyo[i].SetActive(false);
        }
        PUYO_Search();
    }
    #endregion

    #region 消去ぷよ捜索
    //個数確認
    private void PUYO_Search()
    {
        int x = 0;
        int y = 0;
        Puyo_Count = 0;
        int mx = 0;
        int my = 0;
        Conbo_Check = false;
        delete = false;
        for (int i = 0; i < 13; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                y = i;
                x = j;
                //未探索                   　且つカラーデータ無しじゃない
                if (Search[x, y] == false && Puyo_Data[x, y] != 'N')
                {
                    Search[x, y] = true;
                    Puyo_Count++;
                    //上下左右確認
                    for (int k = 0; k < 4; k++)
                    {
                        switch (k)
                        {
                            case 0:
                                mx = 0;
                                my = 1;
                                break;
                            case 1:
                                mx = 0;
                                my = -1;
                                break;
                            case 2:
                                mx = -1;
                                my = 0;
                                break;
                            case 3:
                                mx = 1;
                                my = 0;
                                break;
                        }
                        //フィールドの外じゃない　　                                         且つ参照先と参照元の色データが同じ
                        if ((x + mx != -1 && x + mx != 6 && y + my != -1 && y + my != 13) && Puyo_Data[x + mx, y + my] == Puyo_Data[x, y])
                        {
                            Puyo_Delete[x, y] = true;
                            Add_PUYO_Counter(x + mx, y + my);
                        }
                    }
                    if (Puyo_Count >= 4)
                    {
                        delete = true;
                        //コンボ
                        if (Conbo_Check == false)
                        {
                            Conbo++;
                            Conbo_Obj.SetActive(true);
                            Conbo_Text.text = $"{Conbo}連鎖!!";
                            audioSource.PlayOneShot(SE[1]);
                            Conbo_Check = true;
                        }
                        //スコア
                        int d = 0;
                        switch (Puyo_Data[x, y])
                        {
                            case 'R':
                                d = 0;
                                break;
                            case 'G':
                                d = 1;
                                break;
                            case 'B':
                                d = 2;
                                break;
                            case 'Y':
                                d = 3;
                                break;
                            case 'P':
                                d = 4;
                                break;
                        }
                        if (Delete_Color_Check[d] == false)
                        {
                            Delete_Color_Check[d] = true;
                            Delete_Color++;
                        }
                        if ((Puyo_Count * ((Conbo - 1) * 8 + (Puyo_Count - 4) + ((Delete_Color - 1) * 3)) * 10) == 0)
                        {
                            Score += Puyo_Count * 10;
                        }
                        else
                        {
                            Score += (Puyo_Count * ((Conbo - 1) * 8 + (Puyo_Count - 4) + ((Delete_Color - 1) * 3)) * 10);
                        }
                        Score_Text.text =  $"{Score}";
                        Delete_Start_PUYO_Data(x, y);
                    }
                    for (int a = 0; a < 78; a++)
                    {
                        Puyo_Delete[a % 6, a / 6] = false;
                    }
                    Puyo_Count = 0;
                }
            }
        }
        for (int b = 0; b < 78; b++)
        {
            Search[b % 6, b / 6] = false;
        }
        if (delete == false)
        {
            if (Puyo_Data[2, 11] != 'N') Game_End();
            else StartCoroutine(Summon_PUYO(0.5f));
        }
    }
    private void Add_PUYO_Counter(int x, int y)
    {
        int mx = 0;
        int my = 0;
        //確認前か確認
        if (Puyo_Delete[x, y] == false)
        {
            Puyo_Delete[x, y] = true;
            Search[x, y] = true;
            Puyo_Count++;
            //上下左右確認
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        mx = 0;
                        my = 1;
                        break;
                    case 1:
                        mx = 0;
                        my = -1;
                        break;
                    case 2:
                        mx = -1;
                        my = 0;
                        break;
                    case 3:
                        mx = 1;
                        my = 0;
                        break;
                }
                //フィールドの外じゃない                                             且つ参照先と元の色データが同じ
                if ((x + mx != -1 && x + mx != 6 && y + my != -1 && y + my != 13) && Puyo_Data[x + mx, y + my] == Puyo_Data[x, y])
                {
                    //色が合わないか確認仕切るまで繰り返す
                    Add_PUYO_Counter(x + mx, y + my);
                }
            }
        }
    }
    #endregion

    #region ぷよ消去
    //データ消去開始
    private void Delete_Start_PUYO_Data(int x, int y)
    {
        int mx = 0;
        int my = 0;
        char Orgin_Color = Puyo_Data[x, y];
        Puyo_Data[x, y] = 'N';
        //上下左右確認
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    mx = 0;
                    my = 1;
                    break;
                case 1:
                    mx = 0;
                    my = -1;
                    break;
                case 2:
                    mx = -1;
                    my = 0;
                    break;
                case 3:
                    mx = 1;
                    my = 0;
                    break;
            }
            //フィールドの外じゃない                                             且つ参照先と元の色データが同じ
            if ((x + mx != -1 && x + mx != 6 && y + my != -1 && y + my != 13) && Puyo_Data[x + mx, y + my] == Orgin_Color)
            {
                //連なった同じ色が全て消えるまで繰り返す
                Delete_PUYO_Data(x + mx, y + my);
            }
        }
        Delete_PUYO_Obj(Orgin_Color);
    }
    //データ消去
    private void Delete_PUYO_Data(int x, int y)
    {
        int mx = 0;
        int my = 0;
        char Orgin_Color = Puyo_Data[x, y];
        Puyo_Data[x, y] = 'N';
        //上下左右確認
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    mx = 0;
                    my = 1;
                    break;
                case 1:
                    mx = 0;
                    my = -1;
                    break;
                case 2:
                    mx = -1;
                    my = 0;
                    break;
                case 3:
                    mx = 1;
                    my = 0;
                    break;
            }
            //フィールドの外じゃない                                             且つ参照先と元の色データが同じ
            if ((x + mx != -1 && x + mx != 6 && y + my != -1 && y + my != 13) && Puyo_Data[x + mx, y + my] == Orgin_Color)
            {
                //連なった同じ色が全て消えるまで繰り返す
                Delete_PUYO_Data(x + mx, y + my);
            }
        }
    }
    //Obj消去
    private void Delete_PUYO_Obj(char Delete_Color)
    {
        for(int i = 0; i < 78; i++)
        {
            //データがN　　　　　　　　　　　　　且つオブジェクトが表示
            if(Puyo_Data[i % 6, i / 6] == 'N' && Set_Puyo[i].activeSelf == true)
            {
                StartCoroutine(Delete(Delete_Time, i,Delete_Color));
            }
        }
        StartCoroutine(Down_PUYO_Data(Delete_Time));
    }
    //Obj消去表現の処理
    private IEnumerator Delete(float time, int i,char Delete_Color)
    {
        int c = 0;
        //少し待って
        yield return new WaitForSeconds(time);
        //消す
        switch (Delete_Color)
        {
            case 'R':
                c = 0;
                break;
            case 'G':
                c = 1;
                break;
            case 'B':
                c = 2;
                break;
            case 'Y':
                c = 3;
                break;
            case 'P':
                c = 4;
                break;
        }
        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = Delete_Puyo[c];
        yield return new WaitForSeconds(time);
        Set_Puyo[i].SetActive(false);
    }
    #endregion

    #region 積みぷよ落下
    //消去後のデータ落下
    private IEnumerator Down_PUYO_Data(float time)
    {
        yield return new WaitForSeconds(time * 4);
        for (int i = 0; i < 78; i++)
        {
            if(Puyo_Data[i % 6, i / 6] == 'N')
            {
                int j = 1;
                while (Puyo_Data[i % 6, i / 6] == 'N' && i / 6 + j < 13)
                {
                    if (Puyo_Data[i % 6, i / 6 + j] != 'N')
                    {
                        Puyo_Data[i % 6, i / 6] = Puyo_Data[i % 6, i / 6 + j];
                        Puyo_Data[i % 6, i / 6 + j] = 'N';
                    }
                    else
                    {
                        j++;
                    }
                }
            }
        }
        StartCoroutine(Down_PUYO_Obj(Delete_Time));
    }
    //消去後のObj落下
    private IEnumerator Down_PUYO_Obj(float time)
    {
        for (int i = 0; i < 78; i++)
        {
            if (Puyo_Data[i % 6, i / 6] != 'N')
            {
                int WASD = 0;
                int Set_Color = 0;
                Set_Puyo[i].SetActive(true);
                //上下左右に同じ色がないか確認
                if (i / 6 != 12 && Puyo_Data[i % 6, i / 6] == Puyo_Data[i % 6, i / 6 + 1]) WASD += 1;
                if (i % 6 != 0 && Puyo_Data[i % 6, i / 6] == Puyo_Data[i % 6 - 1, i / 6]) WASD += 2;
                if (i / 6 != 0 && Puyo_Data[i % 6, i / 6] == Puyo_Data[i % 6, i / 6 - 1]) WASD += 4;
                if (i % 6 != 5 && Puyo_Data[i % 6, i / 6] == Puyo_Data[i % 6 + 1, i / 6]) WASD += 8;
                //色の確認
                switch (Puyo_Data[i % 6, i / 6])
                {
                    case 'R':
                        Set_Color = 0;
                        break;
                    case 'G':
                        Set_Color = 1;
                        break;
                    case 'B':
                        Set_Color = 2;
                        break;
                    case 'Y':
                        Set_Color = 3;
                        break;
                    case 'P':
                        Set_Color = 4;
                        break;
                }
                //画像貼り付け
                switch (WASD)
                {
                    case 0:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = Puyo[Set_Color];
                        break;
                    case 1:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = W_Puyo[Set_Color];
                        break;
                    case 2:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = A_Puyo[Set_Color];
                        break;
                    case 3:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = WA_Puyo[Set_Color];
                        break;
                    case 4:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = S_Puyo[Set_Color];
                        break;
                    case 5:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = WS_Puyo[Set_Color];
                        break;
                    case 6:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = AS_Puyo[Set_Color];
                        break;
                    case 7:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = WAS_Puyo[Set_Color];
                        break;
                    case 8:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = D_Puyo[Set_Color];
                        break;
                    case 9:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = WD_Puyo[Set_Color];
                        break;
                    case 10:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = AD_Puyo[Set_Color];
                        break;
                    case 11:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = WAD_Puyo[Set_Color];
                        break;
                    case 12:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = SD_Puyo[Set_Color];
                        break;
                    case 13:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = WSD_Puyo[Set_Color];
                        break;
                    case 14:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = ASD_Puyo[Set_Color];
                        break;
                    case 15:
                        Set_Puyo[i].GetComponent<SpriteRenderer>().sprite = WASD_Puyo[Set_Color];
                        break;
                }
            }
            else
            {
                Set_Puyo[i].SetActive(false);
            }
        }
        for (int i = 0; i < 78; i++)
        {
            Pre_Puyo[i].SetActive(false);
        }
        yield return new WaitForSeconds(time * 4);
        PUYO_Search();
    }
    #endregion
}
