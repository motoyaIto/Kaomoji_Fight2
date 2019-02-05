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

        // 持ち主でないのなら持ち主にデータを要請する
        if (!GetComponent<PhotonView>().isMine)
        {
            this.PleaseMyData();
            return;
        }

        GameObject canves = GameObject.Find("Wait_UI").transform.GetChild(0).gameObject;
        this.transform.SetParent(canves.transform);

        rectTrans = this.transform.GetComponent<RectTransform>();
        rectTrans.localPosition = new Vector3(rectTrans.rect.width * (PhotonNetwork.playerList.Length - 1), 0, 0);

        this.name = NT_PlayerData.Instance.name + "_HPber";
        this.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = NT_PlayerData.Instance.name;
        this.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = NT_PlayerData.Instance.color;


    }

    private void PleaseMyData()
    {
        PhotonView photonView = this.GetComponent<PhotonView>();

        photonView.RPC("PushHPData", PhotonTargets.OthersBuffered, photonView.ownerId, texts[0].text, texts[1].text, texts[2].text, texts[3].text, texts[4].text);
    }

    [PunRPC]
    private void PushHPData(int id, string text1, string text2, string text3, string text4, string text5)
    {
        if (id == this.transform.GetComponent<PhotonView>().ownerId)
        {
            PhotonView photonView = this.GetComponent<PhotonView>();
            photonView.RPC("CatchHPData", PhotonTargets.OthersBuffered, this.transform.GetComponent<PhotonView>().ownerId, NT_PlayerData.Instance.name, NT_PlayerData.Instance.color.r, NT_PlayerData.Instance.color.g, NT_PlayerData.Instance.color.b, NT_PlayerData.Instance.color.a, this.transform.GetComponent<Slider>().value);

            texts[0].text = text1;
            texts[1].text = text2;
            texts[2].text = text3;
            texts[3].text = text4;
            texts[4].text = text5;
        }
    }

    [PunRPC]
    private void CatchHPData(int id, string name, float r, float g, float b, float a, float value)
    {
        if (id == this.transform.GetComponent<PhotonView>().ownerId)
        {
            GameObject canves = GameObject.Find("Wait_UI").transform.GetChild(0).gameObject;
            this.transform.SetParent(canves.transform);

            this.transform.GetComponent<RectTransform>().localPosition = new Vector3(this.transform.GetComponent<RectTransform>().rect.width * (id - 1), 0, 0);

            this.name = name + "_HPber";
            this.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = name;
            this.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color(r, g, b, a);

            this.transform.GetComponent<Slider>().value = value;
        }
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

            this.PushTextData(mozi, textnamber - 1, textnamber);
            return;
        }
        //テキストに空きがなかったら
        for (int i = 1; i < texts.Length; i++)
        {
            texts[i - 1].text = texts[i].text;
        }

        texts[texts.Length - 1].text = mozi;

        texts[texts.Length - 1].color = Color.red;
        this.PushTextData(mozi, -1, texts.Length - 1);
    }

    private void PushTextData(string text, int colorBlack, int colorRed)
    {
        PhotonView photonView = this.GetComponent<PhotonView>();
        photonView.RPC("ChathTextData", PhotonTargets.OthersBuffered, photonView.ownerId, text, colorBlack, colorRed);
    }

    [PunRPC]
    public void ChathTextData(int ownerID, string text, int black, int red)
    {
        if (ownerID == this.transform.GetComponent<PhotonView>().ownerId)
        {
            texts[red].text = text;
            texts[red].color = Color.red;
            if (black > -1)
            {
                texts[black].color = Color.black;
            }
        }
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

                    PhotonView photonView = this.GetComponent<PhotonView>();
                    photonView.RPC("Semi_voicedPointData", PhotonTargets.OthersBuffered, photonView.ownerId, count, texts[count].text);

                    return;
                }
            }
        }
    }

    [PunRPC]
    public void Semi_voicedPointData(int ownerID, int textnamber, string text)
    {
        if (ownerID == this.transform.GetComponent<PhotonView>().ownerId)
        {
            texts[textnamber].text = text;
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

        PhotonView photonView = this.GetComponent<PhotonView>();
        photonView.RPC("BackSpaceData", PhotonTargets.OthersBuffered, photonView.ownerId, count - 1);
    }

    [PunRPC]
    public void BackSpaceData(int ownerID, int textnamber)
    {
        if (ownerID == this.transform.GetComponent<PhotonView>().ownerId)
        {
            texts[textnamber].text = "";
        }
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

        PhotonView photonView = this.GetComponent<PhotonView>();
        photonView.RPC("DestroyData", PhotonTargets.OthersBuffered, photonView.ownerId);
    }

    [PunRPC]
    public void DestroyData(int ownerID)
    {
        if (ownerID == this.transform.GetComponent<PhotonView>().ownerId)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].text = "";
            }
        }
    }
}
