using System;
using UnityEngine;

//*************************************************************************
//@header       AssistLTState
//@abstract     Create the state of Active.
//@discussion   Depend on MachineState.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class AssistLTState : MachineState
    {
        int _assisLTTorque;

        public override void Run()
        {
            Initialize();

            MachineAPI.CmdAssistLT(_assisLTTorque);
        }

        public override void Setup(params object[] args)
        {
            _assisLTTorque = (int)args[0];
        }

        #region OtherMethods
        void Initialize()
        {
            MachineAPI.Instance.Mode = MachineMode.AssistLT;
            MachineAPI.Instance.Torque = _assisLTTorque;
        }
        #endregion
    }
}
