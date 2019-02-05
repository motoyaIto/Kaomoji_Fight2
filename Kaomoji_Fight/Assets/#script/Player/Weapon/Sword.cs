using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon {

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
    }

    private void Update()
    {
        if(weapon_use == true)
        {
            //SE再生
            sound01.PlayOneShot(sound01.clip);
            //剣を回転
            RotationalDistanc += Time.deltaTime * 80.0f;
            this.transform.RotateAround(this.transform.parent.position, Vector3.up, RotationalDistanc);

            //剣が一周したら止める
            if(RotationalDistanc > 20)
            {
                weapon_use = false;
                owner_cs.ControllerLock_Data = false;
                owner_cs.WeaponMoveLockData = false;
                RotationalDistanc = 0.0f;
                collider.enabled = false;

                this.transform.rotation = Quaternion.identity;
            }
        }
    }

    public override void Attack()
    {
        if(weapon_use == true)
        {
            return;
        }
        weapon_use = true;
        owner_cs.ControllerLock_Data = true;
        owner_cs.WeaponMoveLockData = true;
        collider.enabled = true;

        //剣を上に掲げて
        foreach (Transform child in this.transform.parent)
        {
            if (child.name == "Right")
            {           
                this.transform.position = child.position;
                this.transform.rotation = Quaternion.AngleAxis(-90, Vector3.forward);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.name != owner_cs.PlayerName_Data)
        {
            base.EffectOccurrence_World(this.transform.position + new Vector3(0.2f, 0.0f, 0.0f), Vector3.zero);
            PSManager_cs.Player_Damage(collision.gameObject, this.gameObject, collision.transform.GetComponent<Player>().PlayerNumber_data);
            //ヒットSE再生
            sound02.PlayOneShot(sound02.clip);
        }
    }
}
