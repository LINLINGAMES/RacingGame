using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // -------------------------------------------------------
    /// <summary>
    /// �Q�[���X�e�[�g.
    /// </summary>
    // -------------------------------------------------------
    public enum PlayState
    {
        None,
        Ready,
        Play,
        Finish,
    }

    // ���݂̃X�e�[�g.
    public PlayState CurrentState = PlayState.None;

    //! �J�E���g�_�E���X�^�[�g�^�C��.
    [SerializeField] int countStartTime = 3;

    //! �J�E���g�_�E���e�L�X�g.
    [SerializeField] Text countdownText = null;
    //! �^�C�}�[�e�L�X�g.
    [SerializeField] Text timerText = null;
    // �J�E���g�_�E���̌��ݒl.
    float currentCountDown = 0;
    // �Q�[���o�ߎ��Ԍ��ݒl.
    float timer = 0;
    //�v���C���[.
    [SerializeField] PlayerController player = null;
    // ���b�v�e�L�X�g.
    [SerializeField] Text lapText = null;
    // ���g���CUI.
    [SerializeField] GameObject retryUI = null;
    //�@�X�^�[�gUI
    [SerializeField] GameObject startUI = null;
    // CPU���X�g.
    [SerializeField] List<CPUController> cpuList = new List<CPUController>();
    // �S�[�����X�g.
    List<GameObject> goalList = new List<GameObject>();

    // �S�[���C�x���g��`�N���X.
    public class GoalEventClass : UnityEvent<GameObject> { }
    // �S�[�����C�x���g,
    // public UnityEvent GoalEvent = new UnityEvent(); 
    public GoalEventClass GoalEvent = new GoalEventClass();
 
    public void OnClickButtonStart()
    {
        CountDownStart();
    }
    void Start()
    {
        player.LapEvent.AddListener(OnLap);
        player.GoalEvent.AddListener(OnGoal);
        foreach (var cpu in cpuList) cpu.GoalEvent.AddListener(OnGoal);
        timerText.text = "Time : 000.0 s";
        lapText.text = "Lap : 1/" + player.GoalLap;
        retryUI.SetActive(false);
    }

    void Update()
    {
        timerText.text = "Time : 000.0 s";
        // �X�e�[�g��Ready�̂Ƃ�.
        if (CurrentState == PlayState.Ready)
        {
            // ���Ԃ������Ă���.
            currentCountDown -= Time.deltaTime;

            int intNum = 0;
            // �J�E���g�_�E����.
            if (currentCountDown <= (float)countStartTime && currentCountDown > 0)
            {
                // int(����)��.
                intNum = (int)Mathf.Ceil(currentCountDown);
                countdownText.text = intNum.ToString();

                //�J�E���g�_�E��SE�J�n
                SoundManager.Instance.PlaySE(SESoundData.SE.CountDown);
            }
            else if (currentCountDown <= 0)
            {
                // �J�n.
                StartPlay();
                intNum = 0;
                countdownText.text = "Start!!";

                //BGM�J�n
                SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Title);

                // Start�\�����������ď���.
                StartCoroutine(WaitErase());
            }
        }
        // �X�e�[�g��Play�̂Ƃ�.
        else if (CurrentState == PlayState.Play)
        {
            timer += Time.deltaTime;
            timerText.text = "Time : " + timer.ToString("000.0") + " s";
        }

        else if(CurrentState == PlayState.Finish)
        {
              timerText.text = "Time : " + timer.ToString("000.0") + " s";
        }

        else
        {
            timer = 0;
            timerText.text = "Time : 000.0 s";
        }
    }

    // -------------------------------------------------------
    /// <summary>
    /// �J�E���g�_�E���X�^�[�g.
    /// </summary>
    // -------------------------------------------------------
    void CountDownStart()
    {
        startUI.SetActive(false);

        currentCountDown = (float)countStartTime;
        SetPlayState(PlayState.Ready);
        countdownText.gameObject.SetActive(true);
        player.OnStart();
        foreach (var cpu in cpuList) cpu.OnStart(player.GoalLap);
    }

    // -------------------------------------------------------
    /// <summary>
    /// �Q�[���X�^�[�g.
    /// </summary>
    // -------------------------------------------------------
    void StartPlay()
    {
        Debug.Log("Start!!!");
        SetPlayState(PlayState.Play);
    }

    // -------------------------------------------------------
    /// <summary>
    /// �����҂��Ă���Start�\��������.
    /// </summary>
    // -------------------------------------------------------
    IEnumerator WaitErase()
    {
        yield return new WaitForSeconds(2f);
        countdownText.gameObject.SetActive(false);
    }

    // -------------------------------------------------------
    /// <summary>
    /// ���݂̃X�e�[�g�̐ݒ�.
    /// </summary>
    /// <param name="state"> �ݒ肷��X�e�[�g. </param>
    // -------------------------------------------------------
    void SetPlayState(PlayState state)
    {
        CurrentState = state;
        player.CurrentState = state;
        foreach (var cpu in cpuList) cpu.CurrentState = state;
    }

    // -------------------------------------------------------
    /// <summary>
    /// ���b�v���ω�������.
    /// </summary>
    // -------------------------------------------------------
    void OnLap()
    {
        var current = player.LapCount;
        var goalLap = player.GoalLap;

        lapText.text = "Lap : " + current + "/" + goalLap;
    }

    // -------------------------------------------------------
    /// <summary>
    /// �S�[��������.
    /// </summary>
    // -------------------------------------------------------
    void OnGoal(GameObject go)
    {
        var player = go.GetComponent<PlayerController>();
        var cpu = go.GetComponent<CPUController>();

        // �v���C���[.
        if (player != null)
        {
            var playerNumber = goalList.Count + 1;

            CurrentState = PlayState.Finish;
            countdownText.text = "Goal!!  " + playerNumber + "��";
            countdownText.gameObject.SetActive(true);
            retryUI.SetActive(true);

            //BGM�X�g�b�v
            SoundManager.Instance.StopBGM(BGMSoundData.BGM.Title);
            //�S�[��BGM
            SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Goal);
        }
        // CPU.
        else if (cpu != null)
        {
            goalList.Add(go);
        }
    }

    // -------------------------------------------------------
    /// <summary>
    /// ���g���C�{�^���N���b�N�R�[���o�b�N.
    /// </summary>
    // -------------------------------------------------------
    public void OnRetryButtonClicked()
    {
        retryUI.SetActive(false);
        timerText.text = "Time : 000.0 s";
        lapText.text = "Lap : 1/" + player.GoalLap;
        goalList.Clear();

        player.OnRetry();
       
        CountDownStart();

        //�S�[��BGM�X�g�b�v
        SoundManager.Instance.StopBGM(BGMSoundData.BGM.Goal);
    }
}
