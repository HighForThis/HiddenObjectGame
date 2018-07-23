using UnityEngine;

//*************************************************************************
//@header       LoadBackgroundMusic
//@abstract     Create GameObject BackgroundMusic.
//@discussion   The method CreateBackgroundMusic must be used in Awake when Loading Game Scene.
//@version      v1.0.0
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//**************************************************************************
namespace FZ.HiddenObjectGame
{
    public class LoadBackgroundMusic
    {
        static LoadBackgroundMusic _instance;

        public static LoadBackgroundMusic Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LoadBackgroundMusic();
                return _instance;
            }
        }

        /// <summary>
        /// The path of resource must be 'MusicSettingObjects/BackgroundMusic'
        /// </summary>
        public void CreateBackgroundMusic()
        {
            //AudioManager.Instance.ReadSettingFromSysConfig();
            GameObject backgroundMusic = Object.Instantiate(Resources.Load("Objects/BackgroundMusic", typeof(GameObject))) as GameObject;
            backgroundMusic.name = "BackgroundMusic";
        }
    }
}