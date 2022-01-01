using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace WindowsFormsApp1
{
    public partial class UpdateDatasToSqlServer : Form
    {
        private static bool flag;
        public UpdateDatasToSqlServer()
        {
            InitializeComponent();
        }

        private void UpdateDatasToSqlServer_Load(object sender, EventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!flag)
            {
                SqlHelper.Init(this.txtServiceName.Text, this.txtUserName.Text, this.txtPassWord.Text);
                LoadFirstMeun();
                flag = true;
            }
        }

        void LoadFirstMeun() {
                Dictionary<string, string> list = GetAllDataBases();
                List<TreeNode> arr = new List<TreeNode>();
                foreach (var item in list.Keys)
                {
                    TreeNode node = new TreeNode();
                    node.Text = item;
                    node.Tag = list[item];
                    arr.Add(node);
                }
                if (arr.Count > 0)
                {
                    this.treeView1.Nodes.AddRange(arr.ToArray());
                }
                foreach (TreeNode item in this.treeView1.Nodes)
                {
                    string dataName = item.Text;
                    string sql = $"select name,object_id from sys.tables where type='U'";
                    SqlHelper.Init(this.txtServiceName.Text, this.txtUserName.Text, this.txtPassWord.Text, dataName);
                    Dictionary<string, string> dic = SqlHelper.ExcuteQuery(sql);
                    if (dic.Count > 0)
                    {
                        LinkedList<TreeNode> Linkedlist = new LinkedList<TreeNode>();
                        foreach (string i in dic.Keys)
                        {
                            TreeNode node = new TreeNode();
                            node.Text = i;
                            node.Tag = dic[i];
                            node.Name = "2";
                            Linkedlist.AddLast(node);
                        }
                        item.Nodes.AddRange(Linkedlist.ToArray());
                    }
                }
        }

        
        Dictionary<string,string> GetAllDataBases() {
            Dictionary<string, string> dt =null;
            string sql = "select name,database_id from sys.databases where compatibility_level=80";
            dt = SqlHelper.ExcuteQuery(sql);
            return dt;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Name.Equals("2"))
            {
                string dataBaseName = e.Node.Parent.Text;
                string object_id = e.Node.Tag.ToString();
                LoadColumns(dataBaseName,object_id);
            }
        }

        void LoadColumns(string dataBaseName,string object_id) {
            this.listBox1.Items.Clear();
            SqlHelper.Init(this.txtServiceName.Text, this.txtUserName.Text, this.txtPassWord.Text, dataBaseName);
            string sql = "select name,user_type_id from sys.columns where object_id = '" + object_id + "'";
            Dictionary<string,string> dic=SqlHelper.ExcuteQuery(sql);
            LinkedList<ListBox> arr = new LinkedList<ListBox>();
            foreach (string item in dic.Keys)
            {
                ListBox list = new ListBox();
                list.Text = item;
                list.Name = item;
                list.Tag = dic[item];
                arr.AddLast(list);
            }
            this.listBox1.Items.AddRange(arr.ToArray());
            TreeNode node = this.treeView1.SelectedNode;
            this.listBox1.Tag = node.Text + "," + node.Tag + "," + node.Parent.Text + "," + node.Parent.Tag;
            this.listBox1.DisplayMember = "text";
            this.listBox1.ValueMember = "tag";
        }

         
        private void button3_Click(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count>0)
            {
                this.listBox2.Items.Clear();
                this.listBox2.Items.AddRange(this.listBox1.Items);
                this.listBox2.Tag = this.listBox1.Tag;
                this.listBox2.DisplayMember = "text";
                this.listBox2.ValueMember = "tag";
                this.listBox1.Items.Clear();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (this.listBox2.Items.Count > 0)
            {
                this.listBox1.Items.Clear();
                this.listBox1.Items.AddRange(this.listBox2.Items);
                this.listBox1.Tag = this.listBox2.Tag;
                this.listBox1.DisplayMember = "text";
                this.listBox1.ValueMember = "tag";
                this.listBox2.Items.Clear();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MoveListItem(this.listBox1,MoveItem.MoveUp);
        }

        void MoveListItem(object send,MoveItem item) {
            ListBox list = send as ListBox;
            int count = list.Items.Count;
            if (count==0)
            {
                return;
            }
            int index = list.SelectedIndex;
            int i = index - (int)item;
            if (i < 0 || i==count)
            {
                return;
            }
            string value = list.Text;
            object val = list.Items[i];
            list.Items[i] = list.Items[index];
            list.Items[index] = val;
            list.SelectedIndex = i;
        }
        enum MoveItem
        {
            MoveUp=1,
            MoveDown=-1,
            UnMove=0
        }
        private void button5_Click(object sender, EventArgs e)
        {
            MoveListItem(this.listBox1,MoveItem.MoveDown);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MoveListItem(this.listBox2, MoveItem.MoveDown);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MoveListItem(this.listBox2, MoveItem.MoveUp);
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomeValue(this.listBox1);
        }
        void CustomeValue(object send,bool flag=false) {
            ListBox list = ((send as ListBox).SelectedItem as ListBox);
            if (list==null)
            {
                MessageBox.Show("请选中列再更改值");
                return;
            }
            ListBox parent = (send as ListBox);
            int index = parent.SelectedIndex;
            string str = list.Text;
            if (str.Contains("customValue"))
            {
                str = list.Text.Split(':')[1];
            }
            CustomValue value = new CustomValue(str);
            if (value.ShowDialog() == DialogResult.OK)
            {
                list.Text = flag ? "customValue:" + value.Tag.ToString() : value.Tag.ToString();
                parent.Items.RemoveAt(index);
                parent.Items.Insert(index, list);
                parent.SelectedIndex = index;
            }
        }

        private void 更改为ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        Dictionary<string, List<ToolStripButton>> dic = new Dictionary<string, List<ToolStripButton>>();
        void LoadToolItems(object sender)
        {
            ToolStripMenuItem t = (sender as ToolStripMenuItem);
            string val=this.treeView1.SelectedNode.Text;
            t.DropDownItems.AddRange(dic[val].ToArray());
        }

        void InitListItem() {
            string val = this.treeView1.SelectedNode.Text;
            if (dic.ContainsKey(val))
            {
                return;
            }
            List<ToolStripButton> list = new List<ToolStripButton>();
            foreach (ListBox item in this.listBox1.Items)
            {
                ToolStripButton button = new ToolStripButton();
                button.Text = item.Text;
                button.Tag = item.Tag;
                button.Click += Button_Click;
                list.Add(button);
            }
            dic.Add(val, list);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            ToolStripButton t = (sender as ToolStripButton);
            ListBox list=(this.listBox1.SelectedItem as ListBox);
            list.Text = t.Text;
            list.Tag = t.Tag;
            list.Name = list.Text;
            int index = this.listBox1.SelectedIndex;
            this.listBox1.Items.RemoveAt(index);
            this.listBox1.Items.Insert(index, list);
        }

        private void 更改为ToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            
        }

        private void 更改为ToolStripMenuItem_MouseMove(object sender, MouseEventArgs e)
        {
            LoadToolItems(sender);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitListItem();
            if (this.listBox1.SelectedIndex<this.listBox2.Items.Count)
            {
                this.listBox2.SelectedIndex = this.listBox1.SelectedIndex;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateData();
            #region MyRegion
            //            string sql= @"merge into BaseData.dbo.DM_Dict t
            //using BaseData.dbo.XYZCYML d
            //on (d.YLMLBM=t.Code)
            //when not matched then insert values(
            //d.YLMLBM,
            // d.ZCMC,
            //  d.YWMC,
            //   d.ZCMC,
            //   d.YPGG,
            //    d.YPJX, 
            //    d.YPJX
            //, 1, CAST(0.0800 AS Decimal(18, 4)),
            // CAST(0.0800 AS Decimal(18, 4)), 
            // 100, 10, 
            // CAST(0.0000 AS Decimal(18, 4)),
            //  0, 2015044, 0,
            //   2282,
            //    N'国药准字'+d.PZWH, 
            //    0, 1000001, 2015912,
            //     N'', N'', N'', 
            //     CAST(0.0000 AS Decimal(18, 4)), 
            //     0, N'', 
            //     CAST(0.00 AS Decimal(18, 2)), '',
            //      N'', N'', N'1', 
            //      d.ZCMC, 
            //      CAST(0.00 AS Decimal(18, 2)), 
            //      0, 0, CAST(0.00 AS Decimal(18, 2)), 
            //      N'', 0, NULL, NULL, NULL, N'',
            //       NULL, N'', N'', N'', N'', N'',
            //        N'11204', N'', N'', N'', 
            //        CAST(0.0000 AS Decimal(18, 4)), 
            //        CAST(0.0000 AS Decimal(18, 4)), 
            //        CAST(0.0000 AS Decimal(18, 4)), N'',
            //         N'', 0, CAST(0 AS Decimal(18, 0)), 0,
            //          CAST(0 AS Decimal(18, 0)), 0, 
            //          CAST(0 AS Decimal(18, 0)), 0, 
            //          CAST(0 AS Decimal(18, 0)), 0, 
            //          CAST(0 AS Decimal(18, 0)), 0, 
            //          CAST(0 AS Decimal(18, 0)), N'', N'',
            //           N'', N'', N'', N'', N'', N'', 
            //           d.GJYPBH, N'否'); ";
            //            SqlHelper.Init(this.txtServiceName.Text, this.txtUserName.Text, this.txtPassWord.Text);
            //           int count= SqlHelper.ExcuteSql(sql);
            #endregion
        }

        void UpdateData()
        {
            if (this.listBox1.Items.Count == 0 || this.listBox2.Items.Count == 0)
            {
                return;
            }
            string insert = this.listBox2.Tag.ToString();
            string get = this.listBox1.Tag.ToString();
            StringBuilder sb = new StringBuilder();
            sb.Append("merge into ");
            string[] arr = insert.Split(',');
            string[] arr1 = get.Split(',');
            SqlHelper.Init(this.txtServiceName.Text, this.txtUserName.Text, this.txtPassWord.Text, arr1[2]);
            string sql2 = "select name,user_name(ObjectProperty(object_id, 'ownerid')) as FN from sys.all_objects where object_id='" + arr1[1] + "'";
            Dictionary<string, string> dic2 = SqlHelper.ExcuteQuery(sql2);
            SqlHelper.Init(this.txtServiceName.Text, this.txtUserName.Text, this.txtPassWord.Text, arr[2]);
            string sql = "select name,user_name(ObjectProperty(object_id, 'ownerid')) as FN from sys.all_objects where object_id='" + arr[1] + "'";
            Dictionary<string, string> dic = SqlHelper.ExcuteQuery(sql);
            sb.Append($" {arr[2]}.{dic[arr[0]]}.{arr[0]} as d using {arr1[2]}.{dic2[arr1[0]]}.{arr1[0]} as t");
            sb.Append($" on (t.{(this.listBox1.Items[0] as ListBox).Text}=d.{(this.listBox2.Items[0] as ListBox).Text})");
            sb.Append(" when not matched then");/*insert values(*/
            sb.Append(" insert(");
            foreach (ListBox item in this.listBox2.Items)
            {
                sb.Append($"{item.Name},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(") values(");
            int insertCount = this.listBox2.Items.Count;
            int getCount = this.listBox1.Items.Count;
            int totalCount = getCount < insertCount ? getCount : insertCount;
            int i = -1;
            while (++i < totalCount)
            {
                ListBox getListBox = (this.listBox1.Items[i] as ListBox);
                string val = getListBox.Text != getListBox.Name? "'" + getListBox.Text + "'": getListBox.Name;
                sb.Append($"{val},");
             }
            if (totalCount < insertCount)
            {
                int j = totalCount - 1;
                ListBox list = null;
                while (++j < insertCount)
                {
                    list = this.listBox2.Items[j] as ListBox;
                    string text = list.Text;
                    string sqlType = "select user_type_id,name from sys.types where user_type_id='" + list.Tag + "'";
                    dic = SqlHelper.ExcuteQuery(sqlType);
                    string val = dic[list.Tag.ToString()];
                    if (val.Contains("varchar"))
                    {
                        if (text.Contains("customValue"))
                        {
                            sb.Append($"N'{text.Split(':')[1]}',");
                            continue;
                        }
                        sb.Append($"N'',");
                        continue;
                    }
                    if (val.Contains("int") || val.Equals("decimal") || val.Equals("numeric"))
                    {
                        if (text.Contains("customValue"))
                        {
                            sb.Append($"{text},");
                            continue;
                        }
                        sb.Append($"0,");
                        continue;
                    }
                }
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(");");
            MessageBox.Show(sb.ToString());
            int total = SqlHelper.ExcuteSql(sb.ToString());
            MessageBox.Show(total.ToString());
        }

        private void 自定义值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomeValue(this.listBox2,true);
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox2.SelectedIndex<this.listBox1.Items.Count)
            {
                this.listBox1.SelectedIndex = this.listBox2.SelectedIndex;
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode== Keys.F2)
            {
                CustomeValue(this.listBox1);
            }
        }

        private void listBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                CustomeValue(this.listBox2, true);
            }
        }

        private void 删除ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem==null)
            {
                return;
            }
            this.listBox1.Items.RemoveAt(this.listBox1.SelectedIndex);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem == null)
            {
                return;
            }
            int j = this.listBox1.SelectedIndex;
            int count = this.listBox1.Items.Count;
            while (--count>j)
            {
                this.listBox1.Items.RemoveAt(count);
            }
        }
    }

    public sealed class SqlHelper
    {
        private static SqlConnection con;
        public static void Init(string serverName,string uid,string passWord) {
            if (serverName==string.Empty || uid==string.Empty || passWord==string.Empty)
            {
                MessageBox.Show("文本框不能为空");
                return;
            }
            string sql = string.Format("server={0};uid={1};pwd={2};database=master", serverName,uid,passWord);
            try
            {
                con = new SqlConnection(sql);
                con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库连接异常" + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static void Init(string serverName, string uid, string passWord,string databaseName)
        {
            if (serverName == string.Empty || uid == string.Empty || passWord == string.Empty)
            {
                MessageBox.Show("文本框不能为空");
                return;
            }
            string sql = string.Format("server={0};uid={1};pwd={2};database={3}", serverName, uid, passWord,databaseName);
            try
            {
                con = new SqlConnection(sql);
                con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库连接异常" + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static Dictionary<string, string> ExcuteQuery(string sql) {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataReader sdr = cmd.ExecuteReader();
                Dictionary<string,string> list = new Dictionary<string, string>();
                while (sdr.Read())
                {
                    string val = sdr[0].ToString();
                    if (!list.ContainsKey(val))
                    {
                        list.Add(val, sdr[1].ToString());
                    }
                }
                return list;       
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }
            finally {
                con.Close();
            }
            return null;
        }

        public static int ExcuteSql(string sql)
        {
            try
            {
                //string d = "exec ("+sql+")";
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
               
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }
            finally {
                con.Close();
            }
            return -1;
        }
    }
}
