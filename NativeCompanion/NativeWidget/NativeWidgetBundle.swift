//
//  NativeWidgetBundle.swift
//  NativeWidget
//
//  Created by Piotr Romanowski on 22/07/2023.
//

import WidgetKit
import SwiftUI

@main
struct WidgetLauncher {
    static func main() {
        if #available(iOSApplicationExtension 17.0, *) {
            WidgetsBundle17.main()
        } else {
            WidgetsBundle16.main()
        }
    }
}

struct WidgetsBundle16 : WidgetBundle {
    var body: some Widget {
        AttendanceWidget()
        TimetableWidget()
    }
}

@available(iOSApplicationExtension 17.0, *)
struct WidgetsBundle17 : WidgetBundle {
    var body: some Widget {
        AttendanceWidget()
        TimetableWidget()
        LessonsNowWidget()
    }
}
