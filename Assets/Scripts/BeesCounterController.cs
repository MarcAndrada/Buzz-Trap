using TMPro;
using UnityEngine;

public class BeesCounterController : MonoBehaviour
{
    public static BeesCounterController Instance;

    private int totalBeesKilled;

    [SerializeField]
    private TextMeshProUGUI beesCaughText;
    private void Awake()
    {
        if(Instance != null)
            Destroy(Instance);

        Instance = this;
    }


    public void AddCaughBee()
    {
        totalBeesKilled++;

        beesCaughText.text = totalBeesKilled.ToString();
    }

    public int GetBeesCaugh()
    {
        return totalBeesKilled;
    }



}
