using UnityEngine;
using System;
using UnityEditor;

/// <summary>
/// 複数アトリビュートの実装をするために定義したInterfaceTypeAttribute。中身は外部ライブラリの<see cref="InterfaceTypeAttribute"/>および
/// <see cref="WraithavenGames.UnityInterfaceSupport.InterfaceTypeDrawer"/>と同義です。
/// </summary>
public class InterfaceTypeMultiAttribute : MultiPropertyAttribute
{
	public Type type;

	/// <summary>
	/// Creates a new InterfaceType attribute.
	/// </summary>
	/// <param name="type">The type of interface which is allowed.</param>
	public InterfaceTypeMultiAttribute(Type type)
	{
		this.type = type;
	}

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.ObjectReference)
        {
            EditorGUI.LabelField(position, label.text, "InterfaceType Attribute can only be used with MonoBehaviour Components!");
            return;
        }

        // Pick a specific component
        MonoBehaviour oldComp = property.objectReferenceValue as MonoBehaviour;

        GameObject temp = null;
        string oldName = "";

        if (Event.current.type == EventType.Repaint)
        {
            if (oldComp == null)
            {
                temp = new GameObject("None [" + type.Name + "]");
                oldComp = temp.AddComponent<MonoInterfaceMulti>();
            }
            else
            {
                oldName = oldComp.name;
                oldComp.name = oldName + " [" + type.Name + "]";
            }
        }

        MonoBehaviour comp = EditorGUI.ObjectField(position, label, oldComp, typeof(MonoBehaviour), true) as MonoBehaviour;

        if (Event.current.type == EventType.Repaint)
        {
            if (temp != null)
                GameObject.DestroyImmediate(temp);
            else
                oldComp.name = oldName;
        }

        // Make sure something changed.
        if (oldComp == comp) return;

        // If a component is assigned, make sure it is the interface we are looking for.
        if (comp != null)
        {
            // Make sure component is of the right interface
            if (comp.GetType() != type)
                // Component failed. Check game object.
                comp = comp.gameObject.GetComponent(type) as MonoBehaviour;

            // Item failed test. Do not override old component
            if (comp == null) return;
        }

        property.objectReferenceValue = comp;
        property.serializedObject.ApplyModifiedProperties();

    }

    public class MonoInterfaceMulti : MonoBehaviour { }
}
