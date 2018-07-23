using System.Collections;
using UnityEngine;
using DLMotion;

//*************************************************************************
//@header       RangeAssessmentFlowMediator
//@abstract     The mediator of flow in HiddenObiect.
//@discussion   Execute the flow of HiddenObiect.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class HiddenObjectFlowMediator : FlowMediator
    {
        int _flow;
        bool _isCompleted;
        MusicController _musicController;

        public override int Flow
        {
            get
            {
                return _flow;
            }
        }

        public override void Begin(int flow)
        {
            switch (flow)
            {
                case (int)HiddenObjectFlow.Start:
                    Debug.Log("Flow: Start");
                    Next();
                    break;
                case (int)HiddenObjectFlow.CountDown:
                    Debug.Log("Flow: CountDownWarmUp");
                    // Countdown
                    StartCoroutine(CountDown());
                    break;
                case (int)HiddenObjectFlow.Training:
                    Debug.Log("Flow: Training");
                    // Start game
                    GameStart.Instance.CanStartGame = true;
                    EventManager.Instance.PublishEvent(EventTypeSet.GameStart, new EventData(this));
                    // Close page of message
                    //GameStart.Instance.OpenOrCloseMessagePage(false, HiddenObjectPage.StartTrainPage);
                    // Send machine cmd
                    GameStart.Instance.SendMachineCmd();
                    break;
                case (int)HiddenObjectFlow.End:
                    Debug.Log("Flow: End");
                    DynaLinkHS.CmdServoOff();
                    EventManager.Instance.PublishEvent(EventTypeSet.FinishGame, new EventData(this));
                    StartCoroutine(EndFlowResetting());
                    if(_musicController)
                        _musicController.StopBGM();
                    //Next();
                    break;
                default:
                    Debug.LogWarning("Unexist flow! " + flow);
                    break;
            }
        }

        public override void BreakFlow(bool isNormalEnd)
        {
            StopAllCoroutines();
            Exit(isNormalEnd);
        }

        public override void Complete(int flow)
        {
            if(flow != _flow)
                Debug.LogError("Flow sequence doesn't match! Auto Run Next Flow(May cause Unexpected Error)");

            _flow++;
            _isCompleted = true;
        }

        public override void Initialize()
        {
            _musicController = GameObject.Find(HiddenObjectString.BackgroundMusic).GetComponent<MusicController>();
            _flow = (int)HiddenObjectFlow.Start;
            StartCoroutine(FlowLoop());
        }

        public override void Next()
        {
            Complete(_flow);
        }

        protected override IEnumerator FlowLoop()
        {
            while (_flow <= (int)HiddenObjectFlow.End)
            {
                _isCompleted = false;
                Begin(_flow);
                yield return StartCoroutine(WaitCondition.WaitUntil(() => _isCompleted));
            }
            Exit(true);
        }

        public override Coroutine StartCoroutine(IEnumerator iterator)
        {
            return GameStart.Instance.StartCoroutine(iterator);
        }

        public override void StopCoroutine(IEnumerator iterator)
        {
            GameStart.Instance.StopCoroutine(iterator);
        }

        public override void StopAllCoroutines()
        {
            GameStart.Instance.StopAllCoroutines();
        }

        #region OtherMethods
        void Exit(bool isNormalEnd)
        {
            if (isNormalEnd)
                GameStart.Instance.OpenOrCloseMessagePage(true, HiddenObjectPage.HOGUIGameOver);
            else
            // Close all UI
                GameStart.Instance.CloseGameAndUI(isNormalEnd);
        }

        void PromptTyingHand()
        {
            GameStart.Instance.OpenOrCloseMessagePage(true, HiddenObjectPage.PromptTyingHandPage);
        }

        IEnumerator CountDown()
        {
            if (!MainCore.Instance.IsMachineDisabled)
            {
                GameStart.Instance.OpenOrCloseMessagePage(true, HiddenObjectPage.ResettingPage);
                int waitMachineNum = 20;
                while (DynaLinkHS.MechType == 0)
                {
                    yield return new WaitForSeconds(1f);
                    Debug.LogWarning("<color=orange>Waiting for MechType!</color>");
                    waitMachineNum--;
                    if (waitMachineNum < 0)
                        break;
                }
                MachinePara.Initialize(DynaLinkHS.MechType);
                // Resetting
                yield return StartCoroutine(PassiveMoveToRespawnLoc());
            }

            // Show UITyingHandPage.
            PromptTyingHand();
            yield return "End CountDown!";
        }

        IEnumerator EndFlowResetting()
        {
            if (!MainCore.Instance.IsMachineDisabled)
            {
                yield return new WaitForSeconds(0.5f);
                GameStart.Instance.OpenOrCloseMessagePage(true, HiddenObjectPage.ResettingPage);
                var enumertor = PassiveMoveToRespawnLoc();
                StartCoroutine(enumertor);
                float timeRemain = 20f;
                while (timeRemain > 0)
                {
                    yield return new WaitForSeconds(1f);
                    if (GameStart.Instance.CanPlayGame)
                        timeRemain--;
                    else
                        timeRemain = 20;
                    if (enumertor.Current is string && (string)enumertor.Current == "End resetting!")
                    {
                        break;
                    }
                }
                StopCoroutine(enumertor);
            }
            Complete((int)HiddenObjectFlow.End);
        }

        IEnumerator PassiveMoveToRespawnLoc()
        {
            while (DynaLinkHS.MotionInProcess)
            {
                Debug.LogWarning("<color=orange>Motion in process! Waiting one frame!</color>");
                yield return "Wait for one frame";
            }
            Vector2 targetVec = MachinePara.ResetVect;
            CommandFunctions.InitializeStartReset(targetVec, 150000, 300);
            Debug.Log(string.Format("<color=cyan>PassiveMoveTo:</color> ({0}, {1})", (int)targetVec.x, (int)targetVec.y));
            do
            {
                yield return new WaitForSeconds(1f);
            } while (!CommandFunctions.IsResetStartFinished || DynaLinkHS.MotionInProcess);

            CommandFunctions.IsResetStartFinished = false;
            GameStart.Instance.OpenOrCloseMessagePage(false, HiddenObjectPage.ResettingPage);
            Debug.Log("<color=yellow>End resetting!</color>");
            yield return "End resetting!";
        }
        #endregion
    }

    public enum HiddenObjectFlow
    {
        Start,
        CountDown,
        Training,
        End
    }
}
