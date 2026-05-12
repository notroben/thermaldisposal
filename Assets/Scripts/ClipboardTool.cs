using UnityEngine;

public class ClipboardTool : MonoBehaviour
{
    private PlayerController player;
    public bool isFocused = false;

    void Start()
    {
        player = ServiceLocator.PlayerController;
    }

    public void ToggleFocus(bool focusStatus)
    {
        isFocused = focusStatus;

        if (isFocused)
        {
            player.canLook = false;
            player.canMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            player.canLook = true;
            player.canMove = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}