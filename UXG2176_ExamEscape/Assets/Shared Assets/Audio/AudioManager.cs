using UnityEditor;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioDatabase sounds;

    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private AudioSource loopSource;

    private AudioSource[] spatialPool;
    private int nextSpatial;

    [SerializeField] private float bgmVolume;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;
        bgmSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.loop = true;
        loopSource.playOnAwake = false;

        if (sounds != null)
        {
            PlayBgm(sounds.bgm);
        }

        CreateSpatialPool();
    }

    public void PlayBgm(AudioClip clip)
    {
        if (clip == null || bgmSource.clip == clip)
        {
            return;
        }
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBgm()
    {
        bgmSource.Stop();
    }

    public void PlaySfx(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip);
    }

    public void PlaySfxAt(AudioClip clip, Vector3 position, float minDistance = 2f, float maxDistance = 15f)
    {
        if (clip == null)
        {
            return;
        }

        AudioSource src = spatialPool[nextSpatial];
        nextSpatial = (nextSpatial + 1) % spatialPool.Length;

        src.transform.position = position;
        src.minDistance = minDistance;
        src.maxDistance = maxDistance;
        src.clip = clip;
        src.Play();
    }

    public void PlayLoop(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }
        loopSource.clip = clip;
        if (!loopSource.isPlaying)
        {
            loopSource.Play();
        }
    }

    public void StopLoop()
    {
        loopSource.Stop();
    }

    public void SetLoopPitch(float pitch)
    {
        loopSource.pitch = pitch;
    }

    private void CreateSpatialPool(int size = 8)
    {
        spatialPool = new AudioSource[size];
        for (int i = 0; i < size; i++)
        {
            var child = new GameObject("SFX_3D_" + i);
            child.transform.SetParent(transform);
            var src = child.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 1f;
            src.rolloffMode = AudioRolloffMode.Linear;
            spatialPool[i] = src;
        }
    }
}
