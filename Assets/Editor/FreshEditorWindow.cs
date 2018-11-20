using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FreshEditorWindow : EditorWindow
{
    private string objQuery = "";
    private string objName = "";
    private string objType = "";
    private string objMember = "";
    private static Regex searchString = new Regex(@"(?<gameObjectName>.+)\.(?<componentName>.+)\.(?<memberName>.+)");
    [MenuItem("Tools/Search Tool %`")]
    public static void Create()
    {
        EditorWindow.GetWindow<FreshEditorWindow>(false, "Search Tool", true);
        //EditorWindow.GetWindow(typeof(FreshEditorWindow));
    }
    // On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI
    // On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI
    private void OnGUI()
    {
        objQuery = EditorGUILayout.TextField(objQuery);
        var matched = searchString.Match(objQuery);
        var objName = matched.Groups["gameObjectName"].Value;
        var objType = matched.Groups["componentName"].Value;
        var objMember = matched.Groups["memberName"].Value;
        if (!string.IsNullOrEmpty(objName) &&
            !string.IsNullOrEmpty(objType) &&
            !string.IsNullOrEmpty(objMember))
        {
            var gameObjs = Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(obby => obby.scene.isLoaded)         //filter for current scene
                .Where(obby => obby.name.Contains(objName));//filter for text

            foreach (var obby in gameObjs)
            {
                var comps = FindComponentInGameObject(obby, objType);
                if (comps != null)
                {
                    var info = GetMemberFromName(comps, objMember);
                    if (info != null)
                    {
                        var type = GetTypeOfMember(info);
                        if (type == (typeof(Vector3)))
                        {
                            SetValueOfMember<Vector3>(info, EditorGUILayout.Vector3Field(obby.name, GetValueOfMember<Vector3>(info, comps)), comps);
                            obby.transform.position = EditorGUILayout.Vector3Field(obby.name, obby.transform.position); //set objects transform data to our displayed vec3
                        }
                    }
                }
                else
                {
                    EditorGUILayout.LabelField(String.Format("the component \"{0}\"  can't be located", objMember));
                }             
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Enter a valid game object name.", MessageType.Warning);    //didnt find the obj
        }
    }

    // On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI
    // On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  On GUI  

    private Component FindComponentInGameObject(GameObject obby, string objType)
    {
        var components = obby.GetComponents<Component>();
        return components.FirstOrDefault(component => component.GetType().Name.Equals(objType, StringComparison.InvariantCultureIgnoreCase));
    }

    private Type GetTypeOfMember(MemberInfo info)
    {
        switch (info.MemberType)
        {
            case MemberTypes.Field:
               return ((FieldInfo)info).FieldType;
                
            case MemberTypes.Property:
                return ((PropertyInfo)info).PropertyType;
            
            default:
                return null;
        }
    }
    private void SetValueOfMember<T>(MemberInfo info, T input, object source)
    {
        switch (info.MemberType)
        {
            case MemberTypes.Field:
                ((FieldInfo)info).SetValue(source, input);
                break;
            case MemberTypes.Property:
                ((PropertyInfo)info).SetValue(source, input, null);
                break;
            default:
                break;
        }
    }
    private T GetValueOfMember<T>(MemberInfo info, object source)
    {
        switch (info.MemberType)
        {
            case MemberTypes.Field:
                return(T)((FieldInfo)info).GetValue(source);
              
            case MemberTypes.Property:
                return (T)((PropertyInfo)info).GetValue(source, null);
            default:
                return default(T);
        }
    }
    private MemberInfo GetMemberFromName(Component comps, string objmem)
    {
        MemberInfo info = comps.GetType().GetField(objmem);
        if (info == null)
        {
            info = comps.GetType().GetProperty(objmem);
        }
        return info;
    }
}
