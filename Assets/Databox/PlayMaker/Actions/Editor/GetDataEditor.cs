﻿#if DATABOX_PLAYMAKER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMakerEditor;
using Databox.PlayMaker;
using Databox;
using System.Linq;

namespace Databox.PlayMaker.Editor
{
	[CustomActionEditor(typeof(GetData))]
	public class GetDataEditor : CustomActionEditor
	{
		
		public override void OnEnable(){}
		
		public override bool OnGUI()
		{
			var action = target as GetData;
	
			EditorGUI.BeginChangeCheck();
			
			using (new GUILayout.VerticalScope("Box"))
			{
				GUILayout.Label("Databox Data:", "boldLabel");
				EditField("databoxObject");
				EditField("usePlayMakerIDFields");
				
				if (!action.databoxObject.IsNone)
				{			
				
					if (!action.usePlayMakerIDFields)
					{
						DrawDataboxSelectionPopup.Draw(action.databoxObject.Value as DataboxObject, DrawDataboxSelectionPopup.DrawType.tableEntryValue, 
							action.data, out action.data);
					}
					else
					{
						
						using (new GUILayout.VerticalScope("Box"))
						{
							EditField("table");
						}
						using (new GUILayout.VerticalScope("Box"))
						{
							EditField("entry");
						}
						using (new GUILayout.VerticalScope("Box"))
						{
							EditField("value");
						}
					}
				}
				
			}
			
			using (new GUILayout.VerticalScope("Box"))
			{
				GUILayout.Label("Store result:", "boldLabel");
				action.dataType = (GetData.DataType)EditorGUILayout.EnumPopup(action.dataType);
				EditField("storeResult" + action.dataType.ToString());
			}
	
			var isDirty = EditorGUI.EndChangeCheck();
			
			return isDirty || GUI.changed;
		}
		
	}
}
#endif