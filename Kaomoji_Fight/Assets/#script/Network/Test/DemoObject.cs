using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEventType : byte
{
    hello = 1,
}

public class DemoObject : MonoBehaviour {

    private float m_speed = 3.0f;
    private float m_colorSpeed = 1.0f;

    private PhotonView m_photonView = null;
    private Renderer m_render = null;
    private Color m_color = Color.white;

    private readonly Color[] MATERIAL_COLORS = new Color[]
    {
        Color.white, Color.red, Color.green, Color.blue, Color.green,
    };

    private string m_eventLog = null;

    private bool onemore_flag = false;
    // Prefabのパスの用意 ☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=
    private string effect_path = "prefab/Effect/Knife_InstantDeath_Effect";
    //☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=☆-=

    private void Awake()
    {
        m_photonView = GetComponent<PhotonView>();
        m_render = GetComponent<Renderer>();

        PhotonNetwork.OnEventCall += OnRaiseEvent;
    }

    private void Start()
    {
        int ownerID = m_photonView.ownerId;
        m_color = MATERIAL_COLORS[ownerID];
    }

    private void Update()
    {
        // 持ち主でないのなら制御させない
        if (!m_photonView.isMine)
        {
            return;
        }

        if (Input.GetKey(KeyCode.M))
        {
            onemore_flag = false;
        }
        if (Input.GetKey(KeyCode.Mouse3) && !onemore_flag)
        {
            m_photonView.RPC("ShowEffect", PhotonTargets.AllBuffered);
            onemore_flag = true;
        }


        Vector3 pos = transform.position;

        pos.x += Input.GetAxis("Horizontal") * m_speed * Time.deltaTime;
        pos.y += Input.GetAxis("Vertical") * m_speed * Time.deltaTime;

        transform.position = pos;

        // マテリアルの青の成分のみを時間経過によって変化させる.
        m_color.b += m_colorSpeed * Time.deltaTime;
        m_color.b = Mathf.Repeat(m_color.b, 1.0f);
        m_render.material.color = m_color;
    }

    void OnPhotonSerializeView(PhotonStream i_stream, PhotonMessageInfo i_info)
    {
        if (i_stream.isWriting)
        {
            //データの送信
            i_stream.SendNext(m_color.r);
            i_stream.SendNext(m_color.g);
            i_stream.SendNext(m_color.b);
            i_stream.SendNext(m_color.a);
        }
        else
        {
            //データの受信
            float r = (float)i_stream.ReceiveNext();
            float g = (float)i_stream.ReceiveNext();
            float b = (float)i_stream.ReceiveNext();
            float a = (float)i_stream.ReceiveNext();
            m_color = new Color(r, g, b, a);
            m_render.material.color = m_color;
        }
    }

    [PunRPC]
    private void ShowEffect()
    {
        // エフェクトを生成.
        // 適当な時間が経過したら消すように設定.
        GameObject effect = GameObject.Instantiate(Resources.Load(effect_path), transform.position, Quaternion.identity) as GameObject;
        GameObject.Destroy(effect, 3.0f);
    }


    private void RaiseEventTest()
    {

    }

    private void OnRaiseEvent(byte i_eventcode, object i_content, int i_senderid)
    {
        string eventMessage = null;

        var eventType = (EEventType)i_eventcode;
        switch (eventType)
        {
            case EEventType.hello:
                eventMessage = string.Format("[{0}] {1} - Sender({2})", eventType, (string)i_content, i_senderid);
                break;
            default:
                break;
        }

        if (!string.IsNullOrEmpty(eventMessage))
        {
           // m_eventLog.text += eventMessage + System.Environment.NewLine;
        }
    }
}
