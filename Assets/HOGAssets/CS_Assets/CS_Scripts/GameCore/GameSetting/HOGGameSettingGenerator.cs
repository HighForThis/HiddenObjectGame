using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIFrame;

namespace FZ.HiddenObjectGame
{
    public class HOGGameSettingGenerator
    {
        static HOGGameSettingGenerator _instance;

        public static HOGGameSettingGenerator Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new HOGGameSettingGenerator();
                return _instance;
            }
        }

        HOGGameSetting _gameSetting;

        public HOGGameSetting GameSetting
        {
            get
            {
                if (_gameSetting == null)
                    _gameSetting = new HOGGameSetting();
                return _gameSetting;
            }
        }
        
        public HOGGameSetting CreateGameSetting()
        {
            _gameSetting = new HOGGameSetting();

            ReadSettingFromJson();

            HOGRangeType rangeType = HOGRangeType.Big;

            switch (rangeType)
            {
                case HOGRangeType.Small:
                    _gameSetting.WidthScale = 0.5f;
                    _gameSetting.HeightScale = 0.5f;
                    break;
                case HOGRangeType.Medium:
                    _gameSetting.WidthScale = 0.8f;
                    _gameSetting.HeightScale = 0.6f;
                    break;
                case HOGRangeType.Big:
                    _gameSetting.WidthScale = 1.0f;
                    _gameSetting.HeightScale = 0.7f;
                    break;
                default:
                    break;
            }

            return _gameSetting;
        }
        
        void ReadSettingFromJson()
        {
            int trainTime;
            int.TryParse(GameSettingMgr.GetInstance().ShowValue(HiddenObjectKey.TrainTime), out trainTime);
            _gameSetting.TrainTime = trainTime;

            int trainType;
            int.TryParse(GameSettingMgr.GetInstance().ShowValue(HiddenObjectKey.TrainType), out trainType);
            _gameSetting.TrainType = trainType;

            int gameCategory;
            int.TryParse(GameSettingMgr.GetInstance().ShowValue(HiddenObjectKey.GameType), out gameCategory);
            _gameSetting.GameCategory = gameCategory;
            // Get range of motion
        } 
    }
}
