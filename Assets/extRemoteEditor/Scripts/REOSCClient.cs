/* Copyright (c) 2017 ExT (V.Sigalkin) */

using System.Collections.Generic;

using extOSC;

namespace extRemoteEditor
{
    public class REOSCClient : REOSCComponent
    {
        #region Extensions

        public delegate void TaskComplete(int parentId);

        public delegate void TaskProcess(int parentId, int instanceId);

        private class Task
        {
            #region Public Vars

            public int ParentId
            {
                get { return _parentId; }
            }

            public RECommand Command
            {
                get { return _command; }
            }

            public string TaskId
            {
                get { return _taskId; }
            }

            public TaskComplete CompleteCallback;

            public TaskProcess ProcessCallback;

            #endregion

            #region Private Vars

            private int _parentId;

            private RECommand _command;

            private string _taskId;

            #endregion

            #region Public Methods

            public Task(RECommand command, int parentId, string taskId)
            {
                _command = command;
                _parentId = parentId;
                _taskId = taskId;
            }

            public void Complete()
            {
                if (CompleteCallback != null)
                    CompleteCallback(ParentId);
            }

            public void Process(int instanceId)
            {
                if (ProcessCallback != null)
                    ProcessCallback(ParentId, instanceId);
            }

            #endregion
        }

        #endregion

        #region Private Vars

        private Dictionary<string, Task> _tasksDictionary = new Dictionary<string, Task>();

        private Dictionary<int, REItem> _itemsDictionary = new Dictionary<int, REItem>();

        #endregion

        #region Public Methods

        public void ClearObjects(TaskComplete completeCallback)
        {
            var task = CreateTask(RECommand.Clear, 0, null, completeCallback);

            Send(ReceiverAddress, task);
        }

        public void RefreshObjects(int parentId, TaskProcess processCallback, TaskComplete completeCallback)
        {
            var task = CreateTask(RECommand.GetObjects, parentId, processCallback, completeCallback);

            Send(ReceiverAddress, task, OSCValue.Int(0));
        }

        public void RefreshComponents(int parentId, TaskProcess processCallback, TaskComplete completeCallback)
        {
            var task = CreateTask(RECommand.GetComponents, parentId, processCallback, completeCallback);

            Send(ReceiverAddress, task, OSCValue.Int(0));
        }

        public void RefreshFields(int parentId, TaskProcess processCallback, TaskComplete completeCallback)
        {
            var task = CreateTask(RECommand.GetFields, parentId, processCallback, completeCallback);

            Send(ReceiverAddress, task, OSCValue.Int(0));
        }

        public void GetValue(int parentId, int fieldIndex, TaskProcess completeCallback)
        {
            var task = CreateTask(RECommand.GetValue, parentId, completeCallback, null);

            Send(ReceiverAddress, task, OSCValue.Int(fieldIndex));
        }

        public void SetValue(int parentId, int fieldIndex, List<OSCValue> values, TaskComplete completeCallback)
        {
            var task = CreateTask(RECommand.SetValue, parentId, null, completeCallback);

            values.Insert(0, OSCValue.Int(fieldIndex));

            Send(ReceiverAddress, task, values.ToArray());
        }

        public REItem GetItem(int instanceId)
        {
            if (_itemsDictionary.ContainsKey(instanceId))
                return _itemsDictionary[instanceId];

            return null;
        }

        #endregion

        #region Protected Methods

        private void Send(string address, Task task, params OSCValue[] values)
        {
            var tempValues = new List<OSCValue>();
            tempValues.Add(OSCValue.String(task.TaskId));
            tempValues.Add(OSCValue.Int((int)task.Command));
            tempValues.Add(OSCValue.Int(task.ParentId));

            foreach (var value in values)
            {
                tempValues.Add(value);
            }

            Send(address, tempValues);
        }

        protected override void Invoke(OSCMessage message)
        {
            if (message.Values.Count < 2)
                return;

            if (message.Values[0].Type != OSCValueType.String ||
                message.Values[1].Type != OSCValueType.Int)
                return;

            var index = 2;
            var count = message.Values.Count - index;

            var taskId = message.Values[0].StringValue;
            var remoteStatus = (REInvokeStatus)message.Values[1].IntValue;

            if (!_tasksDictionary.ContainsKey(taskId))
                return;

            var task = _tasksDictionary[taskId];

            if (remoteStatus != REInvokeStatus.Complete)
            {
                _tasksDictionary.Remove(taskId);
                task.Complete();
                return;
            }

            var inputValues = message.Values.GetRange(index, count);
            var outputValues = new List<OSCValue>();

            var invokeStatus = InvokeTask(task, inputValues, ref outputValues);
            if (invokeStatus != REInvokeStatus.Cancel)
            {
                outputValues.Insert(0, OSCValue.String(taskId));
                outputValues.Insert(1, OSCValue.Int((int)task.Command));
                outputValues.Insert(2, OSCValue.Int(task.ParentId));

                Send(ReceiverAddress, outputValues);
            }
            else
            {
                _tasksDictionary.Remove(taskId);
                task.Complete();
            }
        }

        #endregion

        #region Private Methods

        private REInvokeStatus InvokeTask(Task task, List<OSCValue> inputValues, ref List<OSCValue> outputValues)
        {
            var command = task.Command;
            if (command == RECommand.GetObjects)
            {
                return InvokeGetObjectsCommand(task, inputValues, ref outputValues);
            }
            else if (command == RECommand.GetComponents)
            {
                return InvokeGetComponentsCommand(task, inputValues, ref outputValues);
            }
            else if (command == RECommand.GetFields)
            {
                return InvokeGetFieldsCommand(task, inputValues, ref outputValues);
            }
            else if (command == RECommand.GetValue)
            {
                return InvokeGetValueCommand(task, inputValues, ref outputValues);
            }
            else if (command == RECommand.SetValue)
            {
                return InvokeSetValueCommand(task, inputValues, ref outputValues);
            }
            else if (command == RECommand.Clear)
            {
                task.Complete();
            }

            return REInvokeStatus.Cancel;
        }

        private REInvokeStatus InvokeGetObjectsCommand(Task task, List<OSCValue> inputValues, ref List<OSCValue> outputValues)
        {
            if (inputValues.Count != 4)
                return REInvokeStatus.Cancel;

            if (inputValues[0].Type != OSCValueType.Int ||
                inputValues[1].Type != OSCValueType.Int ||
                inputValues[2].Type != OSCValueType.Int ||
                inputValues[3].Type != OSCValueType.String)
                return REInvokeStatus.Cancel;

            var index = inputValues[0].IntValue;
            var count = inputValues[1].IntValue;
            var instanceId = inputValues[2].IntValue;
            var objectName = inputValues[3].StringValue;

            var remoteObject = new REObject();
            remoteObject.Name = objectName;
            remoteObject.InstanceId = instanceId;

            if (_itemsDictionary.ContainsKey(task.ParentId) && instanceId != task.ParentId)
            {
                var parentObject = GetItem(task.ParentId) as REObject;
                if (parentObject != null)
                {
                    remoteObject.Parent = parentObject;
                    parentObject.Childs.Add(remoteObject);
                }
            }

            if (_itemsDictionary.ContainsKey(remoteObject.InstanceId))
                _itemsDictionary.Remove(remoteObject.InstanceId);

            _itemsDictionary.Add(remoteObject.InstanceId, remoteObject);

            task.Process(instanceId);

            index++;

            if (index >= count)
                return REInvokeStatus.Cancel;

            outputValues.Add(OSCValue.Int(index));

            return REInvokeStatus.Complete;
        }

        private REInvokeStatus InvokeGetComponentsCommand(Task task, List<OSCValue> inputValues, ref List<OSCValue> outputValues)
        {
            if (inputValues.Count != 4)
                return REInvokeStatus.Cancel;

            if (inputValues[0].Type != OSCValueType.Int ||
                inputValues[1].Type != OSCValueType.Int ||
                inputValues[2].Type != OSCValueType.Int ||
                inputValues[3].Type != OSCValueType.String)
                return REInvokeStatus.Cancel;

            var index = inputValues[0].IntValue;
            var count = inputValues[1].IntValue;
            var instanceId = inputValues[2].IntValue;
            var componentType = inputValues[3].StringValue;

            var parentObject = GetItem(task.ParentId) as REObject;
            if (parentObject == null) return REInvokeStatus.Cancel;

            var remoteComponent = new REComponent();
            remoteComponent.Name = componentType;
            remoteComponent.InstanceId = instanceId;
            remoteComponent.Parent = parentObject;

            parentObject.Components.Add(remoteComponent);

            if (_itemsDictionary.ContainsKey(remoteComponent.InstanceId))
                _itemsDictionary.Remove(remoteComponent.InstanceId);

            _itemsDictionary.Add(remoteComponent.InstanceId, remoteComponent);

            task.Process(instanceId);

            index++;

            if (index >= count)
                return REInvokeStatus.Cancel;
            
            outputValues.Add(OSCValue.Int(index));

            return REInvokeStatus.Complete;
        }

        private REInvokeStatus InvokeGetFieldsCommand(Task task, List<OSCValue> inputValues, ref List<OSCValue> outputValues)
        {
            if (inputValues.Count != 4)
                return REInvokeStatus.Cancel;

            if (inputValues[0].Type != OSCValueType.Int ||
                inputValues[1].Type != OSCValueType.Int ||
                inputValues[2].Type != OSCValueType.String ||
                inputValues[3].Type != OSCValueType.Int)
                return REInvokeStatus.Cancel;

            var index = inputValues[0].IntValue;
            var count = inputValues[1].IntValue;
            var fieldName = inputValues[2].StringValue;
            var valueType = (OSCValueType)inputValues[3].IntValue;

            var remoteComponent = GetItem(task.ParentId) as REComponent;
            if (remoteComponent == null) return REInvokeStatus.Cancel;

            var remoteField = new REField();
            remoteField.Parent = remoteComponent;
            remoteField.FieldName = fieldName;

            remoteComponent.Fields.Add(remoteField);

            task.Process(index);

            index++;

            if (index >= count)
                return REInvokeStatus.Cancel;

            outputValues.Add(OSCValue.Int(index));

            return REInvokeStatus.Complete;
        }

        private REInvokeStatus InvokeGetValueCommand(Task task, List<OSCValue> inputValues, ref List<OSCValue> outputValues)
        {
            if (inputValues.Count <= 1)
                return REInvokeStatus.Cancel;
            
            if (inputValues[0].Type != OSCValueType.Int)
                return REInvokeStatus.Cancel;
            
            var fieldIndex = inputValues[0].IntValue;

            var remoteComponent = GetItem(task.ParentId) as REComponent;
            if (remoteComponent == null) return REInvokeStatus.Cancel;

            if (fieldIndex < 0 || fieldIndex >= remoteComponent.Fields.Count)
                return REInvokeStatus.Cancel;
            
            var field = remoteComponent.Fields[fieldIndex];

            var index = 1;
            var count = inputValues.Count - index;
            var fieldValues = inputValues.GetRange(index, count);

            field.Values = fieldValues;

            task.Process(fieldIndex);

            return REInvokeStatus.Cancel;
        }

        private REInvokeStatus InvokeSetValueCommand(Task task, List<OSCValue> values, ref List<OSCValue> outputValues)
        {
            return REInvokeStatus.Cancel;
        }

        private Task CreateTask(RECommand command, int parentId, TaskProcess processCallback, TaskComplete completeCallback)
        {
            var taskId = string.Format("{0}_{1}", parentId, command);
            var task = new Task(command, parentId, taskId);

            if (_tasksDictionary.ContainsKey(taskId))
                _tasksDictionary.Remove(taskId);
            
            _tasksDictionary.Add(taskId, task);

            task.ProcessCallback = processCallback;
            task.CompleteCallback = completeCallback;

            return task;
        }

        #endregion
    }
}