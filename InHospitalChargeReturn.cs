using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    /// <summary>
    /// 住院费用明细撤销 2302 data 无输出 
    /// </summary>
    public class InHospitalChargeReturn : HospitalBase
    {
        [CheckRealElement("data,")]
        /// <summary>
        /// 费用明细流水号
        /// </summary>
        public string feedetl_sn { get; set; }
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
