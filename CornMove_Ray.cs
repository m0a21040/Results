using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CornMove_Ray : MonoBehaviour
{
    [SerializeField]
    Transform mainCamera;

    public float speed;//キャラクターの移動速度

    //[SerializeField]
    //private PlayBGM PlayBGM;
    PlayerManager waa;
    //public Transform startTfm;

    //ゲージによるスピードアップ倍率の変更はここ
    //2倍する場合は「2」を入れてください
    float x = 1.25f;

    public RectTransform gauge;//ゲージUIのRectTransform
    float speedUp;//スピードアップ倍率
    float dash = 1f;//ダッシュ倍率

    private Rigidbody rb;
    public float jumpPower;//ジャンプ力
    public float groundCheckOffset = 0.6f;//地面判定のYオフセット値
    public float groundCheckDistance = 0.2f;//地面判定の距離

    //接地判定
    public RaycastHit hit;
    public bool isGround = true;

    //public bool isChaseArea = false;
    //public bool isDisappearArea = false;

    //AudioSource audiosource;

    public Animator move_anim;

    private Vector2 inputMove;//入力の移動値
    private InputAction moveAction, dashAction, jumpAction;//入力アクション

    //ポーズ中またはUI表示中かどうかのフラグ
    public UIcontrol2 ui2;
    public UITrigger uitrigger;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        waa = GameObject.Find("AudioManager").GetComponent<PlayerManager>();

        //isChaseArea = false;
        //isDisappearArea = false;
        //isDisappearArea = false;

        //audiosource = GetComponent<AudioSource>();

        var playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        dashAction = playerInput.actions["Dash"];
        jumpAction = playerInput.actions["Jump"];

    }

    #region OnMove:移動の感知とベクトル取得
    public void OnMove(InputAction.CallbackContext movementValue)
    {
        //ポーズ中またはUI表示中は処理しない
        if (ui2 != null)
        {
            if (ui2.Flag == true)
            {
                inputMove = Vector2.zero;
            }
        }
        if (uitrigger != null)
        {
            if (uitrigger.Flag == true)
            {
                inputMove = Vector2.zero;
            }
        }

        inputMove = movementValue.ReadValue<Vector2>();
    }
    #endregion

    #region OnJump:ジャンプ処理
    public void OnJump(InputAction.CallbackContext jumpValue)
    {
        //ボタンが押されていない時または地面にいないときは処理しない
        if (!jumpValue.performed || !isGround) return;
        //ポーズ中またはUI表示中は処理しない
        if (ui2 != null)
        {
            if (ui2.Flag == true) return;
        }
        if (uitrigger != null)
        {
            if (uitrigger.Flag == true) return;
        }

        rb.velocity = Vector3.up * jumpPower;
        isGround = false;
        waa.PlayJump();
    }
    #endregion

    #region OnDash:ダッシュをしてるかどうか
    public void OnDash(InputAction.CallbackContext dashValue)
    {
        //ポーズ中またはUI表示中は処理しない
        if (ui2 != null)
        {
            if (ui2.Flag == true) return;
        }
        if (uitrigger != null)
        {
            if (uitrigger.Flag == true) return;
        }

        var isDash = dashAction.IsPressed();

        //ダッシュボタンが押された場合ダッシュ倍率が2倍、それ以外は1倍
        dash = isDash ? 2f : 1f;
    }
    #endregion

    private void FixedUpdate()
    {
        //ポーズ中またはUI表示中は処理しない
        if (ui2 != null)
        {
            if (ui2.Flag == true) return;
        }
        if (uitrigger != null)
        {
            if (uitrigger.Flag == true) return;
        }

        //接地判定
        if (Physics.SphereCast(this.gameObject.transform.position + new Vector3(0, groundCheckOffset, 0), this.gameObject.transform.lossyScale.x * 0.5f, Vector3.down, out hit, groundCheckDistance))
        {
            isGround = true;
            //Debug.Log("着地！");
        }
        else
        {
            isGround = false;
            //Debug.Log("地面についてない");
        }

        // SphereCastの可視化
        Debug.DrawRay(this.transform.position + new Vector3(0, groundCheckOffset, 0), Vector3.down * groundCheckDistance, Color.red);
    }

    // Update is called once per frame
    void Update()
    {
        //ポーズ中またはUI表示中は処理しない
        if (ui2 != null)
        {
            if (ui2.Flag == true) return;
        }
        if (uitrigger != null)
        {
            if (uitrigger.Flag == true) return;
        }

        //ゲージによるスピードアップ率計算
        //ゲージが半分以下の時はスピードアップしない
        if (gauge.localPosition.x < 400)
        {
            speedUp = 1;
        }
        else
        {
            //スピード倍率の変更の仕方
            //変数のところにある x を変更したい倍率に変えてください
            speedUp = 1 + ((gauge.localPosition.x - 400) * (x - 1) / 400);
        }

        //移動処理
        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        // 方向キーの入力値とカメラの向きから、移動方向を決定
        Vector3 moveForward = cameraForward * inputMove.y + Camera.main.transform.right * inputMove.x;
        //移動方向にスピードをかける
        transform.localPosition += moveForward * speed * dash * speedUp * Time.deltaTime;

        // 入力値がある場合は歩くアニメーションを再生し、ない場合は停止する
        move_anim.SetBool("walking", inputMove != Vector2.zero);
    }

    #region 使わなくなったスクリプト
    /*地面に触れたら再度ジャンプ可能にする
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("敵に当たった");
            //敵に当たった時スタートに戻る
            transform.position = startTfm.position;
        }

    }*/

    /*
    //ChaseAreaまたはDisappearAreaに入ったかどうか
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "ChaseArea")
        {
            Debug.Log("ChaseAreaに突入");
            isChaseArea = true;
        }
        if (other.gameObject.name == "DisappearArea")
        {
            Debug.Log("DisappearAreaに突入");
            isDisappearArea = true;
        }
    }

    //ChaseAreaまたはDisappearAreaを出たかどうか
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "ChaseArea")
        {
            Debug.Log("ChaseAreaを脱出");
            isChaseArea = false;
        }

        if (other.gameObject.name == "DisappearArea")
        {
            Debug.Log("DisappearAreaを脱出");
            isDisappearArea = false;
        }
    }*/
    #endregion
}
