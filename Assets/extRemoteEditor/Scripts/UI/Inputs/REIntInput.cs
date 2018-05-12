/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

using extOSC;

namespace extRemoteEditor.UI
{
    public class REIntInput : REValueInput
    {
        #region Public Vars

        [Header("Int Settings")]
        public InputField Input;

        public override OSCValueType Type
        {
            get { return OSCValueType.Int; }
        }

        #endregion

        #region Public Methods

        public override void SetValue(OSCValue value)
        {
            Input.text = value.IntValue.ToString();;
        }

        public override OSCValue GetValue()
        {
            int value;

            if (int.TryParse(Input.text, out value))
                return OSCValue.Int(value);

            return OSCValue.Int(0);
        }

        #endregion
    }
}