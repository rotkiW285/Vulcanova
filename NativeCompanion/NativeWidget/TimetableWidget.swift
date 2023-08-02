//
//  TimetableWidget.swift
//  NativeWidgetExtension
//
//  Created by Piotr Romanowski on 23/07/2023.
//

import WidgetKit
import SwiftUI

struct TimetableDataElement: Codable {
    let key: Date
    let value: [TimetableDataLesson]
}

struct TimetableDataLesson: Codable {
    let no: Int
    let subjectName, teacherName: String
    let date, start, end: Date
    let roomName: String?
}

typealias TimetableData = [TimetableDataElement]

let timetableSampleEntry = TimetableEntry(date: Date(),
                                         previousLesson: TimetableEntry.TimetableEntryLesson(no: 1, name: "Przyroda", classRoom: "21", start: "8:00", end: "8:45"),
                                         currentLesson: TimetableEntry.TimetableEntryLesson(no: 2, name: "Edb", classRoom: "37", start: "9:50", end: "10:35"),
                                         futureLessons: [
                                             TimetableEntry.TimetableEntryLesson(no: 3, name: "Religia", classRoom: "37", start: "10:45", end: "11:30"),
                                             TimetableEntry.TimetableEntryLesson(no: 4, name: "Przyroda", classRoom: "37", start: "11:45", end: "12:30"),
                                             TimetableEntry.TimetableEntryLesson(no: 5, name: "Wychowanie fizyczne", classRoom: "37", start: "12:40", end: "13:25"),
                                         ],
                                            timetableState: .normal
                                        )

struct TimetableTimelineProvider: TimelineProvider {
    func placeholder(in context: Context) -> TimetableEntry {
        timetableSampleEntry
    }
    
    func getSnapshot(in context: Context, completion: @escaping (TimetableEntry) -> ()) {
        let entry = timetableSampleEntry
        completion(entry)
    }
    
    func getTimeline(in context: Context, completion: @escaping (Timeline<TimetableEntry>) -> ()) {
        var entries: [TimetableEntry] = []
        
        let jsonData: TimetableData = readWidgetData(fileName: "timetable.json", defaultValue: []);
        
        if let dayGroup = jsonData.first(where: {Calendar.current.isDate(Date(), equalTo: $0.key, toGranularity: .day)}) {
            let date = dayGroup.key;
            
            let sortedLessons = dayGroup.value.sorted {
                $0.start < $1.start
            }
            let lessonsInDay = dayGroup.value.count
            
            if (lessonsInDay == 0) {
                entries.append(TimetableEntry(date: date, previousLesson: nil, currentLesson: nil, futureLessons: [], timetableState: .noLessonsThatDay))

                return
            }

            let startOfDay = Calendar.current.startOfDay(for: Date())
            
            entries.append(TimetableEntry(date: startOfDay, previousLesson: nil, currentLesson: nil, futureLessons: sortedLessons.map(TimetableEntry.TimetableEntryLesson.fromTimetableLesson)))
            
            for i in 0...(lessonsInDay - 1) {
                let previousLesson = sortedLessons[safelyIndex: i - 1]
                let currentLesson = sortedLessons[i]
                let futureLessons = lessonsInDay - i > 2 ? sortedLessons[(i + 1)...] : []
                
                entries.append(TimetableEntry(date: currentLesson.start, previousLesson:
                                                previousLesson == nil ? nil : TimetableEntry.TimetableEntryLesson.fromTimetableLesson(lesson: previousLesson!),
                                              currentLesson: TimetableEntry.TimetableEntryLesson.fromTimetableLesson(lesson: currentLesson),
                                              futureLessons: futureLessons.map(TimetableEntry.TimetableEntryLesson.fromTimetableLesson)))
            }

            if let lastLessonInDay = sortedLessons.last {
                entries.append(TimetableEntry(date: lastLessonInDay.end, previousLesson: nil, currentLesson: nil, futureLessons: [], timetableState: .lessonsOver))
            }
        }
        
        let currentDate = Date()
        let midnight = Calendar.current.startOfDay(for: currentDate)
        let nextMidnight = Calendar.current.date(byAdding: .day, value: 1, to: midnight)!
        
        let timeline = Timeline(entries: entries, policy: .after(nextMidnight))
        completion(timeline)
    }
}


struct TimetableEntry: TimelineEntry {
    let date: Date
    
    let previousLesson: TimetableEntryLesson?
    let currentLesson: TimetableEntryLesson?
    let futureLessons: [TimetableEntryLesson]
    var timetableState: TimetableState = .normal
    
    struct TimetableEntryLesson: Hashable {
        let no: Int
        let name: String
        let classRoom: String?
        let start: String
        let end: String
        
        static let timeFormatter: DateFormatter = {
            let formatter = DateFormatter()
            formatter.timeStyle = .short
            return formatter
        }()
        
        static func fromTimetableLesson(lesson: TimetableDataLesson) -> TimetableEntryLesson {
            return TimetableEntryLesson(no: lesson.no, name: lesson.subjectName, classRoom: lesson.roomName, start: timeFormatter.string(from: lesson.start), end: timeFormatter.string(from: lesson.end))
        }
    }
    
    enum TimetableState {
        case normal
        case noLessonsThatDay
        case lessonsOver
    }
}

struct TimetableEntryLessonView : View {
    let lesson: TimetableEntry.TimetableEntryLesson
    let style: TimetableEntryStyle
    var showTime: Bool = false
    
    var body: some View {
        HStack() {
            Text("\(lesson.no). \(lesson.name)" + (lesson.classRoom != nil ? " (\(lesson.classRoom!))" : "")).font(.subheadline).foregroundColor(getColor(style: self.style)).lineLimit(1)
            
            Spacer()
            
            if showTime {
                Text("\(lesson.start) - \(lesson.end)").font(.subheadline).foregroundColor(getColor(style: self.style)).lineLimit(1)
            }
        }
    }
    
    enum TimetableEntryStyle {
        case past
        case current
        case future
    }
    
    private func getColor(style: TimetableEntryStyle) -> Color {
        switch style {
        case .past: return Color.gray
        case .current: return Color.blue
        case .future: return Color.primary
        }
    }
}

struct TimetableWidgetEntryView : View {
    @Environment(\.widgetFamily) var family
    
    var entry: TimetableTimelineProvider.Entry
    
    
    
    var body: some View {
        
        ZStack(){
            VStack(alignment: .leading) {
                HStack() {
                    VStack(alignment: .leading) {
                        Text("Timetable").foregroundColor(.blue).bold().padding(.bottom, 2)
                        
                        if entry.timetableState == .lessonsOver {
                            Text("You are done for today 🥳").font(.subheadline)
                        } else if entry.timetableState == .noLessonsThatDay {
                            Text("No lessons today 🤙").font(.subheadline)
                        } else {
                            let showTime = family == .systemMedium
                            let renderFutureLessonsCnt = TimetableWidgetEntryView.getRenderFutureLessonsCount(entry: entry)
                            
                            if let previous = entry.previousLesson {
                                TimetableEntryLessonView(lesson: previous, style: TimetableEntryLessonView.TimetableEntryStyle.past, showTime: showTime)
                            }
                            
                            if let current = entry.currentLesson {
                                TimetableEntryLessonView(lesson: current, style: TimetableEntryLessonView.TimetableEntryStyle.current, showTime: showTime)
                            }
                            
                            let futureLessons = Array(entry.futureLessons.prefix(renderFutureLessonsCnt))
                            
                            ForEach(futureLessons, id: \.self) { futureLesson in
                                TimetableEntryLessonView(lesson: futureLesson, style: TimetableEntryLessonView.TimetableEntryStyle.future, showTime: showTime)
                            }
                            
                            let remainingToRender = entry.futureLessons.count - renderFutureLessonsCnt;
                            
                            if (remainingToRender > 0) {
                                if (remainingToRender > 1) {
                                    Text("...").font(.subheadline).foregroundColor(.gray)
                                }
                                
                                let lastLesson = entry.futureLessons.last!
                                TimetableEntryLessonView(lesson: lastLesson, style: TimetableEntryLessonView.TimetableEntryStyle.future, showTime: showTime)
                            }
                        }
                    }
                    Spacer()
                }
                Spacer()
            }
        }.padding(14)
        
    }
    
    static func getRenderFutureLessonsCount(entry: TimetableTimelineProvider.Entry) -> Int {
        var futureLessonsRenderCnt = 1;
        
        if entry.previousLesson == nil {
            futureLessonsRenderCnt += 1
        }
        
        if entry.currentLesson == nil {
            futureLessonsRenderCnt += 1
        }
        
        return futureLessonsRenderCnt
    }
}

struct TimetableWidget: Widget {
    let kind: String = "TimetableWidget"
    
    var body: some WidgetConfiguration {
        StaticConfiguration(kind: kind, provider: TimetableTimelineProvider()) { entry in
            TimetableWidgetEntryView(entry: entry)
        }
        .supportedFamilies([.systemSmall, .systemMedium])
        .configurationDisplayName("Timetable")
        .description("Today's timetable overview")
    }
}

struct TimetableWidget_Previews: PreviewProvider {
    static var previews: some View {
        TimetableWidgetEntryView(entry: timetableSampleEntry)
        .previewContext(WidgetPreviewContext(family: .systemMedium))
    }
}

extension Collection {
    subscript(safelyIndex i: Index) -> Element? {
        get {
            guard self.indices.contains(i) else { return nil }
            return self[i]
        }
    }
}
