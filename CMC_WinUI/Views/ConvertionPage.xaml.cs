using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using SetUnitPriceByExcel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Text.Core;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace CMC_WinUI.Views
{
    public sealed partial class ConvertionPage : Page, INotifyPropertyChanged
    {
        public ConvertionPage()
        {
            InitializeComponent();
            if(Data.XlsFiles != null)
            {
                XlsList.Text = Data.XlsText;
            }
            if (Data.BidFile != null)
            {
                BidList.Text = Data.BidText;
            }
            if(!Data.CanCovertFile && Data.IsConvert)
            {
                XlsUploadButton.IsEnabled = false;
                BidUploadButton.IsEnabled = false;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        static public async void DisplayDialog(String dialog)
        {
            ContentDialog Dialog = new ContentDialog
            {
                //Title = "단가를 먼저 세팅해주세요",
                FontWeight = FontWeights.ExtraBold,
                Content = dialog,
                CloseButtonText = "확인"
            };
            ContentDialogResult result = await Dialog.ShowAsync();
        }
        public void ShowProgressRing()
        {
            ProgressRing.IsActive = true;
            ConvertingTextBlock.Visibility = Visibility.Visible;
        }
        public void HideProgressRing()
        {
            ProgressRing.IsActive = false;
            ConvertingTextBlock.Visibility = Visibility.Collapsed;
        }
        /*
        public Task ChangeTextBlock()
        {
            while(true)
            {
                Thread.Sleep(1000);
                ConvertingTextBlock.Text = "변환중입니다.";
                Thread.Sleep(1000);
                ConvertingTextBlock.Text = "변환중입니다..";
                Thread.Sleep(1000);
                ConvertingTextBlock.Text = "변환중입니다...";
            }
        }*/

        // ------------------------ 실내역 엑셀 파일 업로드 버튼 ------------------------------------------------------------------------------------------------------------------------ //
        private async void XlsUploadButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".xls");
            openPicker.FileTypeFilter.Add(".xlsx");
            IReadOnlyList<StorageFile> files = await openPicker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                StorageFolder copiedFolder = await Data.folder.CreateFolderAsync("Actual Xlsx", CreationCollisionOption.ReplaceExisting); // Actual Xlsx 폴더 생성
                StringBuilder output = new StringBuilder();
                // Application now has read/write access to the picked file(s)

                foreach (StorageFile file in files)
                {
                    output.Append(file.Name + "\n");
                    await file.CopyAsync(copiedFolder); // 실내역 파일 Actual Xlsx로 복사
                }
                Data.XlsText = output.ToString();
                XlsList.Text = Data.XlsText;
                Data.XlsFiles = files;

                Data.CanCovertFile = true;
                Data.IsConvert = false;
            }
            else
            {
                DisplayDialog("파일을 업로드 해주세요.");
                Data.XlsFiles = null;
            }
        }
        // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- //


        // -------------------------- 공내역 BID 파일 업로드 버튼 -------------------------------------------------------------------------------------------------------------------------- //
        private async void BidUploadButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".BID");
            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                StorageFolder copiedFolder = await Data.folder.CreateFolderAsync("Empty Bid", CreationCollisionOption.ReplaceExisting); // Empty Bid 폴더 생성
                await file.CopyAsync(copiedFolder); // 공내역 파일 Empty Bid로 복사

                // Application now has read/write access to the picked file
                Data.BidText = file.Name;
                BidList.Text = Data.BidText;
                Data.BidFile = file;

                Data.CanCovertFile = true;
                Data.IsConvert = false;
            }
            else
            {
                DisplayDialog("파일을 업로드 해주세요.");
                Data.BidFile = null;
            }
        }
        // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- //


        // ------------------ 단가세팅 버튼 -------------------------------------------------------------------------------------------------------------------------------------------------- //
        private async void ConvertButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ShowProgressRing();
            if (Data.XlsFiles == null || Data.BidFile == null)
            {
                HideProgressRing();
                DisplayDialog("파일을 업로드 해주세요.");
            }
            else if (!Data.CanCovertFile)
            {
                HideProgressRing();
                DisplayDialog("이미 변환을 완료한 파일입니다. \n새로운 파일 업로드 혹은 저장 버튼을 눌러주세요.");
            }
            else
            {
                XlsUploadButton.IsEnabled = false;
                BidUploadButton.IsEnabled = false;
                try
                {
                    //공내역 bid 파일 -> 공내역 xml 파일
                    await BidHandling.BidToXml();
                    //실내역 데이터 복사 및 단가 세팅 & 직공비 고정금액 비중 계산
                    //Setting.GetData(); -> 비동기 문제로 BidHandling.BidToXml()로 이동
                }
                catch
                {
                    if (!Data.IsFileMatch)
                    {
                        DisplayDialog("공내역 파일과 실내역 파일의 공사가 일치하는지 확인해주세요.");
                        HideProgressRing();
                        return;
                    }
                    DisplayDialog("정상적인 파일이 아닙니다. 파일을 확인해주세요.");
                    HideProgressRing();
                    return;
                }
                if (!Data.IsBidFileOk)
                {
                    DisplayDialog("정상적인 공내역 파일이 아닙니다. 파일을 확인해주세요.");
                    HideProgressRing();
                    return;
                }
                else if (!Data.IsFileMatch)
                {
                    DisplayDialog("공내역 파일과 실내역 파일의 공사가 일치하는지 확인해주세요.");
                    HideProgressRing();
                    return;
                }
                else
                {
                    //원가계산서상 없는 항목들 예외 처리(0 대입)
                    FillCostAccount.CheckKeyNotFound();

                    //원가계산서 항목별 조사금액 계산(보정 전)
                    FillCostAccount.CalculateInvestigationCosts(Data.Correction);

                    ViewCostAccount();

                    Data.CanCovertFile = false;
                    Data.IsConvert = true;
                    AdjustmentPage.isConfirm = true;
                    HideProgressRing();
                }  
            }
        }

        private async void ViewCostAccount ()
        {
            CoreApplicationView newCoreView = CoreApplication.CreateNewView();
            ApplicationView newAppView = null;
            int mainViewId = ApplicationView.GetApplicationViewIdForWindow(
                CoreApplication.MainView.CoreWindow);
            await newCoreView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                newAppView = ApplicationView.GetForCurrentView();
                Window.Current.Content = new Frame();
                (Window.Current.Content as Frame).Navigate(typeof(VerificationPage));
                Window.Current.Activate();
            });
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newAppView.Id, ViewSizePreference.UseHalf, mainViewId, ViewSizePreference.UseHalf);
        }
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ //

        // ------------------------- 초기화 버튼 ---------------------------------------------------------------------------------------------------------------------------------------------- //
        private async void InitButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            //앱 데이터 초기화
            await ApplicationData.Current.ClearAsync();

            // Data 초기화
            Data.ConstructionTerm = 0;
            Data.RealDirectMaterial = 0;
            Data.RealDirectLabor = 0;
            Data.RealOutputExpense = 0;
            Data.FixedPriceDirectMaterial = 0;
            Data.FixedPriceDirectLabor = 0;
            Data.FixedPriceOutputExpense = 0;
            Data.RealPriceDirectMaterial = 0;
            Data.RealPriceDirectLabor = 0;
            Data.RealPriceOutputExpense = 0;
            Data.InvestigateFixedPriceDirectMaterial = 0;
            Data.InvestigateFixedPriceDirectLabor = 0;
            Data.InvestigateFixedPriceOutputExpense = 0;
            Data.InvestigateStandardMaterial = 0;
            Data.InvestigateStandardLabor = 0;
            Data.InvestigateStandardExpense = 0;
            Data.PsMaterial = 0;
            Data.PsLabor = 0;
            Data.PsExpense = 0;
            Data.ExcludingMaterial = 0;
            Data.ExcludingLabor = 0;
            Data.ExcludingExpense = 0;
            Data.AdjustedExMaterial = 0;
            Data.AdjustedExLabor = 0;
            Data.AdjustedExExpense = 0;
            Data.GovernmentMaterial = 0;
            Data.GovernmentLabor = 0;
            Data.GovernmentExpense = 0;
            Data.SafetyPrice = 0;
            Data.StandardMaterial = 0;
            Data.StandardLabor = 0;
            Data.StandardExpense = 0;
            Data.InvestigateStandardMarket = 0;
            Data.FixedPricePercent = 0;

            //자료구조 초기화
            Data.Dic.Clear();
            Data.ConstructionNums.Clear();
            Data.MatchedConstNum.Clear();
            Data.Fixed.Clear();
            Data.Rate1.Clear();
            Data.Rate2.Clear();
            Data.RealPrices.Clear();
            Data.Investigation.Clear();
            Data.Bidding.Clear();
            Data.Correction.Clear();

            //변수 초기화
            Data.XlsFiles = null;
            Data.BidFile = null;
            Data.CanCovertFile = false;
            Data.IsConvert = false;
            Data.IsBidFileOk = true;
            Data.IsFileMatch = true;
            
            //업로드 버튼 활성화
            XlsUploadButton.IsEnabled = true;
            BidUploadButton.IsEnabled = true;
            AdjustmentPage.isConfirm = false;

            //텍스트 초기화
            XlsList.Text = "파일을 업로드 해주세요.";
            BidList.Text = "파일을 업로드 해주세요.";
            DisplayDialog("초기화 되었습니다.");

            //UWP 변수 초기화
            Data.BalanceRateNum = null;
            Data.PersonalRateNum = null;
            Data.UnitPriceTrimming ="1";
            Data.StandardMarketDeduction = "2";
            Data.ZeroWeightDeduction = "2";
            Data.CostAccountDeduction = "2";
            Data.BidPriceRaise = "2";
            Data.LaborCostLowBound = "2";
            Data.ExecuteReset = "0";
            CalculatePrice.targetRate = 0;
            CalculatePrice.myPercent = 0;
        }
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ //
    }
}
