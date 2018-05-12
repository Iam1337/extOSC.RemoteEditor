/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace extRemoteEditor.UI
{
    [RequireComponent(typeof(Button))]
    public class REButton : REElement
    {
        #region Public Vars

        [Header("Button Settings")]
        public Button Button;

        #endregion

        #region Public Methods

        public void SetCallback(UnityAction callback)
        {
            Button.onClick.AddListener(callback);
        }

        #endregion
    }
}