
using System.Collections;
using UnityEditor;
using UnityEngine;

public class StatusCurveWindow : EditorWindow
{
	private Texture2D tex;
	private Vector2 SP = new Vector2(0, 0);
	private Vector2 SP2 = new Vector2(0, 0);
	
	private int[] lvlPoint = new int[2];
	private int[] valPoint = new int[2];
	private EaseType[] easePoint = new EaseType[2];
	
	private int[] value;
	private int[] oldValue;
	private StatusValue status;
	
	private Color c1 = new Color(1, 0, 0, 1);
	private Color c2 = new Color(0, 1, 0, 1);
	
	public static void Init(string title, ref int[] development, StatusValue sv)
	{
		// Get existing open window or if none, make a new one:
		StatusCurveWindow window = (StatusCurveWindow)EditorWindow.GetWindow(typeof(StatusCurveWindow), true, title);
		window.tex = new Texture2D(1, 1);
		window.value = new int[development.Length];
		System.Array.Copy(development, window.value, development.Length);
		window.oldValue = development;
		window.status = sv;
		
		window.lvlPoint[0] = 1;
		window.valPoint[0] = sv.minValue;
		window.lvlPoint[1] = development.Length;
		window.valPoint[1] = sv.maxValue;
		
		Rect pos = window.position;
		pos.x = 100;
		pos.y = 100;
		pos.width = 300+window.value.Length*2+300;
		pos.height = 330;
		window.position = pos;
		window.Show();
	}
	
	void OnGUI()
	{
		Color tmpColor = GUI.color;
		float o = 250.0f/status.maxValue;
		for(int i=0; i<value.Length; i++)
		{
			if(i % 2 == 0)
			{
				GUI.color = c2;
			}
			else
			{
				GUI.color = c1;
			}
			GUI.DrawTexture(new Rect(300+(i*2), (status.maxValue-value[i])*o, 2, value[i]*o), tex);
		}
		
		GUI.color = tmpColor;
		GUILayout.BeginArea(new Rect(0, 0, 300, 300));
		GUILayout.Label ("Curve Settings", EditorStyles.boldLabel);
		if(GUILayout.Button("Generate Curve"))
		{
			this.GenerateCurve();
		}
		if(lvlPoint.Length < value.Length && 
			GUILayout.Button("Add Point"))
		{
			lvlPoint = ArrayHelper.Add(lvlPoint[lvlPoint.Length-1], lvlPoint);
			lvlPoint[lvlPoint.Length-2] = lvlPoint[lvlPoint.Length-3]+1;
			
			valPoint = ArrayHelper.Add(valPoint[valPoint.Length-1], valPoint);
			valPoint[valPoint.Length-2] = valPoint[valPoint.Length-3]+1;
			
			easePoint = ArrayHelper.Add(easePoint[easePoint.Length-1], easePoint);
			easePoint[easePoint.Length-2] = EaseType.Linear;
		}
		
		lvlPoint[0] = 1;
		valPoint[0] = EditorGUILayout.IntField("Level 1 Value", valPoint[0]);
		if(valPoint[0] < status.minValue)
		{
			valPoint[0] = status.minValue;
		}
		else if(valPoint[0] > status.maxValue)
		{
			valPoint[0] = status.maxValue;
		}
		if(lvlPoint.Length > 2)
		{
			SP2 = EditorGUILayout.BeginScrollView(SP2);
			EditorGUILayout.Separator();
			for(int i=1; i<lvlPoint.Length-1; i++)
			{
				int mx = value.Length-1;
				if(i<lvlPoint.Length-1)
				{
					mx = lvlPoint[i+1];
				}
				if(i == 0)
				{
					lvlPoint[i] = EditorGUILayout.IntField("Level", lvlPoint[i]);
					if(lvlPoint[i] < 2)
					{
						lvlPoint[i] = 2;
					}
				}
				else
				{
					lvlPoint[i] = EditorGUILayout.IntField("Level", lvlPoint[i]);
					if(lvlPoint[i] < lvlPoint[i-1]+1)
					{
						lvlPoint[i] = lvlPoint[i-1]+1;
					}
				}
				if(lvlPoint[i] > mx)
				{
					lvlPoint[i] = mx;
				}
				valPoint[i] = EditorGUILayout.IntField("Value", valPoint[i]);
				if(valPoint[i] < status.minValue)
				{
					valPoint[i] = status.minValue;
				}
				else if(valPoint[i] > status.maxValue)
				{
					valPoint[i] = status.maxValue;
				}
				easePoint[i] = (EaseType)EditorGUILayout.EnumPopup("Interpolation", easePoint[i]);
				
				if(GUILayout.Button("Remove Point"))
				{
					lvlPoint = ArrayHelper.Remove(i, lvlPoint);
					valPoint = ArrayHelper.Remove(i, valPoint);
					easePoint = ArrayHelper.Remove(i, easePoint);
					return;
				}
				EditorGUILayout.Separator();
			}
			EditorGUILayout.EndScrollView();
		}
		lvlPoint[lvlPoint.Length-1] = value.Length;
		valPoint[valPoint.Length-1] = EditorGUILayout.IntField("Max. Level Value", valPoint[valPoint.Length-1]);
		if(valPoint[valPoint.Length-1] < status.minValue)
		{
			valPoint[valPoint.Length-1] = status.minValue;
		}
		else if(valPoint[valPoint.Length-1] > status.maxValue)
		{
			valPoint[valPoint.Length-1] = status.maxValue;
		}
		easePoint[easePoint.Length-1] = (EaseType)EditorGUILayout.EnumPopup("Interpolation", easePoint[easePoint.Length-1]);
		GUILayout.EndArea();
		
		GUILayout.BeginArea(new Rect(300+value.Length*2, 0, 300, 300));
		GUILayout.Label ("Level Values", EditorStyles.boldLabel);
		SP = EditorGUILayout.BeginScrollView(SP);
		for(int i=0; i<value.Length; i++)
		{
			value[i] = EditorGUILayout.IntField("Level "+(i+1).ToString(), value[i]);
			if(value[i] < status.minValue)
			{
				value[i] = status.minValue;
			}
			else if(value[i] > status.maxValue)
			{
				value[i] = status.maxValue;
			}
		}
		EditorGUILayout.EndScrollView();
		GUILayout.EndArea();
		
		GUILayout.BeginArea(new Rect(4, 310, 608, 30));
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Ok"))
		{
			System.Array.Copy(value, oldValue, value.Length);
			this.Close();
		}
		if(GUILayout.Button("Cancel"))
		{
			this.Close();
		}
		EditorGUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void GenerateCurve()
	{
		for(int i=1; i<this.lvlPoint.Length; i++)
		{
			Function interpolate = Interpolate.Ease(this.easePoint[i]);
			int range = this.lvlPoint[i] - this.lvlPoint[i - 1];
			int distance = this.valPoint[i] - this.valPoint[i - 1];
			for(int j=0; j<=range; j++)
			{
				this.value[j + this.lvlPoint[i - 1] - 1] = (int)Interpolate.Ease(interpolate, 
					this.valPoint[i - 1], distance, j, range);
			}
			
			this.value[0] = this.valPoint[0];
			this.value[this.value.Length - 1] = this.valPoint[this.valPoint.Length - 1];
		}
	}
}