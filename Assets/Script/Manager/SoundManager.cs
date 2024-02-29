using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource soundEffectSource;
    public AudioClip SoundClip; // Inspector에서 효과음 파일을 할당합니다.

    public void PlaySoundEffect()
    {
        soundEffectSource.PlayOneShot(SoundClip);
    }
}
