$ErrorActionPreference = 'Stop'

$baseUrl = 'http://localhost:5136/api'

# 1. Register a user
$regBody = @{
    email = 'debuguser@test.com'
    password = 'Password123!'
    fullName = 'Debug User'
} | ConvertTo-Json -Depth 5

$regResp = Invoke-RestMethod -Uri "$baseUrl/auth/register" -Method Post -Body $regBody -ContentType 'application/json'
$token = $regResp.data.token

$headers = @{
    Authorization = "Bearer $token"
}

# 2. Create a Project
$projectBody = @{
    name = 'Debug Project'
    departmentId = '00000000-0000-0000-0000-000000000000'
} | ConvertTo-Json -Depth 5

$projectResp = Invoke-RestMethod -Uri "$baseUrl/projects" -Method Post -Body $projectBody -ContentType 'application/json' -Headers $headers
$projectId = $projectResp.data.id

# 3. Create a Task (This is where the 500 happens)
$taskBody = @{
    title = 'Test Task'
    description = ''
    priority = 3
    storyPoints = 0
} | ConvertTo-Json -Depth 5

try {
    Invoke-RestMethod -Uri "$baseUrl/projects/$projectId/WorkTasks" -Method Post -Body $taskBody -ContentType 'application/json' -Headers $headers
} catch {
    Write-Host "
--- ERROR RESPONSE BODY ---"
    Write-Host $_.ErrorDetails.Message
}
