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
    }

    private void Update()
    {
        if (weapon_use == true)
        {
            //SE再生
            sound01.PlayOneShot(sound01.clip);
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
                        owner_cs.ControllerLock_Data = false;
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
                    owner_cs.ControllerLock_Data = false;
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
        if (collision.tag == "Player" && collision.name != owner_cs.PlayerName_Data)
        {
            //プレイヤーにダメージを与える
            PSManager_cs.Player_Damage(collision.gameObject, this.gameObject, collision.transform.GetComponent<Player>().PlayerNumber_data);

            base.EffectOccurrence(this.transform.GetChild(0).position, Vector3.zero);

            //ヒットSE再生
            sound02.PlayOneShot(sound02.clip);
        }
    }
}
