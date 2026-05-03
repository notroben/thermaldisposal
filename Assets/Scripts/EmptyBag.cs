using UnityEngine;

public class EmptyBagLogic : MonoBehaviour
{
    public int requiredTrash = 5;
    private int currentTrash = 0;
    private TrashBag_data bagData;
    private Vector3 startingScale;

    void Start()
    {
        bagData = GetComponent<TrashBag_data>();
        startingScale = transform.localScale;

        if (bagData != null) bagData.bagWeight = TrashBag_data.WeightCategory.UnderLoad;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ExcessTrash"))
        {
            Destroy(collision.gameObject);
            currentTrash++;

            float puffAmount = (float)currentTrash / requiredTrash;
            transform.localScale = Vector3.Lerp(startingScale, new Vector3((float)1.5, (float)1.5, (float)1.5), puffAmount);

            if (currentTrash >= requiredTrash)
            {
                if (bagData != null) bagData.bagWeight = TrashBag_data.WeightCategory.Optimal;
                transform.localScale = new Vector3((float)1.5, (float)1.5, (float)1.5);
            }
        }
    }
}