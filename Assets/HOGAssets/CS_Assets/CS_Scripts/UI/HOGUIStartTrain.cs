using System.Collections;
using UIFrame;

//*************************************************************************
//@header       HOGUIStartTrain
//@abstract     Initial frame of StartTrain.
//@discussion   Depend on BaseUIForm.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class HOGUIStartTrain : BaseUIForm
    {
        private void Awake()
        {
            // 初始化窗体属性
            base.CurrentUIType.UIForms_Type = UIFormType.PopUp;
            base.CurrentUIType.UIForms_ShowMode = UIFormShow.Normal;
            base.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;
            //base.Initialize();
            // 注册按钮事件

        }

        private void Start()
        {
            SendMessage(HiddenObjectMessage.MsgStopGame, "", "False");
            StartCoroutine(CountDownNum());
        }

        IEnumerator CountDownNum()
        {
            FlowMediator.Instance.Next();
            yield return StartCoroutine(MiscUtility.WaitOrPause(1f));
            CloseUIForm(HiddenObjectPage.StartTrainPage, true);            
        }
    }
}
