using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SetUnitPriceByExcel;
using Windows.UI.Text;
using Microsoft.UI.Text;

namespace CMC_WinUI.Views
{
    public sealed partial class VerificationPage : Page
    {
        Frame myFrame = Window.Current.Content as Frame;

        public VerificationPage()
        {
            this.InitializeComponent();
           
            CostAccount_1.Text = Data.Investigation["순공사원가"].ToString("#,##0");
            CostAccount_2.Text = Data.Investigation["직접재료비"].ToString("#,##0");
            CostAccount_3.Text = Data.Investigation["직접재료비"].ToString("#,##0");
            CostAccount_4.Text = Data.Investigation["노무비"].ToString("#,##0");
            CostAccount_5.Text = Data.Investigation["직접노무비"].ToString("#,##0");
            CostAccount_6.Text = Data.Investigation["간접노무비"].ToString("#,##0");
            CostAccount_7.Text = Data.Investigation["경비"].ToString("#,##0");
            CostAccount_8.Text = Data.Investigation["산출경비"].ToString("#,##0");
            CostAccount_9.Text = Data.Investigation["산재보험료"].ToString("#,##0");
            CostAccount_10.Text = Data.Investigation["고용보험료"].ToString("#,##0");
            CostAccount_11.Text = Data.Fixed["국민건강보험료"].ToString("#,##0");
            CostAccount_12.Text = Data.Fixed["노인장기요양보험"].ToString("#,##0");
            CostAccount_13.Text = Data.Fixed["국민연금보험료"].ToString("#,##0");
            CostAccount_14.Text = Data.Fixed["퇴직공제부금"].ToString("#,##0");
            CostAccount_15.Text = Data.Fixed["산업안전보건관리비"].ToString("#,##0");
            CostAccount_16.Text = Data.Fixed["안전관리비"].ToString("#,##0");
            CostAccount_17.Text = Data.Fixed["품질관리비"].ToString("#,##0");
            CostAccount_18.Text = Data.Investigation["환경보전비"].ToString("#,##0");
            CostAccount_19.Value = Data.Investigation["공사이행보증서발급수수료"];
            CostAccount_20.Text = Data.Investigation["건설하도급보증수수료"].ToString("#,##0");
            CostAccount_21.Text = Data.Investigation["건설기계대여대금 지급보증서발급금액"].ToString("#,##0");
            CostAccount_22.Text = Data.Investigation["기타경비"].ToString("#,##0");
            CostAccount_23.Text = Data.Investigation["일반관리비"].ToString("#,##0");
            CostAccount_24.Value = Data.Investigation["이윤"];
            CostAccount_25.Text = Data.Investigation["PS"].ToString("#,##0");
            CostAccount_26.Text = Data.Investigation["제요율적용제외공종"].ToString("#,##0");
            CostAccount_27.Text = Data.Investigation["총원가"].ToString("#,##0");
            CostAccount_28.Value = Data.Investigation["공사손해보험료"];
            CostAccount_29.Text = Data.Investigation["소계"].ToString("#,##0");
            CostAccount_30.Text = Data.Investigation["부가가치세"].ToString("#,##0");
            CostAccount_31.Text = "0";
            CostAccount_32.Text = Data.Investigation["도급비계"].ToString("#,##0");
        }

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

        private void CostAccount_19_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            Data.Correction["공사이행보증서발급수수료"] = (long)CostAccount_19.Value;
        }

        private void CostAccount_24_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            Data.Correction["이윤"] = (long)CostAccount_24.Value;
        }

        private void CostAccount_28_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            Data.Correction["공사손해보험료"] = (long)CostAccount_28.Value;
        }

        private void CorrectionButton_Click(object sender, RoutedEventArgs e)
        {
            FillCostAccount.CalculateInvestigationCosts(Data.Correction);
            //원가계산서_세부결과 조사금액 세팅
            FillCostAccount.FillInvestigationCosts();
            myFrame.Navigate(this.GetType());
            DisplayDialog("보정 완료!");
        }
    }
}
