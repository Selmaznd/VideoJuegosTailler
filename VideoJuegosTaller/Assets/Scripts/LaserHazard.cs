using UnityEngine;

/// <summary>
/// Simple pulsing laser that respawns the player upon contact.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(LineRenderer))]
public class LaserHazard : MonoBehaviour
{
    [Tooltip("Laser length in meters.")]
    public float length = 5f;

    [Tooltip("Beam stays active during this duration.")]
    public float onDuration = 1.5f;

    [Tooltip("Beam stays inactive during this duration.")]
    public float offDuration = 1.0f;

    [Tooltip("Delay before the first activation cycle starts.")]
    public float startDelay = 0f;

    private LineRenderer _lineRenderer;
    private BoxCollider _collider;
    private float _timer;
    private bool _isActive;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _collider = GetComponent<BoxCollider>();

        _collider.isTrigger = true;
        ConfigureLineRenderer();
        ConfigureCollider();

        _timer = -Mathf.Max(0f, startDelay);
        SetActive(false);
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_isActive)
        {
            if (_timer >= onDuration)
            {
                _timer = 0f;
                SetActive(false);
            }
        }
        else
        {
            if (_timer >= offDuration)
            {
                _timer = 0f;
                SetActive(true);
            }
        }
    }

    private void ConfigureLineRenderer()
    {
        _lineRenderer.alignment = LineAlignment.TransformZ;
        _lineRenderer.positionCount = 2;
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.startWidth = 0.2f;
        _lineRenderer.endWidth = 0.2f;

        if (_lineRenderer.material == null)
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
            if (shader != null)
            {
                _lineRenderer.material = new Material(shader);
            }
        }
        _lineRenderer.startColor = new Color(1f, 0.2f, 0.2f, 0.9f);
        _lineRenderer.endColor = new Color(1f, 0.4f, 0.4f, 0.9f);
        _lineRenderer.SetPosition(0, Vector3.zero);
        _lineRenderer.SetPosition(1, Vector3.forward * length);
    }

    private void ConfigureCollider()
    {
        _collider.center = new Vector3(0f, 0f, length * 0.5f);
        _collider.size = new Vector3(0.3f, 1.0f, Mathf.Max(0.1f, length));
    }

    private void SetActive(bool value)
    {
        _isActive = value;
        if (_lineRenderer != null) _lineRenderer.enabled = value;
        if (_collider != null) _collider.enabled = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isActive) return;

        var respawn = other.GetComponent<RespawnOnTrigger>() ?? other.GetComponentInParent<RespawnOnTrigger>();
        if (respawn != null)
        {
            respawn.SendMessage("Respawn", SendMessageOptions.DontRequireReceiver);
        }
    }
}
