using System.Net.Http;
using GoogleVisionBarCodeScanner;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Prism;
using Prism.Common;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Navigation.TabbedPages;
using ReactiveUI;
using Vulcanova.Core.Data;
using Vulcanova.Core.Layout;
using Vulcanova.Core.Mapping;
using Vulcanova.Core.NativeWidgets;
using Vulcanova.Core.Rx;
using Vulcanova.Core.Uonet;
using Vulcanova.Features.Attendance;
using Vulcanova.Features.Auth;
using Vulcanova.Features.Exams;
using Vulcanova.Features.Grades;
using Vulcanova.Features.Homework;
using Vulcanova.Features.LuckyNumber;
using Vulcanova.Features.Messages;
using Vulcanova.Features.Notes;
using Vulcanova.Features.Settings;
using Vulcanova.Features.Settings.HttpTrafficLogger;
using Vulcanova.Features.Shared;
using Vulcanova.Features.Timetable;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AppSettings = Vulcanova.Helpers.AppSettings;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Vulcanova;

public partial class App
{
    public App(IPlatformInitializer initializer) : base(initializer)
    {
    }

    protected override async void OnInitialized()
    {
        InitializeComponent();

        RxApp.DefaultExceptionHandler = new ReactiveExceptionHandler();

        Uonet.Config.HttpClient = new HttpClient(LoggingHttpMessageHandler.Instance);

        Sharpnado.Tabs.Initializer.Initialize(false, false); 
        Sharpnado.Shades.Initializer.Initialize(false);

        Methods.SetSupportBarcodeFormat(BarcodeFormats.QRCode);

        var sheetPopper = Container.Resolve<ISheetPopper>();

        if (sheetPopper != null)
        {
            sheetPopper.SheetWillDisappear += SheetPopperOnSheetWillDisappear;
            sheetPopper.SheetDisappeared += SheetPopperOnSheetDisappeared;
        }

        var accRepo = Container.Resolve<IAccountRepository>();

        // breaks app startup when executed asynchronously
        var activeAccount = accRepo.GetActiveAccountAsync().Result;

        if (activeAccount != null)
        {
            var ctx = Container.Resolve<AccountContext>();
            ctx.Account = activeAccount;

            await NavigationService.NavigateAsync(
                "MainNavigationPage/HomeTabbedPage?selectedTab=GradesSummaryView");
        }
        else
        {
            await NavigationService.NavigateAsync("OnboardingNavigationPage/IntroView");
        }

        var widgetUpdateDispatcher = Container.Resolve<NativeWidgetUpdateDispatcher>();
        widgetUpdateDispatcher.Setup();
    }
    
    #region Sheet navigation handlers

    private Page _previousPage;

    private void SheetPopperOnSheetDisappeared(object sender, SheetEventArgs e)
    {
        if (SheetPageNavigationService.PageNavigationSource == PageNavigationSource.Device)
        {
            PageUtilities.HandleSystemGoBack(e.Sheet, _previousPage);
        }
    }

    private void SheetPopperOnSheetWillDisappear(object sender, SheetEventArgs e)
    {
        if (SheetPageNavigationService.PageNavigationSource == PageNavigationSource.Device)
        {
            _previousPage = PageUtilities.GetOnNavigatedToTarget(e.Sheet, MainPage, true);
        }
    }
    
    #endregion

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterLiteDb();

        containerRegistry.RegisterAutoMapper();

        containerRegistry.RegisterLayout();
        
        containerRegistry.RegisterNativeWidgetsCommunication();

        containerRegistry.RegisterAuth();
        containerRegistry.RegisterLuckyNumber();
        containerRegistry.RegisterGrades();
        containerRegistry.RegisterTimetable();
        containerRegistry.RegisterAttendance();
        containerRegistry.RegisterExams();
        containerRegistry.RegisterHomework();
        containerRegistry.RegisterMessages();
        containerRegistry.RegisterNotes();

        containerRegistry.RegisterSettings();

        containerRegistry.RegisterUonet();
    }

    protected override void OnStart()
    {
        if (AppSettings.EnableAnalytics)
        {
            AppCenter.Start($"ios={AppSettings.AppCenterSecret}", typeof(Analytics), typeof(Crashes));
        }

        base.OnStart();
    }

    public async void OnWidgetInteraction(INativeWidgetProxy.NativeWidget widget)
    {
        var tabName = widget switch
        {
            INativeWidgetProxy.NativeWidget.Timetable => nameof(TimetableView),
            INativeWidgetProxy.NativeWidget.AttendanceStats => nameof(AttendanceView),
            _ => null
        };

        if (tabName is null)
        {
            return;
        }
        
        var navigationService = Container.Resolve<INavigationService>();

        await navigationService.SelectTabAsync(tabName);
    }
}