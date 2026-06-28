using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

#if !REDMATCH
[CustomEditor(typeof(MapConfig))]
public class MapConfigEditor : Editor
{
	MapConfig config => (MapConfig)target;

	public override void OnInspectorGUI()
	{
		GUILayout.Label("Settings", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });

		DrawDefaultInspector();

		if(GUILayout.Button("Generate Thumbnail"))
		{
			if(ImageGenerator.TryGetThumbnail("ThumbnailCamera", 512, 512, out Texture2D tex))
			{
				string thumbnailDirectory = Path.Combine(config.GetMapDirectory(), "Exclude");

				if(!Directory.Exists(thumbnailDirectory))
				{
					Directory.CreateDirectory(thumbnailDirectory);
				}

				string thumbnailPath = Path.Combine(thumbnailDirectory, "thumb.png");

				byte[] textureData = tex.EncodeToPNG();
				File.WriteAllBytes(thumbnailPath, textureData);
				
				AssetDatabase.Refresh();

				Texture2D savedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(thumbnailPath);
				config.thumbnail = savedTexture;
				EditorUtility.SetDirty(config);
			}
		}

		if(GUILayout.Button("Generate Screenshot"))
		{
			if(ImageGenerator.TryGetThumbnail("ScreenshotCamera", 1920, 1080, out Texture2D tex))
			{
				string screenshotDirectory = Path.Combine(config.GetMapDirectory(), "Exclude/Screenshots");

				if(!Directory.Exists(screenshotDirectory))
				{
					Directory.CreateDirectory(screenshotDirectory);
				}

				string screenshotPath = Path.Combine(screenshotDirectory, $"screenshot-{DateTime.Now.ToString("u").Replace(":", "-")}.jpg");

				byte[] textureData = tex.EncodeToJPG(90);
				File.WriteAllBytes(screenshotPath, textureData);

				AssetDatabase.Refresh();

				Texture2D savedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(screenshotPath);
				config.screenshots.Add(savedTexture);
				EditorUtility.SetDirty(config);
			}
		}
	}
}
#endif