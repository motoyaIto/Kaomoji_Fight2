using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HPberController : MonoBehaviour
{

    private RectTransform rectTrans;
    [SerializeField]
    private TextMeshProUGUI[] texts;

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
    void Start()
    {
        this.name = NT_PlayerData.Instance.name + "_HPber";
        this.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = NT_PlayerData.Instance.name;
        this.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = NT_PlayerData.Instance.color;
    }

    /// <summary>
    /// 拾った文字を登録する
    /// </summary>
    /// <param name="mozi">拾った文字</param>
    public void SetTextMozi(string mozi)
    {
        //空のテキストを探す
        int textnamber = 0;
        for (textnamber = 0; textnamber < texts.Length; textnamber++)
        {
            if (texts[textnamber].text == "")
            {
                break;
            }
        }

        //テキストに空きがあったら
        if (textnamber < texts.Length)
        {
            texts[textnamber].text = mozi;

            if (textnamber - 1 > -1)
            {
                texts[textnamber - 1].color = Color.black;
            }

            texts[textnamber].color = Color.red;
            return;
        }
        //テキストに空きがなかったら
        for (int i = 1; i < texts.Length; i++)
        {
            texts[i - 1].text = texts[i].text;
        }

        texts[texts.Length - 1].text = mozi;

        texts[texts.Length - 1].color = Color.red;
    }

    /// <summary>
    /// 半濁点
    /// </summary>
    public void Semi_voicedPoint()
    {
        if (texts[0].text == "") return;

        //変換する文字を取得
        int count = 1;
        for (count = 1; count < texts.Length; count++)
        {
            if (texts[count].text == "")
            {
                count--;
                break;
            }
        }

        if (count == 5)
        {
            count--;
        }

        //変換するテキストデータを探して変換する
        for (int i = 0; i < ChangeMozi.Length; i++)
        {
            for (int j = 0; j < ChangeMozi[i].Length; j++)
            {
                if (texts[count].text == ChangeMozi[i][j])
                {
                    if (j + 1 < ChangeMozi[i].Length)
                    {
                        texts[count].text = ChangeMozi[i][j + 1];
                    }
                    else
                    {
                        texts[count].text = ChangeMozi[i][0];
                    }

                    return;
                }
            }
        }
    }

    /// <summary>
    /// 一文字消す
    /// </summary>
    public void BackSpace()
    {
        //テキストに書き込みがなかったら
        if (texts[0].text == "")
        {
            return;
        }

        //一番後ろのテキストを取得する
        int count = 0;

        for (count = 0; count < texts.Length; count++)
        {
            if (texts[count].text == "")
            {
                break;
            }
        }

        //指定の文字を消す
        texts[count - 1].text = "";
    }

    /// <summary>
    /// 文字をすべて消す
    /// </summary>
    public void AllDestroy()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = "";
        }
    }

    public string AllText()
    {
        string text = "";

        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].text == "")
            {
                break;
            }

            text += texts[i].text;
        }

        return text;
    }
}
