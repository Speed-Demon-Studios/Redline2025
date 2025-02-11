#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace TinyVerse.Common.Editor
{
	[CanEditMultipleObjects]
	public class TinyVerseEditor : UnityEditor.Editor
	{
		public GUIStyle headerWrapStyle;
		public GUIStyle headerStyle;
		public GUIStyle contentWrapStyle;
		public GUIStyle contentStyle;

		public Color primaryColor = Color.gray;
		public Color borderColor = new Color(1, 1, 1, 0.25f);
		public Color bgColor = new Color(1, 1, 1, 0.1f);

		public void StartSection(string label)
		{
			EditorGUILayout.BeginVertical(headerWrapStyle);
			EditorGUILayout.LabelField(new GUIContent(label), headerStyle);
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(contentWrapStyle);
			// EditorGUILayout.BeginVertical(contentStyle);
		}

		public void EndSection()
		{
			// EditorGUILayout.EndVertical();
			EditorGUILayout.EndVertical();
			DrawSeprator();
		}

		public void StartColumns()
		{
			EditorGUILayout.BeginHorizontal();
		}

		public void EndColumns()
		{
			EditorGUILayout.EndHorizontal();
		}

		public void StartHelp(string label)
		{
			EditorGUILayout.BeginVertical(headerWrapStyle);
			EditorGUILayout.LabelField(new GUIContent(label), headerStyle);
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(contentWrapStyle);
		}

		public void EndHelp()
		{
			EditorGUILayout.EndVertical();
			DrawSeprator();
		}

		public void CustomCurveFiled(SerializedProperty property, Color color, Rect ranges, GUIContent label)
		{
			// int width = (int)EditorGUIUtility.currentViewWidth / 2 - 4;
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField(label);
			EditorGUILayout.CurveField(
				property,
				color,
				ranges,
				GUIContent.none /* , GUILayout.Width(width) */
			);
			EditorGUILayout.EndVertical();
		}

		public SerializedProperty Property(string _name)
		{
			SerializedProperty _property = serializedObject.FindProperty(_name);
			EditorGUILayout.PropertyField(_property);
			return _property;
		}

		public SerializedProperty IndentedList(string _name, GUIContent _label, bool _includeChildren)
		{
			SerializedProperty _property = serializedObject.FindProperty(_name);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_property, _label, _includeChildren);
			EditorGUI.indentLevel--;
			return _property;
		}

		public SerializedProperty IndentedList(string _name, GUIContent _label)
		{
			SerializedProperty _property = serializedObject.FindProperty(_name);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_property, _label);
			EditorGUI.indentLevel--;
			return _property;
		}

		public SerializedProperty IndentedList(string _name)
		{
			SerializedProperty _property = serializedObject.FindProperty(_name);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_property);
			EditorGUI.indentLevel--;
			return _property;
		}

		public void DrawSeprator()
		{
			EditorGUILayout.Space(5);
		}

		public override void OnInspectorGUI()
		{
			int width = (int)EditorGUIUtility.currentViewWidth;
			// int width = (int)EditorGUIUtility.fieldWidth;

			// Global styles
			headerWrapStyle = new GUIStyle(EditorStyles.boldLabel);
			headerWrapStyle.border = new RectOffset(0, 0, 0, 0);
			headerWrapStyle.margin = new RectOffset(0, 0, 0, 0);
			headerWrapStyle.padding = new RectOffset(5, 5, 0, 0);
			headerWrapStyle.overflow = new RectOffset(0, 0, 0, 0);
			headerWrapStyle.normal.textColor = Color.white;
			headerWrapStyle.normal.background = MakeTex(width, 22, primaryColor, new RectOffset(2, 2, 2, 0), borderColor);

			contentWrapStyle = new GUIStyle(headerWrapStyle);
			contentWrapStyle.normal.background = MakeTex(width, 1, bgColor, new RectOffset(2, 2, 0, 0), borderColor);
			// contentWrapStyle.normal.background = CreateColoredTexture(borderColor);
			contentWrapStyle.padding = new RectOffset(5, 5, 5, 5);

			headerStyle = new GUIStyle(EditorStyles.boldLabel);
			headerStyle.border = new RectOffset(0, 0, 0, 0);
			headerStyle.margin = new RectOffset(0, 0, 0, 0);
			headerStyle.padding = new RectOffset(3, 3, 0, 0);
			headerStyle.normal.background = null;
			headerStyle.normal.textColor = Color.white;

			contentStyle = new GUIStyle(EditorStyles.helpBox);
			contentStyle.border = new RectOffset(0, 0, 0, 0);
			contentStyle.margin = new RectOffset(0, 0, 0, 0);
			contentStyle.padding = new RectOffset(5, 5, 5, 5);
			contentStyle.normal.background = null;

			EditorGUILayout.Space(4);
		}

		private static Texture2D CreateColoredTexture(Color color)
		{
			Texture2D texture = new Texture2D(1, 1);
			texture.SetPixel(0, 0, color);
			texture.Apply();
			return texture;
		}

		private Texture2D MakeTex(int width, int height, Color textureColor, RectOffset border, Color bordercolor)
		{
			int widthInner = width;
			width += border.left;
			width += border.right;

			Color[] pix = new Color[width * (height + border.top + border.bottom)];

			for (int i = 0; i < pix.Length; i++)
			{
				if (i < (border.bottom * width))
				{
					// Border bottom
					pix[i] = bordercolor;
				}
				else if (i >= ((border.bottom * width) + (height * width)))
				{
					// Border Top
					pix[i] = bordercolor;
				}
				else
				{
					// Center of Texture
					if ((i % width) < border.left)
					{
						// Border left
						pix[i] = bordercolor;
					}
					else if ((i % width) >= (border.left + widthInner))
					{
						// Border right
						pix[i] = bordercolor;
					}
					else
					{
						// Color texture
						pix[i] = textureColor;
					}
				}
			}

			Texture2D result = new Texture2D(width, height + border.top + border.bottom);
			result.SetPixels(pix);
			result.Apply();

			return result;
		}

		public void OnInspectorGUIEnd()
		{
			serializedObject.ApplyModifiedProperties();
			if (GUI.changed)
			{
				EditorUtility.SetDirty(target);
			}
		}
	}
}
#endif
