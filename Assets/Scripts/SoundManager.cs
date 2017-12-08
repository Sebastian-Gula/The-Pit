using UnityEngine;

public class SoundManager : MonoBehaviour {

    private float musicVolume;
    private float soundValume;
    private float decreaseSounds;

    public static SoundManager Instance = null;

    public AudioSource efxSource;
    public AudioSource musicSource;

    public AudioClip camp;
    public AudioClip dungeons;
    public AudioClip boss;
  
    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;


    void Awake()
    {
        Instance = this;

        decreaseSounds = 0.2f;
        musicVolume = 0.5f;
        soundValume = 0.5f;
    }


    public void changeSoundVolume(float value)
    {
        soundValume = value;
        efxSource.volume = decreaseSounds * soundValume;
    }

    public void changeMusicVolume(float value)
    {
        musicVolume = value;
        musicSource.volume = decreaseSounds * musicVolume;
    }


    private void OnLevelWasLoaded(int index)
    {
        efxSource.volume = decreaseSounds * soundValume;
        musicSource.volume = decreaseSounds * musicVolume;

        if (index == 1)
        {      
            musicSource.pitch = 0.9f;
            musicSource.clip = camp;
        }
            
        else if (index == 2)
        {
            musicSource.pitch = 0.5f;
            musicSource.clip = dungeons;
        }

        musicSource.Play();       
    }


    public void PlaySingle(AudioClip clip)
    {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clip;
        efxSource.Play();
    }


    public void RandomizeSfx(AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }
}
