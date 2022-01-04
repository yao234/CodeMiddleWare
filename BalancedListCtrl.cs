using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SmartHIS.Common;
using NeuSoftMedicare.Interface;
using NeuSoftMedicare.SQLServer;
using RunShou.MedicareMgr.DAL.Interface;
using RunShou.MedicareMgr.UI;
using SmartHIS.Clinic.DAL.Interface;
using SmartHIS.Integral.Personnel.DAL.Interface;
using RunShou.ECS.OfficeAssistant;
using System.Configuration;
using NeuSoftMedicare.SQLServer.门诊结算出参实体类;
using System.Web.Script.Serialization;
using NeuSoftMedicare.SQLServer.结算出参实体类;

namespace SmartHIS.Clinic.Charge.UI
{
    public partial class BalancedListCtrl : UserControl
    {
        string medicalType = string.Empty;
        string insureCode = string.Empty;
        int ybType = 0;
        SmartSoft.Sessions.ISession Session;
        //序列化
        JavaScriptSerializer jss = new JavaScriptSerializer();


        MZJS_Class mZJS_Class = new MZJS_Class();
        SmartSoft.Report.DAL.Interface.IReportEx Report;
        SmartSoft.Report.Controls.RDLViewDialog PrintForm;
        public BalancedListCtrl(SmartSoft.Sessions.ISession psession, SmartSoft.Report.DAL.Interface.IReportEx pReport, SmartSoft.Report.Controls.RDLViewDialog pPrintForm)
        {
            Session = psession;
            Report = pReport;
            PrintForm = pPrintForm;
            InitializeComponent();
            //加载下拉框的数据
            LoadComboxDatas();
            LoadYbAndEmploeeType();

        }

        public string MedicalType
        {
            set
            {
                this.medicalType = value;
            }
        }
        public string InsureCode
        {
            set
            {
                this.insureCode = value;
            }
        }



        /// <summary>
        /// 修改总额的值
        /// </summary>
        //void UpdateTotalMoney() {
        //    if (TotalDic.Count>0)
        //    {
        //        foreach (DataGridViewRow item in this.dataGridView1.Rows)
        //        {
        //            object val = item.Cells[(int)EnumColumn.ColRegID].Value;
        //            if (val!=null && TotalDic.Keys.Contains(val.ToString()))
        //            {
        //                item.Cells[(int)EnumColumn.ColTotal].Value = TotalDic[val.ToString()];
        //            }
        //        }
        //    }
        //}
        private void updateDateGrid()
        {
            this.dataGridView1.Rows.Clear();

            IMedicareBalanceList balancedList = DALHelper.MedicareMgrDAL.CreateMedicareBalanceList();
            balancedList.Session = SmartHIS.Common.ContextHelper.Session;

            IClinicRegisterList regList = DALHelper.ClinicDAL.CreateClinicRegisterList();
            regList.Session = this.Session;
            regList.GetClinicRegisterList(this.dtpSDate.Value.Date, this.dtpEndDate.Value.Date);

            DataTable dt = CommonClass.GetProcDataTable("[master].[dbo].[ClinicMoneyCount]",this.dtpSDate.Value,this.dtpEndDate.Value,"11");
            int count = dt.Rows.Count;
            foreach (IClinicRegister reg in regList.Rows)
            {
                if ((reg.ChargeType == 1) && (reg.RegID != string.Empty))
                {
                    //医保
                    balancedList.GetMedicareBalanceList(reg.RegID);
                    foreach (IMedicareBalance balance in balancedList.Rows)
                    {
                        #region 通过条件进行相应的筛选
                        //通过姓名进行筛选条件
                        if (this.txtPName.Text!=string.Empty)
                        {
                            if (!balance.MedicareType.Trim().Equals("11"))
                            {   
                                continue;
                            }
                            if (!balance.PName.Trim().Equals(this.txtPName.Text))
                            {
                                continue;
                            }
                        }//如果没有在姓名筛选之前跳出循环就说明已筛选出和输入的姓名匹配的数据
                        #region 下拉框判断
                        //首先判断是否选中的是全部
                        //如果选中的不是全部，则再通过下拉框的隐藏值和相对应的值进行对比 如果不一样 则说明这行的数据不匹配
                        //通过医生ID进行判断
                        if (!this.DoctorCb.SelectedValue.ToString().Equals("-1"))
                        {
                            if (reg.DoctorID != Convert.ToInt32(this.DoctorCb.SelectedValue))
                            {
                                continue;
                            }
                        }
                        //通过科室判断
                        if (!this.DeptCb.SelectedValue.ToString().Equals("-1"))
                        {
                            if (reg.DeptID != Convert.ToInt32(this.DeptCb.SelectedValue))
                            {
                                continue;
                            }
                        }
                        //通过操作员判断
                        if (!this.OperatorCb.SelectedValue.ToString().Equals("-1"))
                        {
                            if (!balance.MedicareType.Trim().Equals("11"))
                            {
                                continue;
                            }
                            if (balance.Operator != Convert.ToInt32(this.OperatorCb.SelectedValue))
                            {
                                continue;
                            }
                        }
                        #endregion
                       #endregion
                        IMedicareReg Reg = DALHelper.MedicareMgrDAL.CreateMedicareReg();
                        Reg.Session = SmartHIS.Common.ContextHelper.Session;
                        Reg.RegID = balance.RegID;
                        Reg.Refresh();
                        #region 医保、人员类别
                        //医保
                        if (!this.IsYiBaoCb.SelectedValue.ToString().Equals("-1"))
                        {
                            if (!NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Reg.LS, 356, 4).Contains(this.IsYiBaoCb.SelectedValue.ToString()))
                            {
                                continue;
                            }
                        }
                        //人员类别
                        if (!this.emploeeCom.SelectedValue.Equals("-1"))
                        {
                            if (!this.emploeeCom.SelectedValue.Equals(Reg.PatientType))
                            {
                                continue;
                            }
                        }
                        #endregion
                        if (Reg.Exists)
                        {
                            this.appendMedicareDatagrid(Reg, balance, reg,dt);
                        }
                    }
                }
                else
                {
                    //非医保
                    //医保
                    if (reg.RegID != string.Empty)
                    {
                        balancedList.GetMedicareBalanceList(string.Empty, reg.RegID);
                        IClinicBaseInfo baseInfo = SmartHIS.Clinic.DAL.Interface.DALHelper.DALManager.CreateClinicBaseInfo();
                        baseInfo.Session = this.Session;
                        baseInfo.RegCode = reg.RegCode;
                        baseInfo.Refresh();
                        if (!baseInfo.Exists) continue;

                        foreach (IMedicareBalance balance in balancedList.Rows)
                        {
                            #region 通过条件进行相应的筛选
                            //通过姓名进行筛选条件
                            if (this.txtPName.Text != string.Empty)
                            {
                                if (!balance.MedicareType.Trim().Equals("11"))
                                {
                                    continue;
                                }
                                if (!balance.PName.Trim().Equals(this.txtPName.Text))
                                {
                                    continue;
                                }
                            }//如果没有在姓名筛选之前跳出循环就说明已筛选出和输入的姓名匹配的数据
                            #region 下拉框判断
                            //首先判断是否选中的是全部
                            //如果选中的不是全部，则再通过下拉框的隐藏值和相对应的值进行对比 如果不一样 则说明这行的数据不匹配
                            //通过医生ID进行判断
                            if (!this.DoctorCb.SelectedValue.ToString().Equals("-1"))
                            {
                                if (reg.DoctorID != Convert.ToInt32(this.DoctorCb.SelectedValue))
                                {
                                    continue;
                                }
                            }
                            //通过科室判断
                            if (!this.DeptCb.SelectedValue.ToString().Equals("-1"))
                            {
                                if (reg.DeptID != Convert.ToInt32(this.DeptCb.SelectedValue))
                                {
                                    continue;
                                }
                            }
                            //通过操作员判断
                            if (!this.OperatorCb.SelectedValue.ToString().Equals("-1"))
                            {
                                if (!balance.MedicareType.Trim().Equals("11"))
                                {
                                    continue;
                                }
                                if (balance.Operator != Convert.ToInt32(this.OperatorCb.SelectedValue))
                                {
                                    continue;
                                }
                            }
                            //医保
                            if (!this.IsYiBaoCb.SelectedValue.ToString().Equals("-1"))
                            {
                                if (!NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(balance.ls, 356, 4).Contains(this.IsYiBaoCb.SelectedValue.ToString()) && !this.IsYiBaoCb.Text.ToString().Equals("自费"))
                                {
                                    continue;
                                }
                            }
                            //人员类别
                            if (!this.emploeeCom.SelectedValue.Equals("-1"))
                            {
                                if (!this.emploeeCom.SelectedValue.Equals(baseInfo.PatientType))
                                {
                                    continue;
                                }
                            }
                            #endregion
                            #endregion
                            this.appendUnMedicareDatagrid(baseInfo, balance, reg, dt);
                        }
                    }
                }
            }
            //UpdateTotalMoney();
        }
        //List<string> RegList = new List<string>();
        //Dictionary<string, decimal> TotalDic = new Dictionary<string, decimal>();
        //readonly Dictionary<string, string> dicYbType = new Dictionary<string, string>() { {"1101","统计在职"},{ "1102",""} };
        private void appendMedicareDatagrid(IMedicareReg RegInfo, IMedicareBalance Balance, IClinicRegister ClincReg, DataTable dt)
        {
            //if (!RegList.Contains(RegInfo.RegID))
            //{
            //    RegList.Add(RegInfo.RegID);
            //    TotalDic.Add(RegInfo.RegID, Balance.BalanceTotal);
            //}
            //else
            //{
            //    TotalDic[RegInfo.RegID] += Balance.BalanceTotal;
            //    return;
            //}
            #region Original
            //string a = CommonClass.GetStringReturnInformation(Balance.ls, 194, 50);

            ////SC02 OR SC13
            //string val2 = CommonClass.GetStringReturnInformation(Balance.ls, 20, 4);
            //if (!val2.Equals("SC13"))
            //{
            //    return;
            //}
            /////第一个数据行
            //int firtstCount = Convert.ToInt32(CommonClass.GetStringReturnInformation(Balance.ls, 240, 8));
            //string itemcode = "";
            //if (firtstCount != 0)
            //{
            //    ///项目代码
            //    itemcode = CommonClass.GetStringReturnInformation(Balance.ls, 248, 6);
            //}
            #endregion

            #region Test
            //if (interface_code.Equals("SC02"))
            //{
            //    string a = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 124, 40);
            //    string b = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 164, 50);
            //    string c = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 214, 16);
            //    string d = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 230, 8);
            //    string e = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 238, 6);
            //    string f = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 244, 10);
            //}
            //else if (interface_code.Equals("SC13"))
            //{
            //    string a = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 124, 50);
            //    string b = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 174, 50);
            //    string c = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 224, 16);
            //    string d = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 240, 8);
            //    string e = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 248, 6);
            //    string f = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 254, 10);
            //}
            //return;
            #endregion
            mZJS_Class = jss.Deserialize<MZJS_Class>(Balance.ls);
            string interface_code = CommonClass.GetStringReturnInformation(Balance.ls, 20, 4);
            string itemcode = "";
            string accountPayTotal = "0.00";
            if (interface_code.Equals("SC02"))
            {
                int firstCount= Convert.ToInt32(CommonClass.GetStringReturnInformation(Balance.ls, 230, 8));
                if (firstCount!=0)
                {
                    ///项目编码
                    itemcode = CommonClass.GetStringReturnInformation(Balance.ls, 238, 6); 
                    //账户支付
                    accountPayTotal =GetSumOfLs(firstCount, CommonClass.GetStringReturnInformation(Balance.ls, 238, 16 * firstCount)).ToString("0.00");
                }
            }
            else if (interface_code.Equals("SC13"))
            {
                int firstCount = Convert.ToInt32(CommonClass.GetStringReturnInformation(Balance.ls, 240, 8));
                if (firstCount != 0)
                {
                    ///项目编码
                    itemcode = CommonClass.GetStringReturnInformation(Balance.ls, 248, 6);
                    //账户支付
                    accountPayTotal = GetSumOfLs(firstCount, CommonClass.GetStringReturnInformation(Balance.ls, 248, 16 * firstCount)).ToString("0.00"); 
                   // CommonClass.GetStringReturnInformation(Balance.ls, 244, 10);
                }
            }
            //总额
            string balanceTotal = mZJS_Class.output.setlinfo.medfee_sumamt.ToString("f2");//NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 124, 10);
            //现金支付
            string cashPayTotal = mZJS_Class.output.setlinfo.psn_cash_pay.ToString("f2");

            ///项目编码
            //string itemcode = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 238, 6);
            //账户支付
            // string accountPayTotal = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 244, 10);
            balanceTotal = Convert.ToDouble(balanceTotal).ToString("0.00");
            cashPayTotal = Convert.ToDouble(cashPayTotal).ToString("0.00");
            //accountPayTotal = Convert.ToDouble(accountPayTotal).ToString("0.00");

            #region 得到医生和科室的相关信息,科室为DepartmentType表
            //11 医生
            IEmployee emp = SmartHIS.Integral.Personnel.DAL.Interface.DALHelper.DALManager.CreateEmployee();
            emp.Session = this.Session;
            emp.EmployeeID = ClincReg.DoctorID;
            emp.Refresh();
            //12 科室
            string sql = "select DepartmentName from his.dbo.DepartmentType where ID = '" + ClincReg.DeptID + "'";
            object deptName = WindowsFormsApp1.DBHelper.ExecuteScalar(sql);
            #endregion

            DataGridViewRow newRow = new DataGridViewRow();
            newRow.CreateCells(this.dataGridView1);
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();

            #region 新增两列
            //医生姓名
            newRow.Cells[(int)EnumColumn.ColDoctorName].Value = emp.Exists ? emp.Name : "";
            //医生科室
            newRow.Cells[(int)EnumColumn.ColDoctorDeptName].Value = deptName is null ? "" : deptName.ToString().Trim();
            #endregion

            #region 新增医保类型和人员类别

            #endregion
            //医保类型
            string val = "";
            if (mZJS_Class.output.setlinfo.insutype == "310")
            {
                val = "职工基本医疗保险";
            }
            if (mZJS_Class.output.setlinfo.insutype == "320")
            {
                val = "公务员医疗补助";
            }
            if (mZJS_Class.output.setlinfo.insutype == "330")
            {
                val = "大额医疗费用补助";
            }
            if (mZJS_Class.output.setlinfo.insutype == "340")
            {
                val = "离休人员医疗保障";
            }
            if (mZJS_Class.output.setlinfo.insutype == "390")
            {
                val = "城乡居民基本医疗保险";
            }
            if (mZJS_Class.output.setlinfo.insutype == "392")
            {
                val = "城乡居民大病医疗保险";
            }
            if (mZJS_Class.output.setlinfo.insutype == "510")
            {
                val = "生育保险";
            }
           
            newRow.Cells[(int)EnumColumn.ColYbType].Value = val;
            foreach (EmploessListView item in ybDicList)
            {
                if (item.id.Contains(val))
                {
                    newRow.Cells[(int)EnumColumn.ColYbType].Value = item.name;
                    break;
                }   
            }
            newRow.Cells[(int)EnumColumn.ColPeopleType].Value = "";
            foreach (EmploessListView item in ybDicList2)
            {
                if (item.id.Equals(RegInfo.PatientType.Trim()))
                {
                    newRow.Cells[(int)EnumColumn.ColPeopleType].Value = item.name;
                    break;
                }
            }
            newRow.Cells[(int)EnumColumn.colBedFee].Value = "0.00";
            newRow.Cells[(int)EnumColumn.colXiYaoFee].Value = "0.00";
            newRow.Cells[(int)EnumColumn.colZcyFee].Value = "0.00";
            newRow.Cells[(int)EnumColumn.colCheckFee].Value = "0.00";
            newRow.Cells[(int)EnumColumn.colCureFee].Value = "0.00";
            newRow.Cells[(int)EnumColumn.colChemistryFee].Value = "0.00";
            newRow.Cells[(int)EnumColumn.colInOxygenFee].Value = "0.00";
            newRow.Cells[(int)EnumColumn.colFsFee].Value = "0.00";
            #region 将收费的明细显示出来
            //由于存储过程只查询了除了挂号费以外的数据，所以我们需要在这里加上判断来判断是否为挂号费
            if (Balance.Register!=1 && Balance.Register!=2)
            {
                foreach (DataRow i in dt.Rows)
                {
                    if (i["BillCode"].ToString().Equals(Balance.BillCode))
                    {
                        newRow.Cells[(int)EnumColumn.colBedFee].Value = Convert.ToDouble(i["床位费"]).ToString("0.00");
                        newRow.Cells[(int)EnumColumn.colXiYaoFee].Value = Convert.ToDouble(i["西药费"]).ToString("0.00");
                        newRow.Cells[(int)EnumColumn.colZcyFee].Value = Convert.ToDouble(i["中成药费"]).ToString("0.00");
                        newRow.Cells[(int)EnumColumn.colCheckFee].Value = Convert.ToDouble(i["检查费"]).ToString("0.00");
                        newRow.Cells[(int)EnumColumn.colCureFee].Value = Convert.ToDouble(i["治疗费"]).ToString("0.00");
                        newRow.Cells[(int)EnumColumn.colChemistryFee].Value = Convert.ToDouble(i["化验费"]).ToString("0.00");
                        newRow.Cells[(int)EnumColumn.colInOxygenFee].Value = Convert.ToDouble(i["输氧费"]).ToString("0.00");
                        newRow.Cells[(int)EnumColumn.colFsFee].Value = Convert.ToDouble(i["放射费"]).ToString("0.00");
                        //newRow.Cells[(int)EnumColumn.colOthersFee].Value = Convert.ToDouble(i["其他费用"]).ToString("0.00");
                        break;
                    }
                }
            }
            #endregion
            

            newRow.Cells[(int)EnumColumn.ColNum].Value = this.dataGridView1.Rows.Count + 1;
            newRow.Cells[(int)EnumColumn.ColDetailed].Value = "详细";
            newRow.Cells[(int)EnumColumn.ColName].Value = RegInfo.Name.Trim();
            newRow.Cells[(int)EnumColumn.ColPNumber].Value = RegInfo.PNumber;
            newRow.Cells[(int)EnumColumn.colInsureCode].Value = RegInfo.InsureCode;

            newRow.Cells[(int)EnumColumn.colInsureCode].Tag = RegInfo;

            newRow.Cells[(int)EnumColumn.ColRegID].Value = RegInfo.RegID;
            newRow.Cells[(int)EnumColumn.ColRegID].Tag = ClincReg;

            newRow.Cells[(int)EnumColumn.ColBillCode].Value = Balance.BillCode;
            newRow.Cells[(int)EnumColumn.ColRegDate].Value = RegInfo.RegTime.ToString("yyyy-MM-dd");
            newRow.Cells[(int)EnumColumn.ColBalanceDate].Value = Balance.BalanceDate.ToString("yyyy-MM-dd HH:mm");

            IGBCode iGBCode = NeuSoftMedicare.Interface.DALHelper.DALManager.CreateIGBCode();
            iGBCode.Session = SmartHIS.Common.ContextHelper.Session;

            iGBCode.GroupCode = "7";
            iGBCode.Code = RegInfo.MedicareType;
            iGBCode.Refresh();
            newRow.Cells[(int)EnumColumn.ColMedicalType].Value = iGBCode.Name;

            newRow.Cells[(int)EnumColumn.ColOperator].Value = SmartHIS.Common.DataConvertHelper.GetEmpName(Balance.Operator);
            newRow.Cells[(int)EnumColumn.ColTotal].Value = balanceTotal;

            newRow.Cells[(int)EnumColumn.ColAccountTotal].Value = "0.00";
            newRow.Cells[(int)EnumColumn.ColSocialFunds].Value = "0.00";
            if (itemcode.Equals("310003") || itemcode.Equals("310002") || itemcode.Equals("380002") || itemcode.Equals("380003"))
            {
                ///账户支付
                newRow.Cells[(int)EnumColumn.ColAccountTotal].Value = accountPayTotal;
            }
            else if (itemcode.Equals("310001") || itemcode.Equals("380001") || itemcode.Equals("3810001") || itemcode.Equals("330001"))
            {
                //统筹支付
                newRow.Cells[(int)EnumColumn.ColSocialFunds].Value = accountPayTotal;
            }
             
               
            
            newRow.Cells[(int)EnumColumn.ColCashTotal].Value = cashPayTotal;
            ///其他支付
            newRow.Cells[(int)EnumColumn.colOthersFee].Value = (Convert.ToDouble(balanceTotal) - Convert.ToDouble(cashPayTotal)-Convert.ToDouble(newRow.Cells[(int)EnumColumn.ColSocialFunds].Value)- Convert.ToDouble(newRow.Cells[(int)EnumColumn.ColAccountTotal].Value)).ToString("0.00");
            //Balance.Status = 5;//测试
            //当状态等于5的时候，既可以冲正
            if (Balance.Status == 5)
            {

                newRow.Cells[(int)EnumColumn.ColOperation].Value = "冲正";
            }
            else
            {
                newRow.Cells[(int)EnumColumn.ColOperation].Value = "撤销";

            }
            if (Balance.QingDanState == 0)
            {
                newRow.Cells[(int)EnumColumn.QingDanState].Value = "未上传";
            }
            else
            {
                newRow.Cells[(int)EnumColumn.QingDanState].Value = "已上传";
            }

            if ((this.dataGridView1.Rows.Count % 2) != 0)
                cellStyle.BackColor = Color.WhiteSmoke;
            else
                cellStyle.BackColor = Color.White;

            if (Balance.Status == (int)RegStatus.Canceled)
            {
                cellStyle.BackColor = Color.LightGray;
                cellStyle.ForeColor = Color.DarkGray;
            }
            
            newRow.Tag = Balance;

            newRow.DefaultCellStyle = cellStyle;

            this.dataGridView1.Rows.Add(newRow);
        }
        private void appendUnMedicareDatagrid(IClinicBaseInfo RegInfo, IMedicareBalance Balance, IClinicRegister ClincReg,DataTable dt)
        {
            //if (!RegList.Contains(RegInfo.RegCode))
            //{
            //    RegList.Add(RegInfo.RegCode);
            //    TotalDic.Add(RegInfo.RegCode, Balance.BalanceTotal);
            //}
            //else
            //{
            //    TotalDic[RegInfo.RegCode] += Balance.BalanceTotal;
            //    return;
            //}

            //string balanceTotal = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 124, 10);
            //string cashPayTotal = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 144, 10);
            //string itemcode = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 238, 6);
            //string accountPayTotal = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(Balance.ls, 244, 10);
            //balanceTotal = Convert.ToDouble(balanceTotal).ToString("0.00");
            //cashPayTotal = Convert.ToDouble(cashPayTotal).ToString("0.00");
            //accountPayTotal = Convert.ToDouble(accountPayTotal).ToString("0.00");


            #region 得到医生和科室的相关信息,科室为DepartmentType表
            //11 医生
            IEmployee emp = SmartHIS.Integral.Personnel.DAL.Interface.DALHelper.DALManager.CreateEmployee();
            emp.Session = this.Session;
            emp.EmployeeID = ClincReg.DoctorID;
            emp.Refresh();
            //12 科室
            string sql = "select DepartmentName from his.dbo.DepartmentType where ID = '" + ClincReg.DeptID + "'";
            object deptName = WindowsFormsApp1.DBHelper.ExecuteScalar(sql);
            #endregion



            DataGridViewRow newRow = new DataGridViewRow();
            newRow.CreateCells(this.dataGridView1);
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();

            #region 新增两列
            //医生姓名
            newRow.Cells[(int)EnumColumn.ColDoctorName].Value = emp.Exists ? emp.Name : "";
            //医生科室
            newRow.Cells[(int)EnumColumn.ColDoctorDeptName].Value = deptName is null ? "" : deptName.ToString().Trim();
            #endregion


            #region 多新增的列
            ////医保类型
            //newRow.Cells[(int)EnumColumn.ColYbType].Value = "";
            //foreach (EmploessListView item in ybDicList)
            //{
            //    string val = NeuSoftMedicare.SQLServer.WanDaYiBao._Instance.FenJie(, 356, 4).Trim();
            //    if (item.id.Contains(val))
            //    {
            //        newRow.Cells[(int)EnumColumn.ColYbType].Value = item.name;
            //        break;
            //    }
            //}
            newRow.Cells[(int)EnumColumn.ColPeopleType].Value = "";
            foreach (EmploessListView item in ybDicList2)
            {
                if (item.id.Equals(RegInfo.PatientType.ToString().Trim()))
                {
                    newRow.Cells[(int)EnumColumn.ColPeopleType].Value = item.name;
                    break;
                }
            }
            newRow.Cells[(int)EnumColumn.colBedFee].Value = "0.00";
            newRow.Cells[(int)EnumColumn.colXiYaoFee].Value = "0.00";
            newRow.Cells[(int)EnumColumn.colZcyFee].Value = "0.00";
            newRow.Cells[(int)EnumColumn.colCheckFee].Value = "0.00";
            newRow.Cells[(int)EnumColumn.colCureFee].Value = "0.00";
            newRow.Cells[(int)EnumColumn.colChemistryFee].Value = "0.00";
            newRow.Cells[(int)EnumColumn.colInOxygenFee].Value = "0.00";
            newRow.Cells[(int)EnumColumn.colFsFee].Value = "0.00";
            newRow.Cells[(int)EnumColumn.colOthersFee].Value = "0.00";
            //由于存储过程只查询了除了挂号费意外的数据，所以我们需要在这里加上判断来判断是否为挂号费
            if (Balance.Register != 1 && Balance.Register != 2)
            {
                foreach (DataRow i in dt.Rows)
                {
                    if (i["BillCode"].ToString().Equals(Balance.BillCode))
                    {
                        newRow.Cells[(int)EnumColumn.colBedFee].Value = Convert.ToDouble(i["床位费"]).ToString("0.00");
                        newRow.Cells[(int)EnumColumn.colXiYaoFee].Value = Convert.ToDouble(i["西药费"]).ToString("0.00");
                        newRow.Cells[(int)EnumColumn.colZcyFee].Value = Convert.ToDouble(i["中成药费"]).ToString("0.00");
                        newRow.Cells[(int)EnumColumn.colCheckFee].Value = Convert.ToDouble(i["检查费"]).ToString("0.00");
                        newRow.Cells[(int)EnumColumn.colCureFee].Value = Convert.ToDouble(i["治疗费"]).ToString("0.00");
                        newRow.Cells[(int)EnumColumn.colChemistryFee].Value = Convert.ToDouble(i["化验费"]).ToString("0.00");
                        newRow.Cells[(int)EnumColumn.colInOxygenFee].Value = Convert.ToDouble(i["输氧费"]).ToString("0.00");
                        newRow.Cells[(int)EnumColumn.colFsFee].Value = Convert.ToDouble(i["放射费"]).ToString("0.00");
                        //newRow.Cells[(int)EnumColumn.colOthersFee].Value = Convert.ToDouble(i["其他费用"]).ToString("0.00");
                        break;
                    }
                }
            }
            #endregion

            newRow.Cells[(int)EnumColumn.ColNum].Value = this.dataGridView1.Rows.Count + 1;
            newRow.Cells[(int)EnumColumn.ColDetailed].Value = "详细";
            newRow.Cells[(int)EnumColumn.ColName].Value = RegInfo.PName.Trim();
            newRow.Cells[(int)EnumColumn.ColPNumber].Value = string.Empty;
            // newRow.Cells[(int)EnumColumn.colInsureCode].Value = "自费";
            newRow.Cells[(int)EnumColumn.ColYbType].Value = "自费";

            newRow.Cells[(int)EnumColumn.colInsureCode].Tag = RegInfo;

            newRow.Cells[(int)EnumColumn.ColRegID].Value = string.Empty;
            newRow.Cells[(int)EnumColumn.ColRegID].Tag = ClincReg;

            newRow.Cells[(int)EnumColumn.ColBillCode].Value = Balance.BillCode;
            newRow.Cells[(int)EnumColumn.ColRegDate].Value = ClincReg.RegTime.ToString("yyyy-MM-dd");
            newRow.Cells[(int)EnumColumn.ColBalanceDate].Value = Balance.BalanceDate.ToString("yyyy-MM-dd HH:mm");

            IGBCode iGBCode = NeuSoftMedicare.Interface.DALHelper.DALManager.CreateIGBCode();
            iGBCode.Session = SmartHIS.Common.ContextHelper.Session;

            iGBCode.GroupCode = "7";
            iGBCode.Code = ClincReg.ClinicType.ToString().Trim();
            iGBCode.Refresh();
            newRow.Cells[(int)EnumColumn.ColMedicalType].Value = iGBCode.Name;
            //newRow.Cells[(int)EnumColumn.ColTotal].Value = balanceTotal;
            //newRow.Cells[(int)EnumColumn.ColAccountTotal].Value = "0.00";
            //newRow.Cells[(int)EnumColumn.ColSocialFunds].Value = "0.00";
            //if (itemcode.Equals("310003") || itemcode.Equals("310002") || itemcode.Equals("380002") || itemcode.Equals("380003"))
            //{
            //    newRow.Cells[(int)EnumColumn.ColAccountTotal].Value = accountPayTotal;
            //}
            //else if (itemcode.Equals("310001") || itemcode.Equals("380001") || itemcode.Equals("3810001") || itemcode.Equals("330001"))
            //{
            //    newRow.Cells[(int)EnumColumn.ColSocialFunds].Value = accountPayTotal;
            //}
            //newRow.Cells[(int)EnumColumn.ColCashTotal].Value = cashPayTotal;
            ///其他支付
            newRow.Cells[(int)EnumColumn.colOthersFee].Value =
                 (Convert.ToDouble(Balance.BalanceTotal) - Convert.ToDouble(Balance.CashPayTotal) - Convert.ToDouble(Balance.SocialFunds) - Convert.ToDouble(Balance.AccountPayTotal)).ToString("0.00");
            newRow.Cells[(int)EnumColumn.ColOperator].Value = SmartHIS.Common.DataConvertHelper.GetEmpName(Balance.Operator);
            newRow.Cells[(int)EnumColumn.ColTotal].Value = Balance.BalanceTotal.ToString("F2");
            newRow.Cells[(int)EnumColumn.ColSocialFunds].Value = Balance.SocialFunds.ToString("F2");
            newRow.Cells[(int)EnumColumn.ColAccountTotal].Value = Balance.AccountPayTotal.ToString("F2");
            newRow.Cells[(int)EnumColumn.ColCashTotal].Value = Balance.CashPayTotal.ToString("F2");

            newRow.Cells[(int)EnumColumn.ColOperation].Value = "撤销";//自费撤销


            if ((this.dataGridView1.Rows.Count % 2) != 0)
                cellStyle.BackColor = Color.WhiteSmoke;
            else
                cellStyle.BackColor = Color.White;

            if (Balance.Status == (int)RegStatus.Canceled)
            {
                cellStyle.BackColor = Color.LightGray;
                cellStyle.ForeColor = Color.DarkGray;
            }
            newRow.Cells[(int)EnumColumn.QingDanState].Value = "自费";
            newRow.Tag = Balance;

            newRow.DefaultCellStyle = cellStyle;

            this.dataGridView1.Rows.Add(newRow);
        }

        /// <summary>
        /// 得到医保ls需要求和的字段
        /// </summary>
        /// <param name="api">接口名</param>
        /// <param name="count">总条数</param>
        /// <param name="ls">需要求和的string</param>
        /// <returns>总和</returns>
        double GetSumOfLs(int count,string ls) {
            if (ls==null)
            {
                return 0;
            }
            if (ls==string.Empty)
            {
                return 0;
            }
            if (ls.Length==0)
            {
                return 0;
            }
            if (count<1)
            {
                return 0;
            }
            string val = ls;
            double sum = 0;
            for (int i = 1; i <= count; i++)
            {
                int begin = (i - 1) * 16;
                ///总条数
                //int index=Convert.ToInt32(CommonClass.GetStringReturnInformation(val, 0, 6));
                ///值
                double value = Convert.ToDouble(CommonClass.GetStringReturnInformation(val, begin+6, 10));
                sum += value;
            }
            return sum;
        }
        private NeuSoftMedicare.SQLServer.Hospital.NewJsonOutPut jsonOutPut = new NeuSoftMedicare.SQLServer.Hospital.NewJsonOutPut();
        /// <summary>
        /// 医保撤销
        /// </summary>
        public void YiBaoCheXiao(DataGridViewCellEventArgs e, string regID)
        {
            int a;
            //医保:
            if (MessageBox.Show("确认要撤销该结算吗", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {

                string dataBuffer = new string('\0', 1024 * 4);
                string ls2207 = "";
                string ls2207Sql = $"select Ls from his.dbo.MedicareBalance where RegID='{regID}'";
                string ls2201 = "";
                string ls2201Sql = $"select LS from his.dbo.MedicareReg where RegID='{regID}'";

                try
                {
                    ls2207 = SmartHIS.Common.ContextHelper.DbConnection.CreateAccessor().Query(ls2207Sql).ToString();

                    ls2201 = SmartHIS.Common.ContextHelper.DbConnection.CreateAccessor().Query(ls2201Sql).ToString(); ;


                    Input2207 input2208 = new Input2207();
                    input2208.api = "2208";
                    input2208.insuplc_admdvs = ls2201.Split(new string[] { "\"", ":" }, StringSplitOptions.None)[87].ToString().Trim();
                    input2208.psn_no = ls2207.Split(new string[] { ":", "," }, StringSplitOptions.None)[37].Replace("\"", "").Trim();
                    input2208.mdtrt_id = ls2207.Split(new string[] { ":", "," }, StringSplitOptions.None)[63].Replace("\"", "").Trim();
                    input2208.setl_id = ls2207.Split(new string[] { ":", "," }, StringSplitOptions.None)[73].Replace("\"", "").Trim();

                    JsonStr jsonStr = new JsonStr();
                    string json = jsonStr.CancelOutPatientBalanceJson(input2208);
                    HttpAPI httpAPI = new HttpAPI();
                    string result = httpAPI.selectAPI(json);
                    int.TryParse(result.Split(new string[] { "\"", ":" }, StringSplitOptions.None)[4], out int ret);
                    //MessageBox.Show(ret.ToString());
                    WriteJsonLog writeLog = new WriteJsonLog();
                    writeLog.writeJson("2208", json, result);

                    
                    if (ret != 0)
                    {
                        writeLog.writeErrorJson();
                        MessageBox.Show("进行结算撤销出错：" + dataBuffer, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                      
                        //throw new System.Exception("进行结算撤销时出错:" + result);       
                    }
                    else
                    {
                        (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Session = SmartHIS.Common.ContextHelper.Session;
                        (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Status = (int)RegStatus.Canceled;
                        (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Operator = SmartHIS.Common.ContextHelper.Employee.EmployeeID;

                        if (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag != null)
                        {
                            (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag as IMedicareReg).Status = (int)RegStatus.Registered;
                            (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag as IMedicareReg).BalanceTime = MedicareHelper.InvalideTime;
                        }

                        if (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag != null)
                        {
                            (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag as IClinicRegister).RegStatus = 2;
                        }

                    (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Save();
                        (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag as IMedicareReg).Save();
                        (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag as IClinicRegister).Save();

                        IDrugAdviceList adviceList = DALHelper.ClinicDAL.CreateDrugAdviceList();
                        adviceList.Session = ContextHelper.Session;
                        if ((this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag as IClinicRegister).DrugAdvice != string.Empty)
                            adviceList.UpdateAdviceStatus((this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag as IClinicRegister).DrugAdvice);
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("已撤销", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                       
                    }
                    //结算撤销冲正
                    DialogResult results = MessageBox.Show("是否进行结算撤销冲正?", "提示", MessageBoxButtons.YesNo);
                    if (results == DialogResult.Yes)
                    {
                        string lsh = (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).SerialNumber;//被冲正交易医院交易流水号
                        JIesuan_Class jIesuan_Class = new JIesuan_Class();
                        jIesuan_Class = jss.Deserialize<JIesuan_Class>((this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).ls);
                        CZ cZ = new CZ();
                        cZ.psn_no = jIesuan_Class.output.setlinfo.psn_no;
                        cZ.oinfno = "2208";
                        //string RegID = (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).RegID;
                        //string sql = "select inmsgid from [HIS].[dbo].MedicareBalance where RegID='" + RegID + "'";
                        //SmartHIS.Common.ContextHelper.DbConnection.CreateAccessor().Query(sql);
                        //cZ.omsgid = SmartHIS.Common.ContextHelper.DbConnection.CreateAccessor().Query(sql).ToString();
                        cZ.omsgid = CommonClass.inmsgid;
                        JsonStr jsonStrs = new JsonStr();
                        string jsons = jsonStrs.ChongzhengJson("2601", "321199", cZ);
                        string result1 = "";
                        HttpAPI httpAPIs = new HttpAPI();
                        result1 = httpAPIs.selectAPI(jsons);
                        WriteJsonLog writeJsonLog = new WriteJsonLog();
                        writeJsonLog.writeJson("2601", jsons, result1);
                        if (result1.Split(new string[] { "\"", ":" }, StringSplitOptions.None)[4] == "0")
                        {
                            MessageBox.Show("结算撤销冲正成功");
                        }
                        else
                        {
                            throw new System.Exception("进行结算撤销冲正时出错:" + result);
                        }
                    }
                }
                catch (System.Exception exc)
                {
                    MessageBox.Show("进行结算撤销出错：" + exc.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                }
                this.updateDateGrid();

            }

        }
        public void YiBaoCheXiaoXML(DataGridViewCellEventArgs e)
        {
            if (MessageBox.Show("确认要撤销该结算吗", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Session = SmartHIS.Common.ContextHelper.Session;
                (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Status = (int)RegStatus.Canceled;
                (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Operator = SmartHIS.Common.ContextHelper.Employee.EmployeeID;

                if (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag != null)
                {
                    (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag as IMedicareReg).Status = (int)RegStatus.Registered;
                    (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag as IMedicareReg).BalanceTime = MedicareHelper.InvalideTime;
                }

                if (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag != null)
                {
                    (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag as IClinicRegister).RegStatus = 2;
                }

                (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Save();
                (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag as IMedicareReg).Save();
                (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag as IClinicRegister).Save();

                IDrugAdviceList adviceList = DALHelper.ClinicDAL.CreateDrugAdviceList();
                adviceList.Session = ContextHelper.Session;
                if ((this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag as IClinicRegister).DrugAdvice != string.Empty)
                    adviceList.UpdateAdviceStatus((this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag as IClinicRegister).DrugAdvice);


                this.Cursor = System.Windows.Forms.Cursors.Default;
                MessageBox.Show("已撤销", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.updateDateGrid();
            }

        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView1.Rows.Count <= 0)
            {
                MessageBox.Show("当前没有可操作的数据");
                return;
            }
            if (e.RowIndex<0)
            {
                return;
            }
            string ybtype = dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColYbType].Value.ToString();
            if (ybtype==string.Empty)
            {
                return;
            }
            if ((e.ColumnIndex == (int)EnumColumn.ColOperation) && (ybtype == "自费"))//自费操作
            {
                if ((this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Status == (int)RegStatus.Canceled)
                {
                    return;
                }
                if (MessageBox.Show("确认要自费撤销吗", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Session = SmartHIS.Common.ContextHelper.Session;
                    (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Status = (int)RegStatus.Canceled;
                    (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Operator = SmartHIS.Common.ContextHelper.Employee.EmployeeID;
                    if (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag != null)
                    {
                        (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag as IClinicRegister).RegStatus = 2;
                    }

                    (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Save();
                    (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag as IClinicRegister).Save();

                    IDrugAdviceList adviceList = DALHelper.ClinicDAL.CreateDrugAdviceList();
                    adviceList.Session = ContextHelper.Session;
                    if ((this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag as IClinicRegister).DrugAdvice != string.Empty)
                    {
                        adviceList.UpdateAdviceStatus((this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag as IClinicRegister).DrugAdvice);
                    }
                    ContextHelper.DbConnection.CreateAccessor().Execute("update his.his.CM_CUREADVICE set JieSuanState=0 where SuoShu='" + (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).BillCode + "'");
                    ContextHelper.DbConnection.CreateAccessor().Execute("update his.his.CM_DRUGADVICE set JieSuanState=0 where SuoShu='" + (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).BillCode + "'");
                    this.updateDateGrid();

                }
            }
            else//医保
             {

                if ((e.ColumnIndex == (int)EnumColumn.ColOperation) &&
                     (dataGridView1.Columns[e.ColumnIndex] is DataGridViewButtonColumn) && (e.RowIndex > -1)
                     && (this.dataGridView1.Rows[e.RowIndex].Tag != null)
                     && (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag != null)
                    && (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag is IMedicareReg))
                {
                    if ((this.dataGridView1.Rows[e.RowIndex].Tag is IMedicareBalance)
                        && ((this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Status == (int)RegStatus.Balanced))
                    {

                        if (ybType == 0)
                        {
                            YiBaoCheXiao(e, (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag as IClinicRegister).RegID);
                        }
                        else if (ybType == 1 || ybType == 2)//2为医保测试
                        {
                            YiBaoCheXiaoXML(e);
                        }
                    }
                    //(this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Status = 5;//测试
                    if ((this.dataGridView1.Rows[e.RowIndex].Tag is IMedicareBalance)
                    && ((this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Status == 5))
                    {
                        //冲正:
                        if (MessageBox.Show("确认要冲正该结算吗", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            int ret;
                            string dataBuffer = new string('\0', 1024 * 4);
                            try
                            {
                                string code = "2410";
                                string lsh = (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).SerialNumber;//被冲正交易医院交易流水号
                                JIesuan_Class jIesuan_Class = new JIesuan_Class();
                                jIesuan_Class = jss.Deserialize<JIesuan_Class>((this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).ls);
                                CZ cZ = new CZ();
                                cZ.psn_no = jIesuan_Class.output.setlinfo.psn_no;
                                cZ.oinfno = "2207";
                                cZ.omsgid = jIesuan_Class.inf_refmsgid;
                                //string RegID = (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).RegID;
                                //string sql = "select inmsgid from [HIS].[dbo].MedicareBalance where RegID='" + RegID + "'";
                                //SmartHIS.Common.ContextHelper.DbConnection.CreateAccessor().Query(sql);
                                //cZ.omsgid = SmartHIS.Common.ContextHelper.DbConnection.CreateAccessor().Query(sql).ToString();
                                JsonStr jsonStr = new JsonStr();
                                string json = jsonStr.ChongzhengJson("2601", "321199", cZ);
                                string result = "";
                                HttpAPI httpAPI = new HttpAPI();
                                result = httpAPI.selectAPI(json);
                                WriteJsonLog writeJsonLog = new WriteJsonLog();
                                writeJsonLog.writeJson("2601", json, result);
                                //省医保撤销
                                //MZJSCX mZJSCX = new MZJSCX();
                                //mZJSCX.mdtrt_id = jIesuan_Class.output.setlinfo.mdtrt_id;
                                //mZJSCX.setl_id = jIesuan_Class.output.setlinfo.setl_id;
                                //mZJSCX.psn_no = jIesuan_Class.output.setlinfo.psn_no;

                                //string json1 = jsonStr.MenzhenjiesuanJson("2208", "321199", mZJSCX);

                                //string result1 = "";
                                //result1 = httpAPI.selectAPI(json1);

                                //writeJsonLog.writeJson("2208", json1, result1);
                                if (result.Split(new string[] { "\"", ":" }, StringSplitOptions.None)[4] == "0")
                                {
                                    ret = 0;
                                }
                                else
                                {
                                    ret = -1;
                                }
                                string s = "";
                                //ret = NueLib.Instance.ChongZheng(code, lsh, ref s);
                                if (ret != 0)
                                {
                                    MessageBox.Show("进行结算冲正出错：" + dataBuffer, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    throw new System.Exception("进行医保预结算时出错:" + dataBuffer);
                                }
                                (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Session = SmartHIS.Common.ContextHelper.Session;
                                (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Status = (int)RegStatus.Canceled;
                                (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Operator = SmartHIS.Common.ContextHelper.Employee.EmployeeID;
                                (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Cancellation = DateTime.Now;
                                if (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag != null)
                                {
                                    (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag as IMedicareReg).Status = (int)RegStatus.Registered;
                                    (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag as IMedicareReg).BalanceTime = MedicareHelper.InvalideTime;
                                }

                                //if (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag != null)
                                //{
                                //    (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag as IClinicRegister).RegStatus = 2;
                                //}

                                //(this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Save();
                                //(this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag as IMedicareReg).Save();
                                //(this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.ColRegID].Tag as IClinicRegister).Save();
                                (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Save();
                                (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag as IMedicareReg).Save();
                                MessageBox.Show("结算冲正成功");
                                this.updateDateGrid();
                            }
                            catch (System.Exception exc)
                            {
                                MessageBox.Show("进行结算冲正出错：" + exc.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Cursor = System.Windows.Forms.Cursors.Default;
                            }
                            //this.Cursor = System.Windows.Forms.Cursors.Default;
                            //MessageBox.Show("已冲正", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //this.updateDateGrid();
                        }
                    }
                }
                else if ((e.ColumnIndex == (int)EnumColumn.ColDetailed) &&
                     (dataGridView1.Columns[e.ColumnIndex] is DataGridViewButtonColumn) && (e.RowIndex > -1)
                     && (this.dataGridView1.Rows[e.RowIndex].Tag != null) && (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag != null))
                {
                    IMedicareBalance balance = (IMedicareBalance)this.dataGridView1.Rows[e.RowIndex].Tag;
                    if (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag != null)
                    {
                        FormClinicChargeRecord record = new FormClinicChargeRecord(balance, this.Session, this.Report, this.PrintForm, (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag as IMedicareReg));
                        record.Show();
                    }
                    if (this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag == null)
                    {
                        FormClinicChargeRecord record = new FormClinicChargeRecord(balance, this.Session, this.Report, this.PrintForm, null);
                        record.Show();
                    }
                }
            }
            if ((e.ColumnIndex == (int)EnumColumn.QingDanState) && (dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.QingDanState].Value.ToString() == "未上传"))
                {
               int state= JieSuanQingDan((this.dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.colInsureCode].Tag as IMedicareReg), (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance));
                if (state == 0)
                {
                    throw new Exception("结算清单上传失败");
                }
                (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).QingDanState = 1; //清单上传0未上传1已上传
                (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).QingDanFanHui = "";//清单上传返回值
                (this.dataGridView1.Rows[e.RowIndex].Tag as IMedicareBalance).Save();
                dataGridView1.Rows[e.RowIndex].Cells[(int)EnumColumn.QingDanState].Value = "已上传";
            }

        }
        /// <summary>
        /// 万达门诊结算清单上传
        /// </summary>
        /// <param name="MedReg"></param>
        /// <param name="balane"></param>
        /// <returns></returns>
        public int JieSuanQingDan(IMedicareReg MedReg, IMedicareBalance balane)
        {
            string id = ContextHelper.DbConnection.CreateAccessor().Query("select id from his.dbo.ClinicRegister where regid='" + MedReg.RegID + "'").ToString();
            IPatientDetail detail = DALHelper.ClinicDAL.CreatePatientDetail();
            detail.Session = this.Session;
            detail.OrgCode = ContextHelper.Employee.OrgCode;
            detail.RegCode = id.Trim();
            detail.Refresh();


            YiBaoChaXun renyuan = WanDaFangFa._Instance.XinXi(MedReg.LS);//返回SA01人员信息
            RCSB16 rc = new RCSB16();
            rc.MZHao = MedReg.SerialNumber.PadRight(20, ' ');
            rc.ShengQing = balane.JiSuanXuHao.PadRight(30, ' ');
            rc.BingAnHao = MedReg.RegID.PadRight(40, ' ');
            rc.ShengBaoShiJian = DateTime.Now.ToString("yyyyMMdd");
            rc.ReYuanBianHao = MedReg.InsureCode.PadRight(30, ' ');
            rc.XingMing = MedReg.Name.ZiJie(50, true);
            rc.HZZJLeiBie = renyuan.LeiXing.ZiJie(3, true);
            rc.HZZJHaoMa = renyuan.HaoMa.ZiJie(50, true);
            rc.ZhiYe = new string(' ', 6);
            rc.SuoShuQu = new string(' ',8);
            rc.XianZhuZhi = new string(' ', 200);//dengdai
            rc.LXRXingMing = new string(' ',50);
            rc.YHZGuanXi = new string(' ', 6);
            rc.LXRDiZhi = new string(' ', 200);
            rc.LXRDianHua = new string(' ', 50);
            rc.XiangZhongLeiXing = new string(' ', 6);
            rc.TSRYLeiXing = new string(' ', 6);
            rc.XSELeiXing = new string(' ', 3);
            rc.XSECSTiZhong =new string(' ',10);
            rc.XSERYTiZhong = new string(' ', 10);
            rc.MTBKeBie = new string(' ', 10);
            rc.MTBShiJian = new string(' ', 8);
            rc.YeWuLiuShuiHao = MedReg.RegID.ZiJie(50,true);
            rc.PiaoJuDaiMa = new string(' ', 50);
            rc.PiaoJuHao = new string(' ', 50);
            rc.JSKSRiQi = balane.BalanceDate.ToString("yyyyMMdd");
            rc.JSJSRiQi = balane.BalanceDate.ToString("yyyyMMdd");
            rc.TBBuMen = "无".ZiJie(100, true);
            rc.TBRen = ContextHelper.Employee.Name.ZiJie(50, true);
            rc.MXTiaoShu = 1.ToString("00000000");


            List<SB16Ji> list = new List<SB16Ji>();
            SB16Ji ji = new SB16Ji();
            ji.JiBingBianMa = detail.ZhenDuan.ZiJie(20, true);
            ji.SSCZMa = new string(' ', 20);
            list.Add(ji);
            rc.SJJi = list;
            string fanhui = WanDaYiBao._Instance.GetSB16(rc);

            if (WanDaFangFa._Instance.FenJie(fanhui, 24, 6) == "000000")
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        private void BalancedListCtrl_Load(object sender, EventArgs e)
        {
            //医保类型初始化
            SmartMIS.Integral.DataCenter.DAL.Interface.IGBCodeList icodelist = SmartMIS.Integral.DataCenter.DAL.Interface.DALHelper.DALManager.CreateGBCodeList();
            //1077是基础编码中医保对接类型的分组编号
            icodelist.GetGBCodeList(1077);
            SmartMIS.Integral.DataCenter.DAL.Interface.IGBCode icode = icodelist.Rows[0] as SmartMIS.Integral.DataCenter.DAL.Interface.IGBCode;
            ybType = icode.SortCode;
            this.updateDateGrid();
            
        }

        //医保
      readonly  List<EmploessListView> ybDicList = new List<EmploessListView>();
        //人员类别
      readonly  List<EmploessListView> ybDicList2 = new List<EmploessListView>();
        /// <summary>
        /// 加载医保和人员类别
        /// </summary>
        void LoadYbAndEmploeeType()
        {
            //1为医保类型，2为人员类别
            //Dictionary<int, List<EmploessListView>> dic = new Dictionary<int, List<EmploessListView>>();
            //List<EmploessListView> ybDicList = new List<EmploessListView>();
            ybDicList.Add(new EmploessListView("-1", "全部"));
            ybDicList.Add(new EmploessListView() { id="99",name="自费" });
            ybDicList.Add(new EmploessListView( "310","统账结合医疗保险"));
            ybDicList.Add(new EmploessListView("380", "离休人员统筹医疗保险"));
           
            this.IsYiBaoCb.DataSource = ybDicList;
            this.IsYiBaoCb.DisplayMember = "name";
            this.IsYiBaoCb.ValueMember = "id";

            ybDicList2.Add(new EmploessListView("-1", "全部"));
            ybDicList2.Add(new EmploessListView("1101","统账在职"));
            ybDicList2.Add(new EmploessListView("1102","统账退休"));
            ybDicList2.Add(new EmploessListView("1201", "公务员在职"));
            ybDicList2.Add(new EmploessListView("1202","公务员退休"));
            ybDicList2.Add(new EmploessListView("2101", "一至八级伤残军人在职"));
            ybDicList2.Add(new EmploessListView("2102", "一至八级伤残军人在职"));
            ybDicList2.Add(new EmploessListView("3101","离休"));
            ybDicList2.Add(new EmploessListView("4101", "在校学生"));
            ybDicList2.Add(new EmploessListView("5101","居民"));
            this.emploeeCom.DataSource = ybDicList2;
            this.emploeeCom.DisplayMember = "name";
            this.emploeeCom.ValueMember = "id";


          


        }
        class EmploessListView
        {
            public string id { get; set; }
            public string name { get; set; }
            public EmploessListView() { 
            
            }

            public EmploessListView(string id, string name) {

                this.id = id;
                this.name = name;
            }

        }
        enum EnumColumn
        {
            ColNum = 0,
            ColDetailed,
            ColName,
            ColPNumber,
            ColPeopleType, //人员类别
            ColYbType, //医保类型
            colInsureCode,
            ColRegID,
            ColBillCode,
            ColRegDate,
            ColBalanceDate,
            ColMedicalType,
            ColDoctorName,
            ColDoctorDeptName,
            ColOperator,
            ColTotal,
            ColSocialFunds,
            ColAccountTotal,
            ColCashTotal,
            colOthersFee,//其他费用
            colBedFee, //床位费
            colXiYaoFee,//西药费
            colZcyFee,//中成药费
            colCheckFee,//检查费
            colCureFee,//治疗费
            colChemistryFee,//化验费
            colInOxygenFee,//输氧费
            colFsFee,//放射费
            ColOperation,
            QingDanState
        }

        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            //this.RegList.Clear();
            //this.TotalDic.Clear();
            this.updateDateGrid();  
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            CenterForm centerForm = new CenterForm(Report, PrintForm);
            centerForm.Show();

            //Clinic_mingxi clinic_Mingxi = new Clinic_mingxi(Report, PrintForm);
            //clinic_Mingxi.Show();
        }
        private void PrintLongAdvice(IMedicareBalancePrintList PrintList)
        {
            if (PrintList.Rows.Count > 0)
            {
                try
                {
                    if (PrintList.Rows.Count > 0)
                    {
                        this.Report.Name = "医保住院发票";
                        this.Report.Refresh();

                        this.PrintForm.Report = this.Report;
                        this.PrintForm.DataObject = PrintList;
                        this.PrintForm.ShowDialog(this.ParentForm);
                    }

                }
                catch (Exception exc)
                {
                    throw exc;
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void 姓名_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        #region 加载下拉框的值
        private void LoadComboxDatas() {

            ///清除所有下拉框的值
            this.DeptCb.Items.Clear();
            this.DoctorCb.Items.Clear();
            this.OperatorCb.Items.Clear();
            ///加载下拉框所需要的值
            //医生
            string DoctorSql = "select EMPID,NAME from BaseData.dbo.EMPLOYEE where TYPE=2";
            DataTable dt1 = WindowsFormsApp1.DBHelper.GetDataTable(DoctorSql);
            //将新的一列添加到最前列
            DataRow insertRow = dt1.NewRow();
            insertRow["EMPID"] = -1;
            insertRow["NAME"] = "全部";
            dt1.Rows.InsertAt(insertRow, 0);

            this.DoctorCb.DataSource = dt1;
            this.DoctorCb.ValueMember = "EMPID";
            this.DoctorCb.DisplayMember = "NAME";
            
            #region 无效
            //IEmployeeList emp = SmartHIS.Integral.Personnel.DAL.Interface.DALHelper.DALManager.CreateEmployeeList();
            //emp.Session = this.Session;
            //emp.GetEmployeeList();
            //foreach (IEmployee item in emp.Rows)
            //{
            //    int typeValue = (int)item.Type;
            //    if (typeValue==2)
            //    {
            //        this.DoctorCb.Items.Add(item.Name);
            //    }
            //    else if (typeValue==4)
            //    {
            //        this.OperatorCb.Items.Add(item.Name);
            //    }
            //}
            #endregion

            //科室
            string sql = "select * from his.dbo.DepartmentType";
            DataTable dt = WindowsFormsApp1.DBHelper.GetDataTable(sql);

            //将新的一列添加到最前列
            DataRow insertRow2 = dt.NewRow();
            insertRow2["ID"] = -1;
            insertRow2["DepartmentName"] = "全部";
            dt.Rows.InsertAt(insertRow2, 0);

            this.DeptCb.DataSource = dt;
            this.DeptCb.DisplayMember = "DepartmentName";
            this.DeptCb.ValueMember = "ID";
            //操作员
            string OperatorSql = "select EMPID,NAME from BaseData.dbo.EMPLOYEE where TYPE = 4";
            DataTable dt2 = WindowsFormsApp1.DBHelper.GetDataTable(OperatorSql);
            DataRow insertRow3 = dt2.NewRow();
            insertRow3["EMPID"] = -1;
            insertRow3["NAME"] = "全部";
            dt2.Rows.InsertAt(insertRow3, 0);

            this.OperatorCb.DataSource = dt2;
            this.OperatorCb.ValueMember = "EMPID";
            this.OperatorCb.DisplayMember = "NAME";

            ///医保下拉框默认选择全部
            this.IsYiBaoCb.SelectedIndex = 0;
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count > 0)
            {
                DataTable dt = CreateDataTable();
                for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                {
                    DataRow row = dt.NewRow();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                       object val=this.dataGridView1.Rows[i].Cells[j].Value;
                        row[dt.Columns[j]] = val is null ? "" : val;
                    }
                    dt.Rows.Add(row);

                }
                FontFamily fontFamily = new FontFamily("Arial");
                Font font = new Font(
                   fontFamily,
                   8,
                   FontStyle.Regular,
                   GraphicsUnit.Point);
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = "所有excel文件|*.xls,*.xlsx";//设置文件类型
                saveFile.Title = "导出数据";//设置标题
                string FileName = "";
                saveFile.AddExtension = true;//是否自动增加所辍名
                saveFile.AutoUpgradeEnabled = true;//是否随系统升级而升级外观
                if (saveFile.ShowDialog() == DialogResult.OK)//如果点的是确定就得到文件路径
                {
                    FileName = saveFile.FileName;//得到文件路径
                }
                string extension = System.IO.Path.GetExtension(FileName);
                if ((extension != ".xlsx") && extension != ".xls")
                {
                    return;
                }
                if (ExcelHelper.ExportExcel(FileName, "入院登记", dt, fontFamily.Name, (short)font.Size) > 0)
                {
                    MessageBox.Show("导出成功！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("暂无数据，请稍后再试!");
            }

        }


        /// <summary>
        /// 导出时需要创建的表与列
        /// </summary>
        /// <returns></returns>
        private  DataTable CreateDataTable() {
            DataTable dt = new DataTable();
            for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
            {
                DataColumn column = new DataColumn(this.dataGridView1.Columns[i].HeaderText);
                dt.Columns.Add(column);
            }
            return dt;
        }
    }
}
