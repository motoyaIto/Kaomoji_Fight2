using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTest : MonoBehaviour {
 //   #region
 //   // Use this for initialization
 //   void Start () {
 //       // Photonに接続する(引数でゲームのバージョンを指定できる)
 //       PhotonNetwork.ConnectUsingSettings(null);
	//}


 //   // Auto-Join-Llobbyにチェックがついていると呼ばれないはず
 //   void OnConnectedToMaster()
 //   {
 //       Debug.Log("呼ばれちゃいけないんだけどなぁ・・・（´・ω・｀）");
 //   }


 //   // ロビーに入ると呼ばれる
 //   private void OnJoinedLobby()
 //   {
 //       Debug.Log("ロビーに入りました");

 //       // ルームに入室する
 //       PhotonNetwork.JoinRandomRoom();
 //   }

 //   // ルームに入室すると呼ばれる
	//private void OnJoinedRoom()
 //   {
 //       Debug.Log("ルームへ入室しました");
 //   }


 //   // サーバーに到達できないと呼ばれる
 //   void OnFailedToConnectToPhoton(DisconnectCause cause)
 //   {
 //       Debug.Log("接続に失敗しとるやないかい！＼(゜ロ＼)(／ロ゜)／");
 //   }


 //   // ルーム入室に失敗すると呼ばれる
 //   private void OnPhotonRandomJoinFailed()
 //   {
 //       Debug.Log("ルームの入室に失敗しました");

 //       // ルームがないと入室に失敗するため、その時は自分で作る
 //       // 引数でルーム名を指定できる
 //       PhotonNetwork.CreateRoom("MyRoomName");
 //   }
 //   #endregion

    private string m_resourcePath = "prefab/Test/Cube";
    [SerializeField]
    private float m_randomCircle = 4.0f;

    private const string ROOM_NAME = "RoomA";
    private bool m_fg = false;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(null);
    }

    void OnJoinedLobby()
    {
        Debug.Log("ロビー入室");
        PhotonNetwork.JoinOrCreateRoom(ROOM_NAME, new RoomOptions(), TypedLobby.Default);
    }

    public void SpawnObject()
    {
        PhotonNetwork.InstantiateSceneObject(m_resourcePath, GetRandomPosition(), Quaternion.identity, 0, null);
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 rand = Random.insideUnitCircle * m_randomCircle;
        return rand;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) && !m_fg)
        {
            SpawnObject();
            m_fg = true;
        }
    }
}
