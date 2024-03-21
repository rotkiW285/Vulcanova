//
//  TimetableDataReader.swift
//  NativeWidgetExtension
//
//  Created by Piotr Romanowski on 05/02/2024.
//

import Foundation

struct TimetableDataElement: Codable {
    let key: Date
    let value: [TimetableDataLesson]
}

struct TimetableDataLesson: Codable {
    let no: Int
    let subjectName: String
    let date, start, end: Date
    let teacherName, roomName: String?
    let displayColor: ChangeDisplayColor
    let displayTextDecorations: ChangeDisplayTextDecorations
    
    enum ChangeDisplayColor: Int, Codable {
        case normal
        case yellow
        case red
    }
    
    enum ChangeDisplayTextDecorations: Int, Codable {
        case none
        case strikethrough
    }
}

typealias TimetableData = [TimetableDataElement]

func readTimetableData() -> TimetableData? {
    let data: TimetableData? = readWidgetData(fileName: "timetable.json", defaultValue: nil)
    return data
}
