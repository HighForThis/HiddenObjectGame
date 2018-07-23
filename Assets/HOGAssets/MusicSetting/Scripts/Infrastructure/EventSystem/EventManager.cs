using System.Collections.Generic;

//*************************************************************************
//@header       EventManager
//@abstract     Manage event.
//@discussion   Provide functions like PublishEvent, AddListener and RemoveListener .
//@version      v1.0.0
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class EventManager
    {
        EventManager()
        {
            _dicPool = new Dictionary<EventType, EventCallback>();
        }

        #region Fields
        Dictionary<EventType, EventCallback> _dicPool;
        #endregion

        #region Instance
        static EventManager _instance;

        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EventManager();
                return _instance;
            }
        }
        #endregion

        // Each AddListener also needs to be Removed by RemoveListener. Or it may cause unpreditable problems.
        public void AddListener(EventType eventType, EventCallback eventCallback)
        {
            if (_dicPool.ContainsKey(eventType))
            {
                _dicPool[eventType] = _dicPool[eventType] == null ? eventCallback : _dicPool[eventType] + eventCallback;
            }
            else
            {
                _dicPool.Add(eventType, eventCallback);
            }
        }

        public void RemoveListener(EventType eventType, EventCallback eventCallback)
        {
            if (_dicPool.ContainsKey(eventType))
            {
                _dicPool[eventType] = _dicPool[eventType] == null ? null : _dicPool[eventType] - eventCallback;
                if (_dicPool[eventType] == null)
                    _dicPool.Remove(eventType);
            }
        }

        public void RemoveListener(EventType eventType)
        {
            if (_dicPool.ContainsKey(eventType))
            {
                _dicPool[eventType] = null;
                _dicPool.Remove(eventType);
            }
        }

        public void PublishEvent(EventType eventType, EventData eventData)
        {
            if (_dicPool.ContainsKey(eventType))
            {
                if (_dicPool[eventType] != null)
                    _dicPool[eventType](eventData);
            }
        }
    }
}