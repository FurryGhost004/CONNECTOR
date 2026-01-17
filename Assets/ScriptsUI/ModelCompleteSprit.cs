using UnityEngine;
using UnityEngine.SceneManagement;

public class ModelCompleteSprit : MonoBehaviour
{
    public void OnModelComplete(int starsAquied)

    {
        if(MapManager.currModel==MapManager.UnlockedModels)
        {
            MapManager.UnlockedModels++;
            PlayerPrefs.SetInt("UnlockedModels", MapManager.UnlockedModels);
        }
        if (starsAquied > PlayerPrefs.GetInt("stars" + MapManager.currModel.ToString(), 0))
        {
            PlayerPrefs.SetInt("stars" + MapManager.currModel.ToString(), starsAquied);
        }
        SceneManager.LoadScene("Map");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
