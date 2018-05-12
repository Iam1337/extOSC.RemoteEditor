/* Copyright (c) 2018 ExT (V.Sigalkin) */

using extOSC;

namespace extRemoteEditor.UI
{
    public class REDefaultInput : REValueInput
    {
        #region Public Vars

        public override OSCValueType Type
        {
            get { return OSCValueType.Unknown; }
        }

        #endregion

        #region Private Values 

        private OSCValue _value;

        #endregion

        #region Public Methods

        public override void SetValue(OSCValue value)
        {
            _value = value;
        }

        public override OSCValue GetValue()
        {
            return _value;
        }

        #endregion
    }
}