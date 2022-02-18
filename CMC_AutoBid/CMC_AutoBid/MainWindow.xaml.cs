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
            if(Data.XlsFiles != null)
            {
                XlsList.Text = Data.XlsText;
            }
            if(Data.BidFile != null)
            {
                BIDList.Text = Data.BidText;
            }
            if(!Data.CanCovertFile && Data.IsConvert)
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
            openFileDialog.Filter = "BID Files (*.BID)|*.BID|All files (*.*)|*.*" ;
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
                        using (FileStream DestinationStream = File.Create(copiedFolder)) // TODO : access denied 에러 수정
                        {
                            await SourceStream.CopyToAsync(DestinationStream);
                            file = DestinationStream;
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

        private void XlsOpenClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "XML 파일 (*.xls|*.xlsx)|*.xls, *.xlsx|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (openFileDialog.ShowDialog() == true)
            {
                DisplayDialog("test xlsx", "Test");
      //          foreach (string filename in openFileDialog.FileNames)
      //              XlsList.Items.Add(System.IO.Path.GetFileName(filename));
            }
        }
    }


}
