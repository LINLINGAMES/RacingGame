using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    //前方のコライダーコール.
    [SerializeField] ColliderCallReceiver frontColliderCall = null;
    //後方のコライダーコール.
    [SerializeField] ColliderCallReceiver backColliderCall = null;

    void Start()
    {
        frontColliderCall.TriggerEnterEvent.AddListener(OnFrontTriggerEnter);
        backColliderCall.TriggerEnterEvent.AddListener(OnBackTriggerEnter);
    }

    // --------------------------------------------------------------------------
    /// <summary>
    /// 前方トリガーエンターコール.
    /// </summary>
    /// <param name="col"> 侵入してきたコライダー. </param>
    // --------------------------------------------------------------------------
    void OnFrontTriggerEnter(Collider col)
    {
        // 侵入したコライダーのゲームオブジェクトのタグがPlayer.
        if (col.gameObject.tag == "Player")
        {
            var player = col.gameObject.GetComponent<PlayerController>();
            player.OnFrontGateCall();
        }

        // 侵入したコライダーのゲームオブジェクトのタグがCPU.
        else if (col.gameObject.tag == "CPU")
        {
            var cpuObj = col.gameObject.transform.parent.parent.gameObject;
            var cpu = cpuObj.GetComponent<CPUController>();
            cpu?.OnFrontGateCall();
        }
    }

    // --------------------------------------------------------------------------
    /// <summary>
    /// 後方トリガーエンターコール.
    /// </summary>
    /// <param name="col"> 侵入してきたコライダー. </param>
    // --------------------------------------------------------------------------
    void OnBackTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            var player = col.gameObject.GetComponent<PlayerController>();
            player.OnBackGateCall();
        }

        // 侵入したコライダーのゲームオブジェクトのタグがCPU.
        else if (col.gameObject.tag == "CPU")
        {
            var cpuObj = col.gameObject.transform.parent.parent.gameObject;
            var cpu = cpuObj.GetComponent<CPUController>();
            cpu?.OnBackGateCall();
        }
    }
}
