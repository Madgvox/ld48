using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// [http://www.tallior.com/extending-unity-inspectors/]
/// <summary>
/// A base class for creating editors that decorate Unity's built-in editor types.
/// </summary>
public abstract class DecoratorEditor : Editor {
	#region Editor Fields

	/// <summary>
	/// Type object for the internally used (decorated) editor.
	/// </summary>
	private System.Type decoratedEditorType;

	/// <summary>
	/// Type object for the object that is edited by this editor.
	/// </summary>
	private System.Type editedObjectType;

	private Editor editorInstance;

	#endregion

	public delegate void EditorEventDelegate ( Editor editor );
	private static Dictionary<string, EditorEventDelegate> decoratedMethods = new Dictionary<string, EditorEventDelegate>();

	private static Assembly editorAssembly = Assembly.GetAssembly( typeof( Editor ) );

	protected Editor EditorInstance {
		get {
			if( editorInstance == null && targets != null && targets.Length > 0 ) {
				editorInstance = CreateEditor( targets, decoratedEditorType );
			}

			if( editorInstance == null ) {
				Debug.LogError( "Could not create editor !" );
			}

			return editorInstance;
		}
	}

	public DecoratorEditor ( string editorTypeName ) {
		this.decoratedEditorType = editorAssembly.GetTypes().Where( t => t.Name == editorTypeName ).FirstOrDefault();

		Init();

		// Check CustomEditor types.
		var originalEditedType = GetCustomEditorType( decoratedEditorType );

		if( originalEditedType != editedObjectType ) {
			throw new System.ArgumentException(
				string.Format( "Type {0} does not match the editor {1} type {2}",
						  editedObjectType, editorTypeName, originalEditedType ) );
		}
	}

	private System.Type GetCustomEditorType ( System.Type type ) {
		var flags = BindingFlags.NonPublic | BindingFlags.Instance;

		var attributes = type.GetCustomAttributes( typeof( CustomEditor ), true ) as CustomEditor[];
		var field = attributes.Select( editor => editor.GetType().GetField( "m_InspectedType", flags ) ).First();

		return field.GetValue( attributes[ 0 ] ) as System.Type;
	}

	private void Init () {
		var flags = BindingFlags.NonPublic | BindingFlags.Instance;

		var attributes = this.GetType().GetCustomAttributes( typeof( CustomEditor ), true ) as CustomEditor[];
		var field = attributes.Select( editor => editor.GetType().GetField( "m_InspectedType", flags ) ).First();

		editedObjectType = field.GetValue( attributes[ 0 ] ) as System.Type;
	}

	void OnDisable () {
		if( editorInstance != null ) {
			DestroyImmediate( editorInstance );
		}
	}

	/// <summary>
	/// Delegates a method call with the given name to the decorated editor instance.
	/// </summary>
	protected void CallInspectorMethod ( string methodName ) {
		EditorEventDelegate method = null;

		// Add MethodInfo to cache
		if( !decoratedMethods.ContainsKey( methodName ) ) {
			var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

			var info = decoratedEditorType.GetMethod( methodName, flags );

			Debug.Log( methodName + " " + info );

			if( info != null ) {
				decoratedMethods[ methodName ] = (EditorEventDelegate)Delegate.CreateDelegate( typeof( EditorEventDelegate ), info );
			} else {
				decoratedMethods[ methodName ] = null;
			}
		} else {
			method = decoratedMethods[ methodName ];
		}

		if( method != null ) {
			method( EditorInstance );
		}
	}

	public void OnSceneGUI () {
		CallInspectorMethod( "OnSceneGUI" );
	}

	protected override void OnHeaderGUI () {
		CallInspectorMethod( "OnHeaderGUI" );
	}

	public override void OnInspectorGUI () {
		EditorInstance.OnInspectorGUI();
	}

	public override void DrawPreview ( Rect previewArea ) {
		EditorInstance.DrawPreview( previewArea );
	}

	public override string GetInfoString () {
		return EditorInstance.GetInfoString();
	}

	public override GUIContent GetPreviewTitle () {
		return EditorInstance.GetPreviewTitle();
	}

	public override bool HasPreviewGUI () {
		return EditorInstance.HasPreviewGUI();
	}

	public override void OnInteractivePreviewGUI ( Rect r, GUIStyle background ) {
		EditorInstance.OnInteractivePreviewGUI( r, background );
	}

	public override void OnPreviewGUI ( Rect r, GUIStyle background ) {
		EditorInstance.OnPreviewGUI( r, background );
	}

	public override void OnPreviewSettings () {
		EditorInstance.OnPreviewSettings();
	}

	public override void ReloadPreviewInstances () {
		EditorInstance.ReloadPreviewInstances();
	}

	public override Texture2D RenderStaticPreview ( string assetPath, UnityEngine.Object[] subAssets, int width, int height ) {
		return EditorInstance.RenderStaticPreview( assetPath, subAssets, width, height );
	}

	public override bool RequiresConstantRepaint () {
		return EditorInstance.RequiresConstantRepaint();
	}

	public override bool UseDefaultMargins () {
		return EditorInstance.UseDefaultMargins();
	}
}