/* Copyright (c) 2017 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

using extOSC;

namespace extRemoteEditor.UI
{
    public class REStringInput : REValueInput
    {
        #region Public Vars

        [Header("String Settings")]
        public InputField Input;

        public override OSCValueType Type
        {
            get { return OSCValueType.String; }
        }

        #endregion

        #region Public Methods

        public override void SetValue(OSCValue value)
        {
            Input.text = value.StringValue;
        }

        public override OSCValue GetValue()
        {
            return OSCValue.String(Input.text);
        }

        #endregion
    }
}