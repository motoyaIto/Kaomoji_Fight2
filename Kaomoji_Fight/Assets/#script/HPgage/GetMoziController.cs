using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetMoziController : MonoBehaviour {

    private TextMeshProUGUI Texts;      //ゲットしている文字列
    private TextMeshProUGUI MyText;     //自分の文字列
    private float pos_x;                //初期ポジション

    
    // Use this for initialization
    void Start () {
        Texts = this.transform.parent.GetChild(3).GetComponent<TextMeshProUGUI>();
        MyText = this.transform.GetComponent<TextMeshProUGUI>();

        pos_x = this.transform.position.x;
    }
	
	// Update is called once per frame
	void Update () {
        //座標をずらす
        this.transform.position = new Vector3(pos_x + Texts.text.Length * 15.0f, this.transform.position.y, this.transform.position.z);
    }

    /// <summary>
    /// 拾った文字を登録する
    /// </summary>
    /// <param name="mozi">拾った文字</param>
    public void SetTextMozi(string mozi)
    {
        //前の文字が登録されていたら
        if (MyText.text != "" && Texts.text.Length < 4)
        {
            //前に拾った文字を文字群に登録
            Texts.text += MyText.text;
        }
        else if(MyText.text != "" && Texts.text.Length < 5)
        {
            //先頭の文字を消す
            string token = Texts.text;
            Texts.text = "";

            for (int i = 1; i < token.Length; i++)
            {
                Texts.text += token[i];        
            }
            //前に拾った文字を文字群に登録
            Texts.text += MyText.text;
        }

        //今回拾った文字を登録
        MyText.text = mozi;

    }

    /// <summary>
    /// 文字をすべて消す
    /// </summary>
    public void AllDestroy()
    {
        MyText.text = "";
        Texts.text = "";
    }

    /// <summary>
    /// 後ろから一文字消す
    /// </summary>
    public void BackSpace()
    {
        if(MyText.text != "")
        {
            MyText.text = "";
            return;
        }

        string token = Texts.text;
        Texts.text = "";

        for (int i = 0; i < token.Length - 1; i++)
        {
            Texts.text += token[i];
        }
    }
}
