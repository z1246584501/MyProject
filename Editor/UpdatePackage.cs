using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;


#if UNITY_EDITOR
[InitializeOnLoad] // Call static class constructor in editor.

public class UpdatePackage
{

    public class JsonData
    {
        //public string name = "";
        //public string license = "";
        //public string[] keywords;

        //public Dependencies dependencies = new Dependencies();
        public Dictionary<string, string> dependencies = new Dictionary<string, string>();
        //public Dictionary<string, string> dic = new Dictionary<string, string>();

        public Dictionary<string, string> dependenciesUrl = new Dictionary<string, string>();
    }

    public class Dependencies
    {

    }

    public static string packageName = "com.lenovo.trx.interactions";


    public static async void OnButtonClick(System.Action<string> callBack)
    {
        Debug.LogError("find start:");


        UnityEditor.PackageManager.Requests.ListRequest listRequest = UnityEditor.PackageManager.Client.List();
        //TimeSpan NextFrame = TimeSpan.FromMilliseconds(16.67f);
        while (listRequest == null)
        {
            await Task.Delay(1000);
        }
        await Task.Delay(3000);
        Debug.LogError("listRequest:" + listRequest);  
        UnityEditor.PackageManager.PackageCollection packageCollection = listRequest.Result;
        Debug.LogError("packageCollection:" + packageCollection);

        while (packageCollection == null)
        {
            await Task.Delay(1000);
            packageCollection = listRequest.Result;
            Debug.LogError(packageCollection);  
        }

        //if (packageCollection == null)
        //{
        //    return;     
        //}

        string path = "";
        foreach (UnityEditor.PackageManager.PackageInfo packageInfo in packageCollection)
        {
            Debug.LogError(packageInfo.displayName);
            if (packageInfo.resolvedPath.Contains(packageName))
            {
                path = packageInfo.resolvedPath;
                Debug.Log(packageInfo.assetPath);  
                break;
            }
            
        }

        callBack?.Invoke(path);
    }


    static UpdatePackage()
    {
       
        Debug.Log("Execute after adding package");
        //read package.json  

        OnButtonClick((pa) =>
        {

            Debug.LogError("path:" + pa);
            string packagePath = pa+"/package.json";



            string packageJson = File.ReadAllText(packagePath);


            //JsonData dicData =
            //JsonUtility.FromJson<JsonData>(packageJson.Trim());  


            JsonData data = JsonConvert.DeserializeObject<JsonData>(packageJson.Trim());


            Debug.LogError(data.dependenciesUrl);

            //Dictionary<string, string> xxxx = new Dictionary<string, string>();
            //xxxx.Add("com.lenovo.trx.core","2.1.5");
            //string jsonstr = JsonConvert.SerializeObject(xxxx); //JsonUtility.ToJson(xxxx);
            //Debug.Log("json:"+jsonstr);

            //if (dicData.ContainsKey("dependencies"))      
            //{
            //    //Debug.Log(dicData["dependencies"]);           
            //}

            //  EditorJsonUtility.

            //read manifest.json
            if (data.dependenciesUrl != null && data.dependenciesUrl.Count > 0)
            {
                Debug.LogError(data.dependenciesUrl.Count);


                string dataPath = Application.dataPath;

                Debug.Log("dataPath:"+dataPath);
               
                string manifestPath = "Packages/manifest.json";

                //Debug.LogError(Path.GetFullPath(manifestPath));
               string manifestAllPath = Path.GetFullPath(manifestPath);


                string manifestJson = File.ReadAllText(manifestAllPath);

                JsonData manifestData = JsonConvert.DeserializeObject<JsonData>(manifestJson.Trim());

                Dictionary<string, string> addDic = new Dictionary<string, string>();
                foreach (var item in data.dependencies.Keys)
                {
                        
                    if (manifestData.dependencies.ContainsKey(item))
                        continue;
                    else
                        //add new git url
                        addDic.Add(item, data.dependenciesUrl[item]);
                }
                Debug.Log("addDic.Count:"+ addDic.Count);

                // File.ReadAllText(manifestAllPath);
                if (addDic.Count > 0)   
                {
                    string[] jsons = manifestJson.Split(new string[] { "\n" }, StringSplitOptions.None);
                    int dependenciesID = -1;
                    for (int i = 0; i < jsons.Length; i++)
                    {
                        Debug.LogError(jsons[i]);

                        if (jsons[i].Contains("dependencies"))
                        {
                            dependenciesID = i;
                            break;
                        }

                    }

                    string[] newJsons = new string[jsons.Length + addDic.Count];
                    for (int i = 0; i <= dependenciesID; i++)
                    {
                        newJsons[i] = jsons[i];
                    }

                    int addIndex = 1;
                    foreach (var item in addDic.Keys)
                    {
                        newJsons[dependenciesID + addIndex] =
                        string.Format("\"{0}\":\"{1}\",", item, addDic[item]);  
                        
                        //"""+ item.ToString() + "":"" + addDic[item].ToString() + "",";
                        addIndex++;

                        Debug.Log(item + ":" + addDic[item]); 
                    }

                    for (int i = dependenciesID+1  ; i < jsons.Length; i++)
                    {
                        newJsons[dependenciesID + addIndex] = jsons[i];
                        addIndex++;
                    }

                      
                    //for (int i = 0; i < newJsons.Length; i++)
                    {
                        File.WriteAllLines(manifestAllPath, newJsons);
                    }



                    foreach (var item in addDic.Keys)
                    {

                        Debug.Log(item + ":" + addDic[item]);
                    }    
                }
            }
        });


    }

}
#endif
