using UnityEngine;
using TMPro;

public class ScannerTool : MonoBehaviour
{
    public TextMeshProUGUI screenText;
    public float scanTime = 1.5f;
    private float currentScanTime = 0f;

    [HideInInspector] public bool isEquipped = false;

    void Update()
    {
        if (!isEquipped) return;

        if (Input.GetMouseButton(0))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit[] hits = Physics.RaycastAll(ray, 5f);

            TrashBag_data targetBag = null;

            foreach (RaycastHit hit in hits)
            {
                TrashBag_data bag = hit.collider.GetComponent<TrashBag_data>();
                if (bag != null)
                {
                    targetBag = bag;
                    break;
                }
            }

            if (targetBag != null)
            {
                currentScanTime += Time.deltaTime;
                float percent = (currentScanTime / scanTime) * 100f;
                screenText.text = "SCANNING...\n" + percent.ToString("F0") + "%";

                if (currentScanTime >= scanTime)
                {
                    if (targetBag.isOrganic)
                    {
                        screenText.text = "<color=orange>ORGANIC\nMATTER\nDETECTED</color>";
                    }
                    else if (targetBag.hasMetal) // unnecessary, might be removed later
                    {
                        screenText.text = "<color=red>METAL\nDETECTED</color>";
                    }
                    else
                    {
                        screenText.text = "<color=green>NO METAL\nDETECTED</color>";
                    }
                }
            }
            else
            {
                screenText.text = "INVALID\nTARGET";
                currentScanTime = 0f;
            }
        }
        else
        {
            screenText.text = "READY";
            currentScanTime = 0f;
        }
    }
}