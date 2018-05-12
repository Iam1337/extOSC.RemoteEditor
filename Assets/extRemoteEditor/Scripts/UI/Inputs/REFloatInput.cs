/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

using extOSC;

namespace extRemoteEditor.UI
{
    public class REFloatInput : REValueInput
    {
        #region Public Vars

        [Header("Float Settings")]
        public InputField Input;

        public override OSCValueType Type
        {
            get { return OSCValueType.Float; }
        }

        #endregion

        #region Public Methods

        public override void SetValue(OSCValue value)
        {
            Input.text = value.FloatValue.ToString();;
        }

        public override OSCValue GetValue()
        {
            float value;

            if (float.TryParse(Input.text, out value))
                return OSCValue.Float(value);

            return OSCValue.Float(0);
        }

        #endregion
    }
}