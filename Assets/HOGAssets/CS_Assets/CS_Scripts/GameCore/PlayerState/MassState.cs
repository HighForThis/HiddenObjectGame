using System;
using UnityEngine;

//*************************************************************************
//@header       MassState
//@abstract     Create the state of Mass.
//@discussion   Depend on MachineState.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class MassState : MachineState
    {
        int _mass;

        public override void Run()
        {
            Initialize();

            MachineAPI.CmdMassSim(_mass);
        }

        public override void Setup(params object[] args)
        {
            _mass = (int)args[0];
        }

        #region OtherMethods
        void Initialize()
        {
            MachineAPI.Instance.Mode = MachineMode.MassSim;
            MachineAPI.Instance.Mass = _mass;
        }
        #endregion
    }
}
