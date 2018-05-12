/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

using System.Collections.Generic;

using extOSC;

using extRemoteEditor.UI;

namespace extRemoteEditor
{
    public class RemoteEditor : MonoBehaviour
    {
        #region Public Vars

        [Header("OSC Settings")]
        public REOSCClient RemoteClient;

        [Header("Hierarchy Settings")]
        public REElementsList HierarchyList;

        [Header("Inspector Settings")]
        public REElementsList InspectorList;

        [Header("Values Settings")]
        public REValueWindow ValueWindowPrefab;

        public Transform ValueWindowRoot;

        #endregion

        #region Private Vars

        private REValueWindow _valueWindow;

        #endregion

        #region Unity Methods

        protected void Start()
        {
            if (RemoteClient == null) 
                RemoteClient = FindObjectOfType<REOSCClient>();
            if (RemoteClient == null)
                return;

            ShowObjects(0);
        }

        #endregion

        #region Public Methods

        public void Refresh()
        {
            if (RemoteClient == null)
                return;

            RemoteClient.ClearObjects(ShowObjects);

            //ShowObjects(0);
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Private Methods

        // HIERARCHY
        private void ShowObjects(int instanceId)
        {
            HierarchyList.ClearElements();
            InspectorList.ClearElements();

            if (instanceId == 0)
            {
                HierarchyList.CreateSegment("Root:");
            }
            else
            {
                var remoteObject = RemoteClient.GetItem(instanceId) as REObject;
                if (remoteObject == null) return;

                var parent = remoteObject.Parent;
                if (parent != null)
                {
                    HierarchyList.CreateButton("< " + parent.Name, () => { ShowObjects(parent.InstanceId);});
                }
                else
                {
                    HierarchyList.CreateButton("< Root", () => { ShowObjects(0);});
                }

                HierarchyList.CreateSegment(remoteObject.Name + ":");
            }


            RemoteClient.RefreshObjects(instanceId, BuildHierarchyButton, ShowComponents);
        }

        private void ShowComponents(int instanceId)
        {
            HierarchyList.CreateSegment("Components:");

            RemoteClient.RefreshComponents(instanceId, BuildHierarchyButton, null);
        }

        private void BuildHierarchyButton(int parentId, int instanceId)
        {
            var remoteItem = RemoteClient.GetItem(instanceId);
            if (remoteItem == null) return;

            if (remoteItem is REObject)
            {
                var remoteObject = remoteItem as REObject;

                HierarchyList.CreateButton(remoteObject.Name, () => { ShowObjects(remoteObject.InstanceId); });
            }
            else if (remoteItem is REComponent)
            {
                var remoteComponent = remoteItem as REComponent;

                HierarchyList.CreateButton(remoteComponent.Name, () => { ShowFields(remoteComponent.InstanceId); });
            }
        }

        // INSPECTOR
        private void ShowFields(int instanceId)
        {
            InspectorList.ClearElements();

            var remoteComponent = RemoteClient.GetItem(instanceId) as REComponent;
            if (remoteComponent == null) return;

            InspectorList.CreateSegment(remoteComponent.Name + ":");

            RemoteClient.RefreshFields(instanceId, BuildInspectorButton, null);
        }

        private void BuildInspectorButton(int parentId, int fieldIndex)
        {
            var remoteComponent = RemoteClient.GetItem(parentId) as REComponent;
            if (remoteComponent == null) return;

            var remoteField = remoteComponent.Fields[fieldIndex];

            InspectorList.CreateButton(remoteField.FieldName, () => { RemoteClient.GetValue(parentId, fieldIndex, ShowValue); });
        }

        // VALUES
        private void ShowValue(int parentId, int fieldIndex)
        {
            var remoteComponent = RemoteClient.GetItem(parentId) as REComponent;
            if (remoteComponent == null) return;

            var remoteField = remoteComponent.Fields[fieldIndex];

            if (_valueWindow != null)
            {
                _valueWindow.Hide(false);
                _valueWindow = null;
            }

            _valueWindow = Instantiate(ValueWindowPrefab, ValueWindowRoot);
            _valueWindow.Show(remoteField, SendValue);
        }

        private void SendValue(REField remoteField, List<OSCValue> values)
        {
            var remoteComponent = remoteField.Parent;
            if (remoteComponent == null) return;

            var parentId = remoteComponent.InstanceId;
            var fieldIndex = remoteComponent.Fields.IndexOf(remoteField);

            RemoteClient.SetValue(parentId, fieldIndex, values, null);
        }

        #endregion
    }
}