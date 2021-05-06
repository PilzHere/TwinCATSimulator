// Keep these lines for a best effort IntelliSense of Visual Studio 2017 and higher.
/// <reference path="../../Packages/Beckhoff.TwinCAT.HMI.Framework.12.746.0/runtimes/native1.12-tchmi/TcHmi.d.ts" />
(function (/** @type {globalThis.TcHmi} */ TcHmi) {
    var Functions;
    (function (/** @type {globalThis.TcHmi.Functions} */ Functions) {
        var TcHmiProject1;
        (function (TcHmiProject1) {
            function MyFunction2(par1) {
                var arrayn = TcHmi.Server.readSymbol("picture.Task.Outputs.picture")
                //var arrayn = Array.from(par1)
                //var text

                //let fruits = par1

                //var arrayn = Array.from(par1)
                //console.log(arrayn)
                //var arrayn = array
                //console.log(arrayn)
                var text = ""

                var i
                for (i = 0; i < arrayn.length; i++) {
                    //console.log(String.fromCharCode(arrayn[i]))
                    text += String.fromCharCode(arrayn[i])
                }

                //console.log(text)

                return text
            }
            TcHmiProject1.MyFunction2 = MyFunction2;
        })(TcHmiProject1 = Functions.TcHmiProject1 || (Functions.TcHmiProject1 = {}));
        Functions.registerFunctionEx('MyFunction2', 'TcHmi.Functions.TcHmiProject1', TcHmiProject1.MyFunction2);
    })(Functions = TcHmi.Functions || (TcHmi.Functions = {}));
})(TcHmi);