using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    /// <summary>
    /// 门诊预结算 2206
    /// </summary>
    public class ClicinYuSettleMent:HospitalBase
    {
        /// <summary>
        /// 人员编号
        /// </summary>
        [CheckRealElement("data,")]
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
        /// 医疗类别
        /// </summary>
        public string med_type { get; set; }
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
        /// 收费批次号
        /// </summary>
        public string chrg_bchno { get; set; }
        /// <summary>
        /// 个人账户使用标志
        /// </summary>
        public string acct_used_flag { get; set; }
        /// <summary>
        /// 险种类型
        /// </summary>
        public string insutype { get; set; }
        /// <summary>
        /// 字段扩展
        /// </summary>
        public string expContent { get; set; }
    }
}
