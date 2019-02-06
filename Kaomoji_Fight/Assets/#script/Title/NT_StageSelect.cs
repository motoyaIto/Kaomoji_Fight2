using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using XboxCtrlrInput;

public class NT_StageSelect : MonoBehaviour {

    [SerializeField]
    private GameObject TManager;            //Titleマネージャー
    private NT_TitleManager TManager_cs;   //Titleマネージャーcs

    private GameObject text;
    [SerializeField]
    private GameObject[] Target;

    private bool LeftStickflag = false;             //スティックが入力されていない(false)された(true)
    private Vector2 LeftStickInput = Vector2.zero;  //Controllerの左スティックのAxisを取得

    private int Vertical = 4;       //縦の数
    private int Horizontal = 2;    //横の数
    private int VerticalCount = 0;          //縦の今の番号
    private int HorizontalCount = 0;        //横の今の番号

    private new AudioSource audio;
    private new AudioSource audio2;
    private AudioClip cursor_ac;    //カーソル移動音
    private AudioClip decision_ac;
    private AudioClip cancel_ac;

    private void Awake()
    {
        audio = this.GetComponent<AudioSource>();
        audio2 = this.GetComponent<AudioSource>();
        cursor_ac = (AudioClip)Resources.Load("Sound/SE/Select/Decision/cursor");      //カーソル移動音
        decision_ac = (AudioClip)Resources.Load("Sound/SE/Select/Decision/decision");      //決定音
        cancel_ac = (AudioClip)Resources.Load("Sound/SE/Select/Cancel/cancel");

    }

    void Start()
    {
        //各csの取得
        TManager_cs = TManager.GetComponent<NT_TitleManager>();
        text = this.transform.parent.GetChild(3).gameObject;

        //初期座標
        this.transform.position = Target[0].transform.position;
    }

    public void PageUpdate()
    {
        //自分が選ばれていなかったら
        if (TManager_cs.SelectMode_Data != NT_TitleManager.SELECTMODE.STAGESELECT)
        {
            return;
        }
        else
        {
            this.transform.GetComponent<SpriteRenderer>().enabled = true;
            Target[0].transform.parent.gameObject.SetActive(true);
            text.SetActive(true);
        }

        // Controllerの左スティックのAxisを取得            
        LeftStickInput = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, XboxController.First), XCI.GetAxis(XboxAxis.LeftStickY, XboxController.First));

        //上入力
        if (Input.GetKeyDown(KeyCode.UpArrow) || XCI.GetDPadDown(XboxDPad.Up, XboxController.First) || (LeftStickInput.y > 0.9f && LeftStickflag == false))
        {
            audio.PlayOneShot(cursor_ac);
            VerticalCount--;
            LeftStickflag = true;

            //下に戻る
            if (VerticalCount < 0)
            {
                VerticalCount = Vertical - 1;
            }
        }
        //下入力
        else if (Input.GetKeyDown(KeyCode.DownArrow) || XCI.GetDPadDown(XboxDPad.Down, XboxController.First) || (LeftStickInput.y < -0.9f && LeftStickflag == false))
        {
            audio.PlayOneShot(cursor_ac);
            VerticalCount++;
            LeftStickflag = true;

            //上に戻る
            if (VerticalCount > Vertical - 1)
            {
                VerticalCount = 0;
            }
        }
        //右入力
        else if (Input.GetKeyDown(KeyCode.RightArrow) || XCI.GetDPadDown(XboxDPad.Right, XboxController.First) || (LeftStickInput.x > 0.9f && LeftStickflag == false))
        {
            audio.PlayOneShot(cursor_ac);
            HorizontalCount--;
            LeftStickflag = true;

            //最後に移動
            if (HorizontalCount < 0)
            {
                HorizontalCount = Horizontal - 1;
            }
        }
        //左入力
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || XCI.GetDPadDown(XboxDPad.Left, XboxController.First) || (LeftStickInput.x < -0.9f && LeftStickflag == false))
        {
            audio.PlayOneShot(cursor_ac);
            HorizontalCount++;
            LeftStickflag = true;

            //最初に戻る
            if (HorizontalCount > Horizontal - 1)
            {
                HorizontalCount = 0;
            }
        }
        else if (LeftStickInput.y < 0.2f && LeftStickInput.y > -0.2 && LeftStickInput.x < 0.2f && LeftStickInput.x > -0.2f)
        {
            LeftStickflag = false;
        }

        //選択されてるステージに枠を合わせる
        this.transform.position = Target[HorizontalCount * Vertical + VerticalCount].transform.position;

        //ステージを決定する
        if ((XCI.GetButtonDown(XboxButton.B, XboxController.First) || Input.GetKeyDown(KeyCode.Space)))
        {
            audio.PlayOneShot(decision_ac);
            TManager_cs.SelectStage_Data = Target[HorizontalCount * Vertical + VerticalCount].transform.GetComponent<TextMeshPro>().text;

            TManager_cs.ChangePage(true);

            //非表示設定
            this.transform.GetComponent<SpriteRenderer>().enabled = false;
            text.SetActive(false);
            Target[0].transform.parent.gameObject.SetActive(false);
        }

        //カラー選択に戻る
        if ((XCI.GetButtonDown(XboxButton.A, XboxController.First) || Input.GetKeyDown(KeyCode.Backspace)))
        {
            audio.PlayOneShot(cancel_ac);
            TManager_cs.ChangePage(false);

            //非表示設定
            this.transform.GetComponent<SpriteRenderer>().enabled = false;
            text.SetActive(false);
            Target[0].transform.parent.gameObject.SetActive(false);
        }
    }
}
