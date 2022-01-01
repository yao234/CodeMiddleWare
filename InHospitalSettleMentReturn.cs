using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    /// <summary>
    /// 住院结算撤销 2305 data 有输出
    /// </summary>
    public class InHospitalSettleMentReturn : HospitalBase
    {
        [CheckRealElement("data,")]
        /// <summary>
        /// 就诊ID
        /// </summary>
        public string mdtrt_id { get; set; }
        /// <summary>
        /// 结算ID
        /// </summary>
        public string setl_id { get; set; }
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        ///// <summary>
        ///// 字段扩展
        ///// </summary>
        //public string expContent { get; set; }
    }
}
