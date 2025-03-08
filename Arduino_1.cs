using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;
public class Arduino_1 : MonoBehaviour
{
    public string[] ipAddresses = { "192.168.137.86" };  // M5AtomのIPアドレス
    public int[] ports = { 9001 };                        // 各デバイスのポート番号
    public string objectATag = "TagA";                   // オブジェクトAのタグ
    public string objectBTag = "TagB";                   // オブジェクトBのタグ
    private List<OSCClient> clients = new List<OSCClient>();
    private int currentPattern = 2;                      // 現在のパターン（1または2）
    private HashSet<GameObject> reactedObjects = new HashSet<GameObject>(); // 反応済みオブジェクトのセット

    void Start()
    {
        // IPアドレスとポートに基づいてOSCクライアントを作成し、リストに追加
        for (int i = 0; i < ipAddresses.Length; i++)
        {
            OSCClient client = new OSCClient(System.Net.IPAddress.Parse(ipAddresses[i]), ports[i]);
            clients.Add(client);
        }

        Debug.Log("デフォルトのパターン: パターン1");
    }

    void Update()
    {
        // Cキーを押すことでパターンを切り替える
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentPattern = currentPattern == 1 ? 2 : 1;
            Debug.Log($"現在のパターン: パターン{currentPattern}");
        }
    }

    // オブジェクトが他のオブジェクトに衝突したときのイベント処理
    void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;

        // すでに反応済みのオブジェクトは無視
        if (reactedObjects.Contains(collidedObject)) return;

        string address = null;

        // TagAを最優先で評価
        if (collidedObject.CompareTag(objectATag))
        {
            address = currentPattern == 1 ? "/audio1/play" : "/audio3/play";
            Debug.Log($"{objectATag}に衝突しました: {address}を再生");
        }
        else if (collidedObject.CompareTag(objectBTag) && !reactedObjects.Contains(collidedObject))
        {
            address = currentPattern == 1 ? "/audio2/play" : "/audio4/play";
            Debug.Log($"{objectBTag}に衝突しました: {address}を再生");
        }

        // アドレスが設定されていればOSCメッセージを送信
        if (!string.IsNullOrEmpty(address))
        {
            reactedObjects.Add(collidedObject); // 反応済みとして記録
            SendOSCMessage(address);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // 衝突が終了したオブジェクトを反応済みリストから削除
        reactedObjects.Remove(collision.gameObject);
    }

    // OSCメッセージを送信するメソッド
    void SendOSCMessage(string address)
    {
        try
        {
            OSCMessage message = new OSCMessage(address);

            // clientsリスト内の各clientに対してメッセージを送信
            foreach (OSCClient client in clients)
            {
                client.Send(message);
                Debug.Log($"送信: {address}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"送信に失敗しました -> {ex.Message}");
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