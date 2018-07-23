using UnityEngine;
using UnityEngine.UI;
using UIFrame;

//*************************************************************************
//@header       HOGMusicConsole
//@abstract     Control UI of MusicSettingPage
//@discussion   Add to the object as a component.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class HOGMusicConsole : BaseUIForm
    {

        public Image VolumeProgressBar;
        public Image SoundProgressBar;
        public AudioSource SfxAudio;
        public SelectionController MusicTypeSelection;
        public SelectionController MusicNameSelection;


        MusicController _musicController;

        void Awake()
        {
            //初始化窗体属性
            base.CurrentUIType.UIForms_Type = UIFormType.PopUp;
            base.CurrentUIType.UIForms_ShowMode = UIFormShow.ReverseChange;
            base.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;

            _musicController = GameObject.Find(HiddenObjectString.BackgroundMusic).GetComponent<MusicController>();

            //注册按钮事件
            RigisterButtonObjectEvent("Btn_Console_Close",
              p =>
              {
                  print("Btn_Volume_Plus");
                  SendMessage(HiddenObjectMessage.MsgStopGame, "", "False");
                  AudioManager.Instance.SaveAudioSetting();
                  CloseUIForm(HiddenObjectPage.MusicConsole, true);
                  //DynaLinkHS.CmdAssistLT(70);
                  GlobalApplication.IsPause = false;
                  GameStart.Instance.SendMachineCmd();
              }
              );

            RigisterButtonObjectEvent("Btn_Volume_Plus",
             p =>
             {
                 //print("Btn_Volume_Plus");
                 OnClickMusicVolumePlus();
             }
             );

            RigisterButtonObjectEvent("Btn_Volume_Minus",
             p =>
             {
                 //print("Btn_Volume_Minus");
                 OnClickMusicVolumeMinus();
             }
             );

            RigisterButtonObjectEvent("Btn_Sound_Plus",
             p =>
             {
                 //print("Btn_Sound_Plus");
                 OnClickSfxVolumePlus();
             }
             );

            RigisterButtonObjectEvent("Btn_Sound_Minus",
             p =>
             {
                 //print("Btn_Sound_Minus");
                 OnclickSfxVolumeMinus();
             }
             );

            RigisterButtonObjectEvent("Btn_Type_Right",
             p =>
             {
                 //print("Btn_Type_Right");
                 OnClickMusicTypeRight();
             }
             );

            RigisterButtonObjectEvent("Btn_Type_Left",
             p =>
             {
                 //print("Btn_Type_Left");
                 OnClickMusicTypeLeft();
             }
             );

            RigisterButtonObjectEvent("Btn_BGMusic_Right",
             p =>
             {
                 //print("Btn_BGMusic_Right");
                 OnClickBGMusicRight();
             }
             );

            RigisterButtonObjectEvent("Btn_BGMusic_Left",
             p =>
             {
                 //print("Btn_BGMusic_Left");
                 OnClickBGMusicLeft();
             }
             );

        }

        void Start()
        {
            EventManager.Instance.AddListener(EventTypeSet.NextMusic, OnEventNextMusic);
            if (_musicController.CurrentMusicType == 0)
                MusicTypeSelection.CurrentName = HiddenObjectString.Random;
            else
                MusicTypeSelection.CurrentName = _musicController.BGMTypes[_musicController.CurrentMusicType - 1];
            MusicNameSelection.CurrentName = _musicController.CurrentMusicName.Split('.')[0];
            MusicTypeSelection.TurnToPageOne();
            MusicNameSelection.TurnToPageOne();
            _musicController.HaveMusicFrame = true;
        }

        private void OnEnable()
        {
            VolumeProgressBar.fillAmount = AudioManager.Instance.MusicVolume;
            SoundProgressBar.fillAmount = AudioManager.Instance.SfxVolume;
        }

        void OnDestroy()
        {
            _musicController.HaveMusicFrame = false;
            EventManager.Instance.RemoveListener(EventTypeSet.NextMusic, OnEventNextMusic);
        }

        void OnclickSfxVolumeMinus()
        {
            SoundProgressBar.fillAmount -= 0.2f;
            AudioManager.Instance.SfxVolume = SoundProgressBar.fillAmount;

            SfxAudio.Play();
        }

        void OnClickSfxVolumePlus()
        {
            SoundProgressBar.fillAmount += 0.2f;
            AudioManager.Instance.SfxVolume = SoundProgressBar.fillAmount;

            SfxAudio.Play();
        }

        void OnClickMusicVolumeMinus()
        {
            VolumeProgressBar.fillAmount -= 0.2f;
            AudioManager.Instance.MusicVolume = VolumeProgressBar.fillAmount;
            //_musicController.MyMediaPlayer.Control.SetVolume(VolumeProgressBar.fillAmount);
            _musicController.SetMusicVolume(VolumeProgressBar.fillAmount);
        }

        void OnClickMusicVolumePlus()
        {
            VolumeProgressBar.fillAmount += 0.2f;
            AudioManager.Instance.MusicVolume = VolumeProgressBar.fillAmount;
            //_musicController.MyMediaPlayer.Control.SetVolume(VolumeProgressBar.fillAmount);
            _musicController.SetMusicVolume(VolumeProgressBar.fillAmount);
        }

        void OnClickMusicTypeLeft()
        {
            // Chnage Music
            _musicController.PrevMusicType();

            // Change UI
            if (_musicController.CurrentMusicType == 0)
                MusicTypeSelection.CurrentName = HiddenObjectString.Random;
            else
            {
                MusicTypeSelection.CurrentName = _musicController.BGMTypes[_musicController.CurrentMusicType - 1];
                MusicNameSelection.CurrentName = _musicController.CurrentMusicName.Split('.')[0];
                MusicNameSelection.TurnToPageOne();
            }
            MusicTypeSelection.OnClickPreviousPage();
        }

        void OnClickMusicTypeRight()
        {
            // Change Music
            _musicController.NextMusicType();

            // Change UI
            if (_musicController.CurrentMusicType == 0)
                MusicTypeSelection.CurrentName = HiddenObjectString.Random;
            else
            {
                MusicTypeSelection.CurrentName = _musicController.BGMTypes[_musicController.CurrentMusicType - 1];
                MusicNameSelection.CurrentName = _musicController.CurrentMusicName.Split('.')[0];
                MusicNameSelection.TurnToPageOne();
            }
            MusicTypeSelection.OnClickNextPage();
        }

        void OnClickBGMusicLeft()
        {
            // Change Music
            if (_musicController.CurrentMusicType == 0)
            {
                _musicController.NextRandomMusic();
            }
            else
            {
                _musicController.PrevMusic();
            }

            // Change UI
            MusicNameSelection.CurrentName = _musicController.CurrentMusicName.Split('.')[0];
            MusicNameSelection.OnClickPreviousPage();
        }

        void OnClickBGMusicRight()
        {
            // Change Music
            _musicController.AutoNextMusic();

            // Change UI
            MusicNameSelection.CurrentName = _musicController.CurrentMusicName.Split('.')[0];
            MusicNameSelection.OnClickNextPage();
        }

        void OnEventNextMusic(EventData eventData)
        {
            OnClickBGMusicRight();
        }
    }
}
