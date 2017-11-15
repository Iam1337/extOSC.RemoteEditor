/* Copyright (c) 2017 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

namespace extRemoteEditor
{
    public class REOSCClientSettings : MonoBehaviour
    {
        #region Public Vars

        [Header("Remote Client Settings:")]
        public REOSCClient RemoteClient;

        public InputField AddressInput;

        public string PlayerPrefsPrefix = "remoteclient";

        #endregion

        #region Unity Methods

        protected void Start()
        {
            RemoteClient.ReceiverAddress = PlayerPrefs.GetString(PlayerPrefsPrefix + ".address", RemoteClient.ReceiverAddress);

            if (AddressInput != null)
            {
                AddressInput.text = RemoteClient.ReceiverAddress;
                AddressInput.onEndEdit.AddListener(AddressEndEditCallback);
            }
        }

        #endregion

        #region Private Methods

        private void AddressEndEditCallback(string text)
        {
            RemoteClient.ReceiverAddress = text;
            PlayerPrefs.SetString(PlayerPrefsPrefix + ".address", RemoteClient.ReceiverAddress);
        }

        #endregion
    }
}