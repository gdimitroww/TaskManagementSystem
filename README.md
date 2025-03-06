# Task Management System

A modern, containerized task management application built with ASP.NET Core, Entity Framework Core, and Microsoft SQL Server. This application allows users to create, manage, and track tasks with different priorities and due dates.

![Task Management System](screenshots/home-screen.png)

## Features

- User Authentication and Authorization
  - Email confirmation
  - Password reset functionality
  - Google OAuth sign-in
- Task Management
  - Create, edit, view, and delete tasks
  - Set priority levels (Low, Medium, High)
  - Track due dates
  - Filter and sort tasks
- Responsive UI built with Bootstrap
- Email notifications via SMTP (configurable with Gmail, Mailtrap, etc.)
- Docker containerization for easy deployment

## Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop/) and Docker Compose
- Git (for cloning the repository)

## Installation

### Clone the repository

```bash
git clone https://github.com/gdimitroww/TaskManagementSystem.git
cd TaskManagementSystem
```

### Configure Email Settings

The application uses email for account confirmation and password reset. You need to configure an email provider before running the application:

1. Edit `docker-compose.yml` to update the email settings:

```yaml
- EmailSettings__SmtpServer=smtp.mailtrap.io    # Change to your SMTP server
- EmailSettings__SmtpPort=2525                  # Change to your SMTP port
- EmailSettings__SmtpUsername=your_username     # Change to your SMTP username
- EmailSettings__SmtpPassword=your_password     # Change to your SMTP password
- EmailSettings__SenderEmail=noreply@taskmanagementsystem.com  # Change to your sender email
```

### Run the application

```bash
docker-compose up -d
```

This will:
1. Start a SQL Server container
2. Build and start the Task Management application
3. Create database tables and seed initial data

### Access the application

Open your browser and navigate to:
```
http://localhost:8080
```

## Alternative Email Configurations

### Using Gmail

To use Gmail for sending emails:

1. Enable 2-Step Verification in your Google Account
2. Create an App Password at https://myaccount.google.com/apppasswords
3. Use the following configuration in `docker-compose.yml`:

```yaml
- EmailSettings__SmtpServer=smtp.gmail.com
- EmailSettings__SmtpPort=587
- EmailSettings__SmtpUsername=your.email@gmail.com
- EmailSettings__SmtpPassword=your-16-char-app-password
- EmailSettings__SenderEmail=your.email@gmail.com
- EmailSettings__EnableSsl=true
```

### Using Mailtrap (for development/testing)

[Mailtrap](https://mailtrap.io/) is a test mail server solution that simulates sending and receiving emails without delivering to real email addresses.

1. Sign up for a free Mailtrap account
2. Get your SMTP credentials from the Mailtrap inbox
3. Use those credentials in the `docker-compose.yml` file

## Usage

### Creating an Account

1. Click on "Register" in the top right corner
2. Fill out the registration form
3. Confirm your email address through the link sent to your email

### Managing Tasks

1. Log in to your account
2. Click "Create New Task" to add a task
3. Set a title, description, due date, and priority
4. View all your tasks on the dashboard
5. Edit or delete tasks as needed

### Password Reset

1. Click "Forgot your password?" on the login page
2. Enter your email address
3. Check your email for the password reset link
4. Create a new password

## Development

If you want to run the application without Docker for development purposes:

1. Install .NET 9 SDK
2. Install SQL Server
3. Update the connection string in `appsettings.json`
4. Run the application using:

```bash
cd TaskManagementSystem
dotnet run
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
