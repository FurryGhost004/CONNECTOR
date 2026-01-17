using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager: MonoBehaviour
{
    public ModelObject[] modelObjects;
    public Sprite goldenStarSprite;
    public static int currModel;
    public static int UnlockedModels;

    public void OnClickModel(int ModelNum)
    {
        currModel = ModelNum;

        // Nếu là model 0 (model đầu tiên) thì load scene "Model 1"
        if (ModelNum == 0)
        {
            SceneManager.LoadScene("Model01");
        }
        else
        {
            // Các model khác vẫn load GameScene để test
            SceneManager.LoadScene("GameScene");
        }
    }

    void Start()
    {
        UnlockedModels = PlayerPrefs.GetInt("UnlockedModels", 0);
        for (int i = 0; i < modelObjects.Length; i++)
        {
            modelObjects[i].modelButton.interactable = true;
            int stars = PlayerPrefs.GetInt("stars" + i.ToString(), 0);
            for (int j = 0; j < stars; j++)
            {
                modelObjects[i].stars[j].sprite = goldenStarSprite;
            }
        }
    }
    public void OnClickBack()
    {
        PlayerPrefs.SetInt("ReturnToLevelSelect", 1); // báo hiệu quay về Level Select
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");

    }

}