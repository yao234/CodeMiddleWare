using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    /// <summary>
    /// 门诊挂号 2201 data
    /// </summary>
    public class ClinicRegist:HospitalBase
    {
        [CheckRealElement("data,")]
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 险种类型
        /// </summary>
        public string insutype { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string begntime { get; set; }
        /// <summary>
        /// 就诊凭证类型
        /// </summary>
        public string mdtrt_cert_type { get; set; }
        /// <summary>
        /// 就诊凭证编号
        /// </summary>
        public string mdtrt_cert_no { get; set; }
        /// <summary>
        /// 住院/门诊号
        /// </summary>
        public string ipt_otp_no { get; set; }
        /// <summary>
        /// 医师编码
        /// </summary>
        public string atddr_no { get; set; }
        /// <summary>
        /// 医师姓名
        /// </summary>
        public string dr_name { get; set; }
        /// <summary>
        /// 科室编码
        /// </summary>
        public string dept_code { get; set; }
        /// <summary>
        /// 科室名称
        /// </summary>
        public string dept_name { get; set; }
        /// <summary>
        /// 科别
        /// </summary>
        public string caty { get; set; }

    }
}
