using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace TFSChangesService
{
    public class Lib
    {


        public bool ReadyFiles(out string msg)
        {
            var result = true;
            msg = "開始執行";
            try
            {
                #region LoadConfig

                var configs = File.ReadAllLines(Path.Combine(@"D:\VS專案\小工具\TFSChangesService", "Config.txt"));
                Dictionary<string, string> configDics = new Dictionary<string, string>();

                foreach (var config in configs)
                {
                    configDics.Add( config.Split("=").First(),config.Split("=").Last());
                }

                #endregion

                #region 使用者輸入

                //Console.WriteLine("請輸入來源根目錄");
                var sourceRoot = configDics.Where(x=>x.Key=="來源根目錄").FirstOrDefault().Value;
                if (string.IsNullOrEmpty(sourceRoot))
                {
                    sourceRoot = @"D:\VS專案\TFS_ITS";
                }

                //Console.WriteLine("請輸入目的地根目錄");
                var targetRoot = Path.Combine( configDics.Where(x => x.Key == "目的地根目錄").FirstOrDefault().Value,DateTime.Now.ToString("yyyyMMddmmssffff"));
                if (string.IsNullOrEmpty(targetRoot))
                {
                    targetRoot = @$"D:\ITSOWER\包版\{DateTime.Now.ToString("yyyyMMddmmssffff")}";
                }

                //Console.WriteLine("請輸入程式清單根目錄");
                var filetxtRoot = Path.Combine( configDics.Where(x => x.Key == "程式清單根目錄").FirstOrDefault().Value, configDics.Where(x => x.Key == "程式清單資料夾").FirstOrDefault().Value);
                if (string.IsNullOrEmpty(filetxtRoot))
                {
                    filetxtRoot = @$"D:\OneDrive\元威資訊科技股份有限公司\BOP_板信商業銀行 - BOP_多元支付代收整合平台 - BOP_多元支付代收整合平台\19_歷次更版程式及文件\2022新版Source Code\{DateTime.Now.ToString("yyyyMMdd")}";
                }

                #endregion

                var txtPath = Path.Combine(filetxtRoot, "程式清單.txt");
                msg = "開始讀取程式清單";
                var fileLists = File.ReadAllLines(txtPath);

                msg = "開始程式包版作業";
                foreach (var filelist in fileLists)
                {
                    Console.WriteLine(filelist);
                    var sourceFolderPath = Path.Combine(sourceRoot, filelist);
                    var targetFolderPath = Path.Combine(targetRoot, filelist.Replace(filelist.Split('\\').Last(), ""));
                    var targetFullPath = Path.Combine(targetRoot, filelist);
                    if (Directory.Exists(targetFolderPath) == false)
                    {
                        Directory.CreateDirectory(targetFolderPath);
                    }
                    File.Copy(sourceFolderPath, targetFullPath);
                }

                msg = "包版作業執行結束";
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result = false;
                msg= ex.Message;
                return result;
            }
        }

        #region 有問題未解，暫時失敗

        //public bool GetHistoryFile()
        //{
        //    var result = true;

        //    try
        //    {
        //        Console.WriteLine("請輸入<槽位>【ex : D:】");
        //        string disk = Console.ReadLine();
        //        if (string.IsNullOrEmpty(disk))
        //        {
        //            disk = "D:";
        //        }

        //        Console.WriteLine("請輸入<專案根目錄>【ex : D:\\VS專案\\TFS_ITS\\MPC\\MPC_Fortify】");
        //        string projectPath = Console.ReadLine();
        //        if (string.IsNullOrEmpty(projectPath))
        //        {
        //            projectPath = "D:\\VS專案\\TFS_ITS\\MPC\\MPC_Fortify";
        //        }

        //        Console.WriteLine("請輸入<日期起訖>【ex : 2022-7-1~D2023-2-21】");
        //        string tfsDateRegion = Console.ReadLine();
        //        if (string.IsNullOrEmpty(tfsDateRegion))
        //        {
        //            tfsDateRegion = "2022-7-1~D2023-2-21";
        //        }

        //        Console.WriteLine("請輸入<欲產出檔名之完整路徑>【ex : D:\\MPC-History.txt】");
        //        string fileName = Console.ReadLine();
        //        if (string.IsNullOrEmpty(fileName))
        //        {
        //            fileName = "D:\\MPC-History.txt";
        //        }

        //        var getFileResult = Lib.GetFile(disk, projectPath, tfsDateRegion, fileName);

        //        result = getFileResult;

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        result = false;
        //        return result;
        //    }

        //}

        //private static bool GetFile(string disk, string projectPath, string dateRegion, string fileFullPath)
        //{
        //    bool result = true;
        //    try
        //    {
        //        using (PowerShell powershell = PowerShell.Create())
        //        {
        //            powershell.AddScript($"{disk}");
        //            powershell.AddScript(@$"cd {projectPath}");
        //            //powershell.AddScript("Set-Alias tf D:\\軟體安裝\\Microsoft Visual Studio\\2019\\Enterprise\\Common7\\IDE\\CommonExtensions\\Microsoft\\TeamFoundation\\Team Explorer\\TF.exe");
        //            //powershell.AddCommand(@"tf history *");
        //            powershell.AddScript(@$"tf vc history * /noprompt /recursive /format:detailed /v:D{dateRegion} > {fileFullPath}");
        //            //powershell.AddCommand("Get-Command");

        //            foreach (PSObject psResult in powershell.Invoke())
        //            {
        //                Console.WriteLine(psResult);
        //            }
        //        }
        //        //System.Diagnostics.Process.Start("cmd.exe", "/k D:\\軟體安裝\\Microsoft Visual Studio\\2019\\Enterprise\\Common7\\Tools\\VsDevCmd.bat");

        //        Console.WriteLine("執行結束");
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        result = false;
        //        return result;
        //    }
        //}

        #endregion

    }
}
