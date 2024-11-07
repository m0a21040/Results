using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;


public class Arduino_Control : MonoBehaviour
{

    // �eM5Atom Lite��IP�A�h���X�ƑΉ�����|�[�g�ԍ�
    public string[] ipAddresses =
    {
         "192.168.137.186",  // �f�o�C�X1��IP
    };

    public int[] ports = { 8000 };  // �e�f�o�C�X�ɑΉ�����|�[�g�ԍ�

    private List<OSCClient> clients = new List<OSCClient>();

    // ����̃I�u�W�F�N�g���m�̏Փˎ��ɉ���炷���߂ɕK�v�ȃI�u�W�F�N�g�̎Q��
    public GameObject targetObject;  // �ڐG����̃I�u�W�F�N�g

    // Start is called before the first frame update
    void Start()
    {
        // IP�A�h���X�ƃ|�[�g�Ɋ�Â���OSC�N���C�A���g���쐬���A���X�g�ɒǉ�
        for (int i = 0; i < ipAddresses.Length; i++)
        {
            OSCClient client = new OSCClient(System.Net.IPAddress.Parse(ipAddresses[i]), ports[i]);
            clients.Add(client);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // �ڐG�����I�u�W�F�N�g���w�肳�ꂽtargetObject�ł���ꍇ�A����炷
        if (collision.gameObject == targetObject)
        {
            Debug.Log("�w�肳�ꂽ�I�u�W�F�N�g�ƐڐG���܂���: audio1���Đ�");
            SendOSCMessage("/audio1/play");
        }
    }

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
        // clients���X�g���̊eclient�ɑ΂��Đڑ������
        foreach (OSCClient client in clients)
        {
            client.Close();
        }
    }
}
