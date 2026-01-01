using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class WorldLight : MonoBehaviour
{
    [Header("References")]
    private Light2D _light;

    [Header("Day Night Settings")]
    [Tooltip("Màu ánh sáng theo thời gian trong ngày")]
    public Gradient lightColorGradient;

    [Tooltip("Cường độ ánh sáng theo thời gian trong ngày")]
    public AnimationCurve lightIntensityCurve;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
    }

    private void Update()
    {
        if (GameTimer.Instance == null) return;

        float time01 = GameTimer.Instance.TimeOfDay01;

        _light.color = lightColorGradient.Evaluate(time01);
        _light.intensity = lightIntensityCurve.Evaluate(time01);
    }
}
