using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nvmParser
{
    class parseProc : BackgroundWorker
    {
        internal parseProc(MainWindow wnd)
        {
            this.wnd = wnd;
            WorkerReportsProgress = true; WorkerSupportsCancellation = true;
            DoWork += backgroundWorker_DoWork; ProgressChanged += backgroundWorker_ProgressChanged;
            RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
        }

        public static StringBuilder strToLog = new StringBuilder();
        MainWindow wnd;
        internal ReadContext context;
        BaseBinaryFied curField;
        internal void Init(object rootElmnt)
        {
            context = new ReadContext();
            curField = context.InitParseContext(rootElmnt, wnd);
            wnd.fieldsTree.ItemsSource = ((parentBinValue)context.roootParentBin).Children;
            RunWorkerAsync();
        }
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            int level = 0;
            int levInc = 1;
            level += levInc;

            int dbgCnt = 1;
            try
            {
                context.PrintArraySize();
                while (curField != null)
                {
                    printName(curField, level);
//                   Thread.Sleep(10);
                    if (dbgCnt == 10000) break;
                    curField = context.getNextFieldWithParent(curField, out levInc);
                    level += levInc;
                    if (!reportProgressOk(dbgCnt++)) return ;
                }
            }
            catch (Exception expt)
            {
                strToLog.Append("Exception : " + expt.ToString());
                AllLogMsgsSend(dbgCnt);
            }
        }
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
  //          progressBar.Value = e.ProgressPercentage;
        }
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                //MessageBox.Show("Search cancelled.");
            }
            else if (e.Error != null)
            {
                // An error was thrown by the DoWork event handler.
                //MessageBox.Show(e.Error.Message, "An Error Occurred");
            }
            else
            {
            }
            //            MainMenu.IsEnabled = true;
            //            progressBar.Value = 0;
            wnd.fieldsTree.ItemsSource = null;
            wnd.fieldsTree.ItemsSource = ((parentBinValue)context.roootParentBin).Children;
        }
        internal void printName(object field, int level = 0)
        {
            return;
            for (int i = 0; i < level; i++)
            {
                strToLog.Append(" - ");
            }
            strToLog.Append(field.ToString());
            BaseBinaryFied obg1 = field as BaseBinaryFied;
            if (obg1 != null)
                strToLog.Append("; " + obg1.Name);
        }
        internal void AllLogMsgsSend(int cnt)
        {
            string msg = strToLog.ToString();
            strToLog.Clear();
            msg += context.getMessage();
            if (msg.Length > 0)
                //dsp.Invoke(new Action(() => wnd.textBox.AppendText(msg)));
                wnd.Dispatcher.Invoke(new Action(() => 
                { 
                    wnd.textBox.Items.Add(msg);
                    wnd.ValueCnt.Text = cnt.ToString();
                    wnd.scrollToButtom(); 
                }));
            else
                wnd.Dispatcher.Invoke(new Action(() =>
                {
                    wnd.ValueCnt.Text = cnt.ToString();
                }));

        }
        internal bool reportProgressOk(int cnt)
        {
            BackgroundWorker backgroundWorker = this;
            AllLogMsgsSend(cnt);
            Thread.Sleep(0);
            if ((backgroundWorker != null))
            {
                if (backgroundWorker.CancellationPending)
                {
                    // Return without doing any more work.
                    return false;
                }

                if (backgroundWorker.WorkerReportsProgress)
                {
                    //float progress = ((float)(i + 1)) / list.Length * 100;
                    backgroundWorker.ReportProgress(cnt);
                    //(int)Math.Round(progress));
                }
            }
            return true;
        }
    }
}
