/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Editor;

namespace extRemoteEditor.Editor
{
	[CustomEditor(typeof(REOSCComponent), true)]
	public class REOSCComponentEditor : UnityEditor.Editor
	{
		#region Static Private Vars

		private static readonly GUIContent _settingsTitleContent = new GUIContent("Settings:");

		private static readonly GUIContent _transmitterContent = new GUIContent("Transmitter:");

		private static readonly GUIContent _receiverContent = new GUIContent("Receiver:");

		private static readonly GUIContent _addressContent = new GUIContent("Address:");

		#endregion

		#region Private Vars

		private SerializedProperty _transmitterProperty;

		private SerializedProperty _addressProperty;

		private SerializedProperty _receiverProperty;

		#endregion

		#region Unity Methods

		protected virtual void OnEnable()
		{
			_addressProperty = serializedObject.FindProperty("address");
			_transmitterProperty = serializedObject.FindProperty("transmitter");
			_receiverProperty = serializedObject.FindProperty("receiver");
			_settingsTitleContent.text = string.Format("{0} Settings:", target.GetType().Name);
		}

		protected virtual void OnDisable()
		{ }

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();

			// LOGO
			GUILayout.Space(10);
			OSCEditorLayout.DrawLogo();
			GUILayout.Space(5);

			GUILayout.BeginVertical("box");

			EditorGUILayout.LabelField(_settingsTitleContent, EditorStyles.boldLabel);
			GUILayout.BeginVertical("box");

			EditorGUILayout.PropertyField(_addressProperty, _addressContent);

			OSCEditorLayout.TransmittersPopup(_transmitterProperty, _transmitterContent);

			OSCEditorLayout.ReceiversPopup(_receiverProperty, _receiverContent);

			EditorGUILayout.EndVertical();

			EditorGUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}