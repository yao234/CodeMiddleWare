using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    /// <summary>
    /// 2202 门诊挂号撤销
    /// </summary>
    public class ClinicRegistReturn:HospitalBase
    {
        [CheckRealElement("data,")]
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 就诊ID
        /// </summary>
        public string mdtrt_id { get; set; }
        /// <summary>
        /// 住院/门诊号
        /// </summary>
        public string ipt_otp_no { get; set; }
        /// <summary>
        /// 门诊号
        /// </summary>
        public string expContent { get; set; }
    }
}
