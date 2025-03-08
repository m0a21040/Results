using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;
public class Arduino_1 : MonoBehaviour
{
    public string[] ipAddresses = { "192.168.137.86" };  // M5Atom��IP�A�h���X
    public int[] ports = { 9001 };                        // �e�f�o�C�X�̃|�[�g�ԍ�
    public string objectATag = "TagA";                   // �I�u�W�F�N�gA�̃^�O
    public string objectBTag = "TagB";                   // �I�u�W�F�N�gB�̃^�O
    private List<OSCClient> clients = new List<OSCClient>();
    private int currentPattern = 2;                      // ���݂̃p�^�[���i1�܂���2�j
    private HashSet<GameObject> reactedObjects = new HashSet<GameObject>(); // �����ς݃I�u�W�F�N�g�̃Z�b�g

    void Start()
    {
        // IP�A�h���X�ƃ|�[�g�Ɋ�Â���OSC�N���C�A���g���쐬���A���X�g�ɒǉ�
        for (int i = 0; i < ipAddresses.Length; i++)
        {
            OSCClient client = new OSCClient(System.Net.IPAddress.Parse(ipAddresses[i]), ports[i]);
            clients.Add(client);
        }

        Debug.Log("�f�t�H���g�̃p�^�[��: �p�^�[��1");
    }

    void Update()
    {
        // C�L�[���������ƂŃp�^�[����؂�ւ���
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentPattern = currentPattern == 1 ? 2 : 1;
            Debug.Log($"���݂̃p�^�[��: �p�^�[��{currentPattern}");
        }
    }

    // �I�u�W�F�N�g�����̃I�u�W�F�N�g�ɏՓ˂����Ƃ��̃C�x���g����
    void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;

        // ���łɔ����ς݂̃I�u�W�F�N�g�͖���
        if (reactedObjects.Contains(collidedObject)) return;

        string address = null;

        // TagA���ŗD��ŕ]��
        if (collidedObject.CompareTag(objectATag))
        {
            address = currentPattern == 1 ? "/audio1/play" : "/audio3/play";
            Debug.Log($"{objectATag}�ɏՓ˂��܂���: {address}���Đ�");
        }
        else if (collidedObject.CompareTag(objectBTag) && !reactedObjects.Contains(collidedObject))
        {
            address = currentPattern == 1 ? "/audio2/play" : "/audio4/play";
            Debug.Log($"{objectBTag}�ɏՓ˂��܂���: {address}���Đ�");
        }

        // �A�h���X���ݒ肳��Ă����OSC���b�Z�[�W�𑗐M
        if (!string.IsNullOrEmpty(address))
        {
            reactedObjects.Add(collidedObject); // �����ς݂Ƃ��ċL�^
            SendOSCMessage(address);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // �Փ˂��I�������I�u�W�F�N�g�𔽉��ς݃��X�g����폜
        reactedObjects.Remove(collision.gameObject);
    }

    // OSC���b�Z�[�W�𑗐M���郁�\�b�h
    void SendOSCMessage(string address)
    {
        try
        {
            OSCMessage message = new OSCMessage(address);

            // clients���X�g���̊eclient�ɑ΂��ă��b�Z�[�W�𑗐M
            foreach (OSCClient client in clients)
            {
                client.Send(message);
                Debug.Log($"���M: {address}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"���M�Ɏ��s���܂��� -> {ex.Message}");
        }
    }

    void OnDestroy()
    {
        foreach (OSCClient client in clients)
        {
            client.Close();
        }
    }
}