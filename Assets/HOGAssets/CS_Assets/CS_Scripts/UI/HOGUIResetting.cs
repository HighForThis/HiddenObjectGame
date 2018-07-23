using DLMotion;
using UIFrame;

//*************************************************************************
//@header       HOGUIResetting
//@abstract     Initial frame of Resetting.
//@discussion   Depend on BaseUIForm.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class HOGUIResetting : BaseUIForm
    {
        private void Awake()
        {
            // 初始化窗体属性
            base.CurrentUIType.UIForms_Type = UIFormType.PopUp;
            base.CurrentUIType.UIForms_ShowMode = UIFormShow.ReverseChange;
            base.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;
            //base.Initialize();
            // 注册按钮事件
            RigisterButtonObjectEvent("Reset_Pause",
             p =>
             {
                 OpenUIForm("HOGUIPause");
                 GlobalApplication.IsPause = true;
                 CommandFunctions.IsPause = true;
                 DynaLinkHS.CmdServoOn();
                 SendMessage(HiddenObjectMessage.MsgStopGame, "", "True");
             }
             );
        }
    }
}
