using System.Collections;
using UnityEngine;

//*************************************************************************
//@header       MiscUtility
//@abstract     IEnumerator functions.
//@discussion   Provide functions to wait or pause in the game.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public static class MiscUtility
    {
        public static IEnumerator WaitOrPause(float seconds)
        {
            float time = 0;
            while (time < seconds)
            {
                // This won't wait for one frame, it will wait for current frame.
                yield return new WaitForEndOfFrame();

                // This will wait at least for on frame.
                yield return FlowMediator.Instance.StartCoroutine(Pause());

                time += Time.deltaTime;
            }
        }

        static IEnumerator Pause()
        {
            if (GlobalApplication.IsPause)
            {
                yield return FlowMediator.Instance.StartCoroutine(WaitCondition.WaitWhile(() => GlobalApplication.IsPause));
            }
        }
    }
}