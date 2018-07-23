using DLMotion;

//*************************************************************************
//@header       MachineState
//@abstract     Do something when create state.
//@discussion   Depend on PlayerState.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public abstract class MachineState : PlayerState
    {
        public override void Setup(params object[] args)
        {
            // Do nothing here.
        }

        public override void OnStateChanged(PlayerState nextState)
        {
            DynaLinkHS.CmdServoOff();
        }
    }
}
