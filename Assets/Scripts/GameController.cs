using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //�I�u�W�F�N�g
    [SerializeField]
    private GameObject[] Board = new GameObject[64];    //�I�Z����
    [SerializeField]
    private GameObject[] Stone = new GameObject[64];    //��
    GameObject Click_Obj;                               //�N���b�N��̃I�u�W�F�N�g
    [SerializeField]
    private GameObject Pass_Text;                       //�p�X�p�e�L�X�g
    [SerializeField]
    private GameObject End_Text;                        //�Q�[���I���p�e�L�X�g
    [SerializeField]
    private GameObject[] buttons = new GameObject[2];

    //�f�[�^
    private int[,] Stone_Data = new int[8, 8];          //�{�[�h�f�[�^�@0=�Ȃ�,1=��,2=��
    private int Now_Turn;                               //���݂̃^�[���@0=��,1=���@nt
    private bool[,] Possible_Set_Pos = new bool[8, 8];  //�u����ʒu�@PSP
    private bool Set_Controll = true;                   //true=����\
    private bool Pass;                      //�p�X�m�F�p
    private bool End = false;
    private int[] End_Count = new int[2];   //0=��,1=���@�ǂ��炩��0or���v64�ɂȂ�����I��

    //�}�e���A��
    [SerializeField]
    private Material[] Board_Color = new Material[2];   //�I�Z���Ղ̐F
    [SerializeField]
    private Material[] Stone_Color = new Material[2];   //�ΐF�@0=��,1=��

    //SE
    [SerializeField]
    private AudioClip[] SE = new AudioClip[5];          //0=�u����,1=�u���Ȃ���,2=�p�X,3=����,4=��������
    [SerializeField]
    private AudioSource audioSource;

    //�e�L�X�g
    [SerializeField]
    private Text[] text = new Text[2];  //0=�^�[��,1=�Q�[���I��

    void Start()
    {
        Reset();
        Search_Set_Stone();
    }

    void Update()
    {
        StartCoroutine(Select_Board(0.2f));
    }

    //������
    private void Reset()
    {
        //�f�[�^������
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                Stone_Data[i, j] = 0;
            }
        }
        //�����΃f�[�^�i�[
        Stone_Data[3, 3] = 2;
        Stone_Data[3, 4] = 1;
        Stone_Data[4, 3] = 1;
        Stone_Data[4, 4] = 2;
        End_Count[0] = 2;
        End_Count[1] = 2;
        //�����Δz�u
        for (int i = 3; i < 5; i++)
        {
            for(int j = 3; j < 5; j++)
            {
                int Reverse_Pos = Convert2to1(i, j);
                //�Ε\��
                Stone[Reverse_Pos].SetActive(true);
                //�F�ύX
                Stone[Reverse_Pos].GetComponent<Renderer>().material = Stone_Color[Stone_Data[i, j] - 1];
            }
        }
        //���̃^�[���ɕύX
        Now_Turn = 0;
        text[0].text = "���̃^�[��";
    }

    //2�����z��1�����z��̕ϊ�
    private int Convert2to1(int y,int x)
    {
        int c;
        c = (8 * y) + x;
        return c;
    }

    //1�����z��2�����z��̕ϊ�
    private int[] Convert1to2(int a)
    {
        //0=y,1=x
        int[] xy = new int[2];
        xy[1] = a % 8;
        xy[0] = a / 8;
        return xy;
    }

    //�Δz�u
    private void Set_Stone(int y,int x)
    {
        int Reverse_Pos = Convert2to1(y, x);
        //�Ε\��
        Stone[Reverse_Pos].SetActive(true);
        //�F�ύX
        Stone[Reverse_Pos].GetComponent<Renderer>().material = Stone_Color[Stone_Data[y,x]-1];
        audioSource.PlayOneShot(SE[0]);
    }

    private void Turn_Change()
    {
        switch (Now_Turn)
        {
            case 0:
                Now_Turn = 1;
                text[0].text = "���̃^�[��";
                break;
            case 1:
                Now_Turn = 0;
                text[0].text = "���̃^�[��";
                break;
        }
    }

    //�u����ʒu�m�F
    private void Search_Set_Stone()
    {
        Pass = true;
        //PSP�̏������ƃI�Z���Ղ̏�����...�ƃQ�[���I���m�F
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                Possible_Set_Pos[i, j] = false;
                int a = Convert2to1(i, j);
                Board[a].GetComponent<Renderer>().material = Board_Color[0];
            }
        }
        //�u����ʒu�{��
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                //�m�F�}�X����8�����m�F�@0��A1�E��,2�E,3�E��,4��,5����,6��,7����
                for(int k = 0; k < 8; k++)
                {
                    //PSP��false�Ȃ�{��
                    if (Possible_Set_Pos[i, j] == false && Stone_Data[i, j] == 0)
                    {
                        int x = j;
                        int y = i;
                        int Add_x = 0;
                        int Add_y = 0;
                        bool Search = true;
                        bool Change = false;
                        //�m�F�����w��
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
                        //�ِF�̐΂ɓ����邩
                        while (Search == true)
                        {
                            x = x + Add_x;
                            y = y + Add_y;
                            //�ՊOor�΂��Ȃ��ꍇor���F�̐�&Change�t���O��false�̏ꍇ�͑{�����~
                            if (x == -1 || y == -1 || x == 8 || y == 8 || Stone_Data[y, x] == 0 || Stone_Data[y, x] == Now_Turn + 1 && Change == false)
                            {
                                Search = false;
                            }
                            //���F�̐�&Change�t���O��true�̏ꍇ��PSP��true��
                            else if (Stone_Data[y, x] == Now_Turn + 1 && Change == true)
                            {
                                Possible_Set_Pos[i, j] = true;
                                Search = false;
                                Pass = false;
                            }
                            //�ِF�̐΂̏ꍇ��Change��true��
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
            //PSP��true�Ȃ�Ֆʂ����F�ɕύX
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

    //�{�[�h��I�����΂�u��
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
                //�u����ꍇ
                if (Possible_Set_Pos[b[0], b[1]] == true)
                {
                    Set_Controll = false;
                    Stone_Data[b[0], b[1]] = Now_Turn + 1;
                    End_Count[Now_Turn]++;
                    Set_Stone(b[0], b[1]);
                    //�m�F�}�X����8�����m�F�@0��A1�E��,2�E,3�E��,4��,5����,6��,7����
                    for (int i = 0; i < 8; i++)
                    {
                        int x = b[1];
                        int y = b[0];
                        int Add_x = 0;
                        int Add_y = 0;
                        bool Search = true;
                        bool Change = false;
                        //�m�F�����w��
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
                        //�ِF�̐΂ɓ����邩
                        while (Search == true)
                        {
                            x = x + Add_x;
                            y = y + Add_y;
                            //�ՊOor�΂��Ȃ��ꍇor���F�̐�&Change�t���O��false�̏ꍇ�͑{�����~
                            if (x == -1 || y == -1 || x == 8 || y == 8 || Stone_Data[y, x] == 0 || Stone_Data[y, x] == Now_Turn + 1 && Change == false)
                            {
                                Search = false;
                            }
                            //���F�̐�&Change�t���O��true�̏ꍇ�̓��o�[�X����
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
                            //�ِF�̐΂̏ꍇ��Change��true��
                            else
                            {
                                Change = true;
                            }
                        }
                    }
                    Turn_Change();
                    Search_Set_Stone();
                    //�G���h����
                    if(End_Count[0]==0 || End_Count[1]==0 || End_Count[0] + End_Count[1] == 64)
                    {
                        End = true;
                        buttons[0].SetActive(true);
                        buttons[1].SetActive(true);
                        End_Text.SetActive(true);
                        if (End_Count[0] == 0 || End_Count[0]<End_Count[1])
                        {
                            text[1].text = "���̏���";
                            audioSource.PlayOneShot(SE[3]);
                        }
                        else if (End_Count[1] == 0 || End_Count[1] < End_Count[0])
                        {
                            text[1].text = "���̏���";
                            audioSource.PlayOneShot(SE[3]);
                        }
                        else
                        {
                            text[1].text = "��������";
                            audioSource.PlayOneShot(SE[4]);
                        }
                    }
                    //�p�X�̏���
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
                //�u���Ȃ��ꍇ
                else
                {
                    audioSource.PlayOneShot(SE[1]);
                }
            }
        }
    }
}