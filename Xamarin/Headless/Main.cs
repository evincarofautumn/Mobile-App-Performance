﻿using System;
using System.Collections.Generic;
using Xamarin.Shared;
using Xamarin.Test;

namespace Headless
{
	static class MainClass
	{
		static void Main (string[] args)
		{
			int n;
			if (args.Length != 1 || !Int32.TryParse (args [0], out n))
				return;

			String trackJSON = ""
				+ "{"
				+   "\"track\": {"
				+     "\"id\": \"1000\","
				+     "\"name\": \"Test Raceway\","
				+     "\"gates\": ["
				+       "{"
				+       "\"gate_type\": \"SPLIT\","
				+       "\"latitude\": \"37.451775\","
				+       "\"longitude\": \"-122.203657\","
				+       "\"bearing\": \"136\","
				+       "\"split_number\": \"1\""
				+       "},"
				+       "{"
				+       "\"gate_type\": \"SPLIT\","
				+       "\"latitude\": \"37.450127\","
				+       "\"longitude\": \"-122.205499\","
				+       "\"bearing\": \"326\","
				+       "\"split_number\": \"2\""
				+       "},"
				+       "{"
				+       "\"gate_type\": \"START_FINISH\","
				+       "\"latitude\": \"37.452602\","
				+       "\"longitude\": \"-122.207069\","
				+       "\"bearing\": \"32\","
				+       "\"split_number\": \"3\""
				+       "}"
				+     "]"
				+   "}"
				+ "}";

			Track track;
			List<Point> points;

			track = Track.Load(trackJSON)[0];

			string contents = System.IO.File.ReadAllText("Assets/multi_lap_session.csv");
			string[] lines = contents.Split('\n');
			points = new List<Point>();
			foreach(string line in lines) {
				string[] parts = line.Split(',');
				points.Add(new Point(
					latitude: Double.Parse(parts[0]),
					longitude: Double.Parse(parts[1]),
					speed: Double.Parse(parts[2]),
					bearing: Double.Parse(parts[3]),
					horizontalAccuracy: 5.0,
					verticalAccuracy: 15.0,
					timestamp: 0));
			}

			TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
			int start = (int)t.TotalSeconds;
			int startTime = start;
			for (int i = 0; i < n; i++) {
				t = DateTime.UtcNow - new DateTime(1970, 1, 1);
				SessionManager.Instance().Start(track);
				foreach(Point point in points) {
					SessionManager.Instance().GPS(point.LatitudeDegrees(), longitude: point.LongitudeDegrees(), speed: point.speed, bearing: point.bearing, horizontalAccuracy: point.hAccuracy, verticalAccuracy: point.vAccuracy, timestamp: startTime);
					startTime += 1;
				}
				SessionManager.Instance().End();
			}
			t = DateTime.UtcNow - new DateTime(1970, 1, 1);
			Console.WriteLine(Math.Round(t.TotalSeconds - start, 3));

		}

	}
}

