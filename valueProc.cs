using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nvmParser
{
    class DeclarationProc : BackgroundWorker
    {
        internal DeclarationProc(MainWindow wnd)
        {
            this.wnd = wnd;
            WorkerReportsProgress = true; WorkerSupportsCancellation = true;
            DoWork += backgroundWorker_DoWork; ProgressChanged += backgroundWorker_ProgressChanged;
            RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
        }

        public static StringBuilder strToLog = new StringBuilder();
        MainWindow wnd;
        internal ParseContext context;
        BaseBinaryFied curField;
        internal void Init(DescriptionRoot rootElmnt)
        {
            this.rootElmnt = rootElmnt;
            context = new ParseContext();
            context.InitParseContext(rootElmnt);
            level = 0;
            printName(rootElmnt, level);
            int levInc;
            curField = context.getNextFieldWithParent((ComplexField)rootElmnt, out levInc);
            level += levInc;
            RunWorkerAsync();
        }
        int level = 0;
        DescriptionRoot rootElmnt;
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int levInc;
            int dbgCnt = 1;
            //           try
            {
                while (curField != null)
                {
                    printName(curField, level);
                    if (!curField.initInTree())
                    {
                        strToLog.Append("\n ---- Init ERROR!!!");
                        break;
                    }

                    curField = context.getNextFieldWithParent(curField, out levInc);
                    level += levInc;
                    if (!reportProgressOk(dbgCnt++)) return;
                }
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
            List<object> lst = new List<object>(1);
            lst.Add(rootElmnt);
            wnd.fieldsTree.ItemsSource = lst;
        }
        internal void printName(object field, int level = 0)
        {
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
            if (msg.Length > 0)
                //dsp.Invoke(new Action(() => wnd.textBox.AppendText(msg)));
                wnd.Dispatcher.Invoke(new Action(() => 
                {
                    wnd.textBox.Items.Add(msg); 
                    wnd.DeclCnt.Text = cnt.ToString(); 
                    wnd.scrollToButtom(); 
                }));
        }
        internal bool reportProgressOk(int cnt)
        {
            BackgroundWorker backgroundWorker = this;
            AllLogMsgsSend(cnt);
            Thread.Sleep(1);
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
