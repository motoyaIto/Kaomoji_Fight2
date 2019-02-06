using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using XboxCtrlrInput;

public class ColorSelectController : MonoBehaviour {

    private enum SliderMode
    {
        Red,
        Green,
        Blue,

        Enter
    }
    [SerializeField]
    private GameObject TManager;            //Titleマネージャー
    private NT_TitleManager TManager_cs;   //Titleマネージャーcs

    private GameObject text;

    [SerializeField]
    private GameObject Cursor;
    [SerializeField]
    private Sprite Fice = null;
    [SerializeField]
    private string Name = "";

    [SerializeField]
    private GameObject Player_obj;
    [SerializeField]
    private GameObject NameText_obj;

    [SerializeField]
    private SliderMode sliderMode = SliderMode.Red;
    private int SliderMoveValue = 1;
    [SerializeField]
    private Slider R_Slider;
    [SerializeField]
    private Slider G_Slider;
    [SerializeField]
    private Slider B_Slider;
    [SerializeField]
    private GameObject Enter_obj;

    private bool LeftStickflag = false;             //スティックが入力されていない(false)された(true)
    private Vector2 LeftStickInput = Vector2.zero;  //Controllerの左スティックのAxisを取得

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

    private void Start()
    {
        //各csの取得
        TManager_cs = TManager.GetComponent<NT_TitleManager>();
        text = this.transform.parent.GetChild(2).gameObject;
    }

    public void PageUpdate()
    {
        if (TManager_cs.SelectMode_Data != NT_TitleManager.SELECTMODE.COLORSELECT || Fice == null || Name == "")
        {
            return;
        }
        else
        {
            text.SetActive(true);

            //顔を設定
            Player_obj.transform.GetComponent<SpriteRenderer>().sprite = Fice;
            //名前を設定
            NameText_obj.transform.GetComponent<TextMeshPro>().text = Name;
            Player_obj.SetActive(true);
            NameText_obj.SetActive(true);
            Cursor.SetActive(true);
        }

        // Controllerの左スティックのAxisを取得            
        LeftStickInput = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, XboxController.First), XCI.GetAxis(XboxAxis.LeftStickY, XboxController.First));

        //今選択されているスライダー
        Slider slider = this.SelectSlider(sliderMode);
            
        //上入力
        if (Input.GetKeyDown(KeyCode.UpArrow) || XCI.GetDPadDown(XboxDPad.Up, XboxController.First) || (LeftStickInput.y > 0.9f && LeftStickflag == false))
        {
            audio.PlayOneShot(cursor_ac);
            sliderMode--;
            if(sliderMode < SliderMode.Red)
            {
                sliderMode = SliderMode.Enter;
            }
            LeftStickflag = true;
        }
        //下入力
        else if (Input.GetKeyDown(KeyCode.DownArrow) || XCI.GetDPadDown(XboxDPad.Down, XboxController.First) || (LeftStickInput.y < -0.9f && LeftStickflag == false))
        {
            audio.PlayOneShot(cursor_ac);
            sliderMode++;
            if (sliderMode > SliderMode.Enter)
            {
                sliderMode = SliderMode.Red;
            }

            LeftStickflag = true;
        }
        //右入力
        else if (slider != null && (Input.GetKey(KeyCode.RightArrow) || XCI.GetDPad(XboxDPad.Right, XboxController.First) || (LeftStickInput.x > 0.9f && LeftStickflag == false)))
        {
            audio.PlayOneShot(cursor_ac);
            if (slider.value < slider.maxValue)
            {
                slider.value += SliderMoveValue;
            }
        }
        //左入力
        else if (slider != null && (Input.GetKey(KeyCode.LeftArrow) || XCI.GetDPad(XboxDPad.Left, XboxController.First) || (LeftStickInput.x < -0.9f && LeftStickflag == false)))
        {
            audio.PlayOneShot(cursor_ac);
            if (slider.value > slider.minValue)
            {
                slider.value -= SliderMoveValue;
            }
        }
        else if (LeftStickInput.y < 0.2f && LeftStickInput.y > -0.2 && LeftStickInput.x < 0.2f && LeftStickInput.x > -0.2f)
        {
            LeftStickflag = false;
        }

        //顔と名前の色を変更
        Player_obj.transform.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", new Color(R_Slider.value / 255,G_Slider.value / 255, B_Slider.value / 255));
        NameText_obj.transform.GetComponent<TextMeshPro>().color = new Color(R_Slider.value / 255, G_Slider.value / 255, B_Slider.value / 255);

        //スライダーの座標にカーソルを合わせる
        if(slider != null)
        {
            Cursor.transform.position = slider.transform.position;
            Cursor.transform.localScale = new Vector3(53.93f, 6.68f, 0);
        }
        else
        {//OKボタンに合わせる
            Cursor.transform.position = Enter_obj.transform.localPosition;
            Cursor.transform.localScale = new Vector3(12.2f, 18.4f, 0);
        }

        //キャラクター選択に戻る
        if ((XCI.GetButtonDown(XboxButton.A, XboxController.First) || Input.GetKeyDown(KeyCode.Backspace)))
        {
            audio.PlayOneShot(cancel_ac);
            TManager_cs.ChangePage(false);

            //非表示設定
            text.SetActive(false);

            Player_obj.SetActive(false);
            NameText_obj.SetActive(false);
            Cursor.SetActive(false);
        }

        //決定する
        if ((XCI.GetButtonDown(XboxButton.Start, XboxController.First) || (slider == null && ((XCI.GetButtonDown(XboxButton.B, XboxController.First) || Input.GetKeyDown(KeyCode.Space))))))
        {
            audio.PlayOneShot(decision_ac);
            Cursor.SetActive(false);
            Player_obj.SetActive(false);
            Name = "";

            TManager_cs.Color_Data = new Color(R_Slider.value / 255, G_Slider.value / 255, B_Slider.value / 255);
            TManager_cs.ChangePage(true);

            //非表示設定
            text.SetActive(false);

            Player_obj.SetActive(false);
            NameText_obj.SetActive(false);
            Cursor.SetActive(false);

        }
    }

    /// <summary>
    /// 選択されているスライダーを返す
    /// </summary>
    /// <param name="mode">選択中のスライダー名</param>
    /// <returns>スライダー</returns>
    private Slider SelectSlider(SliderMode mode)
    {
        switch(mode)
        {
            case SliderMode.Red:
                return R_Slider;
            case SliderMode.Green:
                return G_Slider;
            case SliderMode.Blue:
                return B_Slider;
        }

        return null;
    }

    public Sprite Fice_Data
    {
        set
        {
            Fice = value;
        }
    }

    public string Name_Data
    {
        set
        {
            Name = value;
        }
    }
}
