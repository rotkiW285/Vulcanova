//
//  NativeWidget.swift
//  NativeWidget
//
//  Created by Piotr Romanowski on 22/07/2023.
//

import WidgetKit
import SwiftUI

struct AttendanceReport: Codable {
    let totalPercentage: Float?
}

struct AttendanceTimelineProvider: TimelineProvider {
    func placeholder(in context: Context) -> AttendanceEntry {
        AttendanceEntry(date: Date(), totalPercentage: 78.88)
    }

    func getSnapshot(in context: Context, completion: @escaping (AttendanceEntry) -> ()) {
        let entry = AttendanceEntry(date: Date(), totalPercentage: 78.88)
        completion(entry)
    }

    func getTimeline(in context: Context, completion: @escaping (Timeline<AttendanceEntry>) -> ()) {
        var entries: [AttendanceEntry] = []
        
        let jsonData = readWidgetData(fileName: "attendance-stats.json", defaultValue: AttendanceReport(totalPercentage: nil));
        
        entries.append(AttendanceEntry(date: Date(), totalPercentage: jsonData.totalPercentage))
        
        let timeline = Timeline(entries: entries, policy: .atEnd)
        completion(timeline)
    }
}

struct AttendanceEntry: TimelineEntry {
    let date: Date
    let totalPercentage: Float?
}

struct AttendanceWidgetEntryView : View {
    var entry: AttendanceTimelineProvider.Entry

    var body: some View {
        VStack(alignment: .trailing) {
                Spacer()
            HStack() {
                Spacer()
                VStack(alignment: .trailing) {
                    Text("Attendance")
                    
                    if let totalPercentage = entry.totalPercentage {
                        Text(String(format: "%.2f%%", totalPercentage)).font(.headline)
                    } else {
                        Text("No data").font(.headline)
                    }
                }.padding(10)
            }
        }.padding(.leading)
    }
}

struct AttendanceWidget: Widget {
    let kind: String = "AttendanceWidget"

    var body: some WidgetConfiguration {
        StaticConfiguration(kind: kind, provider: AttendanceTimelineProvider()) { entry in
            AttendanceWidgetEntryView(entry: entry)
        }
        .supportedFamilies([.systemSmall])
        .configurationDisplayName("Attendance")
        .description("Overall attendance")
    }
}

struct AttendanceWidget_Previews: PreviewProvider {
    static var previews: some View {
        AttendanceWidgetEntryView(entry: AttendanceEntry(date: Date(), totalPercentage: 78.88))
            .previewContext(WidgetPreviewContext(family: .systemSmall))
    }
}
