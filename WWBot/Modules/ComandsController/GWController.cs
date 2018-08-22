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
        protected string TemplateURL = Program.GuildWarsResultsFolder + "\\" + "Template.xlsx";

        [Command("create"), RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task CreateGWExcel(string DDMMYY)
        {
            var fileURL = generateFileURL(DDMMYY);
            if (!File.Exists(fileURL))
            {
                // Copy template to create file if file does not exist
                File.Copy(TemplateURL, fileURL, false);

                using (ExcelPackage excel = new ExcelPackage(new FileInfo(fileURL)))
                {
                    excel.Workbook.Worksheets[0].Name = DDMMYY;
                    excel.Save();

                    Reply($"\"{DDMMYY}\" excel has been created!");
                }
            }
            else
            {
                Reply($"\"{DDMMYY}\" excel already exist!");
            }
        }

        protected string generateFileURL(string fileName)
        {
            return Program.GuildWarsResultsFolder + "\\" + fileName + ".xlsx";
        }
    }
}
