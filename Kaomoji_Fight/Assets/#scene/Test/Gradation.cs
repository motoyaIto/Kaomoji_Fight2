using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gradation : MonoBehaviour {

    public Gradient gradient;

    [Range(0, 1)]
    public float value = 0;

    void OnValidate()
    {
        this.GetComponent<Graphic>().color = gradient.Evaluate(value);
    }

}
