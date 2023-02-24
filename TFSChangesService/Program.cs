using System;
using System.Globalization;
using System.Management.Automation;
using System.Text;
using System.Xml.Linq;
using TFSChangesService;

//TODO排除的附檔名('.'分割取最後一個，該Line符合地跳過)
//相關設定值改成讀Config

Console.WriteLine("請輸入欲執行的服務");
Console.WriteLine("【1】= 產出TFS歷程檔案\n【2】= 依清單包版");

var actionService = Console.ReadLine();
if (string.IsNullOrEmpty(actionService))
{
    actionService = "2";
}

bool serviceResult = false;
string serviceMsg = "";

if (int.TryParse(actionService, NumberStyles.Integer, null, out int actionNum))
{
    Lib lib = new Lib();
    if (actionNum == 1)
    {
        //serviceResult = lib.GetHistoryFile();
    }
    else if (actionNum == 2)
    {
        serviceResult = lib.ReadyFiles(out serviceMsg);
    }
}
else
{
    Console.WriteLine("您輸入的並非數字，請重新啟動程式");
}

if (serviceResult)
{
    Console.WriteLine($"服務執行成功 【{serviceMsg}】");
}
else
{
    Console.WriteLine("服務執行失敗");
    Console.WriteLine($"錯誤訊息{serviceMsg}");
}

Console.WriteLine("執行結束");
Console.ReadKey();



