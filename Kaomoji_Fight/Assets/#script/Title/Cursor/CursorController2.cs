using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using TMPro;

public class CursorController2 : MonoBehaviour {

    [SerializeField]
    private GameObject TManager;            //Titleマネージャー
    private NT_TitleManager TManager_cs;   //Titleマネージャーcs

    private GameObject text;

    [SerializeField]
    private GameObject[] target;    //ターゲット

    private int Vertical = 5;       //縦の数
    private int Horizontal = 11;    //横の数

    private int VerticalCount = 0;          //縦の今の番号
    private int HorizontalCount = 0;        //横の今の番号

    private bool LeftStickflag = false;           //スティックが入力されていない(false)された(true)
    private Vector2 LeftStickInput = Vector2.zero;  //Controllerの左スティックのAxisを取得

    private GameObject[][] TargetMozi;      //ターゲットをジャグ配列で整理

    [SerializeField]
    private GameObject[] NameText;  //名前を入れるテキスト
    private int TextNamber = 0;     //テキスト番号

    private bool BackSpace = false;//バックスペースフラグ

    //変換表
    private static string[][] ChangeMozi = new string[][]
        {
            new string[2]{"か", "が"}, new string[2]{"き", "ぎ"}, new string[2]{"く", "ぐ"}, new string[2]{"け", "げ"}, new string[2]{"こ", "ご"},
            new string[2]{"さ", "ざ"}, new string[2]{"し", "じ"}, new string[2]{"す", "ず"}, new string[2]{"せ", "ぜ"}, new string[2]{"そ", "ぞ"},
            new string[2]{"た", "だ"}, new string[2]{"ち", "ぢ"}, new string[3]{"つ", "づ", "っ"}, new string[2]{"て", "で"}, new string[2]{"と", "ど"},
            new string[3]{"は", "ば", "ぱ"}, new string[3]{"ひ", "び", "ぴ"}, new string[3]{"ふ", "ぶ", "ぷ"}, new string[3]{"へ", "べ", "ぺ"}, new string[3]{"ほ", "ぼ", "ぽ"},

            new string[2]{"あ", "ぁ"}, new string[2]{"い", "ぃ"}, new string[2]{"う", "ぅ"}, new string[2]{"え", "ぇ"}, new string[2]{"お", "ぉ"},
            new string[2]{"や", "ゃ"}, new string[2]{"ゆ", "ゅ"}, new string[2]{"よ", "ょ"}
        };

    void Start() {
        //各csの取得
        TManager_cs = TManager.GetComponent<NT_TitleManager>();
        text = this.transform.parent.GetChild(1).GetChild(0).gameObject;

        this.transform.position = target[0].transform.position;

        //文字設定
        TargetMozi = new GameObject[Horizontal][];

        //は行まで
        for (int i = 0; i < 7; i++)
        {
            TargetMozi[i] = new GameObject[5];
            for(int j = 0; j < 5; j++)
            {
                TargetMozi[i][j] = target[i * 5 + j];
            }
        }

        //や行
        TargetMozi[7] = new GameObject[3];
        for(int i = 0; i < 3; i++)
        {
            TargetMozi[7][i] = target[35 + i];
        }

        //ら行
        TargetMozi[8] = new GameObject[5];
        for(int i = 0; i < 5; i++)
        {
            TargetMozi[8][i] = target[38 + i];
        }

        //わ行
        TargetMozi[9] = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            TargetMozi[9][i] = target[43 + i];
        }

        //バック・エンター
        TargetMozi[10] = new GameObject[2];
        for (int i = 0; i < 2; i++)
        {
            TargetMozi[10][i] = target[48 + i];
        }
    }

    private void Update()
    {
        //自分が選ばれていなかったら
        if (TManager_cs.SelectMode_Data != NT_TitleManager.SELECTMODE.NAME)
        {
            this.transform.GetComponent<SpriteRenderer>().enabled = false;
            text.SetActive(false);
            return;
        }
        else
        {
            this.transform.GetComponent<SpriteRenderer>().enabled = true;
            text.SetActive(true);
        }

        // Controllerの左スティックのAxisを取得            
        LeftStickInput = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, XboxController.First), XCI.GetAxis(XboxAxis.LeftStickY, XboxController.First));

        //上入力
        if (Input.GetKeyDown(KeyCode.UpArrow) || XCI.GetDPadDown(XboxDPad.Up, XboxController.First) || (LeftStickInput.y > 0.9f && LeftStickflag == false))
        {
            VerticalCount--;
            LeftStickflag = true;

            //下に戻る
            if (VerticalCount < 0)
            {
                VerticalCount = TargetMozi[HorizontalCount].Length - 1;
            }
        }
        //下入力
        else if (Input.GetKeyDown(KeyCode.DownArrow) || XCI.GetDPadDown(XboxDPad.Down, XboxController.First) || (LeftStickInput.y < -0.9f && LeftStickflag == false))
        {
            VerticalCount++;
            LeftStickflag = true;           

            //上に戻る
            if (VerticalCount > TargetMozi[HorizontalCount].Length - 1)
            {
                VerticalCount = 0;
            }
        }
        //右入力
        else if (Input.GetKeyDown(KeyCode.RightArrow) || XCI.GetDPadDown(XboxDPad.Right, XboxController.First) || (LeftStickInput.x > 0.9f && LeftStickflag == false))
        {
            HorizontalCount--;
            LeftStickflag = true;

            //や行を出るとき
            if (HorizontalCount == 6)
            {
                //よ→も
                if(VerticalCount == 2) VerticalCount = 4;
                //ゆ→む
                if (VerticalCount == 1) VerticalCount = 2;
            }
            //や行に入るとき
            if (HorizontalCount == 7)
            {
                //り・れ→み・め
                if (VerticalCount == 1 || VerticalCount == 3) HorizontalCount--;
                //る→ゆ
                if (VerticalCount == 2) VerticalCount = 1;
                //ろ→よ
                if (VerticalCount == 4) VerticalCount = 2;
            }

            //わ行に入るとき
            if (HorizontalCount == 9)
            {
                //エンター→ん
                if (VerticalCount == 1) { VerticalCount = 2; }
            }

            //最後に移動
            if (HorizontalCount < 0)
            {
                HorizontalCount = Horizontal - 1;

                //あ・い→バック
                if (VerticalCount == 1) VerticalCount = 0;
                //う・え・お→エンター
                if (VerticalCount == 2 || VerticalCount == 3 || VerticalCount == 4) VerticalCount = 1;
            }
        }
        //左入力
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || XCI.GetDPadDown(XboxDPad.Left, XboxController.First) || (LeftStickInput.x < -0.9f && LeftStickflag == false))
        {
            HorizontalCount++;
            LeftStickflag = true;

            //や行を出るとき
            if (HorizontalCount == 8)
            {
                //よ→ろ
                if(VerticalCount == 2) VerticalCount = 4;
                //ゆ→り
                if (VerticalCount == 1) VerticalCount = 2;
            }

            //や行に入るとき
            if (HorizontalCount == 7)
            {
                //み・め→り・れ
                if(VerticalCount == 1 || VerticalCount == 3) HorizontalCount++;
                //む→ゆ
                if (VerticalCount == 2) VerticalCount = 1;
                //も→よ
                if(VerticalCount == 4) VerticalCount = 2;
            }

            //わ行を出るとき
            if (HorizontalCount == 10)
            {
                //を→バック
                if (VerticalCount == 1) VerticalCount = 0;
                //ん・小濁点→エンター
                if (VerticalCount == 2 || VerticalCount == 3 || VerticalCount == 4) VerticalCount = 1;
            }

            //最初に戻る
            if (HorizontalCount > Horizontal - 1)
            {
                HorizontalCount = 0;

                //エンター→う
                if (VerticalCount == 1) VerticalCount = 2;
            }
        }
        else if (LeftStickInput.y < 0.2f && LeftStickInput.y > -0.2 && LeftStickInput.x < 0.2f && LeftStickInput.x > -0.2f)
        {
            LeftStickflag = false;
        }

        this.transform.position = TargetMozi[HorizontalCount][VerticalCount].transform.position;

        //エンターだけで大きくなるため
        if(HorizontalCount == 10 && VerticalCount == 1)
        {
            this.transform.localScale = new Vector3(5.0f, 10.0f, 0.0f);
        }
        else
        {
            this.transform.localScale = new Vector3(5.0f, 5.0f, 0.0f);
        }

        //文字を決定する
        if((((HorizontalCount <= 6 || HorizontalCount == 8) && VerticalCount <= 4) || (HorizontalCount == 7 && VerticalCount <= 2) || (HorizontalCount == 9 && VerticalCount <= 3)) && (XCI.GetButtonDown(XboxButton.B, XboxController.First) || Input.GetKeyDown(KeyCode.Space)))
        {
            NameText[TextNamber].transform.GetComponent<TextMeshPro>().text = TargetMozi[HorizontalCount][VerticalCount].transform.GetComponent<TextMeshPro>().text;
           
            //次に移動
            if (TextNamber < 4)
            {
                NameText[TextNamber].transform.GetChild(0).GetComponent<BerFlashing>().FlashinColor(false);
                TextNamber++;
                NameText[TextNamber].transform.GetChild(0).GetComponent<BerFlashing>().FlashinColor(true);
            }
        }

        //バックスペース
        if (Input.GetKeyDown(KeyCode.Backspace) || XCI.GetAxis(XboxAxis.LeftTrigger, XboxController.First) > 0.8f && BackSpace == false || HorizontalCount == 10 && VerticalCount == 0 && ((XCI.GetButtonDown(XboxButton.B, XboxController.First)) || Input.GetKeyDown(KeyCode.Space)))
        {
            BackSpace = true;
            NameText[TextNamber].transform.GetComponent<TextMeshPro>().text = "";

            //次に移動
            if (TextNamber > 0)
            {
                NameText[TextNamber].transform.GetChild(0).GetComponent<BerFlashing>().FlashinColor(false);
                TextNamber--;
                NameText[TextNamber].transform.GetChild(0).GetComponent<BerFlashing>().FlashinColor(true);
            }
        }
        if (XCI.GetAxis(XboxAxis.LeftTrigger, XboxController.First) < 0.2f && BackSpace == true)
        {
            BackSpace = false;
        }

        //小濁点
        if (XCI.GetButtonDown(XboxButton.LeftBumper, XboxController.First) || HorizontalCount == 9 && VerticalCount ==4 && (XCI.GetButtonDown(XboxButton.B, XboxController.First) || Input.GetKeyDown(KeyCode.Space)))
        {
            Semi_voicedPoint();
        }

        //エンター
        if(XCI.GetButtonDown(XboxButton.Start, XboxController.First) || HorizontalCount == 10 && VerticalCount == 1 && (Input.GetKeyDown(KeyCode.Space) || (XCI.GetButtonDown(XboxButton.B, XboxController.First))))
        {
            if(TextNamber > 0)
            {
                TManager_cs.ChangePage(true);

                string name = "";
                for(int i = 0; i < NameText.Length; i++)
                {
                    if (NameText[i].transform.GetComponent<TextMeshPro>().text != "")
                    {
                        name += NameText[i].transform.GetComponent<TextMeshPro>().text;
                    }
                    else
                    {
                        break;
                    }
                }
                
                TManager_cs.PlayerName_Data = name;
            }
        }
    }

    private void Semi_voicedPoint()
    {
        string ChangeText = "";

        //変換するテキストの取得
        if (TextNamber > 0)
        {
            ChangeText = NameText[TextNamber - 1].transform.GetComponent<TextMeshPro>().text;
            if (TextNamber == 4 && NameText[TextNamber].transform.GetComponent<TextMeshPro>().text != "")
            {
                ChangeText = NameText[TextNamber].transform.GetComponent<TextMeshPro>().text;
            }
        }
        else
        {
            return;
        }

        //テキストを変換
        for (int i = 0; i < ChangeMozi.Length; i++)
        {
            for (int j = 0; j < ChangeMozi[i].Length; j++)
            {
                if (ChangeText == ChangeMozi[i][j])
                {
                    if (j + 1 < ChangeMozi[i].Length)
                    {
                        ChangeText = ChangeMozi[i][j + 1];
                    }
                    else
                    {
                        ChangeText = ChangeMozi[i][0];
                    }

                    //変換したテキストを代入
                    if (TextNamber == 4 && NameText[TextNamber].transform.GetComponent<TextMeshPro>().text != "")
                    {
                        NameText[TextNamber].transform.GetComponent<TextMeshPro>().text = ChangeText;
                    }
                    else
                    {
                        NameText[TextNamber - 1].transform.GetComponent<TextMeshPro>().text = ChangeText;
                    }
                    return;
                }
            }
        }
    }
    
}
