using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoObject : MonoBehaviour {

    private PhotonView m_photonView = null;
    private Renderer m_render = null;

    private readonly Color[] MATERIAL_COLORS = new Color[]
    {
        Color.white, Color.red, Color.green, Color.blue, Color.green,
    };

    void Awake()
    {
        m_photonView = GetComponent<PhotonView>();
        m_render = GetComponent<Renderer>();
    }

    void Update()
    {
        int ownerID = m_photonView.ownerId;
        m_render.material.color = MATERIAL_COLORS[ownerID];
    }
}
