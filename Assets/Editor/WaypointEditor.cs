using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaypointSystem))]
public class WaypointEditor : Editor
{
	private SerializedProperty propWaypoints;
	private Vector3 inspectorInputPos = new Vector3();
	private int inspectorInputIndex = 0;

	private void OnEnable()
	{
		SceneView.duringSceneGui += DuringSceneGUI;
		propWaypoints = serializedObject.FindProperty("waypoints");
	}

	private void OnDisable()
	{
		SceneView.duringSceneGui -= DuringSceneGUI;
	}

	public override void OnInspectorGUI()
	{
		using (new GUILayout.VerticalScope(EditorStyles.boldLabel))
		{
			EditorGUILayout.PropertyField(propWaypoints);
			GUILayout.Space(10);

			using (new GUILayout.HorizontalScope(EditorStyles.boldLabel))
			{
				if (GUILayout.Button("Squash all Y to 0", GUILayout.Width(150)))
				{
					for (int i = 0; i < propWaypoints.arraySize; ++i)
					{
						Vector3 vec = propWaypoints.GetArrayElementAtIndex(i).vector3Value;
						vec.y = 0;
						propWaypoints.GetArrayElementAtIndex(i).vector3Value = vec;
					}
					serializedObject.ApplyModifiedProperties();
				}

				if (GUILayout.Button("Add Point", GUILayout.Width(100)))
				{
					propWaypoints.InsertArrayElementAtIndex(inspectorInputIndex);
					propWaypoints.GetArrayElementAtIndex(inspectorInputIndex).vector3Value = inspectorInputPos;
					serializedObject.ApplyModifiedProperties();
				}
			}

			inspectorInputPos = EditorGUILayout.Vector3Field("New pos", inspectorInputPos);
			inspectorInputIndex = EditorGUILayout.IntField("Index", inspectorInputIndex);

			if (inspectorInputIndex >= propWaypoints.arraySize)
			{
				inspectorInputIndex = propWaypoints.arraySize;
			}
			if (inspectorInputIndex < 0)
			{
				inspectorInputIndex = 0;
			}
		}

		serializedObject.Update();
		serializedObject.ApplyModifiedProperties();
	}

	private void DuringSceneGUI(SceneView sceneView)
	{
		bool shift = (Event.current.modifiers & EventModifiers.Shift) != 0;
		bool ctrl = (Event.current.modifiers & EventModifiers.Control) != 0;

		for (int i = 0; i < propWaypoints.arraySize; i++)
		{
			SerializedProperty prop = propWaypoints.GetArrayElementAtIndex(i);

			if (shift) // Add points at mouse
			{
				Vector3 a = propWaypoints.GetArrayElementAtIndex(i).vector3Value;
				Vector3 b = propWaypoints.GetArrayElementAtIndex((i + 1) % propWaypoints.arraySize).vector3Value;

				Vector3 betweenPoints = a + 0.5f * (b - a);
				Handles.color = Color.green;
				if (Handles.Button(betweenPoints, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), 0.25f, 0.25f, Handles.SphereHandleCap))
				{
					propWaypoints.InsertArrayElementAtIndex(i + 1);
					propWaypoints.GetArrayElementAtIndex(i + 1).vector3Value = betweenPoints;
				}

			}
			else if (ctrl) // Delete points at mouse
			{
				Handles.color = Color.red;
				if (Handles.Button(prop.vector3Value, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), 0.25f, 0.25f, Handles.SphereHandleCap))
				{
					propWaypoints.DeleteArrayElementAtIndex(i);
					i--;
					continue;
				}
			}

			else // Move point at mouse
			{
				prop.vector3Value = Handles.PositionHandle(prop.vector3Value, Quaternion.identity);
			}

			Handles.color = Color.white;
			Handles.SphereHandleCap(0, prop.vector3Value, Quaternion.identity, 0.05f, EventType.Repaint);
			Handles.DrawAAPolyLine(propWaypoints.GetArrayElementAtIndex(i).vector3Value, propWaypoints.GetArrayElementAtIndex((i + 1) % propWaypoints.arraySize).vector3Value);
		}
		serializedObject.ApplyModifiedProperties();
		DrawInformationBox();
	}


	void DrawInformationBox()
	{
		Rect size = new Rect(0, 0, 220, 100);
		float sizeButton = 20;
		Handles.BeginGUI();

		GUI.BeginGroup(new Rect(Screen.width - size.width - 10, Screen.height - size.height - 50, size.width, size.height));
		GUI.Box(size, "Waypoint system");
		
		Rect rc = new Rect(5, sizeButton, size.width, sizeButton);
		GUI.Label(rc, "Shift Click to Add");
		rc.y += sizeButton;

		GUI.Label(rc, "Ctrl Click to Delete");
		rc.y += sizeButton;


		GUI.EndGroup();
		Handles.EndGUI();
	}
}
