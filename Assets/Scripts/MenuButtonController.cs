using UnityEngine;
using UnityEngine.InputSystem;

public class MenuButtonController : MonoBehaviour
{
    [SerializeField] InputActionAsset IAAsset;
    [SerializeField]MenuUIController menuUIController;

    private MenuType currentOpenMenu = MenuType.None;

    private void DisableCharacterControl()
    {
        if (IAAsset != null) {
            IAAsset.FindActionMap("Character").Disable();
        }
    }
    private void EnableCharacterControl()
    {
        if (IAAsset != null)
        {
            IAAsset.FindActionMap("Character").Enable();
        }
    }
    public void OpenInventory()
    {
        if (currentOpenMenu == MenuType.Inventory)
        {
            menuUIController.CloseCurrent();
            currentOpenMenu = MenuType.None;
            EnableCharacterControl();
        }
        else
        {
            currentOpenMenu = MenuType.Inventory;
            menuUIController.Open(MenuType.Inventory);
            DisableCharacterControl();
        }
            
    }
}
