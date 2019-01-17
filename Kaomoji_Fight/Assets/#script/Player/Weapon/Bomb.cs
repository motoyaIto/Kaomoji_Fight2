using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class Bomb : Weapon {

    private Vector2 speed = Vector2.zero; //飛ばしてる弾の速度
    private float thrust = 30.8f;          // 爆弾の推進力
    protected override void Start()
    {
        base.Start();

        DamageValue = 20;

        collider = this.transform.GetComponent<CircleCollider2D>();

        Effect = Resources.Load<GameObject>("prefab/Effect/Explosion");
        EffectWait = 0.8f;
       

        //エフェクトの生成
        Effect = Instantiate(Effect) as GameObject;

        //爆破エフェクトに値を渡す
        BombEffect bombEffect_cs = Effect.GetComponent<BombEffect>();
        bombEffect_cs.PSManager_csData = PSManager_cs;
        bombEffect_cs.Weapon_Data = this.gameObject;
        bombEffect_cs.OwnerName_Data = owner_cs.PlayerName_Data;
    }

    private void Update()
    {
        if (weapon_use == true)
        {
            Vector2 input = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, owner_cs.GetControllerNamber), XCI.GetAxis(XboxAxis.LeftStickY, owner_cs.GetControllerNamber));

            //Bボタンを離したときに左スティックを投げたい方向に入力していたら
            if (XCI.GetButtonUp(XboxButton.B, owner_cs.GetControllerNamber) && (input.x != 0 && input.y != 0))
            {
                owner_cs.ControllerLock_Data = false;

                owner_cs.Weapon_useData = false;

                // 親から離れる
                this.transform.parent = null;

                //あたり判定をオンにする
                collider.enabled = true;
                

                //リジッドボディをセット
                Rigidbody2D rig2d = this.transform.gameObject.AddComponent<Rigidbody2D>();
                rig2d.gravityScale = .00f;

                // 投げる
                speed = input * thrust;
                this.transform.GetComponent<Rigidbody2D>().velocity = speed;
            }

            //Bボタンを離したら
            if (XCI.GetButtonUp(XboxButton.B, owner_cs.GetControllerNamber))
            {
                owner_cs.ControllerLock_Data = false;
                weapon_use = false;
            }

            if(this.transform.position.y > 80 || this.transform.position.y < -40 || this.transform.position.x < -20 || this.transform.position.x > 80)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public override void Attack()
    {
        owner_cs.ControllerLock_Data = true;
        owner_cs.Directtion_Data = 0.0f;
        weapon_use = true;

        //武器を真ん中でかまえる
        foreach (Transform child in this.transform.parent.transform)
        {
            if (child.name == "Top")
            {
                this.transform.position = child.transform.position;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.name != ownerName)
        {
            this.transform.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            this.gameObject.SetActive(false);

            Effect.transform.parent = null;
            Effect.SetActive(true);
            //エフェクトの発生場所を指定の座標に
            Effect.transform.position = this.transform.position;

            //エフェクト発生を待って破棄する
            //StartCoroutine(this.DelayMethod(EffectWait, () => { Destroy(Effect); }));

            Destroy(this.gameObject, 1.0f);
        }
    }
}
