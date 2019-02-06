using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;


public class Knife : Weapon {

    private int[] InstantDeathNamber;      //ランダムで即死を与える値
    private float InstantDeathProbability = 0.04f;//即死の確率
    private GameObject InstatDeath_Effect;
    private AudioSource sound01;                  // 振るSE
    private AudioSource sound02;                  //ヒットSE
    private AudioSource sound03;                  //即死SE
    /// <summary>
    /// 武器を目的の位置にずらす
    /// </summary>
    private void Weapon_bring(string position)
    {
        foreach (Transform child in this.transform.parent.transform)
        {
            if (child.name == position)
            {
                this.gameObject.transform.position = child.transform.position;
            }
        }
    }


    protected override void Start()
    {
        base.Start();

        DamageValue = 4;
        StiffnessTime = 0.5f;

        collider = this.transform.GetComponent<BoxCollider2D>();
        //this.transform.rotation = new Quaternion(0, 0, 1.0f, this.transform.rotation.w);

        //即死を与える値を取得
        InstantDeathNamber = new int[Mathf.FloorToInt(100 * InstantDeathProbability)];
        for (int i = 0; i < InstantDeathNamber.Length; i++)
        {
            InstantDeathNamber[i] = Random.Range(1, 100);

            if (i == 0)
            {
                i += 1;
                InstantDeathNamber[i] = Random.Range(1, 100);
            }

            //数字のかぶりがないかを検出する
            for (int j = 0; j < i - 1; j++)
            {
                if (InstantDeathNamber[i] == InstantDeathNamber[j])
                {
                    i -= 1;
                    break;
                }
            }
        }

        Effect = Resources.Load<GameObject>("prefab/Effect/Knife_Effect");
        EffectWait = 1.0f;
        InstatDeath_Effect = Resources.Load<GameObject>("prefab/Effect/Knife_InstantDeath_Effect");
    }

    private void Update()
    {
        if (weapon_use == false)
        {
            //SE再生
            sound01.PlayOneShot(sound01.clip);
            //武器を左に寄せる
            if (this.transform.localPosition.x < 0)
            {
                Weapon_bring("DownLeft");
                this.transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
                //SRenderer.flipY = false;
                return;
            }

            //武器を右に寄せる
            if (this.transform.localPosition.x > 0)
            {
                Weapon_bring("DownRight");
                this.transform.rotation = Quaternion.AngleAxis(-90, Vector3.forward);
                //SRenderer.flipY = true;
                return;
            }
        }
    }

    public override void Attack()
    {
        if (weapon_use == true) return;
        weapon_use = true;
        owner_cs.ControllerLock_Data = true;
        owner_cs.Directtion_Data = 0.0f;
        collider.enabled = true;

        //動作ロックを解除する
        StartCoroutine(base.DelayMethod(StiffnessTime, () => { weapon_use = false; owner_cs.ControllerLock_Data = false; collider.enabled = false; }));

        if (weapon_use)
        {
            // Controllerの左スティックのAxisを取得            
            Vector2 input = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, this.transform.parent.GetComponent<Player>().GetControllerNamber), XCI.GetAxis(XboxAxis.LeftStickY, this.transform.parent.GetComponent<Player>().GetControllerNamber));

            //武器を左に寄せる
            if (input.x < 0.0f && input.y <= 0.7f && input.y >= -0.7f)
            {
                Weapon_bring("Left");
                this.transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
                return;
            }

            //武器を右に寄せる
            if (input.x > 0 && input.y <= 0.7f && input.y >= -0.7f)
            {
                Weapon_bring("Right");
                this.transform.rotation = Quaternion.AngleAxis(-90, Vector3.forward);
                return;
            }

            //武器を上に寄せる
            if (input.y > 0 && input.x <= 0.7f && input.x >= -0.7f)
            {
                Weapon_bring("Top");
                this.transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
                return;
            }

            //武器を下に寄せる
            if (input.y < 0 && input.x <= 0.7f && input.x >= -0.7f)
            {
                Weapon_bring("Down");
                this.transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.name != owner_cs.PlayerName_Data)
        {
            bool InstantDeath = false;

            //ランダムで同じ数字が出たら即死
            for (int i = 0; i < InstantDeathNamber.Length; i++)
            {
                int test = Random.Range(1, 100);
                if (InstantDeathNamber[i] == test)
                {
                    InstantDeath = true;

                    //エフェクトの発生
                    GameObject EffectObj = Instantiate(InstatDeath_Effect) as GameObject;
                    // エフェクトの発生場所をプレイヤーの中心に
                    EffectObj.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);

                    //エフェクト発生を待って破棄する
                    StartCoroutine(this.DelayMethod(2.0f, () => { Destroy(EffectObj); }));

                    //即死SE再生
                    sound03.PlayOneShot(sound03.clip);

                    break;
                }
                //ヒットSE再生
                sound02.PlayOneShot(sound02.clip);
            }

            if(InstantDeath == false)
            {
                EffectOccurrence(new Vector3(this.transform.position.x, this.transform.position.y, 0), Vector3.zero);
            }

            PManager_cs.WeaponAttack(collision.gameObject, this.gameObject);
        }
    }
}
