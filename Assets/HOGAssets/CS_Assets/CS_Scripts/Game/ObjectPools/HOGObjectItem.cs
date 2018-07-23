using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FZ.HiddenObjectGame
{
    [Serializable]
    public class HOGObjectItem
    {
        [Tooltip("Must be the same type of object")]
        public Transform[] ObjItems;
    }
}
