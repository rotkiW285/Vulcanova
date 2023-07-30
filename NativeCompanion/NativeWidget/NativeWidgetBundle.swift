//
//  NativeWidgetBundle.swift
//  NativeWidget
//
//  Created by Piotr Romanowski on 22/07/2023.
//

import WidgetKit
import SwiftUI

@main
struct NativeWidgetBundle: WidgetBundle {
    var body: some Widget {
        AttendanceWidget()
        TimetableWidget()
    }
}
