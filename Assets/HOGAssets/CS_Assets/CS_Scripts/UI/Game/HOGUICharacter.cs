using UnityEngine.UI;
using UIFrame;

//*************************************************************************
//@header       HOGUICharacter
//@abstract     Initial frame of Character.
//@discussion   Depend on BaseUIForm.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class HOGUICharacter : BaseUIForm
    {
        public Text Level_Text;
        public Text Exp_Text;
        public Image Exp_Bar;
        public Image Distance_Bar;
        public Image LevelIcon;
        public HOGLevelIcon PrefabLevelIcon;

        //public static int Level_Value = 0;
        //public static int Current_Exp = 0;
        //public static int Total_Exp = 0;



        void Awake()
        {
            base.CurrentUIType.UIForms_Type = UIFormType.Normal;
            base.CurrentUIType.UIForms_ShowMode = UIFormShow.Normal;
            base.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        }

        private void Update()
        {
            Level_Text.text = GameStart.Instance.CurrentGameRecorder.PlayerLevel.ToString();
            Exp_Text.text = GameStart.Instance.CurrentGameRecorder.PlayerExp + "/" + GameStart.Instance.NextLevelExp;
            Exp_Bar.fillAmount = (GameStart.Instance.CurrentGameRecorder.PlayerExp - GameStart.Instance.CurrentLevelExp) * 1f / (GameStart.Instance.NextLevelExp - GameStart.Instance.CurrentLevelExp);
            Distance_Bar.fillAmount = GameStart.Instance.TargetDistanceProportion;
            LevelIcon.sprite = PrefabLevelIcon.LevelIcon[GameStart.Instance.CurrentGameRecorder.PlayerLevel - 1];
        }
    }
}
