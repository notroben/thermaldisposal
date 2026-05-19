using UnityEngine;

public class EmptyBagLogic : MonoBehaviour
{
    public int requiredTrash = 5;
    private int currentTrash = 0;
    private TrashBag_data bagData;

    [Header("Model References")]
    public GameObject emptyModel;
    public GameObject fullModel;

    private Vector3 emptyModelStartScale;

    void Start()
    {
        bagData = GetComponent<TrashBag_data>();
        if (bagData != null) bagData.bagWeight = TrashBag_data.WeightCategory.UnderLoad;

        if (emptyModel != null) emptyModelStartScale = emptyModel.transform.localScale;
        if (fullModel != null) fullModel.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ExcessTrash"))
        {
            Destroy(collision.gameObject);
            currentTrash++;

            if (ServiceLocator.AudioManager != null) ServiceLocator.AudioManager.PlaySFXAtPosition("BagSeal", transform.position);

            if (currentTrash >= requiredTrash)
            {
                if (bagData != null) bagData.bagWeight = TrashBag_data.WeightCategory.Optimal;
                if (emptyModel != null) emptyModel.SetActive(false);
                if (fullModel != null) fullModel.SetActive(true);
            }
            else if (emptyModel != null)
            {
                float puffAmount = (float)currentTrash / requiredTrash;
                Vector3 puffedScale = emptyModelStartScale;
                puffedScale.y = Mathf.Lerp(emptyModelStartScale.y, emptyModelStartScale.y * 10f, puffAmount);
                emptyModel.transform.localScale = puffedScale;
            }
        }
    }
}