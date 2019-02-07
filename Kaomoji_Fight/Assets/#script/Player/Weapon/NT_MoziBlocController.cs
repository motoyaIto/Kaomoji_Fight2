using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using System;

public class NT_MoziBlocController : MonoBehaviour
{
    protected PhotonManager PManager_cs;//プレイシーンマネージャー
    protected string mozi;                  //自分の文字

    [SerializeField]
    protected float DamageValue = 1.0f;     //ダメージ量
    private float thrust = 1000f;           // 投擲物の推進力



    private Vector3 Death_LUpos = new Vector3(-150f, 100f, 0f);    // オブジェクトが破棄されるエリアの左上
    private Vector3 Death_RDpos = new Vector3(200f, -80f, 0f);   // オブジェクトが破棄されるエリアの右下

    

    private string parentName;              //親の名前

    public string owner;                   //所有者の名前
    protected Player owner_cs;              //所有者のplayerスクリプト
    protected bool Mozi_use = false;      //文字を投げた(true)投げてない(false)

    private string hitEffect = "prefab/Effect/Wave_01";           // ヒットエフェクト

    // 音
    private AudioSource As;
    private AudioClip ac;

    private void Start()
    {
        PManager_cs = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();
        // 持ち主でないのなら持ち主にデータを要請する
        if (!this.transform.GetComponent<PhotonView>().isMine)
        {
            this.PleaseMyData();
            return;
        }

        //自分の文字
        mozi = this.transform.GetChild(0).GetComponent<TextMeshPro>().text;
    }

    private void PleaseMyData()
    {
        PhotonView photonView = this.GetComponent<PhotonView>();
        photonView.RPC("PushBlocData", PhotonTargets.Others, photonView.ownerId, photonView.viewID);
    }

    [PunRPC]
    private void PushBlocData(int ownerId, int viewId)
    {
        if (PhotonNetwork.player.ID == ownerId && this.transform.GetComponent<PhotonView>().viewID == viewId)
        {
            PhotonView photonView = this.GetComponent<PhotonView>();
            photonView.RPC("ChatchBlocData", PhotonTargets.AllBuffered,
                this.transform.GetComponent<PhotonView>().ownerId,
                photonView.viewID,
                this.name,
                this.transform.GetChild(0).GetComponent<TextMeshPro>().text);
        }
    }

    [PunRPC]
    private void ChatchBlocData(int ownerId, int viewId, string name, string text)
    {
        if (this.transform.GetComponent<PhotonView>().viewID == viewId && this.transform.GetComponent<PhotonView>().ownerId == ownerId)
        {
            //オーナーオブジェクトの子になる
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                if (PhotonNetwork.playerList[i].ID == ownerId && this.transform.GetComponent<PhotonView>().owner.NickName == PhotonNetwork.playerList[i].NickName)
                {
                    this.transform.SetParent(GameObject.Find(this.transform.GetComponent<PhotonView>().owner.NickName).transform);

                    this.transform.GetChild(0).GetComponent<TextMeshPro>().enabled = true;
                }
            }

            this.name = "MoziBlock(" + name + ")";
            this.transform.GetChild(0).GetComponent<TextMeshPro>().text = text;
        }
    }

    public virtual void Update()
    {
        // 飛んでったブロックの削除
        if (this.transform.position.x < Death_LUpos.x || this.transform.position.x > Death_RDpos.x || this.transform.position.y > Death_LUpos.y || this.transform.position.y < Death_RDpos.y)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }

    }

    /// <summary>
    /// 攻撃
    /// </summary>
    /// <param name="shot">使用した座標</param>
    public void Attack(Vector3 shot)
    {
        Rigidbody2D rig2d = this.gameObject.AddComponent<Rigidbody2D>();
        rig2d.gravityScale = .01f;

        owner_cs.ChangeMozi_Data = false;

        // 親から離れる
        this.transform.SetParent(null);

        //ウェポンにボックスコライダーをオンにする
        this.transform.GetComponent<BoxCollider2D>().enabled = true;
        this.transform.GetComponent<BoxCollider2D>().isTrigger = true;
        Mozi_use = true;

        // 動かずに投げたら
        if (shot == Vector3.zero)
        {
            // 上に投げる
            shot = Vector3.up;
        }
        // ⊂二二二（ ＾ω＾）二⊃ ﾌﾞｰﾝ
        rig2d.AddForce(shot * thrust);

        PhotonView photonView = this.GetComponent<PhotonView>();
        photonView.RPC("AttackData", PhotonTargets.Others, photonView.ownerId, photonView.viewID);
    }

    [PunRPC]
    private void AttackData(int ownerId, int viewId)
    {
        if (this.transform.GetComponent<PhotonView>().ownerId == ownerId && this.transform.GetComponent<PhotonView>().viewID == viewId)
        {
            // 親から離れる
            this.transform.SetParent(null);
        }
    }

    /// <summary>
    /// 攻撃文字
    /// </summary>
    /// <param name="shot">使用した座標</param>
    /// <returns>文字攻撃をしたら(true)していなかったら(fasle)</returns>
    protected virtual bool AttackMozi(Vector3 shot)
    {
        return false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (CheckHit_Rival(collision) == true)
        {
            //プレイヤーにダメージを与える
            PManager_cs.MoziAttack(collision.gameObject, this.gameObject);

            //エフェクト
            var hitobj = PhotonNetwork.Instantiate(hitEffect, this.transform.position + transform.forward, Quaternion.identity, 0);
            PhotonNetwork.Destroy(this.gameObject);
            Mozi_use = false;
        }
    }

    protected bool CheckHit_Rival(Collider2D collider)
    {
        if (parentName != collider.gameObject.name && Mozi_use && collider.transform.tag == "Player")
        {
            return true;
        }

        return false;
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

    //座標を入れる
    public Vector3 SetPosition
    {
        set
        {
            this.transform.position = value;
        }
    }

    public float DamageValue_Data
    {
        get
        {
            return DamageValue;
        }
    }


    public string OwnerName_Data
    {
        set
        {
            parentName = value;
        }
    }

    public Player Ownercs_Data
    {
        set
        {
            owner_cs = value;
        }

        get
        {
            return owner_cs;
        }
    }

}
