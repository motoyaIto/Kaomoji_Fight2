﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class SelectWeapon {

    private static bool start = false;
    private static Dictionary<string, GameObject> WeaponList = new Dictionary<string, GameObject>();


    static void Start () {

        WeaponList["かいふく"] = Resources.Load<GameObject>("prefab/Weapon/Portion");
        WeaponList["けん"] = Resources.Load<GameObject>("prefab/Weapon/Sword");

        WeaponList["じゅう"] = Resources.Load<GameObject>("prefab/Weapon/Gan");
        WeaponList["すないぱー"] = Resources.Load<GameObject>("prefab/Weapon/Doragunohu");

        WeaponList["ないふ"] = Resources.Load<GameObject>("prefab/Weapon/knife");
        WeaponList["にんぎょう"] = null;

        WeaponList["はんまー"] = Resources.Load<GameObject>("prefab/Weapon/Hammer");
        WeaponList["ばくだん"] = Resources.Load<GameObject>("prefab/Weapon/Bomb");

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
        if (name == "きゅあ" || name == "ひーる" || name == "やくそう" || name == "ぽーしょん" || name == "けある" || name == "あすな")
        {
            name = "かいふく";
        }

        //剣
        if(name == "そーど" || name == "つるぎ" || name == "かたな" || name == "しみたー" || name == "ぶれいど" || name == "きりと" || name == "りんく" || name == "えくしあ" || name == "さやか")
        {
            name = "けん";
        }

        //銃
        if(name == "がん" || name == "ぴすとる" || name == "ちゃか" || name == "こると" || name == "てっぽう" || name == "まみ")
        {
            name = "じゅう";
        }
        //スナイパー
        if(name == "そげき" || name == "どらぐのふ" || name == "あさると" || name == "えんきょり" || name == "けるでぃむ" || name == "でゅなめす" || name == "すぽったー" || name == "ろっくおん" || name == "すないぷ" || name == "へかーと" || name == "しのん")
        {
            name = "すないぱー";
        }

        //ナイフ
        if (name == "こがたな" || name == "だがー" || name == "しりか")
        {
            name = "ないふ";
        }

        //ハンマー
        if (name == "つち" || name == "ぴこはん" || name == "もろとーく" || name == "りず")
        {
            name = "はんまー";
        }

        //爆弾
        if(name == "ぼむ" || name == "なぱーむ" || name == "ばくやく" || name == "ぐれねーど" || name == "ほむら")
        {
            name = "ばくだん";
        }

        //槍
        if (name == "すぴあ" || name == "らんす" || name == "さち" || name == "きょうこ　　　")
        {
            name = "やり";
        }

        if (WeaponList.ContainsKey(name) == true)
        {
            return WeaponList[name];
        }

        return null;
    }
}
