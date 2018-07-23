using DLMotion;
using System;
using UnityEngine;

//*************************************************************************
//@header       MachineAPI
//@abstract     Script of command of machine.
//@discussion   Provide command of machine with log.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017-2018 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class MachineAPI
    {
        static MachineAPI _instance;
        MachineMode _mode;
        float _torque;
        float _mass;

        public static MachineAPI Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MachineAPI();
                return _instance;
            }
        }
        
        public MachineMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public float Torque
        {
            get { return _torque; }
            set { _torque = value; }
        }

        public float Mass
        {
            get { return _mass; }
            set { _mass = value; }
        }

        #region MachineCmd
        public static void CmdSyncVectorTorqueTrapezoidalAst(float machineHorizontal, float machineVertical, int torque)
        {
            DynaLinkHS.CmdSyncVectorTorqueTrapezoidalAst((int)machineHorizontal, (int)machineVertical, torque);
            Debug.Log(string.Format("<color=cyan>CmdSyncVectorTorqueTrapezoidalAst:</color> ({0}, {1}, {2}) {3}", (int)machineHorizontal, (int)machineVertical, torque, DateTime.Now));
        }

        public static void CmdAssistLT(int torque)
        {
            DynaLinkHS.CmdAssistLT(torque);
            Debug.Log(string.Format("<color=cyan>CmdAssistLT:</color> ({0}) {1}", torque, DateTime.Now));
        }

        public static void CmdResistLT(int torque)
        {
            DynaLinkHS.CmdResistLT(torque);
            Debug.Log(string.Format("<color=cyan>CmdResistLT:</color> ({0}) {1}", torque, DateTime.Now));
        }

        public static void CmdMassSim(int massValue, int frictionFactor = 1500)
        {
            DynaLinkHS.CmdMassSim(massValue, frictionFactor);
            Debug.Log(string.Format("<color=cyan>CmdMassSim:</color> ({0}, {1}) {2}", massValue, frictionFactor, DateTime.Now));
        }
        #endregion

        #region ExpandedMachineCmd
        /// <summary>
        /// Run this function when creating target in PassiveState or AssistTractionVTState
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="speed"></param>
        /// <param name="torque"></param>
        public void MoveTo(Vector3 pos, int speed = 0, int torque = 0)
        {
            Vector3 machinePos = MachinePara.ToMachinePos(pos);

            switch (_mode)
            {
                case MachineMode.Passive:
                    break;
                case MachineMode.AssistTractionVT:
                    CmdSyncVectorTorqueTrapezoidalAst(machinePos.x, machinePos.y, torque);
                    if(torque == 0)
                        Debug.LogWarning("Torque is zero! Machine won't move!");
                    break;
                default:
                    break;
            }
        }
        #endregion
    }

    public enum MachineMode
    {
        Passive,
        AssistTractionVT,
        AssistLT,
        ResistLT,
        MassSim
    }
}
