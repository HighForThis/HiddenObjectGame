using UnityEngine;
using UnityEngine.UI;

//*************************************************************************
//@header       SelectionController
//@abstract     Control the sliding pages with the script of SelectionItemContainer.
//@discussion   Add to the object as a component.
//@version      v1.0.0
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class SelectionController : MonoBehaviour
    {
        public GameObject PageTemplate;
        SelectionItemContainer[] _pages;
        int _currentPageNum;
        string _currentName;
        bool _isLeft;

        public int CurrentPageNum
        {
            get { return _currentPageNum; }
            set { _currentPageNum = value; }
        }

        public string CurrentName
        {
            get { return _currentName; }
            set { _currentName = value; }
        }

        void Awake()
        {
            _pages = new SelectionItemContainer[2];
            for (int i = 0; i < _pages.Length; i++)
            {
                GameObject go = Instantiate(PageTemplate);
                go.transform.SetParent(PageTemplate.transform.parent, false);
                go.name = PageTemplate.name + i;
                _pages[i] = go.GetComponent<SelectionItemContainer>();
            }
            PageTemplate.SetActive(false);
        }

        /// <summary>
        /// Turn to the first page
        /// </summary>
        public void TurnToPageOne()
        {
            int side = -1;
            int index = 0;
            for (int i = 0; i < 2; i++)
            {
                _pages[index].SetSides(side);
                index++;
                side++;
            }
            _currentPageNum = 1;
            _pages[_currentPageNum].GetComponentInChildren<Text>().text = _currentName;
        }

        /// <summary>
        /// Turn over the page
        /// </summary>
        /// <param name="pageNum"></param>
        void TurnPage(int pageNum)
        {
            if (pageNum == _currentPageNum)
                return;
            if (_isLeft)
            {
                int current = _currentPageNum;
                int previous = pageNum;
                // Fill previous page.
                _pages[previous].SetSides(-1);
                _pages[previous].GetComponentInChildren<Text>().text = _currentName;
                // Turn current page.
                _pages[current].MoveToAnimation(1);
                // Turn previous page.
                _pages[previous].MoveToAnimation(0);
            }
            else
            {
                int current = _currentPageNum;
                int next = pageNum;
                // Fill next page.
                _pages[next].SetSides(1);
                _pages[next].GetComponentInChildren<Text>().text = _currentName;
                // Turn current page.
                _pages[current].MoveToAnimation(-1);
                // Turn next page.
                _pages[next].MoveToAnimation(0);
            }
            _currentPageNum = pageNum;
        }

        /// <summary>
        /// Previous page
        /// </summary>
        public void OnClickPreviousPage()
        {
            _isLeft = true;
            TurnPage(Mathf.Abs(CurrentPageNum - 1));
        }

        /// <summary>
        /// Next page
        /// </summary>
        public void OnClickNextPage()
        {
            _isLeft = false;
            TurnPage(Mathf.Abs(CurrentPageNum - 1));
        }
    }
}