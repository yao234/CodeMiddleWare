using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    /// <summary>
    /// 住院费用明细上传 2301 feedetail 有输出
    /// </summary>
    public class InHospitalChargeUpLoad : HospitalBase
    {
        [CheckRealElement("feedetail,")]
        /// <summary>
        /// 费用明细流水号
        /// </summary>
        public string feedetl_sn { get; set; }
        /// <summary>
        /// 原费用流水号
        /// </summary>
        public string init_feedetl_sn { get; set; }
        /// <summary>
        /// 就诊ID
        /// </summary>
        public string mdtrt_id { get; set; }
        /// <summary>
        /// 医嘱号
        /// </summary>
        public string drord_no { get; set; }
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 医疗类别
        /// </summary>
        public string med_type { get; set; }
        /// <summary>
        /// 费用发生时间
        /// </summary>
        public string fee_ocur_time { get; set; }
        /// <summary>
        /// 医疗目录编码
        /// </summary>
        public string med_list_codg { get; set; }
        /// <summary>
        /// 医疗机构目录编码
        /// </summary>
        public string medins_list_codg { get; set; }
        /// <summary>
        /// 明细项目费用总额
        /// </summary>
        public string det_item_fee_sumamt { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public string cnt { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public string pric { get; set; }
        /// <summary>
        /// 开单科室编码
        /// </summary>
        public string bilg_dept_codg { get; set; }
        /// <summary>
        /// 开单科室名称
        /// </summary>
        public string bilg_dept_name { get; set; }
        /// <summary>
        /// 开单医生编码
        /// </summary>
        public string bilg_dr_codg { get; set; }
        /// <summary>
        /// 开单医生姓名
        /// </summary>
        public string bilg_dr_name { get; set; }
        /// <summary>
        /// 受单科室编码
        /// </summary>
        public string acord_dept_codg { get; set; }
        /// <summary>
        /// 受单科室名称
        /// </summary>
        public string acord_dept_name { get; set; }
        /// <summary>
        /// 受单医生编码
        /// </summary>
        public string orders_dr_code { get; set; }
        /// <summary>
        /// 受单医生姓名
        /// </summary>
        public string orders_dr_name { get; set; }
        /// <summary>
        /// 医院审批标志
        /// </summary>
        public string hosp_appr_flag { get; set; }
        /// <summary>
        /// 中药使用方式
        /// </summary>
        public string tcmdrug_used_way { get; set; }
        /// <summary>
        /// 外检标志
        /// </summary>
        public string etip_flag { get; set; }
        /// <summary>
        /// 外检医院编码
        /// </summary>
        public string etip_hosp_code { get; set; }
        /// <summary>
        /// 出院带药标志
        /// </summary>
        public string dscg_tkdrug_flag { get; set; }
        /// <summary>
        /// 生育费用标志
        /// </summary>
        public string matn_fee_flag { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string memo { get; set; }
        /// <summary>
        /// 字段扩展
        /// </summary>
        public string expContent { get; set; }
        /// <summary>
        /// 草药省码
        /// </summary>
        public string tcmherb_prov_code { get; set; }
        /// <summary>
        /// 耗材省码
        /// </summary>
        public string mcs_prov_code { get; set; }

    }
}
