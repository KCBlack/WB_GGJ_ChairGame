using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum sounds {Footstep, candle, monsterRawr, terminator}


public class AudioManager : MonoBehaviour
{
    public static AudioManager am;
    [SerializeField] AudioClip frontEndSong;
    [SerializeField] AudioClip gameMusic;
    [SerializeField] AudioClip OutsideMusic;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfx;
    [SerializeField] AudioMixer masterMixer;
    bool isVolumeSet;
    public bool isFadingOut;
    public Coroutine fadeOut;
    public AudioClip buttonSound;

    private void Awake()
    {
        if (am == null)
        {
            am = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        SceneManager.sceneLoaded += PlaySong;
    }

    private void Start()
    {
        if (am == this)
        {
            //SetMusicVolume(PlayerPrefs.GetFloat("musicVolume", 80f));
            //SetSfxVolume(PlayerPrefs.GetFloat("sfxVolume", 80f));
            isVolumeSet = true;
        }
    }

    void PlaySong(Scene _scene, LoadSceneMode _mode)
    {
        StartCoroutine(PlaySong());
    }

    IEnumerator PlaySong()
    {
        yield return new WaitUntil(() => isVolumeSet);
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                PlaySong(frontEndSong);
                sfx.enabled = false;
                break;
            case 1:
                musicSource.Stop();
                if (fadeOut != null)
                {
                    StopCoroutine(fadeOut);
                    isFadingOut = false;
                    fadeOut = null;
                }
                StartCoroutine(TurnMusicBackUp());
                break;
            case 2:
                musicSource.Stop();
                if (fadeOut != null)
                {
                    StopCoroutine(fadeOut);
                    isFadingOut = false;
                    fadeOut = null;
                }
                StartCoroutine(TurnMusicBackUp());
                break;
            case 3:
            case 4:
                
                break;
            default:
                break;
        }
    }

    public IEnumerator FadeOut()
    {
        isFadingOut = true;
        float timer = 0f;
        float fadeOutTime = 2f;
        float preferredVolume = PlayerPrefs.GetFloat("musicVolume");

        while (timer < fadeOutTime && isFadingOut)
        {
            timer += Time.deltaTime;
            SetMixerVolume("musicVolume", preferredVolume * (1f - timer / fadeOutTime));
            yield return null;
        }

        isFadingOut = false;
        fadeOut = null;
    }

    IEnumerator TurnMusicBackUp()
    {
        yield return new WaitUntil(() => isFadingOut == false && fadeOut == null);
        //PlaySong(ambienceLibrary);
        //SetMixerVolume("musicVolume", PlayerPrefs.GetFloat("musicVolume"));
        sfx.enabled = true;
    }

    void PlaySong(AudioClip _clip)
    {
        musicSource.Stop();
        musicSource.clip = _clip;
        musicSource.PlayOneShot(_clip);
    }

    public void SetMusicVolume(float _volume)
    {
        PlayerPrefs.SetFloat("musicVolume", _volume);
        SetMixerVolume("musicVolume", _volume);
    }

    public void SetSfxVolume(float _volume)
    {
        PlayerPrefs.SetFloat("sfxVolume", _volume);
        SetMixerVolume("sfxVolume", _volume);
    }

    void SetMixerVolume(string _keyword, float _volume)
    {
        masterMixer.SetFloat(_keyword, 80f / Mathf.Log10(101f) * Mathf.Log10(_volume + 1f) - 80f);
    }

    private void Update()
    {
        if (musicSource.isPlaying == false)
        {
            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 0:
                    PlaySong(frontEndSong);
                    musicSource.enabled = true;
                    break;
                case 1:
                    PlaySong(gameMusic);
                    musicSource.enabled = true;
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                    PlaySong(OutsideMusic);
                    musicSource.enabled = true;
                    break;
                default:
                    break;
            }
        }
    }
}
