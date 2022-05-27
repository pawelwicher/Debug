#r @"..\packages\MoonSharp.2.0.0.0\lib\netstandard1.6\MoonSharp.Interpreter.dll"

open MoonSharp.Interpreter

let scriptCode = """
function add (a, b)
    return a + b
end"""

let script = new Script();

script.DoString(scriptCode)

let luaFactFunction = script.Globals.Get("add");

let res = script.Call(luaFactFunction, 2, 3);

let n = res.Number