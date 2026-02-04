using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;

public class InGameView : MonoBehaviour
{
    [Header("항아리 물 수위")]
    [SerializeField] private Text _jarWaterLVText;
    [SerializeField] private Image _jarWaterImaget;
    [Header("깨진 항아리")]
    [SerializeField] private Text _destroyJarText;
    [Header("우물 물 정보")]
    [SerializeField] private Text _wellWaterLVText;
    [SerializeField] private Image _wellWaterImaget;
    [Header("스테이지 정보")]
    [SerializeField] private Text _stageInfoText;
    [Header("프레젠터 스크립트")]
    [SerializeField] private Presenter _presenter;

    [Header("클리어 판넬 정보")]
    [SerializeField] private Text _clearTime;
    [SerializeField] private Image[] _scoreStar = new Image[3];

    [Header("설정 슬라이드 바")]
    [SerializeField] public Slider soundSlider;
    [SerializeField] public Slider rotateSlider;
    [SerializeField] private Text _soundSliderValue;
    [SerializeField] private Text _rotateSliderValue;

    [SerializeField] private FollowCamera _floowCamera;


    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        _jarWaterLVText.text = "0.0 / 0.0";
        _destroyJarText.text = "0";
        _wellWaterLVText.text = "0 %";
        _stageInfoText.text = "0 스테이지 : 테스트맵";
        _wellWaterImaget.fillAmount = 0;
        _jarWaterImaget.fillAmount = 0;

        soundSlider.value = SoundManager.Instance.audioSource.volume;
        _soundSliderValue.text = $"{soundSlider.value}";
        rotateSlider.value = GameManager.Instance.mouseSpeed;
        _rotateSliderValue.text = $"{rotateSlider.value}";
    }

    public void UpdateJarWater(float current, float max)
    {
        Debug.Log($"<color=yellow> 플레이어{PhotonNetwork.LocalPlayer.ActorNumber} : UI 갱신 </color> ");
        _jarWaterLVText.text = $"{current:0.0} / {max:0.0}";
        _jarWaterImaget.fillAmount = current/max;
    }

    public void UpdateJar(int brokenJar)
    {
        _destroyJarText.text = $"{brokenJar}";
    }

    public void UpdateStage(int stageNum, string stageName)
    {
        _stageInfoText.text = $"{stageNum} 스테이지 : {stageName}";
    }

    public void CurrentWellLV(int persent)
    {
        Debug.Log($"우물 물 이미지 값{(float)persent / 100f}");
        if(persent >= 100)
        {
            persent = 100;
        }
        _wellWaterLVText.text = $"{persent} %";
        _wellWaterImaget.fillAmount = (float)persent /100f;
    }

    public void ClearStageInfo(int score)
    {
        _clearTime.text = "00:00";

        for(int i = 0; i < score; i++)
        {
            _scoreStar[i].color = Color.white;
        }
        for(int i = score; i < 3; i++)
        {
            _scoreStar[i].color = Color.black;
        }
    }
    public void OnSoundSlider()
    {
        SoundManager.Instance.audioSource.volume = soundSlider.value;
        _soundSliderValue.text = $"{soundSlider.value}";
    }
    public void ONRotateSpeedSlider()
    {
        _floowCamera.SpeedSet(rotateSlider.value);
        _rotateSliderValue.text = $"{rotateSlider.value}";
    }
}
