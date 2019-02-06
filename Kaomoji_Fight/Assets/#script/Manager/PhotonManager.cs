using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XboxCtrlrInput;

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

public enum EeventType : byte
{
    stageEvent = 1
}

public class PhotonManager : Photon.MonoBehaviour {

    //  prefabのばしょ ------------------- //
    private string player_prefab = "prefab/Player";
    // ----------------------------------- //

    private string ROOM_NAME = "RoomA";

    private GameObject player_obj;

    // カスタムプロパティを一時保存する
    private string text = "";

    private int NowPlayernum = 0;
    private int CreatedNowPlayernum = 0;

    private PlayerData[] playerdata = null;

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

    private float DownPos = 0.0f;
    private float RightPos = 0.0f;
    //HPバー//////////////////////////////////////////////////////////
    [SerializeField]
    private GameObject HPbers;
    private GameObject HPber;

    void Awake()
    {
        // Photon接続
        PhotonNetwork.ConnectUsingSettings(null);
    }

    private void Start()
    {
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

        CreateHPber();
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

        player_obj = PhotonNetwork.Instantiate(player_prefab, new Vector3(20, 30, 0), Quaternion.identity, 0);

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
        //後から入ってきたプレイヤーを登録する
        if (newPlayerFlag == true)
        {
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                if (newPlayerID == PhotonNetwork.playerList[i].ID)
                {
                    GameObject player = GameObject.Find(PhotonNetwork.playerList[i].NickName);
                    if (player != null)
                    {
                        this.CameraSet(player.transform);

                        PhotonView photonView = this.GetComponent<PhotonView>();
                        photonView.RPC("StageData", PhotonTargets.OthersBuffered, DownPos, RightPos);
                    }
                }
            }
        }

        if (player_obj != null)
        {
            //if (XCI.GetButtonDown(XboxButton.Start, XboxController.First))
            //{
            //    int rand = (int)UnityEngine.Random.Range(.0f, (float)PhotonNetwork.room.PlayerCount) + 1;

            //    // 選ばれたのは「綾鷹」でした
            //    string selectStage = GameObject.Find("P" + rand + "StageSelect").GetComponent<TextMeshProUGUI>().text;
            //    if (selectStage == "ランダム")
            //    {
            //        int srand = (int)UnityEngine.Random.Range(1.0f, 8.0f);
            //        selectStage = "stage" + srand;
            //    }

            //    //プレイヤーデータを渡す
            //    CreatePlayer_data();
            //    if (PhotonNetwork.room.PlayerCount > 2)
            //    {
            //        // シーンロード
            //        SceneManagerController.LoadScene();
            //        SceneManagerController.ChangeScene();
            //    }
            //    else
            //    {
            //        Debug.Log("２人以上ではないのでプレイすることが出来ません(´・ω・｀)");
            //    }

            //}

            //ステージ外に出たら
            if (player_obj.transform.position.y + 10 < DownPos)
            {
                player_obj.transform.position = new Vector3(RightPos / 2, 30);
                HPber.transform.GetComponent<Slider>().value -= HPber.transform.GetComponent<Slider>().maxValue / 10;

                PhotonView photonView = this.GetComponent<PhotonView>();
                photonView.RPC("StageOverData", PhotonTargets.OthersBuffered, HPber.transform.GetComponent<PhotonView>().ownerId, HPber.transform.GetComponent<Slider>().maxValue / 10);
            }

            ////プレイヤーが死んだら回復させて上からリスポンさせる
            //if (HPber.transform.GetComponent<Slider>().value < 0.1f)
            //{
            //    HPber.transform.GetComponent<Slider>().value = 100;
            //    player_obj.transform.position = new Vector3(RightPos / 2, 30);

            //    PhotonView photonView = this.GetComponent<PhotonView>();
            //    photonView.RPC("ResuscitationData", PhotonTargets.OthersBuffered, HPber.transform.GetComponent<PhotonView>().ownerId);
            //}

            //プレイヤーが死んだら切断する
            if (HPber.transform.GetComponent<Slider>().value < 0.1f || Input.GetKeyDown(KeyCode.Z))
            {
                PhotonNetwork.Disconnect();
                SceneManagerController.LoadScene();
                SceneManagerController.ChangeScene();
            }
        }
    }


    [PunRPC]
    public void StageData(float down, float right)
    {
        DownPos = down;
        RightPos = right;
    }

    [PunRPC]
    public void StageOverData(int ownerid, float damage)
    {
        for (int i = 0; i < HPbers.transform.childCount; i++)
        {
            GameObject hpber = HPbers.transform.GetChild(i).gameObject;
            if (hpber.transform.GetComponent<PhotonView>().ownerId == ownerid)
            {
                hpber.transform.GetComponent<Slider>().value -= damage;
            }
        }
    }

    [PunRPC]
    public void ResuscitationData(int ownerid)
    {
        for (int i = 0; i < HPbers.transform.childCount; i++)
        {
            GameObject hpber = HPbers.transform.GetChild(i).gameObject;
            if (hpber.transform.GetComponent<PhotonView>().ownerId == ownerid)
            {
                hpber.transform.GetComponent<Slider>().value = 100;
            }
        }
    }

    /// <summary>
    /// 文字攻撃
    /// </summary>
    /// <param name="damagePlayer">ダメージを受けたキャラ</param>
    /// <param name="weapon">ダメージを与えた武器</param>
    public void MoziAttack(GameObject damagePlayer, GameObject weapon)
    {
        Debug.Log(weapon.name);
        Debug.Log(damagePlayer.name);
        float DamageValue = weapon.GetComponent<NT_MoziBlocController>().DamageValue_Data;

        // ダメージ音
        //audio.volume = .3f;
        //audio.PlayOneShot(audioClip_hit);

        //ダメージを受けたプレイヤーのHPバーを減らす
        for (int i = 0; i < HPbers.transform.childCount; i++)
        {
            GameObject hpber = HPbers.transform.GetChild(i).gameObject;

            if (hpber.transform.GetComponent<PhotonView>().ownerId == damagePlayer.GetComponent<PhotonView>().ownerId)
            {
                hpber.transform.GetComponent<Slider>().value -= weapon.transform.GetComponent<NT_MoziBlocController>().DamageValue_Data;

                PhotonView photonView = this.GetComponent<PhotonView>();
                photonView.RPC("MoziAttackData", PhotonTargets.All, hpber.transform.GetComponent<PhotonView>().ownerId, DamageValue);
            }
        }
    }

    [PunRPC]
    private void MoziAttackData(int ownerId, float damageValue)
    {
        if (HPber.transform.GetComponent<PhotonView>().ownerId == ownerId)
        {
            HPber.transform.GetComponent<Slider>().value -= damageValue;
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
                    BlockController Block_cs = StageBlocks[nam].transform.GetComponent<BlockController>();
                    Block_cs.Mozi = true;
                }
               
               

            }
            //右側の最大値を取得する
            float right = (this.transform.position.x - 10) + StageBlock.transform.localScale.x / 2 + StageBlock.transform.localScale.x * StagexCount;

            if (right > RightPos)
            {
                RightPos = right;
            }

            //右に一文字ずらす
            StagexCount++;
        }
        else
        {
            //下の最大値を取得する
            DownPos = (this.transform.position.y + 10) + StageBlock.transform.localScale.y / 2 + StageBlock.transform.localScale.y * StageyCount;

            //一行下にずらす
            StageyCount--;
            //文字位置をスタートに戻す
            StagexCount = 0;
        }
    }

    //HPバー/////////////////////////////////////////////////////////////////////
    public void CreateHPber()
    {
        HPber = PhotonNetwork.Instantiate("prefab/UI/HPgage2", new Vector3(0, 0, 0), Quaternion.identity, 0);
        player_obj.transform.GetComponent<Player>().HPber_Data = HPber;
    }

    public void CreatedPlayer()
    {
        CreatedNowPlayernum++;
    }

    public void CreatePlayer_data()
    {
        playerdata = new PlayerData[CreatedNowPlayernum];

        for (int i = 0; i < CreatedNowPlayernum; i++)
        {
            Debug.Log(i + " / " + CreatedNowPlayernum);
            //playerdata[i] = new PlayerData(NT_PlayerData.Instance.name[i], NT_PlayerData.Instance.color[i], NT_PlayerData.Instance.Face[i], new Vector3(10 * (i + 1), 30, 0), XboxController.First, 100);
        }
    }
}

