using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Show;
using System.Windows.Forms;

namespace TestShowForm
{
    public class Graphics:GraphicsShow
    {
        private ShowForm showForm;
        public Graphics()
        {
            showForm =new ShowForm();
            showForm.FormClosed+=showForm_FormClosed;
        }

        private void showForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
           this.OnGraphicsShowClosed(this.ShowKey);
        }


        public override string ShowKey {
            get { return "show"; }
        }
        public override string ShowName {
            get { return "视图"; }
        }

        public override void ShowGraphics(IWin32Window windows)
        {
            showForm.MdiParent = (Form)windows;
            showForm.Show();
        }

        public override void CloseGraphics()
        {
            showForm.Close();
        }

        public override void UpdateDevice(string devid, object obj)
        {
            string[] arr = (string[]) obj;
            showForm.Update(arr);
        }

        public override void RemoveDevice(string devid)
        {
            //不操作
        }

        public override void Dispose()
        {
            showForm.Dispose();
        }


    }
}
