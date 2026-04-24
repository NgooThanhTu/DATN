# Estimate, Story Points, Velocity, Rewards, and AI Blueprint

## 1. Goal

This document defines a professional target model for:

- `Story Points`
- `Estimated Hours`
- `Actual Hours`
- `Velocity`
- `Rewards / Gamification`
- `AI-assisted breakdown and repo analysis`

It is written against the current SprintA codebase so implementation can be done incrementally without throwing away existing work.

---

## 2. Current Codebase Reality

### 2.1 Core data that already exists

The current backend already has a useful foundation:

- [WorkTask.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Domain/Entities/WorkTask.cs>)
  - `StoryPoints`
  - `TotalEstimatedHours`
  - `TotalActualHours`
  - `ParentTaskId`
- [TaskAssignment.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Domain/Entities/TaskAssignment.cs>)
  - `EstimatedHours`
  - `ContributionWeight`
  - `ProgressPercent`
  - `TotalActualHours`
- [TimeLog.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Domain/Entities/TimeLog.cs>)
  - `Hours`
  - `WorkType`
  - `LoggedAt`
- [PointTransaction.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Domain/Entities/PointTransaction.cs>)
- [UserWallet.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Domain/Entities/UserWallet.cs>)

### 2.2 Existing roll-up behavior

[ApplicationDbContext.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Infrastructure/Data/ApplicationDbContext.cs>) already does automatic roll-up:

- `TimeLog.Hours` are summed into `WorkTask.TotalActualHours`
- child task actual hours are rolled up to parent task actual hours

This is a strong base and should be kept.

### 2.3 Existing estimate UI behavior

[TaskDetailModal.vue](</D:/A/QuanLyCongViec/Frontend/src/components/TaskDetailModal.vue>) already supports:

- direct edit of `totalEstimatedHours`
- estimate suggestion based on `priority + storyPoints + task title keywords`
- assignee-level estimate split
- `ContributionWeight`
- roll-up parent estimate from sub-work items

### 2.4 Existing rewards behavior

[GamificationService.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Infrastructure/Services/GamificationService.cs>) currently:

- rewards on task done
- rewards assignee completion
- splits assignee rewards by `ContributionWeight`
- gives early completion bonus
- calculates base reward from:
  - `StoryPoints`
  - `Priority`
  - task duration in days

### 2.5 Existing sprint velocity-like behavior

[SprintService.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Infrastructure/Services/SprintService.cs>) already uses `StoryPoints` for burndown. If no story points exist, it falls back to task count.

That means the codebase already assumes `Story Points` are the planning unit.

---

## 3. Main Problems Today

### 3.1 Story Points and Estimated Hours are not clearly separated

Right now they both exist, but the business meaning is not strict enough:

- `Story Points` should represent complexity and uncertainty
- `Estimated Hours` should represent execution effort

### 3.2 Rewards are too influenced by calendar duration

Current reward formula multiplies by planned days. That creates a bad incentive:

- longer task duration can accidentally produce more points
- inflated schedule can look more rewarding than efficient work

### 3.3 Velocity is not yet a first-class concept

There is no explicit:

- team velocity per sprint
- user delivery profile
- estimate accuracy model

### 3.4 Multi-assignee effort is only partially modeled

The code already supports `ContributionWeight` and per-assignee estimate, but the business rules are not fully documented:

- what does total effort mean
- what happens when 2 users work on the same subtask
- how to translate effort into calendar duration

---

## 4. Target Business Model

## 4.1 Core principle

Use 3 different measurement systems:

1. `Story Points`
   - planning complexity
   - relative sizing
   - sprint planning / velocity

2. `Estimated Hours`
   - execution effort
   - staffing / workload split
   - rewards accuracy factor

3. `Actual Hours`
   - measured from time logs only
   - used for fairness, payroll-like reporting, and estimate improvement

---

## 5. Story Points Model

### 5.1 Recommended scale

Use Fibonacci:

- `1`
- `2`
- `3`
- `5`
- `8`
- `13`
- `21`

### 5.2 Meaning

- `1`: trivial, very clear
- `2-3`: small
- `5`: medium
- `8`: large
- `13`: very large, should usually be split
- `21+`: should not go into sprint without further breakdown

### 5.3 Governance rule

Story points should be set by:

- PM
- PO
- project lead
- planning session consensus

They should not be auto-overwritten by actual hours.

### 5.4 Recommended planning rule

- task above `8 SP`: warning to split
- task above `13 SP`: block sprint commitment until breakdown

---

## 6. Estimate Model

### 6.1 What should be estimated manually

Recommended rule:

- parent task:
  - may hold a temporary estimate before breakdown
  - once subtasks exist, parent estimate should become roll-up only
- subtask:
  - this is the main manual estimate unit
- assignment:
  - each assignee gets an effort share

### 6.2 Canonical fields

Keep using current fields:

- `WorkTask.TotalEstimatedHours`
- `TaskAssignment.EstimatedHours`
- `TaskAssignment.ContributionWeight`

Business meaning:

- `WorkTask.TotalEstimatedHours` = total task effort
- `TaskAssignment.EstimatedHours` = assignee share of effort
- `ContributionWeight` = relative share when splitting effort

### 6.3 Actual hours rule

`ActualHours` must come from `TimeLogs` only.

Do not allow free manual editing of:

- `WorkTask.TotalActualHours`
- `TaskAssignment.TotalActualHours`

Instead:

- task actual = sum of all time logs on the task
- assignee actual = sum of assignee time logs on the task
- parent actual = roll-up from children

---

## 7. Recommended Estimate Formula

### 7.1 Baseline from story points

Suggested baseline mapping:

- `1 SP = 2h`
- `2 SP = 4h`
- `3 SP = 8h`
- `5 SP = 16h`
- `8 SP = 24h`
- `13 SP = 40h`

This is intentionally nonlinear.

### 7.2 Adjustment factors

Apply 3 multipliers:

1. `PriorityFactor`
- urgent: `1.20`
- high: `1.10`
- medium: `1.00`
- low: `0.90`

2. `RiskFactor`
- clear / familiar work: `1.00`
- moderate unknowns / integration: `1.15`
- high uncertainty / research / migration: `1.30`

3. `SkillFactor`
- above baseline: `0.85 - 0.95`
- baseline: `1.00`
- below baseline: `1.10 - 1.25`

### 7.3 Formula

`EstimatedHours = BaseHours * PriorityFactor * RiskFactor * SkillFactor`

This should remain a suggestion, not an absolute lock.

---

## 8. Multi-Assignee Model

### 8.1 Total effort vs individual effort

If a task requires `20h` total effort and has:

- user A weight `0.6`
- user B weight `0.4`

then:

- user A estimate = `12h`
- user B estimate = `8h`

### 8.2 Calendar duration is not the same as total effort

Two people working together do not automatically halve calendar time because collaboration has overhead.

Recommended concept:

`CalendarDuration = TotalEffort / (ParallelCapacity * CollaborationEfficiency)`

Example:

- effort = `20h`
- 2 active assignees
- collaboration efficiency = `0.85`

Then:

- duration ~= `20 / (2 * 0.85) = 11.8h`

### 8.3 When 2 people work on the same subtask

That is allowed.

Business rule:

- keep one subtask
- split effort using `ContributionWeight`
- log actuals separately via `TimeLogs`
- reward each user by completion share, not duplicated full reward

---

## 9. Parent Task and Deep Subtask Rule

This should be the standard model:

- lowest executable node gets manual estimate
- parent node gets roll-up estimate
- same applies recursively for subtask of subtask

Therefore:

- if a child exists, parent estimate is derived
- if child subtasks exist, child becomes derived too

This matches the hierarchy already supported by `ParentTaskId`.

---

## 10. Velocity Model

## 10.1 Team Velocity

Definition:

- average completed story points per sprint
- based on last `3-5` completed sprints

Formula:

`TeamVelocity = avg(CompletedSP per recent sprint)`

Use cases:

- sprint capacity planning
- release forecasting
- overload detection

## 10.2 User Velocity

Do not manually say “senior = 1.2, junior = 0.8” as the primary system.

Instead derive user delivery profile from history.

Suggested metrics:

- `EstimateAccuracyRatio = ActualHours / EstimatedHours`
- `StoryPointsCompletedPerWeek`
- `OnTimeRate`
- `ReopenRate`

From those derive:

- `UserVelocityIndex`

Example interpretation:

- strong performer: `1.10 - 1.20`
- baseline: `1.00`
- still ramping up: `0.80 - 0.95`

### 10.3 User-adjusted estimate

`AdjustedEstimate = BaseEstimate / UserVelocityIndex`

Example:

- base estimate = `16h`
- strong user `1.15` => `13.9h`
- new user `0.80` => `20h`

This is the right way to make estimate “fit user level” without hard-coding unfair assumptions.

---

## 11. Reward Model

## 11.1 Problem with current reward logic

Current code in [GamificationService.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Infrastructure/Services/GamificationService.cs>) uses:

- story points
- priority impact
- planned duration in days

That should be changed because duration-based multiplication encourages inflated schedules.

## 11.2 Professional reward model

Recommended reward formula:

`BaseReward = StoryPoints * 10`

Then apply:

- `PriorityMultiplier`
  - urgent: `1.30`
  - high: `1.15`
  - medium: `1.00`
  - low: `0.90`

- `OnTimeMultiplier`
  - on time or early: `1.10`
  - slightly late: `1.00`
  - clearly late: `0.85`

- `AccuracyMultiplier`
  - actual/estimate within `0.85 - 1.15`: `1.10`
  - within `0.70 - 1.30`: `1.00`
  - outside that range: `0.85`

Then split by assignee weight:

`FinalUserReward = TotalTaskReward * ContributionShare`

## 11.3 Why this is better

It rewards:

- difficulty
- urgency
- on-time delivery
- estimate accuracy

It does not reward:

- stretching duration
- fake large estimates

---

## 12. Recommended Professional Rules

### 12.1 Planning

- use `Story Points` for sprint planning
- use `Estimated Hours` for staffing and fairness

### 12.2 Execution

- actual work tracked through `TimeLogs`
- assignment progress tracked per assignee

### 12.3 Review

- compare `estimate vs actual`
- compute user velocity from historical delivery

### 12.4 Reward

- reward good delivery quality, not inflated task size

---

## 13. Exact Codebase Changes Needed

## 13.1 Entities

### Keep and reuse

- [WorkTask.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Domain/Entities/WorkTask.cs>)
- [TaskAssignment.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Domain/Entities/TaskAssignment.cs>)
- [TimeLog.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Domain/Entities/TimeLog.cs>)
- [PointTransaction.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Domain/Entities/PointTransaction.cs>)
- [UserWallet.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Domain/Entities/UserWallet.cs>)

### Add new entities

Recommended additions:

1. `UserSkillProfile`
- `UserId`
- `DomainKey` like `backend`, `frontend`, `qa`, `devops`
- `VelocityIndex`
- `ReliabilityScore`
- `OnTimeRate`
- `EstimateAccuracyScore`
- `LastCalculatedAt`

2. `SprintVelocitySnapshot`
- `SprintId`
- `ProjectId`
- `CompletedStoryPoints`
- `CommittedStoryPoints`
- `CarryOverStoryPoints`
- `VelocityScore`
- `CreatedAt`

3. `TaskEstimateSnapshot`
- `WorkTaskId`
- `EstimatedByUserId`
- `BaseEstimateHours`
- `AdjustedEstimateHours`
- `StoryPointsAtEstimateTime`
- `PriorityAtEstimateTime`
- `RiskFactor`
- `SkillFactor`
- `CreatedAt`

These can be created in:

- `Backend/src/TaskManagement.Domain/Entities`
- registered in [ApplicationDbContext.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Infrastructure/Data/ApplicationDbContext.cs>)

## 13.2 Backend services to change

### A. Work task service

[WorkTaskService.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Infrastructure/Services/WorkTaskService.cs>)

Add or refine:

- roll-up estimate from children automatically
- roll-up assignee actual hours from time logs
- block manual parent estimate override when children exist
- add `suggest-estimate` or `recalculate-estimate` logic

### B. Time tracking / persistence roll-up

[ApplicationDbContext.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Infrastructure/Data/ApplicationDbContext.cs>)

Extend current roll-up logic:

- update `TaskAssignment.TotalActualHours` from `TimeLogs`
- update parent estimated hours from child estimates
- optionally update parent progress from children

### C. Rewards

[GamificationService.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Infrastructure/Services/GamificationService.cs>)

Replace current day-based reward formula with:

- story point base
- priority multiplier
- on-time multiplier
- estimate accuracy multiplier
- weighted split by assignee

### D. Sprint analytics / velocity

[SprintService.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Infrastructure/Services/SprintService.cs>)

Add:

- explicit sprint velocity snapshot calculation
- carry-over workload report
- committed vs completed SP

### E. New analytics service

Recommended new service:

- `VelocityService.cs`

Responsibilities:

- compute team velocity
- compute user velocity
- compute estimate accuracy
- expose recommendation factor for AI/estimate suggestion

Suggested location:

- `Backend/src/TaskManagement.Infrastructure/Services/VelocityService.cs`

## 13.3 Backend APIs to add

Recommended controller additions:

1. `GET /api/analytics/velocity/project/{projectId}`
- sprint-by-sprint team velocity

2. `GET /api/analytics/velocity/users/{projectId}`
- user velocity index and estimate accuracy

3. `POST /api/worktasks/{id}/suggest-estimate`
- returns:
  - suggested story points
  - suggested total estimate
  - suggested assignee split

4. `POST /api/worktasks/{id}/rollup-estimate`
- roll up parent estimate from children

5. `GET /api/rewards/explain/{taskId}`
- explain why reward points were calculated that way

These can live in:

- existing `WorkTasksController`
- or new focused controllers under `TaskManagement.API/Controllers`

## 13.4 Frontend files to change

### A. Task detail

[TaskDetailModal.vue](</D:/A/QuanLyCongViec/Frontend/src/components/TaskDetailModal.vue>)

Needed changes:

- enforce Fibonacci story points selector
- make parent estimate read-only when children exist
- show:
  - total estimate
  - assignee estimates
  - actual hours
  - estimate accuracy
- surface “AI / system suggested estimate”

### B. Work item state/store

[useWorkTaskStore.js](</D:/A/QuanLyCongViec/Frontend/src/store/useWorkTaskStore.js>)

Needed changes:

- normalize new estimate/velocity/reward explanation fields
- normalize assignment actual hours if added from API

### C. Rewards page

[RewardsView.vue](</D:/A/QuanLyCongViec/Frontend/src/views/RewardsView.vue>)

Needed changes:

- show point breakdown by:
  - base SP reward
  - priority modifier
  - on-time modifier
  - estimate accuracy modifier
- show fairness explanation

### D. Your Work / Analytics

[YourWorkView.vue](</D:/A/QuanLyCongViec/Frontend/src/views/YourWorkView.vue>)

Recommended additions:

- estimate accuracy chart
- actual vs estimate chart
- user velocity chart
- throughput trend

### E. Sprint / project analytics

Use or extend current analytics pages to add:

- team velocity
- committed vs completed SP
- carry-over work
- estimate accuracy trend

---

## 14. AI in Current Codebase

## 14.1 What is already implemented

Backend:

- [AiController.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.API/Controllers/AiController.cs>)
- [IAiService.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Application/Interfaces/IAiService.cs>)
- [GeminiAiService.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Infrastructure/Services/GeminiAiService.cs>)
- [AiDtos.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Application/DTOs/AI/AiDtos.cs>)
- [AITokenUsage.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Domain/Entities/AITokenUsage.cs>)
- [AIPromptTemplate.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Domain/Entities/AIPromptTemplate.cs>)

Frontend:

- [AIPage.vue](</D:/A/QuanLyCongViec/Frontend/src/views/AIPage.vue>)

Current AI features:

- chat with user
- generate work item description
- break parent task into subtasks
- optionally create those subtasks
- track token quota
- analyze GitHub repo metadata on frontend

## 14.2 Important note about current GitHub repo analysis

Right now the system does not truly “read the whole repository”.

The current [AIPage.vue](</D:/A/QuanLyCongViec/Frontend/src/views/AIPage.vue>) behavior is:

- parse GitHub URL
- call GitHub API for:
  - repo metadata
  - languages
  - README
  - up to 5 open issues
- compose a prompt
- send that prompt into Gemini chat

So today the AI can analyze:

- repo description
- README
- language mix
- a few open issues

It cannot yet deeply inspect:

- full file tree
- architecture layer by layer
- code semantics across the repository
- dependency graph
- commit history

unless you build extra backend logic for repository ingestion.

---

## 15. What Gemini 2.5 Flash Can Do Well Here

Assuming you are using Gemini 2.5 Flash via the same service pattern, it is well-suited for:

### 15.1 Good use cases

1. Chat assistant
- answer user questions about:
  - project setup
  - agile workflow
  - task planning
  - backlog triage

2. Task breakdown
- break parent task into `3-7` subtasks
- propose title, description, estimate, priority

3. Description generation
- turn rough notes into clean task descriptions

4. Repo-level planning from metadata
- infer likely modules from:
  - README
  - package list
  - language mix
  - issue summaries

5. Lightweight estimate suggestion
- based on title, description, priority, and historical prompt context

6. User conversation
- Q&A
- planning support
- refinement support
- sprint assistance

### 15.2 Where Gemini 2.5 Flash is not enough by itself

It should not be trusted alone for:

- exact codebase-wide dependency reasoning without repo ingestion
- deterministic architecture extraction from only repo title/README
- exact effort estimation by user skill without project history data
- secure autonomous task creation from private repo without proper permission and ingestion pipeline

---

## 16. AI Target Capabilities Roadmap

## 16.1 Level 1: What you can do now with small improvement

Using current Gemini service plus modest changes:

- generate description
- break task into subtasks
- suggest story points
- suggest estimate hours
- suggest assignee split by weight
- summarize sprint risks
- summarize repo metadata into backlog themes

## 16.2 Level 2: Repo-aware AI planning

To truly analyze GitHub repo structure, add:

1. backend GitHub ingestion service
- read repository tree
- read selected files
- chunk README / docs / configs / manifests

2. vector or indexed retrieval
- likely using existing [TaskVectorEmbedding.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Domain/Entities/TaskVectorEmbedding.cs>) pattern as inspiration
- add repo document embeddings

3. repository analysis pipeline
- infer:
  - modules
  - services
  - integration points
  - testing gaps

Then AI can:

- create backlog from repo
- propose modules / cycles / epics
- detect likely missing tasks

## 16.3 Level 3: Personalized AI estimation

After velocity and accuracy metrics exist, AI can:

- suggest estimate by assignee
- explain estimate factors
- compare against user historical performance
- recommend whether a task should be split

---

## 17. Exact AI Codebase Changes Needed

## 17.1 Backend services

### A. Extend Gemini service

[GeminiAiService.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Infrastructure/Services/GeminiAiService.cs>)

Add new methods:

- `SuggestEstimateAsync(...)`
- `SuggestStoryPointsAsync(...)`
- `AnalyzeRepositoryAsync(...)`
- `GenerateSprintPlanAsync(...)`
- `SummarizeRisksAsync(...)`

### B. Add repository ingestion service

Recommended new files:

- `Backend/src/TaskManagement.Infrastructure/Services/GitHubRepositoryIngestionService.cs`
- `Backend/src/TaskManagement.Application/Interfaces/IGitHubRepositoryIngestionService.cs`

Responsibilities:

- fetch repo tree
- fetch README / docs / config files
- fetch selected source files
- normalize into AI-ready context

### C. Add AI orchestration layer

Recommended:

- `Backend/src/TaskManagement.Infrastructure/Services/AiPlanningService.cs`

Responsibilities:

- combine:
  - repo context
  - velocity context
  - estimate context
  - task context
- produce structured planning output

## 17.2 Backend entities for future AI

Recommended additions:

1. `RepositoryIntegrationSnapshot`
- projectId
- provider
- repoUrl
- owner
- repoName
- branch
- lastIndexedAt

2. `RepositoryDocument`
- projectId
- path
- contentType
- contentHash
- extractedText
- updatedAt

3. `RepositoryDocumentEmbedding`
- repositoryDocumentId
- vector payload or external reference

4. `AiEstimateRecommendation`
- workTaskId
- model
- recommendedStoryPoints
- recommendedHours
- explanation
- confidenceScore
- createdAt

## 17.3 Frontend files to extend for AI

### A. AI page

[AIPage.vue](</D:/A/QuanLyCongViec/Frontend/src/views/AIPage.vue>)

Add:

- repo indexing status
- analysis scope selector
- “create backlog draft”
- “suggest modules”
- “suggest cycle plan”
- “suggest estimates”

### B. Task detail

[TaskDetailModal.vue](</D:/A/QuanLyCongViec/Frontend/src/components/TaskDetailModal.vue>)

Add:

- `AI suggest estimate`
- `AI suggest story points`
- `AI split into subtasks`
- `AI explain why`

### C. Project settings integrations

[ProjectSettings.vue](</D:/A/QuanLyCongViec/Frontend/src/views/ProjectSettings.vue>)

Current direction is already correct:

- keep only GitHub integration
- store repo identity for future AI use

Needed next:

- branch selection
- indexing trigger
- indexing status
- access token validation

---

## 18. Recommended Implementation Order

### Phase 1

Estimate and reward business cleanup:

- separate SP vs hours formally
- fix reward formula
- enforce actual hours from time logs

### Phase 2

Velocity:

- team velocity
- user estimate accuracy
- user velocity index

### Phase 3

UI upgrades:

- task detail estimate model
- rewards explanation
- analytics charts

### Phase 4

AI estimate features:

- AI estimate suggestion
- AI story point suggestion
- AI assignee split suggestion

### Phase 5

Repo-aware AI:

- GitHub ingestion
- architecture summary
- backlog generation
- task decomposition from repo

---

## 19. Final Recommendation

The current codebase is already strong enough to support a serious implementation without a rewrite.

The most important business decision is this:

- `Story Points` = planning complexity
- `Estimated Hours` = execution effort
- `Actual Hours` = time logs only
- `Velocity` = historical delivery capability
- `Rewards` = difficulty + urgency + timeliness + accuracy
- `AI` = assistant and recommender, not autonomous source of truth

If this model is adopted, the system can become:

- fairer for rewards
- more professional for sprint planning
- more accurate for multi-assignee work
- much stronger for AI-assisted planning later

---

## 20. Short Action List

Implement first:

1. Refactor reward calculation in [GamificationService.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Infrastructure/Services/GamificationService.cs>)
2. Extend roll-up logic in [ApplicationDbContext.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.Infrastructure/Data/ApplicationDbContext.cs>)
3. Lock parent estimate when children exist in [TaskDetailModal.vue](</D:/A/QuanLyCongViec/Frontend/src/components/TaskDetailModal.vue>)
4. Add team and user velocity service in backend
5. Add AI estimate suggestion endpoints in [AiController.cs](</D:/A/QuanLyCongViec/Backend/src/TaskManagement.API/Controllers/AiController.cs>)
6. Build GitHub ingestion backend if you want real repo-driven task generation

