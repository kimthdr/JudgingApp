using JudgingApp;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JudgingApp
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // EPPlus 라이선스 컨텍스트 설정
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // 비상업적 용도로 설정

            Application.Run(new Form1());
        }
    }
}
