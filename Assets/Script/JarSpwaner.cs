using UnityEngine;

public class JarSpwaner : MonoBehaviour
{
    public bool hasJar;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Jar")
        {
            hasJar = true;
            Debug.Log("항아리 있음");
        }
    }
}
