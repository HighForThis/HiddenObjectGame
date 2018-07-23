using DLMotion;
using System.Collections;
using UIFrame;
using UnityEngine;
using UnityEngine.UI;

//*************************************************************************
//@header       HOGUICountDown
//@abstract     Initial frame of CountDown.
//@discussion   Depend on BaseUIForm.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class HOGUICountDown : BaseUIForm
    {
        public Image IconImage;
        public Sprite[] IconSprites;

        private void Awake()
        {
            // 初始化窗体属性
            base.CurrentUIType.UIForms_Type = UIFormType.PopUp;
            base.CurrentUIType.UIForms_ShowMode = UIFormShow.ReverseChange;
            base.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;
            //base.Initialize();
            // 注册按钮事件

            RigisterButtonObjectEvent("CountDown_Pause",
             p =>
             {
                 OpenUIForm("HOGUIPause");
                 GlobalApplication.IsPause = true;
                 DynaLinkHS.CmdServoOn();
                 SendMessage(HiddenObjectMessage.MsgStopGame, "", "True");
             }
             );
        }

        private void Start()
        {
            StartCoroutine(CountDownNum());
            //CloseUIForm(HiddenObjectPage.PromptTyingHandPage, true);
        }

        IEnumerator CountDownNum()
        {
            for (int i = 0; i < 3; i++)
            {
                IconImage.sprite = IconSprites[i];
                yield return StartCoroutine(MiscUtility.WaitOrPause(1f));
            }
            CloseUIForm(HiddenObjectPage.CountDownPage, true);
            OpenUIForm(HiddenObjectPage.StartTrainPage);
            //GlobalApplication.IsPause = false;
        }

    }
}
