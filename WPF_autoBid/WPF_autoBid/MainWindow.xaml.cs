using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using SetUnitPriceByExcel;
using System.ComponentModel;
using System.Security.AccessControl;
using System.Security.Principal;

namespace CMC_AutoBid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // file 초기화 여부 확인
            InitializeComponent();
            if (Data.XlsFiles != null)
            {
                XlsList.Text = Data.XlsText;
            }
            if (Data.BidFile != null)
            {
                BIDList.Text = Data.BidText;
            }
            if (!Data.CanCovertFile && Data.IsConvert)
            {
                BidOpenFile.IsEnabled = false;
                XlsOpenFile.IsEnabled = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // 메세지 창
        static public void DisplayDialog(String dialog, String title)
        {
            MessageBox.Show(dialog, title, MessageBoxButton.OK, MessageBoxImage.Error);
            // ContentDialogResult result = await Dialog.ShowAsync();
        }

        // Bid File Open
        private async void BIDOpenClick(object sender, RoutedEventArgs e)
        {
            // 파일 탐색기 열기
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "BID Files (*.BID)|*.BID|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (openFileDialog.ShowDialog() == true)
            {
                // 복사 파일 저장 폴더 생성
                String copiedFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\EmptyBid";
                if (!Directory.Exists(copiedFolder)) // 이미 폴더가 있지 않은 경우
                {
                    // directory permission
                    Directory.CreateDirectory(copiedFolder);
                    DirectoryInfo info = new DirectoryInfo(copiedFolder);
                    info.Attributes &= ~FileAttributes.ReadOnly; // not read only 
                    // access control
                    DirectorySecurity security = info.GetAccessControl();
                    security.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    info.SetAccessControl(security);

                    FileStream file;

                    // 파일 복사
                    using (FileStream SourceStream = File.Open(openFileDialog.FileName, FileMode.Open))
                    {
                        using (FileStream DestinationStream = File.Create(copiedFolder + "\\" + System.IO.Path.GetFileName(openFileDialog.FileName))) 
                        {
                            await SourceStream.CopyToAsync(DestinationStream);
                            file = DestinationStream;
                            DisplayDialog(DestinationStream.Name, "확인");
                        }
                    }

                    Data.BidText = System.IO.Path.GetFileName(openFileDialog.FileName);
                    BIDList.Text = Data.BidText;
                    Data.BidFile = file;

                    Data.CanCovertFile = true;
                    Data.IsConvert = false;
                }
                else
                {
                    DisplayDialog("Empty BID 폴더가 이미 존재합니다.", openFileDialog.FileName);
                    Data.CanCovertFile = false;
                    Data.IsConvert = false;
                }

            }
            else
            {
                DisplayDialog("파일을 업로드 해주세요.", "Error");
                Data.XlsFiles = null;
            }
        }

        private async void XlsOpenClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Xls 파일(*.xls, *.xlsx)|*.xls;*.xlsx|All files (*.*)|*.*"; // TODO : 왜 안 먹냐?
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (openFileDialog.ShowDialog() == true)
            {
                String copiedFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Actual Xlsx";
                StringBuilder output = new StringBuilder();

                if (!Directory.Exists(copiedFolder)) // 이미 폴더가 있지 않은 경우
                {

                    // directory permission
                    Directory.CreateDirectory(copiedFolder);
                    DirectoryInfo info = new DirectoryInfo(copiedFolder);
                    info.Attributes &= ~FileAttributes.ReadOnly; // not read only 
                    
                    // access control
                    DirectorySecurity security = info.GetAccessControl();
                    security.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    info.SetAccessControl(security);
                    
                    int filenum = openFileDialog.FileNames.Length;
                    List<FileStream> files = new List<FileStream>(new FileStream[filenum]);
                    int count = 0;

                    foreach (string filepath in openFileDialog.FileNames)
                    {
                        String filename = System.IO.Path.GetFileName(filepath);
                        output.Append(filename + "\n");
                        // 파일 복사

                        using (FileStream SourceStream = File.Open(filepath, FileMode.Open))
                        {
                            using (FileStream DestinationStream = File.Create(copiedFolder + "\\" + filename))
                            {
                                await SourceStream.CopyToAsync(DestinationStream);
                                files[count] = DestinationStream;
                            }
                        }
                        count ++;
                    }

                    Data.XlsFiles = files;
                    Data.XlsText = output.ToString();
                    XlsList.Text = Data.XlsText;

                    Data.CanCovertFile = true;
                    Data.IsConvert = false;
                    count = 0;
                }
                else
                {
                    DisplayDialog("Actual Xlsx 폴더가 이미 존재합니다.", "Error");
                    Data.CanCovertFile = false;
                    Data.IsConvert = false;
                }
            }
            else
            {
                DisplayDialog("파일을 업로드 해주세요.", "Error");
                Data.XlsFiles = null;
            }
        }
    }


}
