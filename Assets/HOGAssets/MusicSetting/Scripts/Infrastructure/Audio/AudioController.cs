using UnityEngine;

//*************************************************************************
//@header       AudioController
//@abstract     Set the type of audio and control the volume.
//@discussion   Add to the object with the AudioSource as a component.
//@version      v1.0.0
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioController : MonoBehaviour
    {
        public AudioType Type;

        AudioSource _audioSource;

        #region Unity Messages
        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            AudioManager.Instance.AddAudio(this);
        }

        void OnEnable()
        {
            _audioSource.volume = GetVolumeByType();
        }

        void OnDestroy()
        {
            AudioManager.Instance.RemoveAudio(this);
        }
        #endregion

        public void AutoSetAudioVolume()
        {
            _audioSource.volume = GetVolumeByType();
        }

        float GetVolumeByType()
        {
            float volume;

            switch (Type)
            {
                case AudioType.Music:
                    volume = AudioManager.Instance.MusicVolume;
                    break;

                case AudioType.SFX:
                    volume = AudioManager.Instance.SfxVolume;
                    break;

                default:
                    volume = AudioManager.Instance.MasterVolume;
                    break;
            }

            return volume;
        }
    }
}