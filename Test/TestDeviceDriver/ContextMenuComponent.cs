using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestDeviceDriver
{
    public partial class ContextMenuComponent : Component
    {
        public ContextMenuComponent()
        {
            InitializeComponent();
        }

        public ContextMenuComponent(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public ContextMenuStrip ContextMenuStrip
        {
            get { return this.contextMenuStrip1; }
        }
    }
}
