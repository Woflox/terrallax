namespace Terrallax
{
    partial class Editor
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
            this.hScale = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.vScale = new System.Windows.Forms.TrackBar();
            this.vOffset = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.octaves = new System.Windows.Forms.TrackBar();
            this.spectral_exp = new System.Windows.Forms.TrackBar();
            this.lacunarity = new System.Windows.Forms.TrackBar();
            this.offset = new System.Windows.Forms.TrackBar();
            this.threshold = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.save = new System.Windows.Forms.Button();
            this.load = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.hScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.octaves)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spectral_exp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lacunarity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.offset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.threshold)).BeginInit();
            this.SuspendLayout();
            // 
            // hScale
            // 
            this.hScale.Location = new System.Drawing.Point(57, 12);
            this.hScale.Maximum = 8000;
            this.hScale.Name = "hScale";
            this.hScale.Size = new System.Drawing.Size(263, 45);
            this.hScale.TabIndex = 0;
            this.hScale.Scroll += new System.EventHandler(this.hScale_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "hScale";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "vScale";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // vScale
            // 
            this.vScale.Location = new System.Drawing.Point(57, 46);
            this.vScale.Maximum = 1000;
            this.vScale.Name = "vScale";
            this.vScale.Size = new System.Drawing.Size(263, 45);
            this.vScale.TabIndex = 3;
            this.vScale.Scroll += new System.EventHandler(this.vScale_Scroll);
            // 
            // vOffset
            // 
            this.vOffset.Location = new System.Drawing.Point(57, 76);
            this.vOffset.Maximum = 600;
            this.vOffset.Name = "vOffset";
            this.vOffset.Size = new System.Drawing.Size(263, 45);
            this.vOffset.TabIndex = 4;
            this.vOffset.Scroll += new System.EventHandler(this.vOffset_Scroll);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "vOffset";
            // 
            // octaves
            // 
            this.octaves.Location = new System.Drawing.Point(57, 108);
            this.octaves.Maximum = 16;
            this.octaves.Name = "octaves";
            this.octaves.Size = new System.Drawing.Size(263, 45);
            this.octaves.TabIndex = 6;
            this.octaves.Scroll += new System.EventHandler(this.octaves_Scroll);
            // 
            // spectral_exp
            // 
            this.spectral_exp.Location = new System.Drawing.Point(57, 142);
            this.spectral_exp.Maximum = 100;
            this.spectral_exp.Name = "spectral_exp";
            this.spectral_exp.Size = new System.Drawing.Size(263, 45);
            this.spectral_exp.TabIndex = 7;
            this.spectral_exp.Scroll += new System.EventHandler(this.spectral_exp_Scroll);
            // 
            // lacunarity
            // 
            this.lacunarity.Location = new System.Drawing.Point(57, 174);
            this.lacunarity.Maximum = 300;
            this.lacunarity.Name = "lacunarity";
            this.lacunarity.Size = new System.Drawing.Size(263, 45);
            this.lacunarity.TabIndex = 8;
            this.lacunarity.Scroll += new System.EventHandler(this.lacunarity_Scroll);
            // 
            // offset
            // 
            this.offset.Location = new System.Drawing.Point(57, 202);
            this.offset.Maximum = 300;
            this.offset.Name = "offset";
            this.offset.Size = new System.Drawing.Size(263, 45);
            this.offset.TabIndex = 9;
            this.offset.Scroll += new System.EventHandler(this.offset_Scroll);
            // 
            // threshold
            // 
            this.threshold.Location = new System.Drawing.Point(57, 235);
            this.threshold.Maximum = 100;
            this.threshold.Name = "threshold";
            this.threshold.Size = new System.Drawing.Size(263, 45);
            this.threshold.TabIndex = 10;
            this.threshold.Scroll += new System.EventHandler(this.threshold_Scroll);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "octaves";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "spectral";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 174);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "lacunarity";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 235);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "threshold";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 206);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "offset";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "xml";
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "XML files|*.xml";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "xml";
            this.saveFileDialog1.Filter = "XML files|*.xml";
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(40, 286);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(112, 39);
            this.save.TabIndex = 16;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // load
            // 
            this.load.Location = new System.Drawing.Point(183, 286);
            this.load.Name = "load";
            this.load.Size = new System.Drawing.Size(112, 39);
            this.load.TabIndex = 17;
            this.load.Text = "Load";
            this.load.UseVisualStyleBackColor = true;
            this.load.Click += new System.EventHandler(this.load_Click);
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 351);
            this.Controls.Add(this.load);
            this.Controls.Add(this.save);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.threshold);
            this.Controls.Add(this.offset);
            this.Controls.Add(this.lacunarity);
            this.Controls.Add(this.spectral_exp);
            this.Controls.Add(this.octaves);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.vOffset);
            this.Controls.Add(this.vScale);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.hScale);
            this.Name = "Editor";
            this.Text = "Editor";
            this.Load += new System.EventHandler(this.Editor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.hScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.octaves)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spectral_exp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lacunarity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.offset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.threshold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar hScale;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar vScale;
        private System.Windows.Forms.TrackBar vOffset;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar octaves;
        private System.Windows.Forms.TrackBar spectral_exp;
        private System.Windows.Forms.TrackBar lacunarity;
        private System.Windows.Forms.TrackBar offset;
        private System.Windows.Forms.TrackBar threshold;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button load;
    }
}