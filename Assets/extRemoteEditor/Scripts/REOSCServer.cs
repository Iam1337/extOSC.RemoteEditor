/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.SceneManagement;

using System.Reflection;
using System.Collections.Generic;

using extOSC;
using extOSC.Serialization;
using System.IO;
using System.Text;

namespace extRemoteEditor
{
    public class REOSCServer : REOSCComponent
    {
        #region Private Vars

        private List<REObject> _rootObjects = new List<REObject>();

        private Dictionary<int, REItem> _itemsDictionary = new Dictionary<int, REItem>();

        #endregion

        #region Unity Methods

        protected void Awake()
        {
            RefreshRootObjects();
        }

		#endregion

		#region Protected Methods

		protected override void Invoke(OSCMessage message)
        {
			Debug.Log(message);

            if (message.Values.Count < 3)
                return;
            
            if (message.Values[0].Type != OSCValueType.String ||
                message.Values[1].Type != OSCValueType.Int ||
                message.Values[2].Type != OSCValueType.Int)
                return;

            var index = 3;
            var count = message.Values.Count - index;

            var taskId = message.Values[0].StringValue;
            var command = (RECommand)(message.Values[1].IntValue);
            var instanceId = message.Values[2].IntValue;
            var inputValues = message.Values.GetRange(index, count);
            var outputValues = new List<OSCValue>();

            var invokeStatus = InvokeCommand(command, instanceId, inputValues, ref outputValues);
            if (invokeStatus != REInvokeStatus.Cancel)
            {
                outputValues.Insert(0, OSCValue.String(taskId));
                outputValues.Insert(1, OSCValue.Int((int)invokeStatus));

                Send(Address, outputValues);
            }
        }

        #endregion

        #region Private Methods

        private REInvokeStatus InvokeCommand(RECommand command, int instanceId, List<OSCValue> inputValues, ref List<OSCValue> outputValues)
        {
            if (command == RECommand.GetObjects)
            {
                return InvokeGetObjectsCommand(instanceId, inputValues, ref outputValues);
            }
            else if (command == RECommand.GetComponents)
            {
                return InvokeGetComponentsCommand(instanceId, inputValues, ref outputValues);
            }
            else if (command == RECommand.GetFields)
            {
                return InvokeGetFieldsCommand(instanceId, inputValues, ref outputValues);
            }
            else if (command == RECommand.GetValue)
            {
                return InvokeGetValueCommand(instanceId, inputValues, ref outputValues);
            }
            else if (command == RECommand.SetValue)
            {
                return InvokeSetValueCommand(instanceId, inputValues, ref outputValues);
            }
            else if (command == RECommand.Clear)
            {
                RefreshRootObjects();

                return REInvokeStatus.Complete;
            }

            return REInvokeStatus.Cancel;
        }

        private REInvokeStatus InvokeGetObjectsCommand(int instanceId, List<OSCValue> inputValues, ref List<OSCValue> outputValues)
        {
            if (inputValues.Count != 1)
                return REInvokeStatus.Error;

            if (inputValues[0].Type != OSCValueType.Int)
                return REInvokeStatus.Error;

            var index = inputValues[0].IntValue;
            var remoteObjects = (List<REObject>)null;

            if (instanceId == 0)
            {
                remoteObjects = _rootObjects;
            }
            else 
            {
                var rootObject = _itemsDictionary[instanceId] as REObject;
                if (rootObject == null) return REInvokeStatus.Error;
                if (rootObject.Childs.Count == 0)
                {
                    RefreshRemoteChildObjects(rootObject);
                }

                remoteObjects = rootObject.Childs;
            }

            if (index >= remoteObjects.Count || index < 0)
            {
                return REInvokeStatus.Error;
            }

            var remoteObject = remoteObjects[index];
            if (remoteObject == null) return REInvokeStatus.Error;

            outputValues.Add(OSCValue.Int(index));
            outputValues.Add(OSCValue.Int(remoteObjects.Count));
            outputValues.Add(OSCValue.Int(remoteObject.InstanceId));
            outputValues.Add(OSCValue.String(remoteObject.Name));

            return REInvokeStatus.Complete;
        }

        private REInvokeStatus InvokeGetComponentsCommand(int instanceId, List<OSCValue> inputValues, ref List<OSCValue> outputValues)
        {
            if (inputValues.Count != 1)
                return REInvokeStatus.Error;

            if (inputValues[0].Type != OSCValueType.Int)
                return REInvokeStatus.Error;

            var index = inputValues[0].IntValue;

            if (!_itemsDictionary.ContainsKey(instanceId))
                return REInvokeStatus.Error;

            var remoteObject = _itemsDictionary[instanceId] as REObject;
            if (remoteObject == null) return REInvokeStatus.Error;
            if (remoteObject.Components.Count == 0)
            {
                RefreshRemoteComponents(remoteObject);
            }

            if (index >= remoteObject.Components.Count || index < 0)
                return REInvokeStatus.Error;

            var component = remoteObject.Components[index];

            outputValues.Add(OSCValue.Int(index));
            outputValues.Add(OSCValue.Int(remoteObject.Components.Count));
            outputValues.Add(OSCValue.Int(component.InstanceId));
            outputValues.Add(OSCValue.String(component.Name));

            return REInvokeStatus.Complete;
        }

        private REInvokeStatus InvokeGetFieldsCommand(int instanceId, List<OSCValue> inputValues, ref List<OSCValue> outputValues)
        {
            if (inputValues.Count != 1)
                return REInvokeStatus.Error;

            if (inputValues[0].Type != OSCValueType.Int)
                return REInvokeStatus.Error;

            var index = inputValues[0].IntValue;

            if (!_itemsDictionary.ContainsKey(instanceId))
                return REInvokeStatus.Error;

            var remoteComponent = _itemsDictionary[instanceId] as REComponent;
            if (remoteComponent == null) return REInvokeStatus.Error;
            if (remoteComponent.Fields.Count == 0)
            {
                RefreshRemoteFields(remoteComponent);
            }

            if (index >= remoteComponent.Fields.Count || index < 0)
                return REInvokeStatus.Error;

            var remoteField = remoteComponent.Fields[index];

            outputValues.Add(OSCValue.Int(index));
            outputValues.Add(OSCValue.Int(remoteComponent.Fields.Count));
            outputValues.Add(OSCValue.String(remoteField.FieldName));
            outputValues.Add(OSCValue.Int(remoteComponent.InstanceId));
			outputValues.Add(OSCValue.Int((int)remoteField.FieldType));
			outputValues.Add(OSCValue.String(remoteField.TypeName));

            return REInvokeStatus.Complete;
        }

        private REInvokeStatus InvokeGetValueCommand(int instanceId, List<OSCValue> inputValues, ref List<OSCValue> outputValues)
        {
            if (inputValues.Count != 1)
                return REInvokeStatus.Error;
            
            if (inputValues[0].Type != OSCValueType.Int)
                return REInvokeStatus.Error;

            var fieldIndex = inputValues[0].IntValue;

            if (!_itemsDictionary.ContainsKey(instanceId))
                return REInvokeStatus.Error;

            var remoteComponent = _itemsDictionary[instanceId] as REComponent;
            if (remoteComponent == null) return REInvokeStatus.Error;
            if (remoteComponent.Fields.Count == 0)
            {
                RefreshRemoteFields(remoteComponent);
            }

            if (fieldIndex >= remoteComponent.Fields.Count || fieldIndex < 0)
                return REInvokeStatus.Error;

            var remoteField = remoteComponent.Fields[fieldIndex];

            outputValues.Add(OSCValue.Int(fieldIndex));

            foreach (var value in remoteField.Values)
            {
                outputValues.Add(value);
            }

            return REInvokeStatus.Complete;
        }

        private REInvokeStatus InvokeSetValueCommand(int instanceId, List<OSCValue> inputValues, ref List<OSCValue> outputValues)
        {
            if (inputValues.Count <= 1)
                return REInvokeStatus.Error;

            if (inputValues[0].Type != OSCValueType.Int)
                return REInvokeStatus.Error;

            var fieldIndex = inputValues[0].IntValue;

            if (!_itemsDictionary.ContainsKey(instanceId))
                return REInvokeStatus.Error;

            var remoteComponent = _itemsDictionary[instanceId] as REComponent;
            if (remoteComponent == null) return REInvokeStatus.Error;
            if (remoteComponent.Fields.Count == 0)
            {
                RefreshRemoteFields(remoteComponent);
            }

            if (fieldIndex >= remoteComponent.Fields.Count || fieldIndex < 0)
                return REInvokeStatus.Error;
        
            var remoteField = remoteComponent.Fields[fieldIndex];

            var index = 1;
            var count = inputValues.Count - index;
            var fieldValues = inputValues.GetRange(index, count);

            remoteField.Values = fieldValues;

            return REInvokeStatus.Complete;
        }

        private void RefreshRootObjects()
        {
            // CLEAR
            _rootObjects.Clear();
            _itemsDictionary.Clear();

            // POPULATE
            var scene = SceneManager.GetActiveScene();
            var rootObjects = scene.GetRootGameObjects();

            foreach (var rootObject in rootObjects)
            {
                var remoteObject = CreateObject(rootObject);

                _rootObjects.Add(remoteObject);
                _itemsDictionary.Add(remoteObject.InstanceId, remoteObject);
            }
        }

        private void RefreshRemoteChildObjects(REObject remoteObject)
        {
            // CLEAR
            foreach (var child in remoteObject.Childs)
            {
                _itemsDictionary.Remove(child.InstanceId);
            }

            remoteObject.Childs.Clear();

            // POPULATE
            var rootTransform = remoteObject.Target.transform;

            foreach (Transform childTransform in rootTransform)
            {
                var childObject = CreateObject(childTransform.gameObject);
                childObject.Parent = remoteObject;

                remoteObject.Childs.Add(childObject);
                _itemsDictionary.Add(childObject.InstanceId, childObject);
            }
        }

        private void RefreshRemoteComponents(REObject remoteObject)
        {
            // CLEAR
            foreach (var component in remoteObject.Components)
            {
                _itemsDictionary.Remove(component.InstanceId);
            }

            remoteObject.Components.Clear();

            // POPULATE
            var components = remoteObject.Target.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component == null)
                    continue;

                var componentType = component.GetType();

                var remoteComponent = new REComponent();
                remoteComponent.Target = component;
                remoteComponent.Name = componentType.Name;
                remoteComponent.Parent = remoteObject;
                remoteComponent.InstanceId = component.GetInstanceID();

                remoteObject.Components.Add(remoteComponent);
                _itemsDictionary.Add(remoteComponent.InstanceId, remoteComponent);
            }
        }

        private void RefreshRemoteFields(REComponent remoteComponent)
        {
            // CLEAR
            remoteComponent.Fields.Clear();

            // POPULATE
            var componentType = remoteComponent.Target.GetType();
            var fieldsInfos = componentType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var fieldInfo in fieldsInfos)
            {
                if (!OSCSerializer.HasPacker(fieldInfo.FieldType))
                    continue;

                var remoteField = new REField(fieldInfo);
				remoteField.FieldType = REFieldType.Field;
				remoteField.TypeName = fieldInfo.FieldType.Name;
				remoteField.FieldName = fieldInfo.Name;
                remoteField.Parent = remoteComponent;

                remoteComponent.Fields.Add(remoteField);
            }

            var propertiesInfos = componentType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in propertiesInfos)
            {
                if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                    continue;

                if (!OSCSerializer.HasPacker(propertyInfo.PropertyType))
                    continue;

                var remoteField = new REField(propertyInfo);
				remoteField.FieldType = REFieldType.Property;
				remoteField.TypeName = propertyInfo.PropertyType.Name;
				remoteField.FieldName = propertyInfo.Name;
                remoteField.Parent = remoteComponent;

                remoteComponent.Fields.Add(remoteField);
            }
        }

        private REObject CreateObject(GameObject obj)
        {
            var remoteObject = new REObject();
            remoteObject.InstanceId = obj.GetInstanceID();
            remoteObject.Target = obj;
            remoteObject.Name = obj.name;

            RefreshRemoteComponents(remoteObject);

            return remoteObject;
        }

        #endregion
    }
}