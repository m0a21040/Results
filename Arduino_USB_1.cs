using System;
using System.IO.Ports;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;


public class Arduino_USB_1 : MonoBehaviour
{
    public string portName = "COM3"; // M5Atom�̐ڑ��|�[�g�i���ɍ��킹�ĕύX�j
    public int baudRate = 115200;    // �ʐM���x
    private SerialPort serialPort;
    private int currentPattern = 2;  // 1�܂���2�̃p�^�[���i�؂�ւ��\�j
    private HashSet<GameObject> reactedObjects = new HashSet<GameObject>(); // �����ς݃I�u�W�F�N�g���X�g

    void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
            serialPort.ReadTimeout = 50; // �^�C���A�E�g�ݒ�
            Debug.Log("�V���A���|�[�g�ڑ�����: " + portName);
        }
        catch (Exception e)
        {
            Debug.LogError("�V���A���|�[�g�ڑ����s: " + e.Message);
        }
    }

    void Update()
    {
        // C�L�[�Ńp�^�[���؂�ւ�
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentPattern = currentPattern == 1 ? 2 : 1;
            Debug.Log($"���݂̃p�^�[��: �p�^�[��{currentPattern}");
        }

        // M�L�[�������� TagA �Ɠ����M���𑗐M
        if (Input.GetKeyDown(KeyCode.M))
        {
            string message = currentPattern == 1 ? "A1" : "A3";
            SendSerialMessage(message);
            Debug.Log($"M�L�[�������ꂽ�̂� {message} �𑗐M�iTagA �Ɠ�������j");
        }

        // N�L�[�������� TagB �Ɠ����M���𑗐M
        if (Input.GetKeyDown(KeyCode.N))
        {
            string message = currentPattern == 1 ? "A2" : "A4";
            SendSerialMessage(message);
            Debug.Log($"N�L�[�������ꂽ�̂� {message} �𑗐M�iTagB �Ɠ�������j");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;

        // ���łɔ����ς݂̃I�u�W�F�N�g�͖���
        if (reactedObjects.Contains(collidedObject)) return;

        string message = null;

        // TagA�i�D��I�ɏ����j
        if (collidedObject.CompareTag("TagA"))
        {
            message = currentPattern == 1 ? "A1" : "A3";
            Debug.Log($"{collidedObject.name} �ɏՓ˂��܂���: {message} �𑗐M");
        }
        else if (collidedObject.CompareTag("TagB"))
        {
            message = currentPattern == 1 ? "A2" : "A4";
            Debug.Log($"{collidedObject.name} �ɏՓ˂��܂���: {message} �𑗐M");
        }

        if (!string.IsNullOrEmpty(message))
        {
            reactedObjects.Add(collidedObject); // �����ς݂Ƃ��ċL�^
            SendSerialMessage(message);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // �Փ˂��I�������I�u�W�F�N�g�𔽉��ς݃��X�g����폜
        reactedObjects.Remove(collision.gameObject);
    }

    void SendSerialMessage(string message)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.WriteLine(message);
            Debug.Log($"���M: {message}");
        }
        else
        {
            Debug.LogError("�V���A���|�[�g���J���Ă��܂���I");
        }
    }

    void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("�V���A���|�[�g����܂���");
        }
    }
}