using UnityEngine;
using System.IO;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;

//*************************************************************************
//@header       MusicController
//@abstract     Control the Music.
//@discussion   Add to the object as a component.Provide functions of music.
//@version      v1.0.0
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    [RequireComponent(typeof(MediaPlayer))]
    public class MusicController : MonoBehaviour
    {
        //static MusicController _instance;

        public MediaPlayer.FileLocation MusicLocation = MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder;
        public string BGMDirectoryName = "BGM";
        public string BGMusicName;
        public bool PlayAwake;

        MediaPlayer _mediaPlayer;
        string _targetFile;
        int _randomIndex;
        int _musicIndex;
        int _checkTimer;
        bool _canControl;
        List<string> _bgmTypes;
        List<string[]> _bgmList;
        int _currentMusicType;
        string _currentMusicName;
        string _pathBGM;
        bool _haveMusicFrame;

        //public static MusicController Instance
        //{
        //    get { return _instance; }
        //}
        //public MediaPlayer MyMediaPlayer
        //{
        //    get { return GetComponent<MediaPlayer>(); }
        //}
        public List<string> BGMTypes
        {
            get { return _bgmTypes; }
            set { _bgmTypes = value; }
        }
        public List<string[]> BGMList
        {
            get { return _bgmList; }
            set { _bgmList = value; }
        }
        public int CurrentMusicType
        {
            get { return _currentMusicType; }
            set { _currentMusicType = value; }
        }
        public string CurrentMusicName
        {
            get { return _currentMusicName; }
            set { _currentMusicName = value; }
        }
        public string PathBGM
        {
            get { return _pathBGM; }
            set { _pathBGM = value; }
        }
        public bool HaveMusicFrame
        {
            get { return _haveMusicFrame; }
            set { _haveMusicFrame = value; }
        }

        void Awake()
        {
            ReadBGM(MusicLocation);
            _mediaPlayer = GetComponent<MediaPlayer>();
        }

        void Start()
        {
            _canControl = true;
            if (PlayAwake)
                Initialize(BGMusicName);
        }

        void Update()
        {
            if (!_canControl)
                return;

            if (!Application.isEditor && Input.GetKeyDown(KeyCode.RightArrow))
                ChangeMusic();

            if (_mediaPlayer.Control.IsPlaying())
            {
                if (_mediaPlayer.Control.GetCurrentTimeMs() >= _mediaPlayer.Info.GetDurationMs())
                {
                    if (_haveMusicFrame)
                        EventManager.Instance.PublishEvent(EventTypeSet.NextMusic, new EventData(this));
                    else
                        AutoNextMusic();
                }
            }
        }

        void Initialize(string musicName)
        {
            _checkTimer++;
            _targetFile = CheckMusicPath(musicName);
            if (_targetFile != null)
            {
                PlayBGM(_targetFile);
                _checkTimer = 0;
            }
            else
            {
                if (_checkTimer > 10)
                    return;
                RandomMusicController();
                Initialize(_bgmList[_randomIndex][_musicIndex]);
            }
        }

        string CheckMusicPath(string musicName)
        {
            if (string.IsNullOrEmpty(musicName))
                return null;
            string path = string.Empty;
            string mediaName = string.Empty;
            int musicTypeNum = _bgmTypes.Count;
            for (int i = 0; i < musicTypeNum; i++)
            {
                path = _pathBGM + @"/" + _bgmTypes[i] + @"/" + musicName;
                mediaName = BGMDirectoryName + @"/" + _bgmTypes[i] + @"/" + musicName;
                if (File.Exists(path))
                {
                    if (_currentMusicType != 0)
                        _currentMusicType = i + 1;
                    _currentMusicName = musicName;
                    return mediaName;
                }
            }
            return null;
        }

        void ChangeMusic()
        {
            if (_currentMusicType == 0)
                NextRandomMusic();
            else
                NextMusic();
        }

        void ChangeMusicType(int typeNum)
        {
            if (typeNum <= 0 || typeNum > _bgmTypes.Count)
                return;
            _currentMusicName = _bgmList[typeNum - 1][0];
            _targetFile = _pathBGM + @"/" + _bgmTypes[typeNum - 1] + @"/" + _currentMusicName;
            PlayBGM(_targetFile);
        }

        void RandomMusicController()
        {
            _randomIndex = Random.Range(0, _bgmTypes.Count);
            _musicIndex = Random.Range(0, _bgmList[_randomIndex].Length);
        }

        #region PublicMethods
        /// <summary>
        /// Read all music from directory.
        /// </summary>
        public void ReadBGM(MediaPlayer.FileLocation musicLocation)
        {
            string pathLocation = string.Empty;
            switch (musicLocation)
            {
                case MediaPlayer.FileLocation.AbsolutePathOrURL:
                    break;
                case MediaPlayer.FileLocation.RelativeToProjectFolder:
#if !UNITY_WINRT_8_1
                    pathLocation = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                    pathLocation = pathLocation.Replace('\\', '/');
#endif
                    break;
                case MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder:
                    pathLocation = Application.streamingAssetsPath;
                    break;
                case MediaPlayer.FileLocation.RelativeToDataFolder:
                    pathLocation = Application.dataPath;
                    break;
                case MediaPlayer.FileLocation.RelativeToPeristentDataFolder:
                    pathLocation = Application.persistentDataPath;
                    break;
                default:
                    break;
            }

            _pathBGM = pathLocation + @"/" + BGMDirectoryName;

            try
            {
                _bgmTypes = FileManager.Instance.GetDirectoryName(_pathBGM);
                _bgmList = new List<string[]>();
                int typeTotalNum = _bgmTypes.Count;
                for (int i = 0; i < typeTotalNum; i++)
                {
                    string path = _pathBGM + @"/" + _bgmTypes[i];
                    string[] nameList = FileManager.Instance.GetFileName(path, "*.mp3").ToArray();
                    if (nameList.Length == 0)
                    {
                        _bgmTypes.RemoveAt(i);
                    }
                    else
                    {
                        _bgmList.Add(nameList);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Play next random music.
        /// </summary>
        public void NextRandomMusic()
        {
            RandomMusicController();
            _targetFile = CheckMusicPath(_bgmList[_randomIndex][_musicIndex]);
            PlayBGM(_targetFile);
        }

        /// <summary>
        /// Play next music.
        /// </summary>
        public void NextMusic()
        {
            List<string> tempList = new List<string>(_bgmList[_currentMusicType - 1]);
            _musicIndex = tempList.IndexOf(_currentMusicName);
            if (_musicIndex < tempList.Count - 1)
                _musicIndex++;
            else
                _musicIndex = 0;
            _targetFile = CheckMusicPath(tempList[_musicIndex]);
            PlayBGM(_targetFile);
        }

        /// <summary>
        /// Play previous music.
        /// </summary>
        public void PrevMusic()
        {
            List<string> tempList = new List<string>(_bgmList[_currentMusicType - 1]);
            _musicIndex = tempList.IndexOf(_currentMusicName);
            if (_musicIndex > 0)
                _musicIndex--;
            else
                _musicIndex = tempList.Count - 1;
            _targetFile = CheckMusicPath(tempList[_musicIndex]);
            PlayBGM(_targetFile);
        }

        /// <summary>
        /// Select next type of music.
        /// </summary>
        public void NextMusicType()
        {
            if (_currentMusicType < _bgmTypes.Count)
                _currentMusicType++;
            else
                _currentMusicType = 0;
            ChangeMusicType(_currentMusicType);
        }

        /// <summary>
        /// Select previous type of music.
        /// </summary>
        public void PrevMusicType()
        {
            if (_currentMusicType == 0)
                _currentMusicType = _bgmTypes.Count;
            else
                _currentMusicType--;
            ChangeMusicType(_currentMusicType);
        }

        /// <summary>
        /// Play music.
        /// </summary>
        /// <param name="targetFile"></param>
        public void PlayBGM(string targetFile)
        {
            if (targetFile == null)
                return;
            _mediaPlayer.m_VideoPath = targetFile;
            _mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, _mediaPlayer.m_VideoPath, true);
            SetMusicVolume(AudioManager.Instance.MusicVolume);
        }

        /// <summary>
        /// Stop music.
        /// </summary>
        public void StopBGM()
        {
            _mediaPlayer.CloseVideo();
        }
        #endregion PublicMethods

        /// <summary>
        /// Set volume of music
        /// </summary>
        /// <param name="volume"></param>
        public void SetMusicVolume(float volume)
        {
            _mediaPlayer.Control.SetVolume(volume);
        }

        /// <summary>
        /// Auto play next music
        /// </summary>
        public void AutoNextMusic()
        {
            // Change Music
            if (CurrentMusicType == 0)
            {
                NextRandomMusic();
            }
            else
            {
                NextMusic();
            }
        }
    }

}