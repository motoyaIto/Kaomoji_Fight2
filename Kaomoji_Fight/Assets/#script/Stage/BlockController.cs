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
    }

    /// <summary>
    /// 床を抜く
    /// </summary>
    public void ChangeMozi()
    {
        this.transform.GetComponent<BoxCollider2D>().enabled = false;

        Stage = false;

        TMPro.color = new Color(TMPro.color.r, TMPro.color.g, TMPro.color.b, 0.0f);

        //ResetTime後、床を復帰する
        Invoke("ReStageBlock", ResetTime);
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
