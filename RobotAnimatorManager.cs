using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class RobotAnimatorManager : MonoBehaviour
{
    public Animator RobotAni;
    public RobotManager RM;

    [SerializeField]
    [Tooltip("巡回する地点の配列")]
    private Transform[] waypoints;

    // NavMeshAgentコンポーネントを入れる変数
    private NavMeshAgent navMeshAgent;
    // 現在の目的地
    private int currentWaypointIndex;

    void Start()
    {
        // navMeshAgent変数にNavMeshAgentコンポーネントを入れる
        navMeshAgent = GetComponent<NavMeshAgent>();
        // 最初の目的地を入れる
        navMeshAgent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        if(RM.isInitRotate) //ここに書き加えてください
        {
            //Debug.Log("aaa");

            // 目的地点までの距離(remainingDistance)が目的地の手前までの距離(stoppingDistance)以下になったら
            if (navMeshAgent.remainingDistance <= 0.5f)
            {
                // 目的地の番号を１更新（右辺を剰余演算子にすることで目的地をループさせれる）
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                // 目的地を次の場所に設定
                navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
            }
            //RobotAni.Play(); //アニメーション再生処理

        }
        else
        {
            //RobotAni.enabled = false; //アニメーション停止処理
        }
    }
}
