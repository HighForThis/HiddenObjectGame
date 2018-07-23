using UnityEngine;

//*************************************************************************
//@header       PlayerState
//@abstract     Base class for all state.
//@discussion   Depend on MonoBehaviour.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public abstract class PlayerState : MonoBehaviour
    {
        /// <summary>
        /// Pass parameters to PlayerState.
        /// </summary>
        /// <param name="args"></param>
        public abstract void Setup(params object[] args);

        public abstract void Run();

        /// <summary>
        /// Triggered when called PlayerEntity.ChangeState(PlayerState). Called before switch to new state.
        /// </summary>
        /// <param name="nextState">New PlayerState</param>
        public abstract void OnStateChanged(PlayerState nextState);
    }
}
