using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class SelectWeapon {

    private static bool start = false;
    private static Dictionary<string, GameObject> WeaponList = new Dictionary<string, GameObject>();


    static void Start () {

        WeaponList["かいふく"] = Resources.Load<GameObject>("prefab/Weapon/Portion");
        WeaponList["けん"] = Resources.Load<GameObject>("prefab/Weapon/Sword");

        WeaponList["ないふ"] = Resources.Load<GameObject>("prefab/Weapon/knife");
        WeaponList["にんぎょう"] = null;

        WeaponList["はんまー"] = Resources.Load<GameObject>("prefab/Weapon/Hammer");
        WeaponList["ばくだん"] = null;

        WeaponList["まくら"] = null;

        WeaponList["やり"] = Resources.Load<GameObject>("prefab/Weapon/Yari");
    }

    public static GameObject CreateSelectWeapon(string name)
    {
        if (start == false)
        {
            Start();
            start = true;
        }

        //回復
        if (name == "きゅあ" || name == "ひーる" || name == "やくそう")
        {
            name = "かいふく";
        }

        //剣
        if(name == "そーど" || name == "つるぎ" || name == "かたな" || name == "しみたー" || name == "ぶれいど")
        {
            name = "けん";
        }
        
        //槍
        if(name == "すぴあ" || name == "らんす")
        {
            name = "やり";
        }

        //ハンマー
        if(name == "つち" || name == "ぴこはん" || name == "もろとーく")
        {
            name = "はんまー";
        }

        //ナイフ
        if(name == "こがたな" || name == "だがー")
        {
            name = "ないふ";
        }

        if (WeaponList.ContainsKey(name) == true)
        {
            return WeaponList[name];
        }

        return null;
    }
}
