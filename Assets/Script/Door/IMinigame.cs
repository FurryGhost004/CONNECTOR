using System;

public interface IMinigame
{
    void StartMinigame();
    void PauseGame();
    void ResumeGame();
    void SetWinCallback(Action callback);
}