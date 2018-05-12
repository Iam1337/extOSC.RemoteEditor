/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

using System.Collections.Generic;

using extOSC;
using extOSC.Core;
using extOSC.Core.Events;

namespace extRemoteEditor
{
    public abstract class REOSCComponent : MonoBehaviour
    {
        #region Public Vars

		public string Address
		{
			get { return address; }
			set
			{
				if (address == value)
					return;

				Unbind();

				address = value;

				Bind();
			}
		}

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

		public OSCTransmitter Transmitter
		{
			get { return transmitter; }
			set { transmitter = value; }
		}

		#endregion

		#region Protected Vars

		[SerializeField]
        protected string address = "/address";

        [SerializeField]
        protected OSCReceiver receiver;

        [SerializeField]
        protected OSCTransmitter transmitter;

		protected OSCBind bind;

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

			if (transmitter != null)
			{
				transmitter.LocalPortMode = OSCLocalPortMode.FromReceiver;
				transmitter.LocalReceiver = receiver;
			}
        }
#endif

        #endregion

        #region Public Methods

        public virtual void Bind()
        {
			if (bind == null)
				bind = new OSCBind(address, InvokeMessage);

			if (receiver != null)
				receiver.Bind(bind);

            bindedReceiver = receiver;
        }

        public virtual void Unbind()
        {
			if (bindedReceiver != null && bind != null)
				bindedReceiver.Unbind(bind);

            bindedReceiver = null;
        }

        #endregion

        #region Protected Methods

        protected abstract void Invoke(OSCMessage message);

		protected virtual void Send(string address, IEnumerable<OSCValue> values)
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