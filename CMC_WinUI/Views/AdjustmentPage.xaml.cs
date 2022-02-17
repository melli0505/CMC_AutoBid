using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using SetUnitPriceByExcel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Foundation;
using Microsoft.UI.Xaml.Core; // using Microsoft.UI.Core
using System.Diagnostics;
using Microsoft.UI.Text;
using Windows.Globalization.NumberFormatting;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Core;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Provider;

namespace CMC_WinUI.Views
{
    public sealed partial class AdjustmentPage : Page, INotifyPropertyChanged
    {
        private static bool isCalculate = false;
        public static bool isConfirm = false;

        public AdjustmentPage()
        {
            InitializeComponent();
            SetNumberBoxNumberFormatter();

            FixedPercentPrice.Text = Data.FixedPricePercent + " %";
            MyPercent.Text = "(+/-) " + CalculatePrice.myPercent * 100.0m + " %";
            TargetRate.Text = CalculatePrice.targetRate + " %";

            // 사정율 초기화
            if (Data.BalanceRateNum != null  && Data.PersonalRateNum != null) 
            {
                BalanceRateBox.Value = (double)Data.BalanceRateNum;
                PersonalRateBox.Value = (double)Data.PersonalRateNum;
            }
            // 라디오 버튼 초기화
            Data.UnitPriceTrimming = "1";
            // 표준시장 단가 체크
            if (Data.StandardMarketDeduction == "1")
                CheckStandardPrice.IsChecked = true;
            else
                CheckStandardPrice.IsChecked = false;
            // 공종 가중치 체크
            if (Data.ZeroWeightDeduction == "1")
                CheckWeightValue.IsChecked = true;
            else
                CheckWeightValue.IsChecked = false;
            // 법정 요율 체크
            if (Data.CostAccountDeduction == "1")
                CheckCAD.IsChecked = true;
            else
                CheckCAD.IsChecked = false;
            // 원단위 체크
            if (Data.BidPriceRaise == "1")
                CheckCeiling.IsChecked = true;
            else
                CheckCeiling.IsChecked = false;
        }

        private void SetNumberBoxNumberFormatter()
        {
            IncrementNumberRounder rounder = new IncrementNumberRounder();
            rounder.Increment = 0.0001;
            rounder.RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp;
            DecimalFormatter formatter = new DecimalFormatter();
            formatter.IntegerDigits = 1;
            formatter.FractionDigits = 4;
            formatter.NumberRounder = rounder;
            PersonalRateBox.NumberFormatter = formatter;
            BalanceRateBox.NumberFormatter = formatter;
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

        static private async void DisplayDialog(string dialog)
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

        // ------------------------- 세부 결과 확인 버튼 ------------------------------------------------------------------------------------------------------------------------------------------------- //
        private async void ShowResult_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (isCalculate)
            {
                CoreApplicationView newCoreView = CoreApplication.CreateNewView();
                ApplicationView newAppView = null;
                int mainViewId = ApplicationView.GetApplicationViewIdForWindow(
                    CoreApplication.MainView.CoreWindow);
// TODO : 이거 고쳐라
                await newCoreView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { // Core dispatcher은 Dispatcher queue로 대체됨
                    newAppView = ApplicationView.GetForCurrentView();
                    Window.Current.Content = new Frame();
                    (Window.Current.Content as Frame).Navigate(typeof(ResultPage));
                    Window.Current.Activate();
                });
                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newAppView.Id, ViewSizePreference.UseHalf, mainViewId, ViewSizePreference.UseHalf);
            }
            else
            {
                DisplayDialog("계산 후 확인해주세요");
            }
        }
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ //

        // ------------------------- 옵션 입력 버튼 ------------------------------------------------------------------------------------------------------------------------------------------- //

        // 소수 1자리 체크
        private void RadioDecimal_Checked(object sender, RoutedEventArgs e)
        {
            Data.UnitPriceTrimming = "1";
        }

        // 정수 체크
        private void RadioInteger_Checked(object sender, RoutedEventArgs e)
        {
            Data.UnitPriceTrimming = "2";
        }

        // 표준시장 단가 체크
        private void CheckStandardPrice_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)CheckStandardPrice.IsChecked)
            {
                Data.StandardMarketDeduction = "1";
            }
            else
            {
                Data.StandardMarketDeduction = "2";
            }
        }

        // 공종 가중치 체크
        private void CheckWeightValue_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)CheckWeightValue.IsChecked)
            {
                Data.ZeroWeightDeduction = "1";
            }
            else
            {
                Data.ZeroWeightDeduction = "2";
            }
        }

        // 법정요율 체크
        private void CheckCAD_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)CheckCAD.IsChecked)
            {
                Data.CostAccountDeduction = "1";
            }
            else
            {
                Data.CostAccountDeduction = "2";
            }
        }

        // 원단위 체크
        private void CheckCeiling_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)CheckCeiling.IsChecked)
            {
                Data.BidPriceRaise = "1";
            }
            else
            {
                Data.BidPriceRaise = "2";
            }
        }

        //노무비 하한율 체크
        private void CheckLaborCost_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)CheckCeiling.IsChecked)
            {
                Data.LaborCostLowBound = "1";
            }
            else
            {
                Data.LaborCostLowBound = "2";
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ //

        // ------------------------- 업체 평균 사정율 입력 ------------------------------------------------------------------------------------------------------------------------------------------ //
        private void BalanceRateBox_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            Data.BalanceRateNum = BalanceRateBox.Value;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ //

        // ------------------------- 내 예가 사정율 입력 ------------------------------------------------------------------------------------------------------------------------------------------ //
        private void PersonalRateBox_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            Data.PersonalRateNum = PersonalRateBox.Value;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ //

        // ------------------------- 계산 버튼 ------------------------------------------------------------------------------------------------------------------------------------------------ //
        private void CalculateButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Before doing any calculations, confirm the user entered both numbers.
            if (Data.PersonalRateNum == null || Data.BalanceRateNum == null)
            {
                //await new MessageDialog("사정율을 입력해주세요.").ShowAsync();
                DisplayDialog("사정율을 입력해주세요");
                return;
            }

            // 단가를 불러온 경우
            if (isConfirm)
            {

                //입찰금액 심사 점수 계산 및 단가 조정
                CalculatePrice.Calculation(Data.PersonalRateNum, Data.BalanceRateNum);

                FixedPercentPrice.Text = Data.FixedPricePercent + " %";
                MyPercent.Text = "(+/-) " + CalculatePrice.myPercent * 100.0m + " %";
                TargetRate.Text = Data.Bidding["도급비계"] + " 원 " + "(" + FillCostAccount.GetRate("도급비계")+ " %)"; // 도급비계
                isCalculate = true;

                //OutputTextBlock.Text = "사정율 적용 완료!";
                DisplayDialog("사정율 적용을 완료하였습니다");
            }

            // 단가를 불러오지 않은 경우
            else
            {
                DisplayDialog("단가를 먼저 세팅해주세요.");
            }

        }
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ //

        // ------------------------- BID파일 저장 버튼 ---------------------------------------------------------------------------------------------------------------------------------------- //
        private async void SaveBidButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // TargetRate가 계산 되어 있을 경우
            if (isCalculate)
            {
                //단가 세팅 완료한 xml 파일을 다시 BID 파일로 변환
                BidHandling.XmlToBid();

                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("BID 파일", new List<string>() { ".BID" });
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = BidHandling.filename.Substring(0, 16);
                StorageFile file = await savePicker.PickSaveFileAsync();
                StorageFolder bidFolder = await Data.folder.GetFolderAsync("Result Bid");
                StorageFile finalBidFile = await bidFolder.GetFileAsync(BidHandling.filename.Substring(0, 16) + ".BID");

                if (file != null)
                {
                    // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(file);

                    // 파일 덮어쓰기
                    await finalBidFile.CopyAndReplaceAsync(file);

                    // Let Microsoft know that we're finished changing the file so the other app can update the remote version of the file.
                    // Completing updates may require Microsoft to ask for user input.
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

                    if (status == FileUpdateStatus.Complete)
                    {
                        //OutputTextBlock.Text = file.Name + "이(가) 성공적으로 저장되었습니다.";
                        DisplayDialog(file.Name + "이(가) 성공적으로 저장되었습니다.");
                    }
                    else if (status == FileUpdateStatus.CompleteAndRenamed)
                    {
                        //OutputTextBlock.Text = file.Name + "이(가) 성공적으로 저장되었습니다.";
                        DisplayDialog(file.Name + "이(가) 성공적으로 저장되었습니다.");
                    }
                    else
                    {
                        //OutputTextBlock.Text = file.Name + "을(를) 저장할 수 없습니다.";
                        DisplayDialog(file.Name + "을(를) 저장할 수 없습니다.");
                    }
                }
                else
                {
                    DisplayDialog("취소되었습니다.");
                    //OutputTextBlock.Text = "취소되었습니다.";
                }
            }

            // 계산 안되어 있을 경우
            else
            {
                DisplayDialog("입찰점수를 계산해주세요.");
                //OutputTextBlock.Text = "입찰점수를 계산해주세요.";
            }
        }
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ //

        // ------------------------- 원가계산서 저장 버튼 ------------------------------------------------------------------------------------------------------------------------------------- //
        private async void SaveCostButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // TargetRate가 계산 되어 있을 경우
            if (isCalculate)
            {
                //가격 조정 후 원가계산서 엑셀파일 생성
                FillCostAccount.FillBiddingCosts();

                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("Microsoft Excel (.xlsx)", new List<string>() { ".xlsx" });
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = "원가계산서_세부결과";
                StorageFile file = await savePicker.PickSaveFileAsync();
                StorageFile costFile = await Data.folder.GetFileAsync("원가계산서_세부결과.xlsx");

                if (file != null)
                {
                    // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(file);

                    // 파일 덮어쓰기
                    await costFile.CopyAndReplaceAsync(file);

                    // Let Microsoft know that we're finished changing the file so the other app can update the remote version of the file.
                    // Completing updates may require Microsoft to ask for user input.
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

                    if (status == FileUpdateStatus.Complete)
                    {
                        //OutputTextBlock.Text = file.Name + "이(가) 성공적으로 저장되었습니다.";
                        DisplayDialog(file.Name + "이(가) 성공적으로 저장되었습니다.");
                    }
                    else if (status == FileUpdateStatus.CompleteAndRenamed)
                    {
                        //OutputTextBlock.Text = file.Name + "이(가) 성공적으로 저장되었습니다.";
                        DisplayDialog(file.Name + "이(가) 성공적으로 저장되었습니다.");
                    }
                    else
                    {
                        //OutputTextBlock.Text = file.Name + "을(를) 저장할 수 없습니다..";
                        DisplayDialog(file.Name + "을(를) 저장할 수 없습니다.");
                    }
                }
                else
                {
                    //OutputTextBlock.Text = "취소되었습니다.";
                    DisplayDialog("취소되었습니다.");
                }
            }

            // 계산 안되어 있을 경우
            else
            {
                DisplayDialog("계산을 먼저 실행해주세요.");
                //OutputTextBlock.Text = "입찰점수를 계산해주세요.";
            }
        }
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ //

        // ------------------------- 입찰 내역 저장 버튼 -------------------------------------------------------------------------------------------------------------------------------------- //
        private async void SaveBiddingZipButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // TargetRate가 계산 되어 있을 경우
            if (isCalculate)
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("Zip 압축 파일", new List<string>() { ".zip" });
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = "입찰내역";
                StorageFile file = await savePicker.PickSaveFileAsync();
                StorageFolder biddingFolder = await Data.folder.GetFolderAsync("입찰내역");
                StorageFile biddingZipFile = await biddingFolder.GetFileAsync("입찰내역.zip");

                if (file != null)
                {
                    // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                    CachedFileManager.DeferUpdates(file);

                    // 파일 덮어쓰기
                    await biddingZipFile.CopyAndReplaceAsync(file);

                    // Let Microsoft know that we're finished changing the file so the other app can update the remote version of the file.
                    // Completing updates may require Microsoft to ask for user input.
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

                    if (status == FileUpdateStatus.Complete)
                    {
                        //OutputTextBlock.Text = file.Name + "이(가) 성공적으로 저장되었습니다.";
                        DisplayDialog(file.Name + "이(가) 성공적으로 저장되었습니다.");
                    }
                    else if (status == FileUpdateStatus.CompleteAndRenamed)
                    {
                        //OutputTextBlock.Text = file.Name + "이(가) 성공적으로 저장되었습니다.";
                        DisplayDialog(file.Name + "이(가) 성공적으로 저장되었습니다.");
                    }
                    else
                    {
                        //OutputTextBlock.Text = file.Name + "을(를) 저장할 수 없습니다.";
                        DisplayDialog(file.Name + "을(를) 저장할 수 없습니다.");
                    }
                }
                else
                {
                    //OutputTextBlock.Text = "취소되었습니다.";
                    DisplayDialog("취소되었습니다.");
                }
            }

            // 계산 안되어 있을 경우
            else
            {
                DisplayDialog("계산을 먼저 실행해주세요.");
                //OutputTextBlock.Text = "입찰점수를 계산해주세요.";
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ //
    }
}
