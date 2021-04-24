using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
//[CustomEditor( typeof( TextureImporter ) )]
public class CopyImportSettingsEditor : DecoratorEditor {

	public CopyImportSettingsEditor () : base( "TextureImporterInspector" ) {}

	Object copyFrom;

	bool copying;

	GUIStyle buttonStyle;

	public override void OnInspectorGUI () {
		base.OnInspectorGUI();

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		var evt = Event.current;

		GUILayout.BeginHorizontal();
		copyFrom = EditorGUILayout.ObjectField( copyFrom, typeof( Texture2D ), false );
		GUI.enabled = copyFrom != null;
		if( buttonStyle == null ) {
			buttonStyle = new GUIStyle( "button" );
			buttonStyle.margin = new RectOffset( 0, 0, 0, 0 );
		}
		if( GUILayout.Button( "Copy", buttonStyle ) ) {
			CopyImportSettings();
			copyFrom = null;
		}
		GUI.enabled = true;
		GUILayout.EndHorizontal();
	}

	void CopyImportSettings () {
		foreach( var t in targets ) {
			var dstImporter = t as TextureImporter;
			var sourcePath = AssetDatabase.GetAssetPath( copyFrom );
			var sourceImporter = AssetImporter.GetAtPath( sourcePath );
			EditorUtility.CopySerialized( sourceImporter, target );

			dstImporter.SaveAndReimport();

			Debug.Log( "Copied importer settings from <b><i>" + sourcePath + "</i></b> to <b><i>" + dstImporter.assetPath + "</i></b>" );
		}
	}
}
