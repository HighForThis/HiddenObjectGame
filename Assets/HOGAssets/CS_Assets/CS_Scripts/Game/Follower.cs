using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*************************************************************************
//@header       Follower
//@abstract     Control the bar of progress.
//@discussion   Add to the object as a component.
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//@version      v1.0.0
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class Follower : MonoBehaviour
    {
        public Transform CenterAnchor;
        public float ProgressBarOffset;

        public Image ImgProgressBar;

        GameObject _progressBar;

        List<GameObject> _collisionObj;
        bool _canCount;
        float _timeCount;
        public int _enterNum;
        //public bool CanCount
        //{
        //    get { return _canCount; }
        //    set { _canCount = value; }
        //}

        #region UnityMethods
        private void Start()
        {
            _collisionObj = new List<GameObject>();
            //_imgProgressBar = GameObject.Find("TargetFillBar").GetComponent<Image>();
            _progressBar = ImgProgressBar.transform.parent.gameObject;
            EnableProgressBar(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.GetComponent<HOGHiddenObject>())
            {
                //Debug.Log("<color=green>Enter trigger</color>");
                _collisionObj.Add(collision.gameObject);
                _enterNum++;
                EnableProgressBar(true);
            }
            
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            //_collisionObj = null;
            if (collision.transform.GetComponent<HOGHiddenObject>())
            {
                if(_enterNum > 0)
                {
                    _enterNum--;
                    _collisionObj.Remove(collision.gameObject);
                }
                //Debug.Log("<color=red>Exit trigger</color>");
                _timeCount = 0;
                EnableProgressBar(false);
                if (_enterNum > 0)
                {
                    EnableProgressBar(true);
                }
            }
            
        }

        //private void OnTriggerStay2D(Collider2D collision)
        //{
        //    if (_enterNum > 0 && !_canCount)
        //    {
        //        _collisionObj = collision.gameObject;
        //        EnableProgressBar(true);
        //    }
        //}

        private void Update()
        {
            if (_canCount)
            {
                _timeCount += Time.deltaTime;
                ImgProgressBar.fillAmount = _timeCount / 3f;
                if (_timeCount >= 3f)
                {
                    if(_collisionObj[_enterNum-1] != null)
                    {
                        Debug.Log("<color=orange>Found!!</color>");

                        _collisionObj[_enterNum - 1].SendMessage("FoundHiddenObjects");
                        _collisionObj[_enterNum - 1].GetComponent<CircleCollider2D>().enabled = false;
                    }
                    EnableProgressBar(false);
                    _timeCount = 0;
                    //_enterNum--;
                }
            }
        }
        #endregion


        #region OtherMethods
        public void EnableProgressBar(bool isEnabled)
        {
            if (_progressBar == null)
                return;
            _canCount = isEnabled;
            if (isEnabled)
            {
                //Vector2 anchorePos = RectTransformUtility.WorldToScreenPoint(Camera.main, CenterAnchor.position);
                Vector3 anchorePos = CalculateUnity.WorldPositionToScreenPoint(_collisionObj[_enterNum-1].transform.position);
                anchorePos.x -= _progressBar.GetComponent<RectTransform>().sizeDelta.x / 2;
                anchorePos.y += ProgressBarOffset;
                _progressBar.GetComponent<RectTransform>().anchoredPosition = anchorePos;
            }
            _progressBar.SetActive(isEnabled);
        }
        #endregion
    }
}
