//*************************************************************************
//@header       EventType
//@version      v1.0.0
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class EventType
    {
        public EventType(int ID, string name)
        {
            _ID = ID;
            _name = name;
        }

        int _ID;
        string _name;

        public int ID
        {
            get
            {
                return _ID;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }
    }
}
