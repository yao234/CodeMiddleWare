using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    /// <summary>
    /// 新医保冲正 2601
    /// </summary>
    public class NewYiBaoChongZheng:HospitalBase
    {
        [CheckRealElement("data,")]
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 原发送报文ID
        /// </summary>
        public string omsgid { get; set; }
        /// <summary>
        /// 原交易编号
        /// </summary>
        public string oinfno { get; set; }
    }
}
