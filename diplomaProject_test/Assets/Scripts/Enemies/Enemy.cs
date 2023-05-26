using UnityEngine;

public class Enemy : MonoBehaviour
{


    private KillCounter _killCounter = null;

    private void Start()
    {
        _killCounter = KillCounter.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        IOnHitable hitable = other.GetComponent<IOnHitable>();
        if (hitable != null)
        {
            hitable.OnHit();
            Destroy(this.gameObject);

            if (_killCounter != null)
                _killCounter.UpdateScoreText();
        }
    }
}
