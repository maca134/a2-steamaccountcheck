# a2-steamaccountcheck
An ARMA extension to check player id are shared or not.

## Install
Place the DLL and INI in a server-side @mod folder (@dayz_server). Then add the following line to some file/function that runs when a player logs in. (Eg. server_playerLogin.sqf).
```sqf
"SteamAccountCheck" callExtension format["%1%3%2%3", _playerID, _playerName, toString[10]];
```

Then edit the settings.ini with the correct details. You can get a Steam API key from [here](https://steamcommunity.com/dev/apikey). The app id for ARMA 2 is 33930.

Once you start the server, when the callExtension is called, an async task is started to check the playerid. The performance impact is minimal as all the checking happens away from ARMA.

### HTTP Log

The http log sends the following POST params.
```
current_steamid => Players Steam ID
parent_steamid => The owners (original) Steam account ID
log => A nice log message
server => Server name from the settings.ini
type => A constant log type
```