// Keep these lines for a best effort IntelliSense of Visual Studio 2017 and higher.
/// <reference path="../../Packages/Beckhoff.TwinCAT.HMI.Framework.12.746.0/runtimes/native1.12-tchmi/TcHmi.d.ts" />
(function (/** @type {globalThis.TcHmi} */ TcHmi) {
    var Functions;
    (function (/** @type {globalThis.TcHmi.Functions} */ Functions) {
        var TcHmiProject1;
        (function (TcHmiProject1) {
            function MyFunction(par1) {
                return "hejsan"
            }
            TcHmiProject1.MyFunction = MyFunction;
        })(TcHmiProject1 = Functions.TcHmiProject1 || (Functions.TcHmiProject1 = {}));
        Functions.registerFunctionEx('MyFunction', 'TcHmi.Functions.TcHmiProject1', TcHmiProject1.MyFunction);
    })(Functions = TcHmi.Functions || (TcHmi.Functions = {}));
})(TcHmi);