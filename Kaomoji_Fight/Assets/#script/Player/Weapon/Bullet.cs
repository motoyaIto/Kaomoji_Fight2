using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Weapon {

    float BulletSpeed = 0.0f;       //弾の移動スピード

    private float Range = 10.0f;    //射程

    protected override void Start()
    {
        base.Start();

        DamageValue = 5;

        Effect = Resources.Load<GameObject>("prefab/Effect/Bullet_Effect");
        EffectWait = 0.2f;
    }

    public void Update()
    {
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
        if (collision.tag == "Player" && collision.name != owner_cs.PlayerName_Data)
        {
            base.EffectOccurrence_World(this.transform.position, Vector3.zero);
            PSManager_cs.Player_Damage(collision.gameObject, this.gameObject, collision.transform.GetComponent<Player>().PlayerNumber_data);
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
}
