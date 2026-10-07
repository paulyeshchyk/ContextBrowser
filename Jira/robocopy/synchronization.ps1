# --- НАСТРОЙКИ (Укажи свои пути) ---
$svnFolder = "D:\projects\ascon\GulfStream9\trunk\Документация\ГОЛЬФСТРИМ WEB"    # Папка, где работает SmartSVN
$gitFolder = "D:\projects\ascon\GULF_Help"     # Твой локальный репозиторий Git
$branchName = "main"                    # Название твоей ветки (main или master)

Write-Host "`n[1/3] Зеркалирование файлов..." -ForegroundColor Cyan

# Robocopy сделает идеальный слепок:
# /MIR - зеркало (удалит в Git то, чего нет в SVN)
# /XD .svn - проигнорирует папку .svn в корне и подпапках
# /R:0 /W:0 - не ждать при ошибках
$excludeDirs = @(".svn", ".git", "robocopy", "build")
robocopy $svnFolder $gitFolder /MIR /XD $excludeDirs /R:0 /W:0 /NFL /NDL /NJH /NJS

Write-Host "[2/3] Анализ изменений в Git..." -ForegroundColor Cyan
Set-Location $gitFolder

# Проверяем, есть ли изменения
$gitStatus = git status --porcelain
if ($gitStatus) {
    Write-Host "Обнаружены изменения. Формирую коммит..." -ForegroundColor Green
    
    # Добавляем всё
    git add .
    
    # Формируем сообщение с датой
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm"
    git commit -m "Sync from VVA ($timestamp)"
    
    # Write-Host "[3/3] Отправка в удаленный репозиторий (10.9.20.9)..." -ForegroundColor Cyan
    # git push origin $branchName
    
    Write-Host "`nУспешно синхронизировано!" -ForegroundColor Green
} else {
    Write-Host "`n[!] Изменений в SVN не обнаружено. Git-репозиторий актуален." -ForegroundColor Yellow
}

# Чтобы окно не закрылось сразу (для ручного запуска)
Write-Host "`nНажми любую клавишу, чтобы выйти..."
$null = [Console]::ReadKey()