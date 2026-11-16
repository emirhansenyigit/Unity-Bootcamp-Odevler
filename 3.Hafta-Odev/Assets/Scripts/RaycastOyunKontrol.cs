using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class RaycastOyunKontrol : MonoBehaviour
{
    public Transform piyon;
    public Transform kale;
    public float hareketHizi = 6f;

    private Vector3 baslangicPozisyonu;
    private Vector3 ilerlemeYonu;
    private float toplamPistMesafesi;
    private float mevcutMesafe = 0f;
    private string durumMesaji = "Dönen küpün bir rengine týkla!";
    private bool oyunBitti = false;

    void Start()
    {
        baslangicPozisyonu = piyon.position;
        Vector3 hedefNokta = new Vector3(kale.position.x, baslangicPozisyonu.y, kale.position.z);
        toplamPistMesafesi = Vector3.Distance(baslangicPozisyonu, hedefNokta);
        ilerlemeYonu = (hedefNokta - baslangicPozisyonu).normalized;
    }

    void Update()
    {
        Vector3 hedefPozisyon = baslangicPozisyonu + (ilerlemeYonu * mevcutMesafe);
        piyon.position = Vector3.Lerp(piyon.position, hedefPozisyon, Time.deltaTime * hareketHizi);

        if (oyunBitti) return;

        bool tiklandi = false;
        Vector2 farePozisyonu = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            tiklandi = true;
            farePozisyonu = Mouse.current.position.ReadValue();
        }
#else
        if (Input.GetMouseButtonDown(0))
        {
            tiklandi = true;
            farePozisyonu = Input.mousePosition;
        }
#endif

        if (tiklandi)
        {
            Ray isin = Camera.main.ScreenPointToRay(farePozisyonu);
            RaycastHit carpanObje;

            if (Physics.Raycast(isin, out carpanObje, 100f))
            {
                YuzeyEtkisi yuzey = carpanObje.collider.GetComponent<YuzeyEtkisi>();

                if (yuzey != null)
                {
                    mevcutMesafe = Mathf.Clamp(mevcutMesafe + yuzey.mesafeDegeri, 0f, toplamPistMesafesi);
                    float kalan = toplamPistMesafesi - mevcutMesafe;
                    durumMesaji = $"Hamle: {yuzey.mesafeDegeri:+#;-#;0} birim | Kaleye Kalan: {kalan:F1} m";

                    if (mevcutMesafe >= toplamPistMesafesi - 0.05f)
                    {
                        oyunBitti = true;
                        durumMesaji = "TEBRÝKLER! KALEYE ULAÞTINIZ!";
                    }
                }
            }
        }
    }

    void OnGUI()
    {
        GUIStyle stil = new GUIStyle();
        stil.fontSize = 24;
        stil.normal.textColor = Color.white;
        GUI.Label(new Rect(20, 20, 650, 40), durumMesaji, stil);
    }
}