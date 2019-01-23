using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTest : MonoBehaviour {

    [SerializeField]
    ColorPicker InputFeld;

    private void Start()
    {
        //nputFeld.transform.GetComponent<HexColorField>().color
    }

    private void Update()
    {
        this.transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(InputFeld.R, InputFeld.G,InputFeld.B));
        Debug.Log(InputFeld.R + "" + InputFeld.G + "" + InputFeld.B);
    }
}
