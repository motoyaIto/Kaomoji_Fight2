using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class Weapon : MonoBehaviour {

    protected Player owner_cs;              //オーナースクリプト
    protected PlaySceneManager PSManager_cs;//プレイシーンマネージャースクリプト

    protected SpriteRenderer SRenderer;      //武器画像を描画するレンダー

    protected GameObject Effect;            //エフェクト
    protected float EffectWait = 0.0f;      //エフェクトの発生している時間

    protected int DamageValue = 0;           //ダメージ値
    protected float StiffnessTime = 0.0f;   //硬直時間
    protected bool weapon_use = false;      //武器を使用中(true)使用していない(false)

    protected float count = 0;             //カウント

    protected virtual void Start()
    {
        PSManager_cs = GameObject.Find("PlaySceneManager").GetComponent<PlaySceneManager>();

        SRenderer = this.transform.GetComponent<SpriteRenderer>();
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

    protected void EffectOccurrence()
    {
        //エフェクトの発生
        GameObject EffectObj = Instantiate(Effect, this.transform) as GameObject;
        // エフェクトの発生場所をプレイヤーの中心に
        EffectObj.transform.position = new Vector3(this.transform.parent.transform.position.x, this.transform.position.y, 0);

        //エフェクト発生を待って破棄する
        StartCoroutine(this.DelayMethod(EffectWait, () => { Destroy(EffectObj); }));
    }

    protected void EffectOccurrence(Vector3 pos, Vector3 rot)
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

    public Player Owner_csData
    {
        set
        {
            owner_cs = value;
        }
    }

    public int DamageValue_Data
    {
        get
        {
            return DamageValue;
        }
    }

    public string OwnerName_Data
    {
        get
        {
            return owner_cs.PlayerName_Data;
        }
    }
}
