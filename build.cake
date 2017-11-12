#addin "Cake.FileHelpers"
#addin "Cake.Powershell"

#load "scripts/utilities.cake"

var target = Argument("target", "Default");
String timeStamp = TimeStamp();
var home = Directory(HomeFolder());

Action<string, string, bool> dotfile = (source, dest, dotting) => {
  var directory = Directory(dest);
  var repo_file = File($"./{source}");
  var dot = dotting ? "." : "";
  var file = $"{dot}{source.Split('/').Last()}";
  var link = directory + File(file);
  if (FileExists(link))
  {
    var old = directory + File($"{file}.{timeStamp}.old");
    MoveFile(link, old);
  }
  SymLinkFile(repo_file, link);
};

Task("Default")
  .IsDependentOn("git")
  .IsDependentOn("vscode")
  .Does(() =>
{
});

Task("git")
  .Does(() =>
{
  dotfile("git/gitconfig", home, true);
  dotfile("git/gitconfig.local", home, true);
  dotfile("git/gitignore.global", home, true);
});

Task("vscode")
  .Does(() =>
{
  var app_home = home;
  if (IsRunningOnWindows())
  {
    app_home = Directory($"{EnvironmentVariable("APPDATA")}/Code/User");
  } else if (IsRunningOnLinux()) {
    app_home = Directory($"{home}/.config/Code"); 
  } else if (IsRunningOnMac()) {
    app_home = Directory($"{home}/Library/Application Support/Code");
  } else {
    return;
  }
  dotfile("vscode/settings.json", app_home, false);
});

RunTarget(target);
