using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    //�O���̃R���C�_�[�R�[��.
    [SerializeField] ColliderCallReceiver frontColliderCall = null;
    //����̃R���C�_�[�R�[��.
    [SerializeField] ColliderCallReceiver backColliderCall = null;

    void Start()
    {
        frontColliderCall.TriggerEnterEvent.AddListener(OnFrontTriggerEnter);
        backColliderCall.TriggerEnterEvent.AddListener(OnBackTriggerEnter);
    }

    // --------------------------------------------------------------------------
    /// <summary>
    /// �O���g���K�[�G���^�[�R�[��.
    /// </summary>
    /// <param name="col"> �N�����Ă����R���C�_�[. </param>
    // --------------------------------------------------------------------------
    void OnFrontTriggerEnter(Collider col)
    {
        // �N�������R���C�_�[�̃Q�[���I�u�W�F�N�g�̃^�O��Player.
        if (col.gameObject.tag == "Player")
        {
            var player = col.gameObject.GetComponent<PlayerController>();
            player.OnFrontGateCall();
        }

        // �N�������R���C�_�[�̃Q�[���I�u�W�F�N�g�̃^�O��CPU.
        else if (col.gameObject.tag == "CPU")
        {
            var cpuObj = col.gameObject.transform.parent.parent.gameObject;
            var cpu = cpuObj.GetComponent<CPUController>();
            cpu?.OnFrontGateCall();
        }
    }

    // --------------------------------------------------------------------------
    /// <summary>
    /// ����g���K�[�G���^�[�R�[��.
    /// </summary>
    /// <param name="col"> �N�����Ă����R���C�_�[. </param>
    // --------------------------------------------------------------------------
    void OnBackTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            var player = col.gameObject.GetComponent<PlayerController>();
            player.OnBackGateCall();
        }

        // �N�������R���C�_�[�̃Q�[���I�u�W�F�N�g�̃^�O��CPU.
        else if (col.gameObject.tag == "CPU")
        {
            var cpuObj = col.gameObject.transform.parent.parent.gameObject;
            var cpu = cpuObj.GetComponent<CPUController>();
            cpu?.OnBackGateCall();
        }
    }
}
