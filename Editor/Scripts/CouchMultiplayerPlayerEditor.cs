using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SLIDDES.Multiplayer.Couch.Editor
{
    [CustomEditor(typeof(CouchMultiplayerPlayer))]
    public class CouchMultiplayerPlayerEditor : UnityEditor.Editor
    {
        private bool foldoutUI;
        private bool foldoutFirstPerson;
        private bool foldoutLobby;

        private SerializedProperty propertyCameraUI;

        private SerializedProperty propertyCameraOverlay;
        private SerializedProperty propertyLayerMaskCamera;
        private SerializedProperty propertyLayerMaskCameraOverlay;
        private SerializedProperty propertyFirstPersonGameObjects;
        private SerializedProperty propertyThirdPersonGameObjects;

        private SerializedProperty propertyCanJoinLobby;
        private SerializedProperty propertyCanLeaveLobby;
        private SerializedProperty propertyCanStartLobby;
        private SerializedProperty propertyCanSendInputToLobby;

        private void OnEnable()
        {
            propertyCameraUI = serializedObject.FindProperty("cameraUI");

            propertyCameraOverlay = serializedObject.FindProperty("cameraOverlay");
            propertyLayerMaskCamera = serializedObject.FindProperty("layerMaskCamera");
            propertyLayerMaskCameraOverlay = serializedObject.FindProperty("layerMaskCameraOverlay");
            propertyFirstPersonGameObjects = serializedObject.FindProperty("firstPersonGameObjects");
            propertyThirdPersonGameObjects = serializedObject.FindProperty("thirdPersonGameObjects");

            propertyCanJoinLobby = serializedObject.FindProperty("canJoinLobby");
            propertyCanLeaveLobby = serializedObject.FindProperty("canLeaveLobby");
            propertyCanStartLobby = serializedObject.FindProperty("canStartLobby");
            propertyCanSendInputToLobby = serializedObject.FindProperty("canSendInputToLobby");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawUI();
            DrawFirstPerson();
            DrawLobby();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            foldoutUI = EditorGUILayout.Foldout(foldoutUI, new GUIContent("UI"), true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });
            if(foldoutUI)
            {

                EditorGUILayout.PropertyField(propertyCameraUI);

            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void DrawFirstPerson()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            foldoutFirstPerson = EditorGUILayout.Foldout(foldoutFirstPerson, new GUIContent("First Person"), true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });
            if(foldoutFirstPerson)
            {
                EditorGUILayout.PropertyField(propertyCameraOverlay);
                if(propertyCameraOverlay.objectReferenceValue != null)
                {
                    EditorGUILayout.PropertyField(propertyLayerMaskCamera);
                    EditorGUILayout.PropertyField(propertyLayerMaskCameraOverlay);
                    EditorGUILayout.PropertyField(propertyFirstPersonGameObjects);
                    EditorGUILayout.PropertyField(propertyThirdPersonGameObjects);
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void DrawLobby()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            foldoutLobby = EditorGUILayout.Foldout(foldoutLobby, new GUIContent("Lobby"), true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });
            if(foldoutLobby)
            {
                EditorGUILayout.PropertyField(propertyCanJoinLobby);
                EditorGUILayout.PropertyField(propertyCanLeaveLobby);
                EditorGUILayout.PropertyField(propertyCanStartLobby);
                EditorGUILayout.PropertyField(propertyCanSendInputToLobby);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
    }
}
