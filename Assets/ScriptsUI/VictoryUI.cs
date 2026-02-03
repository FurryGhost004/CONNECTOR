using TMPro;
using UnityEngine;

public class VictoryUI : MonoBehaviour
{
    public static VictoryUI Instance;

    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject[] starImages;
    [SerializeField] private TextMeshProUGUI rewardText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ShowVictoryPanel(int starRating, int reward)
    {
        victoryPanel.SetActive(true);
        rewardText.text = "Your Payment: + " + reward.ToString();

        foreach (var star in starImages) star.SetActive(false);
        for (int i = 0; i < starRating; i++)
        {
            if (i < starImages.Length) starImages[i].SetActive(true);
        }

        GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        GameObject.FindWithTag("Player").GetComponent<CharaterController>().enabled = false;
    }
}
