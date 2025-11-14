using System;
using Microsoft.Data.Sqlite;
using System.Globalization;

class Program
{
    const string DbFile = "attendance.db";

    static void Main()
    {
        Console.WriteLine("Simple Attendance Console App");
        EnsureDatabase();

        while (true)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1) Add student");
            Console.WriteLine("2) List students");
            Console.WriteLine("3) Mark attendance");
            Console.WriteLine("4) Show attendance report");
            Console.WriteLine("5) Exit");
            Console.Write("Choose: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1": AddStudent(); break;
                case "2": ListStudents(); break;
                case "3": MarkAttendance(); break;
                case "4": ShowReport(); break;
                case "5": return;
                default: Console.WriteLine("Invalid option"); break;
            }
        }
    }

    static SqliteConnection GetConnection()
    {
        var cs = new SqliteConnection($"Data Source={DbFile}");
        cs.Open();
        return cs;
    }

    static void EnsureDatabase()
    {
        using var conn = GetConnection();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Students (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Roll TEXT NOT NULL UNIQUE,
                Name TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS Attendance (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StudentId INTEGER NOT NULL,
                Date TEXT NOT NULL,
                Status TEXT NOT NULL,
                FOREIGN KEY(StudentId) REFERENCES Students(Id),
                UNIQUE(StudentId, Date)
            );";
        cmd.ExecuteNonQuery();
    }

    static void AddStudent()
    {
        Console.Write("Enter roll number: ");
        var roll = Console.ReadLine()?.Trim();
        Console.Write("Enter name: ");
        var name = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(roll) || string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Invalid input.");
            return;
        }

        using var conn = GetConnection();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT OR IGNORE INTO Students (Roll, Name) VALUES ($roll, $name)";
        cmd.Parameters.AddWithValue("$roll", roll);
        cmd.Parameters.AddWithValue("$name", name);
        var count = cmd.ExecuteNonQuery();

        if (count > 0) Console.WriteLine("Student added.");
        else Console.WriteLine("Student with same roll already exists.");
    }

    static void ListStudents()
    {
        using var conn = GetConnection();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Roll, Name FROM Students ORDER BY Roll";
        using var reader = cmd.ExecuteReader();
        Console.WriteLine("\nStudents:");
        while (reader.Read())
        {
            Console.WriteLine($"{reader.GetInt32(0)} | {reader.GetString(1)} | {reader.GetString(2)}");
        }
    }

    static void MarkAttendance()
    {
        Console.Write("Enter date (yyyy-MM-dd) or leave empty for today: ");
        var dateInput = Console.ReadLine();
        var date = string.IsNullOrWhiteSpace(dateInput) ? DateTime.Today : DateTime.ParseExact(dateInput.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var dateStr = date.ToString("yyyy-MM-dd");

        using var conn = GetConnection();
        // list students
        var listCmd = conn.CreateCommand();
        listCmd.CommandText = "SELECT Id, Roll, Name FROM Students ORDER BY Roll";
        using var reader = listCmd.ExecuteReader();

        Console.WriteLine($"\nMarking attendance for {dateStr}. Type P for present, A for absent. Press Enter to skip student.");
        while (reader.Read())
        {
            var id = reader.GetInt32(0);
            var roll = reader.GetString(1);
            var name = reader.GetString(2);

            Console.Write($"{roll} - {name} (P/A/skip): ");
            var resp = Console.ReadLine()?.Trim().ToUpper();

            if (string.IsNullOrEmpty(resp)) continue;
            if (resp != "P" && resp != "A")
            {
                Console.WriteLine("Skipped (invalid).");
                continue;
            }

            var status = resp == "P" ? "Present" : "Absent";
            var upCmd = conn.CreateCommand();
            upCmd.CommandText = @"
                INSERT INTO Attendance (StudentId, Date, Status)
                VALUES ($sid, $date, $status)
                ON CONFLICT(StudentId, Date) DO UPDATE SET Status = excluded.Status;
            ";
            upCmd.Parameters.AddWithValue("$sid", id);
            upCmd.Parameters.AddWithValue("$date", dateStr);
            upCmd.Parameters.AddWithValue("$status", status);
            upCmd.ExecuteNonQuery();
            Console.WriteLine($"Set {name} -> {status}");
        }
    }

    static void ShowReport()
    {
        Console.Write("Enter date (yyyy-MM-dd) to view or leave empty for today: ");
        var dateInput = Console.ReadLine();
        var date = string.IsNullOrWhiteSpace(dateInput) ? DateTime.Today : DateTime.ParseExact(dateInput.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var dateStr = date.ToString("yyyy-MM-dd");

        using var conn = GetConnection();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT s.Roll, s.Name, COALESCE(a.Status, 'Not Marked') as Status
            FROM Students s
            LEFT JOIN Attendance a ON a.StudentId = s.Id AND a.Date = $date
            ORDER BY s.Roll;
        ";
        cmd.Parameters.AddWithValue("$date", dateStr);
        using var reader = cmd.ExecuteReader();
        Console.WriteLine($"\nAttendance for {dateStr}:");
        Console.WriteLine("Roll | Name | Status");
        while (reader.Read())
        {
            Console.WriteLine($"{reader.GetString(0)} | {reader.GetString(1)} | {reader.GetString(2)}");
        }
    }
}
