using System;
using System.IO.Ports;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;


public class Arduino_USB_1 : MonoBehaviour
{
    public string portName = "COM3"; // M5Atomの接続ポート（環境に合わせて変更）
    public int baudRate = 115200;    // 通信速度
    private SerialPort serialPort;
    private int currentPattern = 2;  // 1または2のパターン（切り替え可能）
    private HashSet<GameObject> reactedObjects = new HashSet<GameObject>(); // 反応済みオブジェクトリスト

    void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
            serialPort.ReadTimeout = 50; // タイムアウト設定
            Debug.Log("シリアルポート接続成功: " + portName);
        }
        catch (Exception e)
        {
            Debug.LogError("シリアルポート接続失敗: " + e.Message);
        }
    }

    void Update()
    {
        // Cキーでパターン切り替え
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentPattern = currentPattern == 1 ? 2 : 1;
            Debug.Log($"現在のパターン: パターン{currentPattern}");
        }

        // Mキーを押すと TagA と同じ信号を送信
        if (Input.GetKeyDown(KeyCode.M))
        {
            string message = currentPattern == 1 ? "A1" : "A3";
            SendSerialMessage(message);
            Debug.Log($"Mキーが押されたので {message} を送信（TagA と同じ動作）");
        }

        // Nキーを押すと TagB と同じ信号を送信
        if (Input.GetKeyDown(KeyCode.N))
        {
            string message = currentPattern == 1 ? "A2" : "A4";
            SendSerialMessage(message);
            Debug.Log($"Nキーが押されたので {message} を送信（TagB と同じ動作）");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;

        // すでに反応済みのオブジェクトは無視
        if (reactedObjects.Contains(collidedObject)) return;

        string message = null;

        // TagA（優先的に処理）
        if (collidedObject.CompareTag("TagA"))
        {
            message = currentPattern == 1 ? "A1" : "A3";
            Debug.Log($"{collidedObject.name} に衝突しました: {message} を送信");
        }
        else if (collidedObject.CompareTag("TagB"))
        {
            message = currentPattern == 1 ? "A2" : "A4";
            Debug.Log($"{collidedObject.name} に衝突しました: {message} を送信");
        }

        if (!string.IsNullOrEmpty(message))
        {
            reactedObjects.Add(collidedObject); // 反応済みとして記録
            SendSerialMessage(message);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // 衝突が終了したオブジェクトを反応済みリストから削除
        reactedObjects.Remove(collision.gameObject);
    }

    void SendSerialMessage(string message)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.WriteLine(message);
            Debug.Log($"送信: {message}");
        }
        else
        {
            Debug.LogError("シリアルポートが開いていません！");
        }
    }

    void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("シリアルポートを閉じました");
        }
    }
}