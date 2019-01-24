using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using XboxCtrlrInput;


public class NT_Confirmation : MonoBehaviour {

    [SerializeField]
    private GameObject TManager;            //Titleマネージャー
    private NT_TitleManager TManager_cs;   //Titleマネージャーcs

    [SerializeField]
    GameObject texts;

    [SerializeField]
    private TextMeshPro nameText;
    [SerializeField]
    private GameObject playerFace;
    [SerializeField]
    private TextMeshPro stageText;

    //プレイヤーデータ
    private NT_PlayerData playerdata;
    private string name = "";
    private Sprite Face;
    private Color color;
    private string stage = "";

    void Start()
    {
        //各csの取得
        TManager_cs = TManager.GetComponent<NT_TitleManager>();
    }

    public void PageUpdate()
    {
        //自分が選ばれていなかったら
        if (TManager_cs.SelectMode_Data != NT_TitleManager.SELECTMODE.MAX)
        {
            return;
        }
        else
        {
            texts.SetActive(true);
            playerFace.SetActive(true);
        }

        //各自設定
        nameText.text = name;
        playerFace.transform.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", color);
        playerFace.transform.GetComponent<SpriteRenderer>().sprite = Face;
        nameText.color = color;
        stageText.text = stage;

        //ステージ選択に戻る
        if ((XCI.GetButtonDown(XboxButton.A, XboxController.First) || Input.GetKeyDown(KeyCode.Backspace)))
        {
            TManager_cs.ChangePage(false);

            //非表示設定
            texts.SetActive(false);
            playerFace.SetActive(false);
            nameText.text = "";
            stageText.text = "";
        }

        //決定する
        if ((XCI.GetButtonDown(XboxButton.Start, XboxController.First) || Input.GetKeyDown(KeyCode.Space)))
        {
            playerdata = new NT_PlayerData(name, Face, color, stage);
            SceneManagerController.ChangeScene();
        }
    }

    public string Name_Data
    {
        set
        {
            name = value;
        }
    }

    public Sprite Face_Data
    {
        set
        {
            Face = value;
        }
    }

    public Color Color_Data
    {
        set
        {
            color = value;
        }
    }

    public string Stage_Data
    {
        set
        {
            stage = value;
        }
    }
}
