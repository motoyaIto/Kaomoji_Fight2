using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // Photonに接続する(引数でゲームのバージョンを指定できる)
        PhotonNetwork.ConnectUsingSettings(null);
	}

    // ロビーに入ると呼ばれる
    private void OnJoinedLobby()
    {
        Debug.Log("ロビーに入りました");

        // ルームに入室する
        PhotonNetwork.JoinRandomRoom();
    }

    // ルームに入室すると呼ばれる
	private void OnJoinedRoom()
    {
        Debug.Log("ルームへ入室しました");
    }

    // ルーム入室に失敗すると呼ばれる
    private void OnPhotonRandomJoinFailed()
    {
        Debug.Log("ルームの入室に失敗しました");

        // ルームがないと入室に失敗するため、その時は自分で作る
        // 引数でルーム名を指定できる
        PhotonNetwork.CreateRoom("MyRoomName");
    }

}
