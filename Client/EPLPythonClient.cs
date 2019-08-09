using System;
using Newtonsoft.Json;
using Python.Runtime;

public class EPLPythonClient {
    public MyTeam MyTeam(int teamId) {
        var result = ShellHelper.PythonScript("getLoggedInData.py");
        return null;
    }
}