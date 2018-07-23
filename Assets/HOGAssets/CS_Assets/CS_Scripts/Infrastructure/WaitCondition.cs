using System;
using System.Collections;
using UnityEngine;

//*************************************************************************
//@header       WaitCondition
//@abstract     Functions of waitting.
//@discussion   Provide functions to wait depend on condition.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class WaitCondition
    {
        public static IEnumerator WaitUntil(Func<bool> condition)
        {
            while (!condition())
            {
                yield return new WaitForEndOfFrame();
            }
        }

        public static IEnumerator WaitWhile(Func<bool> condition)
        {
            while (condition())
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}