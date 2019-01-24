﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Controller
{
    First,
    Secon,
    Thired,
    Force
};

public class PhotonManager : Photon.MonoBehaviour {

    //  prefabのばしょ ------------------- //
    private string player_prefab = "prefab/Player";
    // ----------------------------------- //

    private string ROOM_NAME = "RoomA";

    private GameObject[] player_obj = null;
    private Material[] playerMaterial = null;
    private string[] playerName = null;
    private Color[] playerColor = null;
    private Sprite[] playerFace = null;
    private string[] stage = null;

    // デバッグ用
    private Text m_RoomLog = null;


    // カスタムプロパティを一時保存する
    private string text = "";

    void Awake()
    {
        // Photon接続
        PhotonNetwork.ConnectUsingSettings(null);
    }

    private void Start()
    {
        player_obj = new GameObject[4];
        playerMaterial = new Material[4];
        playerName = new string[4];
        playerColor = new Color[4];
        playerFace = new Sprite[4];
        stage = new string[4];

        m_RoomLog = GameObject.Find("Text").GetComponent<Text>();
        m_RoomLog.text = "";
    }

    void OnDestory()
    {
        // Photon切断
        PhotonNetwork.Disconnect();
    }

    void OnJoinedLobby()
    {
        /*ルームオプションの設定*/

        // RoomOptionsのインスタンスを生成
        RoomOptions roomOptions = new RoomOptions();

        // ルームに入室できる最大人数。0を代入すると上限なし。
        roomOptions.MaxPlayers = 4;

        // ルームへの入室を許可するか否か
        roomOptions.IsOpen = true;

        // ロビーのルーム一覧にこのルームが表示されるか否か
        roomOptions.IsVisible = true;

        /* ルームの作成 */
        /* ルームオプションを設定する場合、CreateRoomメソッドは引数3つのオーバーロードを使用します。 */

        // 第1引数はルーム名、第2引数はルームオプション、第3引数はロビーです。
        PhotonNetwork.JoinOrCreateRoom(ROOM_NAME, roomOptions, TypedLobby.Default);
    }

    // ロビー接続と同時に呼び出し
    void OnReceivedRoomListUpdate()
    {
        //ルーム一覧を取る
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        if (rooms.Length == 0)
        {
            Debug.Log("ルームが一つもありません");
        }
        else
        {
            //ルームが1件以上ある時ループでRoomInfo情報をログ出力
            for (int i = 0; i < rooms.Length; i++)
            {
                Debug.Log("RoomName:" + rooms[i].Name);
                Debug.Log("userName:" + rooms[i].CustomProperties["userName"]);
                Debug.Log("userId:" + rooms[i].CustomProperties["userId"]);
                Debug.Log("PlayerCount:" + PhotonNetwork.room.PlayerCount);
                //GameObject.Find("StatusText").GetComponent<Text>().text = rooms[i].Name;
            }
        }


    }

    // ルーム接続時の呼び出し
    void OnJoinedRoom()
    {
        if (PhotonNetwork.room.PlayerCount < PhotonNetwork.room.MaxPlayers)
        {
            SpawnPlayer();
        }

    }


    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("new Player");
    }

    //void OnGUI()
    //{
    //    // ルームにいる場合のみ
    //    if (PhotonNetwork.inRoom)
    //    {
    //        // ルームの状態を取得
    //        Room room = PhotonNetwork.room;
    //        if (room == null)
    //        {
    //            return;
    //        }
    //        // ルームのカスタムプロパティを取得.ルームのカスタムプロパティを取得し、Hashtableにセットしています。
    //        ExitGames.Client.Photon.Hashtable cp = room.CustomProperties;
    //        //Hashtableの要素の”CustomPropeties”をLabelで表示しています。
    //        GUILayout.Label((string)cp["CustomProperties"], GUILayout.Width(150));
    //        text = GUILayout.TextField(text, 100, GUILayout.Width(150));

    //        // カスタムプロパティを更新
    //        if (GUILayout.Button("更新"))
    //        {
    //            cp["CustomProperties"] = text;
    //            //ルームのカスタムプロパティにセットします。
    //            room.SetCustomProperties(cp);
    //        }
    //    }
    //}


    [PunRPC]
    // プレイヤーの生成
    private void SpawnPlayer()
    {
        Debug.Log(PhotonNetwork.room.PlayerCount);
        //自分を生成
        player_obj[PhotonNetwork.room.PlayerCount - 1] = PhotonNetwork.Instantiate(player_prefab, Vector3.zero, Quaternion.identity, 0) as GameObject;
        player_obj[PhotonNetwork.room.PlayerCount - 1].transform.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Material/P" + PhotonNetwork.room.PlayerCount + "Color");
        playerMaterial[PhotonNetwork.room.PlayerCount - 1] = Resources.Load<Material>("Material/P" + PhotonNetwork.room.PlayerCount + "Color");
        player_obj[PhotonNetwork.room.PlayerCount - 1].transform.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", NT_PlayerData.Instance.color);
        playerColor[PhotonNetwork.room.PlayerCount - 1] = NT_PlayerData.Instance.color;
        player_obj[PhotonNetwork.room.PlayerCount - 1].transform.GetComponent<SpriteRenderer>().sprite = NT_PlayerData.Instance.Face;
        playerFace[PhotonNetwork.room.PlayerCount - 1] = NT_PlayerData.Instance.Face;
        player_obj[PhotonNetwork.room.PlayerCount - 1].name = NT_PlayerData.Instance.name;
        playerName[PhotonNetwork.room.PlayerCount - 1] = NT_PlayerData.Instance.name;

        //自分以外の先に入っている人
        for (int i = 0; i < PhotonNetwork.room.PlayerCount; i++)
        {
            Debug.Log(playerColor[i] + " " + playerFace[i].name + " " + playerName[i]);

            player_obj[i].transform.GetComponent<SpriteRenderer>().material = playerMaterial[i];
            player_obj[i].transform.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", playerColor[i]);
            player_obj[i].transform.GetComponent<SpriteRenderer>().sprite = playerFace[i];
            player_obj[i].name = playerName[i];
        }
    }


    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // 送信データ
            stream.SendNext(player_obj[PhotonNetwork.room.PlayerCount].transform.GetComponent<SpriteRenderer>().material);
            stream.SendNext(player_obj[PhotonNetwork.room.PlayerCount].transform.GetComponent<SpriteRenderer>().sprite);
            stream.SendNext(player_obj[PhotonNetwork.room.PlayerCount].transform.GetComponent<SpriteRenderer>().color);
            stream.SendNext(player_obj[PhotonNetwork.room.PlayerCount].name);
        }
        else
        {
            // 受信データ
            player_obj[PhotonNetwork.room.PlayerCount].GetComponent<SpriteRenderer>().material = (Material)stream.ReceiveNext();
            player_obj[PhotonNetwork.room.PlayerCount].GetComponent<SpriteRenderer>().sprite = (Sprite)stream.ReceiveNext();
            player_obj[PhotonNetwork.room.PlayerCount].GetComponent<SpriteRenderer>().color = (Color)stream.ReceiveNext();
            player_obj[PhotonNetwork.room.PlayerCount].name = (string)stream.ReceiveNext();

        }
    }


    private void Update()
    {
        if (PhotonNetwork.room != null) m_RoomLog.text = "" + PhotonNetwork.room.PlayerCount;
        
    }
}
