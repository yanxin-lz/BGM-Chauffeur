using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace 辅助类
{
    /// <summary>
    /// 完成文件浏览器与系统的交互操作
    /// </summary>
    public static class 文件定位器
    {
        [DllImport("shell32.dll")]
        static extern IntPtr ShellExecute(
        IntPtr hwnd,
        string lpOperation,
        string lpFile,
        string lpParameters,
        string lpDirectory,
        ShowCommands nShowCmd);
        private enum ShowCommands : int
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }
        /// <summary>
        /// 打开系统的文件资源管理器，然后定位到某个特定的文件或文件夹
        /// </summary>
        /// <param name="完整路径"></param>
        public static void 定位文件(string 完整路径)
        {
            if (Directory.Exists(完整路径))
            {
                ShellExecute(IntPtr.Zero, "open", "explorer.exe", 完整路径, "", ShowCommands.SW_NORMAL);
                return;
            }

            ShellExecute(IntPtr.Zero, "open", "explorer.exe", @"/e,/select," + 完整路径, "", ShowCommands.SW_NORMAL);

        }

        public static string 选择文件(string 初始目录, string 选择文件类型 = "图像文件(*.jpg)|*.jpg")
        {
            string 导出位置 = "";
            if (!Directory.Exists(初始目录)) { MessageBox.Show("找不到初始目录"); return ""; }
            Microsoft.Win32.OpenFileDialog 文件打开位置框 = new Microsoft.Win32.OpenFileDialog();
            文件打开位置框.InitialDirectory = 初始目录;
            文件打开位置框.Filter = 选择文件类型;
            文件打开位置框.AddExtension = true;
            文件打开位置框.DefaultExt = "jpg";
            文件打开位置框.ShowDialog();
            if (文件打开位置框.FileName == "" || 文件打开位置框.FileName == null) { return ""; }
            return 文件打开位置框.FileName;
        }

        public static string 保存文件(string 初始目录, string 可预览类型 = "图像文件(*.xlsx)|*.xlsx", string 保存文件类型 = "xlsx")
        {
            string 导出位置 = "";
            if (!Directory.Exists(初始目录)) { MessageBox.Show("找不到初始目录"); return ""; }
            Microsoft.Win32.SaveFileDialog 文件打开位置框 = new Microsoft.Win32.SaveFileDialog();
            文件打开位置框.InitialDirectory = 初始目录;
            文件打开位置框.Filter = 可预览类型;
            文件打开位置框.AddExtension = true;
            文件打开位置框.DefaultExt = 保存文件类型;
            文件打开位置框.ShowDialog();
            if (文件打开位置框.FileName == "" || 文件打开位置框.FileName == null) { return ""; }
            return 文件打开位置框.FileName;
        }

        public static string 选择目录(string 初始目录)
        {
            string 选择的目录 = "";
            if (!Directory.Exists(初始目录)) { MessageBox.Show("找不到初始目录"); return ""; }

            var folderDialog = new OpenFolderDialog();
            folderDialog.InitialDirectory = 初始目录;
            folderDialog.Title = "请选择一个目录";

            bool? result = folderDialog.ShowDialog();
            if (result == true)
            {
                选择的目录 = folderDialog.FolderName;
            }

            return 选择的目录;
        }

        /// <summary>
        /// 创建一个快捷方式
        /// </summary>
        /// <param name="lnkFilePath">快捷方式的完全限定路径。</param>
        /// <param name="workDir"></param>
        /// <param name="args">快捷方式启动程序时需要使用的参数。</param>
        /// <param name="targetPath"></param>
        public static void 创建新的快捷方式(string lnkFilePath, string targetPath, string workDir, string args = "")
        {
            var shellType = Type.GetTypeFromProgID("WScript.Shell");
            dynamic shell = Activator.CreateInstance(shellType);
            var shortcut = shell.CreateShortcut(lnkFilePath);
            shortcut.TargetPath = targetPath;
            shortcut.Arguments = args;
            shortcut.WorkingDirectory = workDir;
            shortcut.Save();
        }

        // 获取快捷方式目标路径
        public static readonly Guid CLSID_WshShell = new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8");
        public static string 获得快捷方式路径(string lnk)
        {
            if (System.IO.File.Exists(lnk))
            {
                dynamic objWshShell = null, objShortcut = null;
                try
                {
                    objWshShell = Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_WshShell));
                    objShortcut = objWshShell.CreateShortcut(lnk);
                    return objShortcut.TargetPath;
                }
                finally
                {
                    Marshal.ReleaseComObject(objShortcut);
                    Marshal.ReleaseComObject(objWshShell);
                }
            }
            return null;
        }

        /// <summary>
        /// 拼接程序目录下的文件Url
        /// </summary>
        /// <param name="相对路径字符串">以/开头的路径字符串</param>
        /// <returns></returns>
        public static string 拼接程序目录下的文件Url(string 相对路径字符串)
        {
            return Directory.GetCurrentDirectory() + 相对路径字符串;
        }

    }
}
