using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CPUController : MonoBehaviour
{
    // Cinemachineパス管理.
    [SerializeField] CinemachineSmoothPath smoothPath = null;
    // Cinemachineカート.
    [SerializeField] CinemachineDollyCart dollyCart = null;
    // ラップタイム（１周にかかる時間）.
    [SerializeField, Range(40f, 200f)] float lapTime = 40f;

    // 現在のステイト.
    public GameController.PlayState CurrentState = GameController.PlayState.None;

    // ラップ数.
    public int LapCount = 0;
    // ラップイベント.
    public UnityEvent LapEvent = new UnityEvent();
    // ゴールイベント定義クラス.
    public class GoalEventClass : UnityEvent<GameObject> { }
    // ゴール時イベント,
    public GoalEventClass GoalEvent = new GoalEventClass();

    // ゴールに必要な周回数.
    int goalLap = 0;
    // ラップ計測用フラグ.
    bool lapSwitch = false;

    // 速度.
    float velocity = 0;

    // 車のトランスフォーム.
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
    /// スタート時コール.
    /// </summary>
    /// <param name="goal"> ゴールに必要な周回数. </param>
    // --------------------------------------------------------------------
    public void OnStart(int goal)
    {
        goalLap = goal;
    }

    // ------------------------------------------------------------
    /// <summary>
    /// ゴール時処理.
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
    /// 前方ゲートコール.
    /// </summary>
    // ------------------------------------------------------------
    public void OnFrontGateCall()
    {
        if (CurrentState != GameController.PlayState.Play) return;

        // 通常のゲート通過.
        if (lapSwitch == true)
        {
            LapCount++;
            Debug.Log("CPU Lap " + LapCount);
            lapSwitch = false;

            if (LapCount > goalLap) OnGoal();
            else LapEvent?.Invoke();
        }
        // 逆走ゲート通過.
        else
        {
            LapCount--;
            if (LapCount < 0) LapCount = 0;
            Debug.Log("CPU 逆走 Lap " + LapCount);

            LapEvent?.Invoke();
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 後方ゲートコール.
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
    /// 車の位置をカートに徐々に合わせる.
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
