using UnityEngine.UI;
using UIFrame;

//*************************************************************************
//@header       HOGUITop
//@abstract     Initial frame of Top.
//@discussion   Depend on BaseUIForm.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class HOGUITop : BaseUIForm
    {
        public Image Time_Bar;
        public Text Time_Value;
        public Text Score_Value;
        //public static int Score;

        void Awake()
        {

            //初始化窗体属性
            base.CurrentUIType.UIForms_Type = UIFormType.Normal;
            base.CurrentUIType.UIForms_ShowMode = UIFormShow.Normal;
            base.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;


            //Score = 0;
        }

        private void Update()
        {
            Time_Value.text = CalculateUnity.ChangeTotalTime(GameStart.Instance.TimeLeftSecond);

            Time_Bar.fillAmount = GameStart.Instance.TimeLeftSecond * 1f / HOGGameSettingGenerator.Instance.GameSetting.TrainTime;//GameStart.Instance.TrainTime;

            Score_Value.text = GameStart.Instance.CurrentGameRecorder.Score.ToString();
        }
    }
}
