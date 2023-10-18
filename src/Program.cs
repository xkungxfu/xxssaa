using YueHuan;

namespace YueHuan
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //ApplicationConfiguration.Initialize();
            //Application.Run(new MainForm());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ��õ�ǰ��¼��Windows�û���ʾ
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new(identity);
            // �жϵ�ǰ��¼�û��Ƿ�Ϊ����Ա
            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                //����ǹ���Ա����ֱ������
                Application.Run(new MainForm());
            }
            else
            {
                // ������������
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Application.ExecutablePath;
                // ������������,ȷ���Թ���Ա�������
                startInfo.Verb = "runas";
                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                }
                catch
                {
                    return;
                }
                //�˳�
                Application.Exit();
            }
        }
    }
}