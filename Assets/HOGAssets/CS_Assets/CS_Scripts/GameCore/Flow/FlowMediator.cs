using System;
using System.Collections;
using UnityEngine;

//*************************************************************************
//@header       FlowMediator
//@abstract     The mediator of flow in game.
//@discussion   It is a father class.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public abstract class FlowMediator
    {
        protected FlowMediator()
        {

        }

        static FlowMediator _instance;

        public static FlowMediator Instance
        {
            get
            {
                return _instance;
            }
        }

        public abstract int Flow
        {
            get;
        }

        public static void SetTo<T>() where T : FlowMediator, new()
        {
            _instance = new T();
        }

        public abstract void Initialize();

        /// <summary>
        /// Begin specified flow.
        /// </summary>
        /// <param name="flow">The flow you want to start.</param>
        public abstract void Begin(int flow);

        /// <summary>
        /// Complete specified flow.
        /// </summary>
        /// <param name="flow">The flow you want to complete.</param>
        public abstract void Complete(int flow);

        /// <summary>
        /// Complete current flow and goto next flow.
        /// </summary>
        public abstract void Next();

        /// <summary>
        /// Interrupt and Exit flow.
        /// </summary>
        public abstract void BreakFlow(bool isNormalEnd);

        protected abstract IEnumerator FlowLoop();

        public virtual Coroutine StartCoroutine(IEnumerator iterator)
        {
            throw new NotImplementedException("FlowMediator.StartCoroutine(IEnumerator): Need to be implemented by derived class of FlowMediator");
        }

        public virtual void StopCoroutine(IEnumerator iterator)
        {
            throw new NotImplementedException("FlowMediator.StopCoroutine(IEnumerator): Need to be implemented by derived class of FlowMediator");
        }

        public virtual void StopAllCoroutines()
        {
            throw new NotImplementedException("FlowMediator.StopAllCoroutines(): Need to be implemented by derived class of FlowMediator");
        }

        public virtual void InvokeInSecs(Action method, float time)
        {
            throw new NotImplementedException("FlowMediator.InvokeInSecs(): Need to be implemented by derived class of FlowMediator");
        }
    }


}