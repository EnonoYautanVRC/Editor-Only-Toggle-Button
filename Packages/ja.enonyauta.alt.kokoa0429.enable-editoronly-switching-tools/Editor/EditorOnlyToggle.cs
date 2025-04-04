using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorOnlyToggle
{
    // アイコンのGUIContent
    private static GUIContent iconContent;
    // Enable/DisableアイコンのためのGUIContent
    private static GUIContent enableIconContent;
    private static GUIContent disableIconContent;

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        // 可視性アイコンを使用
        iconContent = EditorGUIUtility.IconContent("d_CustomTool");
        // Enable/Disableアイコンを使用
        enableIconContent = EditorGUIUtility.IconContent("d_VisibilityOn");
        disableIconContent = EditorGUIUtility.IconContent("d_VisibilityOff");
    }

    private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;
        
        // UntaggedかEditorOnlyの場合のみボタンを表示
        if (obj.tag != "Untagged" && obj.tag != "EditorOnly") return;
        
        // Ctrlキーが押されている場合のみボタンを表示
        if (!Event.current.control) return;
        var leftMargin = 0;
        var rightMargin = 35;
        var buttonWidth = 15;
        // EditorOnlyトグル用のRect
        var toggleRect = new Rect(selectionRect);
        toggleRect.x = selectionRect.xMax - rightMargin; // 階層に関係なく右端から固定位置に配置
        // toggleRect.x = leftMargin + buttonWidth; // 階層に関係なく右端から固定位置に配置
        toggleRect.width = buttonWidth;
        
        // Enable/Disableトグル用のRect
        var enableToggleRect = new Rect(selectionRect);
        enableToggleRect.x = selectionRect.xMax - rightMargin - buttonWidth; // 階層に関係なく右端から固定位置に配置
        // enableToggleRect.x = leftMargin; // 階層に関係なく右端から固定位置に配置
        enableToggleRect.width = buttonWidth;

        var editorOnly = obj.tag == "EditorOnly";

        // 元の色を保存
        var originalColor = GUI.color;
        
        // アイコンの色を設定（EditorOnlyの場合は明るく、そうでない場合は暗く）
        GUI.color = editorOnly ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);

        // ボタンとしてアイコンを表示
        var newEditorOnly = GUI.Toggle(toggleRect, editorOnly, iconContent, GUI.skin.button);

        // 色を元に戻す
        GUI.color = originalColor;

        // 状態が変更された場合はタグを更新
        if (newEditorOnly != editorOnly)
        {
            obj.tag = newEditorOnly ? "EditorOnly" : "Untagged";
        }
        
        // Enable/Disableトグルの表示
        var isEnabled = obj.activeSelf;
        
        // 状態に応じたアイコンを選択（アイコンは状態によって変わるが色は統一）
        var iconToUse = isEnabled ? enableIconContent : disableIconContent;

        // Enable/Disableボタンの色はEnable/Disable状態に関わらず統一
        // EditorOnlyの状態は影響しない
        GUI.color = isEnabled ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
        // ボタンとしてEnable/Disableアイコンを表示
        var newIsEnabled = GUI.Toggle(enableToggleRect, isEnabled, iconToUse, GUI.skin.button);
        
        // 色を元に戻す（念のため）
        GUI.color = originalColor;
        
        // 状態が変更された場合はGameObjectのアクティブ状態を更新
        if (newIsEnabled != isEnabled)
        {
            obj.SetActive(newIsEnabled);
        }
    }
}
