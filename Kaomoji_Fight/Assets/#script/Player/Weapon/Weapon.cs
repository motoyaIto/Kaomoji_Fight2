using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class Weapon : MonoBehaviour
{

    protected Player owner_cs;              //オーナースクリプト
    protected string ownerName;             //オーナーの名前

    protected PhotonManager PManager_cs;//プレイシーンマネージャースクリプト

    protected SpriteRenderer SRenderer;      //武器画像を描画するレンダー

    protected GameObject Effect;            //エフェクト
    protected float EffectWait = 0.0f;      //エフェクトの発生している時間

    protected int DamageValue = 0;           //ダメージ値
    protected float StiffnessTime = 0.0f;   //硬直時間
    protected bool weapon_use = false;      //武器を使用中(true)使用していない(false)

    protected float count = 0;             //カウント

    protected Collider2D collider;

    protected virtual void Start()
    {
        PManager_cs = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();

        SRenderer = this.transform.GetComponent<SpriteRenderer>();
    }

    protected void PleaseMyData()
    {
        PhotonView photonView = this.GetComponent<PhotonView>();
        photonView.RPC("PushWeaponData", PhotonTargets.Others, photonView.ownerId, photonView.viewID);
    }

    [PunRPC]
    protected void PushWeaponData(int ownerId, int viewId)
    {
        if (PhotonNetwork.player.ID == ownerId && this.transform.GetComponent<PhotonView>().viewID == viewId)
        {
            PhotonView photonView = this.GetComponent<PhotonView>();
            photonView.RPC("ChatchBlocData", PhotonTargets.Others,
                this.transform.GetComponent<PhotonView>().ownerId,
                photonView.viewID);
        }
    }

    [PunRPC]
    protected void ChatchBlocData(int ownerId, int viewId)
    {
        if (this.transform.GetComponent<PhotonView>().viewID == viewId && this.transform.GetComponent<PhotonView>().ownerId == ownerId)
        {
            //オーナーオブジェクトの子になる
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                if (PhotonNetwork.playerList[i].ID == ownerId && this.transform.GetComponent<PhotonView>().owner.NickName == PhotonNetwork.playerList[i].NickName)
                {
                    GameObject owner = GameObject.Find(this.transform.GetComponent<PhotonView>().owner.NickName);
                    this.transform.SetParent(owner.transform);
                    this.transform.position = owner.transform.position;
                    return;
                }
            }
        }
    }

    public abstract void Attack();

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

    /// <summary>
    /// プレイヤーを中心にエフェクトを発生させる
    /// </summary>
    protected void EffectOccurrence()
    {
        PhotonView photonView = this.GetComponent<PhotonView>();
        photonView.RPC("EffectOccurrenceData", PhotonTargets.AllBuffered, photonView.viewID); 
    }

    [PunRPC]
    protected void EffectOccurrenceData(int id)
    {
        if (this.transform.GetComponent<PhotonView>().viewID == id)
        {
            //エフェクトの発生
            GameObject EffectObj = Instantiate(Effect, this.transform) as GameObject;
            // エフェクトの発生場所をプレイヤーの中心に
            EffectObj.transform.position = new Vector3(this.transform.parent.transform.position.x, this.transform.position.y, 0);

            //エフェクト発生を待って破棄する
            StartCoroutine(this.DelayMethod(EffectWait, () => { Destroy(EffectObj); }));
        }
    }

    /// <summary>
    /// 指定の座標と角度でエフェクトを発生させる
    /// </summary>
    /// <param name="pos">座標</param>
    /// <param name="rot">角度</param>
    protected void EffectOccurrence(Vector3 pos, Vector3 rot)
    {
        PhotonView photonView = this.GetComponent<PhotonView>();
        photonView.RPC("CreateEffect", PhotonTargets.AllBuffered, photonView.viewID, pos, rot);
    }

    [PunRPC]
    protected void CreateEffect(int id, Vector3 pos, Vector3 rot)
    {
        if (this.transform.GetComponent<PhotonView>().viewID == id)
        {
            //エフェクトの発生
            GameObject EffectObj = Instantiate(Effect, this.transform) as GameObject;

            //エフェクトの発生場所を指定の座標に
            EffectObj.transform.position = pos;
            //エフェクトの回転
            EffectObj.transform.rotation = new Quaternion(EffectObj.transform.rotation.x + rot.x, EffectObj.transform.rotation.y + rot.y, EffectObj.transform.rotation.z + rot.z, EffectObj.transform.rotation.w);

            //エフェクト発生を待って破棄する
            StartCoroutine(this.DelayMethod(EffectWait, () => { Destroy(EffectObj); }));
        }
    }

    protected void EffectOccurrence_World(Vector3 pos, Vector3 rot)
    {
        PhotonView photonView = this.GetComponent<PhotonView>();
        photonView.RPC("CreateEffect_WorldData", PhotonTargets.AllBuffered, photonView.viewID, pos, rot);
    }

    [PunRPC]
    protected void CreateEffect_WorldData(int id, Vector3 pos, Vector3 rot)
    {
        if (this.transform.GetComponent<PhotonView>().viewID == id)
        {
            //エフェクトの発生
            GameObject EffectObj = Instantiate(Effect) as GameObject;

            //エフェクトの発生場所を指定の座標に
            EffectObj.transform.position = pos;
            //エフェクトの回転
            EffectObj.transform.rotation = new Quaternion(EffectObj.transform.rotation.x + rot.x, EffectObj.transform.rotation.y + rot.y, EffectObj.transform.rotation.z + rot.z, EffectObj.transform.rotation.w);

            //エフェクト発生を待って破棄する
            StartCoroutine(this.DelayMethod(EffectWait, () => { Destroy(EffectObj); }));
        }
    }

    public Player Owner_csData
    {
        set
        {
            owner_cs = value;

            ownerName = owner_cs.PlayerName_Data;
        }
    }

    public int DamageValue_Data
    {
        get
        {
            return DamageValue;
        }
        set
        {
            DamageValue = value;
        }
    }

    public string OwnerName_Data
    {
        get
        {
            return ownerName;
        }
    }
}
