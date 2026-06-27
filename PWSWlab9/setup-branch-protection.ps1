# C:\Users\jakja\source\repos\PWSWlab9\setup-branch-protection.ps1
# Uruchom po utworzeniu repozytorium na GitHub i pierwszym pushu na main.
# Wymaga: gh auth login

param(
    [string]$Owner = (gh api user -q .login),
    [string]$Repo = "lab09-cicd"
)

Write-Host "Konfiguracja branch protection dla $Owner/$Repo ..."

gh api "repos/$Owner/$Repo/branches/main/protection" `
    --method PUT `
    --field required_status_checks='{"strict":true,"contexts":["build"]}' `
    --field enforce_admins=true `
    --field required_pull_request_reviews='{"required_approving_review_count":1}'

Write-Host "Gotowe. Branch main wymaga zielonego CI i 1 review przed merge."
