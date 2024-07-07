using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{

    [SerializeField]
    private Image[] hearts;
    [SerializeField]
    private Sprite heart;
    [SerializeField]
    private Sprite brokenHeart;

    public void UpdateHearts(int _currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < _currentHealth)
                hearts[i].sprite = heart;
            else
                hearts[i].sprite = brokenHeart;
        }
    }
}
