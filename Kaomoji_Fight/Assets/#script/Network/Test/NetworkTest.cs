using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTest : MonoBehaviour {

    // 生成するPrefabの場所
    private string m_resourcePath = "prefab/Test/Cube";
    [SerializeField]
    private float m_randomCircle = 4.0f;

    private const string ROOM_NAME = "RoomA";

    // Test-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    private bool m_fg = false;
    GameObject obj;
    GameObject test;

    private string e_path = "prefab/Effect/Knife_InstantDeath_Effect";
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


    // オブジェクトの生成
    public void SpawnObject()
    {
        PhotonNetwork.Instantiate(m_resourcePath, GetRandomPosition(), Quaternion.identity, 0);
    }

    // Vector3でランダムにポジションを取得
    private Vector3 GetRandomPosition()
    {
        Vector3 rand = Random.insideUnitCircle * m_randomCircle;
        return rand;
    }

    private void Update()
    {
        // テストオブジェクトの生成
        if (Input.GetKey(KeyCode.Space) && !m_fg)
        {
            SpawnObject();
            m_fg = true;

            obj = GameObject.Find("Cube(Clone)");            
        }
        // もう一個生成したいときに使う
        if (Input.GetKey(KeyCode.M))
        {
            m_fg = false;
        }
        // 生成したオブジェクトを動かす
        if (m_fg)
        {
            Move(obj);
        }

        // RPC（リモートプロシージャコール）を使ってみよう！！
        if (Input.GetKey(KeyCode.Mouse3) && !m_fg)
        {
            m_fg = true;

            // RPCのテストなり（対象を選択して実行命令を飛ばすことができる）
            PhotonView pv = test.GetComponent<PhotonView>();
            int myNum = 777;
            if (pv.isMine)
            {
                // 実行するメソッド名, ターゲット,    引数
                pv.RPC("DebugNum", PhotonTargets.All, myNum);
                //pv.RPC("ShowEffect", PhotonTargets.All);
                // viewIDをセット
                pv.viewID = PhotonNetwork.AllocateViewID();
                // PhotonView の ObservedComponents リストを初期化
                pv.ObservedComponents = new List<Component>();
                // 位置の同期を有効にする
                //test.GetComponent<PhotonTransformView>().m_PositionModel.SynchronizeEnabled = true;
                // 回転の同期を有効にする
                //test.GetComponent<PhotonTransformView>().m_RotationModel.SynchronizeEnabled = true;

                // リストに追加して同期対象に加える
                //pv.ObservedComponents.Add(obj.GetComponent<PhotonTransformView>());
            }        
        }
    }

    // 上下左右に動かすぜぃ( ･´ｰ･｀)ドヤァ
    private void Move(GameObject obj)
    {
        Rigidbody rig = obj.GetComponent<Rigidbody>();

        if (Input.GetKey(KeyCode.UpArrow))
        {
            rig.AddForce(new Vector3(0, .02f, 0) * 5f, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rig.AddForce(new Vector3(0, -.02f, 0) * 5f, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rig.AddForce(new Vector3(-.02f, 0, 0) * 5f, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rig.AddForce(new Vector3(.02f, 0, 0) * 5f, ForceMode.Impulse);
        }
    }

    [PunRPC]
    private void DebugNum(int num)
    {
        Debug.Log(num);
    }

    [PunRPC]
    private void ShowEffect()
    {
        // エフェクトを生成.
        // 適当な時間が経過したら消すように設定.
        GameObject effect = GameObject.Instantiate(Resources.Load(e_path), transform.position, Quaternion.identity) as GameObject;
        GameObject.Destroy(effect, 3.0f);
    }

}
