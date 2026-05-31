using System.Diagnostics;
using System.Windows.Forms;
using VelocityAPI;
namespace Ligma
{
    public partial class Form1 : Form
    {
        VelAPI vel = new VelAPI();
        private System.Windows.Forms.Timer timer1;
        public Form1()
        {
            InitializeComponent();
            timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 1000;
            timer1.Tick += Timer_Tick;
            timer1.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            vel.StartCommunication();
            LoadScriptsIntoListBox();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            vel.StopCommunication();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            vel.Execute(richTextBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            Process[] robloxProcesses = Process.GetProcessesByName("RobloxPlayerBeta");

            if (robloxProcesses.Length > 0)
            {
                int pid = robloxProcesses[0].Id;

                if (vel.IsAttached(pid))
                {
                    label1.ForeColor = Color.LimeGreen;
                    label1.Text = "Online";
                }
                else
                {
                    label1.ForeColor = Color.Red;
                    label1.Text = "Offline";
                }
            }
            else
            {
                label1.ForeColor = Color.Red;
                label1.Text = "Offline";
            }
        }
        private void LoadScriptsIntoListBox()
        {
            string scriptsFolder = Path.Combine(Application.StartupPath, "Scripts");

            listBox1.Items.Clear();

            if (!Directory.Exists(scriptsFolder))
            {

                Directory.CreateDirectory(scriptsFolder);
                MessageBox.Show("Scripts folder was created at:\n" + scriptsFolder + "\n\nPut your script files there.", "Folder Created");
                return;
            }

            try
            {
                var files = Directory.GetFiles(scriptsFolder, "*.*")
                                     .Where(f => !string.IsNullOrEmpty(Path.GetExtension(f)))
                                     .Select(f => Path.GetFileName(f))
                                     .OrderBy(f => f);

                foreach (string file in files)
                {
                    listBox1.Items.Add(file);
                }

                if (listBox1.Items.Count == 0)
                {
                    MessageBox.Show("No files found in Scripts folder.\n\nPut some script files there!", "No Scripts");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading scripts:\n" + ex.Message);
            }
        }
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItem == null) return;

            string scriptsFolder = Path.Combine(Application.StartupPath, "Scripts");
            string fileName = listBox1.SelectedItem.ToString();
            string filePath = Path.Combine(scriptsFolder, fileName);

            if (!File.Exists(filePath))
            {
                MessageBox.Show("File not found:\n" + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string content = File.ReadAllText(filePath);
                richTextBox1.Text = content;
                richTextBox1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading file:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void button3_Click(object sender, EventArgs e)
        {
            int pid = 0;

            Process[] robloxProcesses = Process.GetProcessesByName("RobloxPlayerBeta");

            if (robloxProcesses.Length > 0)
            {
                Process roblox = robloxProcesses[0];
                pid = roblox.Id;


                await vel.Attach(pid);


            }
            else
            {
                MessageBox.Show($"injection failed PID: {pid}", "Fail");
                return;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
