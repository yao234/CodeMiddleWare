using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    /// <summary>
    /// 入院撤销 2404 节点标识data 无输出
    /// </summary>
    public class InHospitalReturn : HospitalBase
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
        ///无用
        ///// <summary>
        ///// 字段扩展
        ///// </summary>
        //public string expContent { get; set; }
    }
}
