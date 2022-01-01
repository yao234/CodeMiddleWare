using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    /// <summary>
    /// 出院办理 2402 无输出
    /// </summary>
    public class OutHospital : HospitalBase
    {
        #region dscginfo
        [CheckRealElement("dscginfo,")]
        /// <summary>
        /// 就诊ID
        /// </summary>
        public string mdtrt_id { get; set; }
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 险种类型
        /// </summary>
        public string insutype { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string endtime { get; set; }
        /// <summary>
        /// 病种编码
        /// </summary>
        public string dise_codg { get; set; }
        /// <summary>
        /// 病种名称
        /// </summary>
        public string dise_name { get; set; }
        /// <summary>
        /// 手术操作代码
        /// </summary>
        public string oprn_oprt_code { get; set; }
        /// <summary>
        /// 手术操作名称
        /// </summary>
        public string oprn_oprt_name { get; set; }
        /// <summary>
        /// 计划生育服务证号
        /// </summary>
        public string fpsc_no { get; set; }
        /// <summary>
        /// 生育类别
        /// </summary>
        public string matn_type { get; set; }
        /// <summary>
        /// 计划生育手术类别
        /// </summary>
        public string birctrl_type { get; set; }
        /// <summary>
        /// 晚育标志
        /// </summary>
        public string latechb_flag { get; set; }
        /// <summary>
        /// 孕周数
        /// </summary>
        public string geso_val { get; set; }
        /// <summary>
        /// 胎次
        /// </summary>
        public string fetts { get; set; }
        /// <summary>
        /// 胎儿数
        /// </summary>
        public string fetus_cnt { get; set; }
        /// <summary>
        /// 早产标志
        /// </summary>
        public string pret_flag { get; set; }
        /// <summary>
        /// 计划生育手术或生育日期
        /// </summary>
        public string birctrl_matn_date { get; set; }
        /// <summary>
        /// 伴有并发症标志
        /// </summary>
        public string cop_flag { get; set; }
        /// <summary>
        /// 出院科室编码
        /// </summary>
        public string dscg_dept_codg { get; set; }
        /// <summary>
        /// 出院科室名称
        /// </summary>
        public string dscg_dept_name { get; set; }
        /// <summary>
        /// 出院床位
        /// </summary>
        public string dscg_bed { get; set; }
        /// <summary>
        /// 离院方式
        /// </summary>
        public string dscg_way { get; set; }
        /// <summary>
        /// 死亡日期
        /// </summary>
        public string die_date { get; set; }
        /// <summary>
        /// 字段扩展
        /// </summary>
        public string expContent { get; set; }
        #endregion

        #region 节点标识 diseinfo
        [CheckRealElement("diseinfo,mdtrt_id")]
        /// <summary>
        /// 就诊ID
        /// </summary>
        public string remdtrt_id { get; set; }
        [CheckRealElement(",psn_no")]
        /// <summary>
        /// 人员编号
        /// </summary>
        public string repsn_no { get; set; }
        /// <summary>
        /// 诊断类别
        /// </summary>
        public string diag_type { get; set; }
        /// <summary>
        /// 主诊断标志
        /// </summary>
        public string maindiag_flag { get; set; }
        /// <summary>
        /// 诊断排序号
        /// </summary>
        public string diag_srt_no { get; set; }
        /// <summary>
        /// 诊断代码
        /// </summary>
        public string diag_code { get; set; }
        /// <summary>
        /// 诊断名称
        /// </summary>
        public string diag_name { get; set; }
        /// <summary>
        /// 诊断科室
        /// </summary>
        public string diag_dept { get; set; }
        /// <summary>
        /// 诊断医生编码
        /// </summary>
        public string dise_dor_no { get; set; }
        /// <summary>
        /// 诊断医生名称
        /// </summary>
        public string dise_dor_name { get; set; }
        /// <summary>
        /// 诊断时间
        /// </summary>
        public string diag_time { get; set; }
        #endregion
    }
}
