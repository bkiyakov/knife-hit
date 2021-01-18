using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
[CreateAssetMenu(fileName = "New Stage", menuName = "Stage", order = 1)]
public class Stage : ScriptableObject
{
    public Sprite Background;
    public LogRoute route;
    public int AppleChance;
    public int LogHealth;
    public int KnifeSpawnMin;
    public int KnifeSpawnMax;
    public AudioClip StageMusic;
    public AudioClip LogHittedSound;
    public AudioClip LogBreakingSound;
    public Sprite LogSprite;
    public Sprite LogPartSprite;
    public bool IsBoss;
    public string BossName;
}
