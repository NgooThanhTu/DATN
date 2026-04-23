# Admin And Project Settings Analysis Report

## 1. Executive Summary

This document provides a complete analysis of the current `Admin` area, the `Project Settings` access flow, the per-project gear button behavior, the current permission model, the relevant business logic, the dashboard and menu structure, and a phased implementation plan that an AI or engineering team can follow.

The most important conclusions are:

- In the current `D:\A\QuanLyCongViec` codebase, `Admin access` and `Project Settings access` should be treated as two different permission types.
- The current implementation mixes those two concepts.
- The gear button inside each project does not open a real `Project Settings` page. It redirects into `Admin > Configuration`.
- Authorization exists, but it is inconsistent across frontend UI, router guards, and backend APIs.

## 2. Scope And Limitations

This report is based only on the source code currently available in `D:\A\QuanLyCongViec`.

Limitations:

- The original Plane source code is not present in this repository.
- Because of that, any comparison with Plane can only be conceptual or UX-oriented. It cannot claim exact Plane implementation details from source evidence inside this repository.

## 3. Report Objectives

This report answers the following questions:

- What `Admin` currently means in this application.
- How users currently enter `Admin`.
- What `Project Settings` currently means in this application.
- How users currently enter `Project Settings`.
- Whether the project gear button opens the correct destination.
- How authorization currently works.
- What the current dashboard and menu structure contains.
- What project creation currently seeds and configures.
- How to split the remediation work into phases for an AI or developer team.

## 4. Current State Overview

### 4.1. Current State Conclusion

The system currently has:

- a fairly clear system-level `Admin` area
- a gear button on each project card
- a route at `/space/:id/settings`

However, `/space/:id/settings` does not render a dedicated `Project Settings` page. Instead, it redirects to `/admin/configuration` and appends a `projectId` query parameter. The `Configuration` page does not use that `projectId`, so this is not a true project settings experience.

### 4.2. What The Business Meaning Should Be

From a product design perspective:

- `Admin` should represent system-level or organization-level management
- `Project Settings` should represent management of one specific project

Those areas should be separated because:

- their scope is different
- their intended audience is different
- the risk and impact of each configuration change is different

## 5. How Users Currently Access Admin

### 5.1. From The Global Settings Dropdown

Frontend settings dropdown:

- [Frontend/src/components/SettingsDropdown.vue](Frontend/src/components/SettingsDropdown.vue)

Inside this component:

- `canAccessAdmin` is computed from `currentUser.systemRoles`
- the currently allowed roles are `System Admin`, `PM`, and `PO`

This entry point can navigate to:

- `/admin/audit-log`
- `/admin/users`
- `/admin/configuration`
- `/admin/instance/general`
- `/admin/instance/authentication`
- `/admin/instance/email`
- `/admin/security/2fa`
- `/admin/security/change-password`
- `/admin/security/ip-whitelist`
- some organization/customization items

### 5.2. Direct Route Access

Admin routes are declared in:

- [Frontend/src/router/adminRoutes.js](Frontend/src/router/adminRoutes.js)

The route-level allowed roles are broader than the dropdown visibility rule:

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

This is already a sign that UI-level permission logic and route-level permission logic are not aligned.

### 5.3. Router Guard

The router guard is located in:

- [Frontend/src/router/index.js](Frontend/src/router/index.js)

It reads:

- the auth token
- `user.systemRoles` from local storage
- the route `meta.requiredRoles`

If the user does not have the expected role, access is blocked.

### 5.4. Backend Authorization For Admin

The backend has:

- [Backend/src/TaskManagement.API/Filters/SystemAuthorizeAttribute.cs](Backend/src/TaskManagement.API/Filters/SystemAuthorizeAttribute.cs)

This filter:

- loads the real user from the database
- reads actual system roles
- enforces permissions on the backend rather than trusting frontend visibility

Example usage:

- [Backend/src/TaskManagement.API/Controllers/AdminUsersController.cs](Backend/src/TaskManagement.API/Controllers/AdminUsersController.cs)

This confirms that `Admin` is already treated as a system-level concept.

## 6. How Users Currently Access Project Settings

### 6.1. From The Gear Button On A Project Card

In:

- [Frontend/src/views/ManageSpaces.vue](Frontend/src/views/ManageSpaces.vue)

each project card includes a gear button.

The current flow is:

- the gear button calls `goToAdmin(space.id)`
- that function navigates to `/space/{projectId}/settings`

### 6.2. UI Permission For The Project Gear Button

In the same file, the gear button visibility or availability is driven by:

- `currentUser.systemRoles`

with currently allowed values:

- `System Admin`
- `Admin`
- `PM`
- `PO`
- `admin`

This is a design issue because `Project Settings` should primarily be controlled by `project roles`, not by `system roles`.

### 6.3. The `/space/:id/settings` Route

This route is declared in:

- [Frontend/src/router/spaceRoutes.js](Frontend/src/router/spaceRoutes.js)

Its current behavior:

- `/space/:id/settings`
- does not render a dedicated settings page
- redirects to `/admin/configuration`
- attaches `projectId` as a query parameter

### 6.4. The Redirect Target

The redirected page is:

- [Frontend/src/views/admin/Configuration.vue](Frontend/src/views/admin/Configuration.vue)

Based on code inspection, this page:

- is a system configuration screen
- does not use `route.query.projectId`
- has no project-specific context

Conclusion:

- a real `Project Settings` screen does not currently exist
- the project gear button points to the wrong destination
- users believe they are opening project configuration, but they are actually entering system configuration

## 7. Current Authorization Model

### 7.1. Does Authorization Exist

Yes.

Authorization currently exists at three layers:

- UI visibility
- route guard
- backend API protection

### 7.2. UI-Level Authorization

Examples:

- `SettingsDropdown.vue` uses `systemRoles` to show admin menu entries
- `ManageSpaces.vue` uses `systemRoles` to show the project gear button

Issues:

- those files do not use the same allowed role list
- neither one properly reflects project-level authorization

### 7.3. Route-Level Authorization

`router/index.js` checks `meta.requiredRoles`.

Issue:

- admin routes allow a broader role set than the dropdown shows
- that creates a gap between “menu visibility” and “actual route access”

### 7.4. Backend System-Level Authorization

`SystemAuthorizeAttribute` protects system-level admin APIs.

Its role is to:

- protect privileged system endpoints
- avoid trusting frontend logic alone

### 7.5. Backend Project-Level Authorization

The backend has a separate project authorization filter:

- [Backend/src/TaskManagement.API/Filters/ProjectAuthorizeAttribute.cs](Backend/src/TaskManagement.API/Filters/ProjectAuthorizeAttribute.cs)

This filter:

- reads `projectId`
- verifies project membership
- evaluates `ProjectRole`
- blocks write operations for `Guest` and `Stakeholder`

Example usage:

- [Backend/src/TaskManagement.API/Controllers/ProjectMembersController.cs](Backend/src/TaskManagement.API/Controllers/ProjectMembersController.cs)

Actions such as add member, remove member, and update member role use:

- `[ProjectAuthorize("PM, Admin")]`

This proves the codebase already has a real project-level authorization concept.

### 7.6. Authorization Conclusion

The codebase already contains both:

- `system authorization`
- `project authorization`

The main problem is that the frontend currently uses the wrong permission layer for the project gear button.

## 8. Are Admin Access And Project Settings Access Different Permissions

The correct answer should be: `Yes`.

### 8.1. Admin Access

This is a permission at:

- system level
- organization level
- instance level

Typical functions:

- user management across the system
- authentication settings
- email/SMTP settings
- IP whitelist settings
- audit log
- global default configuration

### 8.2. Project Settings Access

This is a permission at:

- one specific project level

Typical functions:

- manage project members
- manage project roles
- configure labels
- configure modules
- configure states
- configure cycles or sprints
- project-level danger zone operations

### 8.3. Correct Business Examples

- A `Project Manager` may be allowed to configure project A without being allowed into system admin.
- A `System Admin` may be allowed into system admin and may also be allowed to override project settings.
- A normal team member should typically not enter either area.

### 8.4. Design Conclusion

`Admin access` and `Project Settings access` must be separated in:

- UI
- routing
- backend authorization
- business documentation

## 9. Current Dashboard And Menu Structure

### 9.1. Main Sidebar

The main sidebar lives in:

- [Frontend/src/components/layout/NexusSidebar.vue](Frontend/src/components/layout/NexusSidebar.vue)

Main items:

- Home
- Drafts
- Your work
- Stickies
- Rewards

Workspace area:

- Projects
- More

Inside `More`:

- Views
- Analytics
- Archives

Per-project child entries:

- Work items
- Cycles
- Modules
- Views
- Pages

### 9.2. Dashboard

Main dashboard page:

- [Frontend/src/views/Dashboard.vue](Frontend/src/views/Dashboard.vue)

Notable capabilities:

- project listing
- project search
- favorites
- opening projects
- quick work item creation

## 10. What The Current Admin Area Contains

### 10.1. Admin Layout

Admin layout and sidebar:

- [Frontend/src/components/layout/AdminLayout.vue](Frontend/src/components/layout/AdminLayout.vue)
- [Frontend/src/components/layout/AdminSidebar.vue](Frontend/src/components/layout/AdminSidebar.vue)

### 10.2. Current Admin Menu

The admin sidebar currently contains:

- Audit Log
- User Management
- Configuration
- Instance
- Security

Details:

- `Audit Log`: system event logs
- `User Management`: users, departments, role mapping
- `Configuration`: default statuses and theme-like configuration
- `Instance > General settings`
- `Instance > Authentication`
- `Instance > Email`
- `Security > Two-Factor Auth`
- `Security > Change Password`
- `Security > IP Whitelist`

### 10.3. Menu Inconsistency

`SettingsDropdown.vue` still exposes organization/profile/contact links, while `AdminSidebar.vue` has the Organization section commented out. This means the admin menu is not consistent across entry points.

## 11. Project Creation Business Logic

### 11.1. Project Creation UI

The create project modal is located at:

- [Frontend/src/components/CreateSpaceModal.vue](Frontend/src/components/CreateSpaceModal.vue)

Main fields:

- project name
- key or identifier
- description
- visibility
- lead
- cover
- icon

### 11.2. Backend Project Creation

The handling service is:

- [Backend/src/TaskManagement.Infrastructure/Services/ProjectService.cs](Backend/src/TaskManagement.Infrastructure/Services/ProjectService.cs)

Important logic:

- creates the `Project` entity
- stores `cover/icon` in `NavigationConfig`
- seeds default task statuses:
  - `TO DO`
  - `IN PROGRESS`
  - `DONE`
- seeds task types based on template
- assigns the creator as `PROJECT_MANAGER`
- if a different lead is selected, creates a `PROJECT_LEAD`
- if the creator is the lead, the creator role is updated to `PROJECT_LEAD`

### 11.3. Business Meaning

Immediately after project creation, the system already has:

- a basic workflow structure
- baseline project role assignments
- visibility in dashboard/discovery flows

However:

- there is still no real `Project Settings` module to manage those project-specific settings after creation

## 12. Key Design Gaps In The Current Implementation

### 12.1. The Project Gear Button Opens The Wrong Destination

This is the biggest issue:

- users believe they are opening `Project Settings`
- but the code actually opens `Admin Configuration`

### 12.2. No Real Project Settings Screen Exists

There is no clear page such as:

- `ProjectSettings.vue`
- or any dedicated per-project settings module

### 12.3. The Project Gear Button Uses System Roles

This is incorrect by design because:

- project administration should be based on `project role`
- a project manager should not need `system admin` privileges just to manage a project

### 12.4. Authorization Is Not Aligned

The following are not aligned:

- `ManageSpaces.vue`
- `SettingsDropdown.vue`
- `adminRoutes.js`
- `SystemAuthorizeAttribute`
- `ProjectAuthorizeAttribute`

### 12.5. `Configuration.vue` Is System-Wide

It should not act as `Project Settings`.

### 12.6. Security Consistency Gap

Some endpoints in:

- [Backend/src/TaskManagement.API/Controllers/SystemSettingsController.cs](Backend/src/TaskManagement.API/Controllers/SystemSettingsController.cs)

use helper-based admin checks instead of a fully uniform authorization attribute strategy. This should be reviewed to avoid uneven protection levels.

## 13. Target Design Recommendation

### 13.1. Separate The Two Areas Clearly

The application should have two independent areas:

- `Admin`
- `Project Settings`

### 13.2. Admin

Admin should contain only:

- system configuration
- organization configuration
- user management
- security
- audit
- instance-level settings

Suggested route family:

- `/admin/*`

### 13.3. Project Settings

Project Settings should contain only:

- project-specific configuration
- only for users allowed to administer a project

Suggested route:

- `/space/:id/settings`

Suggested page:

- `Frontend/src/views/ProjectSettings.vue`

### 13.4. Suggested Project Settings Tabs

Recommended tabs:

- General
- Members & Roles
- States
- Labels
- Modules
- Cycles
- Integrations
- Danger Zone

### 13.5. Suggested Authorization Rules

- `System Admin`: can access Admin, can override into Project Settings
- `Admin`: can access Admin, may override Project Settings based on policy
- `PROJECT_MANAGER`: can access Project Settings
- `PROJECT_LEAD`: can access Project Settings
- `PM`, `PO`: can access Project Settings if valid under the business model
- `Developer`, regular member: no Admin access; Project Settings only if explicitly granted
- `Guest`, `Stakeholder`: no project administration access

## 14. Suggested Permission Matrix

| Role group | Admin access | Project Settings access | Notes |
|---|---|---|---|
| System Admin | Yes | Yes | System override |
| Admin | Yes | Yes or policy-based | Must be finalized |
| Organization Admin | Yes | Not by default or policy-based | Depends on org model |
| PROJECT_MANAGER | No | Yes | Project administration role |
| PROJECT_LEAD | No | Yes | Project administration role |
| PM | No or limited | Yes if part of the project | Naming must be normalized |
| PO | No or limited | Yes if part of the project | Naming must be normalized |
| Developer / DEV | No | No by default | Work-item contributor role |
| Regular member | No | No by default | No project configuration role |
| Guest | No | No | Read-only or restricted |
| Stakeholder | No | No | Read-only or restricted |

## 15. Recommended Implementation Phases

### Phase 1. Audit And Scope Freeze

Goal:

- confirm current behavior
- freeze the scope
- formalize the distinction between `Admin` and `Project Settings`

Work items:

- list all related routes, buttons, sidebars, and dropdowns
- inventory all admin APIs and project settings APIs
- build the current permission matrix
- identify mismatches

Deliverables:

- current-state document
- target-state document
- permission matrix v1

### Phase 2. Permission Model Unification

Goal:

- align permissions across frontend, router, and backend

Work items:

- define `system roles` and `project roles` clearly
- define which permissions belong to `Admin`
- define which permissions belong to `Project Settings`
- update router guards and UI visibility to use the same rules
- review backend authorization for consistency

Deliverables:

- final permission matrix
- normalized role list
- standard access rules for each route and API

### Phase 3. Build Real Project Settings

Goal:

- replace the redirect with a true project settings module

Work items:

- create `ProjectSettings.vue`
- update `/space/:id/settings`
- remove the redirect to `/admin/configuration`
- load project data by `:id`
- add project settings tabs

Deliverables:

- real project settings UI
- real project settings route
- real project context

### Phase 4. Clean Up The Admin Area

Goal:

- return `Admin` to a pure system-level role

Work items:

- remove project-specific assumptions from admin configuration
- align `SettingsDropdown`, `AdminSidebar`, and `adminRoutes`
- normalize organization, instance, and security menu structure

Deliverables:

- consistent admin area
- consistent admin navigation
- no overlap with project settings

### Phase 5. QA, Regression, And Rollout

Goal:

- ensure the refactor does not break authorization or navigation

Work items:

- test regular user flows
- test project manager flows
- test system admin flows
- test deep links
- test hidden menu conditions
- test API unauthorized/forbidden behavior
- test legacy redirects

Deliverables:

- test checklist
- post-refactor bug list
- release-ready validation

## 16. Recommended AI Execution Order

An AI should execute the work in this order:

1. Read all admin routes, project routes, settings dropdown logic, and project gear-button code.
2. Build the current permission matrix and the target permission matrix.
3. Create `ProjectSettings.vue` and make `/space/:id/settings` render it directly.
4. Change the project gear permission logic to rely on `project role`.
5. Keep `Admin` strictly system-level.
6. Align `SettingsDropdown.vue`, `AdminSidebar.vue`, `adminRoutes.js`, and `router/index.js`.
7. Review backend APIs so every admin endpoint uses a consistent protection strategy.

## 17. Acceptance Criteria

The system should be considered complete when:

- clicking the project gear button opens a real `Project Settings` page
- `Project Settings` no longer redirects into `Admin`
- a user with project-level management rights but without system admin rights can still access the correct project settings
- a system admin can override project settings if that is the chosen policy
- admin menu visibility and admin route protection are aligned
- the backend correctly rejects unauthorized requests
- dashboard and sidebars no longer expose the wrong settings entry points for the wrong roles

## 18. Final Conclusion

`D:\A\QuanLyCongViec` already has a strong foundation for both `system authorization` and `project authorization`, but the current routing and UX mix those two layers together. The central design mistake is that the project gear button does not open a true `Project Settings` module and instead redirects into system configuration.

If the application is refactored according to the five phases above, it will become:

- clearer in terms of permission boundaries
- easier to maintain
- more correct from a business perspective
- closer to a professional project-management administration experience

## 19. Appendix: Main Reference Files

Frontend:

- [Frontend/src/views/ManageSpaces.vue](Frontend/src/views/ManageSpaces.vue)
- [Frontend/src/router/spaceRoutes.js](Frontend/src/router/spaceRoutes.js)
- [Frontend/src/components/SettingsDropdown.vue](Frontend/src/components/SettingsDropdown.vue)
- [Frontend/src/router/adminRoutes.js](Frontend/src/router/adminRoutes.js)
- [Frontend/src/router/index.js](Frontend/src/router/index.js)
- [Frontend/src/views/admin/Configuration.vue](Frontend/src/views/admin/Configuration.vue)
- [Frontend/src/components/layout/AdminSidebar.vue](Frontend/src/components/layout/AdminSidebar.vue)
- [Frontend/src/components/layout/AdminLayout.vue](Frontend/src/components/layout/AdminLayout.vue)
- [Frontend/src/components/layout/NexusSidebar.vue](Frontend/src/components/layout/NexusSidebar.vue)
- [Frontend/src/views/Dashboard.vue](Frontend/src/views/Dashboard.vue)
- [Frontend/src/components/CreateSpaceModal.vue](Frontend/src/components/CreateSpaceModal.vue)

Backend:

- [Backend/src/TaskManagement.API/Filters/SystemAuthorizeAttribute.cs](Backend/src/TaskManagement.API/Filters/SystemAuthorizeAttribute.cs)
- [Backend/src/TaskManagement.API/Filters/ProjectAuthorizeAttribute.cs](Backend/src/TaskManagement.API/Filters/ProjectAuthorizeAttribute.cs)
- [Backend/src/TaskManagement.API/Controllers/AdminUsersController.cs](Backend/src/TaskManagement.API/Controllers/AdminUsersController.cs)
- [Backend/src/TaskManagement.API/Controllers/ProjectMembersController.cs](Backend/src/TaskManagement.API/Controllers/ProjectMembersController.cs)
- [Backend/src/TaskManagement.API/Controllers/SystemSettingsController.cs](Backend/src/TaskManagement.API/Controllers/SystemSettingsController.cs)
- [Backend/src/TaskManagement.Infrastructure/Services/ProjectService.cs](Backend/src/TaskManagement.Infrastructure/Services/ProjectService.cs)
- [Backend/src/TaskManagement.Application/DTOs/Project/ProjectDiscoveryDto.cs](Backend/src/TaskManagement.Application/DTOs/Project/ProjectDiscoveryDto.cs)
