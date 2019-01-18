using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 

public class RVTest : MonoBehaviour
{
    float time = 0;
    List<RClass> rClasses = new List<RClass>();
    Dictionary<RClass, RClass> rClassesDic = new Dictionary<RClass, RClass>();
    Dictionary<RClass, int> rClassesDic2 = new Dictionary<RClass, int>();

    Dictionary<object, object> objectDic;
    Dictionary<Dictionary<int, int>, int> dic= new Dictionary<Dictionary<int, int>, int>();

    object[] array = new object[5];
    int[] arrayInt = new int[2];

    Countdown cd;
    RClass rc;

    // Use this for initialization
    void Start()
    {
        cd = new Countdown(1f, delegate ()
        {
            rClasses.Add(new RClass() { ID = rClasses.Count });
            if (rClasses.Count > 8)
                rClasses.Clear();
        });
        cd.Start(false);

        for (int i = 0; i < 3; i++)
        {
            rClasses.Add(new RClass() { ID = i });
            rClassesDic.Add(new RClass() { ID = i }, new RClass() { ID = i * 100 });
            rClassesDic2.Add(new RClass() { ID = i }, 999);
        }
        rClasses.Add(null);
        rClassesDic.Add(new RClass() { ID = 9999 }, null);

        array[0] = 99;
        array[1] = null;
        array[2] = Vector3.one;
        array[3] = new List<int>() { 1, 2 };
        array[4] = new RClass();

        arrayInt[0] = 12;
        arrayInt[1] = 13;

        rc = new RClass();

        objectDic = new Dictionary<object, object>();

        Dictionary<object, object> subDic = new Dictionary<object, object>();
        subDic.Add(99, "TEST");
        subDic.Add(98, "TEST2");

        objectDic.Add(subDic, "xxxx");
        objectDic.Add(": )", array);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        cd.Update();
    }
}

public class RClass
{
    public int ID;
    int level = Random.Range(0, 999);
    float exp = Random.Range(990f, 99999f);
    RClass2 rClass2 = new RClass2();  

}

public class RClass2
{
    public float Hp = Random.Range(990f, 99999f);
}