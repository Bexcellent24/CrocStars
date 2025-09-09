using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    private AudioSource soundSource;
    private AudioSource musicSource;

    public List<AudioClip> sceneMusicClips; // Add your music clips in the Inspector, order them by scene name or use a dictionary if preferred.

    private void Awake()
    {
        soundSource = GetComponent<AudioSource>();
        musicSource = transform.GetChild(0).GetComponent<AudioSource>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Register event to handle scene changes
        }
        else
        {
            Destroy(gameObject);
        }

        ChangeMusicVolume(0);
        ChangeSoundVolume(0);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        // Retrieve the clip for the scene, based on the scene name or index
        AudioClip newClip = GetClipForScene(sceneName);

        if (newClip != null && musicSource.clip != newClip) // Only change if there's a new clip
        {
            musicSource.clip = newClip;
            musicSource.Play();
        }
    }

    private AudioClip GetClipForScene(string sceneName)
    {
        // You can use a dictionary if you'd rather have more control over the associations
        if (sceneName == "MainMenu")
            return sceneMusicClips[4]; // Assuming sceneMusicClips[0] is the clip for Level1
        else if (sceneName == "Lobby")
            return sceneMusicClips[0];
        else if (sceneName == "Tutorial")
            return sceneMusicClips[0];
        else if (sceneName == "Jumper")
            return sceneMusicClips[1];
        else if (sceneName == "Runner")
            return sceneMusicClips[2];
        else if (sceneName == "Platformer")
            return sceneMusicClips[3];
        
        return null; // Fallback if no specific music is set
    }

    public void PlaySound(AudioClip _sound)
    {
        soundSource.PlayOneShot(_sound);
    }

    public void ChangeSoundVolume(float _change)
    {
        ChangeSourceVolume(1, "soundVolume", _change, soundSource);
    }

    public void ChangeMusicVolume(float _change)
    {
        ChangeSourceVolume(0.3f, "musicVolume", _change, musicSource);
    }

    private void ChangeSourceVolume(float baseVolume, string volumeName, float change, AudioSource source)
    {
        float currentVolume = PlayerPrefs.GetFloat(volumeName, 1);
        currentVolume += change;

        if (currentVolume > 1) currentVolume = 0;
        else if (currentVolume < 0) currentVolume = 1;

        float finalVolume = currentVolume * baseVolume;
        source.volume = finalVolume;
        PlayerPrefs.SetFloat(volumeName, currentVolume);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
