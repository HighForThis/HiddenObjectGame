//*************************************************************************
//@header       EventBase
//@version      v1.0.0
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class EventBase
    {
        public EventBase()
        {
            //_UID = Global.AutoGenerate.EventUID;
        }

        // Unique ID, auto-generated.
        //int _UID;
        // One type, one ID.
        int _ID;
        EventType _type;
        EventCategory _category;
        string _name;
        string _description;

        //public int UID
        //{
        //    get
        //    {
        //        return _UID;
        //    }
        //}

        public int ID
        {
            get
            {
                return _ID;
            }

            set
            {
                _ID = value;
            }
        }

        public EventType Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
            }
        }

        public EventCategory Category
        {
            get
            {
                return _category;
            }

            set
            {
                _category = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }
    }
}