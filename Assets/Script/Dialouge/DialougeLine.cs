using UnityEngine;

[System.Serializable]
public class DialougeLine
{
    public string charaterName;
    public Sprite charaterPortrait;
    [TextArea(2, 4)]
    public string content;
}
