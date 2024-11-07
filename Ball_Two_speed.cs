using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Two_speed : MonoBehaviour
{
    public Rigidbody _rb;
    public int power;
    public int fastPower; // 速い移動のためのパワー
    Vector3 direction = new Vector3(-0.03f, 0.09f, 1.3f);
    Vector3 fastDirection = new Vector3(-0.04f, 0.004f, 1.9f); // より速い移動のためのVector
    private bool hasThrown = false;
    private Rigidbody rb;
    public float delayTime = 0.5f; // 遅延時間を0.5秒に設定

    private bool isMoving = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public Material transparentMaterial;  // 透明なマテリアル
    public Material visibleMaterial;      // 適用するマテリアル

    private bool isMaterialVisible = false; // マテリアルの状態管理用フラグ
    private bool isToggling = false;        // トグル中かどうか

    public AudioClip catchSound; // キャッチ音のオーディオクリップ
    private AudioSource audioSource; // AudioSourceコンポーネント


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Rigidbodyを初期化時に無効に設定
        rb.isKinematic = true;

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // 初期状態を透明にする
        SetMaterial(transparentMaterial);

        // AudioSourceコンポーネントを取得
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // コントローラーのボタンが押されたら移動コルーチンを開始
        if ((Input.GetMouseButtonDown(0) || OVRInput.GetDown(OVRInput.Button.One)) && !hasThrown)
        {
            StartCoroutine(StartMovementAfterDelay(false));
        }
        else if (OVRInput.GetDown(OVRInput.Button.Two) && !hasThrown)
        {
            StartCoroutine(StartMovementAfterDelay(true));
        }
        else if ((OVRInput.GetDown(OVRInput.Button.One) || OVRInput.GetDown(OVRInput.Button.Two)) && hasThrown)
        {
            ResetBall();
        }

        // メタクエストのAボタンまたはBボタンが押されたらマテリアルを切り替え
        if (OVRInput.GetDown(OVRInput.Button.One) || OVRInput.GetDown(OVRInput.Button.Two))
        {
            ToggleMaterial();
        }
    }

    // コルーチンで遅延後に移動を開始
    IEnumerator StartMovementAfterDelay(bool isFast)
    {
        yield return new WaitForSeconds(delayTime);

        rb.isKinematic = false;
        hasThrown = true;
        if (isFast)
        {
            _rb.AddForce(fastDirection * fastPower);
        }
        else
        {
            _rb.AddForce(direction * power);
        }
    }

    void ResetBall()
    {
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        hasThrown = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // ターゲットに衝突した場合
        if (collision.gameObject.CompareTag("Target"))
        {
            // ボールの速度をゼロに設定
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;

            // ターゲットに当たったときに左コントローラを振動させる
            StartCoroutine(VibrateController(OVRInput.Controller.LTouch, 0.5f, 1.0f)); // 0.5秒間最大強度で振動
        }

        // 左コントローラーに当たった場合
        if (collision.gameObject.CompareTag("LeftController"))
        {
            StopBall(); // ボールを止める
            float ballSpeed = rb.velocity.magnitude; // ボールの速度を取得
            float vibrationStrength = Mathf.Clamp(ballSpeed / 10f, 0, 1); // 振動の強さを速度に基づいて計算
            StartCoroutine(VibrateController(OVRInput.Controller.LTouch, 0.5f, vibrationStrength)); // 0.5秒間、計算した強度で振動

            // キャッチ音を再生
            PlayCatchSound();
        }
    }

    void StopBall()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void ToggleMaterial()
    {
        if (isToggling) return;

        if (isMaterialVisible)
        {
            // 見える状態から透明化する場合、即座に切り替え
            SetMaterial(transparentMaterial);
            isMaterialVisible = false;
        }
        else
        {
            // 透明から見える状態に戻す場合、0.7秒の遅延を設ける
            isToggling = true;
            StartCoroutine(ToggleMaterialCoroutine());
        }
    }

    IEnumerator ToggleMaterialCoroutine()
    {
        // 0.7秒待つ
        yield return new WaitForSeconds(0.7f);

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // マテリアルを切り替える
            if (!isMaterialVisible)
            {
                SetMaterial(visibleMaterial);
                isMaterialVisible = true;
            }
        }

        isToggling = false;
    }

    void SetMaterial(Material material)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }
    }

    IEnumerator VibrateController(OVRInput.Controller controller, float duration, float strength)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            OVRInput.SetControllerVibration(strength, strength, controller);
            elapsed += Time.deltaTime;
            yield return null;
        }
        OVRInput.SetControllerVibration(0, 0, controller); // 振動を停止
    }

    void PlayCatchSound()
    {
        if (catchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(catchSound);
        }
    }
}