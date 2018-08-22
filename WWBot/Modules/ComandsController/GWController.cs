using System;
using System.IO;

// Excel
using OfficeOpenXml;

// Discord.NET features
using Discord;
using Discord.Commands;
using Discord.WebSocket;

// Async
using System.Threading.Tasks;

namespace WWBot.Modules.ComandsController
{
    // Guild wars weekly results excel
    [Group("gw")]
    public class GuildWarsController : BaseController
    {
        [Command("create"), RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task CreateGWExcel(string DDMMYY)
        {
            var fileURL = generateFileURL(DDMMYY);
            if (!File.Exists(fileURL))
            {
                // Create file if file does not exist
                using (ExcelPackage excel = new ExcelPackage())
                {
                    excel.Workbook.Worksheets.Add(DDMMYY);

                    FileInfo excelFile = new FileInfo(fileURL);
                    excel.SaveAs(excelFile);

                    Reply($"\"{DDMMYY}\" excel has been created!");
                }
            }
            Reply($"\"{DDMMYY}\" excel already exist!");
        }

        protected string generateFileURL(string fileName)
        {
            return Program.GuildWarsResultsFolder + "\\" + fileName + ".xlsx";
        }
    }
}
