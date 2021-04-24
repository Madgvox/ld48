using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

public static class GUIStyleStateExtensions {
	public static Texture2D GetScaledBackground ( this GUIStyleState state ) {
		if( state.scaledBackgrounds.Length > 0 ) {
			return state.scaledBackgrounds[ 0 ];
		} else {
			return null;
		}
	}

	public static void SetScaledBackground ( this GUIStyleState state, Texture2D image ) {
		if( state.scaledBackgrounds.Length > 0 ) {
			state.scaledBackgrounds[ 0 ] = image;
		} else {
			state.scaledBackgrounds = new[] { image };
		}
	}

	public static void ClearToNormal ( this GUIStyleState state, GUIStyleState normal ) {
		state.background = null;
		state.scaledBackgrounds = null;
		state.textColor = normal.textColor;
	}

	public static bool Equals ( this GUIStyleState lhs, GUIStyleState rhs ) {
		if( lhs.background != rhs.background ) return false;
		if( lhs.scaledBackgrounds.Length != rhs.scaledBackgrounds.Length ) return false;
		if( lhs.scaledBackgrounds.Length > 0 ) {
			if( lhs.scaledBackgrounds[ 0 ] != rhs.scaledBackgrounds[ 0 ] ) return false;
		}
		if( lhs.textColor != rhs.textColor ) return false;

		return true;
	}
}

[CustomEditor( typeof( GUISkin ) )]
public class GUISkinEditor : Editor {
	class Faders {
		public AnimBool globalSettingsBool;

		public Faders ( GUISkinEditor editor ) {
			globalSettingsBool = new AnimBool( editor.Repaint );
		}
	}

	class Properties {
		public SerializedProperty globalFont;
		public SerializedProperty doubleClickSelectsWord;
		public SerializedProperty tripleClickSelectsLine;
		public SerializedProperty cursorFlashSpeed;
		public SerializedProperty cursorColor;
		public SerializedProperty selectionColor;

		public Properties ( SerializedObject skinObject ) {
			globalFont = skinObject.FindProperty( "m_Font" );

			var settings = skinObject.FindProperty( "m_Settings" );
			doubleClickSelectsWord = settings.FindPropertyRelative( "m_DoubleClickSelectsWord" );
			tripleClickSelectsLine = settings.FindPropertyRelative( "m_TripleClickSelectsLine" );
			cursorFlashSpeed = settings.FindPropertyRelative( "m_CursorFlashSpeed" );
			cursorColor = settings.FindPropertyRelative( "m_CursorColor" );
			selectionColor = settings.FindPropertyRelative( "m_SelectionColor" );
		}
	}

	public static class Styles {
		public static GUIStyle sectionHeading = new GUIStyle( "IN TitleText" );

		public static GUIStyle alignLeft = new GUIStyle( EditorStyles.miniButtonLeft );
		public static GUIStyle alignMiddle = new GUIStyle( EditorStyles.miniButtonMid );
		public static GUIStyle alignRight = new GUIStyle( EditorStyles.miniButtonRight );

		static Styles () {
			alignLeft.padding.left = 2;
			alignLeft.padding.right = 2;

			alignMiddle.padding.left = 2;
			alignMiddle.padding.right = 2;

			alignRight.padding.left = 2;
			alignRight.padding.right = 2;

			alignLeft.padding.top = 0;
			alignLeft.padding.bottom = 0;

			alignMiddle.padding.top = 0;
			alignMiddle.padding.bottom = 0;

			alignRight.padding.top = 0;
			alignRight.padding.bottom = 0;
		}
	}

	new GUISkin target;

	Faders faders;
	Properties properties;


	GUIStyleEditor buttonEditor;

	int selectedStyle;

	List<GUIContent> styleNames = new List<GUIContent>();
	GUIContent[] styleNameArr;
	List<GUIStyleEditor> styleEditors = new List<GUIStyleEditor>();

	private void OnEnable () {
		target = (GUISkin)base.target;

		properties = new Properties( serializedObject );

		faders = new Faders( this );

		buttonEditor = new GUIStyleEditor( target, target.button );

		MakeStyleList();

		selectedStyle = EditorPrefs.GetInt( "guiskineditor.editingStyle", 0 );

		if( selectedStyle > styleEditors.Count - 1 ) {
			selectedStyle = 0;
			EditorPrefs.SetInt( "guiskineditor.editingStyle", selectedStyle );
		}

		//var type = typeof( GUIUtility );
		//var method = type.GetMethod( "GetDefaultSkin", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { }, null );
		//var defaultSkin = (GUISkin)method.Invoke( null, null );

		//Debug.Log( defaultSkin.font );
	}

	private void MakeStyleList () {
		styleEditors = new List<GUIStyleEditor>();
		styleNames = new List<GUIContent>();
		styleEditors.Add( new GUIStyleEditor( target, target.scrollView ) );
		styleEditors.Add( new GUIStyleEditor( target, target.verticalScrollbarDownButton ) );
		styleEditors.Add( new GUIStyleEditor( target, target.verticalScrollbarUpButton ) );
		styleEditors.Add( new GUIStyleEditor( target, target.verticalScrollbarThumb ) );
		styleEditors.Add( new GUIStyleEditor( target, target.verticalScrollbar ) );
		styleEditors.Add( new GUIStyleEditor( target, target.horizontalScrollbarRightButton ) );
		styleEditors.Add( new GUIStyleEditor( target, target.horizontalScrollbarLeftButton ) );
		styleEditors.Add( new GUIStyleEditor( target, target.horizontalScrollbarThumb ) );
		styleEditors.Add( new GUIStyleEditor( target, target.horizontalScrollbar ) );
		styleEditors.Add( new GUIStyleEditor( target, target.verticalSliderThumb ) );
		styleEditors.Add( new GUIStyleEditor( target, target.verticalSlider ) );
		styleEditors.Add( new GUIStyleEditor( target, target.horizontalSliderThumb ) );
		styleEditors.Add( new GUIStyleEditor( target, target.window ) );
		styleEditors.Add( new GUIStyleEditor( target, target.toggle ) );
		styleEditors.Add( new GUIStyleEditor( target, target.button ) );
		styleEditors.Add( new GUIStyleEditor( target, target.textArea ) );
		styleEditors.Add( new GUIStyleEditor( target, target.textField ) );
		styleEditors.Add( new GUIStyleEditor( target, target.label ) );
		styleEditors.Add( new GUIStyleEditor( target, target.box ) );
		styleEditors.Add( new GUIStyleEditor( target, target.horizontalSlider ) );

		styleNames.Add( new GUIContent( "scrollView" ) );
		styleNames.Add( new GUIContent( "verticalScrollbarDownButton" ) );
		styleNames.Add( new GUIContent( "verticalScrollbarUpButton" ) );
		styleNames.Add( new GUIContent( "verticalScrollbarThumb" ) );
		styleNames.Add( new GUIContent( "verticalScrollbar" ) );
		styleNames.Add( new GUIContent( "horizontalScrollbarRightButton" ) );
		styleNames.Add( new GUIContent( "horizontalScrollbarLeftButton" ) );
		styleNames.Add( new GUIContent( "horizontalScrollbarThumb" ) );
		styleNames.Add( new GUIContent( "horizontalScrollbar" ) );
		styleNames.Add( new GUIContent( "verticalSliderThumb" ) );
		styleNames.Add( new GUIContent( "verticalSlider" ) );
		styleNames.Add( new GUIContent( "horizontalSliderThumb" ) );
		styleNames.Add( new GUIContent( "window" ) );
		styleNames.Add( new GUIContent( "toggle" ) );
		styleNames.Add( new GUIContent( "button" ) );
		styleNames.Add( new GUIContent( "textArea" ) );
		styleNames.Add( new GUIContent( "textField" ) );
		styleNames.Add( new GUIContent( "label" ) );
		styleNames.Add( new GUIContent( "box" ) );
		styleNames.Add( new GUIContent( "horizontalSlider" ) );

		for( int i = 0; i < target.customStyles.Length; i++ ) {
			styleEditors.Add( new GUIStyleEditor( target, target.customStyles[ i ] ) );
			styleNames.Add( new GUIContent( target.customStyles[ i ].name ) );
		}

		styleNameArr = styleNames.ToArray();
	}

	public override void OnInspectorGUI () {
		serializedObject.Update();

		//EditorGUILayout.BeginVertical( EditorStyles.helpBox );

		//faders.globalSettingsBool.target = GUILayout.Toggle( faders.globalSettingsBool.target, "Global Settings", EditorStyles.foldout );

		//if( EditorGUILayout.BeginFadeGroup( faders.globalSettingsBool.faded ) ) {

		//	EditorGUILayout.PropertyField( properties.globalFont, new GUIContent( "Global Font" ) );
		//	EditorGUILayout.PropertyField( properties.cursorColor );
		//	EditorGUILayout.PropertyField( properties.selectionColor );
		//	EditorGUILayout.PropertyField( properties.cursorFlashSpeed );
		//	EditorGUILayout.PropertyField( properties.doubleClickSelectsWord );
		//	EditorGUILayout.PropertyField( properties.tripleClickSelectsLine );
		//	EditorGUILayout.LabelField( "Hello" );
		//	EditorGUILayout.LabelField( "Hello" );
		//	EditorGUILayout.LabelField( "Hello" );
		//	EditorGUILayout.LabelField( "Hello" );
		//	EditorGUILayout.LabelField( "Hello" );
		//	EditorGUILayout.LabelField( "Hello" );
		//}

		//EditorGUILayout.EndFadeGroup();
		//EditorGUILayout.EndVertical();
		EditorGUI.BeginChangeCheck();
		selectedStyle = EditorGUILayout.Popup( selectedStyle, styleNameArr );
		if( EditorGUI.EndChangeCheck() ) {
			EditorPrefs.SetInt( "guiskineditor.editingStyle", selectedStyle );
		}

		styleEditors[ selectedStyle ].DoLayoutGUI();

		//DoStateRow( "Hover", target.button.hover );
		//DoStateRow( "Active", target.button.active );
		//DoStateRow( "Focused", target.button.focused );

		//Debug.Log( CompareGUIStyleStates( target.button.normal, target.button.hover ) );

		//base.OnInspectorGUI();

		serializedObject.ApplyModifiedProperties();
	}
}


/*
	GUIStyle properties

	- name

	Text Style:
	- font
	- fontSize
	- fontStyle
	- alignment
	- wordWrap
	- richText
	- clipping
	- imagePosition
	- contentOffset

	Rect Settings:
	- fixedWidth
	- fixedHeight
	- stretchWidth
	- stretchHeight

	- border
	- margin
	- padding
	- overflow

	- Style States:
		normal
		hover
		active
		focused
		onNormal
		onHover
		onActive
		onFocused



	GUIStyleState properties

	- background
	- scaledBackgrounds (but really it's just 1 2x res. "scaledBackground") 
	- textColor
*/

public class GUIStyleEditor {
	const int HOVER = 0x1;
	const int ACTIVE = 0x2;
	const int FOCUSED = 0x4;
	const int ON_NORMAL = 0x8;
	const int ON_HOVER = 0x10;
	const int ON_ACTIVE = 0x20;
	const int ON_FOCUSED = 0x40;
	const int NORMAL = 0x80;

	static GUIContent previewContent;
	static GUIContent alignLeft;
	static GUIContent alignCenter;
	static GUIContent alignRight;
	static GUIContent alignTop;
	static GUIContent alignMiddle;
	static GUIContent alignBottom;

	static GUIContent alignLeftActive;
	static GUIContent alignCenterActive;
	static GUIContent alignRightActive;
	static GUIContent alignTopActive;
	static GUIContent alignMiddleActive;
	static GUIContent alignBottomActive;

	static GUIContent[] offsetRectFieldNames = new GUIContent[] {
		new GUIContent( "L" ),
		new GUIContent( "R" ),
		new GUIContent( "T" ),
		new GUIContent( "B" ),
	};

	int[] layoutFieldValues = new int[ 4 ];

	GUIStyle style;
	GUISkin skin;

	GUIStyleState normalState;

	static readonly Color darkColor = new Color32( 58, 58, 58, 255 );
	static readonly Color lightColor = new Color32( 195, 195, 195, 255 );

	int usingStates;

	public GUIStyleEditor ( GUISkin skin, GUIStyle style ) {
		this.skin = skin;
		this.style = style;
		this.normalState = style.normal;

		UpdateUsingStates();

		if( alignLeft == null ) {
			LoadAlignImages();
		}
	}

	static void LoadAlignImages () {
		var ic = EditorGUIUtility.FindTexture( "console.infoicon.sml" );
		previewContent = new GUIContent( "Preview", ic, "Preview" );
		alignLeft = EditorGUIUtility.IconContent( @"GUISystem/align_horizontally_left", "Left Align" );
		alignCenter = EditorGUIUtility.IconContent( @"GUISystem/align_horizontally_center", "Center Align" );
		alignRight = EditorGUIUtility.IconContent( @"GUISystem/align_horizontally_right", "Right Align" );
		alignLeftActive = EditorGUIUtility.IconContent( @"GUISystem/align_horizontally_left_active", "Left Align" );
		alignCenterActive = EditorGUIUtility.IconContent( @"GUISystem/align_horizontally_center_active", "Center Align" );
		alignRightActive = EditorGUIUtility.IconContent( @"GUISystem/align_horizontally_right_active", "Right Align" );

		alignTop = EditorGUIUtility.IconContent( @"GUISystem/align_vertically_top", "Top Align" );
		alignMiddle = EditorGUIUtility.IconContent( @"GUISystem/align_vertically_center", "Middle Align" );
		alignBottom = EditorGUIUtility.IconContent( @"GUISystem/align_vertically_bottom", "Bottom Align" );
		alignTopActive = EditorGUIUtility.IconContent( @"GUISystem/align_vertically_top_active", "Top Align" );
		alignMiddleActive = EditorGUIUtility.IconContent( @"GUISystem/align_vertically_center_active", "Middle Align" );
		alignBottomActive = EditorGUIUtility.IconContent( @"GUISystem/align_vertically_bottom_active", "Bottom Align" );
	}

	private void UpdateUsingStates () {
		usingStates |= NORMAL;
		UpdateUsingState( style.hover, HOVER );
		UpdateUsingState( style.active, ACTIVE );
		UpdateUsingState( style.focused, FOCUSED );
		UpdateUsingState( style.onNormal, ON_NORMAL );
		UpdateUsingState( style.onHover, ON_HOVER );
		UpdateUsingState( style.onActive, ON_ACTIVE );
		UpdateUsingState( style.onFocused, ON_FOCUSED );
	}

	private void UpdateUsingState ( GUIStyleState styleState, int stateType, bool allowBackgroundSync = false ) {
		if( styleState.background != null || styleState.GetScaledBackground() != null || styleState.textColor != normalState.textColor ) {
			if( allowBackgroundSync && styleState.background == null ) {
				styleState.background = normalState.background;
			}
			usingStates |= stateType;
		} else {
			usingStates &= ~stateType;
		}
	}

	public void DoGUI ( Rect rect ) {
		GUILayout.BeginArea( rect );
		DoLayoutGUI();
		GUILayout.EndArea();
	}

	public void DoLayoutGUI () {

		EditorGUI.BeginChangeCheck();
		Undo.RecordObject( skin, "Edit Style" );

		EditorGUILayout.LabelField( "Character", EditorStyles.boldLabel );

		EditorGUI.indentLevel = 1;

		style.font = (Font)EditorGUILayout.ObjectField( "Font", style.font, typeof( Font ), false );
		style.fontStyle = (FontStyle)EditorGUILayout.EnumPopup( "Font Style", style.fontStyle );
		style.fontSize = EditorGUILayout.IntField( "Font Size", style.fontSize );
		style.richText = EditorGUILayout.Toggle( "Rich Text", style.richText );

		EditorGUI.indentLevel = 0;

		EditorGUILayout.LabelField( "Content", EditorStyles.boldLabel );

		EditorGUI.indentLevel = 1;

		DoTextAlignGUI();
		//style.imagePosition = (ImagePosition)EditorGUILayout.EnumPopup( "Image Position", style.imagePosition );
		style.imagePosition = (ImagePosition)EditorGUILayout.EnumPopup( "Image Position", style.imagePosition );
		style.clipping = (TextClipping)EditorGUILayout.EnumPopup( "Text Clipping", style.clipping );
		style.wordWrap = EditorGUILayout.Toggle( "Word Wrap", style.wordWrap );

		EditorGUI.indentLevel = 0;

		EditorGUILayout.LabelField( "Layout", EditorStyles.boldLabel );

		EditorGUI.indentLevel = 1;

		style.stretchWidth = EditorGUILayout.Toggle( "Stretch Width", style.stretchWidth );
		style.stretchHeight = EditorGUILayout.Toggle( "Stretch Height", style.stretchHeight );
		var fixedSize = new Vector2( style.fixedWidth, style.fixedHeight );
		EditorGUI.BeginChangeCheck();
		fixedSize = EditorGUILayout.Vector2Field( "Fixed Width", fixedSize );
		if( EditorGUI.EndChangeCheck() ) {
			style.fixedWidth = fixedSize.x;
			style.fixedHeight = fixedSize.y;
		}


		var fieldRect = GUILayoutUtility.GetRect( 0, EditorGUIUtility.singleLineHeight * 2 + 4 );
		fieldRect = EditorGUI.PrefixLabel( fieldRect, new GUIContent( "Padding" ) );
		fieldRect.height = EditorGUIUtility.singleLineHeight;

		var boxRect = fieldRect;
		boxRect.width *= 0.5f;
		style.padding = DoRectOffsetField( boxRect, style.padding );

		EditorGUILayout.Space();

		fieldRect = GUILayoutUtility.GetRect( 0, EditorGUIUtility.singleLineHeight * 2 + 4 );
		fieldRect = EditorGUI.PrefixLabel( fieldRect, new GUIContent( "Margin" ) );
		fieldRect.height = EditorGUIUtility.singleLineHeight;

		boxRect = fieldRect;
		boxRect.width *= 0.5f;
		style.margin = DoRectOffsetField( boxRect, style.margin );

		EditorGUILayout.Space();

		fieldRect = GUILayoutUtility.GetRect( 0, EditorGUIUtility.singleLineHeight * 2 + 4 );
		fieldRect = EditorGUI.PrefixLabel( fieldRect, new GUIContent( "Overflow" ) );
		fieldRect.height = EditorGUIUtility.singleLineHeight;

		boxRect = fieldRect;
		boxRect.width *= 0.5f;
		style.overflow = DoRectOffsetField( boxRect, style.overflow );

		EditorGUILayout.Space();

		fieldRect = GUILayoutUtility.GetRect( 0, EditorGUIUtility.singleLineHeight * 2 + 4 );
		fieldRect = EditorGUI.PrefixLabel( fieldRect, new GUIContent( "Border" ) );
		fieldRect.height = EditorGUIUtility.singleLineHeight;

		boxRect = fieldRect;
		boxRect.width *= 0.5f;
		style.border = DoRectOffsetField( boxRect, style.border );

		EditorGUI.indentLevel = 0;

		EditorGUILayout.LabelField( "States", EditorStyles.boldLabel );

		DoStyleRow( "Normal", style.normal, NORMAL );
		DoStyleRow( "Hover", style.hover, HOVER );
		DoStyleRow( "Active", style.active, ACTIVE );
		DoStyleRow( "Focused", style.focused, FOCUSED );

		EditorGUILayout.Space();

		DoStyleRow( "On Normal", style.onNormal, ON_NORMAL );
		DoStyleRow( "On Hover", style.onHover, ON_HOVER );
		DoStyleRow( "On Active", style.onActive, ON_ACTIVE );
		DoStyleRow( "On Focused", style.onFocused, ON_FOCUSED );

		if( EditorGUI.EndChangeCheck() ) {
			EditorUtility.SetDirty( skin );
		}
	}

	private RectOffset DoRectOffsetField ( Rect boxRect, RectOffset offset ) {
		var indent = EditorGUI.indentLevel;
		var width = EditorGUIUtility.labelWidth;
		EditorGUI.indentLevel = 0;
		EditorGUIUtility.labelWidth = 12;
		offset.left = EditorGUI.IntField( boxRect, offsetRectFieldNames[ 0 ], offset.left );
		boxRect.x += boxRect.width;
		offset.right = EditorGUI.IntField( boxRect, offsetRectFieldNames[ 1 ], offset.right );
		boxRect.x -= boxRect.width;
		boxRect.y += boxRect.height + 2;
		offset.top = EditorGUI.IntField( boxRect, offsetRectFieldNames[ 2 ], offset.top );
		boxRect.x += boxRect.width;
		offset.bottom = EditorGUI.IntField( boxRect, offsetRectFieldNames[ 3 ], offset.bottom );
		EditorGUIUtility.labelWidth = width;
		EditorGUI.indentLevel = indent;
		return offset;
	}

	private RectOffset GetFieldValues () {
		return new RectOffset( layoutFieldValues[ 0 ], layoutFieldValues[ 1 ], layoutFieldValues[ 2 ], layoutFieldValues[ 3 ] );
	}

	private void DoTextAlignGUI () {
		var fontRect = GUILayoutUtility.GetRect( 0, EditorGUIUtility.singleLineHeight );

		fontRect = EditorGUI.PrefixLabel( fontRect, new GUIContent( "Text Align" ) );

		var btnRect = fontRect;
		btnRect.width = 20;

		var hSelected = (int)style.alignment % 3;
		var vSelected = (int)style.alignment / 3;
		bool active = hSelected == 0;
		var icon = alignLeft;
		if( active ) {
			icon = alignLeftActive;
		}
		EditorGUI.BeginChangeCheck();
		active = GUI.Toggle( btnRect, active, icon, GUISkinEditor.Styles.alignLeft );
		if( EditorGUI.EndChangeCheck() ) {
			style.alignment = (TextAnchor)( 3 * vSelected + 0 );
		}

		btnRect.x += 20;

		active = hSelected == 1;
		icon = alignCenter;
		if( active ) {
			icon = alignCenterActive;
		}
		EditorGUI.BeginChangeCheck();
		active = GUI.Toggle( btnRect, active, icon, GUISkinEditor.Styles.alignMiddle );
		if( EditorGUI.EndChangeCheck() ) {
			style.alignment = (TextAnchor)( 3 * vSelected + 1 );
		}

		btnRect.x += 20;

		active = hSelected == 2;
		icon = alignRight;
		if( active ) {
			icon = alignRightActive;
		}
		EditorGUI.BeginChangeCheck();
		active = GUI.Toggle( btnRect, active, icon, GUISkinEditor.Styles.alignRight );
		if( EditorGUI.EndChangeCheck() ) {
			style.alignment = (TextAnchor)( 3 * vSelected + 2 );
		}


		btnRect.x += 20 + 10;

		active = vSelected == 0;
		icon = alignTop;
		if( active ) {
			icon = alignTopActive;
		}
		EditorGUI.BeginChangeCheck();
		active = GUI.Toggle( btnRect, active, icon, GUISkinEditor.Styles.alignLeft );
		if( EditorGUI.EndChangeCheck() ) {
			style.alignment = (TextAnchor)( 3 * 0 + hSelected );
		}

		btnRect.x += 20;

		active = vSelected == 1;
		icon = alignMiddle;
		if( active ) {
			icon = alignMiddleActive;
		}
		EditorGUI.BeginChangeCheck();
		active = GUI.Toggle( btnRect, active, icon, GUISkinEditor.Styles.alignMiddle );
		if( EditorGUI.EndChangeCheck() ) {
			style.alignment = (TextAnchor)( 3 * 1 + hSelected );
		}

		btnRect.x += 20;

		active = vSelected == 2;
		icon = alignBottom;
		if( active ) {
			icon = alignBottomActive;
		}
		EditorGUI.BeginChangeCheck();
		active = GUI.Toggle( btnRect, active, icon, GUISkinEditor.Styles.alignRight );
		if( EditorGUI.EndChangeCheck() ) {
			style.alignment = (TextAnchor)( 3 * 2 + hSelected );
		}
	}

	private void DrawStyleWithState ( Rect rect, int stateType ) {
		switch( stateType ) {
			case NORMAL:
				style.Draw( rect, previewContent, false, false, false, false );
				break;
			case HOVER:
				style.Draw( rect, previewContent, true, false, false, false );
				break;
			case ACTIVE:
				style.Draw( rect, previewContent, true, true, false, false );
				break;
			case FOCUSED:
				style.Draw( rect, previewContent, false, false, false, true );
				break;
			case ON_NORMAL:
				style.Draw( rect, previewContent, false, false, true, false );
				break;
			case ON_HOVER:
				style.Draw( rect, previewContent, true, false, true, false );
				break;
			case ON_ACTIVE:
				style.Draw( rect, previewContent, true, true, true, false );
				break;
			case ON_FOCUSED:
				style.Draw( rect, previewContent, false, false, true, true );
				break;
		}
	}

	void DoStyleRow ( string label, GUIStyleState styleState, int stateType ) {
		var layoutRect = GUILayoutUtility.GetRect( 0, 87 + 18 );

		var headerRect = layoutRect;
		headerRect.height = EditorGUIUtility.singleLineHeight;

		GUI.BeginGroup( headerRect, EditorStyles.toolbar );

		var btnWidth = 45;
		var labelRect = new Rect( 0, 0, headerRect.width - btnWidth, headerRect.height );
		GUI.Label( labelRect, label, EditorStyles.label );

		var buttonRect = new Rect( headerRect.width - btnWidth, 0, btnWidth, headerRect.height );
		var enabled = ( usingStates & stateType ) > 0;
		GUI.enabled = enabled;
		if( GUI.Button( buttonRect, "Clear", EditorStyles.toolbarButton ) ) {
			styleState.ClearToNormal( normalState );
			usingStates &= ~stateType;
		}
		GUI.enabled = true;

		GUI.EndGroup();


		var bodyRect = layoutRect;
		bodyRect.y += headerRect.height + 2;
		bodyRect.height -= headerRect.height + 2;

		GUI.BeginGroup( bodyRect );

		var background = styleState.background;
		//var scaledBackground = styleState.GetScaledBackground();
		var textColor = styleState.textColor;

		var backgroundRect = new Rect( 0, 2, 64, 64 );
		EditorGUI.BeginChangeCheck();
		background = (Texture2D)EditorGUI.ObjectField( backgroundRect, background, typeof( Texture2D ), false );
		if( EditorGUI.EndChangeCheck() ) {
			Undo.RecordObject( skin, "Edit GUISkin" );
			styleState.background = background;
			UpdateUsingState( styleState, stateType );
		}

		var colorRect = new Rect( 0, 64 + 4, 64, EditorGUIUtility.singleLineHeight );

		EditorGUI.BeginChangeCheck();
		textColor = EditorGUI.ColorField( colorRect, textColor );
		if( EditorGUI.EndChangeCheck() ) {
			Undo.RecordObject( skin, "Edit GUISkin" );
			styleState.textColor = textColor;
			UpdateUsingState( styleState, stateType, true );
		}

		var previewRect = new Rect( 64 + 2, 0, bodyRect.width - 64 - 2, bodyRect.height );

		var darkPreviewRect = previewRect;
		darkPreviewRect.width = (int)( darkPreviewRect.width * 0.5f );


		var lightPreviewRect = darkPreviewRect;
		lightPreviewRect.x += lightPreviewRect.width;

		GUI.color = darkColor;
		GUI.DrawTexture( darkPreviewRect, EditorGUIUtility.whiteTexture );
		GUI.color = lightColor;
		GUI.DrawTexture( lightPreviewRect, EditorGUIUtility.whiteTexture );
		GUI.color = Color.white;


		float previewWidth = (int)( previewRect.width * 0.4f );
		float previewHeight = EditorGUIUtility.singleLineHeight;

		if( style.fixedWidth != 0 || style.fixedHeight != 0 ) {
			previewWidth = style.fixedWidth;
			previewHeight = style.fixedHeight;
		}

		if( Event.current.type == EventType.Repaint ) {
			GUI.BeginGroup( darkPreviewRect );

			var centeredPreviewRect = new Rect( (int)( darkPreviewRect.width * 0.5f - previewWidth * 0.5f ), (int)( darkPreviewRect.height * 0.5f - previewHeight * 0.5f ), previewWidth, previewHeight );
			DrawStyleWithState( centeredPreviewRect, stateType );

			GUI.EndGroup();

			GUI.BeginGroup( lightPreviewRect );

			centeredPreviewRect = new Rect( (int)( lightPreviewRect.width * 0.5f - previewWidth * 0.5f ), (int)( lightPreviewRect.height * 0.5f - previewHeight * 0.5f ), previewWidth, previewHeight );
			DrawStyleWithState( centeredPreviewRect, stateType );
			GUI.EndGroup();
		}

		GUI.EndGroup();

		//GUILayout.BeginHorizontal();

		//GUILayout.BeginVertical( GUILayout.ExpandWidth( false ) );

		//var background = styleState.background;
		//var scaledBackground = styleState.GetScaledBackground();
		//var textColor = styleState.textColor;

		//EditorGUI.BeginChangeCheck();
		//background = (Texture2D)EditorGUILayout.ObjectField( background, typeof( Texture2D ), false, GUILayout.Width( 46 ), GUILayout.Height( 46 ) );
		//if( EditorGUI.EndChangeCheck() ) {
		//	Undo.RecordObject( skin, "Edit GUISkin" );
		//	styleState.background = background;
		//	UpdateUsingState( styleState, stateType );
		//}

		////EditorGUI.BeginChangeCheck();
		////scaledBackground = (Texture2D)EditorGUILayout.ObjectField( scaledBackground, typeof( Texture2D ), false, GUILayout.Width( 64 ) );
		////if( EditorGUI.EndChangeCheck() ) {
		////	Undo.RecordObject( skin, "Edit GUISkin" );
		////	styleState.SetScaledBackground( scaledBackground );
		////	UpdateUsingState( styleState, stateType, true );
		////}

		//EditorGUI.BeginChangeCheck();
		//textColor = EditorGUILayout.ColorField( textColor, GUILayout.Width( 64 ) );
		//if( EditorGUI.EndChangeCheck() ) {
		//	Undo.RecordObject( skin, "Edit GUISkin" );
		//	styleState.textColor = textColor;
		//	UpdateUsingState( styleState, stateType, true );
		//}

		//GUILayout.EndVertical();

		//GUILayout.BeginHorizontal( GUILayout.ExpandWidth( true ) );
		//GUILayout.EndHorizontal();

		//var previewRect = GUILayoutUtility.GetLastRect();

		//GUI.Box( previewRect, GUIContent.none, EditorStyles.helpBox );

		//GUILayout.EndHorizontal();
		//GUILayout.EndVertical();
	}

}