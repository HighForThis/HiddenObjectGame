using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FZ.HiddenObjectGame
{
    public class HOGTargetPool
    {
        HOGTargetPool()
        {
            _avaliable = new List<Transform>();
            _inUse = new List<Transform>();
        }

        static HOGTargetPool _instacne;
        List<Transform> _avaliable;
        List<Transform> _inUse;

        public static HOGTargetPool Instacne
        {
            get
            {
                if (_instacne == null)
                    _instacne = new HOGTargetPool();
                return _instacne;
            }
        }

        public Transform GetObject(ObjectType type, HOGObjectItem[] objList, int index)
        {
            Transform trans;
            //Debug.Log("<color=green>AvaliableCount-->>" + _avaliable.Count + "</color>");

            for (int i = _avaliable.Count - 1; i >= 0; i--)
            {
                if (_avaliable[i])
                {
                    if (_avaliable[i].name == objList[(int)type].ObjItems[index].name)
                    {
                        //Debug.Log("<color=red>AvaliableName-->>" + _avaliable[i].name + "</color>");

                        trans = _avaliable[i];
                        //if (trans.GetComponent<Rigidbody2D>())
                        //{
                        //    UnityEngine.Object.Destroy(trans.GetComponent<Rigidbody2D>());
                        //    //trans.GetComponent<Rigidbody2D>().simulated = false;
                        //    Debug.Log("<color=red>Rigidbody2D-->>" + trans.name +"</color>");
                        //}
                        //if (trans.GetComponent<CircleCollider2D>())
                        //{
                        //    trans.GetComponent<CircleCollider2D>().enabled = false;
                        //}
                        _inUse.Add(trans);
                        _avaliable.RemoveAt(i);
                        trans.gameObject.SetActive(true);
                        return trans;
                    }
                }
                else
                {
                    _avaliable.RemoveAt(i);
                }
            }

            trans = HOGObjectCreator.Instacne.Create(type, objList, index);
            _inUse.Add(trans);
            //Debug.Log("<color=yellow>InUseCount-->>" + _inUse.Count + "</color>");
            return trans;
        }

        public Transform GetObject(ObjectType type, HOGObjectItem[] objList, int index, Vector3 pos)
        {
            Transform trans = GetObject(type, objList, index);
            trans.position = pos;
            //trans.localScale = Vector3.one;
            return trans;
        }

        public void ReclaimObject(Transform pooledObject)
        {
            if (pooledObject.GetComponent<Rigidbody2D>())
            {
                UnityEngine.Object.Destroy(pooledObject.GetComponent<Rigidbody2D>());
                //trans.GetComponent<Rigidbody2D>().simulated = false;
                //Debug.Log("<color=green>Rigidbody2D-->>" + pooledObject.name + "</color>");
            }
            if (pooledObject.GetComponent<CircleCollider2D>())
            {
                pooledObject.GetComponent<CircleCollider2D>().enabled = false;
            }

            pooledObject.localScale = Vector3.one;
            
            for (int i = _inUse.Count - 1; i >= 0; i--)
            {
                if (_inUse[i] == pooledObject)
                {
                    _avaliable.Add(pooledObject);
                    _inUse.RemoveAt(i);
                    pooledObject.gameObject.SetActive(false);
                    return;
                }
            }

            if (!_avaliable.Contains(pooledObject))
            {
                _avaliable.Add(pooledObject);
                return;
            }
            //pooledObject.gameObject.SetActive(false);
            UnityEngine.Object.Destroy(pooledObject);
        }

        public void ReclaimObject(HOGPooledTarget pooledTarget)
        {
            try
            {
                ReclaimObject(pooledTarget.transform);
            }
            catch (Exception e)
            {

                Debug.Log(e.Message.ToString());
                throw;
            }
        }

        public void RemoveObject(Transform pooledObject)
        {
            for (int i = _inUse.Count - 1; i >= 0; i--)
            {
                if (_inUse[i] == pooledObject)
                {
                    _inUse.RemoveAt(i);
                    return;
                }
            }

            for (int i = _avaliable.Count - 1; i >= 0; i--)
            {
                if (_avaliable[i] == pooledObject)
                {
                    _avaliable.RemoveAt(i);
                    return;
                }
            }
        }

        public void RemoveObject(HOGPooledTarget pooledTarget)
        {
            RemoveObject(pooledTarget.transform);
        }
    }
}
