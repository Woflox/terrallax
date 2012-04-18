using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Terrallax
{
    public partial class Editor : Form
    {
        AreaParameters parameters = AreaParameters.DefaultParameters();

        public Editor()
        {
            InitializeComponent();
            initValues();
        }

        public void initValues()
        {
            hScale.Value = (int)parameters.hScale;
            vScale.Value = (int)parameters.vScale;
            vOffset.Value = (int)parameters.vOffset;
            octaves.Value = parameters.octaves;
            spectral_exp.Value = (int)(parameters.spectral_exp * 100);
            lacunarity.Value = (int)(parameters.lacunarity * 100);
            offset.Value = (int)(parameters.offset * 100);
            threshold.Value = (int)(parameters.threshold * 100);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Editor_Load(object sender, EventArgs e)
        {

        }

        private void hScale_Scroll(object sender, EventArgs e)
        {
            parameters.hScale = hScale.Value;
            notify();
        }

        private void vScale_Scroll(object sender, EventArgs e)
        {
            parameters.vScale = vScale.Value;
            notify();
        }

        private void vOffset_Scroll(object sender, EventArgs e)
        {
            parameters.vOffset = vOffset.Value;
            notify();
        }

        private void octaves_Scroll(object sender, EventArgs e)
        {
            parameters.octaves = octaves.Value;
            notify();
        }

        private void spectral_exp_Scroll(object sender, EventArgs e)
        {
            parameters.spectral_exp = spectral_exp.Value/100.0f;
            notify();
        }

        private void lacunarity_Scroll(object sender, EventArgs e)
        {
            parameters.lacunarity = lacunarity.Value/100.0f;
            notify();
        }

        private void offset_Scroll(object sender, EventArgs e)
        {
            parameters.offset = offset.Value / 100.0f;
            notify();
        }

        private void threshold_Scroll(object sender, EventArgs e)
        {
            parameters.threshold = threshold.Value / 100.0f;
            notify();
        }

        private void notify()
        {
            Game1.instance.terrain.parameters = parameters;
        }

        private void save_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK && saveFileDialog1.FileName != "")
            {
                parameters.Save(saveFileDialog1.FileName);
            }
        }

        private void load_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK && openFileDialog1.FileName != "")
            {
                parameters = AreaParameters.Load(openFileDialog1.FileName);
                initValues();
                notify();
            }
        }
    }
}
