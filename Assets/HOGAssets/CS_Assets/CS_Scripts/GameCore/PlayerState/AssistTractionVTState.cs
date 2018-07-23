using UnityEngine;

//*************************************************************************
//@header       AssistTractionVTState
//@abstract     Create the state of Assist.
//@discussion   Depend on MachineState.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class AssistTractionVTState : MachineState
    {
        int _assistVTTorque;

        public override void Run()
        {
            Initialize();
        }

        public override void Setup(params object[] args)
        {
            _assistVTTorque = (int)args[0];
        }

        public override void OnStateChanged(PlayerState nextState)
        {
            base.OnStateChanged(nextState);
            EventManager.Instance.RemoveListener(EventTypeSet.TargetCreated, OnTargetCreated);
        }

        #region OtherMethods
        void Initialize()
        {
            MachineAPI.Instance.Mode = MachineMode.AssistTractionVT;
            MachineAPI.Instance.Torque = _assistVTTorque;
            EventManager.Instance.AddListener(EventTypeSet.TargetCreated, OnTargetCreated);
        }

        void OnTargetCreated(EventData eventData)
        {
            Vector3 pos = (Vector3)eventData.Data;
            // Send command with given value
            MachineAPI.Instance.MoveTo(pos, 0, _assistVTTorque);
        }
        #endregion
    }
}
