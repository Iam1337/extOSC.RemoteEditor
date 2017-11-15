/* Copyright (c) 2017 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.Events;

using System.Collections.Generic;

namespace extRemoteEditor.UI
{
    public class REElementsList : MonoBehaviour
    {
        #region Public Vars

        [Header("List Settings")]
        public Transform ElementsRoot;

        [Header("Prefabs Settings")]
        public REElement SegmentPrefab;

        public REElement NonePrefab;

        public REButton ButtonPrefab;

        #endregion

        #region Private Vars

        private List<REElement> _elements = new List<REElement>();

        private REElement _lastNone;

        #endregion

        #region Public Methods

        public REElement CreateSegment(string text)
        {
            _lastNone = null;

            var segmentText = CreateElement(SegmentPrefab);
            segmentText.Text = text;

            CreateNone();

            return segmentText;
        }

        public REButton CreateButton(string text, UnityAction callback)
        {
            RemoveLastNone();

            var button = CreateElement(ButtonPrefab);
            button.Text = text;
            button.SetCallback(callback);

            return button;
        }

        public void ClearElements()
        {
            foreach (var element in _elements)
            {
                DestroyImmediate(element.gameObject);
            }

            _elements.Clear();
        }

        #endregion

        #region Private Methods

        private T CreateElement<T>(T elementPrefab) where T : REElement
        {
            var element = Instantiate(elementPrefab, ElementsRoot);

            _elements.Add(element);

            return element;
        }

        private void CreateNone()
        {
            var label = CreateElement(NonePrefab);
            label.Text = "- none -";

            _lastNone = label;
        }

        private void RemoveLastNone()
        {
            if (_lastNone != null)
            {
                _elements.Remove(_lastNone);

                DestroyImmediate(_lastNone.gameObject);
            }

            _lastNone = null;
        }

        #endregion
    }
}