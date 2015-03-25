namespace BusFindingDemo
{
    partial class Form1
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
            this.gmap = new GMap.NET.WindowsForms.GMapControl();
            this.listRouter = new System.Windows.Forms.ComboBox();
            this.btnCompute = new System.Windows.Forms.Button();
            this.tbRouters = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnShowListRouter = new System.Windows.Forms.Button();
            this.btnCompute2 = new System.Windows.Forms.Button();
            this.btnShowStation = new System.Windows.Forms.Button();
            this.btnShowHide = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // gmap
            // 
            this.gmap.Bearing = 0F;
            this.gmap.CanDragMap = true;
            this.gmap.EmptyTileColor = System.Drawing.Color.Navy;
            this.gmap.GrayScaleMode = false;
            this.gmap.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gmap.LevelsKeepInMemmory = 5;
            this.gmap.Location = new System.Drawing.Point(12, 12);
            this.gmap.MarkersEnabled = true;
            this.gmap.MaxZoom = 2;
            this.gmap.MinZoom = 2;
            this.gmap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gmap.Name = "gmap";
            this.gmap.NegativeMode = false;
            this.gmap.PolygonsEnabled = true;
            this.gmap.RetryLoadTile = 0;
            this.gmap.RoutesEnabled = true;
            this.gmap.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gmap.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gmap.ShowTileGridLines = false;
            this.gmap.Size = new System.Drawing.Size(777, 466);
            this.gmap.TabIndex = 0;
            this.gmap.Zoom = 0D;
            // 
            // listRouter
            // 
            this.listRouter.FormattingEnabled = true;
            this.listRouter.Location = new System.Drawing.Point(795, 12);
            this.listRouter.Name = "listRouter";
            this.listRouter.Size = new System.Drawing.Size(121, 21);
            this.listRouter.TabIndex = 1;
            this.listRouter.SelectedIndexChanged += new System.EventHandler(this.listRouter_SelectedIndexChanged);
            // 
            // btnCompute
            // 
            this.btnCompute.Location = new System.Drawing.Point(796, 40);
            this.btnCompute.Name = "btnCompute";
            this.btnCompute.Size = new System.Drawing.Size(75, 23);
            this.btnCompute.TabIndex = 2;
            this.btnCompute.Text = "Compute Path";
            this.btnCompute.UseVisualStyleBackColor = true;
            this.btnCompute.Click += new System.EventHandler(this.btnCompute_Click);
            // 
            // tbRouters
            // 
            this.tbRouters.Location = new System.Drawing.Point(795, 103);
            this.tbRouters.Name = "tbRouters";
            this.tbRouters.Size = new System.Drawing.Size(100, 20);
            this.tbRouters.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(795, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Router show:";
            // 
            // btnShowListRouter
            // 
            this.btnShowListRouter.Location = new System.Drawing.Point(798, 130);
            this.btnShowListRouter.Name = "btnShowListRouter";
            this.btnShowListRouter.Size = new System.Drawing.Size(75, 23);
            this.btnShowListRouter.TabIndex = 5;
            this.btnShowListRouter.Text = "Show";
            this.btnShowListRouter.UseVisualStyleBackColor = true;
            this.btnShowListRouter.Click += new System.EventHandler(this.btnShowListRouter_Click);
            // 
            // btnCompute2
            // 
            this.btnCompute2.Location = new System.Drawing.Point(798, 159);
            this.btnCompute2.Name = "btnCompute2";
            this.btnCompute2.Size = new System.Drawing.Size(75, 23);
            this.btnCompute2.TabIndex = 2;
            this.btnCompute2.Text = "Compute Path";
            this.btnCompute2.UseVisualStyleBackColor = true;
            this.btnCompute2.Click += new System.EventHandler(this.btnCompute2_Click);
            // 
            // btnShowStation
            // 
            this.btnShowStation.Location = new System.Drawing.Point(798, 204);
            this.btnShowStation.Name = "btnShowStation";
            this.btnShowStation.Size = new System.Drawing.Size(82, 23);
            this.btnShowStation.TabIndex = 6;
            this.btnShowStation.Text = "Show stations";
            this.btnShowStation.UseVisualStyleBackColor = true;
            this.btnShowStation.Click += new System.EventHandler(this.btnShowStation_Click);
            // 
            // btnShowHide
            // 
            this.btnShowHide.Location = new System.Drawing.Point(795, 455);
            this.btnShowHide.Name = "btnShowHide";
            this.btnShowHide.Size = new System.Drawing.Size(75, 23);
            this.btnShowHide.TabIndex = 7;
            this.btnShowHide.Text = "show/hide";
            this.btnShowHide.UseVisualStyleBackColor = true;
            this.btnShowHide.Click += new System.EventHandler(this.btnShowHide_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 490);
            this.Controls.Add(this.btnShowHide);
            this.Controls.Add(this.btnShowStation);
            this.Controls.Add(this.btnShowListRouter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbRouters);
            this.Controls.Add(this.btnCompute2);
            this.Controls.Add(this.btnCompute);
            this.Controls.Add(this.listRouter);
            this.Controls.Add(this.gmap);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GMap.NET.WindowsForms.GMapControl gmap;
        private System.Windows.Forms.ComboBox listRouter;
        private System.Windows.Forms.Button btnCompute;
        private System.Windows.Forms.TextBox tbRouters;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnShowListRouter;
        private System.Windows.Forms.Button btnCompute2;
        private System.Windows.Forms.Button btnShowStation;
        private System.Windows.Forms.Button btnShowHide;
    }
}

