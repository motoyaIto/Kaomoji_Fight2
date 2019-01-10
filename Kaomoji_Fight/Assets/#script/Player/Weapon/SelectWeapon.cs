using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class SelectWeapon {

    private static bool start = false;
    private static Dictionary<string, GameObject> WeaponList = new Dictionary<string, GameObject>();


    static void Start () {
        WeaponList["ばくだん"] = null;
        WeaponList["ないふ"] = Resources.Load<GameObject>("prefab/Weapon/knife");
        WeaponList["やり"] = Resources.Load<GameObject>("prefab/Weapon/Yari");
        WeaponList["はんまー"] = Resources.Load<GameObject>("prefab/Weapon/Hammer");

        WeaponList["かいふく"] = Resources.Load<GameObject>("prefab/Weapon/Portion");

        WeaponList["まくら"] = null;
        WeaponList["にんぎょう"] = null;

        WeaponList["けん"] = null;
    }

    public static GameObject CreateSelectWeapon(string name)
    {
        if (start == false)
        {
            Start();
            start = true;
        }

        //回復
        if (name == "きゅあ" || name == "ひーる")
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
        if(name == "つち" || name == "ぴこはん" || name == "きね" || name == "もろとーく")
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
