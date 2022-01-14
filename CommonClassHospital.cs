using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    public sealed class CommonClassHospital
    {
        /// <summary>
        /// 记录1101的出参
        /// </summary>
        public static string LS;
        public static string Insuplc_admdvs;
        private readonly static JsonStr json = new JsonStr();
        private readonly static HttpAPI http = new HttpAPI();
        private readonly static WriteJsonLog writeJsonLog = new WriteJsonLog();
        private readonly static NewJsonOutPut jsonOutPut = new NewJsonOutPut();

        private static string jsonStr;
        private static string result;

        //public string insuplc_admdvs;
        //private static readonly string insuplc_admdvs = "320124";

        private string insuplc_admdvs;

        public void Setinsuplc_admdvs(string insuplc_admdvs) {
            this.insuplc_admdvs = insuplc_admdvs;
        }

        public CommonClassHospital()
        {
            //this.insuplc_admdvs = "320124";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="insuplc_admdvs">参保局编码</param>
        public CommonClassHospital(string insuplc_admdvs)
        {
            
        }
    
        private void InsertToChongZheng(string oinfo,string input, string result) {
            if (input == string.Empty || result == string.Empty)
            {
                throw new NullReferenceException();
            }
            jsonOutPut.Register(input);
            string sendmsgid = jsonOutPut["msgid"].ToString();
            jsonOutPut.Register(result);
            string revicemsgid = jsonOutPut["inf_refmsgid"];
            string psn_no = "";
            if (LS != null && LS != string.Empty)
            {
                jsonOutPut.Register(LS);
                psn_no = jsonOutPut.GetOutPut("output", "baseinfo")[0]["psn_no"].ToString();
            }
            NewChongZheng chongZheng = new NewChongZheng();
            chongZheng.send_msgid = sendmsgid;
            chongZheng.recive_msgid = revicemsgid;
            chongZheng.oinfno = oinfo;
            chongZheng.psn_no = psn_no;
            chongZheng.Save();
            jsonOutPut.ClearJsonStr();
        }
        public static void InsertToChongZheng(string input, string result)
        {
            if (input == string.Empty || result == string.Empty)
            {
                throw new NullReferenceException();
            }
            jsonOutPut.Register(input);
            string sendmsgid = jsonOutPut["msgid"].ToString();
            string oinfo = jsonOutPut["infno"].ToString();
            jsonOutPut.Register(result);
            string revicemsgid = jsonOutPut["inf_refmsgid"];
            jsonOutPut.Register(LS);
            string psn_no = jsonOutPut.GetOutPut("output", "baseinfo")[0]["psn_no"].ToString();
            NewChongZheng chongZheng = new NewChongZheng();
            chongZheng.send_msgid = sendmsgid;
            chongZheng.recive_msgid = revicemsgid;
            chongZheng.oinfno = oinfo;
            chongZheng.psn_no = psn_no;
            chongZheng.Save();
        }
        public static void InsertToChongZheng(string a,string b,string c,string d) {
            if (a==string.Empty || b==string.Empty || c==string.Empty || d==string.Empty)
            {
                return;
            }
            NewChongZheng chongZheng = new NewChongZheng();
            chongZheng.send_msgid = a;
            chongZheng.recive_msgid = b;
            chongZheng.oinfno = c;
            chongZheng.psn_no = d;
            chongZheng.Save();
        }
        public int NewHospitalChongZheng(string send_msgid) {
            return NewHospitalChongZheng(send_msgid, "", "", "");
        }
        public int NewHospitalChongZheng(string send_msgid,string recive_msgid) {
            return NewHospitalChongZheng(send_msgid,recive_msgid,"","");
        }    

        public int NewHospitalChongZheng(string send_msgid, string recive_msgid, string psn_no) {
            return NewHospitalChongZheng(send_msgid, recive_msgid, psn_no,"");
        }
       
        public int NewHospitalChongZheng(string send_msgid,string recive_msgid,string psn_no,string oinfo) {
            NewYiBaoChongZheng chongZheng = new NewYiBaoChongZheng();
            StringBuilder sb = new StringBuilder();
            sb.Append("select ISNULL(send_msgid,'')+','+ISNULL(recive_msgid,'')+','+ISNULL(psn_no,'')+','+ISNULL(oinfno,'') from BaseData.dbo.NewChongZheng where 1=1");
            if (send_msgid!=string.Empty)
            {
                sb.Append(" and send_msgid='"+send_msgid+"'");
                chongZheng.omsgid = send_msgid;
            }
            else if (recive_msgid!=string.Empty)
            {
                sb.Append(" and recive_msgid='" + recive_msgid + "'");
                chongZheng.omsgid = recive_msgid;
            }
            else
            {
                throw new Exception("请传入正确参数");
            }
            object val = SmartHIS.Common.ContextHelper.DbConnection.CreateAccessor().Query(sb.ToString());
            string[] arr = val.ToString().Split(',');   
            if (psn_no!=string.Empty)
            {
                chongZheng.psn_no = psn_no;
            }
            else
            {
                chongZheng.psn_no = arr[2];
            }
            if (oinfo!=string.Empty)
            {
                chongZheng.oinfno = oinfo;
            }
            else
            {
                chongZheng.oinfno = arr[3];
            }
            return AccessFunction("2601", chongZheng);
        }

        #region 冲正暂时注释

        //public int UpdateState(string msgid, bool flag = true)
        //{
        //    if (msgid == string.Empty)
        //    {
        //        return -1;
        //    }
        //    object val = SmartHIS.Common.ContextHelper.DbConnection.CreateAccessor().Query("select is_access+','+recive_msgid from BaseData.dbo.NewChongZheng where recive_msgid = '" + msgid + "' or send_msgid='" + msgid + "'");
        //    if (val == null || val is DBNull)
        //    {
        //        return -1;
        //    }
        //    if (flag)
        //    {
        //        string[] arr = val.ToString().Split(',');
        //        if (arr[0].Equals("0"))
        //        {
        //            return 0;
        //        }
        //    }
        //    else
        //    {
        //        string[] arr = val.ToString().Split(',');
        //        string sql = "update BaseData.dbo.NewChongZheng set is_access='1' where recive_msgid='" + msgid + "'";
        //        return SmartHIS.Common.ContextHelper.DbConnection.CreateAccessor().Execute(sql);
        //    }
        //    return -1;
        //}
        //public int NewHospitalChongZhengCommon(NewYiBaoChongZheng chongZheng)
        //{
        //    if (UpdateState(chongZheng.omsgid) > -1) //未冲正
        //    {
        //        if ((AccessFunction<NewYiBaoChongZheng>("2601", insuplc_admdvs, chongZheng) | UpdateState(chongZheng.omsgid, false)) == 1) //医保冲正成功与数据库修改成功的逻辑或的值:1
        //        {
        //            return 0;
        //        }
        //        return -1;
        //    }
        //    return -1;
        //}
        #endregion
        #region 住院业务
        [NewFunctionCode("2601")]
        private int NewHospitalChongZheng(NewYiBaoChongZheng chongZheng)
        {
            return AccessFunction<NewYiBaoChongZheng>("2601", this.insuplc_admdvs, chongZheng);
        }

        [NewFunctionCode("2401")]
        private int InHospital(InHospitalRegist regist)
        {
            return AccessFunction<InHospitalRegist>("2401", this.insuplc_admdvs, regist);
        }
        [NewFunctionCode("2404")]
        private int InHospitalReturn(InHospitalReturn hospitalReturn)
        {
            return AccessFunction<InHospitalReturn>("2404", this.insuplc_admdvs, hospitalReturn);
        }
        [NewFunctionCode("2403")]
        private int ModifyHospitalRegiser(AlterInHospitalInfomation alterInHospital)
        {
            return AccessFunction<AlterInHospitalInfomation>("2403", this.insuplc_admdvs, alterInHospital);
        }
        [NewFunctionCode("2301")]
        private int UploadInfomation(InHospitalChargeUpLoad chargeUpLoad)
        {
            string input = json.ZhuyuanfeiyongmingxishangchuanJson("2301", this.insuplc_admdvs, new List<InHospitalChargeUpLoad>() { chargeUpLoad });
            return AccessFunction("2301", input);
        }
        [NewFunctionCode("2302")]
        private int UploadBack(InHospitalChargeReturn inHospitalChargeReturn)
        {
            return AccessFunction<InHospitalChargeReturn>("2302", this.insuplc_admdvs, inHospitalChargeReturn);
        }
        [NewFunctionCode("2303")]
        private int YuSettleMent(InHospitalYuSettleMent settleMent)
        {
            return AccessFunction<InHospitalYuSettleMent>("2303", this.insuplc_admdvs, settleMent);
        }
        [NewFunctionCode("2402")]
        private int OutHospital(OutHospital outHospital)
        {
            return AccessFunction<OutHospital>("2402", this.insuplc_admdvs, outHospital);
        }
        [NewFunctionCode("2304")]
        private int SettleMent(InHospitalSettleMent settleMent)
        {
            return AccessFunction<InHospitalSettleMent>("2304", this.insuplc_admdvs, settleMent);
        }
        [NewFunctionCode("2305")]
        private int SettleMentBack(InHospitalSettleMentReturn inHospitalSettleMent)
        {
            return AccessFunction<InHospitalSettleMentReturn>("2305", this.insuplc_admdvs, inHospitalSettleMent);
        }
        [NewFunctionCode("2405")]
        private int OutHospitalBack(OutHospitalReturn outHospitalReturn)
        {
            return AccessFunction<OutHospitalReturn>("2405", this.insuplc_admdvs, outHospitalReturn);
        }
        #endregion

        #region 门诊业务
        [NewFunctionCode("2201")]
        private int ClinicRegist(ClinicRegist regist)
        {
            return AccessFunction<ClinicRegist>("2201", this.insuplc_admdvs, regist);
        }
        [NewFunctionCode("2202")]
        private int ClinicRegistBack(ClinicRegistReturn registReturn)
        {
            return AccessFunction<ClinicRegistReturn>("2202", this.insuplc_admdvs, registReturn);
        }

        [NewFunctionCode("2203")]
        private int ClinicInfomationUpload(ClinicInformationUpload upload)
        {
            return AccessFunction<ClinicInformationUpload>("2203", this.insuplc_admdvs, upload); 
        }

        [NewFunctionCode("2203A")]
        private int ClinicInfomationUploadTwo(ClinicInformationUpload upload)
        {
            return AccessFunction<ClinicInformationUpload>("2203A", this.insuplc_admdvs, upload);
        }
        [NewFunctionCode("2204")]
        private int ClinicChargeUpload(ClinicChargeUpload upload)
        {
            string input=json.UploadOutpatientFeeDetailJson("2204", this.insuplc_admdvs, new List<ClinicChargeUpload>() { upload });
            return AccessFunction("2204",input);
        }
        [NewFunctionCode("2205")]
        private int ClinicChargeBack(ClinicChargeReturn clinicCharge)
        {
            return AccessFunction<ClinicChargeReturn>("2205", this.insuplc_admdvs, clinicCharge);
        }
        [NewFunctionCode("2206")]
        private int ClinicYuSettleMent(ClicinYuSettleMent clicinYuSettleMent)
        {
            return AccessFunction<ClicinYuSettleMent>("2206", this.insuplc_admdvs, clicinYuSettleMent);
        }
        [NewFunctionCode("2206A")]
        private int ClinicYuSettleMentTwo(ClicinYuSettleMent clicinYuSettleMent)
        {
            return AccessFunction<ClicinYuSettleMent>("2206A", this.insuplc_admdvs, clicinYuSettleMent);
        }
        [NewFunctionCode("2207")]
        private int ClinicSettleMent(ClinicSettleMent clinicSettle)
        {
            return AccessFunction<ClinicSettleMent>("2207", this.insuplc_admdvs, clinicSettle);
        }
        [NewFunctionCode("2207A")]
        private int ClinicSettleMentTwo(ClinicSettleMent clinicSettle)
        {
            return AccessFunction<ClinicSettleMent>("2207A", this.insuplc_admdvs, clinicSettle);
        }
        [NewFunctionCode("2208")]
        private int ClinicSettleMentBack(ClinicSettleMentReturn clinicSettle)
        {
            return AccessFunction<ClinicSettleMentReturn>("2208", this.insuplc_admdvs, clinicSettle);
        }
        #endregion
        private int AccessFunction<T>(string api, string insuplc_admdvs, T hospital) where T : HospitalBase
        {
            jsonStr = json.GetHospitalJson<T>(api, insuplc_admdvs, hospital);
            result = http.selectAPI(jsonStr);
            InsertToChongZheng(api, jsonStr, result);
            writeJsonLog.writeJson(api, jsonStr, result);
            jsonOutPut.Register(result);
            if (jsonOutPut["infcode"] != "0")
            {
                writeJsonLog.writeErrorJson();
            }
            return Convert.ToInt32(jsonOutPut["infcode"]);
        }
        private int AccessFunction(string api,string input) {
            jsonStr = input;
            result = http.selectAPI(jsonStr);
            InsertToChongZheng(api, jsonStr, result);
            writeJsonLog.writeJson(api,jsonStr,result);
            jsonOutPut.Register(result);
            if (jsonOutPut["infcode"] != "0")
            { 
                writeJsonLog.writeErrorJson();
            }
            return Convert.ToInt32(jsonOutPut["infcode"]);
        }

        public NewJsonOutPut CreateNewJsonOutPut()
        {
            return jsonOutPut;
        }
        public string GetCurrentResult()
        {
            if (result == null)
            {
                return "";
            }
            return result;
        }
        public string GetCurrentInput()
        {
            return jsonStr;
        }
        public int AccessFunction(string api, HospitalBase hospital)
        {
            if (api == null)
            {
                throw new ArgumentNullException();
            }
            if (api == string.Empty)
            {
                throw new ArgumentNullException();
            }
            if (hospital == null)
            {
                throw new ArgumentNullException();
            }
            Type type = this.GetType();
            MethodInfo[] info = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            int status = 1;
            if (info != null && info.Length > 0)
            {
                MethodInfo m = info.Where(x => x.IsDefined(typeof(NewFunctionCodeAttribute), true) && api.Equals(((NewFunctionCodeAttribute)x.GetCustomAttributes(typeof(NewFunctionCodeAttribute), true)[0]).GetCode())).FirstOrDefault();
                if (m != null)
                {
                    object val = m.Invoke(this, new object[] { hospital });
                    status = Convert.ToInt32(val);
                }
            }
            return status;
        }
        public static DataTable ExcelToDataTable(string fileName, string sheetName, bool isFirstRowColumn)
        {
            NPOI.SS.UserModel.IWorkbook workbook = null;
            NPOI.SS.UserModel.ISheet sheet = null;
            FileStream fs = null;
            DataTable data = new DataTable();
            int startRow = 0;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx") > 0)
                    workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0)
                    workbook = new NPOI.HSSF.UserModel.HSSFWorkbook(fs);
                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {

                    NPOI.SS.UserModel.IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; 
                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            NPOI.SS.UserModel.ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        NPOI.SS.UserModel.IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null)
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        data.Rows.Add(dataRow);
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);

                return null;
            }
        }
    }
}
