using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    /// <summary>
    /// 门诊计算撤销 2208
    /// </summary>
    public class ClinicSettleMentReturn:HospitalBase
    {
        [CheckRealElement("data,")]
        /// <summary>
        /// 结算ID
        /// </summary>
        public string setl_id { get; set; }
        /// <summary>
        /// 就诊ID
        /// </summary>
        public string mdtrt_id { get; set; }
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }

    }
}
