using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlockController : MonoBehaviour {

    [SerializeField]
    private float ResetTime = 10.0f;

    bool setPass;

    private bool MoziBlock = false;//文字(true)ではない(false)

    private bool Stage = true;
    private TextMeshPro TMPro;  //テキストメッシュプロ
    private float count;        //時間を数える

    private void Start()
    {
        TMPro = this.transform.GetChild(0).GetComponent<TextMeshPro>();

        PushStageData();
    }

    private void Update()
    {
        if(Stage == false)
        {
            count += Time.deltaTime;

            TMPro.color = new Color(TMPro.color.r, TMPro.color.g, TMPro.color.b, (50 * (count / ResetTime)) * 0.01f);

            if(count > ResetTime)
            {
                TMPro.color = new Color(TMPro.color.r, TMPro.color.g, TMPro.color.b, (50 * (count / ResetTime)) * 1);

                Stage = true;
                count = 0.0f;
            }
        }
    }
    /// <summary>
    /// 床の復帰処理
    /// </summary>
    public void ReStageBlock()
    {
        this.transform.GetComponent<BoxCollider2D>().enabled = true;

        PushStageData();
    }

    /// <summary>
    /// 床を抜く
    /// </summary>
    public void ChangeMozi()
    {
        this.transform.GetComponent<BoxCollider2D>().enabled = false;

        Stage = false;

        TMPro.color = new Color(TMPro.color.r, TMPro.color.g, TMPro.color.b, 0.0f);

        PushStageData();

        //ResetTime後、床を復帰する
        Invoke("ReStageBlock", ResetTime);
    }

    public void ChangeTextColor(Color color)
    {
        TMPro.color = color;
        PushStageData();
    }

    public void NewConectPlayer()
    {
        PhotonView photonView = this.GetComponent<PhotonView>();
        photonView.RPC("CatchNewStageData", PhotonTargets.All,
            this.transform.GetComponent<BoxCollider2D>().enabled, //あたり判定
            this.transform.GetChild(0).GetComponent<TextMeshPro>().color.r, this.transform.GetChild(0).GetComponent<TextMeshPro>().color.g, this.transform.GetChild(0).GetComponent<TextMeshPro>().color.b, this.transform.GetChild(0).GetComponent<TextMeshPro>().color.a, //色
            this.transform.GetComponent<MeshRenderer>().sharedMaterials[0].name,//マテリアル
            this.transform.GetChild(0).GetComponent<TextMeshPro>().text//ステージテキスト
            );
    }

    public void PushStageData()
    {
        PhotonView photonView = this.GetComponent<PhotonView>();
        photonView.RPC("CatchStageData", PhotonTargets.All, this.transform.GetComponent<BoxCollider2D>().enabled, TMPro.color.r, TMPro.color.g, TMPro.color.b, TMPro.color.a);
    }

    [PunRPC]
    private void CatchStageData(bool collider, float r, float g, float b, float a)
    {
        this.transform.GetComponent<BoxCollider2D>().enabled = collider;
        this.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(r, g, b, a);
    }

    [PunRPC]
    private void CatchNewStageData(bool collider, float r, float g, float b, float a, string mate, string stageText)
    {
        this.transform.GetComponent<BoxCollider2D>().enabled = collider;
        this.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(r, g, b, a);
        this.transform.GetComponent<Renderer>().material = Resources.Load<Material>("Material/" + mate);
        this.transform.GetChild(0).GetComponent<TextMeshPro>().text = stageText;
    }

    /// <summary>
    /// 文字(true)ではない(false)
    /// </summary>
    public bool Mozi
    {
        get
        {
            return MoziBlock;
        }

        set
        {
            MoziBlock = value;
        }
    }
}
