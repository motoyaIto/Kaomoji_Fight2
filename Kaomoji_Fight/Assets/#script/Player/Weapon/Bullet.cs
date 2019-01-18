using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Weapon {

    private float BulletSpeed = 0.0f;       //弾の移動スピード

    private float Range = 10.0f;            //射程

    private Vector3 StartPoint = Vector3.zero;//スタートポジション
    private float EndTime = 0.0f;           //指定の座標まで何秒で飛ばすか
    private Vector3 EndPoint = Vector3.zero;//指定の座標まで弾を飛ばす
    

    protected override void Start()
    {
        base.Start();

        if (DamageValue <= 0)
        {
            DamageValue = 5;
        }

        Effect = Resources.Load<GameObject>("prefab/Effect/Bullet_Effect");
        EffectWait = 0.2f;
    }

    public void Update()
    {
        if(this.transform.parent == null && StartPoint == Vector3.zero)
        {
            StartPoint = this.transform.position;
        }

        //弾を飛ばす指定座標があれば
        if(EndPoint != Vector3.zero && weapon_use == true)
        {
            if (StartPoint == Vector3.zero) return;

            count += Time.deltaTime;

            float rate = count / EndTime;
            this.transform.position = Vector3.Lerp(StartPoint, EndPoint, rate);

            if(rate > 0.98)
            {
                Destroy(this.gameObject);
            }

            return;
        }

        //飛ばす座標の指定がない時
        if (Range > 0 && weapon_use == true)
        {
            Range -= Mathf.Abs(BulletSpeed);

            this.transform.position = new Vector3(this.transform.position.x + BulletSpeed, this.transform.position.y, this.transform.position.z);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public override void Attack()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //ステージとのあたり判定
        if (collision.tag == "Stage" && EndPoint != Vector3.zero)
        {
            if (collision.transform.GetComponent<BlockController>().Mozi == false)
            {
                Destroy(this.gameObject);
                return;
            }
            collision.transform.GetComponent<BlockController>().ChangeMozi();
            DamageValue -= 5;
        }

        //プレイヤーとのあたり判定
        if (collision.tag == "Player" && collision.name != owner_cs.PlayerName_Data)
        {
            base.EffectOccurrence_World(this.transform.position, Vector3.zero);
            PSManager_cs.Player_Damage(collision.gameObject, this.gameObject, collision.transform.GetComponent<Player>().PlayerNumber_data);

            if (EndPoint != Vector3.zero)
            {
                DamageValue -= 10;
            }
        }
    }

    public float BulletSpeed_Data
    {
        set
        {
            BulletSpeed = value;

            weapon_use = true;
        }
    }

    public float BulletRange_Data
    {
        set
        {
            Range = value;
        }
    }

    public Vector3 EndPoint_Data
    {
        set
        {
            EndPoint = value;
        }
    }

    public float EndTime_Data
    {
        set
        {
            EndTime = value;
        }
    }
}
