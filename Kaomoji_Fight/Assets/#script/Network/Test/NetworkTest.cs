using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkTest : MonoBehaviour {

    // 生成するPrefabの場所
    private string m_resourcePath = "prefab/Test/Cube";
    private string m_stagePath = "prefab/Stage/StageBlock";

    private string m_player = "prefab/Player";
    [SerializeField]
    private float m_randomCircle = 4.0f;

    private const string ROOM_NAME = "RoomA";

    private Text m_RoomLog = null;

    // Test-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    private bool m_fg = false;
    GameObject stage;
    GameObject test;
    // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    void Start()
    {
        // Photonに接続（引数にバージョンを入れる）
        PhotonNetwork.ConnectUsingSettings(null);

        test = GameObject.Find("NetworkTestObject");
    }

    void OnJoinedLobby()
    {
        Debug.Log("ロビー入室");

        m_RoomLog = GameObject.Find("nt_text").GetComponent<Text>();
        m_RoomLog.text = "";

        // ルームを生成する
        //PhotonNetwork.JoinOrCreateRoom(ROOM_NAME, new RoomOptions(), TypedLobby.Default);

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


    // OnJoinedLobbyが呼ばれると同時に呼ばれる
    private void OnReceivedRoomListUpdate()
    {

    }

    private void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {

    }


    // オブジェクトの生成（プレイヤーごとに所持する）
    public void SpawnObject()
    {
        PhotonNetwork.Instantiate(m_resourcePath, GetRandomPosition(), Quaternion.identity, 0);
        //PhotonNetwork.Instantiate(m_player, GetRandomPosition(), Quaternion.identity, 0);
    }

    // ルームに引っ付けるオブジェクトの生成（所持者がルームになるので誰が抜けても消えない）
    public void SpawnSceneObject()
    {
        //PhotonNetwork.InstantiateSceneObject(m_stagePath, Vector3.zero, Quaternion.identity, 0, null);
        PhotonNetwork.InstantiateSceneObject(m_stagePath, Vector3.zero, Quaternion.identity, 0, null);
    }

    // Vector3でランダムにポジションを取得
    private Vector3 GetRandomPosition()
    {
        Vector3 rand = Random.insideUnitCircle * m_randomCircle;
        return rand;
    }

    private void Update()
    {
        // テストオブジェクトの生成(各プレイヤー)
        if (Input.GetKey(KeyCode.Space) && !m_fg)
        {
            SpawnObject();
            m_fg = true;
        }

        // オブジェクトの生成(ルーム)
        if (Input.GetKey(KeyCode.V) && !m_fg)
        {
            SpawnSceneObject();
            m_fg = true;
        }

        // もう一個生成したいときに使う
        if (Input.GetKey(KeyCode.M))
        {
            m_fg = false;
        }
        
        if(PhotonNetwork.room != null)m_RoomLog.text = "" + PhotonNetwork.room.ToStringFull();
    }
}
