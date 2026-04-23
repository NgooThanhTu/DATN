# Complete Report: Admin, Project Settings, Authorization, Dashboard/Menu, and Delivery Phases

## 1. Document Purpose

This document summarizes the current state of the `D:\A\QuanLyCongViec` codebase, with a focus on:

- The current `Admin` area.
- The `gear button` shown on each project card.
- How users access `Admin` and `Project Settings`.
- The current authorization model.
- The mismatch between current behavior and the intended design.
- A phased implementation plan suitable for an AI agent or a development team.

This report does **not** analyze Plane's original source code because the current workspace does not contain Plane's repository. All conclusions below are based entirely on the `QuanLyCongViec` codebase.

---

## 2. Executive Summary

### 2.1. Short Conclusion

In the current system:

- `Admin` is a system-level / organization-level management area.
- A real `Project Settings` module does not exist yet.
- The `gear button` on each project card does **not** open a true project settings page.
- Instead, it redirects to `Admin Configuration`.
- Authorization exists, but it is not fully aligned across frontend UI, router guards, and backend APIs.
- The system currently mixes `system admin access` and `project admin access`.

### 2.2. Design Conclusion

The correct target design should be:

- `Admin` and `Project Settings` must be separated.
- `Admin` should use `system roles`.
- `Project Settings` should use `project roles`.
- The project gear button should open a dedicated settings area for that specific project.
- Global admin routes must not be reused as a replacement for project settings.

---

## 3. Current System State

## 3.1. What the project gear button currently does

Primary files:

- [ManageSpaces.vue](/d:/A/QuanLyCongViec/Frontend/src/views/ManageSpaces.vue:80)
- [spaceRoutes.js](/d:/A/QuanLyCongViec/Frontend/src/router/spaceRoutes.js:43)

### Current flow

In [ManageSpaces.vue](/d:/A/QuanLyCongViec/Frontend/src/views/ManageSpaces.vue:80), each project card includes a gear button:

- Clicking it calls `goToAdmin(space.id)`.
- That function routes the user to `/space/{projectId}/settings`.

However, in [spaceRoutes.js](/d:/A/QuanLyCongViec/Frontend/src/router/spaceRoutes.js:43):

- `/space/:id/settings` does not render a dedicated project settings page.
- It redirects to `/admin/configuration`.
- It passes `query.projectId = to.params.id`.

### Current problem

The page [Configuration.vue](/d:/A/QuanLyCongViec/Frontend/src/views/admin/Configuration.vue:1):

- does not read `route.query.projectId`
- does not behave differently per project
- is fundamentally a system configuration page

Conclusion:

- Clicking the project gear button does **not** open `Project Settings`
- it effectively opens `System Admin Configuration`

---

## 3.2. What Admin currently is

The current admin area is built around:

- [SettingsDropdown.vue](/d:/A/QuanLyCongViec/Frontend/src/components/SettingsDropdown.vue:1)
- [AdminSidebar.vue](/d:/A/QuanLyCongViec/Frontend/src/components/layout/AdminSidebar.vue:1)
- [adminRoutes.js](/d:/A/QuanLyCongViec/Frontend/src/router/adminRoutes.js:1)
- [AdminLayout.vue](/d:/A/QuanLyCongViec/Frontend/src/components/layout/AdminLayout.vue:1)

### Current admin pages

- `Audit Log`
- `User Management`
- `Configuration`
- `Instance > General settings`
- `Instance > Authentication`
- `Instance > Email / SMTP`
- `Security > Two-Factor Auth`
- `Security > Change Password`
- `Security > IP Whitelist`

Organization-level pages also exist:

- [OrganizationProfile.vue](/d:/A/QuanLyCongViec/Frontend/src/views/admin/OrganizationProfile.vue:1)
- [OrganizationContact.vue](/d:/A/QuanLyCongViec/Frontend/src/views/admin/OrganizationContact.vue:1)

But the admin sidebar does not fully expose them because the organization submenu is currently commented out in [AdminSidebar.vue](/d:/A/QuanLyCongViec/Frontend/src/components/layout/AdminSidebar.vue:27).

---

## 3.3. What the dashboard and main menu currently contain

### Main dashboard

File:

- [Dashboard.vue](/d:/A/QuanLyCongViec/Frontend/src/views/Dashboard.vue:1)

Current dashboard features:

- project overview
- project counters
- favorites counter
- project search
- project cards
- quick work item creation

### Main application sidebar

File:

- [NexusSidebar.vue](/d:/A/QuanLyCongViec/Frontend/src/components/layout/NexusSidebar.vue:1)

Current main navigation groups:

- `Home`
- `Drafts`
- `Your work`
- `Stickies`
- `Rewards`

Favorites:

- favorite cycles / sprints

Workspace:

- `Projects`
- `More`

Inside `More`:

- `Views`
- `Analytics`
- `Archives`

Inside each project:

- `Work items`
- `Cycles`
- `Modules`
- `Views`
- `Pages`

Conclusion:

- The main dashboard and working navigation are already fairly complete.
- However, `Project Settings` is missing from the project navigation model.

---

## 4. Current Authorization Analysis

## 4.1. Admin access authorization

### Frontend button-level

File:

- [SettingsDropdown.vue](/d:/A/QuanLyCongViec/Frontend/src/components/SettingsDropdown.vue:72)

The global admin gear button is only shown if the user has one of these roles:

- `System Admin`
- `PM`
- `PO`

### Frontend route-level

File:

- [adminRoutes.js](/d:/A/QuanLyCongViec/Frontend/src/router/adminRoutes.js:1)

Admin routes currently allow a broader list:

- `SuperAdmin`
- `Admin`
- `System Admin`
- `Organization Admin`
- `AccessAdmin`
- `PM`
- `PO`
- `PROJECT_MANAGER`
- `Developer`
- `DEV`

### Frontend router guard

File:

- [router/index.js](/d:/A/QuanLyCongViec/Frontend/src/router/index.js:23)

The router guard reads:

- `localStorage.user.systemRoles`
- and compares them to `meta.requiredRoles`

### Backend API-level

File:

- [SystemAuthorizeAttribute.cs](/d:/A/QuanLyCongViec/Backend/src/TaskManagement.API/Filters/SystemAuthorizeAttribute.cs:1)

Some admin controllers use this filter, for example:

- [AdminUsersController.cs](/d:/A/QuanLyCongViec/Backend/src/TaskManagement.API/Controllers/AdminUsersController.cs:15)

### Conclusion

Admin authorization exists, but:

- UI visibility, route guards, and backend checks are not fully aligned
- some places allow `Developer / DEV`
- other places do not
- this creates inconsistency between "seeing the button", "opening the route", and "calling the API"

---

## 4.2. Project Settings authorization

### Current state

There is no real `ProjectSettings.vue`.

The project gear button currently checks `systemRoles` in:

- [ManageSpaces.vue](/d:/A/QuanLyCongViec/Frontend/src/views/ManageSpaces.vue:114)

Roles currently allowed to click the project gear:

- `System Admin`
- `Admin`
- `PM`
- `PO`
- `admin`

### Problem

This is not the correct way to protect project settings because:

- `Project Settings` should be based on `project role`
- not primarily on `system role`

### Actual project-level authorization that already exists in backend

File:

- [ProjectAuthorizeAttribute.cs](/d:/A/QuanLyCongViec/Backend/src/TaskManagement.API/Filters/ProjectAuthorizeAttribute.cs:1)

This filter:

- reads `projectId` from the route
- loads `ProjectMembers`
- checks `ProjectRole`
- blocks inactive or unauthorized members
- blocks `Guest` and `Stakeholder` from write methods

Example controller using this filter:

- [ProjectMembersController.cs](/d:/A/QuanLyCongViec/Backend/src/TaskManagement.API/Controllers/ProjectMembersController.cs:29)

Here:

- inviting members is limited to `PM, Admin`
- removing members is limited to `PM, Admin`
- changing member roles is limited to `PM, Admin`

### Conclusion

Project-level authorization already has a backend foundation.

However:

- the project gear button does not use that model yet
- there is no dedicated project settings route to apply project authorization properly

---

## 5. Current Project Creation Business Logic

Frontend file:

- [CreateSpaceModal.vue](/d:/A/QuanLyCongViec/Frontend/src/components/CreateSpaceModal.vue:1)

Backend file:

- [ProjectService.cs](/d:/A/QuanLyCongViec/Backend/src/TaskManagement.Infrastructure/Services/ProjectService.cs:431)

## 5.1. Project creation fields

The frontend currently lets users specify:

- `Project name`
- `Project ID / key`
- `Description`
- `Visibility`
- `Lead`
- `Cover`
- `Icon`

## 5.2. Backend business logic on project creation

The backend currently:

- creates the project record
- stores `cover/icon` inside `NavigationConfig`
- seeds default task statuses
- seeds task types depending on the template
- adds the creator to `ProjectMembers` as `PROJECT_MANAGER`
- if `leadUserId` differs from creator, adds `PROJECT_LEAD`
- if the lead is the creator, changes the creator role to `PROJECT_LEAD`

### Assessment

This is reasonable project bootstrap logic.

However, the next step is missing:

- there is still no dedicated `Project Settings` module to manage the created project afterward

---

## 6. Core Design Problems

The main design issues are:

### 6.1. `Admin` and `Project Settings` are mixed together

- `Admin` is system / organization configuration
- `Project Settings` should be project-specific configuration
- currently the project gear redirects into admin config

### 6.2. There is no dedicated `Project Settings` module

There is currently no:

- real route
- real page
- real tabbed project-specific settings UI

### 6.3. Authorization is inconsistent

Authorization is not aligned across:

- [ManageSpaces.vue](/d:/A/QuanLyCongViec/Frontend/src/views/ManageSpaces.vue:114)
- [SettingsDropdown.vue](/d:/A/QuanLyCongViec/Frontend/src/components/SettingsDropdown.vue:72)
- [adminRoutes.js](/d:/A/QuanLyCongViec/Frontend/src/router/adminRoutes.js:1)
- [SystemAuthorizeAttribute.cs](/d:/A/QuanLyCongViec/Backend/src/TaskManagement.API/Filters/SystemAuthorizeAttribute.cs:1)

### 6.4. No clean permission matrix exists

There is no single authoritative document defining:

- who can enter admin
- who can enter project settings
- who can manage members
- who can edit states/modules/labels/cycle rules

### 6.5. Incorrect business redirect

`/space/:id/settings` is not a real project settings route.

This is a clear business/design defect and should be fixed early.

---

## 7. Target Design

## 7.1. Separate the two areas

### Area 1: Admin

Scope:

- system / organization

Responsibilities:

- system-wide user management
- authentication management
- security configuration
- SMTP / email
- tenant / organization configuration
- audit logs

Route shape:

- `/admin/*`

Authorization basis:

- `system roles`

### Area 2: Project Settings

Scope:

- per project

Responsibilities:

- project member and role management
- project workflow states
- labels
- modules
- cycle rules
- project defaults
- project danger zone

Route shape:

- `/space/:id/settings`

Authorization basis:

- `project roles`

---

## 7.2. Recommended permission matrix

### System-level

- `SuperAdmin`: full admin access
- `System Admin`: full admin access
- `Organization Admin`: organization admin access
- `Admin`: optional admin access depending on policy

### Project-level

- `PROJECT_MANAGER`: can enter project settings
- `PROJECT_LEAD`: can enter project settings
- `PM`: can enter project settings
- `PO`: can enter project settings

### Should not enter project settings

- `DEV`
- `Guest`
- `Stakeholder`

### Override rule

- `System Admin` may override project settings access
- `SuperAdmin` may override all project scopes

---

## 7.3. Recommended Project Settings structure

Recommended page:

- `ProjectSettings.vue`

Recommended tabs:

1. `General`
2. `Members & Roles`
3. `Workflow States`
4. `Labels`
5. `Modules`
6. `Cycles / Sprint Rules`
7. `Views / Defaults`
8. `Danger Zone`

---

## 8. Recommended Implementation Phases

The recommended delivery plan is `5 phases`.

## Phase 1: Audit and architecture freeze

### Goal

Define clearly:

- what `Admin` is
- what `Project Settings` is
- which flows are system-level
- which flows are project-level

### Work items

- audit routes, buttons, guards, and APIs
- identify incorrect redirects
- finalize the permission matrix
- finalize the target UX

### Deliverables

- current state report
- target state report
- permission matrix

---

## Phase 2: Authorization normalization

### Goal

Align authorization across:

- buttons
- router guards
- backend APIs

### Work items

- separate `system admin guard`
- separate `project settings guard`
- normalize role lists
- stop using `systemRoles` as the primary protection for project settings

### Deliverables

- normalized frontend role checks
- normalized router guards
- normalized backend authorization

---

## Phase 3: Build real Project Settings

### Goal

Turn `/space/:id/settings` into a real project settings module.

### Work items

- create `ProjectSettings.vue`
- update `/space/:id/settings`
- remove redirect to `/admin/configuration`
- load project-specific data
- attach project-level authorization

### Deliverables

- complete project settings page
- correct navigation from project gear

---

## Phase 4: Clean up Admin

### Goal

Bring the admin area back to a proper system-level role.

### Work items

- remove project-specific behavior from admin configuration
- align `SettingsDropdown` and `AdminSidebar`
- clarify organization vs instance vs security sections

### Deliverables

- cleaner admin shell
- consistent admin menu

---

## Phase 5: QA, regression, and polish

### Goal

Ensure that authorization and navigation behavior are correct end-to-end.

### Work items

- test normal user flow
- test project manager / project lead flow
- test system admin flow
- test direct URL access
- test denied access cases
- test project gear
- test global admin gear

### Deliverables

- test checklist
- post-refactor bug list
- release-ready handoff

---

## 9. Delivery Notes for an AI Agent or Development Team

## 9.1. Core questions the AI must answer before implementation

1. Which users can enter `Admin`?
2. Which users can enter `Project Settings`?
3. Is `Project Settings` a dedicated route?
4. Is the project gear button opening the correct page?
5. Is authorization enforced at system level, project level, or both?

## 9.2. Mandatory implementation constraints

- Do not reuse `Admin Configuration` as a replacement for `Project Settings`
- Do not rely primarily on `systemRoles` for project settings protection
- All project settings APIs must use project-level authorization
- `/space/:id/settings` must render a real page
- UI, router, and backend must share the same permission model

---

## 10. Recommended File Structure After Refactor

### Frontend

- `Frontend/src/views/ProjectSettings.vue`
- updated `Frontend/src/router/spaceRoutes.js`
- `Frontend/src/components/project-settings/*`

### Backend

- `ProjectSettingsController.cs`, or separate domain-specific controllers such as:
  - `ProjectMembersController`
  - `ProjectWorkflowController`
  - `ProjectLabelsController`
  - `ProjectModulesController`
  - `ProjectCyclesSettingsController`

### Shared

- a shared permission / role constants file for consistent mapping

---

## 11. Completion Checklist

The refactor should be considered complete only when:

- the project gear opens real `Project Settings`
- the global admin gear opens real `Admin`
- users with project-level permission but no system admin role can still access project settings
- users with system admin role are not implicitly treated as project managers everywhere
- `/space/:id/settings` no longer redirects to `/admin/configuration`
- project settings APIs are protected by project-level authorization
- UI and backend use the same access model

---

## 12. Final Conclusion

The current system already has solid foundations for:

- global admin
- project membership
- project-level authorization
- dashboard and primary work navigation

But the largest missing piece is:

- a real `Project Settings` module
- the current misuse of `Admin Configuration` as a substitute for project settings
- the lack of a clean separation between admin-level and project-level access

If the codebase is refactored according to the 5 phases above, the system will become:

- clearer architecturally
- more correct from a business perspective
- easier to hand off to an AI agent or development team
- easier to evolve toward a Plane-like project management model in the future

