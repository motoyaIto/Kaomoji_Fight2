using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using TMPro;
using Constant;
using UnityEngine.UI;

//[RequireComponent(typeof(Contoroller2d))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : RaycastController
{
    PhotonView view;
    //状態異常
    public struct StatesAbnormality
    {
        public bool Stan;   //麻痺
        public bool Sleep;  //眠る
    }

    public struct UpStates
    {
        public bool Substitution;   //身代わり
        public bool Invincible;     //無敵
    }

    #region 変数群

    private bool newplayer = true;

    [SerializeField, Header("コントローラー番号")]
    private XboxController ControlerNamber = XboxController.First;//何番目のコントローラーを適用するか

    private Collider2D oldRayStage;     //rayが当たってたオブジェクト

    private Vector3 velocity;

    private StatesAbnormality ButState;     //バットステータス
    private UpStates statesUp;              //上昇ステータス

    private GameObject HPgageObj;    //HPゲージオブジェクト

    private float moveSpeed = 10f;          // 移動速度
    float Avoidance_time = .0f;             // 回避時間
    private float Invincible_time = 8.0f;   // クールタイム

    private float scroll = 10f;             // 幅
    private float maxflap = 800f;           // ジャンプの高さ（最大）
    private float minflap = 400f;           // ジャンプの高さ（最小）

    private float direction = 0;            // 方向


    private GameObject MoziObj;

    private GameObject CreateWeapon_Effect;//武器を作るときのエフェクト
    private bool EffectCorutine = false;    //生成中(true)生成前||後(fasle)
    private GameObject weapon;          //武器
    private bool weapon_use = false;    //武器を持っている(true)いない(false)
    private Weapon weapon_cs;           //武器のスクリプト
    private bool weaponMoveLock = false;

    private bool HaveMozi = false;      //文字を持っている(true)いない(false)
    private bool BackSpace = false;     //バックスペースを押している(true)いない(false)
    private bool Avoidance = false;     // 回避フラグ
    private bool jump = false;          // ジャンプ中か？
    private bool EnteFlag = false;      //あったたオブジェクトがあるか

    private bool isQuitting = false;        // エディタ実行終了時か？

    Contoroller2d controller;               // コントローラー
    bool controller_lock = false;           //コントローラーをロックする
    Rigidbody2D rig = null;
    [HideInInspector]
    public CollisionInfo collisions;
    private PlaySceneManager PSM;

    private new AudioSource audio;
    private new AudioSource audio2;
    private AudioClip pickUp_ac;            //拾う
    private AudioClip shot_ac;              // 投げる音
    private AudioClip walk_ac;              //歩く
    private AudioClip jump_ac;              // ジャンプ
    private AudioClip bomb_ac;
    private AudioClip avoidance_ac;         //回避
    private AudioClip delete_ac;            //削除
    private AudioClip create_ac;            //生成音
    private AudioClip change_ac;            //文字変換

    private PhotonView photonView_cs = null;

    private TextMeshProUGUI tmpu;   //選択したステージ名を表示するための場所

    #endregion


    private void Awake()
    {
        CreateWeapon_Effect = Resources.Load<GameObject>("prefab/Effect/CreateWeapon_effect");

        audio = this.GetComponent<AudioSource>();
        audio2 = this.GetComponent<AudioSource>();
        pickUp_ac = (AudioClip)Resources.Load("Sound/SE/Move/pickUp");      //拾う音
        shot_ac = (AudioClip)Resources.Load("Sound/SE/Attack/shoot");   //投げる音
        walk_ac = (AudioClip)Resources.Load("Sound/SE/Move/walk");          //歩く音
        jump_ac = (AudioClip)Resources.Load("Sound/SE/Jump/jump");           //ジャンプ音  
        bomb_ac = Resources.Load<AudioClip>("Sound/SE/Deth/ded2");
        avoidance_ac = (AudioClip)Resources.Load("Sound/SE/Move/avoidance");    //回避音
        delete_ac = (AudioClip)Resources.Load("Sound/SE/Other/char_delete");    //消去音
        create_ac = (AudioClip)Resources.Load("Sound/SE/Other/create");         //生成音
        change_ac = (AudioClip)Resources.Load("Sound/SE/Other/char_switch");    //文字変換

        PhotonNetwork.OnEventCall += OnRaiseEvent;
    }

    new void Start()
    {
        //状態異常の初期化
        ButState.Stan = false;

        //上昇ステータス
        statesUp.Invincible = false;
        statesUp.Substitution = false;

        controller = GetComponent<Contoroller2d>();
        //PSM = GameObject.Find("PlaySceneManager").transform.GetComponent<PlaySceneManager>();
        rig = GetComponent<Rigidbody2D>();

        // プレイヤー同士の当たり判定をしない
        int P_layer = LayerMask.NameToLayer("Player");
        Physics2D.IgnoreLayerCollision(P_layer, P_layer);

        //HPゲージを取得する
        // HPgageObj = PSM.SetHPgage(CNConvert(ControlerNamber) + 1);

        // PhotonViewの取得
        photonView_cs = GetComponent<PhotonView>();

        this.transform.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Material/P" + photonView_cs.ownerId + "Color");

        // 持ち主でないのなら持ち主にデータを要請する
        if (!photonView_cs.isMine)
        {
            this.PleaseMyData();
            return;
        }

        //データを登録する
        this.name = NT_PlayerData.Instance.name;
        this.transform.GetComponent<SpriteRenderer>().sprite = NT_PlayerData.Instance.Face;
        this.transform.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", NT_PlayerData.Instance.color);


        // オプション
        RaiseEventOptions option = new RaiseEventOptions()
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.All,
        };
        // 選択したステージを表示
        PhotonNetwork.RaiseEvent((byte)EeventType.stageEvent, NT_PlayerData.Instance.SelectStage, true, option);
    }

    private void Reset()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 持ち主にデータを要求する
    /// </summary>
    public void PleaseMyData()
    {
        PhotonView photonView = this.GetComponent<PhotonView>();
        photonView.RPC("PushPlayerData", PhotonTargets.Others, photonView.ownerId);
    }

    public void PushPlayerData()
    {
        PhotonView photonView = this.GetComponent<PhotonView>();
        photonView.RPC("MethodRPC", PhotonTargets.OthersBuffered, photonView_cs.ownerId, NT_PlayerData.Instance.name, NT_PlayerData.Instance.Face.name, new Vector3(NT_PlayerData.Instance.color.r, NT_PlayerData.Instance.color.g, NT_PlayerData.Instance.color.b));
    }

    /// <summary>
    /// データを送信する
    /// </summary>
    /// <param name="ownerID">おーなーID</param>
    [PunRPC]
    public void PushPlayerData(int ownerID)
    {
        if (ownerID == this.transform.GetComponent<PhotonView>().ownerId)
        {
            PhotonView photonView = this.GetComponent<PhotonView>();
            photonView.RPC("MethodRPC", PhotonTargets.OthersBuffered, photonView_cs.ownerId, NT_PlayerData.Instance.name, NT_PlayerData.Instance.Face.name, new Vector3(NT_PlayerData.Instance.color.r, NT_PlayerData.Instance.color.g, NT_PlayerData.Instance.color.b));
        }
    }

    /// <summary>
    /// 持ち主からデータを受信する
    /// </summary>
    /// <param name="id">おーなーID</param>
    /// <param name="name">オーナー名</param>
    /// <param name="face">キャラクター</param>
    /// <param name="color">色</param>
    [PunRPC]
    public void MethodRPC(int id, string name, string face, Vector3 color)
    {
        if (id == this.transform.GetComponent<PhotonView>().ownerId)
        {
            this.name = name;
            this.transform.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("textures/use/Player/" + face);
            this.transform.GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", new Color(color.x, color.y, color.z, 1.0f));

            if (newplayer == true)
            {
                GameObject.Find("PhotonManager").GetComponent<PhotonManager>().CreatedPlayer();
                newplayer = false;
            }
        }
    }


    void Update()
    {
        // 自分以外の操作を受け付けない
        if (!photonView_cs.isMine) return;

        //キャラのy軸のdirection方向にscrollの力をかける
        rig.velocity = new Vector2(scroll * direction, rig.velocity.y);

        //コントローラーロック
        if (controller_lock == true)
        {
            return;
        }

        //状態異常処理
        //麻痺・眠り
        if (ButState.Stan == true || ButState.Sleep == true)
        {
            return;
        }
        // Controllerの左スティックのAxisを取得            
        Vector2 input = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, ControlerNamber), XCI.GetAxis(XboxAxis.LeftStickY, ControlerNamber));
        if (input.x > .0f)
        {
            //歩く音
            //  if (direction == 0) { this.PlaySound(audio2, walk_ac, 0.2f); }

            direction = 1f;
        }
        else if (input.x < .0f)
        {
            //歩く音
            // if (direction == 0) { this.PlaySound(audio2, walk_ac, 0.2f); }

            direction = -1f;
        }
        else
        {
            //歩く音を止める
            //audio2.Stop();
            direction = 0f;
        }

        if (HaveMozi)
        {
            //文字の持ち位置を持ち変える
            ItemPositionControll(MoziObj, input);
        }




        if (XCI.GetButtonDown(XboxButton.A, ControlerNamber) && !jump)
        {
            // 大ジャンプ
            audio.volume = .2f;
            audio.PlayOneShot(jump_ac);
            rig.AddForce(Vector2.up * maxflap);
            jump = true;

            //if (XCI.GetButtonUp(XboxButton.Y, ControlerNamber) && !jump)
            //{
            //    // 小ジャンプ・・・したかった・・・(´・ω・｀)
            //    rig.AddForce(Vector2.up * minflap);
            //    jump = true;
            //}
        }

        // 回避をしたい
        if (XCI.GetAxis(XboxAxis.RightTrigger, ControlerNamber) < 0.0f && !Avoidance)
        {
            // アニメーションに差し替え予定？
            if (!Avoidance)
            {
                audio.PlayOneShot(avoidance_ac);
                if (input.x < .0f)
                {
                    this.transform.position += new Vector3(-5f, 0f);
                }
                else if (input.x > .0f)
                {
                    this.transform.position += new Vector3(5f, 0f);
                }
                Avoidance = true;
            }
        }

        // 回避のクールタイム計測
        if (Avoidance && Avoidance_time <= Invincible_time)
        {
            Avoidance_time += .1f;
        }
        else
        {
            // 回避ができるようにする
            Avoidance = false;
            Avoidance_time = .0f;
        }

        //文字を持っている
        if (HaveMozi == true)
        {
            //文字の位置を調整
            NT_MoziBlocController WBController = MoziObj.gameObject.GetComponent<NT_MoziBlocController>();
            Vector3 direction = Vector3.zero;

            if (velocity.x < 0.0f)
            {
                direction = Vector3.left;
                WBController.SetPosition = new Vector3(this.transform.position.x - this.transform.localScale.x, this.transform.position.y + this.transform.localScale.y * 2.5f, 0.0f);
            }
            else if (velocity.x > 0.0f)
            {
                direction = Vector3.right;
                WBController.SetPosition = new Vector3(this.transform.position.x + this.transform.localScale.x + 0.5f, this.transform.position.y + this.transform.localScale.y * 2.5f, 0.0f);
            }

            //文字を投げる
            if (XCI.GetButtonDown(XboxButton.B, ControlerNamber))
            {
                audio.PlayOneShot(shot_ac);
                WBController.Attack(input);
            }

            // 文字を捨てる
            if (XCI.GetButton(XboxButton.X, ControlerNamber))
            {
                audio.PlayOneShot(delete_ac);
                HaveMozi = false;
                PhotonNetwork.Destroy(MoziObj);
            }
        }
        else
        {
            //所持している文字をすべて消去する
            if (XCI.GetButtonDown(XboxButton.X, ControlerNamber))
            {
                audio.PlayOneShot(delete_ac);
                //HPgageObj.transform.GetChild(4).GetComponent<GetMoziController>().AllDestroy();
            }
        }

        //一文字ずつ消す
        if (XCI.GetAxis(XboxAxis.LeftTrigger, ControlerNamber) > 0.8f && BackSpace == false)
        {

            HPgageObj.transform.GetChild(4).GetComponent<GetMoziController>().BackSpace();
            //if (XCI.GetAxis(XboxAxis.LeftTrigger, ControlerNamber) > 0.8f && BackSpace == false)
            //{
            //audio.PlayOneShot(delete_ac);
            //    HPgageObj.transform.GetChild(4).GetComponent<GetMoziController>().BackSpace();
            //    BackSpace = true;
            //}

            if (XCI.GetAxis(XboxAxis.LeftTrigger, ControlerNamber) < 0.2f)
            {
                BackSpace = false;
            }

            if (XCI.GetButtonDown(XboxButton.LeftBumper, ControlerNamber))
            {
                audio.PlayOneShot(change_ac);
                //取得文字として登録
                //HPgageObj.transform.GetChild(4).GetComponent<GetMoziController>().Semi_voicedPoint();
            }

            //武器に変換
            if (XCI.GetButtonDown(XboxButton.Y, ControlerNamber))
            {
                //if (weapon_use == false && EffectCorutine == false)
                //{
                //    weapon = SelectWeapon.CreateSelectWeapon(HPgageObj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text + HPgageObj.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text);


                //    武器化出来たら
                //    if (weapon != null)
                //    {
                //        GameObject EffectObj = null;
                //        foreach (Transform Child in this.transform)
                //        {
                //            if (Child.name == "Top")
                //            {
                //                エフェクトの発生
                //                EffectObj = Instantiate(CreateWeapon_Effect, Child.transform) as GameObject;
                //            }
                //        }
                //        StartCoroutine(this.CreateWeapn(EffectObj));
                //    }
                //}
            }

            //武器を持っているとき
            if (weapon_use == true)
            {
                //座標の調整
                //ItemPositionControll(weapon, input);


                ////武器を使う
                //if (XCI.GetButtonDown(XboxButton.B, ControlerNamber))
                //{
                //    weapon_cs.Attack();
                //}

                //// 武器を捨てる
                //else if (XCI.GetButtonDown(XboxButton.X, ControlerNamber))
                //{
                //    audio.PlayOneShot(delete_ac);
                //    weapon_use = false;
                //    PhotonNetwork.Destroy(weapon.gameObject);
                //    weapon = null;
                //}
            }
        }
        // Ｒａｙ
        this.RayController();
    }

    private IEnumerator CreateWeapn(GameObject effect)
    {
        audio.PlayOneShot(create_ac);
        EffectCorutine = true;
        yield return new WaitForSeconds(1.0f);
        EffectCorutine = false;
        Destroy(effect);
        //武器を生成
        weapon = Object.Instantiate(weapon, this.transform.position, Quaternion.identity);

        weapon.transform.parent = this.transform;

        weapon_cs = weapon.transform.GetComponent<Weapon>();
        weapon_cs.Owner_csData = this.transform.GetComponent<Player>();

        weapon_use = true;

        //持っている文字を破棄
        if (HaveMozi == true)
        {
            this.ChangeMozi_Data = false;
            Destroy(MoziObj);
            HaveMozi = false;
        }
        HPgageObj.transform.GetChild(4).GetComponent<GetMoziController>().AllDestroy();
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;
        public int faceDir;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }


    /// <summary>
    /// rayを飛ばしてアイテムを取得する
    /// </summary>
    private void RayController()
    {
        //rayの開始地点
        Vector3 ray_initial = new Vector3(this.transform.position.x, this.transform.position.y - this.transform.localScale.y, this.transform.position.x);

        //rayを生成
        Ray2D ray = new Ray2D(ray_initial, Vector2.down);
        //rayを可視化する
        Debug.DrawRay(ray.origin, ray.direction * 0.5f, Color.green, 0.01f);
        //rayに当たったものを取得する
        RaycastHit2D hitFoot = Physics2D.Raycast(ray.origin, Vector2.down, ray.direction.y * 0.5f);

        //ステージから離れたときに
        if (hitFoot.collider != null && hitFoot.collider.tag != "Stage" && oldRayStage != null)
        {
            //取得できる文字の色を元に戻す
            oldRayStage.transform.GetComponent<BlockController>().ChangeTextColor(new Color(0, 0, 0, 1));
        }

        //ゲットできるオブジェクトだったら
        if (hitFoot.collider != null && hitFoot.collider.tag == "Stage")
        {
            //前のオブジェクトが登録されていなかったら
            if (oldRayStage == null)
            {
                oldRayStage = hitFoot.collider;
            }
            //今の足元にある床と前足元にあった床が違う時
            if (hitFoot.collider.transform.position != oldRayStage.transform.position)
            {
                //取得できる文字の色を元に戻す
                oldRayStage.transform.GetComponent<BlockController>().ChangeTextColor(new Color(0, 0, 0, 1));

                oldRayStage = hitFoot.collider;
            }

            //取得できるオブジェクトを自分と同じ色に変更
            if (hitFoot.collider.transform.GetComponent<BlockController>().Mozi == true)
            {
                hitFoot.collider.transform.GetComponent<BlockController>().ChangeTextColor(this.transform.GetComponent<Renderer>().material.GetColor("_EmissionColor"));
            }
        }

        // 文字をゲットするかも
        if (XCI.GetButtonDown(XboxButton.B, ControlerNamber) && weapon_use == false && hitFoot.transform.tag == "Stage")
        {
            this.GetMozi(hitFoot, this.transform.position);
        }
    }

    /// <summary>
    ///  文字を獲得する
    /// </summary>
    /// <param name="hitFoot">足元にあった文字</param>
    /// <param name="directionX">右か左か</param>
    private void GetMozi(RaycastHit2D hitFoot, Vector2 pos)
    {
        GameObject block = hitFoot.collider.gameObject;
        BlockController block_cs = block.GetComponent<BlockController>();

        //文字を持っていなかったら
        if (HaveMozi == false && block_cs.Mozi == true && controller_lock == false)
        {
            //拾う音
            this.PlaySound(audio, pickUp_ac, .2f);

            //床を文字として取得
            MoziObj = PhotonNetwork.Instantiate("prefab/Stage/MoziBlock", this.transform.position, Quaternion.identity, 0);
            MoziObj.transform.SetParent(this.transform);
            MoziObj.name = "MoziBlock(" + block.transform.GetChild(0).GetComponent<TextMeshPro>().text + ")";
            MoziObj.transform.GetChild(0).GetComponent<TextMeshPro>().text = block.transform.GetChild(0).GetComponent<TextMeshPro>().text;

            //オーナー登録
            MoziObj.GetComponent<NT_MoziBlocController>().OwnerName_Data = this.name;
            MoziObj.GetComponent<NT_MoziBlocController>().Ownercs_Data = this.transform.GetComponent<Player>();
            ////床から切り抜く
            block.GetComponent<BlockController>().ChangeMozi();

            //MoziObj.GetComponent<BoxCollider2D>().enabled = false;

            HaveMozi = true;

            ////取得文字として登録
            ////HPgageObj.transform.GetChild(4).GetComponent<GetMoziController>().SetTextMozi(block.transform.GetChild(0).GetComponent<TextMeshPro>().text);

            ////プレイヤーの移動する向きに合わせて位置を調整
            this.ItemPositionControll(MoziObj, pos);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ジャンプ制限
        if (collision.gameObject.CompareTag("Stage"))
        {
            EnteFlag = true;
            jump = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (EnteFlag == false)
        {
            jump = true;
        }

        EnteFlag = false;
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDisable()
    {
        if (isQuitting) return;
        //PSM.death_player[CNConvert(ControlerNamber)] = false;
    }

    // Controllerの番号をint型で取得
    private int CNConvert(XboxController controlerNum)
    {
        switch (controlerNum)
        {
            case XboxController.First:
                return 0;
            case XboxController.Second:
                return 1;
            case XboxController.Third:
                return 2;
            case XboxController.Fourth:
                return 3;
            default:
                break;
        }
        return 4;
    }

    public bool ChangeMozi_Data
    {
        set
        {
            HaveMozi = value;
        }
    }


    /// <summary>
    /// XBXcontrollerの番号を取得
    /// </summary>
    public XboxController GetControllerNamber
    {
        get
        {
            return ControlerNamber;
        }
        set
        {
            ControlerNamber = value;
        }
    }

    public float Directtion_Data
    {
        set
        {
            direction = value;
        }
    }

    public void ItemPositionControll(GameObject Item, Vector2 vec2)
    {
        if ((HaveMozi == true || weapon_use == true) && weaponMoveLock == false)
        {
            foreach (Transform child in this.transform)
            {
                if (vec2.x > .0f && vec2.y > .0f && child.name == "TopRight")       //右上
                {
                    Item.transform.position = child.transform.position;
                }
                else if (vec2.x < .0f && vec2.y > .0f && child.name == "TopLeft")   //左下
                {
                    Item.transform.position = child.transform.position;
                }
                else if (vec2.x == .0f && vec2.y == .0f && child.name == "Top")     //移動していない
                {
                    Item.transform.position = child.transform.position;
                }
                else if (vec2.x > .0f && vec2.y < .0f && child.name == "DownRight") //右下
                {
                    Item.transform.position = child.transform.position;
                }
                else if (vec2.x < .0f && vec2.y < .0f && child.name == "DownLeft")  //左下
                {
                    Item.transform.position = child.transform.position;
                }
                else if (vec2.x == .0f && vec2.y < .0f && child.name == "Down")     //下
                {
                    Item.transform.position = child.transform.position;
                }
            }
        }
    }

    /// <summary>
    /// 音を鳴らす
    /// </summary>
    /// <param name="clip">音</param>
    /// <param name="volume">ボリューム</param>
    private void PlaySound(AudioSource audiosource, AudioClip clip, float volume)
    {
        audiosource.volume = volume;
        audiosource.PlayOneShot(clip);
        //audiosource.volume = 1.0f;
    }

    private void OnDestroy()
    {
        //死ぬ直前にステージに接していたら
        if (oldRayStage != null)
        {
            //取得できる文字の色を元に戻す
            oldRayStage.transform.GetComponent<BlockController>().ChangeTextColor(new Color(0, 0, 0));
        }
    }

    private void OnRaiseEvent(byte eventcode, object content, int senderid)
    {
        tmpu = GameObject.Find("P" + senderid + "StageSelect").GetComponent<TextMeshProUGUI>();
        string eventMessage = null;
        var eventtype = (EeventType)eventcode;

        switch (eventtype)
        {
            case EeventType.stageEvent:
                eventMessage = (string)content;
                break;
            default:
                break;
        }

        if (!string.IsNullOrEmpty(eventMessage))
        {
            Debug.Log(tmpu.text = (string)content);
        }
    }

    public int PlayerNumber_data
    {
        get
        {
            return CNConvert(ControlerNamber);
        }
    }

    public bool ControllerLock_Data
    {
        set
        {
            controller_lock = value;
        }
    }

    public bool Jump_data
    {
        get
        {
            return jump;
        }
    }

    public bool Stan_Data
    {
        set
        {
            ButState.Stan = value;
        }
    }

    public bool Sleep_Data
    {
        set
        {
            ButState.Sleep = value;
        }
    }

    public bool Substitution_Data
    {
        get
        {
            return statesUp.Substitution;
        }
        set
        {
            statesUp.Substitution = value;
        }
    }

    public bool Invincible_Data
    {
        get
        {
            return statesUp.Invincible;
        }
        set
        {
            statesUp.Invincible = value;
        }
    }

    public bool Weapon_useData
    {
        set
        {
            weapon_use = value;
        }
    }

    public string PlayerName_Data
    {
        get
        {
            return this.name;
        }
    }

    public bool WeaponMoveLockData
    {
        set
        {
            weaponMoveLock = false;
        }
    }
}
