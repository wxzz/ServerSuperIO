using System.Windows.Forms;

namespace TestShowForm
{
    partial class ShowForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._chart = new ScaleChart.ScaleChart();
            this.SuspendLayout();
            // 
            // _chart
            // 
            this._chart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this._chart.ChartType = ScaleChart.ChartType.Line;
            this._chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this._chart.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._chart.Location = new System.Drawing.Point(0, 0);
            this._chart.Name = "_chart";
            this._chart.Size = new System.Drawing.Size(584, 361);
            this._chart.TabIndex = 0;
            this._chart.YMaxValue = 1000;
            // 
            // ShowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this._chart);
            this.Name = "ShowForm";
            this.Text = "显示视图（可以二次开发）";
            this.ResumeLayout(false);

        }
        private ScaleChart.ScaleChart _chart;
        #endregion
    }
}