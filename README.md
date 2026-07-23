# WorkSync

Section 8 - CSE 325

## Team Members

* James Eberhard
* Kim Kathleen Bown
* Jean Rubin Oxeant
* Mario Leo Junior Meza Mancilla
* Daniel “Ezra” Rivera
* Charles Hemedi
* Natnael Gashaw

## Demo Accounts

These accounts are for demonstration and testing only.

| Role       | Username                                                | Password      | Tenant |
| ---------- | ------------------------------------------------------- | ------------- | ------ |
| Super User | [superuser@worksync.com](mailto:superuser@worksync.com) | SuperUser123! | System |
| Admin      | [admin@worksync.com](mailto:admin@worksync.com)         | Admin123!     | Brainstormers (`3735930`) |
| Leader     | [leader@worksync.com](mailto:leader@worksync.com)       | Leader123!    | Brainstormers (`3735930`) |
| Viewer     | [viewer@worksync.com](mailto:viewer@worksync.com)       | Viewer123!    | Brainstormers (`3735930`) |

## Overview

WorkSync is a company meeting and work-tracking app that helps leaders and teams organize workorders, assignments, employees, reports, and follow-up tasks in one place. Teams can track ownership, due dates, progress, meeting decisions, notes, and completion status.

The goal is to keep decisions and action items from getting lost in messages, meeting notes, spreadsheets, or memory. WorkSync is a class project adapted into a company-focused workflow for practicing .NET web development, team collaboration, authentication, CRUD functionality, reporting, and accessibility improvements.

## The Problem It Solves

Company leaders often manage many projects and commitments at the same time. Workorders, meeting action items, assignments, deadlines, and follow-up discussions can easily become disconnected across tools.

It is easy for something important to get missed if it only lives in a text thread, notebook, spreadsheet, or someone’s memory. WorkSync gives leaders a simple place to see what is open, who is assigned, what is overdue, and what has already been completed.

## Main Features

* Role-based login and navigation
* Tenant-isolated company workspaces and tenant-aware registration
* Super-user tenant request and administrator approval workflow
* Dashboard overview
* Workorders tracking
* Assignments tracking
* Follow-up tracking
* Employee directory CRUD
* Reports page
* Report copy/download export
* Viewer read-only access where needed
* Admin approval/user management
* Accessibility and contrast improvements

## User Roles

WorkSync uses role-based access so users only see the areas they are allowed to use.

### Admin

Admins have the highest level of access. They can manage users, view reports, manage employees, and access confidential or administrative information.

### Leader

Leaders can manage workorders, assignments, meeting follow-ups, employees, and reports.

### Viewer

Viewers can view allowed information but do not have full editing permissions. For example, Viewers can see the Employees page, but they cannot add, edit, delete, or view restricted employee status information.

## Web Pages

The web application includes several pages that work together to support communication, task management, and reporting.

### Home

The Home page is the main landing page. Guests can see the main features of the app before logging in. Logged-in users only see feature cards and links that match their role.

### Dashboard

The Dashboard gives users a quick overview of important items, such as open, completed, and overdue work.

### Workorders

The Workorders page tracks requests and operational work from creation through completion. Users can assign an employee, department, due date, status, notes, and follow-up requirement.

### Assignments

The Assignments page manages action items assigned during meetings. Users can track titles, descriptions, owners, departments, due dates, priorities, statuses, notes, and completion information.

### Follow-Ups

The Follow-Ups page tracks action items from meetings and project discussions. Follow-up items include an owner, related project or meeting topic, due date, status, confidentiality setting, and completion notes.

Confidential follow-ups are limited so they are not shown to users who should not have access.

### Employees

The Employees page is used to manage employee contact and assignment information. Admin and Leader users can add, edit, and delete employees. Viewer users can view the directory but cannot modify records or view restricted status information.

Employee fields include:

* First Name
* Last Name
* Email
* Phone
* Department
* Current Workorder
* Availability Notes
* Active Status

The Employees page also includes email validation so blank or invalid emails do not save.

### Reports

The Reports page summarizes workorders, assignments, meeting follow-ups, and employees. It also provides filters and a simple export feature for copying or downloading report text.

Confidential project information is masked in exported reports.

### Registration

The Registration page allows new users to create an account. Depending on the app settings, accounts may need admin approval before full access is granted.

### Login

The Login page allows users to sign in with their email and password. After logging in, users can access pages based on their assigned role.

### Admin User Management

The admin user management pages allow Admin users to review users, approve access, and manage user information.

## Application Navigation

WorkSync uses a navigation menu and role-based page access. A user can register or log in from the home page. After login, the user sees navigation options based on their role.

For example:

* Admin users can access administrative tools.
* Leader users can manage leadership work areas.
* Viewer users can only view approved pages and do not see editing controls.

This helps keep the interface cleaner and prevents users from accessing tools they should not use.

## Development Environment

The project was developed using the following tools:

* Visual Studio Code
* Git
* GitHub
* .NET
* ASP.NET Core
* Blazor
* Entity Framework Core
* PostgreSQL (Neon-compatible)
* Vercel container deployment
* Trello
* Microsoft Teams

## GitHub Repository

Repository link:

https://github.com/jneberhard/WorkSync

## Cloning the Repository

To clone the project:

1. Go to the GitHub repository.
2. Click the green Code button.
3. Copy the repository link.
4. Open Visual Studio Code.
5. Open the Command Palette.
6. Choose Clone Git Repository.
7. Paste the repository link.
8. Choose a folder on your computer.
9. Open the cloned project folder.

## Restore Packages

Open a terminal in the project folder and run:

```bash
dotnet restore
```

This restores the required NuGet packages.

## Database

WorkSync uses PostgreSQL through Entity Framework Core. A Neon PostgreSQL database is recommended for Vercel because Vercel containers are stateless and cannot safely persist a local SQLite file.

Set the connection string with the `ConnectionStrings__DefaultConnection` environment variable. For local development, either update the placeholder in `appsettings.Development.json` or use .NET user secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_NEON_CONNECTION_STRING"
```

To create or update the local database, run:

```bash
dotnet ef database update
```

The included `InitialPostgreSqlCreate` migration creates a fresh WorkSync schema. It does not copy data from the source application's SQLite database.

## Running the Program

To run the application locally, open a terminal in the project folder and run:

```bash
dotnet restore
dotnet build
dotnet ef database update
dotnet run
```

After the app starts, the terminal will show a local URL, such as:

```text
http://localhost:5131
```

Open that URL in a web browser to use the app.

## Deploying to Vercel

The repository includes `Dockerfile.vercel`, so Vercel can build and run the ASP.NET Core application as a container.

1. Create a Neon database and copy its connection string.
2. Import the GitHub repository into Vercel.
3. Add `ConnectionStrings__DefaultConnection` as a Vercel environment variable for Production and Preview.
4. Deploy. Vercel detects `Dockerfile.vercel` automatically.

The application applies Entity Framework migrations when it starts. For production use, replace the seeded demo passwords and configure a real email sender before enabling public registration.

## Project Management

Trello was used as the project management and planning tool. The team used Trello cards to organize tasks, track progress, assign responsibilities, and document completed work.

Trello link:

https://trello.com/b/UkTMelSr/worksync

## Programming Language and Frameworks

### C# and .NET

C# was the main programming language used for the project. The .NET platform provided the runtime and tools needed to build and run the application.

### ASP.NET Core

ASP.NET Core was used to build the web application, handle routing, authentication, services, and server-side functionality.

### Blazor

Blazor was used to build the user interface with reusable components and C# code. This helped create interactive pages while keeping the project inside the .NET ecosystem.

### Entity Framework Core

Entity Framework Core is used for database access and migrations. It connects the app to PostgreSQL and supports employee records, assignments, workorders, and meeting follow-ups.

### PostgreSQL and Neon

PostgreSQL stores the application data outside the Vercel container. Neon provides a managed PostgreSQL service and supplies the connection string used by WorkSync.

## Accessibility Work

The team worked on accessibility and contrast improvements during development. This included:

* Adding useful page titles.
* Adding labels for form fields.
* Adding labels for search inputs and filter dropdowns.
* Improving color contrast on buttons, badges, navigation, and important page areas.
* Fixing contrast issues found with WAVE and Chrome DevTools.
* Improving keyboard/accessibility behavior where needed.

## User Guide

### Logging In

1. Open WorkSync in your browser.
2. Click Login.
3. Enter your email and password.
4. Click Sign In.
5. After login, use the navigation menu to access the pages available to your role.

### Using the Dashboard

The Dashboard provides a quick overview of important work.

Users can review:

* Open items
* Completed items
* Overdue items
* Quick links to important pages

### Managing Workorders

To manage workorders:

1. Open the Workorders page.
2. Review the list of workorders.
3. Add, edit, or delete workorder records if your role has access.
4. Use status and notes to keep workorder information updated.

### Managing Assignments

To manage assignments:

1. Open the Assignments page.
2. Review current assignments.
3. Create a new assignment if needed.
4. Edit assignment details or status.
5. Mark assignments completed when finished.
6. Delete assignments only when needed.

### Managing Follow-Ups

To manage follow-ups:

1. Open the Follow-Ups page.
2. Review open follow-up items.
3. Create a new follow-up item.
4. Assign an owner and due date.
5. Update the status as progress is made.
6. Mark sensitive follow-ups confidential when access should be limited.

### Managing Employees

To manage employees:

1. Open the Employees page.
2. View the employee directory.
3. Use search or filters if needed.
4. Add an employee if your role has access.
5. Edit employee information if needed.
6. Delete an employee only after confirming through the delete modal.

Viewer users can view the employee directory but cannot add, edit, delete, or view restricted employee status.

### Viewing Reports

To view reports:

1. Open the Reports page.
2. Review the available report summaries.
3. Apply filters if needed.
4. Use Copy Report or Download Report to export report information.

### Logging Out

To log out:

1. Click Logout.
2. Confirm you are signed out.
3. Close the browser if using a shared computer.

## Troubleshooting

### Unable to Log In

* Check the email and password.
* Make sure the account exists.
* Make sure the account has been approved if approval is required.
* Contact an Admin if the issue continues.

### Missing Data

* Refresh the page.
* Make sure the item was saved.
* Confirm you are logged in with the correct role.

### Database Issues

If the database does not load correctly during local development, run:

```bash
dotnet ef database update
```

If a development database is disposable and its schema is out of sync, recreate that PostgreSQL database and run the update command again. Do not recreate a production database that contains data you need to keep.

### Application Errors

* Refresh the browser.
* Stop and restart the app.
* Run `dotnet build` to check for compile errors.
* Check the terminal output for error messages.

## Final Video

Final video link: [To be added before final submission.](https://youtu.be/qTAyHuUp6l4)

## Notes

WorkSync was created as a CSE 325 group project to practice .NET development, team collaboration, CRUD functionality, role-based access, database usage, reporting, and accessibility improvements.
