using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class BackendManager : MonoBehaviour
{
    public static BackendManager Instance { get; private set; }

    private FirebaseApp app;
    public static FirebaseApp App => Instance.app;

    private FirebaseApp auth;
    public static FirebaseApp Auth => Instance.auth;

    //private FirebaseDatabase database;
    //public static FirebaseDatabase Database => Instance.database;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
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
                auth = FirebaseApp.DefaultInstance;
                //database = FirebaseApp.DefaultInstance;

                Debug.Log("파이어베이스 체크 성공");
            }
            else
            {
                Debug.Log($"파이어 베이스 체크 실패 : {task.Result}");

                app = null;
                auth = null;
                //database = null;
            }
        });
    }
}
