#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.SceneManagement;


namespace EModules {
[InitializeOnLoad]
public static class AutoSaveHandler {

    //* private *//
    static float m_saveInterval;
    static bool m_debug;
    static double? launchPlayTime;
    static bool m_enable = false;
    static int m_filesCount;
    //* private *//
    
    
    //* editorprefs *//
    static float? GET_FLOAT(string key)
    {   if (EditorPrefs.HasKey( "EModules/AutoSave/" + key )) return EditorPrefs.GetFloat( "EModules/AutoSave/" + key );
        return null;
    }
    static void SET_FLOAT(string key, float value)    {   EditorPrefs.SetFloat( "EModules/AutoSave/" + key, value );    }
    static string GET_STRING(string key)
    {   if (EditorPrefs.HasKey( "EModules/AutoSave/" + key )) return EditorPrefs.GetString( "EModules/AutoSave/" + key );
        return null;
    }
    static void SET_STRING(string key, string value)    {   EditorPrefs.SetString( "EModules/AutoSave/" + key, value );    }
    static int? GET_INT(string key)
    {   if (EditorPrefs.HasKey( "EModules/AutoSave/" + key )) return EditorPrefs.GetInt( "EModules/AutoSave/" + key );
        return null;
    }
    static void SET_INT(string key, int value)    {   EditorPrefs.SetInt( "EModules/AutoSave/" + key, value );    }
    static bool? GET_BOOL(string key)
    {   if (EditorPrefs.HasKey( "EModules/AutoSave/" + key )) return EditorPrefs.GetBool( "EModules/AutoSave/" + key );
        return null;
    }
    static void SET_BOOL(string key, bool value)    {   EditorPrefs.SetBool( "EModules/AutoSave/" + key, value );    }
    static bool HAS_KEY(string key)
    {   if (EditorPrefs.HasKey( "EModules/AutoSave/" + key )) return true;
        return false;
    }
    //* editorprefs *//
    
    
    //* props *//
    static double? cacheddbl;
    static double lastSave
    {   get
        {   if (cacheddbl.HasValue)  return cacheddbl.Value;
            var str = GET_STRING( "Nextsave" ) ?? "0";
            try
            {   var prs = double.Parse( str );
                return (cacheddbl = prs).Value;
            }
            catch { return lastSave = 0; }
        }
        set
        {   var res = value < 0 ? 0 : value;
            /*var dif = cacheddbl - res;
            if (dif > -1 && dif < 1) return;*/
            SET_STRING( "Nextsave", (cacheddbl = res).ToString() );
        }
    }
    
    static string AutoSaveFolder
    {   get { return string.IsNullOrEmpty( GET_STRING( "Auto-Save Location" ) ) ? "AutoSave" : GET_STRING( "Auto-Save Location" ); }
        set { SET_STRING( "Auto-Save Location", value ); }
    }
    
    static string autoSaveFileName
    {   get
        {   if (!System.IO.Directory.Exists( Application.dataPath + "/" + AutoSaveFolder ))
            {   System.IO.Directory.CreateDirectory( Application.dataPath + "/" + AutoSaveFolder );
                AssetDatabase.Refresh();
            }
            //if (!AssetDatabase.IsValidFolder("Assets/" + AutoSaveFolder)) AssetDatabase.CreateFolder("Assets", AutoSaveFolder);
            var files = System.IO.Directory.GetFiles(Application.dataPath + "/" + AutoSaveFolder).Select(f => f.Replace('\\', '/')).Where(f =>
                        f.EndsWith(".unity") && f.Substring(f.LastIndexOf('/') + 1).StartsWith("AutoSave")).ToArray();
            if (files.Length == 0) return "AutoSave_00";
            
            var times = files.Select(System.IO.File.GetCreationTime).ToList();
            var max = times.Max();
            var ind = times.IndexOf(max);
            var count = 0;
            files = files.Select( n => n.Remove( n.LastIndexOf( '.' ) ) ).ToArray();
            if (int.TryParse( files[ind].Substring( files[ind].Length - 2 ), out count ))
            {   count = (count + 1) % m_filesCount;
                return "AutoSave_" + count.ToString( "D2" );
            }
            return "AutoSave_00";
        }
    }
    //* props *//
    
    
    
    
    
    //* INITIALIZATION *//
    static AutoSaveHandler()
    {   EditorApplication.update -= UpdateCS;
        EditorApplication.update += UpdateCS;
        ResetTimer();
    }
    
    static void ResetTimer()
    {   m_enable = GET_BOOL( "enablesave" ) ?? true;
        m_filesCount = GET_INT( "auto1" ) ?? 10;
        m_saveInterval = Mathf.Clamp( (GET_INT( "auto2" ) ?? 5), 1, 60) * 60;
        m_debug = GET_BOOL( "auto3" ) ?? false;
    }
    //* INITIALIZATION *//
    
    
    
    
    
    
    //* GUI *//
    [PreferenceItem( "Auto-Save" )]
    public static void OnPreferencesGUI()
    {   EditorGUILayout.LabelField( "Assets/" + AutoSaveFolder + " - Auto-Save Location" );
        var R = EditorGUILayout.GetControlRect(GUILayout.Height(30));
        GUI.Box( R, "" );
        R.x += 7;
        R.y += 7;
        m_enable = EditorGUI.ToggleLeft( R, "Enable", m_enable );
        GUI.enabled = m_enable;
        
        
        
        m_filesCount = Mathf.Clamp( EditorGUILayout.IntField( "Maximum Files Version", m_filesCount ), 1, 99 );
        m_saveInterval = Mathf.Clamp( EditorGUILayout.IntField( "Save Every (Minutes)", (int)(m_saveInterval / 60) ), 1, 60 ) * 60;
        
        var location = EditorGUILayout.TextField("Location", AutoSaveFolder).Replace('\\', '/');
        if (location.IndexOfAny( System.IO.Path.GetInvalidPathChars() ) >= 0) location = AutoSaveFolder;
        
        m_debug = EditorGUILayout.Toggle( "Log", m_debug );
        
        if (GUI.changed)
        {   AutoSaveFolder = location;
            SET_INT( "enablesave", m_enable ? 1 : 0 );
            SET_INT( "auto1", m_filesCount );
            SET_INT( "auto2", (int)(m_saveInterval / 60) );
            SET_BOOL( "auto3", m_debug );
            ResetTimer();
        }
        GUI.enabled = true;
    }
    //* GUI *//
    
    
    
    
    
    //* UPDATER *//
    public static void UpdateCS()
    {   if ( !m_enable ) return;
    
        if (Application.isPlaying)
        {   if (launchPlayTime == null) launchPlayTime = EditorApplication.timeSinceStartup;
            return;
        }
        
        if (launchPlayTime != null)
        {   lastSave += (EditorApplication.timeSinceStartup - launchPlayTime.Value);
            launchPlayTime = null;
        }
        
        if (Mathf.Abs( (float)(lastSave - EditorApplication.timeSinceStartup) ) >= m_saveInterval * 2)
        {   ResetTimer();
        }
        
        if (Mathf.Abs( (float)(lastSave - EditorApplication.timeSinceStartup) ) >= m_saveInterval)
        {   ResetTimer();
            SaveScene();
            EditorApplication.update -= UpdateCS;
            EditorApplication.update += UpdateCS;
        }
    }
    
    
    
    static void SaveScene()
    {   if (!System.IO.Directory.Exists( Application.dataPath + "/" + AutoSaveFolder ))
        {   System.IO.Directory.CreateDirectory( Application.dataPath + "/" + AutoSaveFolder );
            AssetDatabase.Refresh();
        }
        
        var relativeSavePath = "Assets/" + AutoSaveFolder + "/";
        EditorSceneManager.SaveScene( EditorSceneManager.GetActiveScene(), relativeSavePath + autoSaveFileName + ".unity", true );
        lastSave = EditorApplication.timeSinceStartup;
        
        if (m_debug)
            Debug.Log( "Auto-Save Current Scene: " + relativeSavePath + autoSaveFileName + ".unity" );
    }
    //* UPDATER *//
    
    
}
}
#endif