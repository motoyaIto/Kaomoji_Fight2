using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{

    Vector3 targetPos;              //中心に回る座標
    private float RotationalDistanc = 0.0f; //回転距離
    private AudioSource sound01;            // 振るSE
    private AudioSource sound02;            //ヒットSE
    protected override void Start()
    {
        base.Start();
        collider = this.transform.GetComponent<BoxCollider2D>();
        DamageValue = 3;

        Effect = Resources.Load<GameObject>("prefab/Effect/Sword_Effect");
        EffectWait = 0.3f;

        // 持ち主でないのなら持ち主にデータを要請する
        if (!this.transform.GetComponent<PhotonView>().isMine)
        {
            base.PleaseMyData();

            return;
        }
    }

    private void Update()
    {
        if (weapon_use == true)
        {
            //SE再生
            //sound01.PlayOneShot(sound01.clip);
            //剣を回転
            RotationalDistanc += Time.deltaTime * 80.0f;
            foreach (Transform child in this.transform.parent)
            {
                if (child.name == "Top")
                {
                    targetPos = child.position;

                    break;
                }
            }

            this.transform.RotateAround(targetPos, Vector3.up, RotationalDistanc);
            //this.transform.RotateAround(this.transform.parent.position, Vector3.up, RotationalDistanc);

            //剣が一周したら止める
            if (RotationalDistanc > 20)
            {
                weapon_use = false;
                // 自分以外のプレイヤーの画面の自分に情報を持たせる
                if (this.transform.GetComponent<PhotonView>().isMine == true)
                {
                    owner_cs.ControllerLock_Data = false;
                }
                RotationalDistanc = 0.0f;
                collider.enabled = false;

                this.transform.rotation = Quaternion.identity;
            }
        }
    }

    public override void Attack()
    {
        weapon_use = true;
        // 自分以外のプレイヤーの画面の自分に情報を持たせる
        if (this.transform.GetComponent<PhotonView>().isMine == true)
        {
            owner_cs.ControllerLock_Data = false;
        }
        owner_cs.WeaponMoveLockData = true;
        collider.enabled = true;

        //剣を上に掲げて
        foreach (Transform child in this.transform.parent)
        {
            if (child.name == "Right")
            {
                this.transform.position = child.position;
                this.transform.rotation = Quaternion.AngleAxis(-90, Vector3.forward);

                PhotonView photonView = this.GetComponent<PhotonView>();
                photonView.RPC("AttackeData", PhotonTargets.OthersBuffered, child.name);
                return;
            }
        }
    }


    [PunRPC]
    private void AttackeData(string attack)
    {
        weapon_use = true;
        collider.enabled = true;

        //剣を上に掲げて
        if (attack == "Right")
        {
            foreach (Transform child in this.transform.parent)
            {
                if (child.name == "Right")
                {
                    this.transform.position = child.position;
                    this.transform.rotation = Quaternion.AngleAxis(-90, Vector3.forward);
                }
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 持ち主でないのなら持ち主にデータを要請する
        if (!this.transform.GetComponent<PhotonView>().isMine)
        {
            return;
        }

        if (collision.tag == "Player" && collision.name != ownerName)
        {
            base.EffectOccurrence_World(this.transform.position + new Vector3(0.2f, 0.0f, 0.0f), Vector3.zero);
            PManager_cs.WeaponAttack(collision.gameObject, this.gameObject);
            //ヒットSE再生
            //sound02.PlayOneShot(sound02.clip);
        }
    }

    [PunRPC]
    private void PleasePlayerName(int ownerid, int id)
    {
        PhotonView photonView = this.GetComponent<PhotonView>();
        if (photonView.ownerId == ownerid && photonView.viewID == id)
        {
            photonView.RPC("ChathOwmerName", PhotonTargets.OthersBuffered);
        }
    }

    void OnPhotonSerializeView(PhotonStream i_stream, PhotonMessageInfo i_info)
    {
        if (weapon_use == false)
        {
            if (i_stream.isWriting)
            {
                //データの送信
                i_stream.SendNext(this.transform.position);
            }
            else
            {
                //データの受信
                Vector3 pos = (Vector3)i_stream.ReceiveNext();
                this.transform.position = pos;
            }
        }
    }
}
