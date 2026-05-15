using UnityEngine;
using TMPro;

public class ScannerTool : MonoBehaviour
{
    public TextMeshProUGUI screenText;
    public float scanTime = 1.5f;
    private float currentScanTime = 0f;
    private bool hasScanned = false;

    [HideInInspector] public bool isEquipped = false;

    void Update()
    {
        if (!isEquipped) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (ServiceLocator.AudioManager != null) ServiceLocator.AudioManager.PlaySFXAtPosition("ScannerClick", transform.position);
        }

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
                if (!hasScanned)
                {
                    float percent = (currentScanTime / scanTime) * 100f;
                    screenText.text = "SCANNING...\n" + percent.ToString("F0") + "%";

                    if (currentScanTime >= scanTime)
                    {
                        hasScanned = true;
                        if (targetBag.isOrganic)
                        {
                            screenText.text = "<color=orange>ORGANIC\nMATTER\nDETECTED</color>";
                            if (ServiceLocator.AudioManager != null) ServiceLocator.AudioManager.PlaySFXAtPosition("ErrorBeep", transform.position);
                        }
                        else
                        {
                            screenText.text = "<color=green>NO METAL\nDETECTED</color>";
                            if (ServiceLocator.AudioManager != null) ServiceLocator.AudioManager.PlaySFXAtPosition("ScannerBeep", transform.position);
                        }
                    }
                }
            }
            else
            {
                screenText.text = "INVALID\nTARGET";
                currentScanTime = 0f;
                hasScanned = false;
            }
        }
        else
        {
            screenText.text = "READY";
            currentScanTime = 0f;
            hasScanned = false;
        }
    }
}