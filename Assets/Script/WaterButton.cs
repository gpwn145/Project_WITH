using System;
using UnityEngine;

public class WaterButton : MonoBehaviour
{
    [Header("물 오브젝트")]
    [SerializeField] private GameObject _waterObject;
    private Collider _waterCollider;

    public bool IsPressed { get; set; }

    private void Start()
    {
        _waterCollider = _waterObject.GetComponent<Collider>();
        _waterCollider.enabled = false;
        _waterObject.tag = "Water";
        _waterObject.SetActive(false);
    }

    public void WaterOpen(bool isPressed)
    {
        _waterObject.SetActive(isPressed);
        _waterCollider.enabled = isPressed;
        Debug.Log("물 버튼 눌림");
    }
}
