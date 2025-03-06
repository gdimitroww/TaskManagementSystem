# Task Management System

A modern, responsive web application for managing tasks with user authentication, email notifications, and role-based permissions.

![Task Management System](https://i.imgur.com/placeholder.png) <!-- You can replace this with an actual screenshot -->

## Features

- **User Authentication**
  - Email/Password registration and login
  - Google OAuth integration
  - Email verification
  - Password reset functionality

- **Task Management**
  - Create, view, edit, and delete tasks
  - Task prioritization (Low, Medium, High)
  - Task status tracking (To Do, In Progress, Done)
  - Due date management
  - Task filtering and sorting

- **Role-Based Permissions**
  - Admin users can view and manage all tasks
  - Regular users can see all tasks but only manage their own
  - Task ownership tracking

- **Modern UI**
  - Responsive design for mobile and desktop
  - Dark theme
  - Interactive task actions menu
  - Status and priority indicators with color coding

- **Email Notifications**
  - SendGrid integration for reliable email delivery
  - Account verification emails
  - Password reset functionality

## Technology Stack

- **Backend**
  - ASP.NET Core 9.0
  - Entity Framework Core
  - SQL Server
  - Identity Framework for authentication

- **Frontend**
  - Bootstrap 5
  - JavaScript
  - Responsive design

- **Authentication**
  - ASP.NET Core Identity
  - Google OAuth integration

- **Email Service**
  - SendGrid for email notifications

- **Deployment**
  - Docker containerization
  - Docker Compose for orchestration

## Getting Started

### Prerequisites

- Docker and Docker Compose
- .NET SDK 9.0 (for development)
- SQL Server (handled by Docker)

### Installation

1. Clone the repository
   ```bash
   git clone https://github.com/gdimitroww/TaskManagementSystem.git
   cd TaskManagementSystem
   ```

2. Update settings in `docker-compose.yml`
   - Set up your SendGrid API key and email settings
   - Configure Google OAuth credentials (if needed)

3. Build and start the application
   ```bash
   docker-compose build
   docker-compose up -d
   ```

4. Access the application at `http://localhost:8080`

## Usage

### User Registration and Authentication

1. Create a new account using the registration page
2. Confirm your email address via the confirmation link
3. Log in with your credentials
4. Alternatively, use Google Sign-in for quick access

### Managing Tasks

1. View all tasks on the main dashboard
2. Create new tasks using the "Create New Task" button
3. Filter tasks by status using the dropdown menu
4. Use the "Actions" button to:
   - View task details
   - Edit your own tasks
   - Delete your own tasks
   - Change task status

### Admin Features

Administrators can:
- View all tasks in the system
- Edit or delete any task regardless of ownership
- See which user created each task

## Project Structure

- `/Controllers` - MVC controllers that handle HTTP requests
- `/Models` - Data models and view models
- `/Views` - Razor views for the UI
- `/Services` - Service classes including email functionality
- `/Data` - Database context and configuration

## Configuration

### Email Settings

Email functionality is configured through environment variables in the `docker-compose.yml` file:

```yaml
- EmailSettings__SmtpServer=smtp.sendgrid.net
- EmailSettings__SmtpPort=587
- EmailSettings__SmtpUsername=apikey
- EmailSettings__SmtpPassword=YOUR_SENDGRID_API_KEY
- EmailSettings__SenderEmail=your-email@example.com
- EmailSettings__SenderName=Task Management System
```

### Google Authentication

Google authentication is configured through environment variables:

```yaml
- Authentication__Google__ClientId=YOUR_GOOGLE_CLIENT_ID
- Authentication__Google__ClientSecret=YOUR_GOOGLE_CLIENT_SECRET
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Bootstrap for the front-end framework
- SendGrid for email services
- ASP.NET Core team for the excellent framework 