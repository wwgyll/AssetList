using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Tawy
{
    internal class SceneAssetList : EditorWindow
    {
        private List<string> objList = new List<string>();
        private List<string> efList = new List<string>();
        private List<string> texList = new List<string>();
        private string[] ttt = new string[] { "模型", "特效", "2D纹理" };
        private int selected;
        private string find = string.Empty;
        private Vector2 vc;


        [MenuItem("Window/大佬虾")]
        static void showWindow()
        {
            var window = GetWindow<SceneAssetList>("资源统计");
            window.maxSize = new Vector2(690, 350);
            window.minSize = new Vector2(690, 350);
        }


        void OnEnable()
        {
            if (File.Exists("Assets/scenlist.asset"))
            {
                var ob = (UnityOBJ)AssetDatabase.LoadAssetAtPath<UnityOBJ>("Assets/scenlist.asset");
                if (ob.efList != null && ob.efList != null && texList != null)
                {
                    objList = ob.objList;
                    efList = ob.efList;
                    texList = ob.texList;
                }
            }
        }




        void OnGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(30));
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Label("模型: " + objList.Count);
            GUILayout.Label("特效: " + efList.Count);
            GUILayout.Label("2D纹理: " + texList.Count);
            GUILayout.Label("资源搜索:", GUILayout.Width(55));
            find = GUILayout.TextField(find, "SearchTextField", GUILayout.Width(260));
            if (GUILayout.Button("", "SearchCancelButton"))
            {
                find = string.Empty;
            }
            if (GUILayout.Button("统计"))
            {
                listRest(objList, efList, texList);
                List<string> str = new List<string>();
                int count = 0;
                getDIr(Application.dataPath, ref str);
                EditorUtility.DisplayProgressBar("资源统计", "统计文件夹:" + count + "/" + str.Count, (float)count / str.Count);
                foreach (var item in str)
                {
                    foreach (var file in Directory.GetFiles(item))
                    {
                        if (file.EndsWith(".fbx"))
                        {
                            objList.Add(file);
                        }
                        if (file.EndsWith(".png") || file.EndsWith(".tga") || file.EndsWith(".jpg"))
                        {
                            texList.Add(file);
                        }
                        if (Path.GetFileNameWithoutExtension(file).StartsWith("fx") && !file.EndsWith(".meta"))
                        {
                            efList.Add(file);
                        }
                    }
                    count++;
                }
                EditorUtility.ClearProgressBar();
            }
            if (GUILayout.Button("保存"))
            {
                UnityOBJ unityOBJ = ScriptableObject.CreateInstance<UnityOBJ>();
                unityOBJ.objList = objList;
                unityOBJ.efList = efList;
                unityOBJ.texList = texList;
                AssetDatabase.CreateAsset(unityOBJ, "Assets/scenlist.asset");
            }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.Space(2);
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            selected = GUILayout.SelectionGrid(selected, ttt, 3);
            GUILayout.EndHorizontal();
            vc = GUILayout.BeginScrollView(vc);
            switch (selected)
            {
                case 0:
                    drawPath(objList);
                    break;
                case 1:
                    drawPath(efList);
                    break;
                case 2:
                    drawPath(texList);
                    break;
                default:
                    drawPath(objList);
                    break;
            }
            GUILayout.EndScrollView();
        }


        void listRest(params List<string>[] list)
        {
            for (var i = 0; i < list.Length; i++)
            {
                list[i].Clear();
            }
        }
        void drawPath(List<string> str)
        {
            if (str.Count > 0)
            {
                foreach (var item in str)
                {

                    if (find != string.Empty)
                    {
                        if (Path.GetFileNameWithoutExtension(item).ToLower().Contains(find.ToLower()))
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.SelectableLabel(item, "box", GUILayout.Width(550));
                            if (GUILayout.Button(Path.GetFileNameWithoutExtension(item), GUILayout.Height(37), GUILayout.Width(115)))
                            {

                                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(getRPath(item));
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.SelectableLabel(item, "box", GUILayout.Width(550));
                        if (GUILayout.Button(Path.GetFileNameWithoutExtension(item), GUILayout.Height(37), GUILayout.Width(115)))
                        {

                            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(getRPath(item));
                        }
                        GUILayout.EndHorizontal();
                    }


                }
            }
        }
        void getDIr(string rootDir, ref List<string> listDir)
        {
            listDir.Add(rootDir);
            var tep = Directory.GetDirectories(rootDir);
            if (tep.Length > 0)
            {
                for (var i = 0; i < tep.Length; i++)
                {
                    getDIr(tep[i], ref listDir);

                }
            }
            else
            {
                return;
            }


        }
        string getRPath(string path)
        {
            return "Assets" + path.Replace(Application.dataPath, "");
        }
    }
    internal class UnityOBJ : ScriptableObject
    {
        [SerializeField] public List<string> objList = new List<string>();
        [SerializeField] public List<string> efList = new List<string>();
        [SerializeField] public List<string> texList = new List<string>();
    }

}