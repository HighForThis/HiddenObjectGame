using System;
using UnityEngine;

//*************************************************************************
//@header       ResistState
//@abstract     Create the state of Resist.
//@discussion   Depend on MachineState.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class ResistState : MachineState
    {
        int _resistTorque;

        public override void Run()
        {
            Initialize();

            MachineAPI.CmdResistLT(_resistTorque);
        }

        public override void Setup(params object[] args)
        {
            _resistTorque = (int)args[0];
        }

        #region OtherMethods
        void Initialize()
        {
            MachineAPI.Instance.Mode = MachineMode.ResistLT;
            MachineAPI.Instance.Torque = _resistTorque;
        }
        #endregion
    }
}
