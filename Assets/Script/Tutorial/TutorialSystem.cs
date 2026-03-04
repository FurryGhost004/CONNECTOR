using UnityEngine;

public enum TutorialType
{
    Move,
    PickItemUP,
    SetUpItem,
    ThrowItem,
    Stealth,
    Finish,
    none

}
public static class TutorialSystem
{

    public static bool IsCompleted(TutorialType type)
    {
        return PlayerPrefs.GetInt("Tutorial_" + type.ToString(), 0) == 1;
    }
    public static void SetCompleted(TutorialType type)
    {
        PlayerPrefs.SetInt("Tutorial_" + type, 1);
        PlayerPrefs.Save();
    }

}
