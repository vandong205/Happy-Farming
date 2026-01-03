using UnityEngine;
public enum MenuType
{
    None,
    Inventory,
    Skill,
    Map,
    Setting
}

public class MenuUIController : MonoBehaviour
{

    [SerializeField] GameObject inventoryPanel;

    GameObject currentPanel;
    private void Awake()
    {
        inventoryPanel.SetActive(false);
    }
    public void Open(MenuType type)
    {
        CloseCurrent();

        currentPanel = GetPanel(type);
        if (currentPanel != null)
            currentPanel.SetActive(true);
    }

    public void CloseCurrent()
    {
        if (currentPanel != null)
            currentPanel.SetActive(false);
    }

    GameObject GetPanel(MenuType type)
    {
        return type switch
        {
            MenuType.Inventory => inventoryPanel,
            _ => null
        };
    }
}
