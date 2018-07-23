using SimpleJSON;
using System.IO;
using UnityEngine;
//*************************************************************************
//@header       GlobalApplication
//@abstract     Global class for HiddenObjectGame.
//@discussion   Provide static parameters and functions.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public static class GlobalApplication
    {
        static bool _isPaused;
        static bool _canRecord;

        public static bool IsPause
        {
            get { return _isPaused; }
            set { _isPaused = value; }
        }

        public static bool CanRecord
        {
            get { return _canRecord; }
            set { _canRecord = value; }
        }

        /// <summary>
        /// Reset the flag
        /// </summary>
        public static void ResetFlag()
        {
            _isPaused = false;
            _canRecord = false;
        }

        public static int GetDataFromPropExpJson(string firstKey, string secondKey, string thirdKey)
        {
            string jsonPath = Application.streamingAssetsPath + @"/JsonPath/PropExpValues.json";
            if (!File.Exists(jsonPath))
                return 0;

            StreamReader sr = new StreamReader(jsonPath);

            if (sr == null)
                return 0;

            string json = sr.ReadToEnd();
            sr.Close();
            var jsonNode = JSON.Parse(json);
            var data = jsonNode[firstKey][secondKey][thirdKey].AsInt;

            return data;
        }
    }
}