/* Copyright (c) 2017 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

using extOSC;

namespace extRemoteEditor.UI
{
    public class RECharInput : REValueInput
    {
        #region Public Vars

        [Header("Char Settings")]
        public InputField Input;

        public override OSCValueType Type
        {
            get { return OSCValueType.Char; }
        }

        #endregion

        #region Public Methods

        public override void SetValue(OSCValue value)
        {
            Input.text = value.CharValue.ToString();;
        }

        public override OSCValue GetValue()
        {
            if (Input.text.Length > 0)
                return OSCValue.Char(Input.text[0]);

            return OSCValue.Char(' ');
        }

        #endregion
    }
}