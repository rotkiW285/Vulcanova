//
//  WidgetJsonDataReader.swift
//  NativeWidgetExtension
//
//  Created by Piotr Romanowski on 27/07/2023.
//

import Foundation

func readWidgetData<T: Decodable>(fileName: String, defaultValue: T) -> T {
    if let url = FileManager.default.containerURL(forSecurityApplicationGroupIdentifier: "group.io.github.vulcanova") {
        let path = url.appendingPathComponent(fileName)
        let data = try? String(contentsOf: path)
        if let data = data {
            let jsonData = data.data(using: .utf8)!
            do {
                let decoder = JSONDecoder()
                decoder.dateDecodingStrategy = .iso8601
                return try decoder.decode(T.self, from: jsonData)
            } catch {
                print(error)
                return defaultValue
            }
        }
    }
    return defaultValue
}
