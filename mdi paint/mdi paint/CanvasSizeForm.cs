using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mdi_paint
{
    public partial class CanvasSizeForm : Form
    {
        public int CanvasWidth
        {
            get { return int.Parse(txtWidth.Text); }
            set { txtWidth.Text = value.ToString(); }
        }

        public int CanvasHeight
        {
            get { return int.Parse(txtHeight.Text); }
            set { txtHeight.Text = value.ToString(); }
        }

        public CanvasSizeForm()
        {
            InitializeComponent();
        }

        private void CanvasSizeForm_Load(object sender, EventArgs e)
        {

        }
        

    }
}
