using System;
using System.IO;
using System.Linq;

// Excel
using OfficeOpenXml;

// Discord.NET features
using Discord;
using Discord.Commands;
using Discord.WebSocket;

// Async
using System.Threading.Tasks;

/* Info regarding EPPlus
 * - Cells row and column start from 1 not 0
 * 
 * 
 * Info regarding this controller
 * - Rows always start from 2 as Row 1 contains all the column data
 */

namespace WWBot.Modules.ComandsController
{
    // Guild wars weekly results excel
    [Group("gw")]
    public class GuildWarsController : BaseController
    {
        enum Column_Data
        {
            IGN = 1,
            Discord_Name,
            Discord_ID,
            WIN,
            LOSE,
            Total,
        }

        public static string GuildExcelPath = "Data\\GuildWars\\WW Guild Stats.xlsx";
        public static string TestExcelPath = "Data\\GuildWars\\WWBot Test Excel.xlsx";

        public static string EndIndicatorSplit = " | ";

        [Command("create"), RequireUserPermission(GuildPermission.Administrator)]
        public async Task CreateNewGuildExcelSheetAsync(string DDMMYY)
        {
            if (File.Exists(GuildExcelPath))
            {
                // Check if date created
                using (ExcelPackage excel = new ExcelPackage(new FileInfo(GuildExcelPath)))
                {
                    bool create = true;

                    // Check if the date exists as a worksheet in the excel file
                    foreach (var sheet in excel.Workbook.Worksheets)
                    {
                        if (sheet.Name == DDMMYY)
                        {
                            Reply($"\"{DDMMYY}\" already exists!");
                            create = false;
                            break;
                        }
                    }
                    if (create)
                    {
                        // Copy a template and add to the front
                        var newSheet = excel.Workbook.Worksheets.Copy("Template", DDMMYY);
                        excel.Workbook.Worksheets.MoveToStart(DDMMYY);
                        excel.Save();

                        Reply($"\"{DDMMYY}\" excel has been created!");
                    }
                }
            }
            /*var fileURL = GenerateFileURL(DDMMYY);
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
            }*/
        }

        // Submitting results using discord id or ign if given
        [Command("result"), RequireUserPermission(GuildPermission.ChangeNickname)]
        public async Task SubmitResultsAsync(int win, int lose, [Remainder]string ign = "")
        {
            var data = new Data(Context);
            bool done = false;
            bool hasIGN = false;

            // Excel
            using (ExcelPackage excel = new ExcelPackage(new FileInfo(GuildExcelPath)))
            {
                var ws = excel.Workbook.Worksheets[0];

                // Set up column number based on searching by ign or discord id
                int col = -1;
                if (ign != "")
                {
                    col = (int)Column_Data.IGN;
                    hasIGN = true;
                }
                else
                {
                    col = (int)Column_Data.Discord_ID;
                    ign = data.User.Id.ToString(); // Get user id since no ign is given
                }

                // Write using ign
                string cellText = "";
                var end = CalcWSEnd(ws);
                for (int i = 0; i < end; ++i)
                {
                    int row = i + 2; // Row starts from 2
                    cellText = FetchCellText(ws, row, col);
                    if (ign == cellText)
                    {
                        // Write data
                        ws.Cells[row, (int)Column_Data.WIN].Value = win.ToString();
                        ws.Cells[row, (int)Column_Data.LOSE].Value = lose.ToString();
                        done = true;
                        GenerateSubmitReply(win, lose, hasIGN ? ign : "");
                        break;
                    }
                }
                excel.Save();
            }
            
            // Send error message if player not found in excel
            if (!done)
            {
                string msg = hasIGN ? $"\"{ign}\" is not inside our list" : $"{data.User.Mention} is not inside our list";
                Reply(msg);
            }
        }

        /*// Submitting results using in game name
        [Command("result"), RequireUserPermission(GuildPermission.ChangeNickname)]
        public async Task SubmitResultsAsync(string ign, int win, int lose)
        {
            GenerateSubmitReply(win, lose, ign);
        }*/

        // Send file to chat for download
        [Command("file"), RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task SendExcelFileAsync()
        {
            if (File.Exists(GuildExcelPath))
            {
                var data = new Data(Context);
                await data.Message.Channel.SendFileAsync(GuildExcelPath);
            }
            else
            {
                Reply($"Excel file does not exist!");
            }
        }

        // Add user
        [Command("add"), RequireUserPermission(GuildPermission.ChangeNickname)]
        public async Task AddUserAsync([Remainder]string ign)
        {
            var data = new Data(Context);
            bool toAdd = true;
            using (ExcelPackage excel = new ExcelPackage(new FileInfo(GuildExcelPath)))
            {
                // Add to Template and latest file
                var templateWS = excel.Workbook.Worksheets[excel.Workbook.Worksheets.Count - 1];
                var latestWS = excel.Workbook.Worksheets[0];

                var end = CalcWSEnd(templateWS); // Use template worksheet as it is always the latest
                string cellText = "";
                for (int  i = 0; i < end; ++i)
                {
                    int row = i + 2;
                    cellText = FetchCellText(templateWS, row, (int)Column_Data.IGN);
                    if (cellText == ign)
                    {
                        toAdd = false;
                        break;
                    }
                }

                // Add if needed
                if (toAdd)
                {
                    AddUser(templateWS, end + 2, ign, data.User);
                    AddUser(latestWS, end + 2, ign, data.User);
                    Reply($"{data.User.Mention} Your user has been added!");
                }
                else
                {
                    Reply($"{data.User.Mention} Your user already exist!");
                }
                excel.Save();
            }
        }

        [Command("test")]
        public async Task TestAsync()
        {
            var data = new Data(Context);
            using (ExcelPackage excel = new ExcelPackage(new FileInfo(TestExcelPath)))
            {
                /*string reply = excel.Workbook.Worksheets[excel.Workbook.Worksheets.Count - 1].Cells[2,1].Text;
                var data = new Data(Context);
                //Reply(reply == data.User.Username ? "Unicode accepted" : "Unicode not accepted");
                Reply(reply);*/
                for (int i = 0; i < data.Guild.Users.Count; ++i)
                {
                    var user = data.Guild.Users.ElementAt(i);
                    var ws = excel.Workbook.Worksheets[excel.Workbook.Worksheets.Count - 1];
                    ws.Cells[i + 2, (int)Column_Data.Discord_Name].Value = user.Username;
                    ws.Cells[i + 2, (int)Column_Data.Discord_ID].Value = user.Id.ToString();
                }
                excel.Save();
                Reply("Done");
            }
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

        protected string FetchCellText(ExcelWorksheet ws, int row, int col)
        {
            return ws.Cells[row, col].Text;
        }

        protected int CalcWSEnd(ExcelWorksheet ws)
        {
            return Int32.Parse(FetchCellText(ws, 1, 1).Split(EndIndicatorSplit)[1]);
        }

        protected void AddUser(ExcelWorksheet ws, int row, string ign, SocketUser user)
        {
            ws.Cells[row, (int)Column_Data.IGN].Value = ign;
            ws.Cells[row, (int)Column_Data.Discord_Name].Value = user.Username;
            ws.Cells[row, (int)Column_Data.Discord_ID].Value = user.Id.ToString();

            // Update end number on ws
            UpdateEndNumber(ws, row - 1);
        }

        protected void UpdateEndNumber(ExcelWorksheet ws, int end)
        {
            ws.Cells[1, 1].Value = $"{Column_Data.IGN.ToString()}{EndIndicatorSplit}{end.ToString()}";
        }
    }
}
