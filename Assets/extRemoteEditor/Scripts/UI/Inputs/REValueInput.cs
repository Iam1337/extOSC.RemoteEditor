/* Copyright (c) 2017 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

using extOSC;

namespace extRemoteEditor.UI
{
    public abstract class REValueInput : MonoBehaviour
    {
        #region Public Vars

        [Header("Input Settings")]
        public Text TitleLabel;

        public Text HelpLabel;

        public string Title
        {
            get { return TitleLabel.text; }
            set { TitleLabel.text = value; }
        }

        public string Help
        {
            get { return HelpLabel.text; }
            set { HelpLabel.text = value; }
        }

        public abstract OSCValueType Type { get; }

        #endregion

        #region Public Vars

        public abstract void SetValue(OSCValue value);

        public abstract OSCValue GetValue();

        #endregion
    }
}