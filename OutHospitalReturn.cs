using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    /// <summary>
    /// 出院撤销 2405 无输出 节点标识data
    /// </summary>
    public class OutHospitalReturn : HospitalBase
    {

        [CheckRealElement("data,")]
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
