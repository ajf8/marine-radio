using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Microsoft.DirectX.DirectSound;
using System.Reflection;

namespace MarineRadioRTP
{
    public partial class ConfigurationDialog : Form
    {
        private static ConfigurationDialog instance = null;
        private static readonly object padlock = new object();

        public static ConfigurationDialog Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null || instance.IsDisposed)
                    {
                        instance = new ConfigurationDialog();
                    }
                    return instance;
                }
            }
        }

        private ConfigurationDialog()
        {
            InitializeComponent();
        }

        private void ConfigurationDialog_Load(object sender, EventArgs e)
        {
            ConfSingleton settings = ConfSingleton.Instance;
            try
            {
                strategyDescription.Text = settings.Network.ToString();
            }
            catch (Exception ex)
            {
                strategyDescription.Text = ex.Message;
            }
            enableSoundEffectsCheckBox.Checked = settings.SoundEffects;
            multicastNetworkTextBox.Text = settings.MulticastNetworkStart;
            multiCastSubnetValue.Value = settings.MulticastPrefix;
            compressDecompressCheckBox.Checked = settings.Compression;
            multiCastSubnetValue.ValueChanged += new System.EventHandler(this.AddressChanged);
            multicastNetworkTextBox.TextChanged += new System.EventHandler(this.AddressChanged);
            CaptureDevicesCollection captureDevices = new CaptureDevicesCollection();
            for (int i = 0; i < captureDevices.Count; i++)
            {
                listBox1.Items.Add(captureDevices[i].Description);
                if (i.Equals(settings.CaptureDeviceIndex))
                    listBox1.SelectedIndex = i;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Save()
        {
            ConfSingleton settings = ConfSingleton.Instance;
            try
            {
                settings.MulticastNetworkStart = multicastNetworkTextBox.Text;
                settings.MulticastPrefix = (int)multiCastSubnetValue.Value;
                settings.CaptureDeviceIndex = listBox1.SelectedIndex;
                settings.SoundEffects = enableSoundEffectsCheckBox.Checked;
                settings.Compression = compressDecompressCheckBox.Checked;
                settings.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to persist configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayAddressingStrategy()
        {
            try
            {
                MulticastNetwork network = new MulticastNetwork(multicastNetworkTextBox.Text, (int)multiCastSubnetValue.Value);
                strategyDescription.Text = network.ToString();
            }
            catch (Exception ex)
            {
                strategyDescription.Text = ex.Message;
            }
        }

        private void AddressChanged(object sender, EventArgs e)
        {
            DisplayAddressingStrategy();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Save();
            Close();
        }
    }
}
