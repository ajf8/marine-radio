using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Jayrock.Json;
using Jayrock.Json.Conversion;

namespace MarineRadioRTP
{
    public partial class DscDebugger : Form
    {
        private static DscDebugger instance = null;
        private static readonly object padlock = new object();

        public static DscDebugger GetInstance(MulticastCommandServer cmdSrv, CommandDispatcher dispatcher)
        {
            lock (padlock)
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new DscDebugger(cmdSrv, dispatcher);
                }
                return instance;
            }
        }

        private MulticastCommandServer cmdSrv;
        private CommandDispatcher dispatcher;

        private DscDebugger(MulticastCommandServer cmdSrv, CommandDispatcher dispatcher)
        {
            this.cmdSrv = cmdSrv;
            this.dispatcher = dispatcher;
            InitializeComponent();
        }

        private void LogIncomingItem(JsonObject obj)
        {
            LogItem(obj, "IN");
        }

        private delegate void LogItemDelegate(JsonObject obj, string direction);
        private void LogItem(JsonObject obj, string direction)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new LogItemDelegate(this.LogItem), obj, direction);
            }
            else
            {
                ListViewItem item = new ListViewItem(DateTime.Now.ToString("HH:mm:ss"));
                item.SubItems.Add(direction);
                item.SubItems.Add(obj.ToString());
                listView1.Items.Add(item);
            }
        }
        
        private void DscDebugger_Load(object sender, EventArgs e)
        {
            foreach (DscLogItem item in cmdSrv.DscLog)
            {
                LogIncomingItem(item.Data);
            }
            cmdSrv.DscLog.OnLogged += new EventHandler<DscRequestLoggedEventArgs>(DscLog_OnLogged);
            dispatcher.OnCmdDispatched += new EventHandler<CommandDispatchedEventArgs>(dispatcher_OnCmdDispatched);
        }

        void dispatcher_OnCmdDispatched(object sender, CommandDispatchedEventArgs e)
        {
            LogItem(e.Request, "OUT");
        }

        private void DscLog_OnLogged(object sender, DscRequestLoggedEventArgs e)
        {
            LogIncomingItem(e.RequestData);
        }

        private void DscDebugger_FormClosing(object sender, FormClosingEventArgs e)
        {
            cmdSrv.DscLog.OnLogged -= new EventHandler<DscRequestLoggedEventArgs>(DscLog_OnLogged);
            dispatcher.OnCmdDispatched -= new EventHandler<CommandDispatchedEventArgs>(dispatcher_OnCmdDispatched);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dispatcher.Dispatch(textBox1.Text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (JsonConvert.Import(textBox1.Text).GetType().Equals(typeof(JsonObject)))
                    button1.Enabled = true;
                else
                    button1.Enabled = false;
            }
            catch
            {
                button1.Enabled = false;
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
                Clipboard.SetText(listView1.SelectedItems[0].SubItems[2].Text);
        }
    }
}
