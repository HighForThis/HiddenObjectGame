using UIFrame;
using UnityEngine;
using UnityEngine.UI;

//*************************************************************************
//@header       HOGUIPromptTyingHand
//@abstract     Initial frame of PromptTyingHand.
//@discussion   Depend on BaseUIForm.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class HOGUIPromptTyingHand : BaseUIForm
    {
        public Text MessageText;
        public Text ConfirmText;
        public float CountDownNum;
        CommandTimer cmdTimer;

        private void Awake()
        {
            // 初始化窗体属性
            base.CurrentUIType.UIForms_Type = UIFormType.PopUp;
            base.CurrentUIType.UIForms_ShowMode = UIFormShow.Normal;
            base.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;
            //base.Initialize();
            // 注册按钮事件
            RigisterButtonObjectEvent("Confirm",
                p =>
                {
                    print("Confirm");
                    FinishTimer();
                });
            RigisterButtonObjectEvent("PromptTyingHandBg",
                p =>
                {
                    print("PromptTyingHandBg");
                    FinishTimer();
                });
        }

        private void Start()
        {
            MessageText.text = LanguageMgr.GetInstance().ShowText(HiddenObjectKey.TieHand);
            cmdTimer = CommandTimer.CreateTimer("PromptTyingHandTimer");
            cmdTimer.StartTimer(CountDownNum, FinishTimer, ShowLeftTime);
            //SendMessage(HiddenObjectMessage.MsgStopGame, "", "True");
            //CloseUIForm(HiddenObjectPage.ResettingPage, true);
        }

        void ShowLeftTime(float timeNum)
        {
            ConfirmText.text = string.Format(LanguageMgr.GetInstance().ShowText(HiddenObjectKey.BeginTraining) + "({0})", Mathf.CeilToInt(timeNum).ToString());
        }

        void FinishTimer()
        {
            //GlobalApplication.IsPause = false;

            if (GameObject.Find("PromptTyingHandTimer"))
            {
                cmdTimer.destory();
            }
            CloseUIForm(HiddenObjectPage.PromptTyingHandPage,true);

            OpenUIForm(HiddenObjectPage.CountDownPage);

        }
    }
}
