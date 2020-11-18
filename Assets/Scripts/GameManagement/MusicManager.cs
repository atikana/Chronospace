using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MusicManager : MonoBehaviour
{

    AudioSource audioSource;
    Object[] audioFiles;
    List<AudioClip> audioClips = new List<AudioClip>();
    Text songName;
    Slider songLength;
    private bool musicIn;
    private bool musicStarted;
    int index = 0;
    // Start is called before the first frame update

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        songName = transform.GetChild(0).GetComponent<Text>();
        songLength = transform.GetChild(2).GetComponent<Slider>();
        getAllMusic();
        musicIn = false;
        musicStarted = false;

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (musicIn)
        {   
           if (!musicStarted) 
            { 
                musicStarted = true;
                audioSource.Play();
            }
            playMusic();
        }
    }

    public void StartMusic() 
    {
        musicIn = true;
    }

    void getAllMusic()
    {
        audioFiles = Resources.LoadAll("Music");

        for (int i = 0; i < audioFiles.Length; i++)
        {
            audioClips.Add((AudioClip)audioFiles[i]);
        }

        SetSong(audioClips[index]);


    }

    void playMusic()
    {
        if (!audioSource.isPlaying)
        {
            index++;

            if (index == audioClips.Count)
            {
                index = 0;
            }

            SetSong(audioClips[index]);
            audioSource.Play();

        }

        songLength.value = audioSource.time;
       


    }

    void SetSong(AudioClip audioClip)
    {
        songName.text = audioClips[index].name;
        audioSource.clip = audioClips[index];
        songLength.minValue = 0;
        songLength.maxValue = audioClip.length;
    }


}
