using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Two_speed : MonoBehaviour
{
    public Rigidbody _rb;
    public int power;
    public int fastPower; // �����ړ��̂��߂̃p���[
    Vector3 direction = new Vector3(-0.03f, 0.09f, 1.3f);
    Vector3 fastDirection = new Vector3(-0.04f, 0.004f, 1.9f); // ��葬���ړ��̂��߂�Vector
    private bool hasThrown = false;
    private Rigidbody rb;
    public float delayTime = 0.5f; // �x�����Ԃ�0.5�b�ɐݒ�

    private bool isMoving = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public Material transparentMaterial;  // �����ȃ}�e���A��
    public Material visibleMaterial;      // �K�p����}�e���A��

    private bool isMaterialVisible = false; // �}�e���A���̏�ԊǗ��p�t���O
    private bool isToggling = false;        // �g�O�������ǂ���

    public AudioClip catchSound; // �L���b�`���̃I�[�f�B�I�N���b�v
    private AudioSource audioSource; // AudioSource�R���|�[�l���g


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Rigidbody�����������ɖ����ɐݒ�
        rb.isKinematic = true;

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // ������Ԃ𓧖��ɂ���
        SetMaterial(transparentMaterial);

        // AudioSource�R���|�[�l���g���擾
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // �R���g���[���[�̃{�^���������ꂽ��ړ��R���[�`�����J�n
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

        // ���^�N�G�X�g��A�{�^���܂���B�{�^���������ꂽ��}�e���A����؂�ւ�
        if (OVRInput.GetDown(OVRInput.Button.One) || OVRInput.GetDown(OVRInput.Button.Two))
        {
            ToggleMaterial();
        }
    }

    // �R���[�`���Œx����Ɉړ����J�n
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
        // �^�[�Q�b�g�ɏՓ˂����ꍇ
        if (collision.gameObject.CompareTag("Target"))
        {
            // �{�[���̑��x���[���ɐݒ�
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;

            // �^�[�Q�b�g�ɓ��������Ƃ��ɍ��R���g���[����U��������
            StartCoroutine(VibrateController(OVRInput.Controller.LTouch, 0.5f, 1.0f)); // 0.5�b�ԍő勭�x�ŐU��
        }

        // ���R���g���[���[�ɓ��������ꍇ
        if (collision.gameObject.CompareTag("LeftController"))
        {
            StopBall(); // �{�[�����~�߂�
            float ballSpeed = rb.velocity.magnitude; // �{�[���̑��x���擾
            float vibrationStrength = Mathf.Clamp(ballSpeed / 10f, 0, 1); // �U���̋����𑬓x�Ɋ�Â��Čv�Z
            StartCoroutine(VibrateController(OVRInput.Controller.LTouch, 0.5f, vibrationStrength)); // 0.5�b�ԁA�v�Z�������x�ŐU��

            // �L���b�`�����Đ�
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
            // �������Ԃ��瓧��������ꍇ�A�����ɐ؂�ւ�
            SetMaterial(transparentMaterial);
            isMaterialVisible = false;
        }
        else
        {
            // �������猩�����Ԃɖ߂��ꍇ�A0.7�b�̒x����݂���
            isToggling = true;
            StartCoroutine(ToggleMaterialCoroutine());
        }
    }

    IEnumerator ToggleMaterialCoroutine()
    {
        // 0.7�b�҂�
        yield return new WaitForSeconds(0.7f);

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // �}�e���A����؂�ւ���
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
        OVRInput.SetControllerVibration(0, 0, controller); // �U�����~
    }

    void PlayCatchSound()
    {
        if (catchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(catchSound);
        }
    }
}