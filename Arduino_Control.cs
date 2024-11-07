using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;


public class Arduino_Control : MonoBehaviour
{

    // 各M5Atom LiteのIPアドレスと対応するポート番号
    public string[] ipAddresses =
    {
         "192.168.137.186",  // デバイス1のIP
    };

    public int[] ports = { 8000 };  // 各デバイスに対応するポート番号

    private List<OSCClient> clients = new List<OSCClient>();

    // 特定のオブジェクト同士の衝突時に音を鳴らすために必要なオブジェクトの参照
    public GameObject targetObject;  // 接触相手のオブジェクト

    // Start is called before the first frame update
    void Start()
    {
        // IPアドレスとポートに基づいてOSCクライアントを作成し、リストに追加
        for (int i = 0; i < ipAddresses.Length; i++)
        {
            OSCClient client = new OSCClient(System.Net.IPAddress.Parse(ipAddresses[i]), ports[i]);
            clients.Add(client);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 接触したオブジェクトが指定されたtargetObjectである場合、音を鳴らす
        if (collision.gameObject == targetObject)
        {
            Debug.Log("指定されたオブジェクトと接触しました: audio1を再生");
            SendOSCMessage("/audio1/play");
        }
    }

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
        // clientsリスト内の各clientに対して接続を閉じる
        foreach (OSCClient client in clients)
        {
            client.Close();
        }
    }
}
