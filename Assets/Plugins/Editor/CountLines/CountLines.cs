using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CountLines : EditorWindow {
	string results;
	List<File> files = new List<File>();
	System.Text.StringBuilder strStats;
	Vector2 scrollPosition = new Vector2( 0, 0 );
	class File {
		public string name;
		public int lines;

		public File ( string name, int nbLines ) {
			this.name = name;
			this.lines = nbLines;
		}
	}

	void OnGUI () {
		if( GUILayout.Button( "Refresh" ) ) {
			DoCountLines();
		}
		GUILayout.Label( results );
		scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, EditorStyles.helpBox );
		EditorGUILayout.BeginVertical(  );

		GUILayout.BeginHorizontal();

		var style = new GUIStyle( EditorStyles.miniLabel );
		style.margin = new RectOffset(0,10,0,5);
		style.alignment = TextAnchor.LowerRight;
		style.richText = true;

		GUILayout.BeginVertical( GUILayout.ExpandWidth( false ) );
		foreach( var file in files ) {
			GUILayout.Label( file.name, style );
		}
		GUILayout.EndVertical();
		GUILayout.BeginVertical( GUILayout.ExpandWidth( false ), GUILayout.MaxWidth( 50 ) );

		style.alignment = TextAnchor.LowerLeft;
		foreach( var file in files ) {
			GUILayout.Label( file.lines.ToString(), style );
		}
		GUILayout.EndVertical();

		GUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();
	}


	[MenuItem( "Tools/Count Lines", priority = int.MaxValue )]
	public static void Init () {
		CountLines window = EditorWindow.GetWindow<CountLines>( "Count Lines" );
		window.Show();
		window.Focus();
		window.DoCountLines();
	}

	void DoCountLines () {
		string strDir = System.IO.Directory.GetCurrentDirectory();
		strDir += @"/Assets";
		files.Clear();
		ProcessDirectory( strDir );

		int totalLines = 0;
		foreach( File f in files ) {
			totalLines += f.lines;
			var filename = System.IO.Path.GetFileName( f.name );
			f.name = f.name.Replace( strDir + @"\", "" );
			f.name = f.name.Replace( @"\", "/" );
			f.name = "<color=#777777>" + f.name;
			f.name = f.name.Replace( filename, "" ) + "</color>" + filename;
		}

		strStats = new System.Text.StringBuilder();
		strStats.Append( "Number of Files: " + files.Count + "\n" );
		strStats.Append( "Number of Lines: " + totalLines );

		files.Sort( ( f1, f2 ) => f2.lines - f1.lines );

		results = strStats.ToString();
	}

	void ProcessDirectory ( string dir ) {
		string[] strArrFiles = System.IO.Directory.GetFiles( dir, "*.cs" );
		foreach( string strFileName in strArrFiles )
			ProcessFile( strFileName );

		string[] strArrSubDir = System.IO.Directory.GetDirectories( dir );
		foreach( string strSubDir in strArrSubDir ) {
			var dirName = System.IO.Path.GetFileNameWithoutExtension( strSubDir );
			if( dirName == "Plugins" || dirName == "PostProcessing" ) continue;
			ProcessDirectory( strSubDir );
		}
	}

	void ProcessFile ( string filename ) {
		System.IO.StreamReader reader = System.IO.File.OpenText( filename );
		int iLineCount = 0;
		while( reader.Peek() >= 0 ) {
			reader.ReadLine();
			++iLineCount;
		}
		files.Add( new File( filename, iLineCount ) );
		reader.Close();
	}
}