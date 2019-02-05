using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yari : Weapon {

    private AudioSource sound01;            // 振るSE
    private AudioSource sound02;            //ヒットSE
    protected override void Start()
    {
        base.Start();

        StiffnessTime = 0.8f;
        DamageValue = 8;

        collider = this.transform.GetComponent<BoxCollider2D>();

        Effect = Resources.Load<GameObject>("prefab/Effect/Yari_Effect");
        EffectWait = 0.2f;
    }

    private void Update()
    {
        if (weapon_use == true)
        {
            //SE再生
            sound01.PlayOneShot(sound01.clip);
            count += Time.deltaTime;

            if (SRenderer.flipX == false)
            {
                owner_cs.Directtion_Data = 3.5f;
            }
            else
            {
                owner_cs.Directtion_Data = -3.5f;
            }

            //硬直時間を過ぎたら
            if (count > StiffnessTime)
            {
                StartCoroutine(base.DelayMethod(0.4f,()=> { owner_cs.ControllerLock_Data = false; weapon_use = false; collider.enabled = false;  }));
                owner_cs.Directtion_Data = 0f;




            }
        }
    }

    public override void Attack()
    {
        if (weapon_use == true) return;
        weapon_use = true;
        owner_cs.ControllerLock_Data = true;
        owner_cs.Directtion_Data = 0.0f;

        count = 0.0f;
        collider.enabled = true;

        //SE再生
        sound01.PlayOneShot(sound01.clip);
        //武器を右か左に寄せる
        if (this.transform.localPosition.x > 0)
        {
            foreach (Transform child in this.transform.parent.transform)
            {
                if (child.name == "Right")
                {
                    this.transform.position = new Vector3(child.transform.position.x + 1.5f, child.transform.position.y, child.transform.position.z);

                    //画像の向きを合わせる
                    SRenderer.flipX = false;
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

                    //画像の向きを合わせる
                    SRenderer.flipX = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.name != owner_cs.PlayerName_Data)
        {
            PSManager_cs.Player_Damage(collision.gameObject, this.gameObject, collision.transform.GetComponent<Player>().PlayerNumber_data);

            if(SRenderer.flipX == false)
            {
                base.EffectOccurrence(this.transform.position + new Vector3(1, 0, 0), Vector3.zero);
                owner_cs.Directtion_Data = 5.5f;
            }
            else
            {
                base.EffectOccurrence(this.transform.position + new Vector3(-1, 0, 0), new Vector3(-180, 0, 0));
                owner_cs.Directtion_Data = -5.5f;
            }
            sound02.PlayOneShot(sound02.clip);
        }
    }
}
