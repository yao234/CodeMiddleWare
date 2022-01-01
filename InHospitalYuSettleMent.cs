using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    /// <summary>
    /// 住院预结算 2303 data 有输出
    /// </summary>
    public class InHospitalYuSettleMent : HospitalBase
    {
        [CheckRealElement("data,")]
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 就诊凭证类型
        /// </summary>
        public string mdtrt_cert_type { get; set; }
        /// <summary>
        /// 就诊凭证编号
        /// </summary>
        public string mdtrt_cert_no { get; set; }
        /// <summary>
        /// 医疗费总额
        /// </summary>
        public string medfee_sumamt { get; set; }
        /// <summary>
        /// 个人结算方式
        /// </summary>
        public string psn_setlway { get; set; }
        /// <summary>
        /// 就诊ID
        /// </summary>
        public string mdtrt_id { get; set; }
        /// <summary>
        /// 个人账户使用标志
        /// </summary>
        public string acct_used_flag { get; set; }
        /// <summary>
        /// 险种类型
        /// </summary>
        public string insutype { get; set; }
        /// <summary>
        /// 发票号
        /// </summary>
        public string invono { get; set; }
        /// <summary>
        /// 中途结算标志
        /// </summary>
        public string mid_setl_flag { get; set; }
        /// <summary>
        /// 全自费金额
        /// </summary>
        public string fulamt_ownpay_amt { get; set; }
        /// <summary>
        /// 超限价金额
        /// </summary>
        public string overlmt_selfpay { get; set; }
        /// <summary>
        /// 先行自付金额
        /// </summary>
        public string preselfpay_amt { get; set; }
        /// <summary>
        /// 符合政策范围金额
        /// </summary>
        public string inscp_scp_amt { get; set; }
        /// <summary>
        /// 出院时间
        /// </summary>
        public string dscgTime { get; set; }
        ///// <summary>
        ///// 字段扩展
        ///// </summary>
        //public string expContent { get; set; }
    }
}
