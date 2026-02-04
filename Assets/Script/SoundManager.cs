using Photon.Pun;
using UnityEngine;

public enum Sound
{
    BaseButtonClick,
    BackButtonClick,
    JarPickUp,
    JarPutDown,
    JarThrow,
    JarDamaged,
    JarDestroy,
    WaterFill,
    WellWaterFill,
    PlayerBumped,
    PlayerRespwan,
    Clear,
    SettingPanelOpen,
    SettingPanelClose,
    BaseBGM,
    LobbyBGM,
    InGameBGM
}

public class SoundManager : MonoBehaviourPunCallbacks
{
    public static SoundManager Instance;
    public AudioSource audioSource;

    [Header("기본 버튼 클릭음")]
    [SerializeField] AudioClip _buttonClick;

    [Header("뒤로 돌아가기")]
    [SerializeField] AudioClip _backButtonClick;

    [Header("유리병 잡기")]
    [SerializeField] AudioClip _jarPickUp;

    [Header("유리병 내려놓기")]
    [SerializeField] AudioClip _jarPutDown;

    [Header("유리병 던지기")]
    [SerializeField] AudioClip _jarThrow;

    [Header("유리병 HP감소")]
    [SerializeField] AudioClip _jarDamage;

    [Header("유리병 파괴음")]
    [SerializeField] AudioClip _jarDestroy;

    [Header("물 차는 음")]
    [SerializeField] AudioClip _waterFill;

    [Header("우물 항아리 상호작용")]
    [SerializeField] AudioClip _wellWaterFill;

    [Header("플레이어 충돌")]
    [SerializeField] AudioClip _bumped;

    [Header("플레이어 리스폰")]
    [SerializeField] AudioClip _playerRespwan;

    [Header("클리어")]
    [SerializeField] AudioClip _clear;

    [Header("팝업/메뉴 열기")]
    [SerializeField] AudioClip _PanelOpen;

    [Header("팝업/메뉴 닫기")]
    [SerializeField] AudioClip _PanelClose;

    [Header("게임 시작")]
    [SerializeField] AudioClip _baseBGM;

    [Header("로비")]
    [SerializeField] AudioClip _lobbyBGM;

    [Header("인게임")]
    [SerializeField] AudioClip _inGameBGM;

    public bool isPlayBGM= false;

    private void Awake()
    {
        #region 싱글톤
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        #endregion

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        audioSource.volume = 0.5f;
        SoundPlay(Sound.BaseBGM);
    }

    public void SoundPlay(Sound sound)
    {
        switch(sound)
        {
            case Sound.BaseButtonClick :
            {
                    Debug.Log($"기본 버튼 클릭");
                    audioSource.PlayOneShot(_buttonClick);
                    break;
            }
            case Sound.BackButtonClick:
            {
                    Debug.Log($"뒤로가기 버튼 클릭");
                    audioSource.PlayOneShot(_backButtonClick);
                    break;
            }

            case Sound.JarPickUp:
            {
                    Debug.Log($"항아리 잡기 사운드");
                    audioSource.PlayOneShot(_jarPickUp);
                    break;
            }
            case Sound.JarPutDown:
            {
                    Debug.Log($"항아리 내려놓기 사운드");
                    audioSource.PlayOneShot(_jarPutDown);
                    break;
            }
            case Sound.JarThrow:
            {
                    Debug.Log($"항아리 던지기 사운드");
                    audioSource.PlayOneShot(_jarThrow);
                    break;
            }
            case Sound.JarDamaged:
            {
                    Debug.Log($"항아리 데미지 사운드");
                    audioSource.PlayOneShot(_jarDamage);
                    break;
            }
            case Sound.JarDestroy:
            {
                    Debug.Log($"항아리 파괴 사운드");
                    audioSource.PlayOneShot(_jarDestroy);
                    break;
            }

            case Sound.WaterFill:
            {
                    Debug.Log($"항아리 물 채우기 사운드");
                    audioSource.PlayOneShot(_waterFill);
                    break;
            }
            case Sound.WellWaterFill:
            {
                    Debug.Log($"우물 물 채우기 사운드");
                    audioSource.PlayOneShot(_wellWaterFill);
                    break;
            }

            case Sound.PlayerBumped:
            {
                    Debug.Log($"충돌 사운드");
                    audioSource.PlayOneShot(_bumped);
                    break;
            }
            case Sound.PlayerRespwan:
            {
                    Debug.Log($"리스폰 사운드");
                    audioSource.PlayOneShot(_playerRespwan);
                    break;
            }

            case Sound.Clear:
            {
                    Debug.Log($"클리어 사운드");
                    audioSource.PlayOneShot(_clear);
                    break;
            }
            case Sound.SettingPanelOpen:
            {
                    Debug.Log($"메뉴 열기 사운드");
                    audioSource.PlayOneShot(_PanelOpen);
                    break;
            }
            case Sound.SettingPanelClose:
            {
                    Debug.Log($"메뉴 닫기 사운드");
                    audioSource.PlayOneShot(_PanelClose);
                    break;
            }
            case Sound.BaseBGM:
            {
                    audioSource.Stop();
                    Debug.Log($"기본 브금");
                    audioSource.loop = true;
                    audioSource.clip = _baseBGM;
                    audioSource.Play();
                    break;
            }
            case Sound.LobbyBGM:
            {
                    audioSource.Stop();
                    Debug.Log($"로비 브금");
                    audioSource.loop = true;
                    audioSource.clip = _lobbyBGM;
                    audioSource.Play();
                    isPlayBGM = true;
                    break;
            }
            case Sound.InGameBGM:
            {
                    audioSource.Stop();
                    Debug.Log($"게임 브금");
                    audioSource.loop = true;
                    audioSource.clip = _inGameBGM;
                    audioSource.Play();
                    break;
            }
        }
    }
}
