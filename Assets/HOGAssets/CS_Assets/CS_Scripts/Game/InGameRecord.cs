//*************************************************************************
//@header       InGameRecord
//@abstract     Record the data in the game.
//@discussion   Provide functions to save points and judge the end of game and so on.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

using System.Collections.Generic;
using UIFrame;

namespace FZ.HiddenObjectGame
{
    public class InGameRecord
    {
        //static InGameRecord _instacne;

        int _gameLevel;
        int _playerLevel;
        int _playerExp;
        int _score;
        int _trainType;
        int _gameCategory;
        int _stressValue;
        int _stressLevel;
        int _obstacleLength;
        int _lostTime;        
        
        public InGameRecord(int trainType)
        {
            _trainType = trainType;
            //_playerLevel = playerLevel;
            //_playerExp = playerExp;
        }

        #region Properties
        public int GameLevel
        {
            get { return _gameLevel; }
            set { _gameLevel = value; }
        }

        public int PlayerLevel
        {
            get { return _playerLevel; }
            set { _playerLevel = value; }
        }

        public int PlayerExp
        {
            get { return _playerExp; }
            set { _playerExp = value; }
        }

        public int Score
        {
            get { return _score; }
            set { _score = value; }
        }

        public int TrainType
        {
            get { return _trainType; }
            set { _trainType = value; }
        }
        public int GameCategory
        {
            get { return _gameCategory; }
            set { _gameCategory = value; }
        }
        public int StressValue
        {
            get { return _stressValue; }
            set { _stressValue = value; }
        }
        public int StressLevel
        {
            get { return _stressLevel; }
            set { _stressLevel = value; }
        }
        public int ObstacleLength
        {
            get { return _obstacleLength; }
            set { _obstacleLength = value; }
        }
        public int LostTime
        {
            get { return _lostTime; }
            set { _lostTime = value; }
        }
        #endregion

        public void RecordTheDataInGame()
        {
            // Save GameLevel,PlayerLevel,PlayerExp in file.
            List<PatientConfigList> patientConfigList = new List<PatientConfigList>();
            PatientConfigList patientConfig = new PatientConfigList();
            patientConfig.Address = PatientinfoMgr.GetInstance().ShowValue("Address") ?? string.Empty;
            patientConfig.Age = PatientinfoMgr.GetInstance().ShowValue("Age") ?? string.Empty;
            patientConfig.AvatarID = PatientinfoMgr.GetInstance().ShowValue("AvatarID") ?? string.Empty;
            patientConfig.CaseID = PatientinfoMgr.GetInstance().ShowValue("CaseID") ?? string.Empty;
            patientConfig.CreateDate = PatientinfoMgr.GetInstance().ShowValue("CreateDate") ?? string.Empty;
            patientConfig.Department = PatientinfoMgr.GetInstance().ShowValue("Department") ?? string.Empty;
            patientConfig.Diagnosis = PatientinfoMgr.GetInstance().ShowValue("Diagnosis") ?? string.Empty;
            patientConfig.Exp = _playerExp.ToString() ?? string.Empty;
            patientConfig.Height = PatientinfoMgr.GetInstance().ShowValue("Height") ?? string.Empty;
            patientConfig.Level = _playerLevel.ToString() ?? string.Empty;
            patientConfig.Name = PatientinfoMgr.GetInstance().ShowValue("Name") ?? string.Empty;
            patientConfig.PhoneNumber = PatientinfoMgr.GetInstance().ShowValue("PhoneNumber") ?? string.Empty;
            patientConfig.PlayerID = PatientinfoMgr.GetInstance().ShowValue("PlayerID") ?? string.Empty;
            patientConfig.Relative = PatientinfoMgr.GetInstance().ShowValue("Relative") ?? string.Empty;
            patientConfig.RelativePhone = PatientinfoMgr.GetInstance().ShowValue("RelativePhone") ?? string.Empty;
            patientConfig.Remark = PatientinfoMgr.GetInstance().ShowValue("Remark") ?? string.Empty;
            patientConfig.Sex = PatientinfoMgr.GetInstance().ShowValue("Sex") ?? string.Empty;
            patientConfig.UpdateDate = PatientinfoMgr.GetInstance().ShowValue("UpdateDate") ?? string.Empty;
            patientConfig.Weight = PatientinfoMgr.GetInstance().ShowValue("Weight") ?? string.Empty;
            patientConfigList.Add(patientConfig);
            PatientinfoMgr.GetInstance().ModifyJson(patientConfigList);

            UnityEngine.Debug.Log("<color=green>Record successfully!</color>");
        }
    }
}
