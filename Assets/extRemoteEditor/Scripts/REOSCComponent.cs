/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

using System.Collections.Generic;

using extOSC;
using extOSC.Core;
using extOSC.Components;
using extOSC.Core.Events;

namespace extRemoteEditor
{
    public abstract class REOSCComponent : MonoBehaviour, IOSCBind, IOSCReceiverComponent
    {
        #region Public Vars

        public OSCReceiver Receiver
        {
            get { return receiver; }
            set
            {
                if (receiver == value)
                    return;

                Unbind();

                receiver = value;

                Bind();
            }
        }

        public string ReceiverAddress
        {
            get { return receiverAddress; }
            set
            {
                if (receiverAddress == value)
                    return;

                Unbind();

                receiverAddress = value;

                Bind();
            }
        }

        public OSCEventMessage Callback
        {
            get
            {
                if (callback == null)
                {
                    callback = new OSCEventMessage();
                    callback.AddListener(InvokeMessage);
                }

                return callback;
            }
        }

        public OSCTransmitter Transmitter
        {
            get { return transmitter; }
            set { transmitter = value; }
        }

        #endregion

        #region Protected Vars

        [SerializeField]
        protected string receiverAddress = "/address";

        [SerializeField]
        protected OSCReceiver receiver;

        [SerializeField]
        protected OSCTransmitter transmitter;

        protected OSCEventMessage callback;

        protected OSCReceiver bindedReceiver;

        #endregion

        #region Unity Methods

        protected virtual void OnEnable()
        {
            Bind();
        }

        protected virtual void OnDisable()
        {
            Unbind();
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (Application.isPlaying)
            {
                Unbind();
                Bind();
            }
        }
#endif

        #endregion

        #region Public Methods

        public virtual void Bind()
        {
            if (receiver != null)
                receiver.Bind(this);

            bindedReceiver = receiver;
        }

        public virtual void Unbind()
        {
            if (bindedReceiver != null)
                bindedReceiver.Unbind(this);

            bindedReceiver = null;
        }

        #endregion

        #region Protected Methods

        protected abstract void Invoke(OSCMessage message);

        protected void Send(string address, IEnumerable<OSCValue> values)
        {
            if (transmitter != null)
            {
                var message = new OSCMessage(address);

                foreach (var value in values)
                {
                    message.AddValue(value);
                }

                transmitter.Send(message);
            }
        }

        protected void Send(string address, params OSCValue[] values)
        {
            if (transmitter != null)
            {
                var message = new OSCMessage(address, values);

                transmitter.Send(message);
            }
        }

        #endregion

        #region Private Methods

        private void InvokeMessage(OSCMessage message)
        {
            if (!enabled) return;

            Invoke(message);
        }

        #endregion
    }
}