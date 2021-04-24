using UnityEditor;

public static class CustomEditorKeys {
	[MenuItem("Edit/Custom Editor Keys/Redo %#z")]
	static void Redo () {
		Undo.PerformRedo();
	}

	[MenuItem( "Edit/Custom Editor Keys/AHK Integration/Play or Restart Play Mode %&#p" )]
	static void PlayOrRestartPlayMode () {
		if( EditorApplication.isPlayingOrWillChangePlaymode ) {
			EditorApplication.playModeStateChanged += WaitForPlaymodeChange;
			EditorApplication.isPlaying = false;
		} else {
			EditorApplication.isPlaying = true;
		}
	}

	private static void WaitForPlaymodeChange ( PlayModeStateChange state ) {
		if( state == PlayModeStateChange.EnteredEditMode ) {
			EditorApplication.playModeStateChanged -= WaitForPlaymodeChange;
			EditorApplication.isPlaying = true;
		}
	}

	[MenuItem( "Edit/Custom Editor Keys/AHK Integration/Stop Play Mode %&#l" )]
	static void StopPlayMode () {
		if( EditorApplication.isPlayingOrWillChangePlaymode ) {
			EditorApplication.isPlaying = false;
		}
	}
}