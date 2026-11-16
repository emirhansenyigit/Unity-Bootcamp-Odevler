using UnityEngine;

public class KupDondurucu : MonoBehaviour
{
    public Vector3 donusHizi = new Vector3(45f, 70f, 30f);

    void Update()
    {
        transform.Rotate(donusHizi * Time.deltaTime);
    }
}