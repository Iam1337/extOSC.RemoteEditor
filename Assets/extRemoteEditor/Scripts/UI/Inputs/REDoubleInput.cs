/* Copyright (c) 2017 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

using extOSC;

namespace extRemoteEditor.UI
{
    public class REDoubleInput : REValueInput
    {
        #region Public Vars

        [Header("Double Settings")]
        public InputField Input;

        public override OSCValueType Type
        {
            get { return OSCValueType.Double; }
        }

        #endregion

        #region Public Methods

        public override void SetValue(OSCValue value)
        {
            Input.text = value.DoubleValue.ToString();;
        }

        public override OSCValue GetValue()
        {
            double value;

            if (double.TryParse(Input.text, out value))
                return OSCValue.Double(value);

            return OSCValue.Double(0);
        }

        #endregion
    }
}