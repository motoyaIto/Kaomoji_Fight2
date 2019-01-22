using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using System;

public class NT_TitleManager : MonoBehaviour{

    public enum SELECTMODE
    {
        TITLE,
        NAME,
        FICESELECT,
        COLORSELECT,
        STAGESELECT,
        CHANGESCENE,

        MAX
    }
    [SerializeField]
    private SELECTMODE mode = SELECTMODE.TITLE;//選択されている画面のモード

    private bool ControllerLock = false;//コントローラーをロックする(true)しない(false)

    [SerializeField]
    private GameObject CenterPoint;     //ページの中心
    [SerializeField]
    private float OpenSpeed = 1.0f;     //ページをめくるスピード
    private bool ChangePageFlag = false;    //ページを切り替える(true)切り替えない(false)
    private bool OpenPageFlag = true;      //次のページ(true)前のページ(false)

    //各ページ
    [SerializeField]
    private GameObject[] Page;



    //プレイヤーデータ
    private string PlayerName = "";
    private Sprite Face;

    // Use this for initialization
    void Start() {
        //シーンのロード
        SceneManagerController.LoadScene();

    }
	// Update is called once per frame
	void Update () {        
		
        //ページ切り替え中
        if(ChangePageFlag == true)
        {
            //次のページ
            if(OpenPageFlag == true)
            {
                this.NextPage();
            }
            else//前のページ
            {
                this.PreviousPage();
            }
        }
	}

    /// <summary>
    /// ページをめくる
    /// </summary>
    /// <param name="Open">次のページ(true)前のページ(false)</param>
    public void ChangePage(bool Open)
    {
        if (ChangePageFlag != false) return;
        OpenPageFlag = Open;
        ChangePageFlag = true;

        //ページをめくるのをやめてモードを切り替える
        StartCoroutine(DelayMethod(1.45f, 
            () => 
            {
                ChangePageFlag = false;
                if(OpenPageFlag == true)
                {
                    mode++;
                }
                else
                {
                    mode--;
                }
            }));
    }

    private void NextPage()
    {
        Page[(int)mode].transform.RotateAround(CenterPoint.transform.position, Vector3.right, -OpenSpeed);
    }

    private void PreviousPage()
    {
        Page[(int)mode - 1].transform.RotateAround(CenterPoint.transform.position, Vector3.right, OpenSpeed);
    }

    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="waitTime">遅延時間</param>
    /// <param name="action">実行する処理</param>
    /// <returns></returns>
    protected IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    public SELECTMODE SelectMode_Data
    {
        get
        {
            return mode;
        }
    }

    public string PlayerName_Data
    {
        set
        {
            PlayerName = value;
        }
    }
    public Sprite Face_Data
    {
        set
        {
            Face = value;
        }
    }
}
