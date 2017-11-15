/* Copyright (c) 2017 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC;

namespace extRemoteEditor
{
    public class OSCMobileController : MonoBehaviour
    {
        #region Private Vars

        private OSCReceiver _receiver;

        private OSCTransmitter _transmitter;

        #endregion

        #region Unity Methods

        protected void Awake()
        {
            _transmitter = FindObjectOfType<OSCTransmitter>();
            _receiver = FindObjectOfType<OSCReceiver>();
        }

        protected void OnApplicationPause(bool pauseStatus)
        {
            if (_transmitter == null)
                return;

            if (pauseStatus)
            {
                if (_transmitter != null)
                    _transmitter.Close();

                if (_receiver != null)
                    _receiver.Close();
            }
            else
            {
                if (_transmitter != null)
                    _transmitter.Connect();

                if (_receiver != null)
                    _receiver.Connect();
            }
        }

        #endregion
    }
}