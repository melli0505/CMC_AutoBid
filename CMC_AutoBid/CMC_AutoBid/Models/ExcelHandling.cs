using System;
using System.IO;
//공통 NPOI
using NPOI;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
//표준 xls 버전 excel 시트
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
//확장 xlsx 버전 excel 시트
using NPOI.XSSF;
using NPOI.XSSF.UserModel;
using System.Linq;

namespace SetUnitPriceByExcel
{
    class ExcelHandling
    {
        public static FileInfo[] files;    //작업폴더에 존재하는 모든 파일의 배열
        public static void ArrangeFiles()  //작업에 필요한 파일들 하나의 폴더에 정리
        {
            //1. xml 파일로 변환할 실내역 파일의 참조번호 입력 받음
            Console.Write("실내역 파일의 참조번호를 입력하세요(ex - 200124000) : ");
            string realFileNum = Console.ReadLine();
            Console.WriteLine("----------------------------------------------------------------------------");

            //2. 바탕화면에 작업 폴더 생성
            if (Directory.Exists(Data.work_path))
            {
                Console.WriteLine("이미 바탕화면에 작업을 위한 폴더가 존재합니다.(폴더명 : WORK DIRECTORY)");
            }
            else
            {
                Directory.CreateDirectory(Data.work_path);
                Console.WriteLine("바탕화면에 작업을 위한 폴더를 생성했습니다.(폴더명 : WORK DIRECTORY)");
            }
            Console.WriteLine("----------------------------------------------------------------------------");

            //3. 바탕화면에서 작업에 필요한 파일들을 추려내고 작업 폴더로 이동
            Console.WriteLine("작업에 필요한 파일들을 작업 폴더(WORK DIRECTORY)에 저장합니다.");

            //LINQ를 이용하여 작업파일의 이름들 List에 저장 or 객체들 저장 한 뒤, foreach로 하나씩 접근 
            var workFiles = from file in Directory.GetFiles(Data.desktop_path)
                            let info = new FileInfo(file)
                            where info.Name.StartsWith(realFileNum)
                            select info;
            //추려낸 파일들 작업 폴더로 이동
            foreach (var file in workFiles)
            {
                string sourceFile = file.FullName;
                string destinationFile = Path.Combine(Data.work_path, file.Name);
                File.Move(sourceFile, destinationFile);
            }
            //작업 폴더에 존재하는 모든 작업 파일들을 FileInfo 배열에 저장
            DirectoryInfo dir = new DirectoryInfo(Data.work_path);
            files = dir.GetFiles();
            Console.WriteLine("----------------------------------------------------------------------------");
        }
        // Sheet로 부터 Row를 취득, 생성하기
        public static IRow GetRow(ISheet sheet, int rownum)
        {
            var row = sheet.GetRow(rownum);
            if (row == null)
            {
                row = sheet.CreateRow(rownum);
            }
            return row;
        }
        // Row로 부터 Cell를 취득, 생성하기
        public static ICell GetCell(IRow row, int cellnum)
        {
            var cell = row.GetCell(cellnum);
            if (cell == null)
            {
                cell = row.CreateCell(cellnum);
            }
            return cell;
        }
        public static ICell GetCell(ISheet sheet, int rownum, int cellnum)
        {
            var row = GetRow(sheet, rownum);
            return GetCell(row, cellnum);
        }
        // Workbook 읽어드리기
        static public IWorkbook GetWorkbook(string filename, string version)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite))
            {
                //표준 xls 버전
                if (".xls".Equals(version))
                {
                    return new HSSFWorkbook(stream);
                }
                //확장 xlsx 버전
                else if (".xlsx".Equals(version))
                {
                    return new XSSFWorkbook(stream);
                }
                throw new NotSupportedException();
            }
        }

        //작업 후, workbook 저장
        static public void WriteExcel(IWorkbook workbook, string filepath)
        {
            using (var file = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite))
            {
                workbook.Write(file);
            }
        }
    }
}