//*************************************************************************
//@header       EventCallback
//@version      v1.0.0
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public delegate void EventCallback(EventData eventData);

    public class EventData
    {
        public EventData(object eventSender, object eventData = null, int eventCode = 0)
        {
            _sender = eventSender;
            _data = eventData;
            _code = eventCode;
            _eventBaseInfo = new EventBase();
        }

        public EventData(object eventSender, object eventData, int eventCode, EventBase eventBaseInfo)
        {
            _sender = eventSender;
            _data = eventData;
            _code = eventCode;
            _eventBaseInfo = eventBaseInfo;
        }

        // Event Sender.
        object _sender;

        // Event Code, 0 means Normal.
        int _code;

        // Event Data.
        object _data;

        EventBase _eventBaseInfo;

        public object Sender
        {
            get
            {
                return _sender;
            }

            set
            {
                _sender = value;
            }
        }

        public int Code
        {
            get
            {
                return _code;
            }

            set
            {
                _code = value;
            }
        }

        public object Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }

        public EventBase EventBaseInfo
        {
            get
            {
                return _eventBaseInfo;
            }

            set
            {
                _eventBaseInfo = value;
            }
        }
    }
}
