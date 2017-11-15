/* Copyright (c) 2017 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

using extOSC;

namespace extRemoteEditor
{
    public class OSCTransmitterSettings : MonoBehaviour
    {
        #region Public Vars

        [Header("Transmitter Settings:")]
        public OSCTransmitter Transmitter;

        public InputField RemoteHostInput;

        public InputField RemotePortInput;

        public string PlayerPrefsPrefix = "transmitter";

        #endregion

        #region Unity Methods

        protected void Start()
        {
            Transmitter.RemoteHost = PlayerPrefs.GetString(PlayerPrefsPrefix + ".host", Transmitter.RemoteHost);
            Transmitter.RemotePort = PlayerPrefs.GetInt(PlayerPrefsPrefix + ".post", Transmitter.RemotePort);

            if (RemoteHostInput != null)
            {
                RemoteHostInput.text = Transmitter.RemoteHost;
                RemoteHostInput.onEndEdit.AddListener(RemoteHostEndEditCallback);
            }

            if (RemotePortInput != null)
            {
                RemotePortInput.text = Transmitter.RemotePort.ToString();
                RemotePortInput.onEndEdit.AddListener(RemotePortEndEditCallback);
            }
        }

        #endregion

        #region Private Methods

        private void RemoteHostEndEditCallback(string text)
        {
            Transmitter.RemoteHost = text;
            PlayerPrefs.SetString(PlayerPrefsPrefix + ".host", Transmitter.RemoteHost);
        }

        private void RemotePortEndEditCallback(string text)
        {
            int port;

            if (int.TryParse(text, out port))
            {
                Transmitter.RemotePort = port;
                PlayerPrefs.SetInt(PlayerPrefsPrefix + ".port", Transmitter.RemotePort);
            }
        }

        #endregion
    }
}