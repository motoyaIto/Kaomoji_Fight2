using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerFlashing : MonoBehaviour {

    private SpriteRenderer color;
    [SerializeField]
    private bool Flashing = false;

    private bool up = false;
	void Start () {
        color = this.transform.GetComponent<SpriteRenderer>();

        color.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    }

    void Update() {
        if (Flashing == true)
        {
            if (up == true)
            {
                color.color += new Color(0.0f, 0.0f, 0.0f, .01f);
            }
            else
            {
                color.color -= new Color(0.0f, 0.0f, 0.0f, .01f);
            }

            if (color.color.a >= 0.9f && up == true)
            {
                up = false;
            }
            if (color.color.a <= 0.1f && up == false)
            {
                up = true;
            }
        }
        else
        {
            color.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            up = false;
        }
	}

    public void FlashinColor(bool FlashinFlag)
    {
        Flashing = FlashinFlag;
    }
}
