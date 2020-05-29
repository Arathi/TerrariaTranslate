using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaTranslate
{
    public class TranslateReference
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public string FileName { get; set; }

        public string Parent { get; set; }

        public string Key { get; set; }

        public string English13 { get; set; }

        public string Chinese13 { get; set; }

        public string English14 { get; set; }

        public string Chinese14 { get; set; }

        public string GetText()
        {
            // 如果Chinese14有值，直接输出
            if (!string.IsNullOrEmpty(Chinese14))
            {
                return Chinese14;
            }

            // English14有值，Chinese14没有翻译
            if (!string.IsNullOrEmpty(English14) && string.IsNullOrEmpty(Chinese14))
            {
                // 检查Chinese13是否能用
                if (English13 == English14 && !string.IsNullOrEmpty(Chinese13))
                {
                    logger.Info($"{FileName}->{Parent}->{Key} 尚未翻译，使用1.3版的译文");
                    return Chinese13;
                }
                if (English13 != English14)
                {
                    logger.Info($"{FileName}->{Parent}->{Key} 尚未翻译，原文与1.3版有差异");
                }
            }

            return null;
        }
    }
}
