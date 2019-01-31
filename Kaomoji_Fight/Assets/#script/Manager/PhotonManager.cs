using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using Cinemachine;
using System;
using System.Text;
using TMPro;

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

    private GameObject player_obj;

    // デバッグ用
    private Text m_RoomLog = null;


    // カスタムプロパティを一時保存する
    private string text = "";

    [SerializeField]
    private GameObject[] players;

    //カメラ//////////////////////////////////////////////////////////
    private CinemachineTargetGroup TargetGroup;
    private bool newPlayerFlag = false;
    private int newPlayerID = 0;
    //ステージ//////////////////////////////////////////////////////////
    //文字になる文字群
    private string[] MOZI =
    {
        "あ", "い", "う", "え", "お",
        "か", "き", "く", "け", "こ",
        "さ", "し", "す", "せ", "そ",
        "た", "ち", "つ", "て", "と",
        "な", "に", "ぬ", "ね", "の",
        "は", "ひ", "ふ", "へ", "ほ",
        "ま", "み", "む", "め", "も",
        "や",       "ゆ",       "よ",
        "ら", "り", "る", "れ", "ろ",
        "わ",       "を",       "ん",

        "が", "ぎ", "ぐ", "げ", "ご",
        "ざ", "じ", "ず", "ぜ", "ぞ",
        "だ", "ぢ", "づ", "で", "ど",
        "ば", "び", "ぶ", "べ", "ぼ",

        "ぱ", "ぴ", "ぷ", "ぺ", "ぽ",

        "ぁ", "ぃ", "ぅ", "ぇ", "ぉ",
        "ゃ", "ゅ", "ょ", "っ", "ー",
    };
    [SerializeField]
    private string StageName;
    private string StageText;
    private GameObject StageBlock;  //ステージ
    private Material Mozi_mate;   //文字material
    private GameObject[] StageBlocks = null;//ブロックの配列

    private int StageTextName = 0;  //テキストの文字数
    private int StagexCount = 0;    //x座標の移動
    private int StageyCount = 0;    //y座標の移動

    void Awake()
    {
        // Photon接続
        PhotonNetwork.ConnectUsingSettings(null);
    }

    private void Start()
    {
        m_RoomLog = GameObject.Find("Text").GetComponent<Text>();
        m_RoomLog.text = "";

        //カメラ////////////////////////////////////////////////////////////////
        //カメラにターゲットするプレイヤーの数を設定
        TargetGroup = this.GetComponent<CinemachineTargetGroup>();
        //TargetGroup.m_Targets = new CinemachineTargetGroup.Target[PlayData.Instance.playerNum];

        //ステージ////////////////////////////////////////////////////////////////
        //テキスト一覧の取得
        StageText = System.IO.File.ReadAllText("Assets/Resources/Texts/" + StageName + ".txt", Encoding.GetEncoding("Shift_JIS"));

        StageTextName = StageText.Length;

        //文字数分の配列
        StageBlocks = new GameObject[StageTextName];
        //文字を表示するボックスをResourcesから読み込む
        StageBlock = (GameObject)Resources.Load("prefab/Stage/StageBlock");
        Mozi_mate = Resources.Load<Material>("Material/StageBlock_Mozi");
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
                //Debug.Log("RoomName:" + rooms[i].Name);
                //Debug.Log("userName:" + rooms[i].CustomProperties["userName"]);
                //Debug.Log("userId:" + rooms[i].CustomProperties["userId"]);
                //Debug.Log("PlayerCount:" + PhotonNetwork.room.PlayerCount);
                //GameObject.Find("StatusText").GetComponent<Text>().text = rooms[i].Name;
            }
        }
    }

    // ルーム接続時の呼び出し
    void OnJoinedRoom()
    {
        Debug.Log("ルーム入室");
        Debug.Log(PhotonNetwork.room.Name);

        if (PhotonNetwork.room.PlayerCount < PhotonNetwork.room.MaxPlayers)
        {
            //プレイヤーを生成
            SpawnPlayer();

            //床を生成(マスターのみ)
            if (PhotonNetwork.isMasterClient == true)
            {
                for (int i = 0; i < StageTextName; i++)
                {
                    string mozi = StageText.Substring(i, 1);

                    CreateStageBlock(i, mozi);
                }
            }
            //今存在するプレイヤーをカメラにアタッチする
            //if(PhotonNetwork.playerList.Length)
            StartCoroutine(this.DelayMethod(0.5f, () =>
            {
                for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
                {
                    if (PhotonNetwork.playerList[i].NickName != NT_PlayerData.Instance.name)
                    {
                        this.CameraSet(GameObject.Find(PhotonNetwork.playerList[i].NickName).transform);
                    }
                }
            }));
            
        }
        
    }


    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        newPlayerFlag = true;

        Debug.Log("new Player");
        //ステージの同期(マスターのみ)
        if (PhotonNetwork.isMasterClient == true)
        {
            StartCoroutine(this.DelayMethod(0.1f, () =>
            {
                //一文字ずつ設定する
                for (int i = 0; i < StageTextName; i++)
                {
                    if (StageBlocks[i] != null) StageBlocks[i].transform.GetComponent<BlockController>().NewConectPlayer();
                }
            }));
        }

        //カメラにアタッチするプレイヤーを設定
        newPlayerID = newPlayer.ID;//おーなーID       
    }

 
    [PunRPC]
    // プレイヤーの生成
    private void SpawnPlayer()
    {
        int PlayerNum = PhotonNetwork.room.PlayerCount - 1;

        player_obj = PhotonNetwork.Instantiate(player_prefab, new Vector3(10, 0, 0), Quaternion.identity, 0);

        //プレイヤー名をphotonに登録
        PhotonNetwork.playerName = NT_PlayerData.Instance.name;
        //カメラに自分を設定
        this.CameraSet(player_obj.transform);       
    }

    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="waitTime">遅延時間</param>
    /// <param name="action">実行する処理</param>
    /// <returns></returns>
    protected IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }


    private void Update()
    {
        if (PhotonNetwork.room != null) m_RoomLog.text = "" + PhotonNetwork.room.PlayerCount;
        
        if(newPlayerFlag == true)
        {
            for(int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                if (newPlayerID == PhotonNetwork.playerList[i].ID)
                {
                    GameObject player = GameObject.Find(PhotonNetwork.playerList[i].NickName);
                    if (player != null)
                    {
                        this.CameraSet(player.transform);
                    }
                }
            }
        }
    }

    //カメラ/////////////////////////////////////////////////////////////////////
    private void CameraSet(Transform player)
    {
        //カメラのターゲットに設定
        for(int i = 0; i < TargetGroup.m_Targets.Length; i++)
        {
            //すでに登録されていたら
            //if(TargetGroup.m_Targets[i].target != null && TargetGroup.m_Targets[i].target.name == player.gameObject.name)
            //{
            //    return;
            //}

            //登録されていないところに登録
            if(TargetGroup.m_Targets[i].target == null)
            {
                TargetGroup.m_Targets[i].target = player;
                TargetGroup.m_Targets[i].weight = 1;
                TargetGroup.m_Targets[i].radius = 1;

                newPlayerFlag = false;

                return;
            }

        }
    }

    //ステージ/////////////////////////////////////////////////////////////////////
    private void CreateStageBlock(int nam, string mozi)
    {
        //改行が入っていないとき
        if (mozi != "\r" && mozi != "\n")
        {
            //スペースが入っていないとき
            if (mozi != " " && mozi != "　")
            {
                //新しく作るオブジェクトの座標
                Vector3 pos = new Vector3(
                       (this.transform.position.x-10) + StageBlock.transform.localScale.x / 2 + StageBlock.transform.localScale.x * StagexCount,
                       (this.transform.position.y+10) + StageBlock.transform.localScale.y / 2 + StageBlock.transform.localScale.y * StageyCount,
                        0.0f);

                //オブジェクトを生成する
                StageBlocks[nam] = PhotonNetwork.InstantiateSceneObject("prefab/Stage/StageBlock", pos, Quaternion.identity, 0, null);
                
                //ボックスの下のテキストを取得する
                GameObject textdata = StageBlocks[nam].transform.Find("Text").gameObject;
                //テキストに文字を書き込む
                textdata.GetComponent<TextMeshPro>().text = mozi;
                StageBlocks[nam].name = "StageBlock" + "(" + mozi + ")";
                // RectTransformを追加
                StageBlocks[nam].AddComponent<RectTransform>();
                //weaponだったら
                if (Array.IndexOf(MOZI, mozi) >= 0)
                {
                    //文字用スクリプトをセットする
                    GameObject weapon = StageBlocks[nam];
                    weapon.AddComponent<MoziBlocController>().enabled = false;
                    StageBlocks[nam] = weapon;

                    //文字用マテリアルに変更
                    Material StageBlock_MoziMateral = Mozi_mate;
                    StageBlocks[nam].GetComponent<Renderer>().material = StageBlock_MoziMateral;


                    //文字フラグを立てる
                    BlockController Block_cs = StageBlock.transform.GetComponent<BlockController>();
                    Block_cs.Mozi = true;
                }
               
               

            }
            //右に一文字ずらす
            StagexCount++;
        }
        else
        {
            //一行下にずらす
            StageyCount--;
            //文字位置をスタートに戻す
            StagexCount = 0;
        }
    }
}

