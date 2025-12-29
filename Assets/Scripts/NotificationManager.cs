using UnityEngine;
public class NotificationManager : SingletonPattern<NotificationManager>
{
    [SerializeField] PopupNotifyManager popupNotifyManager;
    public void ShowPopUpNotify(string text,NotifyType type)
    {
        popupNotifyManager.Show(text, type);
    }
}
