using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum NotifyType
{
    Info,
    Success,
    Warning,
    Error
}

public class PopupNotifyManager : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> pools = new();

    [Header("Config")]
    [SerializeField] private float popupDuration = 3f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float offsetY = 40f;

    private List<TextMeshProUGUI> activePopups = new();
    private Dictionary<TextMeshProUGUI, Vector2> originPos = new();
    private Dictionary<TextMeshProUGUI, Coroutine> runningCoroutines = new();

    private void Awake()
    {
        foreach (var p in pools)
        {
            originPos[p] = p.rectTransform.anchoredPosition;
            p.gameObject.SetActive(false);
            p.alpha = 1f;
        }
    }

    // ================= PUBLIC =================
    public void Show(string text, NotifyType type)
    {
        TextMeshProUGUI popup = GetPopup();

        SetupPopup(popup, text, type);

        popup.gameObject.SetActive(true);
        popup.alpha = 1f;

        activePopups.Add(popup);
        RepositionPopups();

        // start coroutine mới
        Coroutine c = StartCoroutine(AutoHide(popup));
        runningCoroutines[popup] = c;
    }

    // ================= PRIVATE =================

    private TextMeshProUGUI GetPopup()
    {
        foreach (var p in pools)
        {
            if (!p.gameObject.activeSelf)
                return p;
        }

        // reuse popup cũ nhất
        TextMeshProUGUI oldest = activePopups[0];
        activePopups.RemoveAt(0);

        // STOP coroutine của popup này
        if (runningCoroutines.TryGetValue(oldest, out Coroutine c))
        {
            StopCoroutine(c);
            runningCoroutines.Remove(oldest);
        }

        return oldest;
    }

    private void SetupPopup(TextMeshProUGUI popup, string text, NotifyType type)
    {
        popup.text = text;
        popup.color = GetColor(type);
    }

    private IEnumerator AutoHide(TextMeshProUGUI popup)
    {
        yield return new WaitForSeconds(popupDuration - fadeDuration);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            popup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        popup.gameObject.SetActive(false);
        popup.alpha = 1f;

        activePopups.Remove(popup);
        runningCoroutines.Remove(popup);

        RepositionPopups();
    }

    private void RepositionPopups()
    {
        for (int i = 0; i < activePopups.Count; i++)
        {
            TextMeshProUGUI p = activePopups[i];
            p.rectTransform.anchoredPosition =
                originPos[p] + Vector2.up * offsetY * i;
        }
    }

    private Color GetColor(NotifyType type)
    {
        return type switch
        {
            NotifyType.Success => Color.green,
            NotifyType.Warning => Color.yellow,
            NotifyType.Error => Color.red,
            _ => Color.white
        };
    }
}
