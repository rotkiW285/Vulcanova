using System;
using System.Linq;
using Prism.Navigation;
using Vulcanova.Features.Attendance.Justification;
using Vulcanova.Features.Shared;
using Vulcanova.Uonet.Api.Auth;
using Vulcanova.Uonet.Api.Lessons;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Attendance.LessonDetails;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class LessonDetailsView : INavigationAware
{
    private readonly INavigationService _navigationService;
    private readonly AccountContext _accountContext;

    public static readonly BindableProperty LessonProperty =
        BindableProperty.Create(nameof(Lesson), typeof(Lesson), typeof(LessonDetailsView),
            propertyChanged: LessonPropertyChanged);

    public Lesson Lesson
    {
        get => (Lesson) GetValue(LessonProperty);
        set => SetValue(LessonProperty, value);
    }

    private bool _didJustify;

    public LessonDetailsView(INavigationService navigationService, AccountContext accountContext)
    {
        _navigationService = navigationService;
        _accountContext = accountContext;
        InitializeComponent();
        
        UpdateJustifyButtonPresence();
    }

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
        parameters.Add("didJustify", _didJustify);
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        if (parameters.TryGetValue<Lesson>(nameof(Lesson), out var l))
        {
            Lesson = l;
        }

        if (parameters.TryGetValue("didJustify", out _didJustify) && _didJustify)
        {
            Lesson.JustificationStatus = JustificationStatus.Requested;
         
            UpdateJustifyButtonPresence();
        }
    }

    private void JustifyButton_OnClicked(object sender, EventArgs e)
    {
        _navigationService.NavigateAsync(nameof(JustifyAbsenceView), parameters: new NavigationParameters
        {
            {nameof(JustifyAbsenceViewModel.Lessons), new[] { Lesson }}
        }, useModalNavigation: true);
    }
    
    private static void LessonPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        var view = (LessonDetailsView) bindable;
        view.UpdateJustifyButtonPresence();
    }

    private void UpdateJustifyButtonPresence()
    {
        var l = Lesson;

        if (l == null) return;

        JustifyAbsenceButton.IsVisible = _accountContext.Account.Capabilities.Contains(AccountCapabilities.JustificationsEnabled)
                                         && (l.PresenceType.Late || l.PresenceType.Absence)
                                         && !l.PresenceType.AbsenceJustified
                                         && l.JustificationStatus == null;
    }
}