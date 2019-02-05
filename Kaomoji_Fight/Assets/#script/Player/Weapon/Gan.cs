using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gan : Weapon {

    private int Bullet = 5;             //弾の数
    private float BulletSpeed =0.8f; //弾のスピード
    private string LeftRight = "Top";   //かまえている方向

    private float reaction = 0.5f;  //反動で打てない秒数
    private AudioSource sound01;            // 撃つSE

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        count += Time.deltaTime;

        Vector3 scale = this.transform.localScale;
        //左側にかまえる
        if (this.transform.localPosition.x < 0)
        {
            foreach(Transform child in this.transform.parent)
            {
                if(child.name == "Left")
                {
                    this.transform.localPosition = child.transform.localPosition;

                    LeftRight = "Left";
                    //向き調整
                    if (scale.x < 0)
                    {
                        this.transform.localScale = new Vector3(scale.x * -1, scale.y, scale.z);
                    }

                    break;
                }
            }

            return;
        }

        //右側にかまえる
        if (this.transform.localPosition.x > 0)
        {
            foreach (Transform child in this.transform.parent)
            {
                if (child.name == "Right")
                {
                    this.transform.localPosition = child.transform.localPosition;

                    LeftRight = "Right";
                    //向き調整
                    if (scale.x > 0)
                    {
                        this.transform.localScale = new Vector3(scale.x * -1, scale.y, scale.z);
                    }
                    break;
                }
            }

            return;
        }

        LeftRight = "Top";
    }

    public override void Attack()
    {
        if(LeftRight == "Top" || count < reaction) { return; }

        count = 0;

        //撃つ
        if (Bullet > 0)
        {
            //SE再生
            sound01.PlayOneShot(sound01.clip);
            this.transform.GetChild(0).GetComponent<Bullet>().Owner_csData = owner_cs;
            //銃弾の調整
            this.transform.GetChild(0).gameObject.SetActive(true);

            //座標の調整
            if (LeftRight == "Right")
            {
                this.transform.GetChild(0).localPosition += this.transform.position;

                //銃弾に速度を与える
                this.transform.GetChild(0).GetComponent<Bullet>().BulletSpeed_Data = BulletSpeed;
            }
            else
            {
                this.transform.GetChild(0).localPosition = new Vector3(this.transform.GetChild(0).localPosition.x + this.transform.position.x, this.transform.position.y - this.transform.GetChild(0).localPosition.y, this.transform.GetChild(0).localPosition.z + this.transform.position.z);

                //銃弾に速度を与える
                this.transform.GetChild(0).GetComponent<Bullet>().BulletSpeed_Data = -BulletSpeed;
            }

            //大きさ
            this.transform.GetChild(0).localScale = new Vector3(this.transform.GetChild(0).localScale.x * this.transform.localScale.x, this.transform.GetChild(0).localScale.y * this.transform.localScale.y, this.transform.GetChild(0).localScale.z * this.transform.localScale.z);

            this.transform.GetChild(0).SetParent(null, false);

           
            //銃弾の数をマイナス
            Bullet -= 1;

            //弾がなくなったら武器を捨てる
            if(Bullet <= 0)
            {
                Destroy(this.gameObject);

                owner_cs.Weapon_useData = false;
            }
        }
    }
}
