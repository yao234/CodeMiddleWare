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
    public partial class InsutypeAdd : Form
    {
        private readonly EnumOpretion opretion;
        private readonly EnumAddCatetory catetory;
        public InsutypeAdd():this(EnumOpretion.Insert, EnumAddCatetory.Second)
        {
           
        }

        public InsutypeAdd(EnumOpretion enumOpretion,EnumAddCatetory enumAdd) {
            this.opretion = enumOpretion;
            this.catetory = enumAdd;
            InitializeComponent();
        }

       

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InsutypeAdd_Load(object sender, EventArgs e)
        {
            switch (this.opretion)
            {
                case EnumOpretion.Insert:
                    InitCmbTypeItems();
                    InitCmbPositionItems(true);
                    break;
                case EnumOpretion.Update:
                    break;
            }
        }

        void InitCmbPositionItems(bool flag)
        {
            this.cmbPosition.Items.Clear();
            Array array = Enum.GetValues(typeof(EnumDisplayPosition));
            List<ComboBox> list = new List<ComboBox>();
            foreach (var item in array)
            {
                if (!flag)
                {
                    if (((int)item) == -1)
                    {
                        continue;
                    }
                }
                ComboBox cb = new ComboBox();
                cb.Text = item.ToString();
                cb.Tag = (int)item;
                list.Add(cb);
            }
            this.cmbPosition.DataSource = list;
            this.cmbPosition.DisplayMember = "Text";
            this.cmbPosition.ValueMember = "Tag";
        }

        void InitCmbTypeItems()
        {
            this.cmbType.Items.Clear();
            IInsutypeTableList insutypeTable = DALHelper.DALManager.CreateTempIInsutypeTableList();
            DataTable dt = insutypeTable.GetDataTable();
            if (dt.Rows.Count > 0)
            {
                List<ComboBox> boxes = new List<ComboBox>();
                foreach (DataRow item in dt.Rows)
                {
                    int val = item["catoryType"] is DBNull ? int.MinValue : Convert.ToInt32(item["catoryType"]);
                    if (val == 0)
                    {
                        ComboBox cb = new ComboBox();
                        cb.Text = item["Name"].ToString();
                        cb.Tag = item;
                        boxes.Add(cb);
                    }
                }
                this.cmbType.DataSource = boxes;
                this.cmbType.DisplayMember = "Text";
                this.cmbType.ValueMember = "Tag";
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (this.opretion)
            {
                case EnumOpretion.Insert:
                    Insert();
                    break;
                case EnumOpretion.Update:
                    break;
                default:
                    break;
            }
        }

        void Insert() {
            if (this.textBox1.Text.ToString().Substring(0,1).Equals("0"))
            {
                MessageBox.Show("省医保编码第一位不能为0");
                return;
            }
            if (this.textBox2.Text==string.Empty || this.textBox2.Text.Trim().Equals("null"))
            {
                MessageBox.Show("请输入名称");
                return;
            }
            try
            {
                object val = this.cmbPosition.SelectedValue;
                DataRow dt = (this.cmbType.SelectedValue as DataRow);
                IInsutypeTable insutype = DALHelper.DALManager.CreateTempIInsutypeTable();
                insutype.Code = Convert.ToInt32(this.textBox1.Text);
                insutype.Name = this.textBox2.Text;
                insutype.customtype =this.catetory== EnumAddCatetory.First?int.MinValue:Convert.ToInt32(val);
                insutype.catorytype = this.catetory == EnumAddCatetory.First ? 0:Convert.ToInt32(dt["Code"]);
                insutype.Save();
                MessageBox.Show("添加成功");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("添加错误:"+ex.InnerException.Message.ToString());
            }
         
        
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || Convert.ToInt32(e.KeyChar) == 8)
            {
                e.Handled = false;
                return;
            }
            e.Handled = true;
        }
    }

    public enum EnumOpretion
    {
        Insert,
        Update
    }

    public enum EnumAddCatetory { 
        First,
        Second
    }

    enum EnumDisplayPosition
    {
        门诊=11,
        住院=21,
        其他=-1
    }
}
