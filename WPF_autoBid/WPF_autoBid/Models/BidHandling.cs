using System;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace SetUnitPriceByExcel
{
    class BidHandling
    {
        static string filename;
        public static void BidToXml()
        {
            Console.Write("BID 파일 이름을 입력하세요 : ");
            filename = Console.ReadLine();
            string myfile = Path.Combine(Data.desktop_path, filename + ".BID");
            File.Move(myfile, Path.ChangeExtension(myfile, ".zip"));
            ZipFile.ExtractToDirectory(Path.Combine(Data.desktop_path, filename + ".zip"), Data.desktop_path);
            string[] files = Directory.GetFiles(Data.desktop_path, "*.BID");
            string text = File.ReadAllText(files[0]); // 텍스트 읽기
            byte[] decodeValue = Convert.FromBase64String(text);  // base64 변환
            text = Encoding.UTF8.GetString(decodeValue);   // UTF-8로 디코딩
            System.IO.File.WriteAllText(Path.Combine(Data.work_path, "OutputDataFromBID.xml"), text, Encoding.UTF8);
        }

        public static void XmlToBid()
        {
            string myfile = Path.Combine(Data.work_path, "Result_Xml.xml");
            byte[] bytes = File.ReadAllBytes(myfile);
            string encodeValue = Convert.ToBase64String(bytes);
            File.WriteAllText(Path.Combine(Data.work_path, "XmlToBID.BID"), encodeValue);
            string resultFileName = filename.Substring(0,16) + ".zip";
            using (ZipArchive zip = ZipFile.Open(Path.Combine(Data.work_path, resultFileName), ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(Path.Combine(Data.work_path, "XmlToBID.BID"), "XmlToBid.BID");
            }
            File.Move(Path.Combine(Data.work_path, resultFileName), Path.ChangeExtension(Path.Combine(Data.work_path, resultFileName), ".BID"));
        }

        public static void RemoveFiles()    //작업 종료 후, 필요없는 파일들 제거
        {
            //바탕화면에 위치한 zip, BID 파일 삭제
            File.Delete(Directory.GetFiles(Data.desktop_path, "*.BID")[0]);
            File.Delete(Directory.GetFiles(Data.desktop_path, "*.zip")[0]);
            string[] xlsFiles = Directory.GetFiles(Data.desktop_path, "*.xls");
            foreach(var file in xlsFiles)
            {
                File.Delete(file);
            }
            //작업폴더에 위치한 필요없는 파일들 제거
            File.Delete(Path.Combine(Data.work_path, "OutputDataFromBID.xml"));
            File.Delete(Path.Combine(Data.work_path, "Setting_Xml.xml"));
            File.Delete(Path.Combine(Data.work_path, "Result_Xml.xml"));
            File.Delete(Path.Combine(Data.work_path, "XmlToBID.BID"));
            File.Delete(Path.Combine(Data.work_path, "원가계산서.xlsx"));
        }
    }
}
