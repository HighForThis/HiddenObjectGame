using UnityEngine;

//*************************************************************************
//@header       PlayerStateMachine
//@abstract     Initial state of player.
//@discussion   Add to the object as a component.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class PlayerStateMachine : MonoBehaviour
    {
        PlayerState _state;

        public PlayerState State
        {
            get
            {
                return _state;
            }
        }

        public void Run()
        {
            _state.Run();
        }

        public void ChangeState<T>(params object[] args) where T : PlayerState
        {
            PlayerState nextState = gameObject.AddComponent<T>();
            if (_state != null)
            {
                _state.OnStateChanged(nextState);
                Destroy(_state);
                _state = nextState;
            }
            else
                _state = nextState;

            _state.Setup(args);
        }

        #region Unity Messages
        void Awake()
        {
            _state = null;
        }
        #endregion End Unity Messages

    }
}
