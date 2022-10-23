using System.Collections.Generic;
using System.Reactive;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Features.Shared;

namespace Vulcanova.Features.Attendance.Justification;

public class JustifyAbsenceViewModel : ViewModelBase, IInitialize
{
    public ReactiveCommand<Unit, Unit> SubmitJustification { get; }

    [Reactive]
    public string Message { get; set; } = string.Empty;
    
    [Reactive]
    public IEnumerable<Lesson> Lessons { get; private set; }

    public JustifyAbsenceViewModel(
        INavigationService navigationService,
        LessonsService lessonsService,
        AccountContext accountContext) : base(navigationService)
    {
        SubmitJustification = ReactiveCommand.CreateFromTask( async () =>
        {
            foreach (var lesson in Lessons)
            {
                await lessonsService.SubmitAbsenceJustification(accountContext.Account.Id, lesson!.LessonClassId,
                    Message);
            }

            await navigationService.GoBackAsync(("didJustify", true));
        });
    }

    public void Initialize(INavigationParameters parameters)
    {
        Lessons = (IEnumerable<Lesson>) parameters[nameof(Lessons)];
    }
}