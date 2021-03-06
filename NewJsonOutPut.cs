using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    public class NewJsonOutPut
    {
        private object _JsonStr;
        public string this[string KeyName] {get { return GetOutPut(KeyName); } }
        public string this[int Index] { get { return GetJsonOutPut(Index); } }

        private JavaScriptSerializer js = new JavaScriptSerializer();
        private int RegisterJsonOutPut(string str)
        {
            ClearJsonStr();
            this._JsonStr = this.js.DeserializeObject(str);
            return 1;
        }
        public NewJsonOutPut() { }
        public NewJsonOutPut(string JsonStr) { Register(JsonStr); }
        private string GetJsonOutPut(string keyName)
        {
            object val = this._JsonStr;
            Dictionary<string, object> dic = val as Dictionary<string, object>;
            return dic[keyName].ToString();
        }
        private string GetJsonOutPut(int Index)
        {
            if (Index < 0)
                return "";
            return FindValueUponIndex(Index, this._JsonStr);
        }
        private string FindValueUponIndex(int index, object obj)
        {
            if (obj == null)
                return "";
            if (obj is Dictionary<string, object>)
            {
                Dictionary<string, object> dic = obj as Dictionary<string, object>;
                int i = 0;
                foreach (string item in dic.Keys)
                {
                    if (i == index)
                    {
                        return dic[item].ToString();
                    }
                    ++i;
                }
            }
            return "";
        }
        private List<Dictionary<string, object>> GetJsonOutPut(string LastKeyName, string InLastKeyNameNode)
        {
            object val = this._JsonStr;
            if (IsVailble(InLastKeyNameNode) == 1)
            {
                val = (val as Dictionary<string, object>)[LastKeyName];
                val = (val as Dictionary<string, object>)[InLastKeyNameNode];
            }
            else
                val = (val as Dictionary<string, object>)[LastKeyName];
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            object typeName = val;
            if (!(typeName is Object[]))
                list.Add(val as Dictionary<string, object>);
            else
                list = RepeatAddToList(val as object[]);
            return list;
        }
        private List<Dictionary<string, object>> RepeatAddToList(object[] arr)
        {
            if (arr == null)
            {
                return null;
            }
            if (arr.Length == 0)
            {
                return null;
            }
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            for (int i = 0; i < arr.Length; i++)
            {
                list.Add((arr[i] as Dictionary<string, object>));
            }
            return list;
        }
        int IsVailble(string param)
        {
            if (param == null)
            {
                return 0;
            }
            if (param == string.Empty)
            {
                return 0;
            }
            if (param.Length == 0)
            {
                return 0;
            }
            return 1;
        }
        public void Register(string str)
        {
            if (IsVailble(str) << RegisterJsonOutPut(str) != 2)
            {
                throw new Exception("json格式有误");
            }
        }
        public string GetOutPut(string keyName)
        {
            if (IsVailble(keyName) == 1)
            {
                return GetJsonOutPut(keyName);
            }
            return "";
        }
        public List<Dictionary<string, object>> GetOutPut(string LastKeyName, string InLastKeyNameNode)
        {
            if (IsVailble(LastKeyName) == 1)
                return GetJsonOutPut(LastKeyName, InLastKeyNameNode);
            return null;
        }
        public void ClearJsonStr()
        {
            this._JsonStr = null;
           
        }
        
    }
}
