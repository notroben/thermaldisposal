using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public LayerMask interactableMask;

    [Header("Physics Grab Settings")]
    public Transform holdPosition;
    public float grabForce = 15f;

    [Header("Bag Interaction (Day 5)")]
    public Image bagHoldIndicator;
    public float requiredBagHoldTime = 1.5f;
    private float currentBagHoldTime = 0f;

    [Header("Door Settings")]
    public Transform furnaceDoorHinge;
    public float doorOpenAngle = 90f;
    public float doorSpeed = 5f;
    private bool isDoorOpen = false;
    private float currentDoorAngle = 0f;

    [HideInInspector] public GameObject heldObject;

    private Rigidbody heldObjRb;
    private Collider[] heldObjColliders;
    private bool isHoldingTool = false;

    private Vector3 activeToolPosition;
    private Vector3 activeToolRotation;
    public float toolEquipSpeed = 10f;
    private Vector3 toolAnimOffset = Vector3.zero;

    void Update()
    {
        UpdateHoverState();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject != null) DropObject();
            else TryPickUp();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject == null)
            {
                TryInteractEnvironment();
            }
            else
            {
                IronPoker poker = heldObject.GetComponent<IronPoker>();
                if (poker != null)
                {
                    poker.TryPoke(Camera.main.transform);
                    StartCoroutine(PokeAnimation());
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (heldObject != null)
            {
                ClipboardTool clipboard = heldObject.GetComponent<ClipboardTool>();
                if (clipboard != null && !clipboard.isFocused) clipboard.ToggleFocus(true);
            }
            else
            {
                TryHoldBag();
            }
        }
        else
        {
            currentBagHoldTime = 0f;
            if (bagHoldIndicator != null) bagHoldIndicator.fillAmount = 0f;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (heldObject != null)
            {
                ClipboardTool clipboard = heldObject.GetComponent<ClipboardTool>();
                if (clipboard != null) clipboard.ToggleFocus(false);
            }
        }

        if (furnaceDoorHinge != null)
        {
            float targetAngle = isDoorOpen ? doorOpenAngle : 0f;
            currentDoorAngle = Mathf.Lerp(currentDoorAngle, targetAngle, Time.deltaTime * doorSpeed);
            furnaceDoorHinge.localRotation = Quaternion.Euler(0f, currentDoorAngle, 0f);
        }

        if (isHoldingTool && heldObject != null)
        {
            Vector3 targetPos = activeToolPosition + toolAnimOffset;

            float currentSpeed = (toolAnimOffset != Vector3.zero) ? toolEquipSpeed * 3f : toolEquipSpeed;

            heldObject.transform.localPosition = Vector3.Lerp(heldObject.transform.localPosition, targetPos, Time.deltaTime * currentSpeed);
            heldObject.transform.localRotation = Quaternion.Slerp(heldObject.transform.localRotation, Quaternion.Euler(activeToolRotation), Time.deltaTime * currentSpeed);
        }

        //if (isHoldingTool && heldObject != null)
        //{
        //    float distance = Vector3.Distance(heldObject.transform.localPosition, activeToolPosition);
        //    if (distance > 0.001f)
        //    {
        //        heldObject.transform.localPosition = Vector3.Lerp(heldObject.transform.localPosition, activeToolPosition, Time.deltaTime * toolEquipSpeed);
        //        heldObject.transform.localRotation = Quaternion.Slerp(heldObject.transform.localRotation, Quaternion.Euler(activeToolRotation), Time.deltaTime * toolEquipSpeed);
        //    }
        //    else
        //    {
        //        heldObject.transform.localPosition = activeToolPosition;
        //        heldObject.transform.localRotation = Quaternion.Euler(activeToolRotation);
        //    }
        //}
    }

    IEnumerator PokeAnimation()
    {
        toolAnimOffset = new Vector3(0f, 0f, 0.8f);

        yield return new WaitForSeconds(0.15f);

        toolAnimOffset = Vector3.zero;
    }

    void TryHoldBag()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableMask))
        {
            TrashBag_data bag = hit.collider.GetComponent<TrashBag_data>();

            if (bag != null && bag.bagWeight == TrashBag_data.WeightCategory.OverCapacity && !bag.isSealed)
            {
                currentBagHoldTime += Time.deltaTime;
                if (bagHoldIndicator != null) bagHoldIndicator.fillAmount = currentBagHoldTime / requiredBagHoldTime;

                if (currentBagHoldTime >= requiredBagHoldTime)
                {
                    if (!bag.isOpen) bag.OpenBag();
                    else bag.SealBag();

                    currentBagHoldTime = 0f;
                }
            }
        }
    }

    void UpdateHoverState()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        bool lookingAtExit = false;

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableMask))
        {
            if (hit.collider.CompareTag("ExitDoor"))
            {
                lookingAtExit = true;
                ExitDoor exit = hit.collider.GetComponent<ExitDoor>();
                if (exit != null) exit.SetPlayerLooking(true);
            }
        }

        if (!lookingAtExit)
        {
            ExitDoor[] exits = FindObjectsByType<ExitDoor>(FindObjectsSortMode.None);
            foreach (ExitDoor exit in exits) exit.SetPlayerLooking(false);
        }
    }

    void TryPickUp()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableMask))
        {
            if (!hit.collider.CompareTag("Door") && !hit.collider.CompareTag("Switch") && !hit.collider.CompareTag("ExitDoor"))
            {
                GameObject objToPickUp = hit.collider.attachedRigidbody != null ? hit.collider.attachedRigidbody.gameObject : hit.collider.gameObject;

                TrashBag_data bag = objToPickUp.GetComponent<TrashBag_data>();
                if (bag != null && bag.isOpen) return;

                PickUpObject(objToPickUp);
            }
        }
    }

    void TryInteractEnvironment()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableMask))
        {
            if (hit.collider.CompareTag("Switch"))
            {
                FurnaceLogic furnace = FindFirstObjectByType<FurnaceLogic>();
                if (furnace != null) furnace.ActivateFurnace();
            }
            else if (hit.collider.CompareTag("Door")) ToggleDoor();
        }
    }

    void ToggleDoor()
    {
        isDoorOpen = !isDoorOpen;
        FurnaceLogic furnace = FindFirstObjectByType<FurnaceLogic>();
        if (furnace != null) furnace.SetDoorState(!isDoorOpen);
    }

    void FixedUpdate()
    {
        if (!isHoldingTool && heldObject != null && heldObjRb != null)
        {
            Vector3 moveDirection = holdPosition.position - heldObject.transform.position;
            heldObjRb.linearVelocity = moveDirection * grabForce;
        }
    }

    void PickUpObject(GameObject pickObj)
    {
        heldObject = pickObj;
        heldObjRb = pickObj.GetComponent<Rigidbody>();
        heldObjColliders = pickObj.GetComponentsInChildren<Collider>();

        if (pickObj.CompareTag("Tool"))
        {
            isHoldingTool = true;
            if (heldObjRb != null) heldObjRb.isKinematic = true;

            if (heldObjColliders != null)
            {
                foreach (Collider col in heldObjColliders) col.isTrigger = true;
            }

            heldObject.transform.SetParent(transform);

            ToolSettings settings = pickObj.GetComponent<ToolSettings>();
            if (settings != null)
            {
                activeToolPosition = settings.equipPosition;
                activeToolRotation = settings.equipRotation;
            }

            ScannerTool scanner = pickObj.GetComponent<ScannerTool>();
            if (scanner != null) scanner.isEquipped = true;
        }
        else
        {
            isHoldingTool = false;
            if (heldObjRb != null)
            {
                heldObjRb.useGravity = false;
                heldObjRb.linearDamping = 10f;
                heldObjRb.freezeRotation = true;
            }
        }
    }

    void DropObject()
    {
        ClipboardTool clipboard = heldObject.GetComponent<ClipboardTool>();
        if (clipboard != null) clipboard.ToggleFocus(false);

        ScannerTool scanner = heldObject.GetComponent<ScannerTool>();
        if (scanner != null) scanner.isEquipped = false;

        heldObject.transform.SetParent(null);

        if (isHoldingTool)
        {
            if (heldObjRb != null) heldObjRb.isKinematic = false;

            if (heldObjColliders != null)
            {
                foreach (Collider col in heldObjColliders) col.isTrigger = false;
            }
        }
        else
        {
            if (heldObjRb != null)
            {
                heldObjRb.useGravity = true;
                heldObjRb.linearDamping = 0f;
                heldObjRb.freezeRotation = false;
            }
        }

        heldObject = null;
    }
}