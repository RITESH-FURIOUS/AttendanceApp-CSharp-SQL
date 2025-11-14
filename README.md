<h1 align="center">📚 Attendance Management System — C# + SQL (SQLite)</h1>

<p align="center">
A clean, modern and fully working Attendance App built using <b>C# (.NET)</b> and <b>SQLite</b>.  
Perfect for demonstrating Hands-on Skills for SQL and C# in Cognizant SkillPro or any skill assessment.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Language-C%23-blue?style=for-the-badge">
  <img src="https://img.shields.io/badge/Database-SQLite-green?style=for-the-badge">
  <img src="https://img.shields.io/badge/Framework-.NET%206%2F7-purple?style=for-the-badge">
  <img src="https://img.shields.io/badge/Editor-VS%20Code-orange?style=for-the-badge">
</p>

---

## 🚀 Overview

This is a **Console-based Attendance Management System** that maintains student records and attendance using an embedded **SQLite SQL database**.  
It is simple, lightweight, and fully functional — built to demonstrate clear, practical hands-on experience in:

- C# programming  
- SQL table design, constraints & CRUD operations  
- SQLite database handling  
- Connecting C# to SQL using `Microsoft.Data.Sqlite`  
- Building menu-driven console applications  
- Data validation & persistence  

This project is ideal for **Cognizant SkillPro (C#, SQL)** submissions.

---

## ⭐ Features

- 👨‍🎓 Add new students (Roll Number + Name)  
- 📋 View all registered students  
- 🗓️ Mark attendance (Present/Absent)  
- 🔁 Automatically update existing attendance  
- 📅 Display attendance report for any day  
- 🗃️ SQLite database auto-created (`attendance.db`)  
- 🧱 SQL constraints applied:
  - `PRIMARY KEY`
  - `UNIQUE`
  - `FOREIGN KEY`

---

## 🗂️ Folder Structure

AttendanceApp/
│── Program.cs
│── AttendanceApp.csproj
│── attendance.db (auto-created after first run)
│── README.md
└── bin/
└── obj/ (build files)

pgsql
Copy code

---

## 🧠 Database Schema (SQL)

```sql
CREATE TABLE Students (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Roll TEXT NOT NULL UNIQUE,
    Name TEXT NOT NULL
);

CREATE TABLE Attendance (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    StudentId INTEGER NOT NULL,
    Date TEXT NOT NULL,
    Status TEXT NOT NULL,
    UNIQUE(StudentId, Date),
    FOREIGN KEY(StudentId) REFERENCES Students(Id)
);
✔ Automatically created by the C# program
✔ Prevents duplicate attendance for the same date
✔ Maintains correct student → attendance mapping
```
▶️ How to Run the Project
1️⃣ Restore required packages
bash
Copy code
dotnet restore
2️⃣ Run the application
bash
Copy code
dotnet run
3️⃣ Console Menu Options
Option	Description
1	Add Student
2	List Students
3	Mark Attendance (P / A / skip)
4	Show Attendance Report
5	Exit

🛠️ Tech Stack Used
Component	Purpose
C# (.NET)	Application logic
SQLite	Local SQL storage
Microsoft.Data.Sqlite	Database connector
VS Code	Development environment

🔍 Why This Project Is Perfect for Skill Validation
✔ SQL Skills Demonstrated


<h3 align="center">✨ Thanks for exploring this project — Happy Coding! 🚀</h3> ```
