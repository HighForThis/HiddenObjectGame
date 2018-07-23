using UIFrame;
using UnityEngine;

//*************************************************************************
//@header       HiddenObjectString
//@abstract     The data of string.
//@discussion   Provide parameters for string.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public static class HiddenObjectString
    {
        public static string Random
        {
            get { return LanguageMgr.GetInstance().ShowText("Random"); }
        }

        public const string BackgroundMusic = "BackgroundMusic";
        public const string GameResources = "GameResources";
    }

    public static class HiddenObjectPath
    {
        static string _levelExpPath = Application.streamingAssetsPath + @"/Setting/LevelExp.xls";

        public static string LevelExpPath
        {
            get { return _levelExpPath; }
        }
    }

    public static class HiddenObjectKey
    {
        // PatientJSONConfig
        public const string Level = "Level";
        public const string Exp = "Exp";

        // LanguageJSONConfig
        public const string TieHand = "TieHand";
        public const string BeginTraining = "BeginTraining";
        public const string FinalScore = "FinalScore";
        public const string LostTime = "LostTime";
        public const string ExitMessage = "ExitMessage";
        public const string VictoryMessage = "VictoryMessage";
        public const string TimeUpMessage = "TimeUpMessage";
        public const string TargetString = "TargetString";
        public const string QuitGameOrNot = "QuitGameOrNot";
        public const string RandomReward = "RandomReward";
        public const string TopBarTipMessage = "TopBarTipMessage";

        // GameSettingJSONConfig
        public const string TrainType = "TrainType";
        public const string GameType = "GameType";
        public const string MaxAssist = "MaxAssist";
        public const string ActiveAssist = "ActiveAssist";
        public const string ResistiveAssist = "ResistiveAssist";
        public const string Mass = "Mass";
        public const string ObstacleLength = "ObstacleLength";
        public const string TrainTime = "Train";
        public const string Range = "Range";

        // TrainingJSONConfig

    }

    public static class HiddenObjectMessage
    {
        public const string MsgStopGame = "MsgStopGame";
        public const string MsgQuitGame = "MsgQuitGame";
    }

    public static class HiddenObjectPage
    {
        public const string CountDownPage = "HOGUICountDown";
        public const string PromptTyingHandPage = "HOGUIPromptTyingHand";
        public const string ResettingPage = "HOGUIResetting";
        public const string StartTrainPage = "HOGUIStartTrain";
        public const string MenuPage = "HOGMenu";
        public const string MusicConsole = "HOGMusicConsole";
        public const string HOGUITop = "HOGUITop";
        public const string HOGUICharacter = "HOGUICharacter";
        public const string HOGProgressBar = "HOGProgressBar";
        public const string HOGOutterMoveBoard = "HOGOutterMoveBoard";
        public const string HOGUIGameOver = "HOGUIGameOver";
        public const string HOGUILevelUp = "HOGUILevelUp";
    }
}
