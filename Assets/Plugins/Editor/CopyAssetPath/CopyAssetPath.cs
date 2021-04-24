using UnityEditor;
using UnityEngine;

public static class CopyAssetPath {
	[MenuItem("Assets/Copy Asset Path", true, 200 )]
	static bool ValidateGetPath () {
		var id = Selection.activeInstanceID;
		return AssetDatabase.Contains( id );
	}

	[MenuItem("Assets/Copy Asset Path", false, 200 )]
	static void GetPath () {
		var path = AssetDatabase.GetAssetPath( Selection.activeObject );
		var fullPath = Application.dataPath;
		fullPath = fullPath.Replace( "/Assets", "" );
		fullPath += "/" + path;
		EditorGUIUtility.systemCopyBuffer = fullPath;
		Debug.Log( string.Format( "Copied asset path to the clipboard: <b>{0}</b>", fullPath ) );
	}
}
