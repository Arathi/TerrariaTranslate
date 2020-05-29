using NLog;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaTranslate
{
    public class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                Console.WriteLine("参数错误，请传入Excel文件路径");
                return;
            }

            string outputPath = null;
            if (args.Length >= 2)
            {
                outputPath = args[1];
            }

            string inputFilePath = args[0];
            FileInfo inputFileInfo = new FileInfo(inputFilePath);
            if (!inputFileInfo.Exists)
            {
                Console.WriteLine("输入文件不存在");
                return;
            }

            IWorkbook wb = new XSSFWorkbook(inputFileInfo);
            ISheet sheet = wb.GetSheet("convertcsv");
            if (sheet == null)
            {
                Console.WriteLine("找不到convertcsv工作表");
                return;
            }

            IDictionary<string, ResourceFile> resFiles = new Dictionary<string, ResourceFile>();
            resFiles["Game"] = new ResourceFile("Game");
            resFiles["Items"] = new ResourceFile("Items");
            resFiles["Legacy"] = new ResourceFile("Legacy");
            resFiles["Main"] = new ResourceFile("Main");
            resFiles["NPCs"] = new ResourceFile("NPCs");
            resFiles["Projectiles"] = new ResourceFile("Projectiles");
            resFiles["Town"] = new ResourceFile("Town");

            for (int rowNum = 1; rowNum <= sheet.LastRowNum; rowNum++)
            {
                IRow row = sheet.GetRow(rowNum);
                if (row == null)
                {
                    logger.Warn($"第{rowNum}行为空");
                    continue;
                }

                TranslateReference transref = new TranslateReference();

                string fileName = row.GetCell(0).StringCellValue;
                transref.FileName = fileName;
                transref.Parent = row.GetCell(1).StringCellValue;
                ICell keyCell = row.GetCell(2);
                if (keyCell.CellType == CellType.Numeric)
                {
                    transref.Key = keyCell.NumericCellValue.ToString();
                }
                else
                {
                    transref.Key = keyCell.StringCellValue;
                }

                try
                {
                    transref.English13 = row.GetCell(3).StringCellValue;
                    transref.Chinese13 = row.GetCell(4).StringCellValue;
                    transref.English14 = row.GetCell(5).StringCellValue;
                    transref.Chinese14 = row.GetCell(6).StringCellValue;
                }
                catch (Exception ex)
                {
                    logger.Warn($"第{rowNum}行数据异常：{ex.Message}");
                    Console.WriteLine($"第{rowNum}行数据异常");
                }

                resFiles[fileName].References.Add(transref);
            }

            foreach (var file in resFiles.Values)
            {
                file.Save(outputPath);
            }

            Console.WriteLine("转换完成");
        }
    }
}
