using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portion : Weapon
{
    protected override void Start()
    {
        base.Start();

        Effect = Resources.Load<GameObject>("prefab/Effect/Recovery");
        EffectWait = 0.5f;
        DamageValue = -20;
    }

    public override void Attack()
    {
        //回復
        PSManager_cs.Effect_myself(this.transform.parent.gameObject, this.gameObject, this.transform.parent.GetComponent<Player>().PlayerNumber_data);

        //回復エフェクト
        Effect = Instantiate(Effect, this.transform) as GameObject;
        Effect.transform.position = new Vector3(this.transform.parent.transform.position.x, this.transform.position.y, 0);

        //エフェクト発生を待って破棄する
        StartCoroutine(base.DelayMethod(EffectWait, () => { Destroy(this.gameObject); }));
        owner_cs.Weapon_useData = false;
    }
}
