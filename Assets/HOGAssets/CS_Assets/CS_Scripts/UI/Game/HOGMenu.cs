using UIFrame;
using DLMotion;

//*************************************************************************
//@header       HOGMenu
//@abstract     Provide functions of three buttons.
//@discussion   Depend on BaseUIForm.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class HOGMenu : BaseUIForm
    {
        void Awake()
        {
            base.CurrentUIType.UIForms_Type = UIFormType.Normal;
            base.CurrentUIType.UIForms_ShowMode = UIFormShow.Normal;
            base.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

            //注册按钮事件
            RigisterButtonObjectEvent("Btn_Music",
               p =>
               {
                   // print("音乐按钮");
                   GlobalApplication.IsPause = true;
                   OpenUIForm(HiddenObjectPage.MusicConsole);
                   DynaLinkHS.CmdServoOn();
                   SendMessage(HiddenObjectMessage.MsgStopGame, "", "True");
               }
               );

            RigisterButtonObjectEvent("Btn_Pause",
               p =>
               {
                   GlobalApplication.IsPause = true;
                   OpenUIForm("HOGUIPause");
                   DynaLinkHS.CmdServoOn();
                   SendMessage(HiddenObjectMessage.MsgStopGame, "", "True");
               }
               );

            RigisterButtonObjectEvent("Btn_Next",
               p =>
               {
                   print("下一步按钮");
                   //CloseUIForm("Task");
                   OnClickNext();
               }
               );
        }

        void OnClickNext()
        {
            //if (InAssessmentRecord.Instance.CanEndRecord)
            //{
            //    FlowMediator.Instance.BreakFlow(true);
            //}
            //else
            //{
            //    OpenUIForm(AsPageString.RangeMessagePage);
            //}
            //FlowMediator.Instance.BreakFlow(true);
            //EventManager.Instance.PublishEvent(EventTypeSet.NextFlow, new EventData(this));
            FlowMediator.Instance.Next();
        }

    }

}