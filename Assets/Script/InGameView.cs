using UnityEngine;
using UnityEngine.UI;

public class InGameView : MonoBehaviour
{
    [Header("물 수위")]
    [SerializeField] private Text _waterLVText;
    [Header("깨진 항아리")]
    [SerializeField] private Text _destroyJarText;
    [Header("프레젠터")]
    [SerializeField] private Presenter _presenter;

    private void Awake()
    {
        _waterLVText.text = "";
        _destroyJarText.text = "";
    }

    private void Start()
    {
        Jar.OnDestroyJar += UpdateJar;
        Jar.OnWaterLV += UpdateWater;
    }

    private void UpdateWater(Jar jar)
    {
        _waterLVText.text = $"물 수위 : {jar.CurrentWaterLv}/{jar.MaxWaterLv}";

    }

    private void UpdateJar(GameObject gameObject)
    {
        _waterLVText.text = $"깬 항아리 : {GameManager.Instance.DestroyJarNumber} 개";

    }
    private void GaolWaterLv()
    {

    }
}
