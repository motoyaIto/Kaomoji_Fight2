using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class Doragunohu : Weapon {

    bool IsHold = false;        //構えている(true)いない(false) 
    float BulletSpeed = 0.3f;   //弾の速度
    float Range = 100.0f;       //射程
    float ArrivalTime = 0.2f;   //到達タイム
    private AudioSource sound01;            // 撃つSE

    protected override void Start()
    {
        base.Start();

        DamageValue = 50;
    }

    private void Update()
    {
        if(weapon_use == true)
        {
            this.Hold();
        }
        else
        {
            //下に持つ
            foreach (Transform Child in this.transform.parent)
            {
                if (Child.name == "Down")
                {
                    this.transform.position = Child.position;
                }
            }
        }
    }

    /// <summary>
    /// 構えているときの処理
    /// </summary>
    private void Hold()
    {
        Vector2 input = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, owner_cs.GetControllerNamber), XCI.GetAxis(XboxAxis.LeftStickY, owner_cs.GetControllerNamber));

        //構えを解除する
        if (XCI.GetButtonDown(XboxButton.X, owner_cs.GetControllerNamber))
        {
            weapon_use = false;
            owner_cs.ControllerLock_Data = false;
            IsHold = false;
            //カーソルを非表示に
            this.transform.GetChild(1).gameObject.SetActive(false);
            //回転をリセットする
            this.transform.rotation = Quaternion.AngleAxis(0.0f, new Vector3(0, 0, 1));

            return;
        }

        //向きの調整
        this.Direction(input);

        //角度の調整
        this.Angle(input);

        //撃つ
        if (XCI.GetButtonDown(XboxButton.B, owner_cs.GetControllerNamber) && IsHold == true)
        {
            this.Shoot(input);
        }
        else
        {
            IsHold = true;
        }
    }
    /// <summary>
    /// 向きの調整
    /// </summary>
    /// <param name="input">入力</param>
    private void Direction(Vector2 input)
    {
        //右に向く
        if (input.x > 0.8f && this.transform.localScale.x < 0)
        {
            //回転をリセットする
            this.transform.rotation = Quaternion.AngleAxis(0.0f, new Vector3(0, 0, 1));

            //画像の反転
            this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y, this.transform.localScale.z);

            //座標の調整
            foreach (Transform child in this.transform.parent)
            {
                if (child.name == "Right")
                {
                    this.transform.position = new Vector3(child.position.x + 1.5f, child.position.y, child.position.z);
                }
            }
        }
        //左に向く
        if (input.x < -0.8f && this.transform.localScale.x > 0)
        {
            //回転をリセットする
            this.transform.rotation = Quaternion.AngleAxis(0.0f, new Vector3(0, 0, 1));

            //画像の反転
            this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y, this.transform.localScale.z);

            //座標の調整
            foreach (Transform child in this.transform.parent)
            {
                if (child.name == "Left")
                {
                    this.transform.position = new Vector3(child.position.x - 1.5f, child.position.y, child.position.z);
                }
            }

        }
    }

    /// <summary>
    /// 角度の調整
    /// </summary>
    /// <param name="input">入力</param>
    private void Angle(Vector2 input)
    {
        float rotation = 0.0f;
        //右
        if(this.transform.localScale.x > 0)
        {
            //上に向く
            if (input.y > 0.8f && this.transform.localRotation.z < 0.15f)
            {
                rotation = 0.1f;
                
            }
            //下に向く
            if (input.y < -0.8f && this.transform.localRotation.z > -0.15f)
            {
                rotation = -0.1f;
            }
        }

        //左
        if (this.transform.localScale.x < 0)
        {
            //上に向く
            if (input.y > 0.8f && this.transform.localRotation.z > -0.15f)
            {
                rotation = -0.1f;

            }
            //下に向く
            if (input.y < -0.8f && this.transform.localRotation.z < 0.15f)
            {
                rotation = 0.1f;
            }
        }


        this.transform.RotateAround(this.transform.GetChild(0).position, Vector3.forward, rotation);
    }

    public void Shoot(Vector2 input)
    {
        this.transform.GetChild(2).GetComponent<Bullet>().Owner_csData = owner_cs;
        //銃弾の調整
        this.transform.GetChild(2).gameObject.SetActive(true);

        this.transform.GetChild(2).GetComponent<Bullet>().DamageValue_Data = DamageValue;
        this.transform.GetChild(2).GetComponent<Bullet>().BulletRange_Data = Range;
        this.transform.GetChild(2).GetComponent<Bullet>().EndPoint_Data = this.transform.GetChild(1).position;
        this.transform.GetChild(2).GetComponent<Bullet>().EndTime_Data = ArrivalTime;
        
        //右向き
        if (this.transform.localScale.x > 0)
        {
            this.transform.GetChild(2).localPosition += this.transform.position;

            //銃弾に速度を与える
            this.transform.GetChild(2).GetComponent<Bullet>().BulletSpeed_Data = BulletSpeed;
        }
        else//左向き
        {
            this.transform.GetChild(2).localPosition = new Vector3(this.transform.GetChild(2).localPosition.x + this.transform.position.x, this.transform.position.y - this.transform.GetChild(2).localPosition.y, this.transform.GetChild(2).localPosition.z + this.transform.position.z);

            //銃弾に速度を与える
            this.transform.GetChild(2).GetComponent<Bullet>().BulletSpeed_Data = -BulletSpeed;
        }

        //大きさ
        this.transform.GetChild(2).localScale = new Vector3(this.transform.GetChild(2).localScale.x * this.transform.localScale.x, this.transform.GetChild(2).localScale.y * this.transform.localScale.y, this.transform.GetChild(2).localScale.z * this.transform.localScale.z);

        this.transform.GetChild(2).SetParent(null, false);

        //弾切れで破棄
        Destroy(this.gameObject);

        owner_cs.Weapon_useData = false;
        owner_cs.ControllerLock_Data = false;
    }

    public override void Attack()
    {
        Vector2 input = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, owner_cs.GetControllerNamber), XCI.GetAxis(XboxAxis.LeftStickY, owner_cs.GetControllerNamber));

        weapon_use = true;
        owner_cs.ControllerLock_Data = true;
        owner_cs.Directtion_Data = 0.0f;

        this.transform.GetChild(1).gameObject.SetActive(true);

        //SE再生
        sound01.PlayOneShot(sound01.clip);

        //武器を右か左に寄せる
        if (input.x >= 0)
        {
            foreach (Transform child in this.transform.parent.transform)
            {
                if (child.name == "Right")
                {
                    this.transform.position = new Vector3(child.transform.position.x + 1.5f, child.transform.position.y, child.transform.position.z);

                    //画像の向きを調整
                    if(this.transform.localScale.x < 0)
                    {
                        this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y, this.transform.localScale.z);
                    }
                }
            }
        }
        else
        {
            foreach (Transform child in this.transform.parent.transform)
            {
                if (child.name == "Left")
                {
                    this.transform.position = new Vector3(child.transform.position.x - 1.5f, child.transform.position.y, child.transform.position.z);

                    //画像の向きを調整
                    if (this.transform.localScale.x > 0)
                    {
                        this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y, this.transform.localScale.z);
                    }
                }
            }
        }
    }
}
