using UnityEngine;
public class NotificationManager : SingletonPattern<NotificationManager>
{
    [SerializeField] PopupNotifyManager popupNotifyManager;
    public void ShowPopUpNotify(string text,NotifyType type = NotifyType.Info)
    {
        popupNotifyManager.Show(text, type);
    }
}
