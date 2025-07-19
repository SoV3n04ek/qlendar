Qlendar - Life Tracking Application

## Project Overview
**Qlendar** is a productivity application that combines task management, note-taking, and calendar scheduling to help users track their life achievements, habits, and daily activities.

## Technical Stack
- **Frontend**: Angular 16+ (TypeScript)
- **Backend**: ASP.NET Core 8 Web API (C#)
- **Database**: PostgreSQL 16 (Docker container)
- **Authentication**: JWT + OAuth2 (Google)
- **Deployment**: Docker + Docker Compose

## Architecture Decision
**Monolithic Architecture** (Recommended for this project):
- Single solution containing all components
- Modular separation within the project
- Potential to extract microservices later if needed

Reasons against microservices:
1. Development complexity outweighs benefits
2. Single team/solo developer
3. Tight integration between components
4. No need for independent scaling yet

## Core Features

### 1. Authentication Module
- JWT-based authentication
- Email/password login
- Google OAuth integration
- Email confirmation flow
- Password recovery

### 2. Todo Module
Features:

Todo items with subtasks

Folder organization (Business/Personal/Family)

Deadline management

Priority system

Progress tracking

### 3. Notes Module
Markdown support (with preview)

Tag-based organization

Full-text search

Version history

Export options (PDF/Markdown)

### 4. Calendar Module
Day view (24-hour blocks)

Week/Month views

Drag-and-drop scheduling

Integration with Todo items

Time blocking functionality

### Development Setup
\\Prerequisites//

* Docker Desktop

* .NET 8 SDK

* Node.js 18+

* PostgreSQL 16 (via Docker)