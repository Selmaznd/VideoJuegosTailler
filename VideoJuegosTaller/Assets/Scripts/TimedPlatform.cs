using UnityEngine;

/// <summary>
/// Toggles a platform's visibility and colliders on a loop to create timing challenges.
/// </summary>
public class TimedPlatform : MonoBehaviour
{
    [Tooltip("Duration (in seconds) the platform stays visible/solid.")]
    public float visibleDuration = 2f;

    [Tooltip("Duration (in seconds) the platform disappears.")]
    public float hiddenDuration = 1.2f;

    private float _timer;
    private bool _isVisible = true;
    private Renderer[] _renderers = System.Array.Empty<Renderer>();
    private Collider[] _colliders = System.Array.Empty<Collider>();

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
        _colliders = GetComponentsInChildren<Collider>(includeInactive: true);
        ApplyState(_isVisible);
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_isVisible && _timer >= visibleDuration)
        {
            _timer = 0f;
            ApplyState(false);
        }
        else if (!_isVisible && _timer >= hiddenDuration)
        {
            _timer = 0f;
            ApplyState(true);
        }
    }

    private void ApplyState(bool visible)
    {
        _isVisible = visible;

        foreach (var rend in _renderers)
        {
            if (rend != null) rend.enabled = visible;
        }

        foreach (var col in _colliders)
        {
            if (col != null) col.enabled = visible;
        }
    }
}
