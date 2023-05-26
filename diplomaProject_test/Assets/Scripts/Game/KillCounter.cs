using UnityEngine;

public class KillCounter : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _scoreText;
    private int _kills = 0;

    public static KillCounter Instance = null;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    public void UpdateScoreText()
    {
        _kills++;
        _scoreText.text = _kills.ToString();
    }
}
