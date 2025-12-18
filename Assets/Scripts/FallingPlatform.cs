using System.Collections;
using UnityEngine;

/// <summary>
/// Causes a platform to drop after the player steps on it, then rebuilds itself.
/// Works with Rigidbody2D + BoxCollider2D + (optional) PlatformEffector2D.
/// </summary>
public class FallingPlatform : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float fallDelay = 0.35f;
    [SerializeField] private float resetDelay = 3.5f;

    [Header("Physics")]
    [SerializeField] private float gravityScaleWhileFalling = 3f;
    [SerializeField] private bool disableColliderWhileFalling = true;

    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private bool _isTriggered;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        if (_rigidbody == null)
        {
            Debug.LogError($"{nameof(FallingPlatform)} on {name} needs a Rigidbody2D.");
            enabled = false;
            return;
        }

        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isTriggered || collision.collider.CompareTag("Player") == false)
        {
            return;
        }

        StartCoroutine(FallRoutine());
    }

    private IEnumerator FallRoutine()
    {
        _isTriggered = true;
        yield return new WaitForSeconds(fallDelay);

        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.gravityScale = gravityScaleWhileFalling;

        if (disableColliderWhileFalling && _collider != null)
        {
            _collider.enabled = false;
        }

        yield return new WaitForSeconds(resetDelay);

        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _rigidbody.bodyType = RigidbodyType2D.Static;
        _rigidbody.gravityScale = 0f;

        transform.SetPositionAndRotation(_startPosition, _startRotation);

        if (_collider != null)
        {
            _collider.enabled = true;
        }

        _isTriggered = false;
    }
}
