/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

namespace extRemoteEditor.UI
{
    public class REElement : MonoBehaviour
    {
        #region Public Vars

        [Header("Text Settings")]
        public Text TextLabel;

        public string Text
        {
            get { return TextLabel.text; }
            set { TextLabel.text = value; }
        }

        #endregion
    }
}