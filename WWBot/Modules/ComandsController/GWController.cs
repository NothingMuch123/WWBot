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
        public async Task CreateGWExcelAsync(string DDMMYY)
        {
            var fileURL = GenerateFileURL(DDMMYY);
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

        // Submitting results using discord name
        [Command("result"), RequireUserPermission(GuildPermission.ChangeNickname)]
        public async Task SubmitResultsAsync(int win, int lose)
        {
            GenerateSubmitReply(win, lose);
        }

        // Submitting results using in game name
        [Command("result"), RequireUserPermission(GuildPermission.ChangeNickname)]
        public async Task SubmitResultsAsync(string ign, int win, int lose)
        {
            GenerateSubmitReply(win, lose, ign);
        }

        [Command("file"), RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task SendExcelFileAsync(string DDMMYY)
        {
            var fileURL = GenerateFileURL(DDMMYY);
            if (File.Exists(fileURL))
            {
                var data = new Data(Context);
                await data.Message.Channel.SendFileAsync(fileURL);
            }
            else
            {
                Reply($"\"{DDMMYY}\" excel file does not exist!");
            }
        }

        protected string GenerateFileURL(string fileName)
        {
            return Program.GuildWarsResultsFolder + "\\" + fileName + ".xlsx";
        }

        protected async Task GenerateSubmitReply(int win, int lose, string ign = "")
        {
            var data = new Data(Context);

            // Congratulations for win > lose           (E.g 4-0 || 3-1 || 3-2)
            // Words of encouragement for lose >= win   (E.g 2-4 || 1-3 || 2-2)
            string message = win > lose ? "Keep up the good work" : "Better luck next time";

            if (ign == "")
            {
                // No ign, uses discord
                Reply($"{message} {data.User.Mention}");
            }
            else
            {
                // Uses ign
                Reply($"{message} {ign}");
            }
        }
    }
}
