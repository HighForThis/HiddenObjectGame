using System.Collections;
using UIFrame;
using UnityEngine;
using UnityEngine.UI;

//*************************************************************************
//@header       HOGUIGameOver
//@abstract     Initial frame of GameOver.
//@discussion   Depend on BaseUIForm.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class HOGUIGameOver : BaseUIForm
    {
        public Text LostTimeTitle;
        public Text lostTimeNum;
        public Text TotalScoreTitle;
        public Text TotalScoreNum;
        public Text ExitMessage;

        private void Awake()
        {
            // 初始化窗体属性
            base.CurrentUIType.UIForms_Type = UIFormType.PopUp;
            base.CurrentUIType.UIForms_ShowMode = UIFormShow.Normal;
            base.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;

            // 注册按钮事件

            RigisterButtonObjectEvent("GameOverBtn",
                p =>
                {
                    GameStart.Instance.CloseGameAndUI(true);
                    //FlowMediator.Instance.BreakFlow(true);
                });
        }

        private void Start()
        {
            LostTimeTitle.text = LanguageMgr.GetInstance().ShowText(HiddenObjectKey.LostTime);
            lostTimeNum.text = GameStart.Instance.CurrentGameRecorder.LostTime.ToString();
            TotalScoreTitle.text = LanguageMgr.GetInstance().ShowText(HiddenObjectKey.FinalScore);
            TotalScoreNum.text = GameStart.Instance.CurrentGameRecorder.Score.ToString();
            ExitMessage.text = LanguageMgr.GetInstance().ShowText(HiddenObjectKey.ExitMessage);
        }
    }
}
