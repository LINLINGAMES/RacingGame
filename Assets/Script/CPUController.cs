using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CPUController : MonoBehaviour
{
    // Cinemachine�p�X�Ǘ�.
    [SerializeField] CinemachineSmoothPath smoothPath = null;
    // Cinemachine�J�[�g.
    [SerializeField] CinemachineDollyCart dollyCart = null;
    // ���b�v�^�C���i�P���ɂ����鎞�ԁj.
    [SerializeField, Range(40f, 200f)] float lapTime = 40f;

    // ���݂̃X�e�C�g.
    public GameController.PlayState CurrentState = GameController.PlayState.None;

    // ���b�v��.
    public int LapCount = 0;
    // ���b�v�C�x���g.
    public UnityEvent LapEvent = new UnityEvent();
    // �S�[���C�x���g��`�N���X.
    public class GoalEventClass : UnityEvent<GameObject> { }
    // �S�[�����C�x���g,
    public GoalEventClass GoalEvent = new GoalEventClass();

    // �S�[���ɕK�v�Ȏ���.
    int goalLap = 0;
    // ���b�v�v���p�t���O.
    bool lapSwitch = false;

    // ���x.
    float velocity = 0;

    // �Ԃ̃g�����X�t�H�[��.
    [SerializeField] Transform carTransform = null;


    void Start()
    {
        velocity = smoothPath.PathLength / lapTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentState == GameController.PlayState.Play || CurrentState == GameController.PlayState.Finish)
        {
            var deltaDistance = velocity * Time.deltaTime;
            dollyCart.m_Position += deltaDistance;

            if (carTransform.localPosition != Vector3.zero || carTransform.localRotation != Quaternion.identity)
            {
                FixedCarPosition();
            }
        }
        else if (CurrentState == GameController.PlayState.Ready)
        {
            dollyCart.m_Position = 0;
        }
    }

    // --------------------------------------------------------------------
    /// <summary>
    /// �X�^�[�g���R�[��.
    /// </summary>
    /// <param name="goal"> �S�[���ɕK�v�Ȏ���. </param>
    // --------------------------------------------------------------------
    public void OnStart(int goal)
    {
        goalLap = goal;
    }

    // ------------------------------------------------------------
    /// <summary>
    /// �S�[��������.
    /// </summary>
    // ------------------------------------------------------------
    public void OnGoal()
    {
        if (CurrentState != GameController.PlayState.Play) return;

        LapCount = 0;
        Debug.Log("CPU Goal!!");
        CurrentState = GameController.PlayState.Finish;
        GoalEvent?.Invoke(gameObject);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// �O���Q�[�g�R�[��.
    /// </summary>
    // ------------------------------------------------------------
    public void OnFrontGateCall()
    {
        if (CurrentState != GameController.PlayState.Play) return;

        // �ʏ�̃Q�[�g�ʉ�.
        if (lapSwitch == true)
        {
            LapCount++;
            Debug.Log("CPU Lap " + LapCount);
            lapSwitch = false;

            if (LapCount > goalLap) OnGoal();
            else LapEvent?.Invoke();
        }
        // �t���Q�[�g�ʉ�.
        else
        {
            LapCount--;
            if (LapCount < 0) LapCount = 0;
            Debug.Log("CPU �t�� Lap " + LapCount);

            LapEvent?.Invoke();
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// ����Q�[�g�R�[��.
    /// </summary>
    // ------------------------------------------------------------
    public void OnBackGateCall()
    {
        if (CurrentState != GameController.PlayState.Play) return;

        if (lapSwitch == false)
        {
            lapSwitch = true;
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// �Ԃ̈ʒu���J�[�g�ɏ��X�ɍ��킹��.
    /// </summary>
    // ------------------------------------------------------------
    void FixedCarPosition()
    {
        carTransform.localPosition = Vector3.Lerp(carTransform.localPosition, Vector3.zero, Time.deltaTime * 1f);
        carTransform.localRotation = Quaternion.Lerp(carTransform.localRotation, Quaternion.identity, Time.deltaTime * 1f);
    }

    public void OnRetryButtonClicked()
    {
        carTransform.localPosition = new Vector3(0, 0, 0);
        carTransform.localRotation = Quaternion.Euler(0, 0, 0);
    }

}
