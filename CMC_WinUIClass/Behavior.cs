using System;
using System.Globalization;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.Resources;


namespace CMC_WinUIClass
{
    //
    // 요약:
    //     A base class for behaviors, implementing the basic plumbing of IBehavior
    public abstract class Behavior : DependencyObject, IBehavior
    {
        //
        // 요약:
        //     Gets the Windows.UI.Xaml.DependencyObject to which the behavior is attached.
        public DependencyObject AssociatedObject
        {
            get;
            private set;
        }

        //
        // 요약:
        //     Attaches the behavior to the specified Windows.UI.Xaml.DependencyObject.
        //
        // 매개 변수:
        //   associatedObject:
        //     The Windows.UI.Xaml.DependencyObject to which to attach.
        //
        // 예외:
        //   T:System.ArgumentNullException:
        //     associatedObject is null.
        public void Attach(DependencyObject associatedObject)
        {
            if (associatedObject != AssociatedObject && !DesignMode.DesignModeEnabled)
            {
                if (AssociatedObject != null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, ResourceHelper.CannotAttachBehaviorMultipleTimesExceptionMessage, associatedObject, AssociatedObject));
                }

                if (associatedObject == null)
                {
                    throw new ArgumentNullException("associatedObject");
                }

                AssociatedObject = associatedObject;
                OnAttached();
            }
        }

        //
        // 요약:
        //     Detaches the behaviors from the Microsoft.Xaml.Interactivity.Behavior.AssociatedObject.
        public void Detach()
        {
            OnDetaching();
            AssociatedObject = null;
        }

        //
        // 요약:
        //     Called after the behavior is attached to the Microsoft.Xaml.Interactivity.Behavior.AssociatedObject.
        //
        // 설명:
        //     Override this to hook up functionality to the Microsoft.Xaml.Interactivity.Behavior.AssociatedObject
        protected virtual void OnAttached()
        {
        }

        //
        // 요약:
        //     Called when the behavior is being detached from its Microsoft.Xaml.Interactivity.Behavior.AssociatedObject.
        //
        // 설명:
        //     Override this to unhook functionality from the Microsoft.Xaml.Interactivity.Behavior.AssociatedObject
        protected virtual void OnDetaching()
        {
        }

        internal static class ResourceHelper
        {
            public static string CallMethodActionValidMethodNotFoundExceptionMessage => GetString("CallMethodActionValidMethodNotFoundExceptionMessage");

            public static string ChangePropertyActionCannotFindPropertyNameExceptionMessage => GetString("ChangePropertyActionCannotFindPropertyNameExceptionMessage");

            public static string ChangePropertyActionCannotSetValueExceptionMessage => GetString("ChangePropertyActionCannotSetValueExceptionMessage");

            public static string ChangePropertyActionPropertyIsReadOnlyExceptionMessage => GetString("ChangePropertyActionPropertyIsReadOnlyExceptionMessage");

            public static string GoToStateActionTargetHasNoStateGroups => GetString("GoToStateActionTargetHasNoStateGroups");

            public static string CannotAttachBehaviorMultipleTimesExceptionMessage => GetString("CannotAttachBehaviorMultipleTimesExceptionMessage");

            public static string CannotFindEventNameExceptionMessage => GetString("CannotFindEventNameExceptionMessage");

            public static string InvalidLeftOperand => GetString("InvalidLeftOperand");

            public static string InvalidRightOperand => GetString("InvalidRightOperand");

            public static string InvalidOperands => GetString("InvalidOperands");

            public static string GetString(string resourceName)
            {
                return ResourceLoader.GetForCurrentView("Microsoft.Xaml.Interactivity/Strings").GetString(resourceName);
            }
        }
    
    }
}
#if false // 디컴파일 로그
캐시의 '199'개 항목
------------------
확인: 'System.Runtime, Version=4.0.20.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
단일 어셈블리를 찾았습니다. 'System.Runtime, Version=4.2.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
WARN: 버전이 일치하지 않습니다. 예상: '4.0.20.0', 실제: '4.2.1.0'
로드 위치: 'C:\Users\KangDain\.nuget\packages\microsoft.netcore.universalwindowsplatform\6.2.10\ref\uap10.0.15138\System.Runtime.dll'
------------------
확인: 'Windows.Foundation.UniversalApiContract, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null'
단일 어셈블리를 찾았습니다. 'Windows.Foundation.UniversalApiContract, Version=10.0.0.0, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime'
WARN: 버전이 일치하지 않습니다. 예상: '2.0.0.0', 실제: '10.0.0.0'
로드 위치: 'C:\Program Files (x86)\Windows Kits\10\References\10.0.19041.0\Windows.Foundation.UniversalApiContract\10.0.0.0\Windows.Foundation.UniversalApiContract.winmd'
------------------
확인: 'Windows.Foundation.FoundationContract, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null'
단일 어셈블리를 찾았습니다. 'Windows.Foundation.FoundationContract, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime'
WARN: 버전이 일치하지 않습니다. 예상: '2.0.0.0', 실제: '4.0.0.0'
로드 위치: 'C:\Program Files (x86)\Windows Kits\10\References\10.0.19041.0\Windows.Foundation.FoundationContract\4.0.0.0\Windows.Foundation.FoundationContract.winmd'
------------------
확인: 'System.Collections, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
단일 어셈블리를 찾았습니다. 'System.Collections, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
WARN: 버전이 일치하지 않습니다. 예상: '4.0.10.0', 실제: '4.1.1.0'
로드 위치: 'C:\Users\KangDain\.nuget\packages\microsoft.netcore.universalwindowsplatform\6.2.10\ref\uap10.0.15138\System.Collections.dll'
------------------
확인: 'System.Runtime.InteropServices.WindowsRuntime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
단일 어셈블리를 찾았습니다. 'System.Runtime.InteropServices.WindowsRuntime, Version=4.0.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
WARN: 버전이 일치하지 않습니다. 예상: '4.0.0.0', 실제: '4.0.3.0'
로드 위치: 'C:\Users\KangDain\.nuget\packages\microsoft.netcore.universalwindowsplatform\6.2.10\ref\uap10.0.15138\System.Runtime.InteropServices.WindowsRuntime.dll'
------------------
확인: 'System.Globalization, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
단일 어셈블리를 찾았습니다. 'System.Globalization, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
WARN: 버전이 일치하지 않습니다. 예상: '4.0.10.0', 실제: '4.1.1.0'
로드 위치: 'C:\Users\KangDain\.nuget\packages\microsoft.netcore.universalwindowsplatform\6.2.10\ref\uap10.0.15138\System.Globalization.dll'
------------------
확인: 'System.Runtime, Version=4.2.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
단일 어셈블리를 찾았습니다. 'System.Runtime, Version=4.2.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
로드 위치: 'C:\Users\KangDain\.nuget\packages\microsoft.netcore.universalwindowsplatform\6.2.10\ref\uap10.0.15138\System.Runtime.dll'
#endif
