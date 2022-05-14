using UnityEngine;
using UnityEditor;
using System.IO;
//using System.Collections.Generic;
//using UnityEditorInternal;

namespace RuneFoxMods.CustomItemDisplays
{
  [CustomEditor(typeof(CustomItemDisplayInfo))]
  public class CustomItemDisplayInfoPropertyDrawer : Editor
  {
    //private ReorderableList list;
    //private void OnEnable()
    //{
    //  list = new ReorderableList(serializedObject, serializedObject.FindProperty("BaseItemDisplays"), true, true, true, true);
    //}

    public override void OnInspectorGUI()
    {
      //serializedObject.Update();
      //list.DoLayoutList();
      //serializedObject.ApplyModifiedProperties();
      base.OnInspectorGUI();

      //draw line
      //Rect rect = EditorGUILayout.GetControlRect(false);
      //rect.y += EditorGUIUtility.singleLineHeight/2;
      //rect.height = 1;
      //EditorGUI.DrawRect(rect, Color.gray);

      HorizontalLine(Color.grey);

      if (GUILayout.Button("Build"))
      {
        Build(serializedObject.targetObject as CustomItemDisplayInfo);
      }
      //EditorGUI.DrawRect(new Rect()
    }

    private void Build(CustomItemDisplayInfo customItemDisplayInfo)
    {
      if (customItemDisplayInfo.assetInfo == null) customItemDisplayInfo.InitializeAssetInfo();

      var path = Path.Combine(customItemDisplayInfo.assetInfo.modFolder, customItemDisplayInfo.modInfo.name + "CustomItemDisplay.cs");
      //dynamicSkinInfo.assetInfo.CreateNecessaryAssetsAndFillPaths(dynamicSkinInfo.modInfo.regenerateAssemblyDefinition);
      var DynamicSkinCode = new CustomItemDisplayTemplate(customItemDisplayInfo);
      File.WriteAllText(path, DynamicSkinCode.TransformText());

      //turns out that this still works the same even if the package is in the package cache
      File.Copy("Packages/com.runefox237.customitemdisplays/Editor/CopyPath/CustomItemDisplays.cs", customItemDisplayInfo.assetInfo.modFolder + "/CustomItemDisplays.cs", true);
      
      AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
      AssetDatabase.ImportAsset(customItemDisplayInfo.assetInfo.modFolder + "/CustomItemDisplays.cs", ImportAssetOptions.ForceUpdate);
    }

    // utility method
    static void HorizontalLine ( Color color ) {
      GUIStyle horizontalLine = new GUIStyle();
      horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
      horizontalLine.margin = new RectOffset(0, 0, 4, 4);
      horizontalLine.fixedHeight = 1;
      var c = GUI.color;
      GUI.color = color;
      GUILayout.Box( GUIContent.none, horizontalLine );
      GUI.color = c;
    }
  }


    [CustomPropertyDrawer(typeof(CustomItemDisplayInfo.ItemDisplay))]
  public class ItemDisplayDrawer : PropertyDrawer
  {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      //Debug.Log(EditorGUIUtility.labelWidth + "  " + EditorGUIUtility.fieldWidth + "   " +  Screen.width);
      //Debug.Log(EditorGUIUtility.labelWidth + "   " +  Screen.width);
      float height = EditorGUIUtility.singleLineHeight * 6 + EditorGUIUtility.standardVerticalSpacing * 5;
      //return Screen.width >= 433 ? height : (height * 2);
      return height;
    }
  
  
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      float width = EditorGUIUtility.labelWidth;
      EditorGUIUtility.labelWidth = 60;
  
      label = EditorGUI.BeginProperty(position, label, property);
      label.text = property.FindPropertyRelative("childName").stringValue;
  
      EditorGUIUtility.labelWidth = width;
      //var indent = EditorGUI.indentLevel;
      //EditorGUI.indentLevel = 0;
      //EditorGUIUtility.labelWidth = 160;
      
      float lineHeight = EditorGUIUtility.singleLineHeight;
  
      Rect contentPos = EditorGUI.PrefixLabel(position, label);
      
      var stretch = contentPos.x * 0.4f;
      contentPos.x = contentPos.x * 0.6f;
      contentPos.width += stretch;
  
      contentPos.height = EditorGUIUtility.singleLineHeight;
      // Draw fields - pass GUIContent.none to each so they are drawn without labels
      EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("childName"), new GUIContent("Child Name"));
      contentPos.y += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
      EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("localPos"), new GUIContent("Local Pos"));
      contentPos.y += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
      EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("localAngle"), new GUIContent("Local Angle"));
      contentPos.y += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
      EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("localScale"), new GUIContent("Local Scale"));
      contentPos.y += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
  
      //var parsable = ; 
      //parsable = EditorGUI.TextField(contentPos, parsable);
      EditorGUI.PropertyField(contentPos, property.FindPropertyRelative("import"), GUIContent.none);
      contentPos.y += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
      
      //contentPos.x += 45;
      //contentPos.width -= 45;
      
      if (GUI.Button(contentPos, new GUIContent("Update")))
      {
        //Pelvis,0.1643,-0.112,0.0542,85.66718,143.032,353.5245,0.51513,0.51513,0.51513
        var parsable = property.FindPropertyRelative("import").stringValue;
        property.FindPropertyRelative("import").stringValue = ""; //clear parsable since it appears on all item displays atm 
        //Debug.Log(parsable);
        var list = parsable.Split(',');
        //Debug.Log(list.Length);
        if (list.Length != 1)
        {
          Vector3 localPos = new Vector3();
          Vector3 localAngle = new Vector3();
          Vector3 localScale = new Vector3();
  
          string childname = list[0];
          localPos.x = float.Parse(list[1]);
          localPos.y = float.Parse(list[2]);
          localPos.z = float.Parse(list[3]);
          localAngle.x = float.Parse(list[4]);
          localAngle.y = float.Parse(list[5]);
          localAngle.z = float.Parse(list[6]);
          localScale.x = float.Parse(list[7]);
          localScale.y = float.Parse(list[8]);
          localScale.z = float.Parse(list[9]);
  
          property.FindPropertyRelative("childName").stringValue = childname;
          property.FindPropertyRelative("localPos").vector3Value = localPos;
          property.FindPropertyRelative("localAngle").vector3Value = localAngle;
          property.FindPropertyRelative("localScale").vector3Value = localScale;
          //var test = property.serializedObject.targetObject as CustomItemDisplayInfo.ItemDisplay;
          property.serializedObject.ApplyModifiedProperties();
        }
      }
  
      // Set indent back to what it was
      //EditorGUI.indentLevel = indent;
      EditorGUIUtility.labelWidth = width;
  
  
      EditorGUI.EndProperty();
    }
  }
}
