using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopTrap : MonoBehaviour
{
    /*このスクリプトはゲーム内ギミックのガムになります
      ガムに触れると2秒動けなくなり、その後通常よりも遅い速度
    　数秒後元の速度に戻る
    */

    private CornMove_Ray main;
    private Cornove2 slow;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("coon"))
        {
            // ポイント
            // コーンの動きを司るスクリプトをオフにする。
            main = collision.gameObject.GetComponent<CornMove_Ray>();
            slow = collision.gameObject.GetComponent<Cornove2>();
            main.enabled = false;
            slow.enabled = false;

            // ポイント
            // ２秒後にボールが動けるようにする。
            Invoke("Stopoff", 2.0f);
            Invoke("Slowoff", 10.0f);
        }
    }

    void Stopoff()
    {
        // ポイント
        // Ballの動きを司るスクリプトをオンにする。
        slow.enabled = true;
    }

    void Slowoff()
    {
        // ポイント
        // コーンの動きを司るスクリプトをオンにする。
        slow.enabled = false;
        main.enabled = true;
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Gum"))
        {

            speed = 0;
            // ポイント
            // ２秒後にコーンが動けるようにする。
            Invoke("Stopoff", 2.0f);
            Invoke("Slowoff", 10.0f);
        }
    }*/
}

/*
 *  void Stopoff()
    {

        speed = 1;
        // ポイント
        // コーンの動きを司るスクリプトをオンにする。
        
    }

    void Slowoff()
    {
        speed = 3;
        // ポイント
        //コーンの動きを司るスクリプトをオンにする。
        
    }
*/
