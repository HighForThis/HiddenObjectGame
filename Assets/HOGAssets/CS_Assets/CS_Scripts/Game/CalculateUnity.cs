using UnityEngine;

//*************************************************************************
//@header       CalculateUnity
//@abstract     Provide the functions about calculation.
//@discussion   Static class.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public static class CalculateUnity
    {
        // The multiple of score you can get
        static int[] PreciousMultiples = { 2, 3, 5, 5, 10, 20, 30, 1 };
        // The rate of multiple
        static float[] PreciousRates = { 0.2f, 0.1f, 0.05f, 0.01f, 0.005f, 0.003f, 0.001f, 0.631f };

        /// <summary>
        /// x, y is position of Screen. z is distance to screen. Camera is the first enabled tagged "MainCamera".
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns>Screen point position</returns>
        public static Vector3 WorldPositionToScreenPoint(Vector3 worldPosition)
        {
            Vector3 screenViewport = Camera.main.WorldToViewportPoint(worldPosition);
            Vector3 screenPos = new Vector3(Screen.currentResolution.width * screenViewport.x, Screen.currentResolution.height * screenViewport.y, screenViewport.z);
            return screenPos;
        }

        public static string ChangeTotalTime(int totalTime)
        {
            string totalStringTime;
            string leftString;
            string middleString;

            if (totalTime % 60 < 10)
                middleString = ":0";
            else
                middleString = ":";
            if (totalTime < 60)
                //totalStringTime = "00:" + totalTime.ToString();
                leftString = "00";
            else
            {
                leftString = totalTime / 60 + "";
                //if ((totalTime % 60) < 10)
                //    totalStringTime = totalTime / 60 + ":0" + totalTime % 60;
                //else
                //    totalStringTime = totalTime / 60 + ":" + totalTime % 60;
            }
            totalStringTime = leftString + middleString + totalTime % 60;
            return totalStringTime;
        }

        public static int FinalScore(int minValue, int maxValue)
        {                       
            return Random.Range(minValue,maxValue);
        }

        public static bool LevelUpCalculator(ref int currentLevelNum, int currentExp, int nextExp)
        {
            bool isLevelUp = false;
            if (currentExp >= nextExp)
            {
                if (currentLevelNum < 100)
                {
                    currentLevelNum++;
                    isLevelUp = true;
                }
            }
            return isLevelUp;
        }

        /// <summary>
        /// Get random multiple for score
        /// </summary>
        /// <returns></returns>
        public static int GetPreciousValue()
        {
            float totalRate = 0;
            for (int i = 0; i < PreciousRates.Length; i++)
            {
                totalRate += PreciousRates[i];
            }
            float tempRate = Random.Range(0, totalRate);
            int multipleIndex = GetMultipleIndexFromRate(PreciousRates,tempRate);

            return PreciousMultiples[multipleIndex];
        }

        static int GetMultipleIndexFromRate(float[] pRates, float rate)
        {
            float compRate = rate;
            for (int i = 0; i < pRates.Length; i++)
            {
                if (compRate < pRates[i])
                    return i;
                else
                    compRate -= pRates[i];
            }
            return pRates.Length - 1;
        }
    }
}
