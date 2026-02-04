using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using Firebase.Database;

public class BackendManager : MonoBehaviour
{
    [SerializeField] private Launcher _launcher;

    private bool isReady;
    public static bool IsReady => Instance.isReady;

    public static BackendManager Instance { get; private set; }

    private FirebaseApp app;
    public static FirebaseApp App => Instance.app;

    private FirebaseAuth auth;
    public static FirebaseAuth Auth => Instance.auth;

    private FirebaseDatabase database;
    public static FirebaseDatabase Database => Instance.database;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            isReady = false;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance;

                isReady = true;
                Debug.Log("파이어베이스 체크 성공");
            }
            else
            {
                Debug.Log($"파이어 베이스 체크 실패 : {task.Result}");

                app = null;
                auth = null;
                database = null;
            }
        });
    }
}
