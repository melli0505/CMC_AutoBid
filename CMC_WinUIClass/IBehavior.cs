#region 어셈블리 Microsoft.Xaml.Interactivity, Version=2.0.1.0, Culture=neutral, PublicKeyToken=null
// C:\Users\KangDain\.nuget\packages\microsoft.xaml.behaviors.uwp.managed\2.0.1\lib\uap10.0\Microsoft.Xaml.Interactivity.dll
// Decompiled with ICSharpCode.Decompiler 6.1.0.5902
#endregion

using Windows.UI.Xaml;
using Microsoft.UI.Xaml;

namespace CMC_WinUIClass
{
    //
    // 요약:
    //     Interface implemented by all custom behaviors.
    public interface IBehavior
    {
        //
        // 요약:
        //     Gets the Windows.UI.Xaml.DependencyObject to which the Microsoft.Xaml.Interactivity.IBehavior
        //     is attached.
        DependencyObject AssociatedObject
        {
            get;
        }

        //
        // 요약:
        //     Attaches to the specified object.
        //
        // 매개 변수:
        //   associatedObject:
        //     The Windows.UI.Xaml.DependencyObject to which the Microsoft.Xaml.Interactivity.IBehavior
        //     will be attached.
        void Attach(DependencyObject associatedObject);

        //
        // 요약:
        //     Detaches this instance from its associated object.
        void Detach();
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
