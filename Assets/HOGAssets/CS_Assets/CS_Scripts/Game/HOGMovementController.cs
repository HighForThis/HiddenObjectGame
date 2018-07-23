using UnityEngine;
using DLMotion;
using System.Collections;

//*************************************************************************
//@header       HOGMovementController
//@abstract     Move the player.
//@discussion   Add to the object as a component.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class HOGMovementController : MonoBehaviour
    {
        public RectTransform OutterMoveBoard;
        public RectTransform InnerMoveBoard;
        public Transform FollowerObj;
        public HOGGameController GCIcons;
        public bool CanShowGUI;

        Camera _uiCamera;
        bool _canFollow;

        bool IsCursorInMoveBoard
        {
            get
            {
                return RectTransformUtility.RectangleContainsScreenPoint(InnerMoveBoard, Input.mousePosition, _uiCamera);
            }
        }

        private void Awake()
        {
            _uiCamera = GameObject.Find("HOGUICamera").GetComponent<Camera>();
            //FollowerObj.gameObject.SetActive(false);
            MachinePara.Initialize(DynaLinkHS.MechType);
            EventManager.Instance.AddListener(EventTypeSet.FinishGame, GameOverEvent);
            _canFollow = true;
        }

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener(EventTypeSet.FinishGame, GameOverEvent);
            StopAllCoroutines();
        }

        //private void Start()
        //{
        //    StartCoroutine(CountDown());
        //}

        private void Update()
        {
            // Can follow cursor or machine
            //if (!GCIcons.isPaused && GameStart.Instance.CanPlayGame)

            if(GameStart.Instance.CanPlayGame && _canFollow)
            {
                if (MainCore.Instance.IsMachineDisabled)
                    FollowCursor(FollowerObj);
                else
                    FollowMachine(FollowerObj, DynaLinkHS.StatusMotRT.PosDataJ1 * MachinePara.WidthScale, DynaLinkHS.StatusMotRT.PosDataJ2 * MachinePara.HeightScale);
            }           
        }

        private void OnGUI()
        {
            if (!CanShowGUI)
                return;
            GUILayout.Space(40);
            if (!DynaLinkHS.ServerLinkActBit && !MainCore.Instance.IsMachineDisabled)
            {
                if (GUILayout.Button("     重连     ",GUILayout.Width(160),GUILayout.Height(60)))
                {
                    StartCoroutine(OnReconnect());
                }
            }

            if(GUILayout.Button("     断开     ", GUILayout.Width(160), GUILayout.Height(60)))
            {
                if (!MainCore.Instance.IsMachineDisabled)
                    DynaLinkHS.CmdServoOff();
                DynaLinkCore.StopSocket();
                Application.Quit();
            }
        }

        #region OtherMethods
        void FollowMachine(Transform objPoint, float machinePointX, float machinePointY)
        {
            //Debug.Log("<color=red>Machine</color>");
            objPoint.localPosition = new Vector3(machinePointX, machinePointY, objPoint.localPosition.z);
        }

        void FollowCursor(Transform objPoint)
        {
            Vector3 worldPos = Vector3.zero;
            if (IsCursorInMoveBoard)
            {
                //if (RectTransformUtility.ScreenPointToWorldPointInRectangle(InnerMoveBoard, Input.mousePosition, _uiCamera, out worldPos))
                //{
                //    objPoint.position = new Vector3(worldPos.x,worldPos.y,objPoint.position.z);
                //}
                //Debug.Log("<color=green>Cursor</color>");
                worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                objPoint.position = new Vector3(worldPos.x, worldPos.y, objPoint.position.z);
            }
        }

        //IEnumerator PassiveMoveToRespawnLoc()
        //{
        //    while (DynaLinkHS.MotionInProcess)
        //    {
        //        Debug.LogWarning("<color=orange>Motion in process! Waiting one frame!</color>");
        //        yield return "Wait for one frame";
        //    }
        //    Vector2 targetVec = new Vector2(81500, 35600);
        //    CommandFunctions.InitializeStartReset(targetVec, 150000, 300);
        //    Debug.Log(string.Format("<color=cyan>PassiveMoveTo:</color> ({0}, {1})", (int)targetVec.x, (int)targetVec.y));
        //    do
        //    {
        //        yield return new WaitForSeconds(1f);
        //    } while (!CommandFunctions.IsResetStartFinished || DynaLinkHS.MotionInProcess);

        //    CommandFunctions.IsResetStartFinished = false;
        //    Debug.Log("<color=yellow>End resetting!</color>");
        //    yield return "End resetting!";
        //    DynaLinkHS.CmdAssistLT(70);
        //}

        //IEnumerator CountDown()
        //{
        //    if (!MainCore.Instance.IsMachineDisabled)
        //    {
        //        while (!DynaLinkHS.ServerLinkActBit)
        //        {
        //            yield return "Wait Connectting!";
        //        }
        //        yield return StartCoroutine(PassiveMoveToRespawnLoc());
        //    }

        //    FollowerObj.gameObject.SetActive(true);
        //    //_canFollow = true;
        //}

        IEnumerator OnReconnect()
        {
            if (!DynaLinkHS.ServerLinkActBit)
            {
                DynaLinkCore.StopSocket();
                yield return new WaitForSeconds(1f);
                DynaLinkCore.ConnectClick();
            }
        }

        void GameOverEvent(EventData eventData)
        {
            //bool isNormal = (bool)eventData.Data;

            _canFollow = false;
        }
        #endregion
    }
}
