# HARD PROMPT FOR AI: READ, IMPLEMENT, VERIFY

You are a senior full-stack engineer working directly inside this codebase:

- `D:\A\QuanLyCongViec`

Your task is not to brainstorm, discuss, or produce a high-level proposal only.

Your task is to:

1. read the required report
2. inspect the real code
3. implement the refactor in code
4. verify the behavior
5. summarize the actual changes made

If you stop at analysis or planning, you have failed the task.

## 1. Single Source Of Truth

Before doing anything else, read this file:

- `D:\A\QuanLyCongViec\ADMIN_PROJECT_SETTINGS_REPORT_EN.md`

This English report is the single source of truth for:

- current-state analysis
- permission analysis
- business logic analysis
- target architecture
- implementation phases

Do not ask for a second report.
Do not rely on assumptions when the report already provides direction.

## 2. Core Refactor Goal

Refactor the codebase so that:

- `Admin` is a true system-level area
- `Project Settings` is a true project-level area
- the project gear button opens a real project settings module
- project settings access is governed by project-level permissions
- admin access is governed by system-level permissions
- frontend UI, routing, and backend authorization follow the same rules

## 3. Current Problem You Must Fix

The codebase currently has a broken design:

- users expect the gear button on a project card to open `Project Settings`
- the current implementation sends users to `Admin > Configuration`
- `/space/:id/settings` does not render a real project settings page
- authorization is inconsistent across UI, router, and backend
- system-level and project-level permissions are mixed

You must fix this in the code.

## 4. Required Working Outcome

When you finish, the following must be true:

1. `/space/:id/settings` renders a real project settings page.
2. The project gear button no longer redirects into admin configuration.
3. `Admin access` and `Project Settings access` are separated.
4. Users with project-management rights can access project settings without needing system admin access.
5. Unauthorized users are blocked at both frontend and backend levels.
6. Admin menus, route guards, and backend protections are consistent.

## 5. Files You Must Inspect

At minimum, inspect these files before editing:

Frontend:

- `Frontend/src/views/ManageSpaces.vue`
- `Frontend/src/router/spaceRoutes.js`
- `Frontend/src/components/SettingsDropdown.vue`
- `Frontend/src/router/adminRoutes.js`
- `Frontend/src/router/index.js`
- `Frontend/src/views/admin/Configuration.vue`
- `Frontend/src/components/layout/AdminSidebar.vue`
- `Frontend/src/components/layout/AdminLayout.vue`
- `Frontend/src/components/CreateSpaceModal.vue`
- `Frontend/src/components/layout/NexusSidebar.vue`
- `Frontend/src/views/Dashboard.vue`

Backend:

- `Backend/src/TaskManagement.API/Filters/SystemAuthorizeAttribute.cs`
- `Backend/src/TaskManagement.API/Filters/ProjectAuthorizeAttribute.cs`
- `Backend/src/TaskManagement.API/Controllers/AdminUsersController.cs`
- `Backend/src/TaskManagement.API/Controllers/ProjectMembersController.cs`
- `Backend/src/TaskManagement.API/Controllers/SystemSettingsController.cs`
- `Backend/src/TaskManagement.Infrastructure/Services/ProjectService.cs`
- `Backend/src/TaskManagement.Application/DTOs/Project/ProjectDiscoveryDto.cs`

If additional files are required, inspect and modify them as needed.

## 6. Mandatory Design Rules

You must follow all of these rules.

### Rule 1

Treat these as different permission domains:

- `Admin access`
- `Project Settings access`

### Rule 2

`Admin` must remain system-level only.

It may contain:

- system settings
- organization settings
- instance settings
- security settings
- audit
- user administration

### Rule 3

`Project Settings` must be project-level only.

It may contain:

- General
- Members & Roles
- States
- Labels
- Modules
- Cycles
- Integrations
- Danger Zone

### Rule 4

Do not leave `/space/:id/settings` as a redirect into `/admin/configuration`.

### Rule 5

The new project settings page must use the actual current `projectId`.

### Rule 6

Do not trust frontend visibility alone.

Backend protection must remain active and must be aligned with the new behavior.

### Rule 7

Do not return a proposal instead of code changes.

This task requires implementation.

## 7. Permission Direction You Should Implement

Unless the codebase contains a stronger and more correct existing pattern, implement this direction:

- `System Admin`: can access `Admin`, can override into `Project Settings`
- `Admin`: can access `Admin`, and may override project settings if consistent with the business model
- `PROJECT_MANAGER`: can access `Project Settings`
- `PROJECT_LEAD`: can access `Project Settings`
- `PM`, `PO`: can access `Project Settings` if they are valid managerial project roles in the existing logic
- `Developer`, regular member: no system admin access; no project settings access unless explicitly allowed
- `Guest`, `Stakeholder`: no project settings management access

If the actual codebase uses a different canonical role vocabulary, normalize carefully and consistently.

## 8. Required Execution Order

You must perform the work in this order.

### Step 1. Read

Read `ADMIN_PROJECT_SETTINGS_REPORT_EN.md` completely before changing code.

### Step 2. Inspect

Inspect the current routing, menu, gear-button behavior, and backend authorization implementation.

### Step 3. Implement

Implement the refactor in code.

This includes at minimum:

- a real `Project Settings` page or module
- a real route for `/space/:id/settings`
- corrected project gear button behavior
- corrected frontend permission logic
- corrected route protection
- corrected backend authorization behavior where needed
- cleanup of admin navigation and admin-only assumptions

### Step 4. Verify

Run the relevant checks that are available in the environment.

You must attempt verification.

At minimum, verify:

- route behavior
- role behavior
- no remaining fake redirect
- build/test output if runnable

If any verification cannot be completed, say exactly what could not be verified and why.

### Step 5. Report

After implementation, provide:

- a short summary of changes made
- the list of modified files
- the new access model
- any remaining risks

## 9. Concrete Acceptance Criteria

Your implementation is only acceptable if all of the following are true:

- clicking the project gear button opens project-scoped settings
- `/space/:id/settings` no longer routes into system admin configuration
- a valid project manager can access project settings without needing system admin rights
- an unauthorized member cannot access project settings
- system admin routes remain system-level
- admin menu visibility is consistent with admin route protection
- backend still rejects unauthorized settings operations

## 10. Strict Output Requirements

You must not give a vague answer.

Your final response must include:

1. what you changed
2. which files you changed
3. how the new Admin behavior works
4. how the new Project Settings behavior works
5. what you verified
6. what remains risky or incomplete

## 11. Failure Conditions

You have failed this task if you do any of the following:

- only analyze without implementing
- only write a plan
- leave `/space/:id/settings` redirecting into admin configuration
- keep the project gear button tied only to system-level access
- ignore backend authorization alignment
- skip verification without explicitly explaining why

## 12. Final Instruction

Read the English report.
Inspect the actual code.
Implement the refactor.
Verify the result.
Then report the real completed work.
