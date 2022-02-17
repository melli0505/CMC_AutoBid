using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using Windows.Storage;
using System.Threading.Tasks;

namespace SetUnitPriceByExcel
{
    class BidHandling
    {
        public static StorageFolder folder = ApplicationData.Current.LocalFolder; // 액세스 허용 구역 (User\AppData\Local\Packages\~~\LocalState) : 앱 임시 데이터
        public static string filename;

        public static async Task BidToXml()
        {
            //string nextName = file.DisplayName + ".zip";
            //await file.RenameAsync(nextName, NameCollisionOption.GenerateUniqueName);

            StorageFolder copiedFolder = await Data.folder.GetFolderAsync("Empty Bid"); // Empty Bid 폴더
            IReadOnlyList<StorageFile> bidFile = await copiedFolder.GetFilesAsync();
            string myfile = bidFile[0].Path;
            filename = bidFile[0].DisplayName;
            File.Move(myfile, Path.ChangeExtension(myfile, ".zip"));
            ZipFile.ExtractToDirectory(Path.Combine(copiedFolder.Path, bidFile[0].DisplayName + ".zip"), copiedFolder.Path);
            string[] files = Directory.GetFiles(copiedFolder.Path, "*.BID");
            string text = File.ReadAllText(files[0]); // 텍스트 읽기
            byte[] decodeValue = Convert.FromBase64String(text);  // base64 변환
            text = Encoding.UTF8.GetString(decodeValue);   // UTF-8로 디코딩
            File.WriteAllText(Path.Combine(Data.folder.Path, "OutputDataFromBID.xml"), text, Encoding.UTF8);

            //실내역 데이터 복사 및 단가 세팅 & 직공비 고정금액 비중 계산
            Setting.GetData();
        }

        public static async void XmlToBid()
        {
            StorageFolder copiedFolder = await Data.folder.CreateFolderAsync("Result Bid", CreationCollisionOption.ReplaceExisting); // Result Bid 폴더 생성

            string myfile = Path.Combine(Data.folder.Path, "Result_Xml.xml");
            byte[] bytes = File.ReadAllBytes(myfile);
            string encodeValue = Convert.ToBase64String(bytes);
            File.WriteAllText(Path.Combine(Data.folder.Path, "XmlToBID.BID"), encodeValue);
            string resultFileName = filename.Substring(0, 16) + ".zip";
            using (ZipArchive zip = ZipFile.Open(Path.Combine(Data.folder.Path, resultFileName), ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(Path.Combine(Data.folder.Path, "XmlToBID.BID"), "XmlToBid.BID");
            }
            File.Move(Path.Combine(Data.folder.Path, resultFileName), Path.ChangeExtension(Path.Combine(copiedFolder.Path, resultFileName), ".BID"));
        }
    }
}
