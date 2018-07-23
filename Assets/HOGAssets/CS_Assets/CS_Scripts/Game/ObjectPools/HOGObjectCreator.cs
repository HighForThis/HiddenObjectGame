using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FZ.HiddenObjectGame
{
    public class HOGObjectCreator
    {
        static HOGObjectCreator _instacne;

        public static HOGObjectCreator Instacne
        {
            get
            {
                if (_instacne == null)
                    _instacne = new HOGObjectCreator();
                return _instacne;
            }
        }

        /// <summary>
        /// Create object by default position and rotation
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objList"></param>
        /// <param name="targetIndex"></param>
        /// <returns></returns>
        public Transform Create(ObjectType type, HOGObjectItem[] objList, int targetIndex)
        {
            Transform trans = CreateByType(type, objList, targetIndex, Vector3.zero, Quaternion.identity);
            return trans;
        }

        /// <summary>
        /// Create object by default rotation
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objList"></param>
        /// <param name="targetIndex"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Transform Create(ObjectType type, HOGObjectItem[] objList, int targetIndex, Vector3 pos)
        {
            Transform trans = CreateByType(type, objList, targetIndex, pos, Quaternion.identity);
            return trans;
        }

        Transform CreateByType(ObjectType type, HOGObjectItem[] objList, int targetIndex, Vector3 pos, Quaternion rot)
        {
            Transform trans;

            if (objList.Length > (int)type)
            {
                if (objList[(int)type].ObjItems.Length > targetIndex)
                {
                    trans = UnityEngine.Object.Instantiate(objList[(int)type].ObjItems[targetIndex], pos, rot) as Transform;
                    trans.name = objList[(int)type].ObjItems[targetIndex].name;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            
            return trans;

        }
    }

    public enum ObjectType
    {
        Default,
        Vegetable,
        Fruit
    }
}
