﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects;

namespace ASL.Manipulation.Controllers.PC
{
    public class Keyboard : MonoBehaviour
    {
        private MoveBehavior _moveBehavior;
        private ObjectInteractionManager objManager;

        private void Awake()
        {
            objManager = GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>();
            _moveBehavior = objManager.RegisterBehavior<MoveBehavior>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.DownArrow)
                || Input.GetKey(KeyCode.S))
            {
                MoveBehavior.Down();
            }
            if(Input.GetKey(KeyCode.UpArrow)
                || Input.GetKey(KeyCode.W))
            {
                MoveBehavior.Up();
            }
            if(Input.GetKey(KeyCode.LeftArrow)
                || Input.GetKey(KeyCode.A))
            {
                MoveBehavior.Left();
            }
            if(Input.GetKey(KeyCode.RightArrow)
                || Input.GetKey(KeyCode.D))
            {
                MoveBehavior.Right();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                MoveBehavior.RotateClockwise();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                MoveBehavior.RotateCounterClockwise();
            }
            
            if (Input.GetKey(KeyCode.R))
            {
                //gameObject.GetComponent<CreateObject>().CreatePUNObject("Rooms/Room2/Room2");
                string prefabName = "Rooms/Room2/Room2";
                //objManager.Instantiate(prefabName);
                objManager.InstantiateOwnedObject(prefabName);
            }

            if (Input.GetKey(KeyCode.P))
            {
                GameObject go = GameObject.Find("Sphere");
                UWBNetworkingPackage.NetworkManager nm = GameObject.Find("NetworkManager").GetComponent<UWBNetworkingPackage.NetworkManager>();
                nm.UnRestrictOwnership(go);
            }

            if (Input.GetKey(KeyCode.O))
            {
                List<int> IDsToAdd = new List<int>();
                IDsToAdd.Add(2);
                UWBNetworkingPackage.NetworkManager nm = GameObject.Find("NetworkManager").GetComponent<UWBNetworkingPackage.NetworkManager>();
                GameObject go = GameObject.Find("Sphere");
                nm.WhiteListOwnership(go, IDsToAdd);
            }
        }

        public MoveBehavior MoveBehavior
        {
            get
            {
                return _moveBehavior;
            }
            set
            {
                if(value != null)
                {
                    _moveBehavior = value;
                }
            }
        }
    }
}