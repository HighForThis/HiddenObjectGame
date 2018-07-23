using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FZ.HiddenObjectGame
{
    public class HOGPooledTarget : MonoBehaviour
    {
        public ObjectType Type;

        private void OnDestroy()
        {
            HOGTargetPool.Instacne.RemoveObject(this);
        }

        public void Recycle()
        {
            HOGTargetPool.Instacne.ReclaimObject(this);
        }
    }
}
