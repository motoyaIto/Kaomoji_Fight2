using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEventType : byte
{
    hello = 1,
    Timer = 2
};

public class NetworkConnect : MonoBehaviour {

    // ルームの名前
    private const string ROOM_NAME = "RoomA";

    private void Awake()
    {
        // PhotonServerに接続（引数にバージョンを入れる）
        PhotonNetwork.ConnectUsingSettings(null);
    }

    private void Start()
    {
        
    }

    private void OnJoinedLobby()
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


    // Room参加NG時
    private void OnPhotonJoinFailed()
    {
        // 名前なしRoom作成
        PhotonNetwork.CreateRoom(null);
    }


    // Room参加OK時
    private void OnJoinedRoom()
    {
        if(PhotonNetwork.countOfPlayers < PhotonNetwork.room.MaxPlayers)
        {
            // プレイヤーの生成
            return;
        }
    }

}
