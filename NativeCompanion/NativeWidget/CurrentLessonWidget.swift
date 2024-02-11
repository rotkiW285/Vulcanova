//
//  CurrentLessonWidget.swift
//  NativeWidgetExtension
//
//  Created by Piotr Romanowski on 05/02/2024.
//

import WidgetKit
import SwiftUI

@available(iOSApplicationExtension 17.0, *)
struct ViewSizeWidgetView : View {
    
    let entry: CurrentLessonState
    
    var body: some View {
        ZStack {
            VStack(alignment: .leading) {
                HStack(spacing: 4) {
                    Image(systemName: "graduationcap.fill").imageScale(.small)
                    Text("Lessons").font(.caption.smallCaps())
                }
                if entry.timetableState == .lessonsOver {
                    Text("That's it for today").font(.subheadline)
                } else if entry.timetableState == .noLessonsThatDay {
                    Text("Nothing today").font(.subheadline)
                } else if entry.timetableState == .missingData {
                    Text("Refresh data in app").font(.subheadline)
                } else if entry.timetableState == .noData {
                    Text("Add account in app").font(.subheadline)
                } else {
                    if let currentLesson = entry.currentLesson, let futureLesson = entry.futureLesson {
                        LessonView(lesson: currentLesson)
                        LessonView(lesson: futureLesson, font: .footnote)
                    } else {
                        if let lessonToDisplay = entry.currentLesson ?? entry.futureLesson {
                            LessonView(lesson: lessonToDisplay)
                        }
                    }
                }
            }.padding(4)
            
        }
        .containerBackground(for: .widget) { }
    }
    
    struct LessonView : View {
        let lesson: CurrentLessonState.Lesson
        var font: Font = .subheadline
        
        var body: some View {
            HStack {
                Text("\(lesson.start) \(lesson.name)").font(font).lineLimit(1)
                if let classRoom = lesson.classRoom {
                    Text(classRoom).font(font).lineLimit(1)
                }
            }
        }
    }
}

let lessonStateSample = CurrentLessonState(date: Date(), currentLesson: CurrentLessonState.Lesson(no: 1, name: "Język polski", classRoom: "22", start: "7:30"), futureLesson: CurrentLessonState.Lesson(no: 1, name: "Matematyka", classRoom: "22", start: "8:20"))

struct CurrentLessonTimelineProvider: TimelineProvider {
    
    typealias Entry = CurrentLessonState
    
    func placeholder(in context: Context) -> CurrentLessonState {
        lessonStateSample
    }
    
    func getSnapshot(in context: Context, completion: @escaping (CurrentLessonState) -> ()) {
        let entry = lessonStateSample
        completion(entry)
    }
    
    func getTimeline(in context: Context, completion: @escaping (Timeline<CurrentLessonState>) -> ()) {
        let entries = getEntries()
        let currentDate = Date()
        let midnight = Calendar.current.startOfDay(for: currentDate)
        let nextMidnight = Calendar.current.date(byAdding: .day, value: 1, to: midnight)!
        
        let timeline = Timeline(entries: entries, policy: .after(nextMidnight))
        completion(timeline)
    }
    
    private func getEntries() -> [CurrentLessonState] {
        var entries: [CurrentLessonState] = []
        
        let jsonData = readTimetableData()
        
        guard let jsonData = jsonData else {
            return [CurrentLessonState.empty(date: Date(), state: .noData)]
        }
        
        guard let dayGroup = jsonData.first(where: {Calendar.current.isDate(Date(), equalTo: $0.key, toGranularity: .day)}) else {
            return [CurrentLessonState.empty(date: Date(), state: .missingData)]
        }
        
        let date = dayGroup.key;
        
        // red is for lessons not happening
        let lessonsHappening = dayGroup.value.filter { $0.displayColor != .red }
        
        let sortedLessons = lessonsHappening.sorted {
            $0.start < $1.start
        }
        let lessonsInDay = lessonsHappening.count
        
        if lessonsInDay == 0 {
            entries.append(CurrentLessonState.empty(date: date, state: .noLessonsThatDay))
            
            return entries
        }
        
        let startOfDay = Calendar.current.startOfDay(for: Date())
        
        entries.append(CurrentLessonState(date: startOfDay, currentLesson: nil, futureLesson: CurrentLessonState.Lesson.fromTimetableLesson(lesson: sortedLessons[0])))
        
        for i in 0...(lessonsInDay - 1) {
            let currentLesson = sortedLessons[i]
            let futureLesson = lessonsInDay - i >= 2 ? CurrentLessonState.Lesson.fromTimetableLesson(lesson: sortedLessons[i + 1]) : nil
            
            entries.append(CurrentLessonState(date: currentLesson.start,
                                              currentLesson: CurrentLessonState.Lesson.fromTimetableLesson(lesson: currentLesson),
                                              futureLesson: futureLesson))
        }
        
        if let lastLessonInDay = sortedLessons.last {
            entries.append(CurrentLessonState.empty(date: lastLessonInDay.end, state: .lessonsOver))
        }
        
        return entries
    }
}

struct CurrentLessonState: TimelineEntry {
    let date: Date
    
    let currentLesson: Lesson?
    let futureLesson: Lesson?
    var timetableState: TimetableState = .normal
    
    struct Lesson: Hashable {
        let no: Int
        let name: String
        let classRoom: String?
        let start: String
        
        static let timeFormatter: DateFormatter = {
            let formatter = DateFormatter()
            formatter.timeStyle = .short
            return formatter
        }()
        
        static func fromTimetableLesson(lesson: TimetableDataLesson) -> Lesson {
            return Lesson(no: lesson.no, name: lesson.subjectName, classRoom: lesson.roomName, start: timeFormatter.string(from: lesson.start))
        }
    }
    
    static func empty(date: Date, state: TimetableState) -> CurrentLessonState {
        CurrentLessonState(date: date, currentLesson: nil, futureLesson: nil, timetableState: state)
    }
    
    enum TimetableState {
        case normal
        case noLessonsThatDay
        case lessonsOver
        case missingData
        case noData
    }
}

@available(iOSApplicationExtension 17.0, *)
struct CurrentLessonWidget: Widget {
    let kind: String = "CurrentLessonWidget"
    
    var body: some WidgetConfiguration {
        StaticConfiguration(
            kind: kind,
            provider: CurrentLessonTimelineProvider()
        ) { entry in
            ViewSizeWidgetView(entry: entry)
        }
        .configurationDisplayName("Lessons now")
        .description("Current and upcoming lesson at a glance")
        .supportedFamilies([
            .accessoryRectangular
        ])
    }
}

@available(iOSApplicationExtension 17.0, *)
struct CurrentLessonWidget_Previews: PreviewProvider {
    static var previews: some View {
        ViewSizeWidgetView(entry: lessonStateSample)
            .previewContext(WidgetPreviewContext(family: .accessoryRectangular))
    }
}
