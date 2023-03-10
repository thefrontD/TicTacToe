using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private List<AudioSource> sePlayer;
    [SerializeField] private AudioSource bgmPlayer;

    public float masterVolumeSFX = 1f;
    public float masterVolumeBGM = 1f;

    [SerializeField] private StringSoundDictionary seDictionary; //SoundEffect Dictionary
    [SerializeField] private StringSoundDictionary bgmDictionary; //BGM Dictionary

    // SE Playing
    public void PlaySE(string name, float volume = 1f)
    {
        if (seDictionary.ContainsKey(name) == false)
        {
            Debug.Log(name + " is not Contained audioClipsDic");
            return;
        }

        for (int i = 0; i < sePlayer.Count; i++)
        {
            if (!sePlayer[i].isPlaying)
            {
                sePlayer[i].PlayOneShot(seDictionary[name], volume * masterVolumeSFX);
                return;
            }
        }
        
        Debug.Log("All Source is Playing Now");
    }

    //BGM Playing
    public void PlayBGM(string name, float volume = 1f)
    {
        bgmPlayer.loop = true; //BGM 사운드이므로 루프설정
        bgmPlayer.volume = volume * masterVolumeBGM;

        if (bgmDictionary.ContainsKey(name) == false)
        {
            Debug.Log(name + " is not Contained audioClipsDic");
            return;
        }
        bgmPlayer.clip = bgmDictionary[name];
        bgmPlayer.Play();
    }
}