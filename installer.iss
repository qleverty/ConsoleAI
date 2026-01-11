[Setup]
AppName=ConsoleAI
AppVersion=1.0.0
AppPublisher=qleverty
AppPublisherURL=https://github.com/qleverty/ConsoleAI
DefaultDirName={autopf}\ConsoleAI
DefaultGroupName=ConsoleAI
DisableProgramGroupPage=yes
UninstallDisplayName=ConsoleAI
UninstallDisplayIcon={app}\ConsoleAI.exe
OutputDir=installer_output
OutputBaseFilename=ConsoleAI-Setup-1.0.0
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64
PrivilegesRequired=admin
ChangesEnvironment=yes
Compression=lzma2
SolidCompression=yes

[Files]
Source: "D:\Desktop\f\ConsoleAI\bin\Release\net10.0\win-x64\publish\ConsoleAI.exe"; DestDir: "{app}"

[Icons]
Name: "{group}\Uninstall ConsoleAI"; Filename: "{uninstallexe}"

[Code]
procedure CurStepChanged(CurStep: TSetupStep);
var
  OldPath, NewPath, AppDir: string;
  BatchContent: string;
begin
  if CurStep = ssPostInstall then
  begin
    AppDir := ExpandConstant('{app}');
    
    BatchContent := '@echo off' + #13#10 + 'ConsoleAI.exe "deepseek/deepseek-chat" %*';
    SaveStringToFile(AppDir + '\dp.bat', BatchContent, False);
    
    BatchContent := '@echo off' + #13#10 + 'ConsoleAI.exe "google/gemini-2.0-flash-exp:free" %*';
    SaveStringToFile(AppDir + '\gn.bat', BatchContent, False);
    
    BatchContent := '@echo off' + #13#10 + 'ConsoleAI.exe "openai/gpt-4o-mini" %*';
    SaveStringToFile(AppDir + '\gpt.bat', BatchContent, False);
    
    BatchContent := '@echo off' + #13#10 + 'ConsoleAI.exe "meta-llama/llama-3.1-8b-instruct" %*';
    SaveStringToFile(AppDir + '\lm.bat', BatchContent, False);
    
    SaveStringToFile(AppDir + '\prompt.txt', '', False);
    SaveStringToFile(AppDir + '\openrouterkey.txt', '', False);
    
    if RegQueryStringValue(HKLM, 'SYSTEM\CurrentControlSet\Control\Session Manager\Environment', 'Path', OldPath) then
    begin
      if Pos(Uppercase(AppDir), Uppercase(OldPath)) = 0 then
      begin
        NewPath := OldPath + ';' + AppDir;
        RegWriteStringValue(HKLM, 'SYSTEM\CurrentControlSet\Control\Session Manager\Environment', 'Path', NewPath);
      end;
    end;
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  OldPath, NewPath, AppDir: string;
  FilesToDelete: array of string;
  I: Integer;
begin
  if CurUninstallStep = usUninstall then
  begin
    AppDir := ExpandConstant('{app}');
    
    SetArrayLength(FilesToDelete, 6);
    FilesToDelete[0] := AppDir + '\dp.bat';
    FilesToDelete[1] := AppDir + '\gn.bat';
    FilesToDelete[2] := AppDir + '\gpt.bat';
    FilesToDelete[3] := AppDir + '\lm.bat';
    FilesToDelete[4] := AppDir + '\prompt.txt';
    FilesToDelete[5] := AppDir + '\openrouterkey.txt';
    
    for I := 0 to GetArrayLength(FilesToDelete) - 1 do
    begin
      if FileExists(FilesToDelete[I]) then
        DeleteFile(FilesToDelete[I]);
    end;
    
    if RegQueryStringValue(HKLM, 'SYSTEM\CurrentControlSet\Control\Session Manager\Environment', 'Path', OldPath) then
    begin
      NewPath := OldPath;
      StringChangeEx(NewPath, ';' + AppDir, '', True);
      StringChangeEx(NewPath, AppDir + ';', '', True);
      StringChangeEx(NewPath, AppDir, '', True);
      RegWriteStringValue(HKLM, 'SYSTEM\CurrentControlSet\Control\Session Manager\Environment', 'Path', NewPath);
    end;
  end;
end;