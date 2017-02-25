#!/usr/bin/env node

var spawn = require('child_process').spawn;
var process = require("process");

var file = __dirname + "/CodeConsole/bin/Debug/CodeConsole.exe";

if(process.platform === "win32") {
    spawn(file, [], { stdio: "inherit"});
} else {
    spawn("mono", [file] , {stdio: "inherit"});
}