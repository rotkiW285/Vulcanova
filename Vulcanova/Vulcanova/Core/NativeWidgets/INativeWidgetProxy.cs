namespace Vulcanova.Core.NativeWidgets;

public interface INativeWidgetProxy
{
    public void UpdateWidgetState<T>(NativeWidget widget, T data);

    public enum NativeWidget
    {
        AttendanceStats,
        Timetable
    }
}