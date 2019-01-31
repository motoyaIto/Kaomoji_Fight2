using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HPberController : MonoBehaviour {

    RectTransform rectTrans;

    // Use this for initialization
    void Start () {

        // 持ち主でないのなら持ち主にデータを要請する
        if (!GetComponent<PhotonView>().isMine)
        {
            this.PleaseMyData();
            return;
        }

        GameObject canves = GameObject.Find("Wait_UI").transform.GetChild(0).gameObject;
        this.transform.SetParent(canves.transform);

        rectTrans = this.transform.GetComponent<RectTransform>();
        rectTrans.localPosition = new Vector3(rectTrans.rect.width * (PhotonNetwork.playerList.Length - 1), 0, 0);

        this.name = NT_PlayerData.Instance.name + "_HPber";
        this.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = NT_PlayerData.Instance.name;
        this.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = NT_PlayerData.Instance.color;

        
    }
	
    private void PleaseMyData()
    {
        PhotonView photonView = this.GetComponent<PhotonView>();
        photonView.RPC("PushHPData", PhotonTargets.OthersBuffered, photonView.ownerId);
    }

    [PunRPC]
    private void PushHPData(int id)
    {
        if (id == this.transform.GetComponent<PhotonView>().ownerId)
        {
            PhotonView photonView = this.GetComponent<PhotonView>();
            photonView.RPC("CatchHPData", PhotonTargets.OthersBuffered, this.transform.GetComponent<PhotonView>().ownerId, NT_PlayerData.Instance.name, NT_PlayerData.Instance.color.r, NT_PlayerData.Instance.color.g, NT_PlayerData.Instance.color.b, NT_PlayerData.Instance.color.a);
        }
    }

    [PunRPC]
    private void CatchHPData(int id, string name, float r, float g, float b, float a)
    {
        if (id == this.transform.GetComponent<PhotonView>().ownerId)
        {
            GameObject canves = GameObject.Find("Wait_UI").transform.GetChild(0).gameObject;
            this.transform.SetParent(canves.transform);

            this.transform.GetComponent<RectTransform>().localPosition = new Vector3(this.transform.GetComponent<RectTransform>().rect.width * (id - 1), 0, 0);

            this.name = name + "_HPber";
            this.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = name;
            this.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color(r, g, b, a);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
