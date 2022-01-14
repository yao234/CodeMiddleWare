using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NeuSoftMedicare.Interface;
using NeuSoftMedicare.SQLServer;
namespace SmartHIS.Integral.DataCenter.UI
{
    public partial class InsutypeForm : Form
    {
        
        public InsutypeForm()
        {
            InitializeComponent();
        }

        private void InsutypeForm_Load(object sender, EventArgs e)
        {
            this.dataGridView1.Rows.Clear();
            IInsutypeTableList list = DALHelper.DALManager.CreateTempIInsutypeTableList();
            DataTable dt=list.GetDataTable();
            LinkedList<DataGridViewRow> rows = new LinkedList<DataGridViewRow>();
            foreach (DataRow item in dt.Rows)
            {
                int val =  item["catorytype"] is DBNull?int.MinValue:(int)item["catorytype"];
                if (val != int.MinValue && val != 0)
                {
                    DataGridViewRow dr = new DataGridViewRow();
                    dr.CreateCells(this.dataGridView1);
                    dr.Cells[0].Value = item["Code"].ToString();
                    dr.Cells[1].Value = item["Name"].ToString();
                    dr.Cells[2].Value = SmartHIS.Common.ContextHelper.DbConnection.CreateAccessor().Query("select Name from BaseData.dbo.InsutypeTable where Code='"+ item["catorytype"] + "'");
                    dr.Cells[3].Value = GetRealName(item["customtype"]);
                    dr.Tag = item;
                    rows.AddLast(dr);
                }
            }
            this.dataGridView1.Rows.AddRange(rows.ToArray());
        }

        string GetRealName(object val) {
            if (val is DBNull || val==null)
            {
                return "";
            }
            switch (Convert.ToInt32(val))
            {
                case 11:
                    return "门诊";
                case 21:
                    return "住院";
                default:
                    return "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InsutypeAdd insutype = new InsutypeAdd();
            insutype.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            InsutypeAdd insutype = new InsutypeAdd(EnumOpretion.Insert, EnumAddCatetory.First);
            insutype.ShowDialog();
        }
    }
}
