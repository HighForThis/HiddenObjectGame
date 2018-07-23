using UnityEngine.UI;
using UIFrame;
using System.Collections;
using UnityEngine;

//*************************************************************************
//@header       HOGUILevelUp
//@abstract     Initial frame of LevelUp.
//@discussion   Depend on BaseUIForm.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class HOGUILevelUp : BaseUIForm
    {
        void Awake()
        {
            base.CurrentUIType.UIForms_Type = UIFormType.PopUp;
            base.CurrentUIType.UIForms_ShowMode = UIFormShow.ReverseChange;
            base.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;
        }

        private void Start()
        {
            StartCoroutine(CloseLevelUp());
        }

        IEnumerator CloseLevelUp()
        {
            yield return new WaitForSeconds(2.0f);
            CloseUIForm(HiddenObjectPage.HOGUILevelUp, true);
        }
    }
}
