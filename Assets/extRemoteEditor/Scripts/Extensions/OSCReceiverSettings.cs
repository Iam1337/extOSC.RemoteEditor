/* Copyright (c) 2017 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

using extOSC;

namespace extRemoteEditor
{
    public class OSCReceiverSettings : MonoBehaviour
    {
        #region Public Vars

        [Header("Receiver Settings:")]
        public OSCReceiver Receiver;

        public InputField LocalPortInput;

        public string PlayerPrefsPrefix = "receiver";

        #endregion

        #region Unity Methods

        protected void Start()
        {
            Receiver.LocalPort = PlayerPrefs.GetInt(PlayerPrefsPrefix + ".post", Receiver.LocalPort);

            if (LocalPortInput != null)
            {
                LocalPortInput.text = Receiver.LocalPort.ToString();
                LocalPortInput.onEndEdit.AddListener(LocalPortEndEditCallback);
            }
        }

        #endregion

        #region Private Methods

        private void LocalPortEndEditCallback(string text)
        {
            int port;

            if (int.TryParse(text, out port))
            {
                Receiver.LocalPort = port;
                PlayerPrefs.SetInt(PlayerPrefsPrefix + ".port", Receiver.LocalPort);
            }
        }

        #endregion
    }
}