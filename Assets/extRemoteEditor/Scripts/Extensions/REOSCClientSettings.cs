/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

namespace extRemoteEditor
{
    public class REOSCClientSettings : MonoBehaviour
    {
        #region Public Vars

        [Header("Remote Client Settings:")]
		public REOSCComponent Component;

        public InputField AddressInput;

        public string PlayerPrefsPrefix = "remoteclient";

        #endregion

        #region Unity Methods

        protected void Start()
        {
            Component.Address = PlayerPrefs.GetString(PlayerPrefsPrefix + ".address", Component.Address);

            if (AddressInput != null)
            {
                AddressInput.text = Component.Address;
                AddressInput.onEndEdit.AddListener(AddressEndEditCallback);
            }
        }

        #endregion

        #region Private Methods

        private void AddressEndEditCallback(string text)
        {
            Component.Address = text;

            PlayerPrefs.SetString(PlayerPrefsPrefix + ".address", Component.Address);
        }

        #endregion
    }
}