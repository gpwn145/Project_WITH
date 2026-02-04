using System;
using UnityEngine;

public class WaterButton : MonoBehaviour
{
    [Header("물 오브젝트")]
    [SerializeField] private GameObject _waterObject;
    [Header("빛 오브젝트")]
    [SerializeField] private GameObject _lightObject;
    [Header("프레젠터")]
    [SerializeField] private Presenter _presenter;
    private Collider _waterCollider;

    public bool IsPressed { get; set; }

    private void Start()
    {
        _waterCollider = _waterObject.GetComponent<Collider>();
        _waterCollider.enabled = false;
        _waterObject.tag = "Water";
        _waterObject.SetActive(false);
        _lightObject.SetActive(false);
    }

    public void WaterOpen(bool isPressed)
    {
        if(isPressed == true)
        {
            Debug.Log("물 버튼 눌림");
        }
        _waterObject.SetActive(isPressed);
        _lightObject.SetActive(isPressed);
        _waterCollider.enabled = isPressed;
    }
}
