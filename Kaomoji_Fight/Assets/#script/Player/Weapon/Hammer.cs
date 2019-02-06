using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : Weapon
{
    float RotationalDistanc = 0.0f; //回転距離
    float Acceleration = 1.12f;     //加速度
    string LeftRight = "";          //右か左か
    Vector3 targetPos;              //中心に回る座標
    private AudioSource sound01;    // 振るSE
    private AudioSource sound02;    //ヒットSE

    protected override void Start()
    {
        base.Start();

        DamageValue = 15;

        collider = this.transform.GetComponent<BoxCollider2D>();

        Effect = Resources.Load<GameObject>("prefab/Effect/Hammer_Effect");
        EffectWait = 0.2f;

        // 持ち主でないのなら持ち主にデータを要請する
        if (!this.transform.GetComponent<PhotonView>().isMine)
        {
            base.PleaseMyData();

            return;
        }
    }

    private void Update()
    {
        // 持ち主でないのなら持ち主にデータを要請する
        //if (!this.transform.GetComponent<PhotonView>().isMine) return;

        if (weapon_use == true)
        {
            //SE再生
            //sound01.PlayOneShot(sound01.clip);
            //ニュートラルの時
            if (LeftRight == "")
            {
                DamageValue = 8;
                if (RotationalDistanc < 90.0f && RotationalDistanc > -90.0f)
                {
                    //時間毎に加速する
                    RotationalDistanc -= Time.deltaTime;
                    RotationalDistanc *= Acceleration;

                    foreach (Transform child in this.transform.parent)
                    {
                        if (child.name == "Top")
                        {
                            targetPos = child.position;

                            break;
                        }
                    }

                    this.transform.RotateAround(targetPos, Vector3.forward, RotationalDistanc);
                }
                else
                {
                    StartCoroutine(base.DelayMethod(0.5f, () =>
                    {
                        DamageValue = 15;
                        // 持ち主でないのなら持ち主にデータを要請する
                        if (this.transform.GetComponent<PhotonView>().isMine == true)
                        {
                            owner_cs.ControllerLock_Data = false;
                        }
                        weapon_use = false;

                        this.transform.GetComponent<BoxCollider2D>().enabled = false;

                        //元の角度に戻す
                        RotationalDistanc = 0.0f;
                        this.transform.rotation = Quaternion.AngleAxis(RotationalDistanc, new Vector3(0, 0, 1));

                        LeftRight = "";
                    }));

                    return;
                }

            }


            if (RotationalDistanc < 10.0f && RotationalDistanc > -10.0f)
            {
                //左に振る
                if (LeftRight == "Left")
                {
                    //時間毎に加速する
                    RotationalDistanc += Time.deltaTime;
                    RotationalDistanc *= Acceleration;

                    foreach (Transform child in this.transform.parent)
                    {
                        if (child.name == "TopLeft")
                        {
                            targetPos = child.position;

                            break;
                        }
                    }
                }
                //右に振る
                if (LeftRight == "Right")
                {
                    //時間毎に加速する
                    RotationalDistanc -= Time.deltaTime;
                    RotationalDistanc *= Acceleration;

                    foreach (Transform child in this.transform.parent)
                    {
                        if (child.name == "TopRight")
                        {
                            targetPos = child.position;

                            break;
                        }
                    }
                }

                //ハンマーを回す
                //this.transform.rotation = Quaternion.AngleAxis(RotationalDistanc, new Vector3(0, 0, 1));
                this.transform.RotateAround(targetPos, Vector3.forward, RotationalDistanc);
            }
            else
            {
                StartCoroutine(base.DelayMethod(0.5f, () =>
                {
                    // 持ち主でないのなら持ち主にデータを要請する
                    if (this.transform.GetComponent<PhotonView>().isMine == true)
                    {
                        owner_cs.ControllerLock_Data = false;
                    }
                    weapon_use = false;

                    this.transform.GetComponent<BoxCollider2D>().enabled = false;

                    //元の角度に戻す
                    RotationalDistanc = 0.0f;
                    this.transform.rotation = Quaternion.AngleAxis(RotationalDistanc, new Vector3(0, 0, 1));

                    LeftRight = "";
                }));

            }
        }
        else
        {
            if (this.transform.parent != null)
            {
                //武器を使用してないときは常に上でかまえる
                foreach (Transform child in this.transform.parent)
                {
                    if (child.name == "Top")
                    {
                        this.transform.position = child.position;
                    }
                }
            }
        }
    }

    public override void Attack()
    {
        weapon_use = true;
        owner_cs.ControllerLock_Data = true;
        owner_cs.Directtion_Data = 0.0f;

        collider.enabled = true;

        //右上かまえる
        if (this.transform.localPosition.x > 0)
        {
            foreach (Transform child in this.transform.parent)
            {
                if (child.name == "TopRight")
                {
                    targetPos = child.position;
                    this.transform.position = child.position + new Vector3(0, 1.5f, 0);
                    LeftRight = "Right";

                    PhotonView photonView = this.GetComponent<PhotonView>();
                    photonView.RPC("AttackeData", PhotonTargets.OthersBuffered, LeftRight);
                    return;
                }
            }
        }

        //左上かまえる
        if (this.transform.localPosition.x < 0)
        {
            foreach (Transform child in this.transform.parent)
            {
                if (child.name == "TopLeft")
                {
                    targetPos = child.position;
                    this.transform.position = child.position + new Vector3(0, 1.5f, 0);
                    LeftRight = "Left";
                    this.transform.rotation = new Quaternion(this.transform.rotation.x, 180, this.transform.rotation.z, this.transform.rotation.w);

                    PhotonView photonView = this.GetComponent<PhotonView>();
                    photonView.RPC("AttackeData", PhotonTargets.OthersBuffered, LeftRight);
                    return;
                }
            }
        }

        foreach (Transform child in this.transform.parent)
        {
            if (child.name == "Top")
            {
                targetPos = child.position;
                this.transform.position = child.position + new Vector3(0, 1.5f, 0);

                PhotonView photonView = this.GetComponent<PhotonView>();
                photonView.RPC("AttackeData", PhotonTargets.OthersBuffered, LeftRight);
                return;
            }
        }
    }

    [PunRPC]
    private void AttackeData(string attack)
    {
        weapon_use = true;

        collider.enabled = true;

        //右上かまえる
        if (attack == "Right")
        {
            foreach (Transform child in this.transform.parent)
            {
                if (child.name == "TopRight")
                {
                    targetPos = child.position;
                    this.transform.position = child.position + new Vector3(0, 1.5f, 0);
                    LeftRight = "Right";
                    return;
                }
            }
        }

        //左上かまえる
        if (attack == "Left")
        {
            foreach (Transform child in this.transform.parent)
            {
                if (child.name == "TopLeft")
                {
                    targetPos = child.position;
                    this.transform.position = child.position + new Vector3(0, 1.5f, 0);
                    LeftRight = "Left";
                    this.transform.rotation = new Quaternion(this.transform.rotation.x, 180, this.transform.rotation.z, this.transform.rotation.w);
                    return;
                }
            }
        }

        foreach (Transform child in this.transform.parent)
        {
            if (child.name == "Top")
            {
                targetPos = child.position;
                this.transform.position = child.position + new Vector3(0, 1.5f, 0);
                return;
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
            //プレイヤーにダメージを与える
            PManager_cs.WeaponAttack(collision.gameObject, this.gameObject);

            base.EffectOccurrence(this.transform.GetChild(0).position, Vector3.zero);

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
