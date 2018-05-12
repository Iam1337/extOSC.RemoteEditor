/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

using extOSC;

namespace extRemoteEditor.UI
{
    public class RELongInput : REValueInput
    {
        #region Public Vars

        [Header("Long Settings")]
        public InputField Input;

        public override OSCValueType Type
        {
            get { return OSCValueType.Long; }
        }

        #endregion

        #region Public Methods

        public override void SetValue(OSCValue value)
        {
            Input.text = value.LongValue.ToString();;
        }

        public override OSCValue GetValue()
        {
            long value;

            if (long.TryParse(Input.text, out value))
                return OSCValue.Long(value);

            return OSCValue.Long(0);
        }

        #endregion
    }
}