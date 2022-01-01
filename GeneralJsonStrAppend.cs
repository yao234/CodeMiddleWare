using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    public class GeneralJsonStrAppend
    {
        private Dictionary<string, string> saveJsonStr;
        void InitSaveJson()
        {
            if (saveJsonStr == null)
            {
                this.saveJsonStr = new Dictionary<string, string>();
            }
        }
        string CreateDataJson<T>(T[] arr)
        {
            string recodeState = "";
            InitSaveJson();
            Type type = typeof(T);
            PropertyInfo[] propertyInfos = type.GetProperties();
            StringBuilder sbJson = new StringBuilder();
            sbJson.Append("\"input\":{");
            for (int j = 0; j < arr.Length; j++)
            {
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    if (propertyInfos[i].IsDefined(typeof(CheckRealElementAttribute), true))
                    {
                        string relName = ((CheckRealElementAttribute)propertyInfos[i].GetCustomAttributes(typeof(CheckRealElementAttribute), true)[0]).GetRelName();
                        if (relName != null && relName != string.Empty)
                        {
                            if (relName.Contains(","))
                            {
                                string[] rename = relName.Split(',');
                                string first = rename[0];
                                string dataName = rename[1] == string.Empty ? propertyInfos[i].Name : rename[1];
                                if (recodeState != string.Empty && !recodeState.Equals(first))
                                {
                                    if (first == string.Empty && dataName != string.Empty)
                                    {
                                        sbJson.Append($"\"{dataName}\":\"{(propertyInfos[i].GetValue(arr[j], null) == null ? "" : propertyInfos[i].GetValue(arr[j], null))}\",");
                                        continue;
                                    }
                                    sbJson.Remove(sbJson.Length - 1, 1);
                                    sbJson.Append("},");
                                }
                                recodeState = first;
                                sbJson.Append($"\"{first}\":");
                                sbJson.Append("{");
                                sbJson.Append($"\"{dataName}\":\"{(propertyInfos[i].GetValue(arr[j], null) == null ? "" : propertyInfos[i].GetValue(arr[j], null))}\",");
                            }
                            else
                            {
                                sbJson.Append($"\"{relName}\":\"{(propertyInfos[i].GetValue(arr[j], null) == null ? "" : propertyInfos[i].GetValue(arr[j], null))}\",");
                            }

                        }
                        else
                        {
                            if (recodeState != string.Empty)
                            {
                                sbJson.Append($"\"{propertyInfos[i].Name}\":\"{(propertyInfos[i].GetValue(arr[j], null) == null ? "" : propertyInfos[i].GetValue(arr[j], null))}\",");
                            }
                        }
                    }
                    else
                    {
                        sbJson.Append($"\"{propertyInfos[i].Name}\":\"{(propertyInfos[i].GetValue(arr[j], null) == null ? "" : propertyInfos[i].GetValue(arr[j], null))}\",");
                    }
                }
            }
            sbJson.Remove(sbJson.Length - 1, 1);
            sbJson.Append("}}}");
            return sbJson.ToString();
        }
        string CreateDataJson(object[] arr)
        {
            string recodeState = "";
            InitSaveJson();
            Type type = arr.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties();
            StringBuilder sbJson = new StringBuilder();
            sbJson.Append("\"input\":{");
            for (int j = 0; j < arr.Length; j++)
            {
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    if (propertyInfos[i].IsDefined(typeof(CheckRealElementAttribute), true))
                    {
                        string relName = ((CheckRealElementAttribute)propertyInfos[i].GetCustomAttributes(typeof(CheckRealElementAttribute), true)[0]).GetRelName();
                        if (relName != null && relName != string.Empty)
                        {
                            if (relName.Contains(","))
                            {
                                string[] rename = relName.Split(',');
                                string first = rename[0];
                                string dataName = rename[1] == string.Empty ? propertyInfos[i].Name : rename[1];
                                if (recodeState != string.Empty && !recodeState.Equals(first))
                                {
                                    if (first == string.Empty && dataName != string.Empty)
                                    {
                                        sbJson.Append($"\"{dataName}\":\"{(propertyInfos[i].GetValue(arr[j], null) == null ? "" : propertyInfos[i].GetValue(arr[j], null))}\",");
                                        continue;
                                    }
                                    sbJson.Remove(sbJson.Length - 1, 1);
                                    sbJson.Append("},");
                                }
                                recodeState = first;
                                sbJson.Append($"\"{first}\":");
                                sbJson.Append("{");
                                sbJson.Append($"\"{dataName}\":\"{(propertyInfos[i].GetValue(arr[j], null) == null ? "" : propertyInfos[i].GetValue(arr[j], null))}\",");
                            }
                            else
                            {
                                sbJson.Append($"\"{relName}\":\"{(propertyInfos[i].GetValue(arr[j], null) == null ? "" : propertyInfos[i].GetValue(arr[j], null))}\",");
                            }

                        }
                        else
                        {
                            if (recodeState != string.Empty)
                            {
                                sbJson.Append($"\"{propertyInfos[i].Name}\":\"{(propertyInfos[i].GetValue(arr[j], null) == null ? "" : propertyInfos[i].GetValue(arr[j], null))}\",");
                            }
                        }
                    }
                    else
                    {
                        sbJson.Append($"\"{propertyInfos[i].Name}\":\"{(propertyInfos[i].GetValue(arr[j], null) == null ? "" : propertyInfos[i].GetValue(arr[j], null))}\",");
                    }
                }
            }
            sbJson.Remove(sbJson.Length - 1, 1);
            sbJson.Append("}}}");
            return sbJson.ToString();
        }
        private bool FindIDInJson(string api)
        {
            if (api == null)
            {
                return false;
            }
            if (api == string.Empty)
            {
                return false;
            }
            InitSaveJson();
            if (this.saveJsonStr.ContainsKey(api))
            {
                return true;
            }
            return false;
        }
        private string CreateHeadJson(string api, params string[] parmArray)
        {
            StringBuilder sbJson = new StringBuilder();
            string val = api == string.Empty ? parmArray[0] : api;
            sbJson.Append("{\"infno\": \"" + val + "\",");
            sbJson.Append("\"msgid\": \"" + parmArray[1] + "\",");
            sbJson.Append("\"insuplc_admdvs\": \"" + "" + parmArray[3] + "" + "\",");
            sbJson.Append("\"mdtrtarea_admvs\": \"" + parmArray[2] + "\",");
            sbJson.Append("\"recer_sys_code\": \"" + parmArray[4] + "\",");
            sbJson.Append("\"cainfo\": \"" + parmArray[7] + "\",\"dev_no\": \"" + parmArray[5] + "\",\"dev_safe_info\": \"" + parmArray[6] + "\",");
            sbJson.Append("\"signtype\": \"" + parmArray[8] + "\",\"infver\": \"" + parmArray[9] + "\",");
            sbJson.Append("\"opter_type\": \"" + parmArray[10] + "\",\"opter\": \"" + parmArray[11] + "\",\"opter_name\": \"" + parmArray[12] + "\",");
            sbJson.Append("\"inf_time\": \"" + parmArray[13] + "\",");
            sbJson.Append("\"fixmedins_code\": \"" + parmArray[14] + "\",");
            sbJson.Append("\"fixmedins_name\": \"" + parmArray[15] + "\",");
            sbJson.Append("\"sign_no\": \"" + parmArray[16] + "\",");
            sbJson.Append("\"fixmedins_soft_fcty\":\"" + parmArray[17] + "\",");
            return sbJson.ToString();
        }
        private string GetJson<T>(string api, T[] arr, params string[] parmArray)
        {
            StringBuilder sbJson = new StringBuilder();
            sbJson.Append(CreateHeadJson(api, parmArray));
            sbJson.Append(CreateDataJson<T>(arr));
            return sbJson.ToString();
        }
        private string GetJson(string api, object[] arr, params string[] parmArray)
        {
            StringBuilder sbJson = new StringBuilder();
            sbJson.Append(CreateHeadJson(api, parmArray));
            sbJson.Append(CreateDataJson(arr));
            return sbJson.ToString();
        }
        public static string GetCreateJson<T>(string api, T[] arr, params string[] parmArray)
        {
            if (api == null)
            {
                return "";
            }
            if (api == string.Empty)
            {
                return "";
            }
            if (arr == null)
            {
                return "";
            }
            if (arr.Length == 0)
            {
                return "";
            }
            return new GeneralJsonStrAppend().GetJson<T>(api, arr, parmArray);
        }
        public static string GetCreateJson(string api, object[] arr, params string[] parmArray)
        {
            if (api == null)
            {
                return "";
            }
            if (arr == null)
            {
                return "";
            }
            if (api == string.Empty)
            {
                return "";
            }
            if (arr.Length == 0)
            {
                return "";
            }
            return new GeneralJsonStrAppend().GetJson(api, arr, parmArray);

        }

    }
}
