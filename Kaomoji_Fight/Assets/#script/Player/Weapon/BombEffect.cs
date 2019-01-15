using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEffect : MonoBehaviour {

    private string OwnerName;       //オーナーの名前
    private GameObject Weapon;      //発生させたオブジェクト
    private PlaySceneManager PSManager_cs;//プレイシーンマネージャースクリプト


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.name != OwnerName)
        {
            PSManager_cs.Player_Damage(collision.gameObject, Weapon, collision.transform.GetComponent<Player>().PlayerNumber_data);
        }
    }

    public PlaySceneManager PSManager_csData
    {
        set
        {
            PSManager_cs = value;
        }
    }

    public GameObject Weapon_Data
    {
        set
        {
            Weapon = value;
        }
    }

    public string OwnerName_Data
    {
        set
        {
            OwnerName = value;
        }
    }
}
