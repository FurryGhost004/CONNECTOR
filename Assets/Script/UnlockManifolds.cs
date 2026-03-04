using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockManifolds : MonoBehaviour
{
    public static UnlockManifolds instance;
    [SerializeField] GameObject MiniGame;

    public List<Button> buttons;
    public List<Button> shuffledButtons;
    int counter = 0;

    // Cửa đang mở minigame (được gán khi player chạm cửa)
    public static GameObject currentDoor;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        RestartTheGame();
    }

    public void RestartTheGame()
    {
        counter = 0;

        // Shuffle nút
        shuffledButtons = buttons.OrderBy(a => Random.Range(0, 100)).ToList();

        for (int i = 1; i <= 10; i++)
        {
            shuffledButtons[i - 1].GetComponentInChildren<TMP_Text>().text = i.ToString();
            shuffledButtons[i - 1].interactable = true;
            shuffledButtons[i - 1].image.color = new Color32(177, 220, 233, 255);
        }
    }

    public void pressButton(Button button)
    {
        int value = int.Parse(button.GetComponentInChildren<TMP_Text>().text);

        if (value - 1 == counter)
        {
            counter++;
            button.interactable = false;
            button.image.color = Color.green;

            if (counter == 10)
                StartCoroutine(presentResult(true));
        }
        else
        {
            StartCoroutine(presentResult(false));
        }
    }

    public IEnumerator presentResult(bool win)
    {
        if (!win)
        {
            // tô đỏ các nút sai
            foreach (var button in shuffledButtons)
            {
                button.image.color = Color.red;
                button.interactable = false;
                
            }
            yield return new WaitForSeconds(1.5f);
            RestartTheGame();
        }
        else
        {
            yield return new WaitForSeconds(1.5f);

            // 🔥 Tắt đúng cửa player đang mở
            if (currentDoor != null)
            {
                currentDoor.SetActive(false);
                currentDoor = null; // reset
            }
            
            MiniGame.SetActive(false);
            RestartTheGame();
        }
    }
}
