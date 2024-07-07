using UnityEngine;
using UnityEngine.UI;

public class UICooldownController : MonoBehaviour
{
    [SerializeField]
    private Image fillImage;
    public float maxTime;
    public float currentTime;

    // Update is called once per frame
    void Update()
    {
        fillImage.fillAmount = currentTime / maxTime;
    }
}
