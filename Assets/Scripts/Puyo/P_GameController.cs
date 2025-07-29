using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Pre_Puyo = new GameObject[84];
    [SerializeField]
    private GameObject[] Set_Puyo = new GameObject[72];
    [SerializeField]
    GameObject Move_Puyo;
    [SerializeField]
    private Material[] Puyo_Color = new Material[5];//RGBYP
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
    public char[,] Puyo_Data = new char[6,14];//RGBYP N
    public int[] N_Color = new int[2];//�������̐F
    public int[] Color = new int[2];//�������̐F
    private bool[,] pro_Puyo_Delete = new bool[8,12];
    private bool[,] Search = new bool[8, 12];
    private int Puyo_Count = 0;
    public static P_GameController GC;
    private float Delete_Time = 0.25f;
    [SerializeField]
    private AudioClip[] SE = new AudioClip[5];
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private Text Conbo_Text;
    [SerializeField]
    private Text Score_Text;
    [SerializeField]
    private GameObject Conbo_Obj;

    private int Score = 0;
    private int Conbo = 0;
    private bool Conbo_Check = false;
    private int Delete_Color = 0;
    private bool[] Delete_Color_Check = new bool[5];
    private bool delete = false;
    private bool Game_Over_Flag = false;

    [SerializeField]
    private GameObject Fade;
    [SerializeField]
    private Text Fade_Text;
    [SerializeField]
    private GameObject[] Game_Over_Obj = new GameObject[3];
    [SerializeField]
    private Text Game_Over_Text;

    

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
        for(int i=0; i < 14; i++)
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
        
    }

    private IEnumerator Game_Start(float time)
    {
        audioSource.PlayOneShot(SE[2]);
        Fade_Text.text = "�p�ӁE�E�E";
        yield return new WaitForSeconds(time);
        audioSource.PlayOneShot(SE[3]);
        Fade_Text.text = "�X�^�[�g!!";
        yield return new WaitForSeconds(time);
        Fade.SetActive(false);
        StartCoroutine(Summon_PUYO(0.5f));
    }


    //���m�F
    private void PUYO_Search()
    {
        int x = 0;
        int y = 0;
        Puyo_Count = 0;
        int mx = 0;
        int my = 0;
        Conbo_Check = false;
        delete = false;
        for (int i = 0; i < 12; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                y = i;
                x = j;
                //���T��                   �@���J���[�f�[�^��������Ȃ�
                if (Search[x, y] == false && Puyo_Data[x, y] != 'N')
                {
                    Search[x, y] = true;
                    Puyo_Count++;
                    //�㉺���E�m�F
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
                        //�t�B�[���h�̊O����Ȃ��@�@                                         ���Q�Ɛ�ƎQ�ƌ��̐F�f�[�^������
                        if ((x + mx != -1 && x + mx != 6 && y + my != -1 && y + my != 12) && Puyo_Data[x + mx, y + my] == Puyo_Data[x, y])
                        {
                            pro_Puyo_Delete[x, y] = true;
                            Add_PUYO_Counter(x + mx, y + my);
                        }
                    }
                    //Debug.Log(Puyo_Count);
                    //Debug.Log($"\n{Puyo_Data[0, 2]}{Puyo_Data[1, 2]}{Puyo_Data[2, 2]}{Puyo_Data[3, 2]}{Puyo_Data[4, 2]}{Puyo_Data[5, 2]}\n{Puyo_Data[0, 1]}{Puyo_Data[1, 1]}{Puyo_Data[2, 1]}{Puyo_Data[3, 1]}{Puyo_Data[4, 1]}{Puyo_Data[5, 1]}\n{Puyo_Data[0, 0]}{Puyo_Data[1, 0]}{Puyo_Data[2, 0]}{Puyo_Data[3, 0]}{Puyo_Data[4, 0]}{Puyo_Data[5, 0]}");
                    if (Puyo_Count >= 4)
                    {
                        delete = true;
                        //Debug.Log($"Count={Puyo_Count}");
                        //�R���{
                        if (Conbo_Check == false)
                        {
                            Conbo++;
                            Conbo_Obj.SetActive(true);
                            Conbo_Text.text = $"{Conbo}�A��!!";
                            audioSource.PlayOneShot(SE[1]);
                            Conbo_Check = true;
                        }
                        //�X�R�A
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
                        Delete_PUYO_Data(x, y);
                    }
                    //Debug.Log($"2�i��{pro_Puyo_Delete[0, 1]}{pro_Puyo_Delete[1, 1]}{pro_Puyo_Delete[2, 1]}{pro_Puyo_Delete[3, 1]}{pro_Puyo_Delete[4, 1]}{pro_Puyo_Delete[5, 1]}");
                    //Debug.Log($"1�i��{pro_Puyo_Delete[0, 0]}{pro_Puyo_Delete[1, 0]}{pro_Puyo_Delete[2, 0]}{pro_Puyo_Delete[3, 0]}{pro_Puyo_Delete[4, 0]}{pro_Puyo_Delete[5, 0]}");
                    for (int a = 0; a < 72; a++)
                    {
                        pro_Puyo_Delete[a % 6, a / 6] = false;
                    }
                    Puyo_Count = 0;
                }
            }
        }
        for (int b = 0; b < 72; b++)
        {
            Search[b % 6, b / 6] = false;
        }
        if (delete == false)
        {
            StartCoroutine(Summon_PUYO(0.5f));
        }
    }

    private void Add_PUYO_Counter(int x, int y)
    {
        int mx = 0;
        int my = 0;
        //�m�F�O���m�F
        if (pro_Puyo_Delete[x, y] == false)
        {
            pro_Puyo_Delete[x, y] = true;
            Search[x, y] = true;
            Puyo_Count++;
            //�㉺���E�m�F
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
                //�t�B�[���h�̊O����Ȃ�                                             ���Q�Ɛ�ƌ��̐F�f�[�^������
                if ((x + mx != -1 && x + mx != 6 && y + my != -1 && y + my != 12) && Puyo_Data[x + mx, y + my] == Puyo_Data[x, y])
                {
                    //�F������Ȃ����m�F�d�؂�܂ŌJ��Ԃ�
                    Add_PUYO_Counter(x + mx, y + my);
                }
            }
        }
    }

    //����
    private void Delete_PUYO_Data(int x, int y)
    {
        int mx = 0;
        int my = 0;
        char Orgin_Color = Puyo_Data[x, y];
        Puyo_Data[x, y] = 'N';
        //�㉺���E�m�F
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
            //�t�B�[���h�̊O����Ȃ�                                             ���Q�Ɛ�ƌ��̐F�f�[�^������
            if ((x + mx != -1 && x + mx != 6 && y + my != -1 && y + my != 12) && Puyo_Data[x + mx, y + my] == Orgin_Color)
            {
                //�A�Ȃ��������F���S�ď�����܂ŌJ��Ԃ�
                //Debug.Log($"x={x + mx},y={y + my}");
                Second_Delete_PUYO_Data(x + mx, y + my);
            }
        }
        Delete_PUYO_Obj(Orgin_Color);
    }

    private void Second_Delete_PUYO_Data(int x, int y)
    {
        int mx = 0;
        int my = 0;
        char Orgin_Color = Puyo_Data[x, y];
        Puyo_Data[x, y] = 'N';
        //�㉺���E�m�F
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
            //�t�B�[���h�̊O����Ȃ�                                             ���Q�Ɛ�ƌ��̐F�f�[�^������
            if ((x + mx != -1 && x + mx != 6 && y + my != -1 && y + my != 12) && Puyo_Data[x + mx, y + my] == Orgin_Color)
            {
                //�A�Ȃ��������F���S�ď�����܂ŌJ��Ԃ�
                //Debug.Log($"x={x + mx},y={y + my}");
                Second_Delete_PUYO_Data(x + mx, y + my);
            }
        }
    }

    private void Delete_PUYO_Obj(char Delete_Color)
    {
        for(int i = 0; i < 72; i++)
        {
            //�f�[�^��N�@�@�@�@�@�@�@�@�@�@�@�@�@���I�u�W�F�N�g���\��
            if(Puyo_Data[i % 6, i / 6] == 'N' && Set_Puyo[i].activeSelf == true)
            {
                StartCoroutine(Delete(Delete_Time, i,Delete_Color));
            }
        }
        StartCoroutine(Down_PUYO_Data(Delete_Time));
    }

    private IEnumerator Delete(float time, int i,char Delete_Color)
    {
        int c = 0;
        //�����҂���
        yield return new WaitForSeconds(time);
        //����
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

    private IEnumerator Down_PUYO_Data(float time)
    {
        yield return new WaitForSeconds(time * 4);
        for (int i = 0; i < 72; i++)
        {
            if(Puyo_Data[i % 6, i / 6] == 'N')
            {
                int j = 1;
                while (Puyo_Data[i % 6, i / 6] == 'N' && i / 6 + j < 12)
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

    private IEnumerator Down_PUYO_Obj(float time)
    {
        for (int i = 0; i < 72; i++)
        {
            if (Puyo_Data[i % 6, i / 6] != 'N')
            {
                int WASD = 0;
                int Set_Color = 0;
                Set_Puyo[i].SetActive(true);
                //�㉺���E�ɓ����F���Ȃ����m�F
                if (i / 6 != 12 && Puyo_Data[i % 6, i / 6] == Puyo_Data[i % 6, i / 6 + 1]) WASD += 1;
                if (i % 6 != 0 && Puyo_Data[i % 6, i / 6] == Puyo_Data[i % 6 - 1, i / 6]) WASD += 2;
                if (i / 6 != 0 && Puyo_Data[i % 6, i / 6] == Puyo_Data[i % 6, i / 6 - 1]) WASD += 4;
                if (i % 6 != 5 && Puyo_Data[i % 6, i / 6] == Puyo_Data[i % 6 + 1, i / 6]) WASD += 8;
                //if (Puyo_Data[i % 6, i / 6] == 'R') Debug.Log(WASD);
                //�F�̊m�F
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
                //�摜�\��t��
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
        for (int i = 0; i < 72; i++)
        {
            Pre_Puyo[i].SetActive(false);
        }
        yield return new WaitForSeconds(time * 4);
        PUYO_Search();
    }

    private void Rand_Color()
    {
        for (int i = 0; i < 2; i++)
        {
            Color[i] = N_Color[i];
            N_Color[i] = Random.Range(0, 5);
        }
    }

    //�Ղ揢��
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
        float x = 0.35f;
        float y = 5f;
        float z = 0;
        if(Puyo_Data[3,11]!='N')
        {
            for(int i = 0; i < 6; i++)
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
    }

    //�u���ʒu�\��
    public void Move_judge(float w_x,float w_angle)
    {
        bool end = false;
        int x = 0;
        for (int i = 0; i < 84; i++)
        {
            Pre_Puyo[i].SetActive(false);
        }
        switch ((int)(w_x * 3))
        {
            case -5:
                x = 0;
                break;
            case -3:
                x = 1;
                break;
            case -1:
                x = 2;
                break;
            case 1:
                x = 3;
                break;
            case 3:
                x = 4;
                break;
            case 5:
                x = 5;
                break;
        }
        int y = 0;
        switch ((int)w_angle)
        {
            case 0:
            case 360:
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
            case 90:
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
            case 180:
            case -180:
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
            case 270:
            case -90:
            case -89:
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
    
    public void Set_PUYO_Data()
    {
        for(int i = 0; i < 78; i++)
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
        for(int i = 0; i < 6; i++)
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
            audioSource.PlayOneShot(SE[4]);
            Fade_Text.text = "�I��!!";
            Game_Over_Text.text = $"�X�R�A�F{Score}";
            Fade.SetActive(true);
            Game_Over_Obj[0].SetActive(true);
            Game_Over_Obj[1].SetActive(true);
            Game_Over_Obj[2].SetActive(true);
        }
    }

    public void Set_PUYO_Obj()
    {
        audioSource.PlayOneShot(SE[0]);
        for (int i = 0; i < 72; i++)
        {
            if (Puyo_Data[i % 6, i / 6] != 'N')
            {
                int WASD = 0;
                int Set_Color = 0;
                Set_Puyo[i].SetActive(true);
                //�㉺���E�ɓ����F���Ȃ����m�F
                if (i / 6 != 12 && Puyo_Data[i % 6, i / 6] == Puyo_Data[i % 6, i / 6 + 1]) WASD += 1;
                if (i % 6 != 0 && Puyo_Data[i % 6, i / 6] == Puyo_Data[i % 6 - 1, i / 6]) WASD += 2;
                if (i / 6 != 0 && Puyo_Data[i % 6, i / 6] == Puyo_Data[i % 6, i / 6 - 1]) WASD += 4;
                if (i % 6 != 5 && Puyo_Data[i % 6, i / 6] == Puyo_Data[i % 6 + 1, i / 6]) WASD += 8;
                //if (Puyo_Data[i % 6, i / 6] == 'R') Debug.Log(WASD);
                //�F�̊m�F
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
                //�摜�\��t��
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
        for (int i = 0; i < 72; i++)
        {
            Pre_Puyo[i].SetActive(false);
        }
        PUYO_Search();
    }
}
