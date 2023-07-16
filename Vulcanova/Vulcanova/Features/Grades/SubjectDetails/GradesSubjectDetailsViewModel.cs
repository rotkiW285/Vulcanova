using System.IO;
using System.Reactive;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Features.Grades.Summary;
using Vulcanova.Features.Settings;
using Xamarin.Essentials;

namespace Vulcanova.Features.Grades.SubjectDetails;

public sealed class GradesSubjectDetailsViewModel : ReactiveObject, IInitialize
{
    [Reactive]
    public SubjectGrades Subject { get; private set; }
    
    [ObservableAsProperty]
    public bool CanShare { get; }

    public ReactiveCommand<Grade, Unit> ShareGrade { get; }

    public GradesSubjectDetailsViewModel(AppSettings appSettings)
    {
        ShareGrade = ReactiveCommand.CreateFromTask(async (Grade grade) =>
        {
            var bytes = GradeShareImageGenerator.DrawImageForGrade(grade);
            
            var file = Path.Combine(FileSystem.CacheDirectory, "grade.png");

            await File.WriteAllBytesAsync(file, bytes);

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Share this grade",
                File = new ShareFile(file)
            });
        }, appSettings.WhenAnyValue(settings => settings.LongPressToShareGrade));

        ShareGrade.CanExecute.ToPropertyEx(this, vm => vm.CanShare);
    }

    public void Initialize(INavigationParameters parameters)
    {
        Subject = (SubjectGrades) parameters[nameof(Subject)];
    }
}