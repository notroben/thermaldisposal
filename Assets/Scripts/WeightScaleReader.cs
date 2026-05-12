using UnityEngine;
using TMPro;

public class WeightScaleReader : MonoBehaviour
{
    [Header("Scale Screen")]
    public TextMeshPro scaleDisplay;
    public GameManager gameManager;

    private Collider currentCollider;

    void Update()
    {
        if (currentCollider != null && !currentCollider.enabled)
        {
            ResetScale();
            currentCollider = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        currentCollider = other;
        TrashBag_data bagData = other.GetComponent<TrashBag_data>();

        if (bagData != null)
        {
            if (!bagData.hasBeenWeighed)
            {
                if (gameManager != null && gameManager.physicalWeighedCount > gameManager.uiCrossedOutCount)
                {
                    GameEvents.OnTriggerGameOver?.Invoke(RuleBreak.ScaleUnfinalized);
                }

                bagData.hasBeenWeighed = true;
                if (gameManager != null) gameManager.physicalWeighedCount++;
            }

            if (bagData.bagWeight == TrashBag_data.WeightCategory.Optimal)
            {
                scaleDisplay.text = "OPTIMAL";
                scaleDisplay.color = Color.green;
            }
            else if (bagData.bagWeight == TrashBag_data.WeightCategory.OverCapacity)
            {
                scaleDisplay.text = "OVER_CAPACITY";
                scaleDisplay.color = Color.red;
            }
            else
            {
                scaleDisplay.text = "UNDER_LOAD"; // might be removed later idk
                scaleDisplay.color = Color.yellow;
            }
        }
        if (other.CompareTag("Player"))
        {
            scaleDisplay.text = "OPTIMAL";
            scaleDisplay.color = Color.green;
        }
        else if (other.CompareTag("Tool"))
        {
            scaleDisplay.text = "INVALID";
            scaleDisplay.color = Color.yellow;
        }
        else if (other.CompareTag("ExcessTrash"))
        {
            scaleDisplay.text = "INVALID";
            scaleDisplay.color = Color.yellow;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (currentCollider == other)
        {
            ResetScale();
            currentCollider = null;
        }
    }

    void ResetScale()
    {
        scaleDisplay.text = "EMPTY";
        scaleDisplay.color = Color.white;
    }
}