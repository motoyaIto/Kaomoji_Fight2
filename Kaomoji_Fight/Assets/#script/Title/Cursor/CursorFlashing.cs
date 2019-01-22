using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFlashing : MonoBehaviour {

    private SpriteRenderer CursorSprite;

    [SerializeField]
    private float ChangeColor_red;
    [SerializeField]
    private float ChangeColor_green;
    [SerializeField]
    private float ChangeColor_blue;
   

    [SerializeField]
    private bool Colorchange = false;//色替え(true)しない(false)

    private float red = 0.0f;
    private bool redFlag = false;
    private float green = 0.0f;
    private bool greenFlag = false;
    private float blue = 0.0f;
    private bool blueFlag = false;

    private void Start()
    {
        CursorSprite = this.transform.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (redFlag == false) { red += ChangeColor_red; }else { red -= ChangeColor_red; }
        if (greenFlag == false) { green += ChangeColor_green; } else { green -= ChangeColor_green; }
        if (blueFlag == false) { blue += ChangeColor_blue; } else { blue -= ChangeColor_blue; }
        
        if(red > 254 && redFlag == false) { redFlag = true; return; }
        if(green > 254 && redFlag == false) { greenFlag = true; return; }
        if(blue > 254 && redFlag == false) { blueFlag = true; return; }

        if (red < 1 && redFlag == true) { redFlag = false; return; }
        if (green < 1 && redFlag == true) { greenFlag = false; return; }
        if (blue < 1 && redFlag == true) { blueFlag = false; return; }

        CursorSprite.color = new Color(red / 255, green / 255, blue / 255);
    }
}
