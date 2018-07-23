using UnityEngine;

//*************************************************************************
//@header       MachinePara
//@abstract     Provide the proportion about machine and unity.
//@discussion   Static class.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{

    public static class MachinePara
    {
        #region Fields
        static int _machineWidth;
        static int _machineHeight;
        static int _machineXAxis;
        static int _machineYAxis;
        static int _machineXOffset;
        static int _machineYOffset;
        static int _xAxisMax;
        static int _yAxisMax;

        static float _orpWidth;
        static float _orpHeight;
        static float _plusScaleY;
        static Vector2 _resetVect;
        #endregion

        #region Properties

        public static int MachineWidth
        {
            get { return _machineWidth; }
        }
        public static int MachineHeight
        {
            get { return _machineHeight; }
        }
        public static int XAxisMax
        {
            get { return _xAxisMax; }
        }
        public static int YAxisMax
        {
            get { return _yAxisMax; }
        }
        public static float OrpWidth
        {
            get { return _orpWidth; }
        }
        public static float OrpHeight
        {
            get { return _orpHeight; }
        }

        public static int MachineXAxis
        {
            get { return _machineXAxis; }
        }
        public static int MachineYAxis
        {
            get { return _machineYAxis; }
        }
        public static float MachineXOffset
        {
            get { return _machineXOffset / 10000f; }
        }
        public static float MachineYOffset
        {
            get { return _machineYOffset / 10000f; }
        }
        public static float CentimeterPerUnit
        {
            get { return MachineWidth / OrpWidth; }
        }
        //public static float CentimeterPerUnitZ
        //{
        //    get { return MachineHeight / OrpHeight; }
        //}
        public static float WidthScale
        {
            get { return OrpWidth / MachineXAxis; }
        }
        public static float HeightScale
        {
            get { return OrpHeight / MachineYAxis; }
        }

        public static Vector2 ResetVect
        {
            get { return _resetVect; }
        }

        //public static float CentimeterPerUnitZ
        //{
        //    get { return MachineHeight / MachineYAxis; }
        //}

        #endregion

        /// <summary>
        /// 初始化机器与Unity相关比例系数
        /// </summary>
        /// <param name="machineType"></param>
        public static void Initialize(int machineType)
        {
            switch (machineType)
            {
                case 1:
                    _machineWidth = 62;
                    _machineHeight = 45;
                    _machineXAxis = 163000;//163000
                    _machineYAxis = 116000;//116000
                    _machineXOffset = 3000;
                    _machineYOffset = 3000;
                    _xAxisMax = 64; //62+2
                    _yAxisMax = 45;
                    _plusScaleY = 1f;
                    _orpWidth = 18f;
                    _orpHeight = _orpWidth * _machineYAxis * _plusScaleY / _machineXAxis;
                    _resetVect = new Vector2(81500, 10000);
                    //_orpHeight = _orpWidth * _machineHeight / _machineWidth;
                    UnityEngine.Debug.Log("Type-1");
                    break;
                case 2:
                    _machineWidth = 87;
                    _machineHeight = 72;
                    _machineXAxis = 116000;
                    _machineYAxis = 191000;
                    _machineXOffset = 3000;
                    _machineYOffset = 3000;
                    _xAxisMax = 88; //87+1
                    _yAxisMax = 72;
                    _plusScaleY = 0.5f;
                    _orpWidth = 18f;
                    _orpHeight = _orpWidth * _machineYAxis * _plusScaleY / _machineXAxis;
                    _resetVect = new Vector2(58000, 20000);
                    //_orpHeight = _orpWidth * _machineHeight / _machineWidth;
                    UnityEngine.Debug.Log("Type-2");
                    break;
                default:
                    _machineWidth = 62;
                    _machineHeight = 45;
                    _machineXAxis = 163000;//163000
                    _machineYAxis = 116000;//116000
                    _machineXOffset = 3000;
                    _machineYOffset = 3000;
                    _xAxisMax = 64; //62+2
                    _yAxisMax = 45;
                    _plusScaleY = 1f;
                    _orpWidth = 18f;
                    _orpHeight = _orpWidth * _machineYAxis * _plusScaleY / _machineXAxis;
                    _resetVect = new Vector2(81500, 10000);
                    //_orpHeight = _orpWidth * _machineHeight / _machineWidth;
                    UnityEngine.Debug.Log("Type-default");
                    break;
            }
        }

        public static Vector3 ToMachinePos(Vector3 unityPos)
        {
            Vector3 machinePos = new Vector3();
            machinePos.x = unityPos.x / WidthScale;
            machinePos.y = unityPos.y / HeightScale;
            return machinePos;
        }
    }

}