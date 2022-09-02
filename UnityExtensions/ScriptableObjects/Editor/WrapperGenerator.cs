using System;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace UnityExtensions.ScriptableObjects.Editor;

/// <summary>
///     The wrapper template needs to look something like this:
///     #USINGS#
///     namespace #NAMESPACE#
///     {
///     [System.Serializable]
///     [CreateAssetMenu(fileName = "#NAME#Variable", menuName = "Wrappers/#NAME#", order = -10000)]
///     public class #NAME#Wrapper : Wrapper
///     <#TYPE#>
///         {
///         }
///         }
/// </summary>
public class WrapperGenerator : OdinEditorWindow
{
    [BoxGroup("Generate Type Wrapper"), LabelText("Type in Class:"), Required,
     ValidateInput("MustBeValidType", "Couldn't find type, but you probably know what you're doing")]
    public string ClassName;

    [Sirenix.OdinInspector.FilePath, LabelText("Wrapper template")] 
    public string TemplatePath;

    [BoxGroup("Generate Type Wrapper")] public string[] Usings    = { "UnityEngine", "System" };
    [BoxGroup("Generate Type Wrapper")] public string   NameSpace = "UnityExtensions.ScriptableObjects";


    [BoxGroup("Generate Type Wrapper")] public (string @namespace, string assembly)[] PathsToCheck =
    {
        ("UnityEngine", "UnityEngine"),
        ("System", "mscorlib"),
        ("UnityExtensions.ScriptableObjects", "Assembly-CSharp"),
        ("UnityExtensions", "Assembly-CSharp"),
    };

    [MenuItem("Tools/Wrapper Generator")]
    private static void OpenWindow() => GetWindow<WrapperGenerator>().Show();

    private bool MustBeValidType(string type) =>
        PathsToCheck.Any(x => Type.GetType($"{x.@namespace}.{ClassName}, {x.assembly}") != null);

    [BoxGroup("Generate Type Wrapper"), Button]
    private void GenerateClass()
    {
        string upperCaseClassName = FirstLetterToUpperCaseOrConvertNullToEmptyString(ClassName);
        string? usings = Usings.Aggregate("",
                                          (aggregated, addition) =>
                                              $"{aggregated}using {addition};\n");

        string fileContents;
        if (File.Exists(TemplatePath)) fileContents = File.ReadAllText(TemplatePath);
        else
            fileContents = @"#USINGS#
namespace #NAMESPACE#
{
    [System.Serializable]
    [CreateAssetMenu(fileName = ""#NAME#Variable"", menuName = ""Wrappers/#NAME#"", order = -10000)]
    public class #NAME#Wrapper : Wrapper<#TYPE#>
    {
    }
}";

        fileContents = fileContents.Replace("#TYPE#", ClassName)
                                   .Replace("#NAME#",      upperCaseClassName)
                                   .Replace("#NAMESPACE#", NameSpace)
                                   .Replace("#USINGS#",    usings);

        string[] pathStrings = { Application.dataPath, "_Code", "Common" };
        string path = Path.Combine(pathStrings);

        string filePath = Path.Combine(path, $"{upperCaseClassName}Wrapper.cs");

        File.WriteAllText(filePath, fileContents);
    }

    /// <summary>
    ///     Returns the input string with the first character converted to uppercase, or mutates any nulls passed into
    ///     string.Empty
    /// </summary>
    private static string FirstLetterToUpperCaseOrConvertNullToEmptyString(string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;

        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }

    [Button]
    private void RefreshAssetDatabase() => AssetDatabase.Refresh();
}