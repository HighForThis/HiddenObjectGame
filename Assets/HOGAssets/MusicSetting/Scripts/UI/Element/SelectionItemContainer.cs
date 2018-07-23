using UnityEngine;
using DG.Tweening;

//*************************************************************************
//@header       SelectionItemContainer
//@abstract     UI Sliding Selection
//@discussion   Add the script to each sliding page as a component.
//@version      v1.0.0
//@author       Felix Zhang
//@copyright    Copyright (c) 2017 FFTAI Co.,Ltd.All rights reserved.
//**************************************************************************

namespace FZ.HiddenObjectGame
{
    public class SelectionItemContainer : MonoBehaviour
    {
        /// <summary>
        /// Change the position of page
        /// </summary>
        /// <param name="side"></param>
        public void SetSides(int side)
        {
            Vector2 pos = GetComponent<RectTransform>().anchoredPosition;
            switch (side)
            {
                case -1:
                    GetComponent<RectTransform>().anchoredPosition = new Vector2(-GetComponent<RectTransform>().rect.width - 10, pos.y);
                    break;
                case 0:
                    GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    break;
                case 1:
                    GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().rect.width + 10, pos.y);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Play animation with DOTween
        /// </summary>
        /// <param name="side"></param>
        public void MoveToAnimation(int side)
        {
            Vector2 pos = GetComponent<RectTransform>().anchoredPosition;
            switch (side)
            {
                case -1:
                    pos = new Vector2(-GetComponent<RectTransform>().rect.width - 10, pos.y);
                    break;
                case 0:
                    pos = new Vector2(0, 0);
                    break;
                case 1:
                    pos = new Vector2(GetComponent<RectTransform>().rect.width + 10, pos.y);
                    break;
                default:
                    break;
            }
            DOTween.To(() => GetComponent<RectTransform>().anchoredPosition, x => GetComponent<RectTransform>().anchoredPosition = x, pos, 0.35f).SetEase(Ease.OutCirc);
        }
    }
}