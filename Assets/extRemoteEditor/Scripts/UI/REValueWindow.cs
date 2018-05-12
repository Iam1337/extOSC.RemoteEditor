/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using extOSC;

namespace extRemoteEditor.UI
{
    public class REValueWindow : MonoBehaviour
    {
        #region Extensions

        public delegate void WindowCallback(REField field, List<OSCValue> values);

        #endregion

        #region Public Vars

        [Header("UI Settings")]
        public Text Title;

        [Header("Prefabs Settings")]
        public List<REValueInput> Prefabs = new List<REValueInput>();

        public REValueInput DefaultPrefab;

        public Transform ValuesRoot;

        #endregion

        #region Private Vars

        private REField _field;

        private WindowCallback _closeCallback;

        private List<REValueInput> _valuesInputs = new List<REValueInput>();

        #endregion

        #region Public Methods

        public void Show(REField field, WindowCallback closeCallback)
        {
            _field = field;
            _closeCallback = closeCallback;

			Title.text = string.Format("<color=grey>{0}</color> {1}:", _field.FieldType, _field.FieldName);

            BuildInputs();

            gameObject.SetActive(true);
        }

        public void Hide(bool send)
        {
            if (send)
            {
                if (_closeCallback != null)
                    _closeCallback(_field, CreateValues());
            }

            _field = null;
            _closeCallback = null;

            gameObject.SetActive(false);

            DestroyObject(gameObject);
        }

        #endregion

        #region Private Methods

        private void BuildInputs()
        {
            foreach (var valueInput in _valuesInputs)
            {
                DestroyImmediate(valueInput.gameObject);
            }

            _valuesInputs.Clear();

            foreach (var value in _field.Values)    
            {
                var prefab = GetPrefab(value.Type);
                if (prefab == null) continue;

                var valueInput = Instantiate(prefab, ValuesRoot);
                valueInput.Title = value.Type.ToString();
                valueInput.Help = _field.FieldName;
                valueInput.SetValue(value);

                _valuesInputs.Add(valueInput);
            }
        }

        private List<OSCValue> CreateValues()
        {
            var values = new List<OSCValue>();

            foreach (var valueInput in _valuesInputs)
            {
                values.Add(valueInput.GetValue());
            }

            return values;
        }

        private REValueInput GetPrefab(OSCValueType valueType)
        {
            foreach (var prefab in Prefabs)
            {
                if (prefab.Type == valueType)
                    return prefab;
            }

            return DefaultPrefab;
        }

        #endregion
    }
}