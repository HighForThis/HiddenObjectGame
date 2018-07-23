using UIFrame;

//*************************************************************************
//@header       HOGOutterMoveBoard
//@abstract     Initial frame of OutterMoveBoard.
//@discussion   Depend on BaseUIForm.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class HOGOutterMoveBoard : BaseUIForm
    {
        void Awake()
        {

            //初始化窗体属性
            base.CurrentUIType.UIForms_Type = UIFormType.Normal;
            base.CurrentUIType.UIForms_ShowMode = UIFormShow.Normal;
            base.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        }
    }
}
