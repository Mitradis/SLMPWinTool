using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace SLMPWinTool
{
    public partial class FormMain : Form
    {
        static string folderSystem = Environment.GetFolderPath(Environment.SpecialFolder.System);
        static string folderWindows = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        string folderProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        List<string> exeList = new List<string>() { Path.Combine(folderSystem, "reg.exe"), Path.Combine(folderSystem, "taskkill.exe"), Path.Combine(folderWindows, "explorer.exe") };
        const int WS_MINIMIZEBOX = 0x20000;
        const int CS_DBLCLKS = 0x8;
        string tempImport = Path.Combine(Path.GetTempPath(), "_WinToolImport.reg");
        string tempExport = Path.Combine(Path.GetTempPath(), "_WinToolExport.reg");
        string sOn = "Включена";
        string sOff = "Выключена";
        string sHalf = "Частично";
        string sLaunch = "Запустить?";
        string sConfirm = "Подтвеждение";
        string sExplorer = "Выключить Проводник?";
        string sFolders = "Сбросить настройки отображения для всех папок?";
        string sMixer = "Сбросить настройки аудио микшера?";
        string sCompatibility = "Сбросить все параметры совместимости для всех приложений?";
        string eStart = "Не удалось запустить процесс: ";
        string eRegistry = "Ошибка доступа к реестру: ";
        string eRestart = "Требуется перезагрузка для запуска: ";
        string eWrite = "Не удалось записать файл: ";
        string eDelete = "Не удалось удалить файл: ";
        bool windows11 = !Environment.OSVersion.ToString().Remove(0, Environment.OSVersion.ToString().LastIndexOf(" ") + 1).StartsWith("10.0.19");
        Point lastLocation;

        public FormMain()
        {
            InitializeComponent();
            if (!File.Exists(Path.Combine(folderWindows, "ru-RU", "explorer.exe.mui")))
            {
                toEnglish();
                toolTip1.SetToolTip(buttonRefresh, "Refresh");
            }
            else
            {
                toolTip1.SetToolTip(buttonRefresh, "Обновить");
            }
            if (windows11)
            {
                Text = "Win 11 Tool";
                labelLogo.Image = Properties.Resources.MainLogo11;
                tabControl1.Controls.Remove(tabPage4);
            }
            refrashValues();
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                return cp;
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void refrashValues()
        {
            appx_label3.Text = getValue(3, @"SYSTEM\ControlSet001\Services\InstallService", "Start", "4") ? sOff : getValue(3, @"SYSTEM\ControlSet001\Services\mpssvc", "Start", "4") ? sHalf : sOn;
            setColor(contex_button1, 1, @"*\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}");
            setColor(contex_button2, 1, @"exefile\shellex\ContextMenuHandlers\PintoStartScreen");
            setColor(contex_button3, 1, @"Folder\shell\pintohome");
            setColor(contex_button4, 1, @"exefile\shellex\ContextMenuHandlers\Compatibility");
            setColor(contex_button5, 1, windows11 ? @"AllFilesystemObjects\shellex\ContextMenuHandlers\ModernSharing" : @"*\shellex\ContextMenuHandlers\ModernSharing");
            setColor(contex_button6, 1, @"*\shellex\ContextMenuHandlers\Sharing");
            setColor(contex_button6, 1, @"Directory\shellex\ContextMenuHandlers\Sharing");
            setColor(contex_button7, 1, @"Folder\shellex\ContextMenuHandlers\Library Location");
            setColor(contex_button8, 1, @"exefile\shellex\ContextMenuHandlers\StartMenuExt");
            if (!windows11)
            {
                setColor(thispc_button1, 3, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}");
                setColor(thispc_button2, 3, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{D3162B92-9365-467A-956B-92703ACA08AF}");
                setColor(thispc_button3, 3, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{0DB7E03F-FC29-4DC6-9020-FF41B59E513A}");
                setColor(thispc_button4, 3, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{088e3905-0323-4b02-9826-5d99428e115f}");
                setColor(thispc_button5, 3, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{24ad3ad4-a569-4530-98e1-ab02f9417aa8}");
                setColor(thispc_button6, 3, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{3dfdf296-dbec-4fb4-81d1-6a3438bcf4de}");
                setColor(thispc_button7, 3, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{f86fa3ab-70d2-4fc7-9c99-fcbf05467f3a}");
                contex_button8.Visible = File.Exists(Path.Combine(folderProgramFiles, "Classic Shell", "ClassicStartMenu.exe"));
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void appx_button1_Click(object sender, System.EventArgs e)
        {
            importRegistry(new List<string>() { @"[HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Services\mpssvc]", "\"Start\"=dword:00000002", @"[HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Services\InstallService]", "\"Start\"=dword:00000003" });
        }
        private void appx_button2_Click(object sender, System.EventArgs e)
        {
            importRegistry(new List<string>() { @"[HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Services\mpssvc]", "\"Start\"=dword:00000004", @"[HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Services\InstallService]", "\"Start\"=dword:00000004" });
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void firewall_button1_Click(object sender, EventArgs e)
        {
            importRegistry(new List<string>() { @"[HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Services\mpssvc]", "\"Start\"=dword:00000002" });
        }
        private void firewall_button2_Click(object sender, EventArgs e)
        {
            importRegistry(new List<string>() { @"[HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Services\mpssvc]", "\"Start\"=dword:00000004" });
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void contex_button1_Click(object sender, EventArgs e)
        {
            toggleButton((Button)sender, new List<string>() { @"[HKEY_CLASSES_ROOT\*\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", "@=\"Taskband Pin\"", @"[HKEY_CLASSES_ROOT\Application.Reference\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", @"[HKEY_CLASSES_ROOT\IE.AssocFile.WEBSITE\ShellEx\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", @"[HKEY_CLASSES_ROOT\Launcher.AllAppsDesktopApplication\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", "@=\"Taskband Pin\"", @"[HKEY_CLASSES_ROOT\Launcher.DesktopPackagedApplication\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", "@=\"Taskband Pin\"", @"[HKEY_CLASSES_ROOT\Launcher.ImmersiveApplication\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", "@=\"Taskband Pin\"", @"[HKEY_CLASSES_ROOT\Launcher.SystemSettings\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", "@=\"Taskband Pin\"", @"[HKEY_CLASSES_ROOT\Microsoft.Website\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", @"[HKEY_CLASSES_ROOT\MSILink\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", "@=\"Taskband Pin\"" }, new List<string>() { @"[-HKEY_CLASSES_ROOT\*\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", @"[-HKEY_CLASSES_ROOT\Application.Reference\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", @"[-HKEY_CLASSES_ROOT\IE.AssocFile.WEBSITE\ShellEx\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", @"[-HKEY_CLASSES_ROOT\Launcher.AllAppsDesktopApplication\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", @"[-HKEY_CLASSES_ROOT\Launcher.DesktopPackagedApplication\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", @"[-HKEY_CLASSES_ROOT\Launcher.ImmersiveApplication\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", @"[-HKEY_CLASSES_ROOT\Launcher.SystemSettings\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", @"[-HKEY_CLASSES_ROOT\Microsoft.Website\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]", @"[-HKEY_CLASSES_ROOT\MSILink\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}]" });
        }
        private void contex_button2_Click(object sender, EventArgs e)
        {
            toggleButton((Button)sender, new List<string>() { @"[HKEY_CLASSES_ROOT\exefile\shellex\ContextMenuHandlers\PintoStartScreen]", "@=\"{470C0EBD-5D73-4d58-9CED-E91E22E23282}\"", @"[HKEY_CLASSES_ROOT\Folder\shellex\ContextMenuHandlers\PintoStartScreen]", "@=\"{470C0EBD-5D73-4d58-9CED-E91E22E23282}\"", @"[HKEY_CLASSES_ROOT\Launcher.AllAppsDesktopApplication\shellex\ContextMenuHandlers\PintoStartScreen]", "@=\"{470C0EBD-5D73-4d58-9CED-E91E22E23282}\"", @"[HKEY_CLASSES_ROOT\Launcher.Computer\shellex\ContextMenuHandlers\PintoStartScreen]", "@=\"{470C0EBD-5D73-4d58-9CED-E91E22E23282}\"", @"[HKEY_CLASSES_ROOT\Launcher.DesktopPackagedApplication\shellex\ContextMenuHandlers\PintoStartScreen]", "@=\"{470C0EBD-5D73-4d58-9CED-E91E22E23282}\"", @"[HKEY_CLASSES_ROOT\Launcher.DualModeApplication\shellex\ContextMenuHandlers\PintoStartScreen]", "@=\"{470C0EBD-5D73-4d58-9CED-E91E22E23282}\"", @"[HKEY_CLASSES_ROOT\Launcher.ImmersiveApplication\shellex\ContextMenuHandlers\PintoStartScreen]", "@=\"{470C0EBD-5D73-4d58-9CED-E91E22E23282}\"", @"[HKEY_CLASSES_ROOT\Launcher.SystemSettings\shellex\ContextMenuHandlers\PintoStartScreen]", "@=\"{470C0EBD-5D73-4d58-9CED-E91E22E23282}\"", @"[HKEY_CLASSES_ROOT\Microsoft.Website\shellex\ContextMenuHandlers\PintoStartScreen]", "@=\"{470C0EBD-5D73-4d58-9CED-E91E22E23282}\"", @"[HKEY_CLASSES_ROOT\mscfile\shellex\ContextMenuHandlers\PintoStartScreen]", "@=\"{470C0EBD-5D73-4d58-9CED-E91E22E23282}\"" }, new List<string>() { @"[-HKEY_CLASSES_ROOT\exefile\shellex\ContextMenuHandlers\PintoStartScreen]", @"[-HKEY_CLASSES_ROOT\Folder\shellex\ContextMenuHandlers\PintoStartScreen]", @"[-HKEY_CLASSES_ROOT\Launcher.AllAppsDesktopApplication\shellex\ContextMenuHandlers\PintoStartScreen]", @"[-HKEY_CLASSES_ROOT\Launcher.Computer\shellex\ContextMenuHandlers\PintoStartScreen]", @"[-HKEY_CLASSES_ROOT\Launcher.DesktopPackagedApplication\shellex\ContextMenuHandlers\PintoStartScreen]", @"[-HKEY_CLASSES_ROOT\Launcher.DualModeApplication\shellex\ContextMenuHandlers\PintoStartScreen]", @"[-HKEY_CLASSES_ROOT\Launcher.ImmersiveApplication\shellex\ContextMenuHandlers\PintoStartScreen]", @"[-HKEY_CLASSES_ROOT\Launcher.SystemSettings\shellex\ContextMenuHandlers\PintoStartScreen]", @"[-HKEY_CLASSES_ROOT\Microsoft.Website\shellex\ContextMenuHandlers\PintoStartScreen]", @"[-HKEY_CLASSES_ROOT\mscfile\shellex\ContextMenuHandlers\PintoStartScreen]" });
        }
        private void contex_button3_Click(object sender, EventArgs e)
        {
            toggleButton((Button)sender, new List<string>() { @"[HKEY_CLASSES_ROOT\Folder\shell\pintohome]", "\"AppliesTo\"=\"System.ParsingName:<>\\\"::{679f85cb-0220-4080-b29b-5540cc05aab6}\\\" AND System.ParsingName:<>\\\"::{645FF040-5081-101B-9F08-00AA002F954E}\\\" AND System.IsFolder:=System.StructuredQueryType.Boolean#True\"", "\"MUIVerb\"=\"@shell32.dll,-51377\"", @"[HKEY_CLASSES_ROOT\Folder\shell\pintohome\command]", "\"DelegateExecute\"=\"{b455f46e-e4af-4035-b0a4-cf18d2f6f28e}\"" }, new List<string>() { @"[-HKEY_CLASSES_ROOT\Folder\shell\pintohome]" });
        }
        private void contex_button4_Click(object sender, EventArgs e)
        {
            toggleButton((Button)sender, new List<string>() { @"[HKEY_CLASSES_ROOT\batfile\shellex\ContextMenuHandlers\Compatibility]", "@=\"{1d27f844-3a1f-4410-85ac-14651078412d}\"", @"[HKEY_CLASSES_ROOT\cmdfile\shellex\ContextMenuHandlers\Compatibility]", "@=\"{1d27f844-3a1f-4410-85ac-14651078412d}\"", @"[HKEY_CLASSES_ROOT\exefile\shellex\ContextMenuHandlers\Compatibility]", "@=\"{1d27f844-3a1f-4410-85ac-14651078412d}\"", @"[HKEY_CLASSES_ROOT\Msi.Package\shellex\ContextMenuHandlers\Compatibility]", "@=\"{1d27f844-3a1f-4410-85ac-14651078412d}\"" }, new List<string>() { @"[-HKEY_CLASSES_ROOT\batfile\shellex\ContextMenuHandlers\Compatibility]", @"[-HKEY_CLASSES_ROOT\cmdfile\shellex\ContextMenuHandlers\Compatibility]", @"[-HKEY_CLASSES_ROOT\exefile\shellex\ContextMenuHandlers\Compatibility]", @"[-HKEY_CLASSES_ROOT\Msi.Package\shellex\ContextMenuHandlers\Compatibility]" });
        }
        private void contex_button5_Click(object sender, EventArgs e)
        {
            toggleButton((Button)sender, new List<string>() { windows11 ? @"[HKEY_CLASSES_ROOT\AllFilesystemObjects\shellex\ContextMenuHandlers\ModernSharing]" : @"[HKEY_CLASSES_ROOT\*\shellex\ContextMenuHandlers\ModernSharing]", "@=\"{e2bf9676-5f8f-435c-97eb-11607a5bedf7}\"", @"[HKEY_CLASSES_ROOT\AllFilesystemObjects\shellex\ContextMenuHandlers\SendTo]", "@=\"{7BA4C740-9E81-11CF-99D3-00AA004AE837}\"" }, new List<string>() { windows11 ? @"[-HKEY_CLASSES_ROOT\AllFilesystemObjects\shellex\ContextMenuHandlers\ModernSharing]" : @"[-HKEY_CLASSES_ROOT\*\shellex\ContextMenuHandlers\ModernSharing]", @"[-HKEY_CLASSES_ROOT\AllFilesystemObjects\shellex\ContextMenuHandlers\SendTo]" });
        }
        private void contex_button6_Click(object sender, EventArgs e)
        {
            toggleButton((Button)sender, new List<string>() { @"[HKEY_CLASSES_ROOT\*\shellex\ContextMenuHandlers\Sharing]", "@=\"{f81e9010-6ea4-11ce-a7ff-00aa003ca9f6}\"", @"[HKEY_CLASSES_ROOT\Directory\Background\shellex\ContextMenuHandlers\Sharing]", "@=\"{f81e9010-6ea4-11ce-a7ff-00aa003ca9f6}\"", @"[HKEY_CLASSES_ROOT\Directory\shellex\ContextMenuHandlers\Sharing]", "@=\"{f81e9010-6ea4-11ce-a7ff-00aa003ca9f6}\"", @"[HKEY_CLASSES_ROOT\Drive\shellex\ContextMenuHandlers\Sharing]", "@=\"{f81e9010-6ea4-11ce-a7ff-00aa003ca9f6}\"", @"[HKEY_CLASSES_ROOT\LibraryFolder\background\shellex\ContextMenuHandlers\Sharing]", "@=\"{f81e9010-6ea4-11ce-a7ff-00aa003ca9f6}\"", @"[HKEY_CLASSES_ROOT\UserLibraryFolder\shellex\ContextMenuHandlers\Sharing]", "@=\"{f81e9010-6ea4-11ce-a7ff-00aa003ca9f6}\"" }, new List<string>() { @"[-HKEY_CLASSES_ROOT\*\shellex\ContextMenuHandlers\Sharing]", @"[-HKEY_CLASSES_ROOT\Directory\background\shellex\ContextMenuHandlers\Sharing]", @"[-HKEY_CLASSES_ROOT\Directory\shellex\ContextMenuHandlers\Sharing]", @"[-HKEY_CLASSES_ROOT\Drive\shellex\ContextMenuHandlers\Sharing]", @"[-HKEY_CLASSES_ROOT\LibraryFolder\background\shellex\ContextMenuHandlers\Sharing]", @"[-HKEY_CLASSES_ROOT\UserLibraryFolder\shellex\ContextMenuHandlers\Sharing]" });
        }
        private void contex_button7_Click(object sender, EventArgs e)
        {
            toggleButton((Button)sender, new List<string>() { @"[HKEY_CLASSES_ROOT\Folder\shellex\ContextMenuHandlers\Library Location]", "@=\"{3dad6c5d-2167-4cae-9914-f99e41c12cfa}\"" }, new List<string>() { @"[-HKEY_CLASSES_ROOT\Folder\shellex\ContextMenuHandlers\Library Location]" });
        }
        private void contex_button8_Click(object sender, EventArgs e)
        {
            toggleButton((Button)sender, new List<string>() { @"[HKEY_CLASSES_ROOT\exefile\shellex\ContextMenuHandlers\StartMenuExt]", "@=\"{E595F05F-903F-4318-8B0A-7F633B520D2B}\"", @"[HKEY_CLASSES_ROOT\Folder\shellex\ContextMenuHandlers\StartMenuExt]", "@=\"{E595F05F-903F-4318-8B0A-7F633B520D2B}\"", @"[HKEY_CLASSES_ROOT\lnkfile\shellex\ContextMenuHandlers\StartMenuExt]", "@=\"{E595F05F-903F-4318-8B0A-7F633B520D2B}\"", @"[HKEY_CLASSES_ROOT\Launcher.SystemSettings\shellex\ContextMenuHandlers\StartMenuExt]", "@=\"{E595F05F-903F-4318-8B0A-7F633B520D2B}\"", @"[HKEY_CLASSES_ROOT\Launcher.ImmersiveApplication\shellex\ContextMenuHandlers\StartMenuExt]", "@=\"{E595F05F-903F-4318-8B0A-7F633B520D2B}\"" }, new List<string>() { @"[-HKEY_CLASSES_ROOT\exefile\shellex\ContextMenuHandlers\StartMenuExt]", @"[-HKEY_CLASSES_ROOT\Folder\shellex\ContextMenuHandlers\StartMenuExt]", @"[-HKEY_CLASSES_ROOT\lnkfile\shellex\ContextMenuHandlers\StartMenuExt]", @"[-HKEY_CLASSES_ROOT\Launcher.SystemSettings\shellex\ContextMenuHandlers\StartMenuExt]", @"[-HKEY_CLASSES_ROOT\Launcher.ImmersiveApplication\shellex\ContextMenuHandlers\StartMenuExt]" });
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void thispc_button1_Click(object sender, EventArgs e)
        {
            toggleButton((Button)sender, new List<string>() { @"[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}]", @"[HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}]" }, new List<string>() { @"[-HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}]", @"[-HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}]" });
        }
        private void thispc_button2_Click(object sender, EventArgs e)
        {
            toggleButton((Button)sender, new List<string>() { @"[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{D3162B92-9365-467A-956B-92703ACA08AF}]", @"[HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{D3162B92-9365-467A-956B-92703ACA08AF}]" }, new List<string>() { @"[-HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{D3162B92-9365-467A-956B-92703ACA08AF}]", @"[-HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{D3162B92-9365-467A-956B-92703ACA08AF}]" });
        }
        private void thispc_button3_Click(object sender, EventArgs e)
        {
            toggleButton((Button)sender, new List<string>() { @"[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{0DB7E03F-FC29-4DC6-9020-FF41B59E513A}]", @"[HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{0DB7E03F-FC29-4DC6-9020-FF41B59E513A}]" }, new List<string>() { @"[-HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{0DB7E03F-FC29-4DC6-9020-FF41B59E513A}]", @"[-HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{0DB7E03F-FC29-4DC6-9020-FF41B59E513A}]" });
        }
        private void thispc_button4_Click(object sender, EventArgs e)
        {
            toggleButton((Button)sender, new List<string>() { @"[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{088e3905-0323-4b02-9826-5d99428e115f}]", @"[HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{088e3905-0323-4b02-9826-5d99428e115f}]" }, new List<string>() { @"[-HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{088e3905-0323-4b02-9826-5d99428e115f}]", @"[-HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{088e3905-0323-4b02-9826-5d99428e115f}]" });
        }
        private void thispc_button5_Click(object sender, EventArgs e)
        {
            toggleButton((Button)sender, new List<string>() { @"[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{24ad3ad4-a569-4530-98e1-ab02f9417aa8}]", @"[HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{24ad3ad4-a569-4530-98e1-ab02f9417aa8}]" }, new List<string>() { @"[-HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{24ad3ad4-a569-4530-98e1-ab02f9417aa8}]", @"[-HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{24ad3ad4-a569-4530-98e1-ab02f9417aa8}]" });
        }
        private void thispc_button6_Click(object sender, EventArgs e)
        {
            toggleButton((Button)sender, new List<string>() { @"[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{3dfdf296-dbec-4fb4-81d1-6a3438bcf4de}]", @"[HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{3dfdf296-dbec-4fb4-81d1-6a3438bcf4de}]" }, new List<string>() { @"[-HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{3dfdf296-dbec-4fb4-81d1-6a3438bcf4de}]", @"[-HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{3dfdf296-dbec-4fb4-81d1-6a3438bcf4de}]" });
        }
        private void thispc_button7_Click(object sender, EventArgs e)
        {
            toggleButton((Button)sender, new List<string>() { @"[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{f86fa3ab-70d2-4fc7-9c99-fcbf05467f3a}]", @"[HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{f86fa3ab-70d2-4fc7-9c99-fcbf05467f3a}]" }, new List<string>() { @"[-HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{f86fa3ab-70d2-4fc7-9c99-fcbf05467f3a}]", @"[-HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{f86fa3ab-70d2-4fc7-9c99-fcbf05467f3a}]" });
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void services_button1_Click(object sender, EventArgs e)
        {
            if (dialogResult(sExplorer, sConfirm))
            {
                startProcess(1, "/f /im explorer.exe");
                if (dialogResult(sLaunch, sConfirm))
                {
                    startProcess(2, null);
                }
            }
        }
        private void services_button2_Click(object sender, EventArgs e)
        {
            if (dialogResult(sFolders, sConfirm))
            {
                startProcess(1, "/f /im explorer.exe");
                deleteFile(tempExport);
                startProcess(0, "export " + "\"" + @"HKEY_CURRENT_USER\SOFTWARE\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell" + "\" " + "\"" + tempExport + "\"");
                importRegistry(new List<string>() { @"[-HKEY_CURRENT_USER\SOFTWARE\Classes\Local Settings\Software\Microsoft\Windows\Shell]" });
                startProcess(0, "import \"" + tempExport + "\"");
                deleteFile(tempExport);
                startProcess(2, null);
            }
        }
        private void services_button3_Click(object sender, EventArgs e)
        {
            if (dialogResult(sMixer, sConfirm))
            {
                startProcess(1, "/f /im explorer.exe");
                importRegistry(new List<string>() { @"[-HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer\LowRegistry\Audio\PolicyConfig\PropertyStore]", @"[HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer\LowRegistry\Audio\PolicyConfig\PropertyStore]" });
                startProcess(2, null);
            }
        }
        private void services_button4_Click(object sender, EventArgs e)
        {
            if (dialogResult(sCompatibility, sConfirm))
            {
                importRegistry(new List<string>() { @"[-HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers]", @"[HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers]", @"[-HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers]", @"[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers]", @"[-HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers]", @"[HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers]", @"[-HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\DirectDraw\Compatibility]", @"[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\DirectDraw\Compatibility]", @"[-HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\DirectDraw\Compatibility]", @"[HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\DirectDraw\Compatibility]" });
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void importRegistry(List<string> list)
        {
            if (deleteFile(tempImport))
            {
                list.Insert(0, "Windows Registry Editor Version 5.00");
                if (writeToFile(tempImport, list))
                {
                    startProcess(0, "import \"" + tempImport + "\"");
                    deleteFile(tempImport);
                }
            }
            refrashValues();
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void startProcess(int index, string args)
        {
            Process process = new Process();
            process.StartInfo.FileName = exeList[index];
            process.StartInfo.Arguments = args;
            process.StartInfo.CreateNoWindow = true;
            try
            {
                process.Start();
                if (index != 4)
                {
                    process.WaitForExit();
                }
            }
            catch
            {
                MessageBox.Show(eStart + process.StartInfo.FileName + " " + args);
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private bool getValue(int index, string path, string key, string expect)
        {
            try
            {
                RegistryKey regkey = index == 1 ? Registry.ClassesRoot.OpenSubKey(path) : (index == 2 ? Registry.CurrentUser.OpenSubKey(path) : Registry.LocalMachine.OpenSubKey(path));
                if (regkey != null)
                {
                    if (key != null)
                    {
                        var val = regkey.GetValue(key);
                        regkey.Close();
                        if (val != null)
                        {
                            return val.ToString() == expect;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    regkey.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                MessageBox.Show(eRegistry + path);
                return false;
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private bool checkService(string name)
        {
            ServiceController sc = new ServiceController(name);
            bool resut = sc.Status == ServiceControllerStatus.Running || sc.Status == ServiceControllerStatus.StartPending;
            sc.Close();
            return resut;
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void startService(string name)
        {
            ServiceController sc = new ServiceController(name);
            if (sc.Status == ServiceControllerStatus.Stopped)
            {
                try
                {
                    sc.Start();
                }
                catch
                {
                    MessageBox.Show(eRestart + name);
                }
            }
            sc.Close();
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private bool writeToFile(string path, List<string> list)
        {
            try
            {
                File.WriteAllLines(path, list, new UTF8Encoding(false));
                return true;
            }
            catch
            {
                MessageBox.Show(eWrite + path);
                return false;
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private bool deleteFile(string path)
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch
            {
                MessageBox.Show(eDelete + path);
                return false;
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void toggleButton(Button button, List<string> on, List<string> off)
        {
            importRegistry((button.ForeColor == Color.Red) ? on : off);
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void setColor(Button button, int index, string path)
        {
            button.ForeColor = getValue(index, path, null, null) ? SystemColors.ControlText : Color.Red;
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private bool dialogResult(string message, string title)
        {
            DialogResult dialog = MessageBox.Show(message, title, MessageBoxButtons.YesNo);
            return dialog == DialogResult.Yes;
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            labelMain.Focus();
            refrashValues();
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void buttonClose_Click(object sender, EventArgs e)
        {
            labelMain.Focus();
            Application.Exit();
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            labelMain.Focus();
            WindowState = FormWindowState.Minimized;
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void mainLabels_MouseDown(object sender, MouseEventArgs e)
        {
            lastLocation = e.Location;
            ((Label)sender).MouseMove += mainLabels_MouseMove;
            ((Label)sender).MouseLeave += mainLabels_MouseLeave;
        }
        private void mainLabels_MouseUp(object sender, MouseEventArgs e)
        {
            ((Label)sender).MouseMove -= mainLabels_MouseMove;
            ((Label)sender).MouseLeave -= mainLabels_MouseLeave;
        }
        private void mainLabels_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).MouseMove -= mainLabels_MouseMove;
            ((Label)sender).MouseLeave -= mainLabels_MouseLeave;
        }
        private void mainLabels_MouseMove(object sender, MouseEventArgs e)
        {
            Location = new Point((Location.X - lastLocation.X) + e.X, (Location.Y - lastLocation.Y) + e.Y);
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void buttonClose_MouseEnter(object sender, EventArgs e)
        {
            buttonClose.BackgroundImage = Properties.Resources.buttonCloseGlow;
        }
        private void buttonClose_MouseLeave(object sender, EventArgs e)
        {
            buttonClose.BackgroundImage = Properties.Resources.buttonClose;
        }
        private void buttonMinimize_MouseEnter(object sender, EventArgs e)
        {
            buttonMinimize.BackgroundImage = Properties.Resources.buttonMinimizeGlow;
        }
        private void buttonMinimize_MouseLeave(object sender, EventArgs e)
        {
            buttonMinimize.BackgroundImage = Properties.Resources.buttonMinimize;
        }
        private void buttonRefresh_MouseEnter(object sender, EventArgs e)
        {
            buttonRefresh.BackgroundImage = Properties.Resources.buttonRefreshGlow;
        }
        private void buttonRefresh_MouseLeave(object sender, EventArgs e)
        {
            buttonRefresh.BackgroundImage = Properties.Resources.buttonRefresh;
        }
        // ------------------------------------------------ BORDER OF FUNCTION ------------------------------------------------ //
        private void toEnglish()
        {
            sOn = "Enabled";
            sOff = "Off";
            sHalf = "Partially";
            sLaunch = "Start?";
            sConfirm = "Confirmation";
            sExplorer = "Shutdown Explorer?";
            sFolders = "Reset display settings for all folders?";
            sMixer = "Reset audio mixer settings?";
            sCompatibility = "Reset all compatibility settings for all apps?";
            eStart = "Failed to start process: ";
            eRegistry = "Error accessing registry: ";
            eRestart = "Restart required to start: ";
            eWrite = "Failed to write file: ";
            eDelete = "Failed to delete file: ";
            appx_button1.Text = "Turn on";
            appx_button2.Text = "Turn off";
            firewall_button1.Text = "Turn on";
            firewall_button2.Text = "Turn off";
            contex_button1.Text = "Pin to taskbar";
            contex_button2.Text = "Pin to home screen";
            contex_button3.Text = "Pin to Quick Access Toolbar";
            contex_button4.Text = "Fix compatibility issues";
            contex_button5.Text = "Send";
            contex_button6.Text = "Grant access to";
            contex_button7.Text = "Add to Library";
            contex_button8.Text = "Pin for Classic Shell";
            thispc_button1.Text = "Desktop";
            thispc_button2.Text = "Documents";
            thispc_button3.Text = "3D objects";
            thispc_button4.Text = "Downloads";
            thispc_button5.Text = "Images";
            thispc_button6.Text = "Music";
            thispc_button7.Text = "Video";
            services_button1.Text = "Restart explorer";
            services_button2.Text = "Reset folders";
            services_button3.Text = "Reset mixer";
            services_button4.Text = "Reset compatibility";
            appx_label1.Text = "AppX installation support";
            appx_label2.Text = "Current state:";
            appx_label4.Text = "Changes require a reboot.";
            contex_label1.Text = "Managing Standard Context Menu Items";
            thispc_label1.Text = "Managing elements in" + Environment.NewLine + "This computer";
            services_label1.Text = "Various service commands";
            firewall_label4.Text = "Changes require a reboot.";
            firewall_label2.Text = "Current state:";
            firewall_label1.Text = "Firefall service";
            tabPage2.Text = "Firefall";
            tabPage3.Text = "Event Log";
            tabPage3.Text = "Context menu";
            tabPage4.Text = "This computer";
            tabPage5.Text = "Service";
        }
    }
}
