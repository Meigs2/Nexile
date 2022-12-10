## If the first parameter is "init" then run "dotnet user-secrets init --project Nexile.Infrastructure.Tests.Integration"

if [ "$1" == "init" ]; then
  dotnet user-secrets init --project Nexile.Infrastructure.Tests.Integration
elif [ "$1" == "set" ]; then
  if [ "$2" == "sessionid" ]; then
    dotnet user-secrets set --project Nexile.Infrastructure.Tests.Integration "PathOfExileApiSessionId" "$2"
  fi
fi