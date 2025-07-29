using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //オブジェクト
    [SerializeField]
    private GameObject[] Board = new GameObject[64];    //オセロ盤
    [SerializeField]
    private GameObject[] Stone = new GameObject[64];    //石
    GameObject Click_Obj;                               //クリック先のオブジェクト
    [SerializeField]
    private GameObject Pass_Text;                       //パス用テキスト
    [SerializeField]
    private GameObject End_Text;                        //ゲーム終了用テキスト
    [SerializeField]
    private GameObject[] buttons = new GameObject[2];

    //データ
    private int[,] Stone_Data = new int[8, 8];          //ボードデータ　0=なし,1=黒,2=白
    private int Now_Turn;                               //現在のターン　0=黒,1=白　nt
    private bool[,] Possible_Set_Pos = new bool[8, 8];  //置ける位置　PSP
    private bool Set_Controll = true;                   //true=操作可能
    private bool Pass;                      //パス確認用
    private bool End = false;
    private int[] End_Count = new int[2];   //0=黒,1=白　どちらかが0or合計64になったら終了

    //マテリアル
    [SerializeField]
    private Material[] Board_Color = new Material[2];   //オセロ盤の色
    [SerializeField]
    private Material[] Stone_Color = new Material[2];   //石色　0=黒,1=白

    //SE
    [SerializeField]
    private AudioClip[] SE = new AudioClip[5];          //0=置く音,1=置けない音,2=パス,3=勝利,4=引き分け
    [SerializeField]
    private AudioSource audioSource;

    //テキスト
    [SerializeField]
    private Text[] text = new Text[2];  //0=ターン,1=ゲーム終了

    void Start()
    {
        Reset();
        Search_Set_Stone();
    }

    void Update()
    {
        StartCoroutine(Select_Board(0.2f));
    }

    //初期化
    private void Reset()
    {
        //データ初期化
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                Stone_Data[i, j] = 0;
            }
        }
        //初期石データ格納
        Stone_Data[3, 3] = 2;
        Stone_Data[3, 4] = 1;
        Stone_Data[4, 3] = 1;
        Stone_Data[4, 4] = 2;
        End_Count[0] = 2;
        End_Count[1] = 2;
        //初期石配置
        for (int i = 3; i < 5; i++)
        {
            for(int j = 3; j < 5; j++)
            {
                int Reverse_Pos = Convert2to1(i, j);
                //石表示
                Stone[Reverse_Pos].SetActive(true);
                //色変更
                Stone[Reverse_Pos].GetComponent<Renderer>().material = Stone_Color[Stone_Data[i, j] - 1];
            }
        }
        //黒のターンに変更
        Now_Turn = 0;
        text[0].text = "黒のターン";
    }

    //2次元配列→1次元配列の変換
    private int Convert2to1(int y,int x)
    {
        int c;
        c = (8 * y) + x;
        return c;
    }

    //1次元配列→2次元配列の変換
    private int[] Convert1to2(int a)
    {
        //0=y,1=x
        int[] xy = new int[2];
        xy[1] = a % 8;
        xy[0] = a / 8;
        return xy;
    }

    //石配置
    private void Set_Stone(int y,int x)
    {
        int Reverse_Pos = Convert2to1(y, x);
        //石表示
        Stone[Reverse_Pos].SetActive(true);
        //色変更
        Stone[Reverse_Pos].GetComponent<Renderer>().material = Stone_Color[Stone_Data[y,x]-1];
        audioSource.PlayOneShot(SE[0]);
    }

    private void Turn_Change()
    {
        switch (Now_Turn)
        {
            case 0:
                Now_Turn = 1;
                text[0].text = "白のターン";
                break;
            case 1:
                Now_Turn = 0;
                text[0].text = "黒のターン";
                break;
        }
    }

    //置ける位置確認
    private void Search_Set_Stone()
    {
        Pass = true;
        //PSPの初期化とオセロ盤の初期化...とゲーム終了確認
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                Possible_Set_Pos[i, j] = false;
                int a = Convert2to1(i, j);
                Board[a].GetComponent<Renderer>().material = Board_Color[0];
            }
        }
        //置ける位置捜索
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                //確認マスから8方向確認　0上、1右上,2右,3右下,4下,5左下,6左,7左上
                for(int k = 0; k < 8; k++)
                {
                    //PSPがfalseなら捜索
                    if (Possible_Set_Pos[i, j] == false && Stone_Data[i, j] == 0)
                    {
                        int x = j;
                        int y = i;
                        int Add_x = 0;
                        int Add_y = 0;
                        bool Search = true;
                        bool Change = false;
                        //確認方向指定
                        switch (k)
                        {
                            case 0:
                                Add_x = 0;
                                Add_y = -1;
                                break;
                            case 1:
                                Add_x = 1;
                                Add_y = -1;
                                break;
                            case 2:
                                Add_x = 1;
                                Add_y = 0;
                                break;
                            case 3:
                                Add_x = 1;
                                Add_y = 1;
                                break;
                            case 4:
                                Add_x = 0;
                                Add_y = 1;
                                break;
                            case 5:
                                Add_x = -1;
                                Add_y = 1;
                                break;
                            case 6:
                                Add_x = -1;
                                Add_y = 0;
                                break;
                            case 7:
                                Add_x = -1;
                                Add_y = -1;
                                break;
                        }
                        //異色の石に当たるか
                        while (Search == true)
                        {
                            x = x + Add_x;
                            y = y + Add_y;
                            //盤外or石がない場合or同色の石&Changeフラグがfalseの場合は捜索中止
                            if (x == -1 || y == -1 || x == 8 || y == 8 || Stone_Data[y, x] == 0 || Stone_Data[y, x] == Now_Turn + 1 && Change == false)
                            {
                                Search = false;
                            }
                            //同色の石&Changeフラグがtrueの場合はPSPをtrueへ
                            else if (Stone_Data[y, x] == Now_Turn + 1 && Change == true)
                            {
                                Possible_Set_Pos[i, j] = true;
                                Search = false;
                                Pass = false;
                            }
                            //異色の石の場合はChangeをtrueへ
                            else
                            {
                                Change = true;
                            }
                        }
                    }
                }
            }
        }
        if (Pass == false)
        {
            //PSPがtrueなら盤面を黄色に変更
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Possible_Set_Pos[i, j] == true)
                    {
                        int a = Convert2to1(i, j);
                        Board[a].GetComponent<Renderer>().material = Board_Color[1];
                    }
                }
            }
        }
    }

    //ボードを選択し石を置く
    private IEnumerator Select_Board(float time)
    {
        if (Input.GetMouseButtonDown(0) && Set_Controll == true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
            if (hit2D)
            {
                Click_Obj = hit2D.transform.gameObject;
                int a = int.Parse(Click_Obj.name);
                //0=y,1=x
                int[] b = Convert1to2(a);
                //置ける場合
                if (Possible_Set_Pos[b[0], b[1]] == true)
                {
                    Set_Controll = false;
                    Stone_Data[b[0], b[1]] = Now_Turn + 1;
                    End_Count[Now_Turn]++;
                    Set_Stone(b[0], b[1]);
                    //確認マスから8方向確認　0上、1右上,2右,3右下,4下,5左下,6左,7左上
                    for (int i = 0; i < 8; i++)
                    {
                        int x = b[1];
                        int y = b[0];
                        int Add_x = 0;
                        int Add_y = 0;
                        bool Search = true;
                        bool Change = false;
                        //確認方向指定
                        switch (i)
                        {
                            case 0:
                                Add_x = 0;
                                Add_y = -1;
                                break;
                            case 1:
                                Add_x = 1;
                                Add_y = -1;
                                break;
                            case 2:
                                Add_x = 1;
                                Add_y = 0;
                                break;
                            case 3:
                                Add_x = 1;
                                Add_y = 1;
                                break;
                            case 4:
                                Add_x = 0;
                                Add_y = 1;
                                break;
                            case 5:
                                Add_x = -1;
                                Add_y = 1;
                                break;
                            case 6:
                                Add_x = -1;
                                Add_y = 0;
                                break;
                            case 7:
                                Add_x = -1;
                                Add_y = -1;
                                break;
                        }
                        //異色の石に当たるか
                        while (Search == true)
                        {
                            x = x + Add_x;
                            y = y + Add_y;
                            //盤外or石がない場合or同色の石&Changeフラグがfalseの場合は捜索中止
                            if (x == -1 || y == -1 || x == 8 || y == 8 || Stone_Data[y, x] == 0 || Stone_Data[y, x] == Now_Turn + 1 && Change == false)
                            {
                                Search = false;
                            }
                            //同色の石&Changeフラグがtrueの場合はリバース処理
                            else if (Stone_Data[y, x] == Now_Turn + 1 && Change == true)
                            {
                                x = b[1] + Add_x;
                                y = b[0] + Add_y;
                                while (Stone_Data[y, x] != Now_Turn + 1)
                                {
                                    Stone_Data[y, x] = Now_Turn + 1;
                                    yield return new WaitForSeconds(time);
                                    if (Now_Turn + 1 == 1)
                                    {
                                        End_Count[0]++;
                                        End_Count[1]--;
                                    }
                                    else
                                    {
                                        End_Count[1]++;
                                        End_Count[0]--;
                                    }
                                    Set_Stone(y, x);
                                    x = x + Add_x;
                                    y = y + Add_y;
                                }
                                Search = false;
                            }
                            //異色の石の場合はChangeをtrueへ
                            else
                            {
                                Change = true;
                            }
                        }
                    }
                    Turn_Change();
                    Search_Set_Stone();
                    //エンド処理
                    if(End_Count[0]==0 || End_Count[1]==0 || End_Count[0] + End_Count[1] == 64)
                    {
                        End = true;
                        buttons[0].SetActive(true);
                        buttons[1].SetActive(true);
                        End_Text.SetActive(true);
                        if (End_Count[0] == 0 || End_Count[0]<End_Count[1])
                        {
                            text[1].text = "白の勝利";
                            audioSource.PlayOneShot(SE[3]);
                        }
                        else if (End_Count[1] == 0 || End_Count[1] < End_Count[0])
                        {
                            text[1].text = "黒の勝利";
                            audioSource.PlayOneShot(SE[3]);
                        }
                        else
                        {
                            text[1].text = "引き分け";
                            audioSource.PlayOneShot(SE[4]);
                        }
                    }
                    //パスの処理
                    else if (Pass == true)
                    {
                        Pass_Text.SetActive(true);
                        audioSource.PlayOneShot(SE[2]);
                        yield return new WaitForSeconds(2.0f);
                        Pass_Text.SetActive(false);
                        Turn_Change();
                        Search_Set_Stone();
                    }
                    if (End == false)
                    {
                        Set_Controll = true;
                    }
                }
                //置けない場合
                else
                {
                    audioSource.PlayOneShot(SE[1]);
                }
            }
        }
    }
}