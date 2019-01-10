using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetMoziController : MonoBehaviour {

    private TextMeshProUGUI Texts;      //ゲットしている文字列
    private TextMeshProUGUI MyText;     //自分の文字列
    private float pos_x;                //初期ポジション

    //変換表
    public static string[][] ChangeMozi = new string[][]
        {
            new string[2]{"か", "が"}, new string[2]{"き", "ぎ"}, new string[2]{"く", "ぐ"}, new string[2]{"け", "げ"}, new string[2]{"こ", "ご"},
            new string[2]{"さ", "ざ"}, new string[2]{"し", "じ"}, new string[2]{"す", "ず"}, new string[2]{"せ", "ぜ"}, new string[2]{"そ", "ぞ"},
            new string[2]{"た", "だ"}, new string[2]{"ち", "ぢ"}, new string[3]{"つ", "づ", "っ"}, new string[2]{"て", "で"}, new string[2]{"と", "ど"},
            new string[3]{"は", "ば", "ぱ"}, new string[3]{"ひ", "び", "ぴ"}, new string[3]{"ふ", "ぶ", "ぷ"}, new string[3]{"へ", "べ", "ぺ"}, new string[3]{"ほ", "ぼ", "ぽ"},

            new string[2]{"あ", "ぁ"}, new string[2]{"い", "ぃ"}, new string[2]{"う", "ぅ"}, new string[2]{"え", "ぇ"}, new string[2]{"お", "ぉ"},
            new string[2]{"や", "ゃ"}, new string[2]{"ゆ", "ゅ"}, new string[2]{"よ", "ょ"}
        };
    
    // Use this for initialization
    void Start () {
        Texts = this.transform.parent.GetChild(3).GetComponent<TextMeshProUGUI>();
        MyText = this.transform.GetComponent<TextMeshProUGUI>();

        pos_x = this.transform.position.x;
    }
	
	// Update is called once per frame
	void Update () {
        //座標をずらす
        this.transform.position = new Vector3(pos_x + Texts.text.Length * (15.0f * (Screen.width / 441)), this.transform.position.y, this.transform.position.z);
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
    
    public void Semi_voicedPoint()
    {
        if (MyText.text == "") return;

        for (int i = 0; i < ChangeMozi.Length; i++)
        {
            for(int j = 0; j < ChangeMozi[i].Length; j++)
            {
                if(MyText.text == ChangeMozi[i][j])
                {
                    if(j + 1 < ChangeMozi[i].Length)
                    {
                        MyText.text = ChangeMozi[i][j + 1];
                    }
                    else
                    {
                        MyText.text = ChangeMozi[i][0];
                    }

                    return;
                }
            }
        }
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
