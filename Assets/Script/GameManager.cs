using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int DestroyJarNumber {  get; private set; }

    private void Awake()
    {
        #region 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        #endregion

        DestroyJarNumber = 0;
    }

    private void Start()
    {
        Jar.OnDestroyJar += CountJar;
    }

    public void CountJar()
    {
        DestroyJarNumber++;
    }


}
