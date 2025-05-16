using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PluginContracts;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace mdi_paint
{
    public partial class PluginManagerForm : Form
    {
        public PluginManagerForm(IEnumerable<IImageFilter> filters)
        {
            InitializeComponent();
            listView1.View = View.Details;
            listView1.Columns.Add("Name");
            listView1.Columns.Add("Author");
            listView1.Columns.Add("Version");

            foreach (var filter in filters)
            {
                var item = new ListViewItem(filter.Name);
                item.SubItems.Add(filter.Author);
                item.SubItems.Add(filter.Version.ToString());
                item.Tag = filter;
                listView1.Items.Add(item);
            }
        }

        private void buttonInfo_Click(object sender, EventArgs e)
        {
            
        }

        private void buttoninfo_Click_1(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var filter = listView1.SelectedItems[0].Tag as IImageFilter;
                MessageBox.Show($"Name: {filter.Name}\nAuthor: {filter.Author}\nVersion: {filter.Version}", "Plugin Info");
            }
        }

        private void PluginManagerForm_Load(object sender, EventArgs e)
        {

        }
    }
}