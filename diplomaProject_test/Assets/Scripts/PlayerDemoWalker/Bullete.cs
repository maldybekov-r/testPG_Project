using UnityEngine;

public class Bullete : MonoBehaviour, IOnHitable
{
    private const float _BULLET_SPEED = 100.0f;

    [Header("Bullete Components:")]
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Collider _collider;
    [SerializeField] private Rigidbody _rb;

    public void OnHide()
    {
        _meshRenderer.enabled = false;
        _collider.enabled = false;
    }

    public void OnShow()
    {
        _meshRenderer.enabled = true;
        _collider.enabled = true;
    }

    public void OnPushed(Vector3 dir, Vector3 originPos)
    {
        transform.position = originPos;
        _rb.velocity = Vector3.zero;
        _rb.velocity = dir * _BULLET_SPEED;
    }

    public void OnHit()
    {
        OnHide();
    }
}
