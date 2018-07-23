using System.Collections.Generic;
using UnityEngine;

//*************************************************************************
//@header       AudioManager
//@abstract     Manage the volume of all audioes.
//@version      v1.0.0
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class AudioManager
    {
        AudioManager()
        {
            _lstAudioController = new List<AudioController>();
            ReadSettingFromSysConfig();
        }

        #region Fields
        static AudioManager _instance;

        string[] _fileLines;
        //SysConfig _sysConfig;

        float _musicVolume;
        float _sfxVolume;
        float _masterVolume;

        List<AudioController> _lstAudioController;
        #endregion

        #region Properties
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AudioManager();
                return _instance;
            }
        }

        public float MasterVolume
        {
            get
            {
                return AudioListener.volume;
            }
            set
            {
                AudioListener.volume = value;
                //_sysConfig.MasterVolume = (int)(value * 100);
                FileManager.Instance.SetValue(_fileLines, "MasterVolume", (int)(value * 100), "AudioSetting");
            }
        }

        public float MusicVolume
        {
            get
            {
                return _musicVolume;
            }

            set
            {
                _musicVolume = value;
                AutoSetAllAudioVolume();
                //_sysConfig.MusicVolume = (int)(value * 100);
                FileManager.Instance.SetValue(_fileLines, "MusicVolume", (int)(value * 100), "AudioSetting");
            }
        }

        public float SfxVolume
        {
            get
            {
                return _sfxVolume;
            }

            set
            {
                _sfxVolume = value;
                AutoSetAllAudioVolume();
                //_sysConfig.SfxVolume = (int)(value * 100);
                FileManager.Instance.SetValue(_fileLines, "SfxVolume", (int)(value * 100), "AudioSetting");
            }
        }
        #endregion

        public void AddAudio(AudioController audioController)
        {
            _lstAudioController.Add(audioController);
        }

        public void RemoveAudio(AudioController audioController)
        {
            _lstAudioController.Remove(audioController);
        }

        public void ReadSettingFromSysConfig()
        {
            _fileLines = FileManager.Instance.ReadAllLines(Application.streamingAssetsPath + @"/Setting/AudioSetting.ini");
            MasterVolume = FileManager.Instance.GetIntValue(_fileLines, "MasterVolume", "AudioSetting", 0, 100) / 100f;
            MusicVolume = FileManager.Instance.GetIntValue(_fileLines, "MusicVolume", "AudioSetting", 0, 100) / 100f;
            SfxVolume = FileManager.Instance.GetIntValue(_fileLines, "SfxVolume", "AudioSetting", 0, 100) / 100f;
        }

        public void SaveAudioSetting()
        {
            try
            {
                FileManager.Instance.WriteAllLines(Application.streamingAssetsPath + @"/Setting/AudioSetting.ini", _fileLines);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                throw;
            }
        }

        void AutoSetAllAudioVolume()
        {
            for (int i = 0; i < _lstAudioController.Count; i++)
            {
                _lstAudioController[i].AutoSetAudioVolume();
            }
        }
    }

    public enum AudioType
    {
        Music,
        SFX,
        Announcer,
        Voice
    }

}