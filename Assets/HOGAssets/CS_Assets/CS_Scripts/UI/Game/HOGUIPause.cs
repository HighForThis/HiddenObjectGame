using DLMotion;
using UIFrame;
using UnityEngine.UI;

//*************************************************************************
//@header       HOGUIPause
//@abstract     Initial frame of Pause.
//@discussion   Depend on BaseUIForm.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class HOGUIPause : BaseUIForm
    {
        public Text Text_Message;

        void Awake()
        {
            base.CurrentUIType.UIForms_Type = UIFormType.PopUp;
            base.CurrentUIType.UIForms_ShowMode = UIFormShow.ReverseChange;
            base.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;

            //注册按钮事件
            RigisterButtonObjectEvent("Image_Quit",
               p =>
               {
                   CloseUIForm("HOGUIPause", true);
                   SendMessage(HiddenObjectMessage.MsgQuitGame, "", null);
               }
               );

            RigisterButtonObjectEvent("Image_Continue",
               p =>
               {
                   CloseUIForm("HOGUIPause", true);                   
                   SendMessage(HiddenObjectMessage.MsgStopGame, "", "False");
                   GlobalApplication.IsPause = false;
                   CommandFunctions.IsPause = false;
                   if (GameStart.Instance.CanStartGame)
                       GameStart.Instance.SendMachineCmd();
               }
               );
        }

        private void Start()
        {
            Text_Message.text = LanguageMgr.GetInstance().ShowText(HiddenObjectKey.QuitGameOrNot);
        }
    }
}
