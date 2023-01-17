using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;


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
    }

    public class Dependencies
    {
        
    }

    static UpdatePackage()
    {
        Debug.Log("Execute after adding package");
        //read package.json  
        string packagePath = "package.json";
        string packageJson =  File.ReadAllText(packagePath);


        //JsonData dicData =
        //JsonUtility.FromJson<JsonData>(packageJson.Trim());  

      
        JsonData data = JsonConvert.DeserializeObject<JsonData>(packageJson.Trim());

        


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
        string manifestPath = "manifest.json";
        string manifestJson = File.ReadAllText(manifestPath);

        JsonData manifestData = JsonConvert.DeserializeObject<JsonData>(manifestJson.Trim());

        Dictionary<string, string> addDic = new Dictionary<string, string>();
        foreach (var item in data.dependencies.Keys)
        {
           
            if (manifestData.dependencies.ContainsKey(item))
                continue;
            else
                addDic.Add(item, data.dependencies[item]);
        }


        foreach (var item in addDic.Keys)
        {
            // add new git url
            Debug.Log(item + ":" + addDic[item]);  
        }

    }

}
#endif
