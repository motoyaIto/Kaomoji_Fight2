using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : Photon.MonoBehaviour {

    // カスタムプロパティを一時保存する
    private string text = "";

    void Awake()
    {
        // Photon接続
        PhotonNetwork.ConnectUsingSettings(null);
    }

    void OnDestory()
    {
        // Photon切断
        PhotonNetwork.Disconnect();
    }

    void OnJoinedLobby()
    {
        // ルームオプションの作成
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 4;
        //ルーム作成時にパラメータとしてHashtableとstring配列をセットする必要があります。Hashtableにはカスタムプロパティの要素
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "CustomProperties", "カスタムプロパティ" } };
        //string配列にはロビーで利用するためにカスタムプロパティの名前を配列としてセットします。
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "CustomProperties" };
        // ルームの作成.ルームが作成されるとすぐにカスタムプロパティを利用することが可能です。
        PhotonNetwork.CreateRoom("CustomPropertiesRoom", roomOptions, null);
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
                //GameObject.Find("StatusText").GetComponent<Text>().text = rooms[i].Name;
            }
        }
    }

    // ルーム接続時の呼び出し
    void OnJoinedRoom()
    {

    }

    void OnGUI()
    {
        // ルームにいる場合のみ
        if (PhotonNetwork.inRoom)
        {
            // ルームの状態を取得
            Room room = PhotonNetwork.room;
            if (room == null)
            {
                return;
            }
            // ルームのカスタムプロパティを取得.ルームのカスタムプロパティを取得し、Hashtableにセットしています。
            ExitGames.Client.Photon.Hashtable cp = room.CustomProperties;
            //Hashtableの要素の”CustomPropeties”をLabelで表示しています。
            GUILayout.Label((string)cp["CustomProperties"], GUILayout.Width(150));
            text = GUILayout.TextField(text, 100, GUILayout.Width(150));

            // カスタムプロパティを更新
            if (GUILayout.Button("更新"))
            {
                cp["CustomProperties"] = text;
                //ルームのカスタムプロパティにセットします。
                room.SetCustomProperties(cp);
            }
        }
    }
}
