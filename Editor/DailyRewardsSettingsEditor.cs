using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DGames.DailyRewards.Editor
{
    
        [CustomEditor(typeof(DailyRewardSettings))]

    public class DailyRewardSettingsEditor : UnityEditor.Editor
    {

        public const string SYMBOL = "DAILY_REWARD";
        private SerializedProperty _enableProperty;
        private SerializedProperty _rewardsProperty;

        private void OnEnable()
        {
            _enableProperty = serializedObject.FindProperty(DailyRewardSettings.ACTIVE_FIELD);
            _rewardsProperty = serializedObject.FindProperty(DailyRewardSettings.REWARDS);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_enableProperty);
            if (EditorGUI.EndChangeCheck())
            {
                ScriptingDefineSymbolHandler.HandleScriptingSymbol(BuildTargetGroup.Android,_enableProperty.boolValue,SYMBOL);
                ScriptingDefineSymbolHandler.HandleScriptingSymbol(BuildTargetGroup.iOS,_enableProperty.boolValue,SYMBOL);
            }
            EditorGUI.BeginDisabledGroup(!_enableProperty.boolValue);

            EditorGUILayout.PropertyField(_rewardsProperty);
            EditorGUI.EndDisabledGroup();

            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
            
            DrawFixIfNeeded();
        }
        
        private void DrawFixIfNeeded()
        {
            if (HasSymbolsProblem())
            {
                if (GUILayout.Button("Fix Missing Symbols"))
                {
                    FixSymbolsProblem();
                }
            }
        }
        
        private  bool HasSymbolsProblem()
        {
           
            return _enableProperty.boolValue &&
                (!ScriptingDefineSymbolHandler.HaveBuildSymbol(BuildTargetGroup.iOS, SYMBOL) ||
                 !ScriptingDefineSymbolHandler.HaveBuildSymbol(BuildTargetGroup.Android,
                     SYMBOL)) || !_enableProperty.boolValue &&
                (ScriptingDefineSymbolHandler.HaveBuildSymbol(BuildTargetGroup.iOS, SYMBOL) ||
                 ScriptingDefineSymbolHandler.HaveBuildSymbol(BuildTargetGroup.Android,
                     SYMBOL));
        }

        private void FixSymbolsProblem()
        {
            ScriptingDefineSymbolHandler.HandleScriptingSymbol(BuildTargetGroup.iOS, _enableProperty.boolValue, SYMBOL);
            ScriptingDefineSymbolHandler.HandleScriptingSymbol(BuildTargetGroup.Android, _enableProperty.boolValue, SYMBOL);
        }
    }
    
     public static class ScriptingDefineSymbolHandler
    {
        public static bool HaveBuildSymbol(BuildTargetGroup group, string symbol)
        {
            var scriptingDefineSymbolsForGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            var strings = scriptingDefineSymbolsForGroup.Split(';').ToList();

            return strings.Contains(symbol);
        }

        public static void AddBuildSymbol(BuildTargetGroup group, string symbol)
        {
            if (HaveBuildSymbol(group, symbol))
                return;
            var scriptingDefineSymbolsForGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            var strings = scriptingDefineSymbolsForGroup.Split(';').ToList();
            strings.Add(symbol);
            var str = "";
            foreach (var s in strings)
            {
                str += s + ";";
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, str);
        }

        public static void RemoveBuildSymbol(BuildTargetGroup group, string symbol)
        {
            if (!HaveBuildSymbol(group, symbol))
                return;
            var scriptingDefineSymbolsForGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            var strings = scriptingDefineSymbolsForGroup.Split(';').ToList();
            strings.Remove(symbol);
            var str = "";
            foreach (var s in strings)
            {
                str += s + ";";
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, str);
        }

        public static void HandleScriptingSymbol(BuildTargetGroup buildTargetGroup, bool enable, string key)
        {
            var scriptingDefineSymbolsForGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var strings = scriptingDefineSymbolsForGroup.Split(';').ToList();

            if (enable)
            {
                strings.Add(key);
            }
            else
            {
                strings.Remove(key);
            }


            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", strings.Distinct()));
        }
    }

}