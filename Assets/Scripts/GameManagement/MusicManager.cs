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
    int index = 0;
    // Start is called before the first frame update

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        songName = transform.GetChild(0).GetComponent<Text>();
        songLength = transform.GetChild(2).GetComponent<Slider>();
        getAllMusic();


    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        playMusic();


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
        if (audioSource.time == audioClips[index].length)
        {
            index++;

            if (index == audioClips.Count)
            {
                index = 0;
            }

            SetSong(audioClips[index]);

        }

        songLength.value = audioSource.time;

    }

    void SetSong(AudioClip audioClip)
    {
        songName.text = audioClips[index].name;
        audioSource.clip = audioClips[index];
        songLength.minValue = 0;
        songLength.maxValue = audioClip.length;
        audioSource.Play();
    }


}
