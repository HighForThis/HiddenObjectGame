using DLMotion;
using UIFrame;
using UnityEngine;
using UnityEngine.SceneManagement;
//*************************************************************************
//@header       GameStart
//@abstract     Monitor the execution of game.
//@discussion   Add the scripts on GameObject as a component.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class GameStart : BaseUIForm
    {
        static GameStart _instance;
        bool _isbool;
        bool _isGameStop;
        bool _canSendCmd;
        bool _canStartGame;
        bool _isCaution;

        #region Data from Json
        int _minTargetScore;
        int _maxTargetScore;
        //int _trainTime;
        #endregion

        #region Data from excel
        int _currentLevelExp;
        int _nextLevelExp;
        #endregion

        string _trainTypeName;
        int _timeLeftSecond;
        int _timeDestroySecond;
        float _currentDistance;
        string[] _victoryMessage;
        string _timeUpMessage;
        string _targetString;
        string _rewardString;
        string _topBarTipMessage;

        Vector3 _targetPos;
        Vector3 _lastPos;
        float _targetDistanceProportion;
        PlayerStateMachine _playerStateMachine;
        InGameRecord _currentGameRecorder;

        public Object BackgroundMusic;
        public bool TestVersion;

        public static GameStart Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.Find("GameStart").GetComponent<GameStart>();
                    _instance._targetPos = MachinePara.ResetVect;
                    //_instance._targetPos.x = 81500 * MachinePara.WidthScale;
                    //_instance._targetPos.y = 35600 * MachinePara.HeightScale;
                    _instance._lastPos = new Vector3();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Just start game
        /// </summary>
        public bool CanStartGame
        {
            get { return _canStartGame; }
            set { _canStartGame = value; }
        }

        public int MinTargetScore
        {
            get { return _minTargetScore; }
        }

        public int MaxTargetScore
        {
            get { return _maxTargetScore; }
        }

        //public int TrainTime
        //{
        //    get { return _trainTime; }
        //}

        public int NextLevelExp
        {
            get { return _nextLevelExp; }
        }

        public int CurrentLevelExp
        {
            get { return _currentLevelExp; }
            //set { _currentLevelExp = value; }
        }

        public Vector3 TargetPos
        {
            get { return _targetPos; }
            set { _targetPos = value; }
        }

        public Vector3 LastPos
        {
            get { return _lastPos; }
            set { _lastPos = value; }
        }

        public float TargetDistanceProportion
        {
            get { return _targetDistanceProportion; }
            set { _targetDistanceProportion = value; }
        }

        public InGameRecord CurrentGameRecorder
        {
            get { return _currentGameRecorder; }
            set { _currentGameRecorder = value; }
        }

        public int TimeLeftSecond
        {
            get { return _timeLeftSecond; }
            set { _timeLeftSecond = value; }
        }

        public int TimeDestroySecond
        {
            get { return _timeDestroySecond; }
            set { _timeDestroySecond = value; }
        }

        public float CurrentDistance
        {
            get { return _currentDistance; }
            set { _currentDistance = value; }
        }

        public string[] VictoryMessage
        {
            get { return _victoryMessage; }
        }

        public string TimeUpMessage
        {
            get { return _timeUpMessage; }
        }

        public string RewardString
        {
            get { return _rewardString; }
        }

        public string TargetString
        {
            get { return _targetString; }
        }

        public string TopBarTipMessage
        {
            get { return _topBarTipMessage; }
        }

        /// <summary>
        /// Can play game without limit
        /// </summary>
        public bool CanPlayGame
        {
            get
            {
                return !_isGameStop && !GlobalApplication.IsPause && _canStartGame;
            }
        }

        #region UnityMethods
        private void Awake()
        {
            ReceiveMessage(HiddenObjectMessage.MsgStopGame, p =>
            {
                string info = p.Values as string;
                if (info == "True")
                    _isGameStop = true;
                if (info == "False")
                    _isGameStop = false;
            });
            ReceiveMessage(HiddenObjectMessage.MsgQuitGame, p =>
            {
                //CloseGameAndUI(false);
                // Break flow normally and loading scene
                FlowMediator.Instance.BreakFlow(true);
            });
            ReceiveMessage("MsgFaultFinish", p =>
            {
                // Break flow not normally and not loading scene
                FlowMediator.Instance.BreakFlow(false);
                CommandFunctions.IsHalt = true;
            });
            // Clear Flag
            GlobalApplication.ResetFlag();
            _isGameStop = false;
            _canStartGame = false;

            // Clear prefabs
            if (GameObject.Find(HiddenObjectString.GameResources) != null)
                Destroy(GameObject.Find(HiddenObjectString.GameResources));
            if (GetComponent<PlayerStateMachine>() != null)
                _playerStateMachine = GetComponent<PlayerStateMachine>();
            else
                Debug.LogError("No PlayerStateMachine!!!");
        }

        private void OnDestroy()
        {
            RemoveMessage(HiddenObjectMessage.MsgStopGame);
            RemoveMessage(HiddenObjectMessage.MsgQuitGame);
            RemoveMessage("MsgFaultFinish");
        }
        private void OnEnable()
        {
            // Create music of background
            CreateBackgroundMusic();

            // Set initial data of game
            InitializeGameSettingData();

            // Start flow
            FlowMediator.SetTo<HiddenObjectFlowMediator>();
            FlowMediator.Instance.Initialize();

            // Open UIForm
            OpenUIForm(HiddenObjectPage.HOGUICharacter);
            OpenUIForm(HiddenObjectPage.HOGUITop);
            OpenUIForm(HiddenObjectPage.MenuPage);
            //OpenUIForm(HiddenObjectPage.HOGProgressBar);
            //OpenUIForm(HiddenObjectPage.HOGOutterMoveBoard);

        }

        private void OnApplicationQuit()
        {
            UdpBasicClass.UdpSocketClient.SocketQuit();
        }

        private void Update()
        {
            if (!MainCore.Instance.IsMachineDisabled)
            {
                MonitorMachineStatus();
            }
        }
        #endregion


        #region OtherMethods
        /// <summary>
        /// Create music of background
        /// </summary>
        void CreateBackgroundMusic()
        {
            GameObject backgroundMusic = Instantiate(BackgroundMusic) as GameObject;
            backgroundMusic.name = HiddenObjectString.BackgroundMusic;
        }

        /// <summary>
        /// Set the value of GameSetting
        /// </summary>
        void InitializeGameSettingData()
        {
            HOGGameSettingGenerator.Instance.CreateGameSetting();

            int _trainType = HOGGameSettingGenerator.Instance.GameSetting.TrainType;
            //int.TryParse(GameSettingMgr.GetInstance().ShowValue(HiddenObjectKey.TrainType), out _trainType);
            int _gameCategory = HOGGameSettingGenerator.Instance.GameSetting.GameCategory;
            //int.TryParse(GameSettingMgr.GetInstance().ShowValue(HiddenObjectKey.GameType), out _gameCategory);
            //int.TryParse(GameSettingMgr.GetInstance().ShowValue(HiddenObjectKey.TrainTime), out _trainTime);

            #region GetStressValue
            int _stressValue = 0;
            int _stressLevel = 0;
            int _obstacleLength = 0;
            switch (_trainType)
            {
                case 2:
                    int.TryParse(GameSettingMgr.GetInstance().ShowValue(HiddenObjectKey.MaxAssist), out _stressValue);
                    if (_stressValue == 100)
                    {
                        _stressLevel = 1;
                    }
                    else if (_stressValue == 200)
                    {
                        _stressLevel = 2;
                    }
                    else if (_stressValue == 300)
                    {
                        _stressLevel = 3;
                    }
                    else if (_stressValue == 400)
                    {
                        _stressLevel = 4;
                    }
                    else if (_stressValue == 500)
                    {
                        _stressLevel = 5;
                    }
                    _trainTypeName = "Assistance";
                    break;
                case 3:
                    if (_gameCategory == 2)
                        int.TryParse(GameSettingMgr.GetInstance().ShowValue(HiddenObjectKey.ObstacleLength), out _obstacleLength);
                    int.TryParse(GameSettingMgr.GetInstance().ShowValue(HiddenObjectKey.ActiveAssist), out _stressValue);
                    if (_stressValue == 20)
                    {
                        _stressLevel = 1;
                    }
                    else if (_stressValue == 35)
                    {
                        _stressLevel = 2;
                    }
                    else if (_stressValue == 40)
                    {
                        _stressLevel = 3;
                    }
                    else if (_stressValue == 55)
                    {
                        _stressLevel = 4;
                    }
                    else if (_stressValue == 70)
                    {
                        _stressLevel = 5;
                    }
                    _trainTypeName = "Active";
                    break;
                case 4:
                    if (_gameCategory == 3)
                    {
                        int.TryParse(GameSettingMgr.GetInstance().ShowValue(HiddenObjectKey.Mass), out _stressValue);
                        if (_stressValue == 60000)
                        {
                            _stressLevel = 1;
                        }
                        else if (_stressValue == 46000)
                        {
                            _stressLevel = 2;
                        }
                        else if (_stressValue == 34000)
                        {
                            _stressLevel = 3;
                        }
                        else if (_stressValue == 22000)
                        {
                            _stressLevel = 4;
                        }
                        else if (_stressValue == 10000)
                        {
                            _stressLevel = 5;
                        }
                        _trainTypeName = "Mass";
                    }
                    if (_gameCategory == 1)
                    {
                        int.TryParse(GameSettingMgr.GetInstance().ShowValue(HiddenObjectKey.ResistiveAssist), out _stressValue);
                        if (_stressValue == 50)
                        {
                            _stressLevel = 1;
                        }
                        else if (_stressValue == 100)
                        {
                            _stressLevel = 2;
                        }
                        else if (_stressValue == 150)
                        {
                            _stressLevel = 3;
                        }
                        else if (_stressValue == 200)
                        {
                            _stressLevel = 4;
                        }
                        else if (_stressValue == 250)
                        {
                            _stressLevel = 5;
                        }
                        _trainTypeName = "Resistance";
                    }
                    break;
                default:
                    break;
            }
            #endregion

            #region GetScoreDependOnStress
            string levelName = "Level" + _stressLevel;
            _minTargetScore = GlobalApplication.GetDataFromPropExpJson(_trainTypeName, levelName, "Min");
            _maxTargetScore = GlobalApplication.GetDataFromPropExpJson(_trainTypeName, levelName, "Max");
            #endregion            

            #region SetRecorder
            _currentGameRecorder = new InGameRecord(_trainType);
            _currentGameRecorder.GameCategory = _gameCategory;
            _currentGameRecorder.StressValue = _stressValue;
            _currentGameRecorder.StressLevel = _stressLevel;
            _currentGameRecorder.ObstacleLength = _obstacleLength;
            // Get level and exp.
            int playerLevel = 0;
            int.TryParse(PatientinfoMgr.GetInstance().ShowValue(HiddenObjectKey.Level), out playerLevel);
            _currentGameRecorder.PlayerLevel = playerLevel;
            int playerExp = 0;
            int.TryParse(PatientinfoMgr.GetInstance().ShowValue(HiddenObjectKey.Exp), out playerExp);
            _currentGameRecorder.PlayerExp = playerExp;
            _currentGameRecorder.GameLevel = 1;
            _currentGameRecorder.Score = 0;
            _currentGameRecorder.LostTime = 0;
            #endregion

            GetNextLevelExp(playerLevel);

            // Get Victory Message
            _victoryMessage = LanguageMgr.GetInstance().ShowText(HiddenObjectKey.VictoryMessage).Split('#');

            // Get TimeUp Message
            _timeUpMessage = LanguageMgr.GetInstance().ShowText(HiddenObjectKey.TimeUpMessage);

            // Get Target String
            _targetString = LanguageMgr.GetInstance().ShowText(HiddenObjectKey.TargetString);

            // Get Reward String
            _rewardString = LanguageMgr.GetInstance().ShowText(HiddenObjectKey.RandomReward);

            // Get TopBarTipMessage
            _topBarTipMessage = LanguageMgr.GetInstance().ShowText(HiddenObjectKey.TopBarTipMessage);
        }

        /// <summary>
        /// Called on Update
        /// </summary>
        void MonitorMachineStatus()
        {
            if (DiagnosticStatus.MotStatus.Caution && !_isbool)
            {
                OpenUIForm("Spasm");
                _isCaution = true;
                //CommandFunctions.SpasmRecover(new Vector2(81500, 35600), 150000, 300, 3, 60);                
                Vector2 rePoint = new Vector2();
                if (_canStartGame)
                {
                    rePoint.x = Mathf.CeilToInt(_lastPos.x / MachinePara.WidthScale);
                    rePoint.y = Mathf.CeilToInt(_lastPos.y / MachinePara.HeightScale);
                }
                else
                {
                    rePoint = MachinePara.ResetVect;
                }
                CommandFunctions.SpasmRecover(rePoint, 150000, 300, 3, 60);
                _isbool = true;
            }

            if (CommandFunctions.IsSpasmFinished)
            {
                Debug.LogWarning("<color=orange>" + CommandFunctions.IsInReset + "</color>");
                if (CommandFunctions.IsInReset)
                    CommandFunctions.RestartReset();
                //else
                //{
                //    if (GlobalApplication.CanRecord)
                //        SendMachineCmd();
                //}
                CommandFunctions.IsSpasmFinished = false;
                _isbool = false;
                _isCaution = false;
            }

            //if (CommandFunctions.IsSpasmAgain)
            //{
            //    Debug.Log("二次痉挛");
            //    CommandFunctions.SpasmAgainRecover();
            //    CommandFunctions.IsSpasmAgain = false;
            //}


            if (!CommandFunctions.IsConnected || DynaLinkHS.EMstop || _isCaution)
            {
                GlobalApplication.IsPause = true;
                _canSendCmd = true;
            }
            else
            {
                if (!_isGameStop)
                    GlobalApplication.IsPause = false;
            }

            if (_canSendCmd && _canStartGame && !_isCaution && CommandFunctions.IsConnected && !DynaLinkHS.EMstop)
            {
                if (_isGameStop)
                {
                    DynaLinkHS.CmdServoOn();
                }
                else
                {
                    SendMachineCmd();
                }
                _canSendCmd = false;

            }
        }

        /// <summary>
        /// Close all UI and load new scene when the game is over.
        /// </summary>
        /// <param name="isNormal"></param>
        public void CloseGameAndUI(bool isNormal)
        {
            // Close UI
            CloseUIs();

            if (!isNormal)
                return;
            Debug.Log("<color=green>END!!!</color>");

            // Load scene of Main
            SceneManager.LoadSceneAsync("Main");
            if (!TestVersion)
            {
                OpenUIForm("Background");
                OpenUIForm("Main");
            }
        }

        void CloseUIs()
        {
            // Stop machine.
            DynaLinkHS.CmdServoOff();

            // Close all UI
            CloseUIForm(HiddenObjectPage.MenuPage, true);
            CloseUIForm(HiddenObjectPage.PromptTyingHandPage, true);
            CloseUIForm(HiddenObjectPage.ResettingPage, true);
            CloseUIForm(HiddenObjectPage.CountDownPage, true);
            CloseUIForm(HiddenObjectPage.StartTrainPage, true);
            CloseUIForm(HiddenObjectPage.HOGUICharacter, true);
            CloseUIForm(HiddenObjectPage.HOGUITop, true);
            CloseUIForm(HiddenObjectPage.HOGUIGameOver, true);
        }

        /// <summary>
        /// Open or close the page with name.
        /// </summary>
        /// <param name="isOpen"></param>
        /// <param name="pageName"></param>
        public void OpenOrCloseMessagePage(bool isOpen, string pageName)
        {
            if (isOpen)
                OpenUIForm(pageName);
            else
                CloseUIForm(pageName, true);
        }

        /// <summary>
        /// Send the message of game.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgName"></param>
        /// <param name="msgContent"></param>
        public void SendGameStartMessage(string msg, string msgName, object msgContent)
        {
            SendMessage(msg, msgName, msgContent);
        }

        /// <summary>
        /// Send command of machine depend on TrainType
        /// </summary>
        public void SendMachineCmd()
        {
            switch (_currentGameRecorder.TrainType)
            {
                case 2:
                    //DynaLinkHS.CmdServoOff();
                    _playerStateMachine.ChangeState<AssistTractionVTState>(_currentGameRecorder.StressValue);
                    break;
                case 3:
                    _playerStateMachine.ChangeState<AssistLTState>(_currentGameRecorder.StressValue);
                    break;
                case 4:
                    if (_currentGameRecorder.GameCategory == 3)
                    {
                        _playerStateMachine.ChangeState<MassState>(_currentGameRecorder.StressValue);
                    }
                    if (_currentGameRecorder.GameCategory == 1)
                    {
                        _playerStateMachine.ChangeState<ResistState>(_currentGameRecorder.StressValue);
                    }
                    break;
                default:
                    break;
            }

            _playerStateMachine.Run();
        }

        /// <summary>
        /// Send command when it is assist.
        /// </summary>
        public void SendCmdWhenAssist()
        {
            if (_currentGameRecorder.TrainType == 2)
            {
                DynaLinkHS.CmdServoOff();
            }
        }

        /// <summary>
        /// Get the level and experience from excel.
        /// </summary>
        /// <param name="playerLevel"></param>
        public void GetNextLevelExp(int playerLevel)
        {
            _currentLevelExp = (int)FileManager.Instance.ReadExcelNumber(HiddenObjectPath.LevelExpPath, 0, playerLevel, 1);
            _nextLevelExp = (int)FileManager.Instance.ReadExcelNumber(HiddenObjectPath.LevelExpPath, 0, playerLevel + 1, 1);
        }
        #endregion
    }
}