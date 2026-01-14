using UnityEngine;
using UnityEngine.UI;

public class InGameView : MonoBehaviour
{
    [Header("물 수위")]
    [SerializeField] private Text _waterLVText;
    [Header("깨진 항아리")]
    [SerializeField] private Text _destroyJarText;
    [Header("항아리 모델")]
    [SerializeField] private Jar _jarScript;
    [Header("물버튼 모델")]
    [SerializeField] private WaterButton _waterScript;

    private void Awake()
    {
        _waterLVText.text = "";
        _destroyJarText.text = "";
    }

    private void Start()
    {
        Jar.OnDestroyJar += UpdateJar;
        _jarScript.OnWaterLV += UpdateWater;
    }

    private void UpdateWater(Jar jar)
    {
        _waterLVText.text = $"물 수위 : {jar.CurrentWaterLv}/{_jarScript.MaxWaterLv}";

    }

    private void UpdateJar()
    {
        _waterLVText.text = $"깬 항아리 : {GameManager.Instance.DestroyJarNumber} 개";

    }
}
