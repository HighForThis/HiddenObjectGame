using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FZ.HiddenObjectGame
{
    public class HOGGameSetting
    {
        public HOGGameSetting()
        {

        }

        public HOGGameSetting(HOGRangeType rangeType)
        {
            _rangeType = rangeType;
        }

        #region Fields
        private HOGRangeType _rangeType;
        private float _widthScale;
        private float _heightScale;
        private int _trainTime;
        private int _trainType;
        private int _gameCategory;
        #endregion

        #region Properties
        public HOGRangeType RangeType
        {
            get { return _rangeType; }
            set { _rangeType = value; }
        }

        public float WidthScale
        {
            get { return _widthScale; }
            set { _widthScale = value; }
        }

        public float HeightScale
        {
            get { return _heightScale; }
            set { _heightScale = value; }
        }

        public int TrainTime
        {
            get { return _trainTime; }
            set { _trainTime = value; }
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
        #endregion
    }

    public enum HOGRangeType
    {
        Small=1,
        Medium=2,
        Big=3
    }
}
