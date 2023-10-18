using System.Diagnostics;
using System.Reflection;

namespace YueHuan
{
    public partial class MainForm : Form
    {

        private readonly WeChatWin weChatWin;
        public MainForm()
        {
            InitializeComponent();
            weChatWin = new WeChatWin();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await weChatWin.InitializeAsync();

            DisplayAssemblyVersion();   // ��ȡ���򼯰汾��
            DisplayFileVersion();       // ��ȡ��ǰ���򼯰汾��
            DisplayProjectVersion();    // �滻�汾��
            SetLabels();                // ���ó������
            SetLoggerListBox();         // ������־д��
            SetDownloadLinks();         // ��������·��
        }

        /// <summary>
        /// ��ȡ���򼯵İ汾��
        /// </summary>
        private static void DisplayAssemblyVersion()
        {
            // ��ȡ��ǰ����ִ�еĳ���
            Assembly? assembly = Assembly.GetEntryAssembly();

            // �ӳ����л�ȡ��汾��Ϣ
            Version? assemblyVersion = assembly?.GetName().Version;

            // ����ȡ���İ汾��Ϣ���������̨������汾��ϢΪnull�������"δ֪"
            Console.WriteLine("���򼯰汾�ţ� " + (assemblyVersion?.ToString() ?? "δ֪"));

        }

        /// <summary>
        /// ��ȡ��ǰ����汾��
        /// </summary>
        private async void DisplayFileVersion()
        {
            // ��ȡ��ǰ���̵�·��
            string? filePath = Environment.ProcessPath;

            // ��ȡ�ļ��İ汾��Ϣ
            FileVersionInfo? fileVersion = FileVersionInfo.GetVersionInfo(filePath!);

            // ���ļ��汾��Ϣ����ȡ���汾�ŵĸ������֣�������һ���µ�Version����
            Version? fileVer = new(fileVersion.FileVersion!);

            // ���汾�ŵĸ�������ƴ�ӳ�һ���ַ���
            string version = $"{fileVer.Major}.{fileVer.Minor}.{fileVer.Build}.{fileVer.Revision}";

            var githubUpdate = new GithubUpdate("redsonw", "WeChatMO");
            var latestRelease = await githubUpdate.GetLatestRelease();
            string newversion = latestRelease.TagName.Replace("v", "");
            string exeDownloadUrl = await githubUpdate.GetExeDownloadUrl();
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "new" + GetFileNameFromUrl(exeDownloadUrl);
            IProgress<double> progress = new Progress<double>(percent =>
            {
                Console.WriteLine($"Downloaded: {percent}%");
            });

            if (newversion != version)
            {
                bool result = MessageBox.Show("�����µİ汾������£�����", "�汾����", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;

                if (result)
                {
                    double finalProgress = await githubUpdate.DownloadExe(fileName, progress);
                    LoggerListBox.Items.Add($"�Ѿ����أ�{finalProgress}%");
                }
            }
        }

        static string GetFileNameFromUrl(string url)
        {
            string fileName = Path.GetFileName(new Uri(url).LocalPath);
            return fileName;
        }

        private static void DisplayProjectVersion()
        {
            // ������Ҫ�滻Ϊʵ�ʵ���Ŀ�汾�Ż�ȡ�߼�
            Version projectVersion = new("1.0.0.0"); // ���磺����汾���� 1.0.0.0
            Console.WriteLine("��Ŀ�汾��: " + projectVersion);
        }

        /// <summary>
        /// ���ó������
        /// </summary>
        private void SetLabels()
        {
            // ��ȡ��ǰ���̵�·��
            string? infoPath = Environment.ProcessPath;
            // ����·������һ��FileInfo����
            FileInfo fileInfo = new(infoPath!);

            // ���ô���ı��⣬��ʾ�汾��Ϣ�ͽ��΢�Ŷ࿪���Ƶ���Ϣ
            this.Text = $" ���΢�Ŷ࿪���� v{Assembly.GetEntryAssembly()?.GetName()?.Version}";

            // ���ð汾��ǩ���ı�����ʾ����֧�ֵİ汾��Ϣ
            VersionLabel.Text = $" {weChatWin.WechatVer.Last()}"; // ����֧�ְ汾

            // ���ò�����Ϣ��ǩ���ı�����ʾ��������Ϣ
            PatchInfoLabel.Text = " ���΢�Ŷ࿪���Ʋ���";

            // ���÷�����ǩ���ı�����ʾ�ļ������д��ʱ��
            ReleaseLabel.Text = $" {fileInfo.LastWriteTime:yyyy-MM-dd}";
        }

        /// <summary>
        /// д����־��Ϣ
        /// </summary>
        private void SetLoggerListBox()
        {
            if (!string.IsNullOrEmpty(weChatWin.WeChatVersion))
            {
                LoggerListBox.Items.Add($"���߰汾��{GetFileVersion()}");
                LoggerListBox.Items.Add($"��ǰ�汾��{weChatWin.WeChatVersion}");
                LoggerListBox.Items.Add($"��װ·����{weChatWin.WeChatPath}");
            }
            else
            {
                LoggerListBox.Items.Add($"���ĵ��Ի�δ��װPC ΢�ţ��뵽�����������°汾");
                PatchesButton.Enabled = false;
            }
        }

        /// <summary>
        /// ��ȡ��ǰ�����ļ��İ汾��Ϣ
        /// </summary>
        /// <returns></returns>
        private static string GetFileVersion()
        {
            // ��ȡ��ǰ�����·��
            string? filePath = Environment.ProcessPath;

            // ʹ��FileVersionInfo���ȡ�ļ��İ汾��Ϣ
            FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(filePath!);

            // ���汾��Ϣ��װ��Version������
            Version fileVer = new(fileVersion.FileVersion!);

            // ��ʽ���汾��Ϣ�������ַ�����ʽ
            return $"{fileVer.Major}.{fileVer.Minor}.{fileVer.Build}.{fileVer.Revision}";
        }

        /// <summary>
        /// ����΢������·��
        /// </summary>
        private void SetDownloadLinks()
        {
            DownloadLinkLabel.Text = " [ --- ���԰� - ��ʽ�� - ������־ --- ]";
            DownloadLinkLabel.Links.Add(new LinkLabel.Link(7, 3, "https://www.redsonw.com/wechatwinbeta.html"));
            DownloadLinkLabel.Links.Add(new LinkLabel.Link(13, 3, "https://dldir1.qq.com/weixin/Windows/WeChatSetup.exe"));
            DownloadLinkLabel.Links.Add(new LinkLabel.Link(19, 4, "https://www.redsonw.com/wechatmo.html"));
        }


        private void DownloadLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Link!.LinkData is string url)
            {
                WeChatWin.OpenURL(url);
            }
        }

        private void PatchesButton_Click(object sender, EventArgs e)
        {
            LimitRemover remover = new(weChatWin, LoggerListBox);
            remover.RemoveLimit();
        }
    }
}