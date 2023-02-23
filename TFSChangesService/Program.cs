using System;
using System.Globalization;
using System.Management.Automation;
using System.Text;
using System.Xml.Linq;

//TODO排除的附檔名('.'分割取最後一個，該Line符合地跳過)


Console.WriteLine("請輸入欲執行的服務");
Console.WriteLine("【1】= 產出TFS歷程檔案\n【2】= 產出程式清單");
var actionService = Console.ReadLine();
if (int.TryParse(actionService,NumberStyles.Integer,null,out int actionNum))
{
    Lib lib = new Lib();
    if (actionNum == 1)
    {
        lib.GetHistoryFile();
    }
    else if (actionNum == 2)
    {
        lib.GetFileList();
    }
}
else
{
    Console.WriteLine("您輸入的並非數字，請重新啟動程式");
}

Console.WriteLine("執行結束");
Console.ReadKey();



public class Lib
{
    public bool GetHistoryFile()
    {
        var result = true;

        try
        {
            Console.WriteLine("請輸入<槽位>【ex : D:】");
            string disk = Console.ReadLine();
            if (string.IsNullOrEmpty(disk))
            {
                disk = "D:";
            }

            Console.WriteLine("請輸入<專案根目錄>【ex : D:\\VS專案\\TFS_ITS\\MPC\\MPC_Fortify】");
            string projectPath = Console.ReadLine();
            if (string.IsNullOrEmpty(projectPath))
            {
                projectPath = "D:\\VS專案\\TFS_ITS\\MPC\\MPC_Fortify";
            }

            Console.WriteLine("請輸入<日期起訖>【ex : 2022-7-1~D2023-2-21】");
            string tfsDateRegion = Console.ReadLine();
            if (string.IsNullOrEmpty(tfsDateRegion))
            {
                tfsDateRegion = "2022-7-1~D2023-2-21";
            }

            Console.WriteLine("請輸入<欲產出檔名之完整路徑>【ex : D:\\MPC-History.txt】");
            string fileName = Console.ReadLine();
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "D:\\MPC-History.txt";
            }

            var getFileResult = Lib.GetFile(disk, projectPath, tfsDateRegion, fileName);

            result = getFileResult;

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            result = false;
            return result;
        }
        
    }

    public bool GetFileList()
    {
        var result = true;
        try
        {
            //發現tf history > *.txt即使chcp 65001也是BIG5編碼，故讀取時需指定語系
            using (var sr = new StreamReader(@"MPC-History.txt",
                Encoding.GetEncoding(950)))
            {
                var report = new Dictionary<string, List<string>>();

                string line;
                int changeSetNo = 0;
                string author = "NA";
                DateTime date = DateTime.Today;
                string comment = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("變更集: "))
                        changeSetNo = int.Parse(line.Split(' ').Last());
                    else if (line.StartsWith("使用者: "))
                        author = line.Substring(5);
                    else if (line.StartsWith("日期: "))
                        date = DateTime.Parse(line.Substring(4), new CultureInfo("zh-TW"));
                    else if (line.StartsWith("註解:"))
                    {
                        comment = sr.ReadLine();
                    }
                    else if (line.StartsWith("項目:"))
                    {
                        while ((line = sr.ReadLine()).Contains("$"))
                        {
                            var i = line.IndexOf("$");
                            var act = line.Substring(0, i - 1).Trim().Replace(", ", "/");
                            var item = line.Substring(i).Trim();
                            if (!report.ContainsKey(item))
                                report.Add(item, new List<string>());
                            var x = new XElement("span") { Value = $"{changeSetNo} {act}" };
                            x.SetAttributeValue("title",
                                $"{author} {date:yyyy-MM-dd HH:mm} {comment}");
                            report[item].Add(x.ToString());
                        }
                    }
                }

                var sb = new StringBuilder();
                sb.AppendLine("<html>");
                sb.AppendLine(@"
<head><style>
table { font-size: 9pt; color: #444; width: 640px; border-collapse: collapse; }
td,th { padding: 3px; border: 1px solid gray; }
th { background-color: #679ac3; color: white; font-weight: normal; }
td span { cursor: pointer; color: blue; text-decoration: underline; margin-right: 6px; }
td span:hover { color: purple; font-weight: bold; }
tr:nth-child(even) { background-color: #edeff3; }
</style></head>
");
                sb.AppendLine("<body><table><thead>");
                sb.AppendLine("<tr><th>項目</th><th>異動記錄</th></tr>");
                sb.AppendLine("<tbody>");
                foreach (var item in report.Keys.OrderBy(o => o))
                {
                    sb.AppendLine($@"<tr>
<td>{item}</td>
<td>{string.Join(" ", report[item].ToArray())}</td>
</tr>");
                }
                sb.AppendLine("</tbody></table></body></html>");
                File.WriteAllText("E:\\TFS-Report.html", sb.ToString(), Encoding.UTF8);
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            result = false;
            return result;
        }
    }

    private static  bool GetFile(string disk, string projectPath, string dateRegion, string fileFullPath)
    {
        bool result = true;
        try
        {
            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.AddScript($"{disk}");
                powershell.AddScript(@$"cd {projectPath}");
                //powershell.AddScript("Set-Alias tf D:\\軟體安裝\\Microsoft Visual Studio\\2019\\Enterprise\\Common7\\IDE\\CommonExtensions\\Microsoft\\TeamFoundation\\Team Explorer\\TF.exe");
                //powershell.AddCommand(@"tf history *");
                powershell.AddScript(@$"tf vc history * /noprompt /recursive /format:detailed /v:D{dateRegion} > {fileFullPath}");
                //powershell.AddCommand("Get-Command");

                foreach (PSObject psResult in powershell.Invoke())
                {
                    Console.WriteLine(psResult);
                }
            }
            //System.Diagnostics.Process.Start("cmd.exe", "/k D:\\軟體安裝\\Microsoft Visual Studio\\2019\\Enterprise\\Common7\\Tools\\VsDevCmd.bat");

            Console.WriteLine("執行結束");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            result = false;
            return result;
        }
    }
}