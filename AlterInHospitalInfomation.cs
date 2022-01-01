using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuSoftMedicare.SQLServer.Hospital
{
    public class AlterInHospitalInfomation : HospitalBase
    {
        [CheckRealElement("adminfo,")]
        #region adminfo
     
        public string mdtrt_id { get; set; }
      
        public string psn_no { get; set; }
      
        public string coner_name { get; set; }
        
        public string tel { get; set; }
       
        public string begntime { get; set; }
      
        public string endtime { get; set; }
     
        public string mdtrt_cert_type { get; set; }
     
        public string med_type { get; set; }
       
        public string ipt_otp_no { get; set; }
       
        public string medrcdno { get; set; }
       
        public string atddr_no { get; set; }
      
        public string chfpdr_name { get; set; }
       
        public string adm_diag_dscr { get; set; }
       
        public string adm_dept_codg { get; set; }
        
        public string adm_dept_name { get; set; }
        
        public string adm_bed { get; set; }
      
        public string dscg_maindiag_code { get; set; }
        
        public string dscg_maindiag_name { get; set; }
        
        public string main_cond_dscr { get; set; }
        
        public string dise_codg { get; set; }
        
        public string dise_name { get; set; }
        
        public string oprn_oprt_code { get; set; }
      
        public string oprn_oprt_name { get; set; }
        
        public string fpsc_no { get; set; }
     
        public string matn_type { get; set; }
     
        public string birctrl_type { get; set; }
       
        public string latechb_flag { get; set; }
     
        public string geso_val { get; set; }
       
        public string fetts { get; set; }
        
        public string fetus_cnt { get; set; }
     
        public string pret_flag { get; set; }
       
        public string birctrl_matn_date { get; set; }
       
        public string dise_type_code { get; set; }
       
        //public string expContent { get; set; }

        #endregion

        #region diseinfo    
        [CheckRealElement("diseinfo,mdtrt_id")]
        
        public string remdtrt_id { get; set; }
        [CheckRealElement(",psn_no")]
 
        public string repsn_no { get; set; }
    
        public string diag_type { get; set; }
     
        public string maindiag_flag { get; set; }
      
        public string diag_srt_no { get; set; }
     
        public string diag_code { get; set; }
     
        public string diag_name { get; set; }
 
        public string adm_cond { get; set; }
     
        public string diag_dept { get; set; }
    
        public string dise_dor_no { get; set; }
       
        public string dise_dor_name { get; set; }
     
        public string diag_time { get; set; }
        #endregion
    }
}
