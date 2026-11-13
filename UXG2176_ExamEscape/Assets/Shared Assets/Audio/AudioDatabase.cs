using UnityEngine;

[CreateAssetMenu(menuName = "Game/Audio Database")]
public class AudioDatabase : ScriptableObject
{
    [Header("Music")]
    public AudioClip bgm;

    [Header("Player")]
    public AudioClip footsteps;

    [Header("Puzzles")]
    public AudioClip select;
    public AudioClip swap;
    public AudioClip place;
    public AudioClip throwObj;
    public AudioClip pickup;
    public AudioClip unlock;
    public AudioClip wrong;
    public AudioClip correct;
    public AudioClip flashlightBtn;
    public AudioClip btnPress;
    public AudioClip laserPowered;
    public AudioClip laserUnpowered;
    public AudioClip mirrorTurn;
    public AudioClip gameOver;
    public AudioClip win;
}
