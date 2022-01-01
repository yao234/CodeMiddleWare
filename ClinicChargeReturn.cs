using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    /// <summary>
    /// 门诊费用信息撤销 2205
    /// </summary>
    public class ClinicChargeReturn:HospitalBase
    {
        [CheckRealElement("data,")]
        /// <summary>
        /// 就诊ID
        /// </summary>
        public string mdtrt_id { get; set; }
        /// <summary>
        /// 收费批次号
        /// </summary>
        public string chrg_bchno { get; set; }
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
    }
}
